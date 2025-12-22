using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using wisheo_backend_v2.Data;
using wisheo_backend_v2.DTOs;
using wisheo_backend_v2.Models;
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
}