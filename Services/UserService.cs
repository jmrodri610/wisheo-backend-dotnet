namespace wisheo_backend_v2.Services;

using wisheo_backend_v2.Models;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Data;
using wisheo_backend_v2.Helpers;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
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
            Birthday = DateTime.SpecifyKind(dto.Birthday, DateTimeKind.Utc)
        };

        user.PasswordHash = PasswordHasher.Hash(user, dto.Password);

        await _userRepository.Add(user);
        return user;
    }
}