using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wisheo_backend_v2.Models;

[Table("device_tokens")]
public class DeviceToken
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Platform { get; set; } = "unknown";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastUsed { get; set; } = DateTime.UtcNow;
}
