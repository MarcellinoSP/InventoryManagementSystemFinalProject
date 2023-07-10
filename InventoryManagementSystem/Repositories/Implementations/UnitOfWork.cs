using InventoryManagementSystem.Data;
namespace InventoryManagementSystem.Repositories;

public class UnitOfWork : IUnitOfWork
{
    public ICategoryRepository CategoryRepository { get; }
    public ISubCategoryRepository SubCategoryRepository { get; }
    public ISupplierRepository SupplierRepository { get; }
    public IItemRepository ItemRepository { get; }

    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context, ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository, ISupplierRepository supplierRepository, IItemRepository itemRepository)
    {
        _context = context;
        CategoryRepository = categoryRepository;
        SubCategoryRepository = subCategoryRepository;
        SupplierRepository = supplierRepository;
        ItemRepository = itemRepository;
    }
    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }
}