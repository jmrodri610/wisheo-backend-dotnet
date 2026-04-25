using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wisheo_backend_v2.Models;

[Table("users")]
public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public string Surname { get; set; } = string.Empty;
    [Required, MaxLength(50)]
    public string Username { get; set; } = string.Empty;
    [MaxLength(200)]
    public string? Email { get; set; }
    [Required]
    public DateTime Birthday { get; set; }
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    public string? FirebaseUid { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Wishlist> Wishlists { get; set; } = [];
    public ICollection<Follow> Followers { get; set; } = [];
    public ICollection<Follow> Following { get; set; } = [];
}