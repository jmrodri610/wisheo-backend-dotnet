using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using wisheo_backend_v2.Repositories;

namespace wisheo_backend_v2.Services;

public class NotificationService(DeviceTokenRepository tokenRepository)
{
    private readonly DeviceTokenRepository _tokenRepository = tokenRepository;

    public async Task SendToUser(
        Guid userId,
        string title,
        string body,
        IDictionary<string, string>? data = null)
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            // Firebase not initialised (e.g. in dev without service account); skip silently.
            return;
        }

        var tokens = await _tokenRepository.GetTokensForUser(userId);
        if (tokens.Count == 0) return;

        var messaging = FirebaseMessaging.DefaultInstance;
        if (messaging == null) return;

        var invalidTokens = new List<string>();

        foreach (var token in tokens)
        {
            try
            {
                var message = new Message
                {
                    Token = token,
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body,
                    },
                    Data = data?.ToDictionary(k => k.Key, v => v.Value),
                };
                await messaging.SendAsync(message);
            }
            catch (FirebaseMessagingException ex) when (
                ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
            {
                invalidTokens.Add(token);
            }
            catch (Exception)
            {
                // Swallow other errors — a single failure shouldn't break the request.
            }
        }

        if (invalidTokens.Count > 0)
        {
            await _tokenRepository.RemoveTokens(invalidTokens);
        }
    }
}
