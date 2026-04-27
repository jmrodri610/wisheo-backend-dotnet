using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Repositories;
using wisheo_backend_v2.Services;

namespace wisheo_backend_v2.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WishlistsController(WishlistService wishlistService, UserRepository userRepository) : BaseController
{
    private readonly WishlistService _wishlistService = wishlistService;
    private readonly UserRepository _userRepository = userRepository;

    [HttpPost]
    public async Task<IActionResult> Create(CreateWishlistDto dto)
    {
        var id = await _wishlistService.CreateWishlist(dto, UserId);
        return Ok(new { message = "Lista creada", id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CreateWishlistDto dto)
    {
        var success = await _wishlistService.UpdateWishlist(id, UserId, dto);
        
        if (!success) return NotFound("Lista no encontrada o no tienes permiso");
        
        return Ok(new { message = "Lista actualizada correctamente" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _wishlistService.DeleteWishlist(id, UserId);
        
        if (!success) return NotFound("Lista no encontrada o no tienes permiso");
        
        return Ok(new { message = "Lista eliminada correctamente" });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WishlistResponseDto>>> GetAll()
    {
        var lists = await _wishlistService.GetUserWishlists(UserId);
        return Ok(lists);
    }

    [HttpPost("{id}/share")]
    public async Task<IActionResult> Share(Guid id)
    {
        var slug = await _wishlistService.EnsureShareSlug(id, UserId);
        if (slug == null) return NotFound("Lista no encontrada o no tienes permiso");

        return Ok(new { slug });
    }

    [HttpPost("{wishlistId}/items")]
    public async Task<IActionResult> AddItem(Guid wishlistId, CreateWishItemDto dto)
    {
        var itemId = await _wishlistService.AddItemToWishlist(wishlistId, UserId, dto);

        if (itemId == null) return NotFound("Lista no encontrada o no tienes acceso");

        return Ok(new { message = "Deseo añadido", id = itemId });
    }

    [HttpPatch("items/{itemId}/toggle-purchased")]
    public async Task<IActionResult> TogglePurchased(Guid itemId)
    {
        var success = await _wishlistService.TogglePurchased(itemId);
        if (!success) return NotFound("Ítem no encontrado");
        return Ok(new { message = "Estado del ítem actualizado" });
    }

    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> DeleteItem(Guid itemId)
    {
        var success = await _wishlistService.DeleteItem(itemId, UserId);
        
        if (!success) return NotFound();
        
        return Ok(new { message = "Ítem eliminado correctamente" });
    }

    [HttpPut("items/{itemId}")]
    public async Task<IActionResult> UpdateItem(Guid itemId, UpdateWishItemDto dto)
    {
        var success = await _wishlistService.UpdateItem(itemId, UserId, dto);
        
        if (!success) return NotFound("Ítem no encontrado o no tienes permisos para editarlo");

        return Ok(new { message = "Ítem actualizado correctamente" });
    }

    // ── Collaborators ──────────────────────────────────────────────────────────

    [HttpGet("{id}/collaborators")]
    public async Task<IActionResult> GetCollaborators(Guid id)
    {
        var list = await _wishlistService.GetCollaborators(id, UserId);
        if (list == null) return NotFound("Lista no encontrada o sin acceso");
        return Ok(list);
    }

    [HttpPost("{id}/collaborators")]
    public async Task<IActionResult> AddCollaborator(Guid id, [FromBody] AddCollaboratorDto dto)
    {
        var (ok, error) = await _wishlistService.AddCollaborator(id, UserId, dto, _userRepository);
        if (!ok) return BadRequest(new { message = error });
        return Ok(new { message = "Colaborador añadido" });
    }

    [HttpDelete("{id}/collaborators/{userId}")]
    public async Task<IActionResult> RemoveCollaborator(Guid id, Guid userId)
    {
        var ok = await _wishlistService.RemoveCollaborator(id, UserId, userId);
        if (!ok) return NotFound("Colaborador no encontrado o no tienes permisos");
        return Ok(new { message = "Colaborador eliminado" });
    }
}