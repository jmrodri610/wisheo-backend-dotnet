using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Models;
using wisheo_backend_v2.Repositories;

namespace wisheo_backend_v2.Services;

public class WishlistService(WishlistRepository repository)
{
    private readonly WishlistRepository _repository = repository;

    public async Task<int> CreateWishlist(CreateWishlistDto dto, int userId)
    {
        var wishlist = new Wishlist
        {
            Title = dto.Title,
            Description = dto.Description,
            IsPublic = dto.IsPublic,
            UserId = userId
        };

        await _repository.AddWishlist(wishlist);
        return wishlist.Id;
    }

    public async Task<List<WishlistResponseDto>> GetUserWishlists(int userId)
    {
        var lists = await _repository.GetWishlistsByUserId(userId);

        return [.. lists.Select(w => new WishlistResponseDto(
            w.Id, w.Title, w.Description, w.IsPublic,
            [.. w.Items.Select(i => new WishItemResponseDto(
                i.Id, i.Name, i.Description, i.ImageUrl, i.ProductUrl, i.Price, i.IsPurchased
            ))]
        ))];
    }

    public async Task<int?> AddItemToWishlist(int wishlistId, int userId, CreateWishItemDto dto)
    {
        var wishlist = await _repository.GetByIdAndUserId(wishlistId, userId);
        if (wishlist == null) return null;

        var item = new WishItem
        {
            Name = dto.Name,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            ProductUrl = dto.ProductUrl,
            Price = dto.Price,
            WishlistId = wishlistId
        };

        await _repository.AddWishItem(item);
        return item.Id;
    }

    public async Task<bool> UpdateItem(int itemId, int userId, UpdateWishItemDto dto)
    {
        var item = await _repository.GetItemById(itemId);


        if (item == null || item.Wishlist.UserId != userId) return false;

        if (dto.Name != null) item.Name = dto.Name;
        item.Description = dto.Description;
        item.ProductUrl = dto.ProductUrl;
        item.ImageUrl = dto.ImageUrl;
        item.Price = dto.Price;
        if (dto.Currency != null) item.Currency = dto.Currency;

        await _repository.UpdateItem(item);
        return true;
    }

    public async Task<bool> TogglePurchased(int itemId)
    {
        var item = await _repository.GetItemById(itemId);
        if (item == null) return false;

        item.IsPurchased = !item.IsPurchased;
        await _repository.UpdateItem(item);
        return true;
    }

    public async Task<bool> DeleteItem(int itemId, int userId)
    {
        var item = await _repository.GetItemById(itemId);
        if (item == null || item.Wishlist.UserId != userId) return false;

        await _repository.DeleteItem(item);
        return true;
    }

    public async Task<bool> AddFromFeed(int userId, AddFromFeedDto dto)
{

    var wishlist = await _repository.GetByIdAndUserId(dto.WishlistId, userId);
    if (wishlist == null) return false;

    var newItem = new WishItem
    {
        Name = dto.Name,
        Description = dto.Description,
        ProductUrl = dto.ProductUrl,
        ImageUrl = dto.ImageUrl,
        Price = dto.Price,
        WishlistId = dto.WishlistId,
        CreatedAt = DateTime.UtcNow
    };

    await _repository.AddWishItem(newItem);
    return true;
}
}