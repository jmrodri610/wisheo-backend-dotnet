namespace wisheo_backend_v2.DTOs;

public record PublicWishlistResponseDto(
    string Slug,
    string Title,
    string? Description,
    string? Emoji,
    string OwnerName,
    string OwnerUsername,
    List<PublicWishItemDto> Items
);

public record PublicWishItemDto(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    string? ProductUrl,
    decimal? Price,
    bool IsReserved
);

public record ReserveItemDto(
    string GuestName,
    string? GuestEmail
);

public record ReserveItemResponseDto(
    Guid ReservationId,
    string CancelToken
);
