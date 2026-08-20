using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Services;
using Xunit;

namespace TaskManagement.Tests;

public class CategoryServiceTests
{
    private AppDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task CreateCategory_ShouldReturn_NewCategory()
    {
        var db      = CreateDb();
        var service = new CategoryService(db);

        var result = await service.CreateAsync(new CreateCategoryDto("Bug Fixes", "Track bugs"));

        Assert.NotNull(result);
        Assert.Equal("Bug Fixes", result.Name);
        Assert.Equal(0, result.TaskCount);
    }

    [Fact]
    public async Task GetAll_ShouldReturn_AllCategories()
    {
        var db      = CreateDb();
        var service = new CategoryService(db);

        await service.CreateAsync(new CreateCategoryDto("Alpha", null));
        await service.CreateAsync(new CreateCategoryDto("Beta", null));

        var all = (await service.GetAllAsync()).ToList();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturn_True_WhenExists()
    {
        var db      = CreateDb();
        var service = new CategoryService(db);

        var created = await service.CreateAsync(new CreateCategoryDto("ToDelete", null));
        var deleted = await service.DeleteAsync(created.Id);

        Assert.True(deleted);
    }

    [Fact]
    public async Task DeleteCategory_ShouldReturn_False_WhenNotFound()
    {
        var db      = CreateDb();
        var service = new CategoryService(db);

        var deleted = await service.DeleteAsync(9999);

        Assert.False(deleted);
    }
}
