using Microsoft.EntityFrameworkCore;
using wisheo_backend_v2.Models;

namespace wisheo_backend_v2.Repositories;

public class FeedRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<List<WishItem>> GetRecentActivity(int limit = 10)
    {
        return await _context.WishItems
            .Include(i => i.Wishlist)
            .ThenInclude(w => w.User)
            .Where(i => i.Wishlist.IsPublic)
            .OrderByDescending(i => i.CreatedAt)
            .Take(limit)
            .ToListAsync();
    }
}