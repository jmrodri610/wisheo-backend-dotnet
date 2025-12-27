using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Repositories;

namespace wisheo_backend_v2.Services;

public class ProfileService(
    UserRepository userRepository,
    WishlistRepository wishlistRepository,
    SocialRepository socialRepository)
{
    public async Task<UserProfileDto?> GetPublicProfile(Guid targetUserId, Guid? currentUserId)
    {
        var user = await userRepository.GetUserById(targetUserId);
        if (user == null) return null;

        var followers = await socialRepository.GetFollowers(targetUserId);
        var following = await socialRepository.GetFollowing(targetUserId);
        
        var followersCount = followers.Count;
        var followingCount= following.Count;

        bool isFollowing = false;
        if (currentUserId.HasValue)
        {
            isFollowing = await socialRepository.IsFollowing(currentUserId.Value, targetUserId);
        }

        var allWishlists = await wishlistRepository.GetWishlistsByUser(targetUserId);
        var publicWishlists = allWishlists
            .Where(w => w.IsPublic)
            .Select(w => new WishlistDto(w.Id, w.Title, w.Items.Count))
            .ToList();

        return new UserProfileDto(
            user.Id,
            user.Username,
            followersCount,
            followingCount,
            isFollowing,
            publicWishlists
        );
    }
}