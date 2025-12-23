using wisheo_backend_v2.Models;
using Microsoft.EntityFrameworkCore;

namespace wisheo_backend_v2.Repositories
{
    public class SocialRepository(AppDbContext context)
    {
        private readonly AppDbContext _context = context;
        public async Task AddFollow(Follow follow)
        {
            _context.Follows.Add(follow);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFollow(Guid followerId, Guid followedId)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
            if (follow != null)
            {
                _context.Follows.Remove(follow);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<User>> GetFollowers(Guid userId)
        {
            return await _context.Follows
                .Where(f => f.FollowedId == userId)
                .Select(f => f.Follower)
                .ToListAsync();
        }

        public async Task<List<User>> GetFollowing(Guid userId)
        {
            return await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.Followed)
                .ToListAsync();
        }
    }
}