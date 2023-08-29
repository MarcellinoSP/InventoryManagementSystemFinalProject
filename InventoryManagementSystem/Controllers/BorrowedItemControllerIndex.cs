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
    public partial class BorrowedItemsController : Controller
    {
        [Authorize]
        // GET: BorrowedItems that losted
        public async Task<IActionResult> IndexLostItem(string? SearchString)
        {

            if (!String.IsNullOrEmpty(SearchString))
            {
                var BorrowedItems = await Search(SearchString);
                return View(BorrowedItems);
            }

            List<BorrowedItem> allLostItem = await GetAllLostItem();

            if (User.IsInRole("Employee"))
            {
                var userId = _userManager.GetUserId(User);
                allLostItem = allLostItem.Where(w => w.UserId == userId).ToList();

            }
            await _context.SaveChangesAsync();
            return View(allLostItem);
        }
        private async Task<List<BorrowedItem>> GetAllLostItem()
        {
            return await _context.BorrowedItems
            .Include(c => c.Item)
            .Include(c => c.User)
            .Where(d => d.Status == BorrowedItemStatus.DoneAndLost)
            .ToListAsync();
            // show all rows in items table
        }
        [Authorize]
        // GET: BorrowedItems that was broken
        public async Task<IActionResult> IndexBrokenItem(string? SearchString)
        {

            if (!String.IsNullOrEmpty(SearchString))
            {
                var BorrowedItems = await Search(SearchString);
                return View(BorrowedItems);
            }

            List<BorrowedItem> allBrokenItem = await GetAllBrokenItem();

            if (User.IsInRole("Employee"))
            {
                var userId = _userManager.GetUserId(User);
                allBrokenItem = allBrokenItem.Where(w => w.UserId == userId).ToList();

            }
            await _context.SaveChangesAsync();
            return View(allBrokenItem);
        }
        private async Task<List<BorrowedItem>> GetAllBrokenItem()
        {
            return await _context.BorrowedItems
            .Include(c => c.Item)
            .Include(c => c.User)
            .Where(d => d.Status == BorrowedItemStatus.DoneAndBroken)
            .ToListAsync();
            // show all rows in items table
        }
        [Authorize]
        // GET: BorrowedItems that was returned
        public async Task<IActionResult> IndexReturnedItem(string? SearchString)
        {

            if (!String.IsNullOrEmpty(SearchString))
            {
                var BorrowedItems = await Search(SearchString);
                return View(BorrowedItems);
            }

            List<BorrowedItem> allReturnedItem = await GetAllReturnedItem();

            if (User.IsInRole("Employee"))
            {
                var userId = _userManager.GetUserId(User);
                allReturnedItem = allReturnedItem.Where(w => w.UserId == userId).ToList();

            }
            await _context.SaveChangesAsync();
            return View(allReturnedItem);
        }
        private async Task<List<BorrowedItem>> GetAllReturnedItem()
        {
            return await _context.BorrowedItems
            .Include(c => c.Item)
            .Include(c => c.User)
            .Where(d => d.Status == BorrowedItemStatus.DoneBorrowing)
            .ToListAsync();
            // show all rows in items table
        }
    }
}
