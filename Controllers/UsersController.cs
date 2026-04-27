namespace wisheo_backend_v2.Controllers;
using Microsoft.AspNetCore.Mvc;
using wisheo_backend_v2.Services;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Repositories;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class UsersController(UserService userService, DeviceTokenRepository deviceTokenRepository) : BaseController
{
    private readonly UserService _userService = userService;
    private readonly DeviceTokenRepository _deviceTokenRepository = deviceTokenRepository;

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

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var response = await _userService.Login(dto);
        
        if (response == null)
            return Unauthorized(new { message = "Usuario o contraseña incorrectos" });

        return Ok(response);
    }

    [Authorize]
    [HttpPatch("me")]
    public async Task<IActionResult> UpdateProfile(UpdateUserDto dto)
    {

        var success = await _userService.UpdateUser(UserId, dto);

        if (!success) return BadRequest();

        return Ok(new { message = "Perfil actualizado correctamente" });
    }

    [HttpPost("firebase-login")]
    public async Task<IActionResult> FirebaseLogin([FromBody] FirebaseLoginDto dto)
    {
        var response = await _userService.FirebaseLogin(dto.IdToken);
        if (response == null)
            return Unauthorized(new { message = "Firebase token inválido." });
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRequestDto request)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            return BadRequest("El Refresh Token es obligatorio.");
        }

        var result = await _userService.RefreshSessionAsync(request.RefreshToken);

        if (result == null)
        {
            return Unauthorized(new { message = "Sesión inválida o expirada. Inicie sesión nuevamente." });
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost("me/device-token")]
    public async Task<IActionResult> RegisterDeviceToken([FromBody] RegisterDeviceTokenDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Token))
            return BadRequest(new { message = "Token vacío." });

        await _deviceTokenRepository.Upsert(UserId, dto.Token, dto.Platform);
        return Ok(new { message = "Device token registrado." });
    }
}