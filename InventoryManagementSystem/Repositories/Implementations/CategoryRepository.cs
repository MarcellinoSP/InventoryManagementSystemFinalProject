using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category> GetByCodeAndName(string categoryCode, string categoryName)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.CategoryCode == categoryCode && c.CategoryName == categoryName);
    }

    public async Task Add(Category category)
    {
        await _context.Categories.AddAsync(category);
    }
}
