using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Repositories;

namespace wisheo_backend_v2.Services;

public class FeedService(FeedRepository repository)
{
    private readonly FeedRepository _repository = repository;

    public async Task<List<FeedItemDto>> GetAnonymousFeed()
    {
        var feed = new List<FeedItemDto>();

        var globalItems = await _repository.GetRecentActivity(limit: 10);
        feed.AddRange(MapToFeedDto(globalItems));

        feed.AddRange(GetHardcodedSuggestions());

        return feed.OrderByDescending(f => f.CreatedAt).ToList();
    }

    public async Task<List<FeedItemDto>> GetFullFeed(int userId)
    {
        return await GetAnonymousFeed(); 
    }
    
    private static List<FeedItemDto> MapToFeedDto(List<Models.WishItem> items)
    {
        return items.Select(i => new FeedItemDto(
            FeedItemType.UserPost,
            i.Name,
            i.Description,
            i.ImageUrl,
            i.ProductUrl,
            i.Price,
            i.Wishlist.User.Username,
            i.CreatedAt
        )).ToList();
    }

    private static List<FeedItemDto> GetHardcodedSuggestions()
    {
        return new List<FeedItemDto>
        {
            new (FeedItemType.Suggestion, "Sony WH-1000XM5", "Cancelación de ruido top.", "https://link-a-imagen.com/sony.jpg", "https://amazon.es/sony", 329.00m, "Wisheo Bot", DateTime.UtcNow),
            new (FeedItemType.Suggestion, "Kindle Paperwhite", "Perfecto para leer en verano.", "https://link-a-imagen.com/kindle.jpg", "https://amazon.es/kindle", 149.99m, "Wisheo Bot", DateTime.UtcNow)
        };
    }
}