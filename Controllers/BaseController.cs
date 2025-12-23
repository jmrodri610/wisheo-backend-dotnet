using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace wisheo_backend_v2.Controllers;

public abstract class BaseController : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException("Usuario no autenticado");
            return Guid.Parse(claim.Value);
        }
    }

    protected Guid? OptionalUserId 
    {
        get 
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null) return null;
            
            return Guid.Parse(claim.Value);
        }
    }
}