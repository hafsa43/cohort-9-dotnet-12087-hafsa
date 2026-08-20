using TaskManagement.Application.DTOs;

namespace TaskManagement.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<bool> DeleteAsync(int id);
}
