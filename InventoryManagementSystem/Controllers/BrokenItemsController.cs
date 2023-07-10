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
using CsvHelper;
using System.Globalization;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class BrokenItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrokenItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BrokenItems
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BrokenItems.Include(l => l.BorrowedItem).Include(l => l.Item).Include(l => l.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BrokenItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BrokenItems == null)
            {
                return NotFound();
            }

            var brokenItem = await _context.BrokenItems
                .Include(l => l.BorrowedItem)
                .Include(l => l.Item)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.BrokenId == id);
            if (brokenItem == null)
            {
                return NotFound();
            }

            return View(brokenItem);
        }

        // GET: BrokenItems/Create
        public IActionResult Create()
        {
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId");
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "Name");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: BrokenItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BrokenId,ItemId,UserId,CreateAt,BrokenDate,NoteItemBroken,NoteItemFound,BorrowedId,Status")] BrokenItem brokenItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brokenItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId", brokenItem.BorrowedId);
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "Name", brokenItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", brokenItem.UserId);
            return View(brokenItem);
        }

        // GET: BrokenItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BrokenItems == null)
            {
                return NotFound();
            }

            var brokenItem = _context.BrokenItems.Include(c=>c.User).Where(d=>d.BrokenId==id).FirstOrDefault();
            if (brokenItem == null)
            {
                return NotFound();
            }
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId", brokenItem.BorrowedId);
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "Name", brokenItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", brokenItem.UserId);
            return View(brokenItem);
        }

        // POST: BrokenItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BrokenId,ItemId,UserId,CreateAt,BrokenDate,NoteItemBroken,NoteItemFound,BorrowedId,Status")] BrokenItem brokenItem)
        {
            if (id != brokenItem.BrokenId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(brokenItem.Status == BrokenItemStatus.Resolve){
                        var item = _context.Items.Where(c=> c.IdItem == brokenItem.ItemId).FirstOrDefault();
                        if(item == null){
                            return NotFound();
                        }

                        item.Availability = true;
                        _context.Update(item);
                        await _context.SaveChangesAsync();
                    }
                    
                    _context.Update(brokenItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrokenItemExists(brokenItem.BrokenId))
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
            ViewData["BorrowedId"] = new SelectList(_context.BorrowedItems, "BorrowedId", "BorrowedId", brokenItem.BorrowedId);
            ViewData["ItemId"] = new SelectList(_context.Items, "IdItem", "Name", brokenItem.ItemId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", brokenItem.UserId);
            return View(brokenItem);
        }

        // GET: BrokenItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BrokenItems == null)
            {
                return NotFound();
            }

            var brokenItem = await _context.BrokenItems
                .Include(l => l.BorrowedItem)
                .Include(l => l.Item)
                .Include(l => l.User)
                .FirstOrDefaultAsync(m => m.BrokenId == id);
            if (brokenItem == null)
            {
                return NotFound();
            }

            return View(brokenItem);
        }

        // POST: BrokenItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BrokenItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BrokenItems'  is null.");
            }
            var brokenItem = await _context.BrokenItems.FindAsync(id);
            if (brokenItem != null)
            {
                _context.BrokenItems.Remove(brokenItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BrokenItemExists(int id)
        {
            return (_context.BrokenItems?.Any(e => e.BrokenId == id)).GetValueOrDefault();
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult ExportToCsv(string searchString)
        {
            var brokenItems = _context.BrokenItems
                .Include(r => r.Item)
                .Include(r => r.User)
                .ToList();

            if (!string.IsNullOrEmpty(searchString))
            {
                brokenItems = brokenItems
                    .Where(r => r.Item != null && r.Item.Name.ToLower().Contains(searchString.ToLower()))
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
                    csvWriter.WriteRecords(brokenItems);
                }

                // Mengatur header respons HTTP untuk file CSV
                Response.Headers.Add("Content-Disposition", "attachment; filename=request_items.csv");
                Response.ContentType = "text/csv";

                // Menulis data CSV ke respons HTTP
                return Content(sw.ToString());
            }
        }
    
    }
}
