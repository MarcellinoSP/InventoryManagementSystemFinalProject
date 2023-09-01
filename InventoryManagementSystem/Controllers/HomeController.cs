using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InventoryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using InventoryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Google.Apis.Auth.OAuth2;

namespace InventoryManagementSystem.Controllers;
[Authorize]
public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly UserManager<User> _userManager;
	private readonly ApplicationDbContext _context;

	public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationDbContext context)
	{
		_logger = logger;
		_userManager = userManager;
		_context = context;
	}

	public IActionResult Index()
	{
		var userId = _userManager.GetUserId(User);
		var totalRequestsItems = _context.RequestItems.Where(c => c.UserId == userId).Count();
		var totalOrderItems = _context.OrderItems.Where(c => c.UserId == userId).Count();
		var totalBorrowable = _context.BorrowedItems.Where(c => c.UserId == userId).Count();
		var totalGoodReceipt = _context.GoodReceipts.Where(c => c.UserId == userId).Count();
		var borrowedItems = _context.BorrowedItems.Where(c => c.Status == BorrowedItemStatus.StillBorrowed);
		var requestedItems = _context.RequestItems.Where(c => c.Status == RequestItemStatus.WaitingApproval);
		var lostItems = _context.BorrowedItems.Where(c => c.Status == BorrowedItemStatus.DoneAndLost);

		if (User.IsInRole("Admin"))
		{
			totalRequestsItems = _context.RequestItems.Count();
			totalOrderItems = _context.OrderItems.Count();
			totalBorrowable = _context.BorrowedItems.Count();
			totalGoodReceipt = _context.GoodReceipts.Count();
			borrowedItems = _context.BorrowedItems
							.Include(b => b.Item)
							.Where(c => c.Status == BorrowedItemStatus.StillBorrowed);
			requestedItems = _context.RequestItems
							.Include(b => b.Item)
							.Where(c => c.Status == RequestItemStatus.WaitingApproval);
			lostItems = _context.BorrowedItems
						.Include(b => b.Item)
						.Where(c => c.Status == BorrowedItemStatus.DoneAndLost);
		}

		ViewBag.TotalRequestsBorrow = totalRequestsItems;
		ViewBag.TotalOrderItems = totalOrderItems;
		ViewBag.totalBorrowable = totalBorrowable;
		ViewBag.TotalGoodReceipt = totalGoodReceipt;
		ViewBag.BorrowedItems = borrowedItems;
		ViewBag.RequestedItems = requestedItems;
		ViewBag.LostItems = lostItems;
		return View();
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
