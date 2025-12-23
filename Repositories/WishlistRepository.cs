using Microsoft.EntityFrameworkCore;
using wisheo_backend_v2.Models;

namespace wisheo_backend_v2.Repositories;

public class WishlistRepository(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task AddWishlist(Wishlist wishlist)
    {
        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Wishlist>> GetWishlistsByUserId(int userId)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }

    public async Task<Wishlist?> GetByIdAndUserId(int wishlistId, int userId)
    {
        return await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId);
    }

    public async Task AddWishItem(WishItem item)
    {
        _context.WishItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task<WishItem?> GetItemById(int itemId)
    {
        return await _context.WishItems
            .Include(i => i.Wishlist)
            .FirstOrDefaultAsync(i => i.Id == itemId);
    }

    public async Task UpdateItem(WishItem item)
    {
        _context.WishItems.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteItem(WishItem item)
    {
        _context.WishItems.Remove(item);
        await _context.SaveChangesAsync();
    }
}