using System.Security.Claims;
using AngularNetBase.Identity.Dtos;
using AngularNetBase.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularNetBase.API.Controllers;

[ApiController]
[Authorize]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly AuthService _authService;

    public ProfileController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _authService.GetProfileAsync(userId);
        return Ok(result);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileMeRequest request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _authService.UpdateProfileAsync(userId, request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
