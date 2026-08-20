using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService users, ILogger<UsersController> logger)
    {
        _users  = users;
        _logger = logger;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    /// <summary>Get all users (Admin only)</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Admin listing all users");
        var result = await _users.GetAllUsersAsync();
        return Ok(result);
    }

    /// <summary>Get user by ID (Admin only)</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _users.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>Change own password (any authenticated user)</summary>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        _logger.LogInformation("User {UserId} changing password", UserId);
        await _users.ChangePasswordAsync(UserId, dto);
        _logger.LogInformation("Password changed for user {UserId}", UserId);
        return Ok(new { message = "Password changed successfully." });
    }

    /// <summary>Change a user's role (Admin only)</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("change-role")]
    public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDto dto)
    {
        _logger.LogInformation("Admin changing role of user {UserId} to {Role}", dto.UserId, dto.NewRole);
        await _users.ChangeRoleAsync(dto);
        return Ok(new { message = $"Role updated to {dto.NewRole}." });
    }

    /// <summary>Delete a user (Admin only)</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        _logger.LogWarning("Admin deleting user {UserId}", id);
        var deleted = await _users.DeleteUserAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
