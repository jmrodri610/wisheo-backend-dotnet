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
    public async Task UpdateWishlist(Wishlist wishlist)
    {
        _context.Wishlists.Update(wishlist);
        await _context.SaveChangesAsync();
    }

    public async Task<Wishlist?> GetWishlistById(Guid id)
{
    return await _context.Wishlists.FindAsync(id);
}

    public async Task DeleteWishlist(Wishlist wishlist)
    {
        _context.Wishlists.Remove(wishlist);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Wishlist>> GetWishlistsByUserId(Guid userId)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Collaborators)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<Wishlist>> GetWishlistsAccessibleByUser(Guid userId)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
            .Include(w => w.Collaborators)
            .Where(w => w.UserId == userId
                || w.Collaborators.Any(c => c.UserId == userId))
            .ToListAsync();
    }

    public async Task<Wishlist?> GetByIdAndUserId(Guid wishlistId, Guid userId)
    {
        return await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == wishlistId && w.UserId == userId);
    }

    public async Task<bool> CanEdit(Guid wishlistId, Guid userId)
    {
        return await _context.Wishlists.AnyAsync(w =>
            w.Id == wishlistId && (
                w.UserId == userId ||
                w.Collaborators.Any(c =>
                    c.UserId == userId && c.Role == CollaboratorRole.Editor)
            ));
    }

    public async Task<List<WishlistCollaborator>> GetCollaborators(Guid wishlistId)
    {
        return await _context.WishlistCollaborators
            .Include(c => c.User)
            .Where(c => c.WishlistId == wishlistId)
            .ToListAsync();
    }

    public async Task<WishlistCollaborator?> GetCollaborator(Guid wishlistId, Guid userId)
    {
        return await _context.WishlistCollaborators
            .FirstOrDefaultAsync(c => c.WishlistId == wishlistId && c.UserId == userId);
    }

    public async Task AddCollaborator(WishlistCollaborator collaborator)
    {
        _context.WishlistCollaborators.Add(collaborator);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCollaborator(WishlistCollaborator collaborator)
    {
        _context.WishlistCollaborators.Update(collaborator);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveCollaborator(WishlistCollaborator collaborator)
    {
        _context.WishlistCollaborators.Remove(collaborator);
        await _context.SaveChangesAsync();
    }

    public async Task AddWishItem(WishItem item)
    {
        _context.WishItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task<WishItem?> GetItemById(Guid itemId)
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

    public async Task<List<Wishlist>> GetWishlistsByUser(Guid userId)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
            .Where(w => w.UserId == userId)
            .ToListAsync();
    }

    public async Task<bool> SlugExists(string slug)
    {
        return await _context.Wishlists.AnyAsync(w => w.PublicSlug == slug);
    }

    public async Task<Wishlist?> GetPublicWishlistBySlug(string slug)
    {
        return await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.Items)
                .ThenInclude(i => i.Reservations)
            .FirstOrDefaultAsync(w => w.PublicSlug == slug && w.IsPublic);
    }

    public async Task AddReservation(Reservation reservation)
    {
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<Reservation?> GetReservationByToken(string cancelToken)
    {
        return await _context.Reservations
            .FirstOrDefaultAsync(r => r.CancelToken == cancelToken);
    }

    public async Task RemoveReservation(Reservation reservation)
    {
        _context.Reservations.Remove(reservation);
        await _context.SaveChangesAsync();
    }
}