using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wisheo_backend_v2.Services;

namespace wisheo_backend_v2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilesController(ProfileService profileService, UserService userService) : BaseController
    {
        [HttpGet("{username}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfile(string username)
        {
            var user = await userService.GetUserByUsername(username); 
            if (user == null) return NotFound();

            var profile = await profileService.GetPublicProfile(user.Id, OptionalUserId);
            
            return Ok(profile);
        }
    }
}