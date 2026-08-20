namespace TaskManagement.Domain.Enums;

public enum TaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3
}

/// <summary>Renamed to AppTaskStatus to avoid conflict with System.Threading.Tasks.TaskStatus</summary>
public enum AppTaskStatus
{
    Pending = 1,
    InProgress = 2,
    Completed = 3
}

public enum UserRole
{
    Admin = 1,
    User = 2
}
