using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Models;
using wisheo_backend_v2.Repositories;

namespace wisheo_backend_v2.Services;

public class SocialService(SocialRepository socialRepository)
{
    private readonly SocialRepository _socialRepository = socialRepository;

    public async Task<bool> FollowUser(Guid followerId, Guid followedId)
    {
        if (followerId == followedId) return false;

        var follow = new Follow
        {
            FollowerId = followerId,
            FollowedId = followedId,
            CreatedAt = DateTime.UtcNow
        };

        await _socialRepository.AddFollow(follow);
        return true;
    }

    public async Task<bool> UnfollowUser(Guid followerId, Guid followedId)
    {
        await _socialRepository.RemoveFollow(followerId, followedId);
        return true;
    }

    public async Task<SocialListDto> GetFollowersList(Guid userId)
    {
        var users = await _socialRepository.GetFollowers(userId);
        var summary = users.Select(u => new UserSummaryDto(u.Id, u.Name, u.Surname));
        return new SocialListDto(users.Count, summary);
    }

    public async Task<SocialListDto> GetFollowingList(Guid userId)
    {
        var users = await _socialRepository.GetFollowing(userId);
        var summary = users.Select(u => new UserSummaryDto(u.Id, u.Name, u.Surname));
        return new SocialListDto(users.Count, summary);
    }
}