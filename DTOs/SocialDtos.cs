namespace wisheo_backend_v2.DTOs;

public record SocialListDto(int Count, IEnumerable<UserSummaryDto> Users);
public record UserSummaryDto(Guid Id, string Name, string? Surname);
    