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
}
