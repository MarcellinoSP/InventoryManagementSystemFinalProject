using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using CsvHelper;
using System.Globalization;

namespace InventoryManagementSystem.Controllers
{
	[Authorize]
	public class RequestItemsConsumableController : Controller
	{
		private readonly ApplicationDbContext _context;

		private readonly UserManager<User> _userManager;

		public RequestItemsConsumableController(ApplicationDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}


		[Authorize(Roles = "Admin,Employee")]
		// GET: RequestItemsConsumable

		public async Task<IActionResult> Index(string? SearchString)
		{
			if (!String.IsNullOrEmpty(SearchString))
			{
				var RequestItemsConsumable = await Search(SearchString);
				return View(RequestItemsConsumable);
			}

			List<RequestItemConsumable> allRequestItems = await GetAllDataFromDatabase();

			if (User.IsInRole("Employee"))
			{
				var userId = _userManager.GetUserId(User);
				allRequestItems = allRequestItems.Where(w => w.UserId == userId).ToList();

			}
			return View(allRequestItems);
		}

		private async Task<List<RequestItemConsumable>> GetAllDataFromDatabase()
		{
			return await _context.RequestItemsConsumable
			.Include(c => c.ItemConsumable)
			.Include(c => c.User)
			.ToListAsync();
			// show all rows in items table
		}

		public async Task<List<RequestItemConsumable>> Search(string searchString)
		{
			var requestItemConsumable = await _context.RequestItemsConsumable
			.Include(c => c.ItemConsumable)
			.Include(c => c.User)
			.Where(
				s => s.ItemConsumable!.Name!.ToLower().Contains(searchString.ToLower()) ||
				s.ItemConsumable!.KodeItemConsumable!.ToLower().Contains(searchString.ToLower()) ||
				s.User!.UserName!.ToLower().Contains(searchString.ToLower()) ||
				s.User!.Email!.ToLower().Contains(searchString.ToLower())
			).ToListAsync();

			if (User.IsInRole("Employee"))
			{
				var userId = _userManager.GetUserId(User);
				requestItemConsumable = requestItemConsumable.Where(w => w.UserId == userId).ToList();
			}

			return requestItemConsumable;
		}

		// GET: RequestItems/Details/5
		[Authorize(Roles = "Admin,Employee")]
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.RequestItemsConsumable == null)
			{
				return NotFound();
			}

			var requestItemConsumable = await _context.RequestItemsConsumable
				.Include(r => r.ItemConsumable)
				.Include(r => r.User)
				.FirstOrDefaultAsync(m => m.RequestConsumableId == id);
			if (requestItemConsumable == null)
			{
				return NotFound();
			}

