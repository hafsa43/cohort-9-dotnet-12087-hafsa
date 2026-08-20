using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.DTOs;

public record CreateTaskDto(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate,
    string? AssignedToUserId,
    int? CategoryId
);

public record UpdateTaskDto(
    string Title,
    string? Description,
    AppTaskStatus Status,
    TaskPriority Priority,
    DateTime? DueDate,
    string? AssignedToUserId,
    int? CategoryId
);

public record TaskResponseDto(
    int Id,
    string Title,
    string? Description,
    string Status,
    string Priority,
    DateTime? DueDate,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    string? AssignedToUserId,
    string? AssignedToUserName,
    int? CategoryId,
    string? CategoryName
);

public record TaskCountDto(
    int Pending,
    int InProgress,
    int Completed,
    int Total
);

public record TaskFilterDto(
    string? Search       = null,
    string? Status       = null,
    string? Priority     = null,
    int?    CategoryId   = null,
    string? AssignedToUserId = null
);

