using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Services;

namespace wisheo_backend_v2.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WishlistsController(WishlistService wishlistService) : ControllerBase
{
    private readonly WishlistService _wishlistService = wishlistService;

    [HttpPost]
    public async Task<IActionResult> Create(CreateWishlistDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var id = await _wishlistService.CreateWishlist(dto, userId);
        return Ok(new { message = "Lista creada", id });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishlistResponseDto>>> GetAll()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var lists = await _wishlistService.GetUserWishlists(userId);
        return Ok(lists);
    }

    [HttpPost("{wishlistId}/items")]
    public async Task<IActionResult> AddItem(int wishlistId, CreateWishItemDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var itemId = await _wishlistService.AddItemToWishlist(wishlistId, userId, dto);

        if (itemId == null) return NotFound("Lista no encontrada o no tienes acceso");

        return Ok(new { message = "Deseo añadido", id = itemId });
    }

    [HttpPatch("items/{itemId}/toggle-purchased")]
    public async Task<IActionResult> TogglePurchased(int itemId)
    {
        var success = await _wishlistService.TogglePurchased(itemId);
        if (!success) return NotFound("Ítem no encontrado");
        return Ok(new { message = "Estado del ítem actualizado" });
    }

    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> DeleteItem(int itemId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var success = await _wishlistService.DeleteItem(itemId, userId);
        
        if (!success) return NotFound();
        
        return Ok(new { message = "Ítem eliminado correctamente" });
    }

    [HttpPut("items/{itemId}")]
    public async Task<IActionResult> UpdateItem(int itemId, UpdateWishItemDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var success = await _wishlistService.UpdateItem(itemId, userId, dto);
        
        if (!success) return NotFound("Ítem no encontrado o no tienes permisos para editarlo");

        return Ok(new { message = "Ítem actualizado correctamente" });
    }
}