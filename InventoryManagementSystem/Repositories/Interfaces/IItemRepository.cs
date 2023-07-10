using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Repositories;

public interface IItemRepository
{
    Task Add(Item item);
}
