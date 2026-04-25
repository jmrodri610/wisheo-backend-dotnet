namespace wisheo_backend_v2.Services;

using wisheo_backend_v2.Models;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Repositories;
using wisheo_backend_v2.Helpers;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin.Auth;

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
            throw new Exception("Username already taken.");

        if (await _userRepository.ExistsByEmail(dto.Email))
            throw new Exception("Email is already registered.");

        var user = new User
        {
            Name = dto.Name,
            Surname = dto.Surname,
            Username = dto.Username,
            Email = dto.Email,
            Birthday = DateTime.SpecifyKind(dto.Birthday, DateTimeKind.Utc),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        await _userRepository.Add(user);
        return user;
    }

    public async Task<AuthResponseDto?> FirebaseLogin(string idToken)
    {
        FirebaseToken decoded;
        try
        {
            decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        }
        catch
        {
            return null;
        }

        var firebaseUid = decoded.Uid;
        var email = decoded.Claims.TryGetValue("email", out var e) ? e?.ToString() : null;
        var name = decoded.Claims.TryGetValue("name", out var n) ? n?.ToString() ?? "" : "";

        var user = await _userRepository.GetByFirebaseUid(firebaseUid);

        if (user == null && email != null)
            user = await _userRepository.GetByEmail(email);

        if (user == null)
        {
            var nameParts = (name ?? "").Split(' ', 2);
            user = new User
            {
                Name = nameParts.Length > 0 ? nameParts[0] : "User",
                Surname = nameParts.Length > 1 ? nameParts[1] : "",
                Username = email?.Split('@')[0] ?? firebaseUid[..8],
                Email = email,
                FirebaseUid = firebaseUid,
                Birthday = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString())
            };
            await _userRepository.Add(user);
        }
        else if (user.FirebaseUid == null)
        {
            user.FirebaseUid = firebaseUid;
            await _userRepository.UpdateUser(user);
        }

        var accessToken = _jwtHelper.GenerateAccessToken(user);
        var refreshTokenValue = _jwtHelper.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        await _context.SaveChangesAsync();

        return new AuthResponseDto(accessToken, refreshTokenValue, user.Username);
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