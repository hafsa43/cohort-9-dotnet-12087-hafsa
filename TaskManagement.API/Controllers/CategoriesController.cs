using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;

namespace TaskManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categories;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(ICategoryService categories, ILogger<CategoriesController> logger)
    {
        _categories = categories;
        _logger     = logger;
    }

    /// <summary>Get all categories (any authenticated user)</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categories.GetAllAsync();
        return Ok(result);
    }

    /// <summary>Create a new category (Admin only)</summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        _logger.LogInformation("Creating category: {Name}", dto.Name);
        var result = await _categories.CreateAsync(dto);
        _logger.LogInformation("Category created with ID {CategoryId}", result.Id);
        return CreatedAtAction(nameof(GetAll), result);
    }

    /// <summary>Delete a category (Admin only)</summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting category {CategoryId}", id);
        var deleted = await _categories.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
