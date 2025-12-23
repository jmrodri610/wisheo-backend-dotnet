using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using wisheo_backend_v2.Services;

namespace wisheo_backend_v2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedController(FeedService feedService) : ControllerBase
{
    private readonly FeedService _feedService = feedService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeed()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
        {
            var anonymousFeed = await _feedService.GetAnonymousFeed();
            return Ok(anonymousFeed);
        }
        
        var userId = int.Parse(userIdClaim.Value);
        var personalizedFeed = await _feedService.GetFullFeed(userId);
        return Ok(personalizedFeed);
    }
}