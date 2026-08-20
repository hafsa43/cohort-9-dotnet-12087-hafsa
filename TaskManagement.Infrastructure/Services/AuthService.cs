using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<AppUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw new InvalidOperationException("Email already registered.");

        var user = new AppUser
        {
            FirstName = dto.FirstName,
            LastName  = dto.LastName,
            Email     = dto.Email,
            UserName  = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        // Assign role
        var role = dto.Role == "Admin" ? "Admin" : "User";
        await _userManager.AddToRoleAsync(user, role);

        return await GenerateTokenResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid) throw new UnauthorizedAccessException("Invalid email or password.");

        return await GenerateTokenResponse(user);
    }

    public async Task<UserProfileDto> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");
        var roles = await _userManager.GetRolesAsync(user);
        return new UserProfileDto(user.Id, user.FirstName, user.LastName, user.Email!, roles.FirstOrDefault() ?? "User", user.CreatedAt);
    }

    // ── helpers ──────────────────────────────────────────────────────────────
    private async Task<AuthResponseDto> GenerateTokenResponse(AppUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var role  = roles.FirstOrDefault() ?? "User";

        var jwtKey     = _config["Jwt:Key"]!;
        var jwtIssuer  = _config["Jwt:Issuer"]!;
        var expireHours = int.Parse(_config["Jwt:ExpireHours"] ?? "24");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddHours(expireHours);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtIssuer,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new AuthResponseDto(
            Token:    new JwtSecurityTokenHandler().WriteToken(token),
            Email:    user.Email!,
            FullName: $"{user.FirstName} {user.LastName}",
            Role:     role,
            Expires:  expires
        );
    }
}
