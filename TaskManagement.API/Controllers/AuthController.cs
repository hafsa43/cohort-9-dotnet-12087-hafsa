using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService auth, ILogger<AuthController> logger)
    {
        _auth   = auth;
        _logger = logger;
    }

    /// <summary>Register a new user</summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        _logger.LogInformation("Register attempt for email: {Email}", dto.Email);
        var result = await _auth.RegisterAsync(dto);
        _logger.LogInformation("User registered successfully: {Email}", dto.Email);
        return Ok(result);
    }

    /// <summary>Login with email and password</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        _logger.LogInformation("Login attempt for email: {Email}", dto.Email);
        var result = await _auth.LoginAsync(dto);
        _logger.LogInformation("Login successful for: {Email}", dto.Email);
        return Ok(result);
    }

    /// <summary>Get current user profile</summary>
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _auth.GetProfileAsync(userId);
        return Ok(result);
    }
}
