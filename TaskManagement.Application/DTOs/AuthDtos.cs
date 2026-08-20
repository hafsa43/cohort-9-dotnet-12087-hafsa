namespace TaskManagement.Application.DTOs;

public record RegisterDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string Role = "User"
);

public record LoginDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    string Token,
    string Email,
    string FullName,
    string Role,
    DateTime Expires
);

public record UserProfileDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    DateTime CreatedAt
);
