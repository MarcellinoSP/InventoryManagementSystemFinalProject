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

namespace InventoryManagementSystem.Controllers
{
	[Authorize]
	public class ReStockConsumableItemController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ReStockConsumableItemController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: ReStockConsumableItem
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.ReStockConsumableItems.Include(r => r.Category).Include(r => r.SubCategory).Include(r => r.Supplier);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: ReStockConsumableItem/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.ReStockConsumableItems == null)
			{
				return NotFound();
			}

			var reStockConsumableItem = await _context.ReStockConsumableItems
				.Include(r => r.Category)
				.Include(r => r.SubCategory)
				.Include(r => r.Supplier)
				.FirstOrDefaultAsync(m => m.IdItemConsumable == id);
			if (reStockConsumableItem == null)
			{
				return NotFound();
			}

			return View(reStockConsumableItem);
		}

		// GET: ReStockConsumableItem/Create
		public IActionResult Create()
		{
			ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode");
			ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode");
			ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName");
			return View();
		}

		// POST: ReStockConsumableItem/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("IdItemConsumable,KodeItemConsumable,Name,PicturePath,Description,Availability,CategoryId,SubCategoryId,CreateAt,SupplierId,Quantity,Status")] ReStockConsumableItem reStockConsumableItem)
		{
			if (ModelState.IsValid)
			{
				_context.Add(reStockConsumableItem);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode", reStockConsumableItem.CategoryId);
			ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", reStockConsumableItem.SubCategoryId);
			ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", reStockConsumableItem.SupplierId);
			return View(reStockConsumableItem);
		}

		// GET: ReStockConsumableItem/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.ReStockConsumableItems == null)
			{
				return NotFound();
			}

			var reStockConsumableItem = await _context.ReStockConsumableItems.FindAsync(id);
			if (reStockConsumableItem == null)
			{
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode", reStockConsumableItem.CategoryId);
			ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", reStockConsumableItem.SubCategoryId);
			ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", reStockConsumableItem.SupplierId);
			return View(reStockConsumableItem);
		}

		// POST: ReStockConsumableItem/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("IdItemConsumable,KodeItemConsumable,Name,PicturePath,Description,Availability,CategoryId,SubCategoryId,CreateAt,SupplierId,Quantity,Status")] ReStockConsumableItem reStockConsumableItem)
		{
			if (id != reStockConsumableItem.IdItemConsumable)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(reStockConsumableItem);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ReStockConsumableItemExists(reStockConsumableItem.IdItemConsumable))
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
			ViewData["CategoryId"] = new SelectList(_context.Categories, "IdCategory", "CategoryCode", reStockConsumableItem.CategoryId);
			ViewData["SubCategoryId"] = new SelectList(_context.SubCategories, "IdSubCategory", "SubCategoryCode", reStockConsumableItem.SubCategoryId);
			ViewData["SupplierId"] = new SelectList(_context.Suppliers, "SupplierId", "CompanyName", reStockConsumableItem.SupplierId);
			return View(reStockConsumableItem);
		}

		// GET: ReStockConsumableItem/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.ReStockConsumableItems == null)
			{
				return NotFound();
			}

			var reStockConsumableItem = await _context.ReStockConsumableItems
				.Include(r => r.Category)
				.Include(r => r.SubCategory)
				.Include(r => r.Supplier)
				.FirstOrDefaultAsync(m => m.IdItemConsumable == id);
			if (reStockConsumableItem == null)
			{
				return NotFound();
			}

			return View(reStockConsumableItem);
		}

		// POST: ReStockConsumableItem/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.ReStockConsumableItems == null)
			{
				return Problem("Entity set 'ApplicationDbContext.ReStockConsumableItems'  is null.");
			}
			var reStockConsumableItem = await _context.ReStockConsumableItems.FindAsync(id);
			if (reStockConsumableItem != null)
			{
				_context.ReStockConsumableItems.Remove(reStockConsumableItem);
			}
			
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ReStockConsumableItemExists(int id)
		{
		  return (_context.ReStockConsumableItems?.Any(e => e.IdItemConsumable == id)).GetValueOrDefault();
		}
	}
}
