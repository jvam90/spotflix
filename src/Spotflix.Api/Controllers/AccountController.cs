using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotflix.Api.Dtos.Auth;
using Spotflix.Api.Dtos.Users;
using Spotflix.Api.Models;
using Spotflix.Api.Services;

namespace Spotflix.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUser;

    public AccountController(UserManager<ApplicationUser> userManager, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(ToDto(user, roles));
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
            return Unauthorized();

        user.FullName = dto.FullName;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return NoContent();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var user = await GetCurrentUserAsync();
        if (user is null)
            return Unauthorized();

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(new { message = "Senha alterada com sucesso." });
    }

    private Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var id = _currentUser.UserId;
        return id is null ? Task.FromResult<ApplicationUser?>(null) : _userManager.FindByIdAsync(id.Value.ToString());
    }

    private static UserDto ToDto(ApplicationUser user, IList<string> roles) => new()
    {
        Id = user.Id,
        Email = user.Email!,
        FullName = user.FullName,
        EmailConfirmed = user.EmailConfirmed,
        CreatedAt = user.CreatedAt,
        LockoutEnd = user.LockoutEnd,
        IsLockedOut = user.LockoutEnd is not null && user.LockoutEnd > DateTimeOffset.UtcNow,
        Roles = roles.ToArray(),
    };
}
