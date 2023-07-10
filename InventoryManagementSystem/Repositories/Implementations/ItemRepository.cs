using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly ApplicationDbContext  _context;

    public ItemRepository(ApplicationDbContext  context)
    {
        _context = context;
    }

    public async Task Add(Item item)
    {
        await _context.Items.AddAsync(item);
    }
}
