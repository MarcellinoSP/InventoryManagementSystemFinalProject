namespace InventoryManagementSystem.Test;
using FakeItEasy;
using InventoryManagementSystem.Controllers;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

[TestFixture]
public class Tests
{
	private ApplicationDbContext _fakeDbContext;
	private IWebHostEnvironment _fakeWebHostEnvironment;
	private IUnitOfWork _fakeUnitOfWork;
	private SqliteConnection _dbConnection;

	[SetUp]
	public void Setup()
	{
		_dbConnection = new SqliteConnection("DataSource = :memory:");
		_dbConnection.Open();

		var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseSqlite(_dbConnection)
				.Options;
		_fakeDbContext = new ApplicationDbContext(options);
		_fakeDbContext.Database.EnsureCreated();
		Category categories = new Category()
		{
			IdCategory = 1,
			CategoryName = "Test",
			CategoryCode = "01"
		};
		SubCategory subCategories = new SubCategory()
		{
			SubCategoryName = "Test",
			SubCategoryCode = "01",
			CategoryId = 1
		};
		Supplier supplier = new Supplier()
		{
			SupplierId = 1,
			CompanyName = "Test"
		};
		_fakeDbContext.Add(supplier);
		_fakeDbContext.Add(subCategories);
		_fakeDbContext.Add(categories);
		_fakeDbContext.SaveChanges();
		Item item1 = new Item
		{
			Name = "Sofa1",
			CategoryId = 1,
			SubCategoryId = 1,
			SupplierId = 1
		};
		Item item2 = new Item
		{
			Name = "Sofa2",
			CategoryId = 1,
			SubCategoryId = 1,
			SupplierId = 1
		};
		Item item3 = new Item
		{
			Name = "Sofa3",
			CategoryId = 1,
			SubCategoryId = 1,
			SupplierId = 1
		};
		_fakeDbContext.Items.Add(item1);
		_fakeDbContext.Items.Add(item2);
		_fakeDbContext.Items.Add(item3);
		_fakeDbContext.SaveChanges();
		_fakeWebHostEnvironment = A.Fake<IWebHostEnvironment>();
		_fakeUnitOfWork = A.Fake<IUnitOfWork>();
	}

	[Test]
	public async Task ItemIndexPage_TestSearchFunction()
	{
		var itemController = new ItemsController(_fakeDbContext, _fakeWebHostEnvironment, _fakeUnitOfWork);
		var resultList = new List<Item>();

		var result = await itemController.Index("Ballpoint") as ViewResult;
		var model = result.Model as List<Item>;

		Assert.IsNotNull(result);
		Assert.AreEqual(0, model.Count());
	}

	[Test]
	public async Task SupplierDetailsPage_TestReturnObject()
	{
		var supplierControllerTest = new SuppliersController(_fakeDbContext);
		var result = await supplierControllerTest.Details(1) as ViewResult;
		
		Assert.IsNotNull(result);
		StringAssert.AreEqualIgnoringCase("supplier", result.Model.GetType().Name);
	}
	
	[Test]
	public async Task CategoriesPage_TestSearchIndex()
	{
		var controller = new CategoriesController(_fakeDbContext);
		var result = await controller.Search("Tool");
		
		Assert.IsNotNull(result);
		Assert.That(result, Is.InstanceOf<List<Category>>());
	}
	
	[Test]
	public async Task CategoriesPage_TestDetailsPage()
	{
		var controller = new CategoriesController(_fakeDbContext);
		var result = await controller.Details(1);
		
		Assert.IsNotNull(result);
		Assert.That(result, Is.InstanceOf<IActionResult>());
	}
	
	[Test]
	public async Task SubCategoriesControllers_TestSubCategoriesExists()
	{
		var controller = new SubCategoriesController(_fakeDbContext);
		var result = await controller.Details(0) as NotFoundResult;
		var resultFound = await controller.Details(1) as ViewResult;
		
		var model = resultFound.Model as SubCategory;
		string modelResult = model.SubCategoryName;
		string expectedModelResult = "Test";
		
		Assert.IsNotNull(result);
		Assert.That(resultFound, Is.InstanceOf<IActionResult>());
		Assert.AreEqual(expectedModelResult, modelResult);
	}
}