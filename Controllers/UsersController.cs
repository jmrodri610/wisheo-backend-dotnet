namespace wisheo_backend_v2.Controllers;

using Microsoft.AspNetCore.Mvc;
using wisheo_backend_v2.Services;
using wisheo_backend_v2.DTOs;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        try 
        {
            var user = await _userService.RegisterUser(dto);
            return Ok(new { message = "Usuario registrado", id = user.Id });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}