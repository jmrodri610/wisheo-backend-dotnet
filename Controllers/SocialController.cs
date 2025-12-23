using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wisheo_backend_v2.Services;

namespace wisheo_backend_v2.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SocialController(SocialService socialService) : BaseController
{
    [HttpPost("follow/{followedId}")]
    public async Task<IActionResult> Follow(Guid followedId)
    {
        var result = await socialService.FollowUser(UserId, followedId);
        if (!result) return BadRequest("Acción no válida (posiblemente intentes seguirte a ti mismo)");
        return Ok(new { message = "Ahora sigues a este usuario" });
    }

    [HttpDelete("unfollow/{followedId}")]
    public async Task<IActionResult> Unfollow(Guid followedId)
    {
        await socialService.UnfollowUser(UserId, followedId);
        return Ok(new { message = "Has dejado de seguir a este usuario" });
    }

    [HttpGet("followers")]
    public async Task<IActionResult> GetMyFollowers()
    {
        var data = await socialService.GetFollowersList(UserId);
        return Ok(data);
    }

    [HttpGet("following")]
    public async Task<IActionResult> GetMyFollowing()
    {
        var data = await socialService.GetFollowingList(UserId);
        return Ok(data);
    }
}