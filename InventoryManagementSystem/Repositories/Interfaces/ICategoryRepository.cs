using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Repositories;

public interface ICategoryRepository
{
    Task<Category> GetByCodeAndName(string categoryCode, string categoryName);
    Task Add(Category category);
}
