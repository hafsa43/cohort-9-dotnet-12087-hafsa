using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _tasks;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITaskService tasks, ILogger<TasksController> logger)
    {
        _tasks  = tasks;
        _logger = logger;
    }

    private string UserId   => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private bool   IsAdmin  => User.IsInRole("Admin");

    /// <summary>Get task dashboard counts</summary>
    [HttpGet("counts")]
    public async Task<IActionResult> GetCounts()
    {
        var counts = await _tasks.GetTaskCountsAsync(UserId, IsAdmin);
        return Ok(counts);
    }

    /// <summary>Get all tasks (admin = all, user = own)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching tasks for user {UserId}, isAdmin={IsAdmin}", UserId, IsAdmin);
        var tasks = await _tasks.GetAllTasksAsync(UserId, IsAdmin);
        return Ok(tasks);
    }

    /// <summary>Get task by ID</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _tasks.GetTaskByIdAsync(id, UserId, IsAdmin);
        return task == null ? NotFound() : Ok(task);
    }

    /// <summary>Create a new task</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        _logger.LogInformation("Creating task '{Title}' by user {UserId}", dto.Title, UserId);
        var task = await _tasks.CreateTaskAsync(dto, UserId);
        _logger.LogInformation("Task created with ID {TaskId}", task.Id);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    /// <summary>Update existing task</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        _logger.LogInformation("Updating task {TaskId} by user {UserId}", id, UserId);
        var task = await _tasks.UpdateTaskAsync(id, dto, UserId, IsAdmin);
        return Ok(task);
    }

    /// <summary>Delete a task</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting task {TaskId} by user {UserId}", id, UserId);
        var deleted = await _tasks.DeleteTaskAsync(id, UserId, IsAdmin);
        return deleted ? NoContent() : NotFound();
    }
}
