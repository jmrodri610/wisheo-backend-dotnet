namespace wisheo_backend_v2.DTOs;

public record AddCollaboratorDto(string Username, string Role);

public record UpdateCollaboratorDto(string Role);

public record CollaboratorResponseDto(
    Guid UserId,
    string Username,
    string Name,
    string Surname,
    string Role,
    DateTime InvitedAt,
    DateTime? AcceptedAt
);
