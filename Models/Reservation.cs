using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wisheo_backend_v2.Models;

[Table("reservations")]
public class Reservation
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid WishItemId { get; set; }
    public WishItem WishItem { get; set; } = null!;

    [MaxLength(100)]
    public string GuestName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? GuestEmail { get; set; }

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    [Required]
    [MaxLength(64)]
    public string CancelToken { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
