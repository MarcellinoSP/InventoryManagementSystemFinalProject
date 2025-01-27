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
using System.IO;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using InventoryManagementSystem.Repositories;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class ItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvirontment;
        private readonly IUnitOfWork _unitOfWork;

        public ItemsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, IUnitOfWork unitOfWork)
        {
            _context = context;
            _webHostEnvirontment = hostEnvironment;
            _unitOfWork = unitOfWork;
        }
        // GET: Items
        public async Task<IActionResult> Index(string SearchString)
        {
            if (!String.IsNullOrEmpty(SearchString))
            {
                var items = await Search(SearchString);
                return View(items);
            }

            var allItems = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.SubCategory)
                .ToListAsync();

            return View(allItems);
        }

        public async Task<List<Item>> Search(string searchString)
        {
            var items = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .Include(i => i.SubCategory)
                .Where(s => s.Name != null && s.Name.ToLower().Contains(searchString.ToLower()) ||
                s.Description!.ToLower().Contains(searchString.ToLower())
                || s.Category.CategoryName!.ToLower().Contains(searchString.ToLower()) ||
                s.SubCategory.SubCategoryName!.ToLower().Contains(searchString.ToLower()))
                .ToListAsync();

            return items;
        }//.Where(p => p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm))

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.SubCategory)
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(m => m.IdItem == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {

            var item = new ItemViewModel //untuk tampilan web
            {

                CreateAt = DateTime.Now,

            };

            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName");
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
            return View(item);
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ItemViewModel itemViewModel)
        {
            if (ModelState.IsValid)
            {
                var uniqueFileName = await UploadFile(itemViewModel);
                var newItem = new Item //untuk simpan ke database
                {
                    Name = itemViewModel.Name,
                    KodeItem = GenerateItemCode(itemViewModel.CategoryId, itemViewModel.SubCategoryId),
                    PicturePath = uniqueFileName,
                    Description = itemViewModel.Description,
                    Availability = itemViewModel.Availability,
                    CategoryId = itemViewModel.CategoryId,
                    SubCategoryId = itemViewModel.SubCategoryId,
                    SupplierId = itemViewModel.SupplierId,
                    CreateAt = DateTime.Now,
                };

                _context.Add(newItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName", itemViewModel.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", itemViewModel.SubCategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", itemViewModel.SupplierId);
            return View(itemViewModel);
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


        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Create(ItemViewModel itemViewModel)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         _context.Add(itemViewModel);
        //         await _context.SaveChangesAsync();
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode", itemViewModel.CategoryId);
        //     ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", itemViewModel.SubCategoryId);
        //     ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", itemViewModel.SupplierId);
        //     return View(itemViewModel);
        // }

        //method for processing the image upload to folder
        private async Task<String?> UploadFile(ItemViewModel itemViewModel)
        {
            //process the uploaded file
            //example : save file to a directory
            if (itemViewModel.Picture != null && itemViewModel.Picture.Length > 0)
            {
                string fileName = GetUniqueFileName(itemViewModel.Picture.FileName);

                string filePath = Path.Combine(_webHostEnvirontment.WebRootPath, "uploads", fileName);
                //webhostenvirontment itu get alamat di wwroot untuk set alaamt image yg di upload agar di save ke wwroot
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await itemViewModel.Picture.CopyToAsync(fileStream);
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

        // GET: Items/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var itemViewModel = new ItemViewModel()
            {
                IdItem = item.IdItem,
                Name = item.Name,
                KodeItem = item.KodeItem,
                PicturePath = item.PicturePath,
                Description = item.Description,
                CreateAt = item.CreateAt,
                Availability = item.Availability,
                CategoryId = item.CategoryId,
                SubCategoryId = item.SubCategoryId,
                SupplierId = item.SupplierId,
            };

            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName", item.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", item.SubCategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", item.SupplierId);
            return View(itemViewModel);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, ItemViewModel itemViewModel)
        {
            if (id != itemViewModel.IdItem)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var uniqueFileName = await UploadFile(itemViewModel);
                    var newItem = new Item //untkuk database
                    {
                        IdItem = itemViewModel.IdItem,
                        Name = itemViewModel.Name,
                        KodeItem = itemViewModel.KodeItem,
                        PicturePath = !string.IsNullOrEmpty(uniqueFileName) ? uniqueFileName : itemViewModel.PicturePath,
                        Description = itemViewModel.Description,
                        CreateAt = itemViewModel.CreateAt,
                        Availability = itemViewModel.Availability,
                        CategoryId = itemViewModel.CategoryId,
                        SubCategoryId = itemViewModel.SubCategoryId,
                        SupplierId = itemViewModel.SupplierId,

                    };
                    _context.Update(newItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(itemViewModel.IdItem))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Message"] = "data success updated";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryName", itemViewModel.CategoryId);
            ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", itemViewModel.SubCategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", itemViewModel.SupplierId);
            return View(itemViewModel);
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(int id, [Bind("IdItem,KodeItem,Name,Description,Availability,CategoryId,SubCategoryId,SupplierId")] Item item)
        // {
        //     if (id != item.IdItem)
        //     {
        //         return NotFound();
        //     }

        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(item);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!ItemExists(item.IdItem))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode", item.CategoryId);
        //     ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", item.SubCategoryId);
        //     ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", item.SupplierId);
        //     return View(item);
        // }

        // GET: Items/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Items == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .Include(i => i.Category)
                .Include(i => i.SubCategory)
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(m => m.IdItem == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Items == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Items'  is null.");
            }
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return (_context.Items?.Any(e => e.IdItem == id)).GetValueOrDefault();
        }

        // POST: Items/UploadCsv/5
        [HttpPost, ActionName("UploadCsv")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadCsv(IFormFile file, [FromServices] IUnitOfWork unitOfWork)
        {
            if (file != null && file.Length > 0)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    var config = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.CurrentCulture)
                    {
                        HeaderValidated = null,
                        MissingFieldFound = null,
                    };
                    using (var csv = new CsvReader(reader, config))
                    {
                        var items = csv.GetRecords<Item>().ToList();

                        foreach (var item in items)
                        {
                            var existingCategory = await unitOfWork.CategoryRepository.GetByCodeAndName(item.Category.CategoryCode, item.Category.CategoryName);
                            if (existingCategory == null)
                            {
                                existingCategory = new Category { CategoryCode = item.Category.CategoryCode, CategoryName = item.Category.CategoryName };
                                await unitOfWork.CategoryRepository.Add(existingCategory);
                            }

                            var existingSubCategory = await unitOfWork.SubCategoryRepository.GetByCodeAndName(item.SubCategory.SubCategoryCode, item.SubCategory.SubCategoryName);
                            if (existingSubCategory == null)
                            {
                                existingSubCategory = new SubCategory { SubCategoryCode = item.SubCategory.SubCategoryCode, SubCategoryName = item.SubCategory.SubCategoryName, CategoryId = existingCategory.IdCategory };
                                await unitOfWork.SubCategoryRepository.Add(existingSubCategory);
                            }

                            var existingSupplier = await unitOfWork.SupplierRepository.GetByNameAndContactNumber(item.Supplier.CompanyName, item.Supplier.ContactNumber);
                            if (existingSupplier == null)
                            {
                                existingSupplier = new Supplier { CompanyName = item.Supplier.CompanyName, ContactNumber = item.Supplier.ContactNumber, Address = item.Supplier.Address, EmailCompany = item.Supplier.EmailCompany };
                                await unitOfWork.SupplierRepository.Add(existingSupplier);
                            }

                            var newItem = new Item
                            {
                                KodeItem = item.KodeItem,
                                Name = item.Name,
                                PicturePath = item.PicturePath,
                                Description = item.Description,
                                Availability = item.Availability,
                                CategoryId = existingCategory.IdCategory,
                                SubCategoryId = existingSubCategory.IdSubCategory,
                                SupplierId = existingSupplier.SupplierId,
                                CreateAt = item.CreateAt,
                            };

                            await unitOfWork.ItemRepository.Add(newItem);
                        }

                        await unitOfWork.Commit();
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
