using System.ComponentModel.DataAnnotations;

namespace wisheo_backend_v2.Models;

public class Wishlist
{
    public int Id { get; set; }
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(500)]
    public string? Description { get; set; }
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<WishItem> Items { get; set; } = [];
}