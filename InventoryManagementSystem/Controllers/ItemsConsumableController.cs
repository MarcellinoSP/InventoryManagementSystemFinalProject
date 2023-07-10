using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.Sqlite;

namespace InventoryManagementSystem.Controllers
{
	[Authorize]
	public class ItemsConsumableController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvirontment;

		public ItemsConsumableController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
		{
			_context = context;
			_webHostEnvirontment = hostEnvironment;
		}
		/////// fungsi search
		// GET: Items
		public async Task<IActionResult> Index(string SearchString)
		{
			if (!String.IsNullOrEmpty(SearchString))
			{
				var itemsConsumable = await Search(SearchString);
				return View(itemsConsumable);
			}

			var allItemsConsumable = await _context.ItemsConsumable
				.Include(i => i.Category)
				.Include(i => i.Supplier)
				.Include(i => i.SubCategory)
				.ToListAsync();

			return View(allItemsConsumable);
		}
		
		public async Task<List<ItemConsumable>> Search(string searchString)
		{
			var itemsConsumable = await _context.ItemsConsumable
				.Include(i => i.Category)
				.Include(i => i.Supplier)
				.Include(i => i.SubCategory)
				.Where(s => s.Name != null && s.Name.ToLower().Contains(searchString.ToLower()) ||
				s.Description!.ToLower().Contains(searchString.ToLower())
				|| s.Category.CategoryName!.ToLower().Contains(searchString.ToLower()) ||
				s.SubCategory.SubCategoryName!.ToLower().Contains(searchString.ToLower()))
				.ToListAsync();

			return itemsConsumable;
		}

		
		//GET: ItemsConsumable/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if(id == null || _context.ItemsConsumable == null)
			{
				return NotFound();
			}
			
			var itemConsumable = await _context.ItemsConsumable
				.Include(i => i.Category)
				.Include(i => i.SubCategory)
				.Include(i => i.Supplier)
				.FirstOrDefaultAsync(m => m.IdItemConsumable == id);
			if(itemConsumable == null)
			{
				return NotFound();
			}
			
