using wisheo_backend_v2.Models;
using wisheo_backend_v2.Repositories;
using wisheo_backend_v2.DTOs;

namespace wisheo_backend_v2.Services;

public class PostService(
    PostRepository postRepository,
    UserRepository userRepository,
    NotificationService notificationService)
{
    private readonly PostRepository _postRepository = postRepository;
    private readonly UserRepository _userRepository = userRepository;
    private readonly NotificationService _notificationService = notificationService;

    public async Task<Post> CreatePost(CreatePostDto dto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new ArgumentException("El contenido del post no puede estar vacío.");

        var post = new Post
        {
            Content = dto.Content,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _postRepository.AddPost(post);
        return post;
    }

    public async Task<Comment?> AddComment(Guid postId, CreateCommentDto dto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Text))
            throw new ArgumentException("El comentario no puede estar vacío.");

        var post = await _postRepository.GetPostById(postId);
        if (post == null) return null;

        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Text = dto.Text,
            CreatedAt = DateTime.UtcNow
        };

        await _postRepository.AddComment(comment);

        if (post.UserId != userId)
        {
            var commenter = await _userRepository.GetUserById(userId);
            var who = commenter != null
                ? $"{commenter.Name} {commenter.Surname}".Trim()
                : "Someone";
            _ = _notificationService.SendToUser(
                post.UserId,
                "New comment",
                "$who commented on your post.".Replace("$who", who),
                new Dictionary<string, string>
                {
                    ["type"] = "post_comment",
                    ["postId"] = postId.ToString()
                });
        }

        return comment;
    }
}