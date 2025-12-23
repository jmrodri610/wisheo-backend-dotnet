namespace wisheo_backend_v2.DTOs;

public record CreateWishItemDto(
    string Name, 
    string? Description, 
    string? ProductUrl, 
    string? ImageUrl, 
    decimal? Price
);

public record WishItemResponseDto(
    int Id, 
    string Name, 
    string? Description, 
    string? ImageUrl, 
    string? ProductUrl, 
    decimal? Price, 
    bool IsPurchased
);

public record UpdateWishItemDto(
    string? Name, 
    string? Description, 
    string? ProductUrl, 
    string? ImageUrl, 
    decimal? Price,
    string? Currency
);

public record AddFromFeedDto(
    int WishlistId,
    string Name,
    string? Description,
    string? ProductUrl,
    string? ImageUrl,
    decimal? Price
);