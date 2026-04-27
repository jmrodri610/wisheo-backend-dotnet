using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace wisheo_backend_v2.Models;

public enum CollaboratorRole
{
    Viewer = 0,
    Editor = 1,
}

[Table("wishlist_collaborators")]
public class WishlistCollaborator
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid WishlistId { get; set; }
    public Wishlist Wishlist { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public CollaboratorRole Role { get; set; } = CollaboratorRole.Editor;

    public DateTime InvitedAt { get; set; } = DateTime.UtcNow;

    public DateTime? AcceptedAt { get; set; }
}
