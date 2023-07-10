using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Repositories;

public interface ISupplierRepository
{
    Task<Supplier> GetByNameAndContactNumber(string companyName, string contactNumber);
    Task Add(Supplier supplier);
}
