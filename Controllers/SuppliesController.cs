using InventorySystem.Data;
using InventorySystem.Models;
using InventorySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class SuppliesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IInventoryService _inventory;

    public SuppliesController(ApplicationDbContext context, IInventoryService inventory)
    {
        _context = context;
        _inventory = inventory;
    }

    private void LoadLists(int? supplierId = null, int? productId = null)
    {
        ViewBag.SupplierId = new SelectList(_context.Suppliers, "Id", "Name", supplierId);
        ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", productId);
    }

    // GET: Supplies
    public async Task<IActionResult> Index()
    {
        var supplies = _context.Supplies
            .Include(s => s.Product)
            .Include(s => s.Supplier)
            .OrderByDescending(s => s.Date);
        return View(await supplies.ToListAsync());
    }

    // GET: Supplies/Create
    public IActionResult Create()
    {
        LoadLists();
        return View();
    }

    // POST: Supplies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("SupplierId,ProductId,Quantity")] Supply supply)
    {
        if (ModelState.IsValid)
        {
            supply.Date = DateTime.Now;
            supply.Status = "Pending";
            _context.Add(supply);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        LoadLists(supply.SupplierId, supply.ProductId);
        return View(supply);
    }

    // POST: Supplies/Receive/5  (marks supply as received and increases stock)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Receive(int id)
    {
        await _inventory.ReceiveSupplyAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // GET: Supplies/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var supply = await _context.Supplies
            .Include(s => s.Product)
            .Include(s => s.Supplier)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (supply == null) return NotFound();

        return View(supply);
    }

    // POST: Supplies/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var supply = await _context.Supplies.FindAsync(id);
        if (supply != null)
        {
            _context.Supplies.Remove(supply);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}