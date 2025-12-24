using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Hubs;
using wisheo_backend_v2.Services;

namespace wisheo_backend_v2.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PostsController(
    PostService postService, 
    SocialService socialService,
    IHubContext<SocialHub> hubContext) : BaseController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreatePostDto dto)
    {
        var post = await postService.CreatePost(dto, UserId);

        var followersData = await socialService.GetFollowersList(UserId);
        var followerIds = followersData.Users.Select(u => u.Id.ToString()).ToList();

        await hubContext.Clients.Groups(followerIds).SendAsync("ReceiveNewPost", new {
            post.Id,
            post.Content,
            post.CreatedAt,
            AuthorName = User.Identity?.Name, 
            AuthorId = UserId
        });

        return Ok(new { message = "Post publicado", id = post.Id });
    }

    [HttpPost("{postId}/comments")]
    public async Task<IActionResult> AddComment(Guid postId, CreateCommentDto dto)
    {
        var comment = await postService.AddComment(postId, dto, UserId);
        
        if (comment == null) return NotFound("El post no existe");
                
        return Ok(new { message = "Comentario añadido", id = comment.Id });
    }
}