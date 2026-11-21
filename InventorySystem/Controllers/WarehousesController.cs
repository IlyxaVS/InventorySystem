using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class WarehousesController : Controller
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<WarehousesController> _logger;

    public WarehousesController(InventoryDbContext context, ILogger<WarehousesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Warehouses.OrderBy(w => w.Name).ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Address")] Warehouse warehouse)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to save warehouse {Name}", warehouse.Name);
                ModelState.AddModelError("", "Could not save the warehouse. Please try again.");
            }
        }
        return View(warehouse);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null) return NotFound();

        return View(warehouse);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] Warehouse warehouse)
    {
        if (id != warehouse.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(warehouse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Warehouses.AnyAsync(e => e.Id == warehouse.Id)) return NotFound();
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update warehouse {Id}", warehouse.Id);
                ModelState.AddModelError("", "Could not save the changes. Please try again.");
            }
        }
        return View(warehouse);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(m => m.Id == id);
        if (warehouse == null) return NotFound();

        return View(warehouse);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse != null)
        {
            try
            {
                _context.Warehouses.Remove(warehouse);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete warehouse {Id}", id);
                TempData["Error"] = "This warehouse cannot be deleted because products are stored in it.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}