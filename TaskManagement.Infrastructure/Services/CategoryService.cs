using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        return await _db.Categories
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.Tasks.Count
            ))
            .ToListAsync();
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name        = dto.Name,
            Description = dto.Description,
            CreatedAt   = DateTime.UtcNow
        };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return new CategoryDto(category.Id, category.Name, category.Description, 0);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category == null) return false;
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return true;
    }
}
