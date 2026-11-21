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
    private readonly InventoryDbContext _context;
    private readonly IInventoryService _inventory;
    private readonly ILogger<SuppliesController> _logger;

    public SuppliesController(InventoryDbContext context, IInventoryService inventory, ILogger<SuppliesController> logger)
    {
        _context = context;
        _inventory = inventory;
        _logger = logger;
    }

    private void LoadLists(int? supplierId = null, int? productId = null)
    {
        ViewBag.SupplierId = new SelectList(_context.Suppliers.OrderBy(s => s.Name), "Id", "Name", supplierId);
        ViewBag.ProductId = new SelectList(_context.Products.OrderBy(p => p.Name), "Id", "Name", productId);
    }

    public async Task<IActionResult> Index()
    {
        var supplies = _context.Supplies
            .Include(s => s.Product)
            .Include(s => s.Supplier)
            .OrderByDescending(s => s.Date);
        return View(await supplies.ToListAsync());
    }

    public IActionResult Create()
    {
        LoadLists();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("SupplierId,ProductId,Quantity")] Supply supply)
    {
        if (supply.Quantity <= 0)
        {
            ModelState.AddModelError("Quantity", "Quantity must be greater than 0.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                supply.Date = DateTime.Now;
                supply.Status = "Pending";
                _context.Add(supply);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to save supply for product {ProductId}", supply.ProductId);
                ModelState.AddModelError("", "Could not save the supply. Check that the supplier and the product exist.");
            }
        }

        LoadLists(supply.SupplierId, supply.ProductId);
        return View(supply);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Receive(int id)
    {
        try
        {
            await _inventory.ReceiveSupplyAsync(id);
        }
        catch (InventoryException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to receive supply {Id}", id);
            TempData["Error"] = "Could not receive the supply. Please try again.";
        }
        return RedirectToAction(nameof(Index));
    }

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

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var supply = await _context.Supplies.FindAsync(id);
        if (supply != null)
        {
            try
            {
                _context.Supplies.Remove(supply);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete supply {Id}", id);
                TempData["Error"] = "Could not delete the supply. Please try again.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}