using wisheo_backend_v2.Models;
using Microsoft.EntityFrameworkCore;


namespace wisheo_backend_v2.Repositories
{
    public class PostRepository(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public async Task<List<Post>> GetFollowedUsersPosts(Guid userId)
        {
            var followedIds = await _context.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowedId)
                .ToListAsync();

            followedIds.Add(userId);

            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Where(p => followedIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task AddPost(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task AddComment(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> PostExists(Guid postId)
        {
            return await _context.Posts.AnyAsync(p => p.Id == postId);
        }
    }

    
}