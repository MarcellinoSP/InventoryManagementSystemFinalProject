using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Repositories;

public class SubCategoryRepository : ISubCategoryRepository
{
    private readonly ApplicationDbContext _context;

    public SubCategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubCategory> GetByCodeAndName(string subCategoryCode, string subCategoryName)
    {
        return await _context.SubCategories
            .FirstOrDefaultAsync(s => s.SubCategoryCode == subCategoryCode && s.SubCategoryName == subCategoryName);
    }

    public async Task Add(SubCategory subCategory)
    {
        await _context.SubCategories.AddAsync(subCategory);
    }
}
