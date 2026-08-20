using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserListDto>> GetAllUsersAsync();
    Task<UserListDto?> GetUserByIdAsync(string userId);
    Task ChangePasswordAsync(string userId, ChangePasswordDto dto);
    Task ChangeRoleAsync(ChangeRoleDto dto);
    Task<bool> DeleteUserAsync(string userId);
}
