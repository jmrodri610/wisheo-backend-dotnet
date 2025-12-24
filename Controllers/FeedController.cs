using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wisheo_backend_v2.Services;

namespace wisheo_backend_v2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedController(FeedService feedService) : BaseController
{
    private readonly FeedService _feedService = feedService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeed()
    {

        if (OptionalUserId == null)
        {
            var anonymousFeed = await _feedService.GetAnonymousFeed();
            return Ok(anonymousFeed);
        }
        
        var personalizedFeed = await _feedService.GetFullFeed(OptionalUserId.Value);
        return Ok(personalizedFeed);
    }
}