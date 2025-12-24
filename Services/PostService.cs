using wisheo_backend_v2.Models;
using wisheo_backend_v2.Repositories;
using wisheo_backend_v2.DTOs;

namespace wisheo_backend_v2.Services;

public class PostService(PostRepository postRepository)
{
    private readonly PostRepository _postRepository = postRepository;

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

        var postExists = await _postRepository.PostExists(postId);
        if (!postExists) return null;

        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Text = dto.Text,
            CreatedAt = DateTime.UtcNow
        };

        await _postRepository.AddComment(comment);
        return comment;
    }
}