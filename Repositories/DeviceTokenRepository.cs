using Microsoft.EntityFrameworkCore;
using wisheo_backend_v2.Models;

namespace wisheo_backend_v2.Repositories;

public class DeviceTokenRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<DeviceToken?> GetByToken(string token)
    {
        return await _context.DeviceTokens
            .FirstOrDefaultAsync(d => d.Token == token);
    }

    public async Task<List<string>> GetTokensForUser(Guid userId)
    {
        return await _context.DeviceTokens
            .Where(d => d.UserId == userId)
            .Select(d => d.Token)
            .ToListAsync();
    }

    public async Task Upsert(Guid userId, string token, string platform)
    {
        var existing = await _context.DeviceTokens
            .FirstOrDefaultAsync(d => d.Token == token);

        if (existing != null)
        {
            existing.UserId = userId;
            existing.Platform = platform;
            existing.LastUsed = DateTime.UtcNow;
            _context.DeviceTokens.Update(existing);
        }
        else
        {
            _context.DeviceTokens.Add(new DeviceToken
            {
                UserId = userId,
                Token = token,
                Platform = platform
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveTokens(IEnumerable<string> tokens)
    {
        var entities = await _context.DeviceTokens
            .Where(d => tokens.Contains(d.Token))
            .ToListAsync();
        if (entities.Count == 0) return;
        _context.DeviceTokens.RemoveRange(entities);
        await _context.SaveChangesAsync();
    }
}
