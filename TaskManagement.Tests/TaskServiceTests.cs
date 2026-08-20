using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Enums;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Services;
using Xunit;

namespace TaskManagement.Tests;

public class TaskServiceTests
{
    private AppDbContext CreateInMemoryDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    // ── Create ────────────────────────────────────────────────────────────────
    [Fact]
    public async Task CreateTask_ShouldReturn_CreatedTask()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);
        var dto     = new CreateTaskDto("Test Task", "Description", TaskPriority.High, null, "user-1", null);

        var result = await service.CreateTaskAsync(dto, "user-1");

        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal("Pending", result.Status);
        Assert.Equal("High", result.Priority);
    }

    // ── Counts ────────────────────────────────────────────────────────────────
    [Fact]
    public async Task GetTaskCounts_ShouldReturn_CorrectCounts()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);

        await service.CreateTaskAsync(new CreateTaskDto("Task 1", null, TaskPriority.Low,  null, "user-1", null), "user-1");
        await service.CreateTaskAsync(new CreateTaskDto("Task 2", null, TaskPriority.High, null, "user-1", null), "user-1");

        var counts = await service.GetTaskCountsAsync("user-1", false);

        Assert.Equal(2, counts.Pending);
        Assert.Equal(0, counts.InProgress);
        Assert.Equal(0, counts.Completed);
        Assert.Equal(2, counts.Total);
    }

    // ── Delete ────────────────────────────────────────────────────────────────
    [Fact]
    public async Task DeleteTask_ShouldReturn_True_WhenExists()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);
        var created = await service.CreateTaskAsync(new CreateTaskDto("To Delete", null, TaskPriority.Low, null, "user-1", null), "user-1");

        var deleted = await service.DeleteTaskAsync(created.Id, "user-1", false);

        Assert.True(deleted);
    }

    [Fact]
    public async Task DeleteTask_ShouldReturn_False_WhenNotFound()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);

        var deleted = await service.DeleteTaskAsync(9999, "user-1", true);

        Assert.False(deleted);
    }

    // ── Role-based access ────────────────────────────────────────────────────
    [Fact]
    public async Task GetAllTasks_Admin_ShouldReturn_AllTasks()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);

        await service.CreateTaskAsync(new CreateTaskDto("Task A", null, TaskPriority.Low,  null, "user-1", null), "user-1");
        await service.CreateTaskAsync(new CreateTaskDto("Task B", null, TaskPriority.High, null, "user-2", null), "user-2");

        var tasks = (await service.GetAllTasksAsync("user-1", isAdmin: true)).ToList();

        Assert.Equal(2, tasks.Count);
    }

    [Fact]
    public async Task GetAllTasks_User_ShouldReturn_OwnTasksOnly()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);

        await service.CreateTaskAsync(new CreateTaskDto("My Task",    null, TaskPriority.Low,  null, "user-1", null), "user-1");
        await service.CreateTaskAsync(new CreateTaskDto("Other Task", null, TaskPriority.High, null, "user-2", null), "user-2");

        var tasks = (await service.GetAllTasksAsync("user-1", isAdmin: false)).ToList();

        Assert.Single(tasks);
        Assert.Equal("My Task", tasks[0].Title);
    }

    // ── Server-side filtering (Task 3) ────────────────────────────────────────
    [Fact]
    public async Task GetAllTasks_WithSearchFilter_ShouldReturn_MatchingTasks()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);

        await service.CreateTaskAsync(new CreateTaskDto("Fix login bug",   null, TaskPriority.High, null, "user-1", null), "user-1");
        await service.CreateTaskAsync(new CreateTaskDto("Write unit tests", null, TaskPriority.Low,  null, "user-1", null), "user-1");

        var filter = new TaskFilterDto(Search: "bug");
        var tasks  = (await service.GetAllTasksAsync("user-1", isAdmin: false, filter)).ToList();

        Assert.Single(tasks);
        Assert.Equal("Fix login bug", tasks[0].Title);
    }

    [Fact]
    public async Task GetAllTasks_WithPriorityFilter_ShouldReturn_OnlyHighPriority()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);

        await service.CreateTaskAsync(new CreateTaskDto("High task",   null, TaskPriority.High,   null, "user-1", null), "user-1");
        await service.CreateTaskAsync(new CreateTaskDto("Medium task", null, TaskPriority.Medium, null, "user-1", null), "user-1");
        await service.CreateTaskAsync(new CreateTaskDto("Low task",    null, TaskPriority.Low,    null, "user-1", null), "user-1");

        var filter = new TaskFilterDto(Priority: "High");
        var tasks  = (await service.GetAllTasksAsync("user-1", isAdmin: false, filter)).ToList();

        Assert.Single(tasks);
        Assert.Equal("High", tasks[0].Priority);
    }

    [Fact]
    public async Task UpdateTask_ShouldChange_StatusAndTitle()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);
        var created = await service.CreateTaskAsync(
            new CreateTaskDto("Original", "desc", TaskPriority.Low, null, "user-1", null), "user-1");

        var updateDto = new UpdateTaskDto(
            "Updated Title", "new desc", AppTaskStatus.InProgress,
            TaskPriority.High, null, "user-1", null);

        var updated = await service.UpdateTaskAsync(created.Id, updateDto, "user-1", false);

        Assert.Equal("Updated Title", updated.Title);
        Assert.Equal("InProgress", updated.Status);
        Assert.Equal("High", updated.Priority);
    }

    [Fact]
    public async Task DeleteTask_UnauthorizedUser_ShouldThrow()
    {
        var db      = CreateInMemoryDb();
        var service = new TaskService(db);
        var created = await service.CreateTaskAsync(
            new CreateTaskDto("Owned by user-1", null, TaskPriority.Low, null, "user-1", null), "user-1");

        // user-2 (non-admin) tries to delete user-1's task
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => service.DeleteTaskAsync(created.Id, "user-2", isAdmin: false));
    }
}
