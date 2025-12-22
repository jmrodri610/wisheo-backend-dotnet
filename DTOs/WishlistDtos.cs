namespace wisheo_backend_v2.DTOs;

public record CreateWishlistDto(string Title, string? Description, bool IsPublic);

public record WishlistResponseDto(int Id, string Title, string? Description, bool IsPublic, List<WishItemResponseDto> Items);