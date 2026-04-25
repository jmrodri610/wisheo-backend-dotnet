
namespace wisheo_backend_v2.DTOs;
public record UserRegisterDto(
    string Name,
    string Surname,
    string Username,
    string Email,
    DateTime Birthday,
    string Password
);