			return View(requestItemConsumable);
		}

		// GET: RequestItemsConsumable/Create
		[Authorize(Roles = "Employee")]
		public IActionResult Create(int? itemConsumableId)
		{

			if (User.IsInRole("Admin"))
			{
				ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "KodeItemConsumable");
				ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
				return View();
			}

			else
			{
				if (itemConsumableId == null) return NotFound();
				ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable
				.Where(c => c.IdItemConsumable == itemConsumableId), "IdItemConsumable", "KodeItemConsumable");
				// Console.WriteLine(ViewData["ItemConsumableId"]);
				var itemConsumable = _context.ItemsConsumable.Where(c => c.IdItemConsumable == itemConsumableId).FirstOrDefault();
				// var quantity = _context.ItemsConsumable;
				//  ViewData["UserId"] = _userManager.GetUserId(User);

				var requestItemConsumable = new RequestItemConsumable
				{
					ItemConsumable = itemConsumable,
					ItemConsumableId = (int)itemConsumableId!,
					UserId = _userManager.GetUserId(User)!,
					RequestConsumeDate = DateTime.Now,

				};

				return View(requestItemConsumable);

			}
		}


		// POST: RequestItemsConsumable/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize(Roles = "Employee")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("RequestConsumableId,ItemConsumableId,UserId,CreateAt,RequestConsumeDate,Quantity,NoteRequest,Status")] RequestItemConsumable requestItemConsumable)
		{

			if (ModelState.IsValid)
			{
				var itemConsumable = _context.ItemsConsumable.Where(c => c.IdItemConsumable == requestItemConsumable.ItemConsumableId).FirstOrDefault();
				if (itemConsumable == null)
				{
					return NotFound();
				}
				itemConsumable.Availability = false;
				_context.Update(itemConsumable);
				// _context.Add(requestItemConsumable);
				await _context.SaveChangesAsync();

				//Kurang Quantity di ItemConsumable
				// var itemConsumable2 = await _context.ItemsConsumable.FindAsync(requestItemConsumable.ItemConsumableId);

				if (requestItemConsumable.Quantity > itemConsumable.Quantity)
				{
					ModelState.AddModelError(string.Empty, "The quantity of demand exceeds what is available");
					ViewData["ItemId"] = new SelectList(_context.ItemsConsumable, "IdItemconsumable", "KodeItemConsumable", requestItemConsumable.ItemConsumableId);
					ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", requestItemConsumable.UserId);
					return View(requestItemConsumable);
				}

				//Buat objek baru tiap request
				var newRequestItemConsumable = new RequestItemConsumable
				{
					ItemConsumableId = requestItemConsumable.ItemConsumableId,
					UserId = requestItemConsumable.UserId,
					CreateAt = DateTime.Now,
					RequestConsumeDate = requestItemConsumable.RequestConsumeDate,
					Quantity = requestItemConsumable.Quantity,
					NoteRequest = requestItemConsumable.NoteRequest,
					Status = requestItemConsumable.Status
				};

				itemConsumable.Quantity -= requestItemConsumable.Quantity;
				itemConsumable.Availability = itemConsumable.Quantity > 0;
				_context.Update(itemConsumable);
				_context.Add(newRequestItemConsumable);
				await _context.SaveChangesAsync();

				return RedirectToAction(nameof(Index));
			}
			ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "KodeItemConsumable", requestItemConsumable.ItemConsumableId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", requestItemConsumable.UserId);
			return View(requestItemConsumable);
		}

		// 	public async Task<IActionResult> Create([Bind("RequestConsumableId,ItemConsumableId,UserId,CreateAt,RequestConsumeDate,Quantity,NoteRequest,Status")] RequestItemConsumable requestItemConsumable)
		// {

		// 	if (ModelState.IsValid)
		// 	{
		// 		var itemConsumable = _context.ItemsConsumable.Where(c => c.IdItemConsumable == requestItemConsumable.ItemConsumableId).FirstOrDefault();
		// 		if (itemConsumable == null)
		// 		{
		// 			return NotFound();
		// 		}
		// 		itemConsumable.Availability = false;
		// 		_context.Update(itemConsumable);
		// 		_context.Add(requestItemConsumable);
		// 		await _context.SaveChangesAsync();

		// 		//Kurang Quantity di ItemConsumable
		// 		var itemConsumable2 = await _context.ItemsConsumable.FindAsync(requestItemConsumable.ItemConsumableId);

		// 		if(requestItemConsumable.Quantity > itemConsumable2.Quantity)
		// 		{
		// 			ModelState.AddModelError(string.Empty, "The quantity of demand exceeds what is available");
		// 			ViewData["ItemId"] = new SelectList(_context.ItemsConsumable, "IdItemconsumable", "KodeItemConsumable", requestItemConsumable.ItemConsumableId);
		// 			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", requestItemConsumable.UserId);
		// 			return View(requestItemConsumable);
		// 		}

		// 		itemConsumable2.Quantity -= requestItemConsumable.Quantity;
		// 		_context.Update(itemConsumable2);
		// 		await _context.SaveChangesAsync();

		// 		return RedirectToAction(nameof(Index));
		// 	}
		// 	ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "KodeItemConsumable", requestItemConsumable.ItemConsumableId);
		// 	ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", requestItemConsumable.UserId);
		// 	return View(requestItemConsumable);
		// }

		// GET: RequestItems/Edit/5
		[Authorize(Roles = "Employee")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null || _context.RequestItemsConsumable == null)
			{
				return NotFound();
			}

			var requestItemConsumable = await _context.RequestItemsConsumable.Include(c => c.ItemConsumable).FirstOrDefaultAsync(a => a.RequestConsumableId == id);
			if (requestItemConsumable == null)
			{
				return NotFound();
			}
			ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "KodeItemConsumable", requestItemConsumable.ItemConsumableId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", requestItemConsumable.UserId);
			return View(requestItemConsumable);

		}

		// GET: RequestItems/Edit/5
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Reject(int? id)
		{
			if (id == null || _context.RequestItemsConsumable == null)
			{
				return NotFound();
			}

			var requestItemConsumable = await _context.RequestItemsConsumable.Include(c => c.ItemConsumable).FirstOrDefaultAsync(a => a.RequestConsumableId == id);
			if (requestItemConsumable == null)
			{
				return NotFound();
			}
			ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "KodeItemConsumable", requestItemConsumable.ItemConsumableId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", requestItemConsumable.UserId);
			return View(requestItemConsumable);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Reject(int id, [Bind("RequestConsumableId,ItemConsumableId,UserId,CreateAt,RequestConsumeDate,NoteRequest,NoteActionRequest,Quantity,Status")] RequestItemConsumable requestItemConsumable)
		{
			var itemConsumable = _context.ItemsConsumable.Where(c => c.IdItemConsumable == requestItemConsumable.ItemConsumableId).FirstOrDefault();
			if (id != requestItemConsumable.RequestConsumableId || itemConsumable == null)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{ //di dalam try karena default code generator ada di sini. baiknya, jika database tb tb disconnect, maka bisa nangkep error
					itemConsumable.Availability = true; //untuk ubah status avalability pada item jadi true
					_context.Update(itemConsumable);
					await _context.SaveChangesAsync();


					requestItemConsumable.Status = RequestItemConsumableStatus.Rejected;
					_context.Update(requestItemConsumable);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!RequestItemConsumableExists(requestItemConsumable.RequestConsumableId))
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
			ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "KodeItemConsumable", requestItemConsumable.ItemConsumableId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", requestItemConsumable.UserId);
			return View(requestItemConsumable);
		}

		// POST: RequestItems/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Employee,Admin")]
		public async Task<IActionResult> Edit(int id, [Bind("RequestConsumableId,ItemConsumableId,UserId,CreateAt,RequestConsumeDate,Quantity,NoteRequest,NoteActionRequest,Status")] RequestItemConsumable requestItemConsumable)
		{
			if (id != requestItemConsumable.RequestConsumableId)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(requestItemConsumable);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!RequestItemConsumableExists(requestItemConsumable.RequestConsumableId))
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
			ViewData["ItemConsumableId"] = new SelectList(_context.ItemsConsumable, "IdItemConsumable", "KodeItemConsumable", requestItemConsumable.ItemConsumableId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", requestItemConsumable.UserId);
			return View(requestItemConsumable);
		}

		// GET: RequestItems/Delete/5
		[Authorize(Roles = "Employee")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.RequestItemsConsumable == null)
			{
				return NotFound();
			}

			var requestItemConsumable = await _context.RequestItemsConsumable
				.Include(r => r.ItemConsumable)
				.Include(r => r.User)
				.FirstOrDefaultAsync(m => m.RequestConsumableId == id);
			if (requestItemConsumable == null)
			{
				return NotFound();
			}

			return View(requestItemConsumable);
		}

		// POST: RequestItems/Delete/5
		[Authorize(Roles = "Employee")]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.RequestItemsConsumable == null)
			{
				return Problem("Entity set 'ApplicationDbContext.RequestItemsConsumable'  is null.");
			}
			var requestItemConsumable = await _context.RequestItemsConsumable.FindAsync(id);
			if (requestItemConsumable != null)
			{
				_context.RequestItemsConsumable.Remove(requestItemConsumable);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		[Authorize(Roles = "Employee")]
		[HttpPost, ActionName("Cancel")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Cancel(int id)
		{
			if (_context.RequestItemsConsumable == null)
			{
				return Problem("Entity set 'ApplicationDbContext.RequestItemsConsumable'  is null.");
			}
			var requestItemConsumable = await _context.RequestItemsConsumable.FindAsync(id);


			if (requestItemConsumable == null)
			{
				return NotFound();
			}
			requestItemConsumable.Status = RequestItemConsumableStatus.Cancel;
			try
			{
				_context.Update(requestItemConsumable);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException)
			{
				throw;
			}
			return RedirectToAction(nameof(Index));
		}

		private bool RequestItemConsumableExists(int id)
		{
			return (_context.RequestItemsConsumable?.Any(e => e.RequestConsumableId == id)).GetValueOrDefault();
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public IActionResult ExportToCsv(string searchString)
		{
			var requestItemsConsumable = _context.RequestItemsConsumable
				.Include(r => r.ItemConsumable)
				.Include(r => r.User)
				.ToList();

			if (!string.IsNullOrEmpty(searchString))
			{
				requestItemsConsumable = requestItemsConsumable
					.Where(r => r.ItemConsumable != null && r.ItemConsumable.Name.ToLower().Contains(searchString.ToLower()))
					.ToList();
			}

			// Membuat StringWriter untuk menulis data CSV
			using (var sw = new StringWriter())
			{
				using (var csvWriter = new CsvWriter(sw, CultureInfo.InvariantCulture))
				{
					// Menulis header kolom
					csvWriter.WriteHeader<RequestItemConsumable>();

					csvWriter.NextRecord();

					// Menulis data baris
					csvWriter.WriteRecords(requestItemsConsumable);
				}

				// Mengatur header respons HTTP untuk file CSV
				Response.Headers.Add("Content-Disposition", "attachment; filename=request_items_consumable.csv");
				Response.ContentType = "text/csv";

				// Menulis data CSV ke respons HTTP
				return Content(sw.ToString());
			}
		}

	}
}