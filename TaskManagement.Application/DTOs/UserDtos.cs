namespace TaskManagement.Application.DTOs;

public record UserListDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    int TaskCount,
    DateTime CreatedAt
);

public record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword
);

public record ChangeRoleDto(
    string UserId,
    string NewRole
);
