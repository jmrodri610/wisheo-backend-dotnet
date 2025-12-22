using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wisheo_backend_v2.Models;

public class WishItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? ProductUrl { get; set; }
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Price { get; set; }
    public string? Currency { get; set; } = "EUR";
    public bool IsPurchased { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int WishlistId { get; set; }
    public Wishlist Wishlist { get; set; } = null!;
}