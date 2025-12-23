namespace wisheo_backend_v2.DTOs
{
    public record UpdateUserDto(
    string? Name,
    string? Surname,
    DateTime? Birthday
);
}