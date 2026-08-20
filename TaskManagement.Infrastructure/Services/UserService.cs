using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDbContext _db;

    public UserService(UserManager<AppUser> userManager, AppDbContext db)
    {
        _userManager = userManager;
        _db          = db;
    }

    public async Task<IEnumerable<UserListDto>> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<UserListDto>();

        foreach (var user in users)
        {
            var roles     = await _userManager.GetRolesAsync(user);
            var taskCount = await _db.Tasks.CountAsync(t => t.AssignedToUserId == user.Id);
            result.Add(MapToDto(user, roles.FirstOrDefault() ?? "User", taskCount));
        }

        return result.OrderBy(u => u.CreatedAt);
    }

    public async Task<UserListDto?> GetUserByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;

        var roles     = await _userManager.GetRolesAsync(user);
        var taskCount = await _db.Tasks.CountAsync(t => t.AssignedToUserId == userId);
        return MapToDto(user, roles.FirstOrDefault() ?? "User", taskCount);
    }

    public async Task ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task ChangeRoleAsync(ChangeRoleDto dto)
    {
        if (dto.NewRole != "Admin" && dto.NewRole != "User")
            throw new InvalidOperationException("Role must be 'Admin' or 'User'.");

        var user = await _userManager.FindByIdAsync(dto.UserId)
            ?? throw new KeyNotFoundException("User not found.");

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, dto.NewRole);
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        await _userManager.DeleteAsync(user);
        return true;
    }

    // ── helper ────────────────────────────────────────────────────────────────
    private static UserListDto MapToDto(AppUser u, string role, int taskCount) => new(
        u.Id, u.FirstName, u.LastName, u.Email!, role, taskCount, u.CreatedAt
    );
}