			return View(itemConsumable);
		}
		
		// GET: Items/Create
		[Authorize(Roles = "Admin")]
		public IActionResult Create()
		{

			var itemConsumableViewModel = new ItemConsumableViewModel //untuk tampilan web
			{

				CreateAt = DateTime.Now,
				Quantity = 1

			};

			ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName");
			ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode");
			ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
			return View(itemConsumableViewModel);
		}
		
		// POST: ItemsConsumable/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(ItemConsumableViewModel itemConsumableViewModel)
		{
			if (ModelState.IsValid)
			{
				var uniqueFileName = await UploadFile(itemConsumableViewModel);
				var newItemConsumable = new ItemConsumable //untuk simpan ke database
				{
					Name = itemConsumableViewModel.Name,
					KodeItemConsumable = GenerateItemCode(itemConsumableViewModel.CategoryId, itemConsumableViewModel.SubCategoryId),
					PicturePath = uniqueFileName,
					Description = itemConsumableViewModel.Description,
					Availability = itemConsumableViewModel.Availability,
					CategoryId = itemConsumableViewModel.CategoryId,
					SubCategoryId = itemConsumableViewModel.SubCategoryId,
					SupplierId = itemConsumableViewModel.SupplierId,
					CreateAt = DateTime.Now,
					Quantity = itemConsumableViewModel.Quantity
				};

				_context.Add(newItemConsumable);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName", itemConsumableViewModel.CategoryId);
			ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", itemConsumableViewModel.SubCategoryId);
			ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", itemConsumableViewModel.SupplierId);
			return View(itemConsumableViewModel);
		}
		
		private string GenerateItemCode(int categoryId, int subCategoryId)
		{
			var categoryCode = _context.Categories.FirstOrDefault(c => c.IdCategory == categoryId)?.CategoryCode;
			var subCategoryCode = _context.SubCategories.FirstOrDefault(s => s.IdSubCategory == subCategoryId)?.SubCategoryCode;

			int lastIdValue;
			var tableName = _context.Model.FindEntityType(typeof(Item)).GetTableName();
			using (var connection = _context.Database.GetDbConnection() as SqliteConnection)
			{
				connection.Open();

				var command = connection.CreateCommand();
				command.CommandText = $"SELECT seq FROM sqlite_sequence WHERE name='{tableName}';";
				var result = command.ExecuteScalar();

				lastIdValue = result != null ? Convert.ToInt32(result) : 0;

			}
			var itemCode = $"{categoryCode}-{subCategoryCode}-{lastIdValue}";
			return itemCode;
		}
		
		public IActionResult GetSubcategoriesByCategory(int categoryId)
		{
			var subcategories = _context.SubCategories.Where(s => s.CategoryId == categoryId).Select(s => new { value = s.IdSubCategory, text = s.SubCategoryName }).ToList();
			return Json(subcategories);
		}
		
		//method for processing the image upload to folder
		private async Task<String?> UploadFile(ItemConsumableViewModel itemConsumableViewModel)
		{
			//process the uploaded file
			//example : save file to a directory
			if (itemConsumableViewModel.Picture != null && itemConsumableViewModel.Picture.Length > 0)
			{
				string fileName = GetUniqueFileName(itemConsumableViewModel.Picture.FileName);

				string filePath = Path.Combine(_webHostEnvirontment.WebRootPath, "uploads", fileName);
				//webhostenvirontment itu get alamat di wwroot untuk set alaamt image yg di upload agar di save ke wwroot
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await itemConsumableViewModel.Picture.CopyToAsync(fileStream);
				}
				return fileName;
			}
			return null;
		}

		private string GetUniqueFileName(string fileName)
		{
			//Generate a unique file name using a combination of timestamp and original file name
			string uniqueFileName = Path.GetFileNameWithoutExtension(fileName) +
			"_" + DateTime.Now.Ticks.ToString() + Path.GetExtension(fileName);

			return uniqueFileName;
		}
		
		 // GET: ItemsConsumable/Edit/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.ItemsConsumable == null)
			{
				return NotFound();
			}

			var itemConsumable = await _context.ItemsConsumable.FindAsync(id);
			if (itemConsumable == null)
			{
				return NotFound();
			}

			var itemConsumableViewModel = new ItemConsumableViewModel()
			{
				IdItemConsumable = itemConsumable.IdItemConsumable,
				Name = itemConsumable.Name,
				KodeItemConsumable = itemConsumable.KodeItemConsumable,
				PicturePath = itemConsumable.PicturePath,
				Description = itemConsumable.Description,
				CreateAt = itemConsumable.CreateAt,
				Availability = itemConsumable.Availability,
				CategoryId = itemConsumable.CategoryId,
				SubCategoryId = itemConsumable.SubCategoryId,
				SupplierId = itemConsumable.SupplierId,
				Quantity = itemConsumable.Quantity
			};

			ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName", itemConsumable.CategoryId);
			ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", itemConsumable.SubCategoryId);
			ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", itemConsumable.SupplierId);
			return View(itemConsumableViewModel);
		}
		
		// POST: ItemsConsumable/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Edit(int id, ItemConsumableViewModel itemConsumableViewModel)
		{
			if (id != itemConsumableViewModel.IdItemConsumable)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					var uniqueFileName = await UploadFile(itemConsumableViewModel);
					var newItemConsumable = new ItemConsumable //untkuk database
					{
						IdItemConsumable = itemConsumableViewModel.IdItemConsumable,
						Name = itemConsumableViewModel.Name,
						KodeItemConsumable = itemConsumableViewModel.KodeItemConsumable,
						PicturePath = !string.IsNullOrEmpty(uniqueFileName) ? uniqueFileName : itemConsumableViewModel.PicturePath,
						Description = itemConsumableViewModel.Description,
						CreateAt = itemConsumableViewModel.CreateAt,
						Availability = itemConsumableViewModel.Availability,
						CategoryId = itemConsumableViewModel.CategoryId,
						SubCategoryId = itemConsumableViewModel.SubCategoryId,
						SupplierId = itemConsumableViewModel.SupplierId,
						Quantity = itemConsumableViewModel.Quantity

					};

					_context.Update(newItemConsumable);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ItemExists(itemConsumableViewModel.IdItemConsumable))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName", itemConsumableViewModel.CategoryId);
			ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", itemConsumableViewModel.SubCategoryId);
			ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", itemConsumableViewModel.SupplierId);
			return View(itemConsumableViewModel);
		}
		
		// GET: Items/Delete/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.ItemsConsumable == null)
			{
				return NotFound();
			}

			var itemConsumable = await _context.ItemsConsumable
				.Include(i => i.Category)
				.Include(i => i.SubCategory)
				.Include(i => i.Supplier)
				.FirstOrDefaultAsync(m => m.IdItemConsumable == id);
			if (itemConsumable == null)
			{
				return NotFound();
			}

			return View(itemConsumable);
		}
		
				// POST: Items/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.ItemsConsumable == null)
			{
				return Problem("Entity set 'ApplicationDbContext.ItemsConsumable'  is null.");
			}
			var itemConsumable = await _context.ItemsConsumable.FindAsync(id);
			if (itemConsumable != null)
			{
				_context.ItemsConsumable.Remove(itemConsumable);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}
	
		private bool ItemExists(int id)
		{
			return (_context.ItemsConsumable?.Any(e => e.IdItemConsumable == id)).GetValueOrDefault();
		}
	}
}