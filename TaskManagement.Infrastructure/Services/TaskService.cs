using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync(string userId, bool isAdmin)
    {
        var query = _db.Tasks
            .Include(t => t.AssignedToUser)
            .Include(t => t.Category)
            .AsQueryable();

        if (!isAdmin)
            query = query.Where(t => t.AssignedToUserId == userId);

        return await query.OrderByDescending(t => t.CreatedAt)
                          .Select(t => MapToDto(t))
                          .ToListAsync();
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(int id, string userId, bool isAdmin)
    {
        var task = await _db.Tasks
            .Include(t => t.AssignedToUser)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return null;
        if (!isAdmin && task.AssignedToUserId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        return MapToDto(task);
    }

    public async Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto dto, string createdByUserId)
    {
        var task = new TaskItem
        {
            Title            = dto.Title,
            Description      = dto.Description,
            Priority         = dto.Priority,
            DueDate          = dto.DueDate,
            AssignedToUserId = dto.AssignedToUserId ?? createdByUserId,
            CategoryId       = dto.CategoryId,
            Status           = AppTaskStatus.Pending,
            CreatedAt        = DateTime.UtcNow,
            UpdatedAt        = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        await _db.Entry(task).Reference(t => t.AssignedToUser).LoadAsync();
        await _db.Entry(task).Reference(t => t.Category).LoadAsync();

        return MapToDto(task);
    }

    public async Task<TaskResponseDto> UpdateTaskAsync(int id, UpdateTaskDto dto, string userId, bool isAdmin)
    {
        var task = await _db.Tasks
            .Include(t => t.AssignedToUser)
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new KeyNotFoundException($"Task {id} not found.");

        if (!isAdmin && task.AssignedToUserId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        task.Title            = dto.Title;
        task.Description      = dto.Description;
        task.Status           = dto.Status;
        task.Priority         = dto.Priority;
        task.DueDate          = dto.DueDate;
        task.AssignedToUserId = dto.AssignedToUserId;
        task.CategoryId       = dto.CategoryId;
        task.UpdatedAt        = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapToDto(task);
    }

    public async Task<bool> DeleteTaskAsync(int id, string userId, bool isAdmin)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return false;

        if (!isAdmin && task.AssignedToUserId != userId)
            throw new UnauthorizedAccessException("Access denied.");

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<TaskCountDto> GetTaskCountsAsync(string userId, bool isAdmin)
    {
        var query = _db.Tasks.AsQueryable();
        if (!isAdmin) query = query.Where(t => t.AssignedToUserId == userId);

        var pending    = await query.CountAsync(t => t.Status == AppTaskStatus.Pending);
        var inProgress = await query.CountAsync(t => t.Status == AppTaskStatus.InProgress);
        var completed  = await query.CountAsync(t => t.Status == AppTaskStatus.Completed);

        return new TaskCountDto(pending, inProgress, completed, pending + inProgress + completed);
    }

    private static TaskResponseDto MapToDto(TaskItem t) => new(
        t.Id,
        t.Title,
        t.Description,
        t.Status.ToString(),
        t.Priority.ToString(),
        t.DueDate,
        t.CreatedAt,
        t.UpdatedAt,
        t.AssignedToUserId,
        t.AssignedToUser != null ? $"{t.AssignedToUser.FirstName} {t.AssignedToUser.LastName}" : null,
        t.CategoryId,
        t.Category?.Name
    );
}
