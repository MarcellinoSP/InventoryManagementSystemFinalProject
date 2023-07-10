using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Repositories;

public class SupplierRepository : ISupplierRepository
{
    private readonly ApplicationDbContext _context;

    public SupplierRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Supplier> GetByNameAndContactNumber(string companyName, string contactNumber)
    {
        return await _context.Suppliers
            .FirstOrDefaultAsync(s => s.CompanyName == companyName && s.ContactNumber == contactNumber);
    }

    public async Task Add(Supplier supplier)
    {
        await _context.Suppliers.AddAsync(supplier);
    }
}
