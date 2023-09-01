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

namespace InventoryManagementSystem.Controllers
{
	[Authorize]
	public class OrderItemsConsumableController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<User> _userManager;

		public OrderItemsConsumableController(ApplicationDbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		// GET: OrderItems
		public async Task<IActionResult> Index(string? SearchString)
		{

			if (!String.IsNullOrEmpty(SearchString))
			{
				var OrderItems = await Search(SearchString);
				return View(OrderItems);
			}

			List<OrderItemConsumable> allOrderItems = await GetAllDataFromDatabase();

			if (User.IsInRole("Employee"))
			{
				var userId = _userManager.GetUserId(User);
				allOrderItems = allOrderItems.Where(w => w.UserId == userId).ToList();

			}
			return View(allOrderItems);

		}

		private async Task<List<OrderItemConsumable>> GetAllDataFromDatabase()
		{
			return await _context.OrderItemsConsumable
			.Include(c => c.ItemConsumable)
			.Include(c => c.User)
			.ToListAsync();
			// show all rows in items table
		}

		public async Task<List<OrderItemConsumable>> Search(string searchString)
		{
			var orderItem = await _context.OrderItemsConsumable
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
				orderItem = orderItem.Where(w => w.UserId == userId).ToList();
			}

			return orderItem;
		}

		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.OrderItemsConsumable == null)
			{
				return NotFound();
			}

			var orderItem = await _context.OrderItemsConsumable
				.Include(o => o.ItemConsumable)
				.Include(o => o.User)
				.FirstOrDefaultAsync(m => m.OrderConsumableId == id);
			if (orderItem == null)
			{
				return NotFound();
			}

			return View(orderItem);
		}

		// GET: OrderItems/Create

		public IActionResult Create(int requestConsumableId, RequestItemConsumableStatus status)
		{
			var requestItemConsumable = _context.RequestItemsConsumable
			.Include(c => c.ItemConsumable)
			.Include(d => d.User)
			.Where(d => d.RequestConsumableId == requestConsumableId)
			.FirstOrDefault();

			if (requestItemConsumable == null)
			{
				return NotFound();
			}

			var orderItemConsumable = new OrderItemConsumable()
			{
				RequestItemConsumable = requestItemConsumable,
				RequestId = requestItemConsumable.RequestConsumableId,
				ItemConsumable = requestItemConsumable.ItemConsumable,
				ItemConsumableId = requestItemConsumable.ItemConsumableId,
				User = requestItemConsumable.User,
				UserId = requestItemConsumable.UserId,
				CreateAt = DateTime.Now,
				Quantity = requestItemConsumable.Quantity,
				ConsumeDateApproved = requestItemConsumable.RequestConsumeDate
			};
			ViewData["statusReqAction"] = status;

			// ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "KodeItem");
			// ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
			return View(orderItemConsumable);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("OrderConsumableId,RequestId,ItemConsumableId,UserId,CreateAt,ConsumeDateApproved,NoteDonePickUp,NoteWaitingPickUp,Quantity,Status")] OrderItemConsumable orderItemConsumable)
		{
			var requestItemConsumable = _context.RequestItemsConsumable
										.Include(c => c.User)
										.Include(c => c.ItemConsumable)
										.Where(d => d.RequestConsumableId == orderItemConsumable.RequestId)
										.FirstOrDefault();

			if (requestItemConsumable != null)
			{
				_context.Add(orderItemConsumable);
				await _context.SaveChangesAsync();

				requestItemConsumable.Status = RequestItemConsumableStatus.Approved;
				requestItemConsumable.OrderItemConsumableId = orderItemConsumable.OrderConsumableId;
				_context.Update(requestItemConsumable);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}

			ViewData["ItemId"] = new SelectList(_context.ItemsConsumable, "IdItem", "KodeItem", orderItemConsumable.ItemConsumableId);
			ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", orderItemConsumable.UserId);
			return View(requestItemConsumable);
		}

		private bool OrderItemExists(int id)
		{
			return (_context.OrderItemsConsumable?.Any(e => e.OrderConsumableId == id)).GetValueOrDefault();
		}

		[HttpGet]
		[Authorize(Roles = "Admin")]
		public IActionResult ExportToCsv(string searchString)
		{
			var orderItems = _context.OrderItemsConsumable
				.Include(r => r.ItemConsumable)
				.Include(r => r.User)
				.ToList();

			if (!string.IsNullOrEmpty(searchString))
			{
				orderItems = orderItems
					.Where(r => r.ItemConsumable != null && r.ItemConsumable.Name.ToLower().Contains(searchString.ToLower()))
					.ToList();
			}

			// Membuat StringWriter untuk menulis data CSV
			using (var sw = new StringWriter())
			{
				using (var csvWriter = new CsvWriter(sw, CultureInfo.InvariantCulture))
				{
					// Menulis header kolom
					csvWriter.WriteHeader<RequestItem>();

					csvWriter.NextRecord();

					// Menulis data baris
					csvWriter.WriteRecords(orderItems);
				}

				// Mengatur header respons HTTP untuk file CSV
				Response.Headers.Add("Content-Disposition", "attachment; filename=request_Consumeditems.csv");
				Response.ContentType = "text/csv";

				// Menulis data CSV ke respons HTTP
				return Content(sw.ToString());
			}
		}
	}
}