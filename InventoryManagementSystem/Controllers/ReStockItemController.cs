using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Controllers
{
	public class ReStockItemController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ReStockItemController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: ReStockItem
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.ReStockItem
										.Include(r => r.ItemConsumable);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: ReStockItem/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.ReStockItem == null)
			{
				return NotFound();
			}

			var reStockItem = await _context.ReStockItem
				.Include(r => r.ItemConsumable)
				.FirstOrDefaultAsync(m => m.ReStockID == id);
			if (reStockItem == null)
			{
				return NotFound();
			}

			return View(reStockItem);
		}

		// GET: ReStockItem/Create
		public IActionResult Create()
		{
			ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "Name");
			return View();
		}
		
		private async Task<List<ReStockItem>> GetData()
		{
			return await _context.ReStockItem
			.Include(c => c.ItemConsumable)
			.ToListAsync();
		}

		// POST: ReStockItem/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ReStockID,ItemConsumableId,KodeItemConsumable,RequestStockDate,Quantity,Status")] ReStockItem reStockItem)
		{
			if (ModelState.IsValid)
			{
				_context.Add(reStockItem);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "Name", reStockItem.ItemConsumableId);
			return View(reStockItem);
		}

		// GET: ReStockItem/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.ReStockItem == null)
			{
				return NotFound();
			}

			var reStockItem = await _context.ReStockItem.FindAsync(id);
			if (reStockItem == null)
			{
				return NotFound();
			}
			ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "Name", reStockItem.ItemConsumableId);
			return View(reStockItem);
		}

		// POST: ReStockItem/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ReStockID,ItemConsumableId,KodeItemConsumable,RequestStockDate,Quantity,Status")] ReStockItem reStockItem)
		{
			if (id != reStockItem.ReStockID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(reStockItem);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ReStockItemExists(reStockItem.ReStockID))
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
			ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "Name", reStockItem.ItemConsumableId);
			return View(reStockItem);
		}

		// GET: ReStockItem/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.ReStockItem == null)
			{
				return NotFound();
			}

			var reStockItem = await _context.ReStockItem
				.Include(r => r.ItemConsumable)
				.FirstOrDefaultAsync(m => m.ReStockID == id);
			if (reStockItem == null)
			{
				return NotFound();
			}

			return View(reStockItem);
		}

		// POST: ReStockItem/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.ReStockItem == null)
			{
				return Problem("Entity set 'ApplicationDbContext.ReStockItem'  is null.");
			}
			var reStockItem = await _context.ReStockItem.FindAsync(id);
			if (reStockItem != null)
			{
				_context.ReStockItem.Remove(reStockItem);
			}
			
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ReStockItemExists(int id)
		{
		  return (_context.ReStockItem?.Any(e => e.ReStockID == id)).GetValueOrDefault();
		}
	}
}
