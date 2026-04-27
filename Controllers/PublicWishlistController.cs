using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Services;

namespace wisheo_backend_v2.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/public/wishlists")]
public class PublicWishlistController(WishlistService wishlistService) : BaseController
{
    private readonly WishlistService _wishlistService = wishlistService;

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var wishlist = await _wishlistService.GetPublicWishlist(slug);
        if (wishlist == null) return NotFound(new { message = "Lista no encontrada o no es pública." });
        return Ok(wishlist);
    }

    [HttpPost("{slug}/items/{itemId}/reserve")]
    public async Task<IActionResult> Reserve(string slug, Guid itemId, [FromBody] ReserveItemDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.GuestName))
            return BadRequest(new { message = "El nombre del invitado es obligatorio." });

        var result = await _wishlistService.ReserveItem(slug, itemId, dto, OptionalUserId);
        if (result == null) return Conflict(new { message = "Este item ya está reservado o no existe." });

        return Ok(result);
    }

    [HttpDelete("{slug}/items/{itemId}/reserve")]
    public async Task<IActionResult> CancelReservation(string slug, Guid itemId, [FromQuery] string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest(new { message = "Token de cancelación obligatorio." });

        var success = await _wishlistService.CancelReservation(slug, itemId, token);
        if (!success) return NotFound(new { message = "Reserva no encontrada." });

        return Ok(new { message = "Reserva cancelada correctamente." });
    }
}
