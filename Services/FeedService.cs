using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Repositories;

namespace wisheo_backend_v2.Services;

public class FeedService(FeedRepository repository, PostRepository postRepository)
{
    private readonly FeedRepository _repository = repository;
    private readonly PostRepository _postRepository = postRepository;

    public async Task<List<FeedItemDto>> GetAnonymousFeed()
    {
        var feed = new List<FeedItemDto>();

        var globalItems = await _repository.GetRecentActivity(limit: 10);
        feed.AddRange(MapToFeedDto(globalItems));

        feed.AddRange(GetHardcodedSuggestions());

        return [.. feed.OrderByDescending(f => f.CreatedAt)];
    }

    public async Task<List<FeedItemDto>> GetFullFeed(Guid userId)
    {
        var feed = new List<FeedItemDto>();

        var globalItems = await _repository.GetRecentActivity(limit: 10);
        feed.AddRange(globalItems.Select(i => new FeedItemDto(
            FeedItemType.WishItemActivity, 
            i.CreatedAt, 
            i
        )));

        var suggestions = GetHardcodedSuggestions();
        feed.AddRange(suggestions.Select(s => new FeedItemDto(
            FeedItemType.Suggestion, 
            s.CreatedAt, 
            s
        )));

        var socialPosts = await _postRepository.GetFollowedUsersPosts(userId);
        feed.AddRange(socialPosts.Select(p => new FeedItemDto(
            FeedItemType.UserPost, 
            p.CreatedAt, 
            p 
        )));

        return feed.OrderByDescending(f => f.CreatedAt).ToList();
    }
    
    private static List<FeedItemDto> MapToFeedDto(List<Models.WishItem> items)
    {
        return [.. items.Select(i => new FeedItemDto(
            FeedItemType.WishItemActivity,
            i.CreatedAt, 
            new {
                i.Name,
                i.Description,
                i.ImageUrl,
                i.ProductUrl,
                i.Price,
                Username = i.Wishlist.User.Username ?? "Usuario"
            }
        ))];
    }

    private static List<FeedItemDto> GetHardcodedSuggestions()
    {
        var now = DateTime.UtcNow;
        return
        [
            new (FeedItemType.Suggestion, now, new {
                Title = "Sony WH-1000XM5",
                Description = "Cancelación de ruido top.",
                ImageUrl = "https://link-a-imagen.com/sony.jpg",
                ProductUrl = "https://amazon.es/sony",
                Price = 329.00m,
                AuthorName = "Wisheo Bot"
            }),
            new (FeedItemType.Suggestion, now.AddMinutes(-5), new {
                Title = "Kindle Paperwhite",
                Description = "Perfecto para leer en verano.",
                ImageUrl = "https://link-a-imagen.com/kindle.jpg",
                ProductUrl = "https://amazon.es/kindle",
                Price = 149.99m,
                AuthorName = "Wisheo Bot"
            })
        ];
    }
}