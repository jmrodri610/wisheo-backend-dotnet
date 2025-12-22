namespace wisheo_backend_v2.Models;
using System.ComponentModel.DataAnnotations;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}