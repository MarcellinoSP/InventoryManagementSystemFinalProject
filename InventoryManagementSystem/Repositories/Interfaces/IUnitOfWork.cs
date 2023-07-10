namespace InventoryManagementSystem.Repositories;

public interface IUnitOfWork
{
    ICategoryRepository CategoryRepository { get; }
    ISubCategoryRepository SubCategoryRepository { get; }
    ISupplierRepository SupplierRepository { get; }
    IItemRepository ItemRepository { get; }

    Task Commit();
}
