namespace wisheo_backend_v2.DTOs;

public enum FeedItemType { UserPost, WishItemActivity, Suggestion }

public class FeedItemDto(FeedItemType type, DateTime createdAt, object data)
{
    public FeedItemType Type { get; set; } = type;
    public DateTime CreatedAt { get; set; } = createdAt;
    public object Data { get; set; } = data;
}