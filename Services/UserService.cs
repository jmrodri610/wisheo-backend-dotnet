namespace wisheo_backend_v2.Services;

using wisheo_backend_v2.Models;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Repositories;
using wisheo_backend_v2.Helpers;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly JwtHelper _jwtHelper;
    private readonly AppDbContext _context;

    public UserService(UserRepository userRepository, JwtHelper jwtHelper, AppDbContext context)
    {
        _userRepository = userRepository;
        _jwtHelper = jwtHelper;
        _context = context;
    }

    public async Task<User> RegisterUser(UserRegisterDto dto)
    {
        if (await _userRepository.Exists(dto.Username))
        {
            throw new Exception("El usuario ya existe.");
        }

        var user = new User
        {
            Name = dto.Name,
            Surname = dto.Surname,
            Username = dto.Username,
            Birthday = DateTime.SpecifyKind(dto.Birthday, DateTimeKind.Utc),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _userRepository.Add(user);
        return user;
    }

    public async Task<AuthResponseDto?> Login(LoginDto dto)
    {
        var user = await _userRepository.GetByUsername(dto.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        var accessToken = _jwtHelper.GenerateAccessToken(user);
        var refreshTokenValue = _jwtHelper.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto(accessToken, refreshTokenValue, user.Username);
    }

    public async Task<User?> GetUserByUsername(string username)
    {
       var user = await _userRepository.GetByUsername(username);

       return user;
    }

    public async Task<bool> UpdateUser(Guid userId, UpdateUserDto dto)
    {
        var user = await _userRepository.GetUserById(userId);


        if (user == null) return false;

        if (dto.Name != null) user.Name = dto.Name;
        if (dto.Surname != null) user.Surname = dto.Surname;
        if (dto.Birthday != null) user.Birthday = (DateTime)dto.Birthday;

        await _userRepository.UpdateUser(user);
        return true;
    }

    public async Task<AuthResponseDto?> RefreshSessionAsync(string token)
    {
        var storedToken = await _context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);

        if (storedToken == null || storedToken.Expires < DateTime.UtcNow || storedToken.IsRevoked)
        {
            return null;
        }

        var newAccessToken = _jwtHelper.GenerateAccessToken(storedToken.User);
    
        var newRefreshTokenString = _jwtHelper.GenerateRefreshToken();
        storedToken.IsRevoked = true;

        var newTokenEntity = new RefreshToken
        {
            Token = newRefreshTokenString,
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = storedToken.UserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.RefreshTokens.Add(newTokenEntity);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        (
            newAccessToken,
            newRefreshTokenString,
            storedToken.User.Username
        );
    }
}