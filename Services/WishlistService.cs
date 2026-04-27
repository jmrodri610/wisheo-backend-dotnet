using System.Security.Cryptography;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Models;
using wisheo_backend_v2.Repositories;

namespace wisheo_backend_v2.Services;

public class WishlistService(
    WishlistRepository repository,
    NotificationService notificationService)
{
    private readonly WishlistRepository _repository = repository;
    private readonly NotificationService _notificationService = notificationService;

    public async Task<Guid> CreateWishlist(CreateWishlistDto dto, Guid userId)
    {
        var wishlist = new Wishlist
        {
            Title = dto.Title,
            Description = dto.Description,
            Emoji = dto.Emoji,
            IsPublic = dto.IsPublic,
            UserId = userId,
            PublicSlug = dto.IsPublic ? await GenerateUniqueSlug() : null
        };

        await _repository.AddWishlist(wishlist);
        return wishlist.Id;
    }

    public async Task<bool> UpdateWishlist(Guid wishlistId, Guid userId, CreateWishlistDto dto)
    {
        var wishlist = await _repository.GetWishlistById(wishlistId);

        if (wishlist == null || wishlist.UserId != userId) return false;

        wishlist.Title = dto.Title;
        wishlist.Description = dto.Description;
        wishlist.Emoji = dto.Emoji;
        wishlist.IsPublic = dto.IsPublic;

        if (dto.IsPublic && string.IsNullOrEmpty(wishlist.PublicSlug))
        {
            wishlist.PublicSlug = await GenerateUniqueSlug();
        }

        await _repository.UpdateWishlist(wishlist);
        return true;
    }

    private async Task<string> GenerateUniqueSlug()
    {
        for (int i = 0; i < 5; i++)
        {
            var slug = GenerateRandomSlug();
            if (!await _repository.SlugExists(slug)) return slug;
        }
        throw new Exception("Could not generate a unique slug.");
    }

    private static string GenerateRandomSlug()
    {
        const string chars = "abcdefghijkmnpqrstuvwxyz23456789";
        var bytes = RandomNumberGenerator.GetBytes(10);
        var slug = new char[10];
        for (int i = 0; i < 10; i++)
        {
            slug[i] = chars[bytes[i] % chars.Length];
        }
        return new string(slug);
    }

    public async Task<string?> EnsureShareSlug(Guid wishlistId, Guid userId)
    {
        var wishlist = await _repository.GetWishlistById(wishlistId);
        if (wishlist == null || wishlist.UserId != userId) return null;

        if (!wishlist.IsPublic)
        {
            wishlist.IsPublic = true;
        }

        if (string.IsNullOrEmpty(wishlist.PublicSlug))
        {
            wishlist.PublicSlug = await GenerateUniqueSlug();
        }

        await _repository.UpdateWishlist(wishlist);
        return wishlist.PublicSlug;
    }

    public async Task<bool> DeleteWishlist(Guid wishlistId, Guid userId)
    {
        var wishlist = await _repository.GetWishlistById(wishlistId);

        if (wishlist == null || wishlist.UserId != userId) return false;

        await _repository.DeleteWishlist(wishlist);
        return true;
    }

    public async Task<List<WishlistResponseDto>> GetUserWishlists(Guid userId)
    {
        var lists = await _repository.GetWishlistsAccessibleByUser(userId);

        return [.. lists.Select(w => new WishlistResponseDto(
            w.Id,
            w.Title,
            w.Description,
            w.Emoji,
            w.IsPublic,
            w.PublicSlug,
            [.. w.Items.Select(i => new WishItemResponseDto(
                i.Id, i.Name, i.Description, i.ImageUrl, i.ProductUrl, i.Price, i.IsPurchased
            ))],
            w.UserId == userId,
            w.Collaborators?.Count ?? 0
        ))];
    }

    public async Task<PublicWishlistResponseDto?> GetPublicWishlist(string slug)
    {
        var wishlist = await _repository.GetPublicWishlistBySlug(slug);
        if (wishlist == null) return null;

        return new PublicWishlistResponseDto(
            wishlist.PublicSlug ?? string.Empty,
            wishlist.Title,
            wishlist.Description,
            wishlist.Emoji,
            $"{wishlist.User.Name} {wishlist.User.Surname}".Trim(),
            wishlist.User.Username,
            [.. wishlist.Items.Select(i => new PublicWishItemDto(
                i.Id,
                i.Name,
                i.Description,
                i.ImageUrl,
                i.ProductUrl,
                i.Price,
                i.Reservations.Count > 0
            ))]
        );
    }

    public async Task<ReserveItemResponseDto?> ReserveItem(string slug, Guid itemId, ReserveItemDto dto, Guid? userId)
    {
        var wishlist = await _repository.GetPublicWishlistBySlug(slug);
        if (wishlist == null) return null;

        var item = wishlist.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null) return null;

        if (item.Reservations.Count > 0) return null;

        var token = GenerateCancelToken();
        var reservation = new Reservation
        {
            WishItemId = itemId,
            GuestName = string.IsNullOrWhiteSpace(dto.GuestName) ? "Anonymous" : dto.GuestName.Trim(),
            GuestEmail = string.IsNullOrWhiteSpace(dto.GuestEmail) ? null : dto.GuestEmail.Trim(),
            UserId = userId,
            CancelToken = token
        };

        await _repository.AddReservation(reservation);

        // Notify the wishlist owner without revealing who reserved.
        _ = _notificationService.SendToUser(
            wishlist.UserId,
            "Item reserved",
            $"Someone reserved \"{item.Name}\" from your list \"{wishlist.Title}\".",
            new Dictionary<string, string>
            {
                ["type"] = "item_reserved",
                ["wishlistId"] = wishlist.Id.ToString()
            });

        return new ReserveItemResponseDto(reservation.Id, token);
    }

    public async Task<bool> CancelReservation(string slug, Guid itemId, string cancelToken)
    {
        var reservation = await _repository.GetReservationByToken(cancelToken);
        if (reservation == null || reservation.WishItemId != itemId) return false;

        await _repository.RemoveReservation(reservation);
        return true;
    }

    private static string GenerateCancelToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(24);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    public async Task<Guid?> AddItemToWishlist(Guid wishlistId, Guid userId, CreateWishItemDto dto)
    {
        if (!await _repository.CanEdit(wishlistId, userId)) return null;

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

    public async Task<bool> UpdateItem(Guid itemId, Guid userId, UpdateWishItemDto dto)
    {
        var item = await _repository.GetItemById(itemId);
        if (item == null) return false;
        if (!await _repository.CanEdit(item.WishlistId, userId)) return false;

        if (dto.Name != null) item.Name = dto.Name;
        item.Description = dto.Description;
        item.ProductUrl = dto.ProductUrl;
        item.ImageUrl = dto.ImageUrl;
        item.Price = dto.Price;
        if (dto.Currency != null) item.Currency = dto.Currency;

        await _repository.UpdateItem(item);
        return true;
    }

    public async Task<bool> TogglePurchased(Guid itemId)
    {
        var item = await _repository.GetItemById(itemId);
        if (item == null) return false;

        item.IsPurchased = !item.IsPurchased;
        await _repository.UpdateItem(item);
        return true;
    }

    public async Task<bool> DeleteItem(Guid itemId, Guid userId)
    {
        var item = await _repository.GetItemById(itemId);
        if (item == null) return false;
        if (!await _repository.CanEdit(item.WishlistId, userId)) return false;

        await _repository.DeleteItem(item);
        return true;
    }

    public async Task<bool> AddFromFeed(Guid userId, AddFromFeedDto dto)
    {
        if (!await _repository.CanEdit(dto.WishlistId, userId)) return false;

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

    // ── Collaborators ──────────────────────────────────────────────────────────

    public async Task<List<CollaboratorResponseDto>?> GetCollaborators(Guid wishlistId, Guid userId)
    {
        var wishlist = await _repository.GetWishlistById(wishlistId);
        if (wishlist == null) return null;

        // Owner or any collaborator can list collaborators.
        if (wishlist.UserId != userId)
        {
            var existing = await _repository.GetCollaborator(wishlistId, userId);
            if (existing == null) return null;
        }

        var list = await _repository.GetCollaborators(wishlistId);
        return [.. list.Select(c => new CollaboratorResponseDto(
            c.UserId,
            c.User.Username,
            c.User.Name,
            c.User.Surname,
            c.Role.ToString(),
            c.InvitedAt,
            c.AcceptedAt
        ))];
    }

    public async Task<(bool ok, string? error)> AddCollaborator(
        Guid wishlistId,
        Guid ownerId,
        AddCollaboratorDto dto,
        UserRepository userRepository)
    {
        var wishlist = await _repository.GetWishlistById(wishlistId);
        if (wishlist == null) return (false, "Lista no encontrada");
        if (wishlist.UserId != ownerId)
            return (false, "Solo el propietario puede invitar a colaboradores");

        var user = await userRepository.GetByUsername(dto.Username);
        if (user == null) return (false, "Usuario no encontrado");
        if (user.Id == ownerId) return (false, "Ya eres el propietario de esta lista");

        var existing = await _repository.GetCollaborator(wishlistId, user.Id);
        if (existing != null) return (false, "El usuario ya es colaborador");

        if (!Enum.TryParse<CollaboratorRole>(dto.Role, true, out var role))
            role = CollaboratorRole.Editor;

        var collaborator = new WishlistCollaborator
        {
            WishlistId = wishlistId,
            UserId = user.Id,
            Role = role,
            InvitedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow,
        };

        await _repository.AddCollaborator(collaborator);

        _ = _notificationService.SendToUser(
            user.Id,
            "Added as collaborator",
            $"You can now contribute to the wishlist \"{wishlist.Title}\".",
            new Dictionary<string, string>
            {
                ["type"] = "collaborator_added",
                ["wishlistId"] = wishlistId.ToString()
            });

        return (true, null);
    }

    public async Task<bool> RemoveCollaborator(Guid wishlistId, Guid ownerId, Guid collaboratorUserId)
    {
        var wishlist = await _repository.GetWishlistById(wishlistId);
        if (wishlist == null || wishlist.UserId != ownerId) return false;

        var collaborator = await _repository.GetCollaborator(wishlistId, collaboratorUserId);
        if (collaborator == null) return false;

        await _repository.RemoveCollaborator(collaborator);
        return true;
    }
}