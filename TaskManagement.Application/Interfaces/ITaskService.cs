using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(string userId, bool isAdmin, TaskFilterDto? filter = null);
    Task<TaskResponseDto?> GetTaskByIdAsync(int id, string userId, bool isAdmin);
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, string createdByUserId);
    Task<TaskResponseDto> UpdateTaskAsync(int id, UpdateTaskDto dto, string userId, bool isAdmin);
    Task<bool> DeleteTaskAsync(int id, string userId, bool isAdmin);
    Task<TaskCountDto> GetTaskCountsAsync(string userId, bool isAdmin);
}
