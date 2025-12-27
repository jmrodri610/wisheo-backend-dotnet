namespace wisheo_backend_v2.DTOs;
 

public record UserProfileDto(
    Guid UserId,
    string Username,
    int FollowersCount,
    int FollowingCount,
    bool IsFollowing,
    List<WishlistDto> PublicWishlists
);