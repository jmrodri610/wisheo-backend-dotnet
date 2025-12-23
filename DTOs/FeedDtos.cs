namespace wisheo_backend_v2.DTOs;

public enum FeedItemType { Suggestion, UserPost }

public record FeedItemDto(
    FeedItemType Type,
    string Title,
    string? Description,
    string? ImageUrl,
    string? ProductUrl,
    decimal? Price,
    string? Username,
    DateTime CreatedAt
);