namespace wisheo_backend_v2.DTOs;

public record CreateWishlistDto(string Title, string? Description, bool IsPublic);

public record WishlistResponseDto(Guid Id, string Title, string? Description, bool IsPublic, List<WishItemResponseDto> Items);

public record WishlistDto (Guid Id, string Title, int ItemsCount);