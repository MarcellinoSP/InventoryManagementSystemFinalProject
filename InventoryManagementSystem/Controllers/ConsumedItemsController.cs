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
using Microsoft.AspNetCore.Identity;
using CsvHelper;
using System.Globalization;

namespace InventoryManagementSystem.Controllers;

	public class ConsumedItemsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvirontment;
		private readonly UserManager<User> _userManager;

		public ConsumedItemsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<User> userManager)
		{
			_context = context;
			_webHostEnvirontment = hostEnvironment;
			_userManager = userManager;
		}

		[Authorize]
		// Path
		// Get : ConsumedItems, searching item, .ConsumedItems di Db di DATA
		public async Task<IActionResult> Index(string? SearchString)
		{
			
			if(!String.IsNullOrEmpty(SearchString))
			{
				var ConsumableItems = await Search(SearchString);
				return View(ConsumableItems);
				
			}

			List<ConsumedItem> allConsumableItems = await GetAllDataFromDatabase();

			if(User.IsInRole("Employee"))
			{
				var userId = _userManager.GetUserId(User);
				allConsumableItems = allConsumableItems.Where(w => w.UserId == userId).ToList();
			}
			return View(allConsumableItems);
		}

		
		private async Task<List<ConsumedItem>> GetAllDataFromDatabase()
		{
			return await _context.ConsumedItems
			.Include(c => c.ItemConsumable)
			.Include(c => c.User)
			.ToListAsync();
			// show all rows in items table
		}

		public async Task<List<ConsumedItem>> Search(string searchString)
		{
			var consumableItem  = await _context.ConsumedItems
			.Include(c => c.ItemConsumable)
			.Include(c => c.User)
			.Where(
				s => s.ItemConsumable!.Name!.ToLower().Contains(searchString.ToLower()) ||
				s.ItemConsumable!.KodeItemConsumable!.ToLower().Contains(searchString.ToLower()) ||
				s.User!.UserName!.ToLower().Contains(searchString.ToLower()) ||
				s.User!.Email!.ToLower().Contains(searchString.ToLower())
			).ToListAsync();

			if(User.IsInRole("Employee"))
			{
				var userId = _userManager.GetUserId(User);
				consumableItem = consumableItem.Where(w => w.UserId == userId).ToList();
			}

			 return consumableItem;
		}
		
		// GET: ConsumableItems/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			
			if(id == null || _context.ConsumedItems == null)
			{
				return NotFound();
			}
			
			var consumableItem = await _context.ConsumedItems
				.Include(c => c.ItemConsumable)
				.Include(c => c.OrderItemConsumable)
				.Include(c => c.User)
				.FirstOrDefaultAsync(m => m.ConsumedId == id);
				
			if(consumableItem == null)
			{
				return NotFound();
			}
			
			return View(consumableItem);
		}
		
		//GET : ConsumableItems/Create
		public IActionResult Create(int orderId)
		
		{
			var orderItem = _context.OrderItemsConsumable
			.Include(c => c.ItemConsumable).Include(d => d.User)
			.Where(d => d.OrderConsumableId == orderId)
			.FirstOrDefault();
			
			if(orderItem == null)
			{
				return NotFound();
			}
			
			var consumableItem = new ConsumedItemViewModel()
			{
				OrderItemConsumable = orderItem,
				OrderId = orderItem.OrderConsumableId,
				ItemConsumableId = orderItem.ItemConsumableId,
				ItemConsumable = orderItem.ItemConsumable,
				UserId = orderItem.UserId,
				User = orderItem.User,
				CreateAt = DateTime.Now,
				//Pake .BorrowDateApproved karena sama untuk satu borrow atau consumable
				ConsumedDate = orderItem.ConsumeDateApproved,
				Status = ConsumedItemStatus.Consumed
			};
			
			return View(consumableItem);
		}
		
		// POST: BorrowedItems/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(ConsumedItemViewModel consumedItemViewModel)
		{
			if(ModelState.IsValid)
			
			{
				var uniqueFileName = await UploadFile(consumedItemViewModel);
				var orderItem = _context.OrderItemsConsumable
				.Include(c => c.ItemConsumable)
				.Include(d => d.User).Where(d => d.OrderConsumableId == consumedItemViewModel.OrderId)
				.FirstOrDefault();
				
				if(orderItem == null)
				{
					return NotFound();
				}
				
				//Set ke DB
				var consumableItemAction = new ConsumedItem
				{
					OrderId = orderItem.OrderConsumableId,
					ItemConsumableId = consumedItemViewModel.ItemConsumableId,
					PicturePath = uniqueFileName,
					UserId = consumedItemViewModel.UserId,
					CreateAt = DateTime.Now,
					ConsumedDate = consumedItemViewModel.ConsumedDate,
					NoteConsumed = consumedItemViewModel.NoteConsumed,

				};

				_context.Add(consumableItemAction);
				await _context.SaveChangesAsync();

				orderItem.Status = OrderItemConsumableStatus.DonePickUp;
				orderItem.ConsumableId = consumedItemViewModel.ConsumedId;
				_context.Update(orderItem);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			
			}
			ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", consumedItemViewModel.ItemConsumableId);
			ViewData["OrderId"] = new SelectList(_context.OrderItems, "OrderId", "OrderId", consumedItemViewModel.OrderId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", consumedItemViewModel.UserId);
			return View(consumedItemViewModel);
		}
		
		private async Task<String?> UploadFile(ConsumedItemViewModel consumableItemViewModel)
		{
			//process the uploaded file
			//example : save file to a directory
			if (consumableItemViewModel.Picture != null && consumableItemViewModel.Picture.Length > 0)
			{
				string fileName = GetUniqueFileName(consumableItemViewModel.Picture.FileName);

				string filePath = Path.Combine(_webHostEnvirontment.WebRootPath, "uploads", fileName);
				//webhostenvirontment itu get alamat di wwroot untuk set alaamt image yg di upload agar di save ke wwroot
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await consumableItemViewModel.Picture.CopyToAsync(fileStream);
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

		// GET : ConsumableItems/Edit/5

		public async Task<IActionResult> Edit(int? id)
		{
			if(id == null || _context.ConsumedItems == null)
			{
				return NotFound();
			}

			var consumableItem = await _context.ConsumedItems.FindAsync(id);
			if(consumableItem == null)
			{
				return NotFound();
			}
			ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", consumableItem.ItemConsumableId);
			ViewData["OrderId"] = new SelectList(_context.OrderItems, "OrderId", "OrderId", consumableItem.OrderId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", consumableItem.UserId);
			return View(consumableItem);

		}

		// POST: BorrowedItems/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		
		public async Task<IActionResult> Edit(int id, [Bind("ConsumableId,OrderId,ReceiptId,ItemId,UserId,CreateAt,ConsumedDate,NoteConsumed,PicturePath,Status")] ConsumedItem consumedItem)
		{
			if (id != consumedItem.ConsumedId)
			{
				return NotFound();
			}

			if(ModelState.IsValid)
			{
				try
				{
					_context.Update(consumedItem);
					await _context.SaveChangesAsync();
				}
				catch(DbUpdateConcurrencyException)
				{
					if(!ConsumedItemExists(consumedItem.ConsumedId))
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
			ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem", consumedItem.ItemConsumableId);
			ViewData["OrderId"] = new SelectList(_context.OrderItems, "OrderId", "OrderId", consumedItem.OrderId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", consumedItem.UserId);
			return View(consumedItem);
		}

		private bool ConsumedItemExists(int id)
		{
			return (_context.ConsumedItems?.Any(e => e.ConsumedId == id)).GetValueOrDefault();
		}

		// GET: BorrowedItems/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.ConsumedItems == null)
			{
				return NotFound();
			}

			var consumableItem = await _context.ConsumedItems
				.Include(c => c.ItemConsumable)
				.Include(b => b.OrderItemConsumable)
				.Include(b => b.User)
				.FirstOrDefaultAsync(m => m.ConsumedId == id);
			if(consumableItem == null)
			{
				return NotFound();
			}

			return View(consumableItem);
		}

		//POST : ConsumableItems/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if(_context.ConsumedItems == null)
			{
				return Problem("Entity set 'ApplicationDbContext.ConsumableItems' is null. ");
			}

			var consumableItem = await _context.ConsumedItems.FindAsync(id);
			if(consumableItem != null)
			{
				_context.ConsumedItems.Remove(consumableItem);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public IActionResult ExportToCsv(string searchString)
		{
			var consumableItems = _context.ConsumedItems
				.Include(r => r.ItemConsumable)
				.Include(r => r.User)
				.ToList();

			if(!string.IsNullOrEmpty(searchString))
			{
				consumableItems = consumableItems
					.Where(r => r.ItemConsumable != null && r.ItemConsumable.Name.ToLower().Contains(searchString.ToLower()))
					.ToList();
			}
		
			//Membuat StringWriter untuk menulis data CSV
			using (var sw = new StringWriter())
			{
				using (var csvWriter = new CsvWriter(sw, CultureInfo.InvariantCulture))
				{
					// Menulis header kolom
					csvWriter.WriteHeader<RequestItem>();

					csvWriter.NextRecord();

					// Menulis data baris
					csvWriter.WriteRecords(consumableItems);
				}

				// Mengatur header respons HTTP untuk file CSV
				Response.Headers.Add("Content-Disposition", "attachment; filename=request_items.csv");
				Response.ContentType = "text/csv";

				// Menulis data CSV ke respons HTTP
				return Content(sw.ToString());
			}
	
		}
	}

