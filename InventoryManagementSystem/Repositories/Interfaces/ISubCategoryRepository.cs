using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Repositories;

public interface ISubCategoryRepository
{
    Task<SubCategory> GetByCodeAndName(string subCategoryCode, string subCategoryName);
    Task Add(SubCategory subCategory);
}
