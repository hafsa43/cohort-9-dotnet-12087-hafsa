namespace TaskManagement.Application.DTOs;

public record CategoryDto(
    int Id,
    string Name,
    string? Description,
    int TaskCount
);

public record CreateCategoryDto(
    string Name,
    string? Description
);
