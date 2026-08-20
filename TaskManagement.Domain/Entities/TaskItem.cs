using TaskManagement.Domain.Enums;

namespace TaskManagement.Domain.Entities;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AppTaskStatus Status { get; set; } = AppTaskStatus.Pending;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // FK to AppUser
    public string? AssignedToUserId { get; set; }
    public AppUser? AssignedToUser { get; set; }

    // FK to Category
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
}
