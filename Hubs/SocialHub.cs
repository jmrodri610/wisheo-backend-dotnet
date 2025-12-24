using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace wisheo_backend_v2.Hubs;

[Authorize]
public class SocialHub : Hub
{
    public async Task JoinSocialGroup(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, userId);
    }
}