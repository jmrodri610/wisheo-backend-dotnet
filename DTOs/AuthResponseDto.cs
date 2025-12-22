namespace wisheo_backend_v2.DTOs;

public record AuthResponseDto(
    string AccessToken, 
    string RefreshToken, 
    string Username
);