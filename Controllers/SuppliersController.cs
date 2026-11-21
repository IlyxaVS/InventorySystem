using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class SuppliersController : Controller
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<SuppliersController> _logger;

    public SuppliersController(InventoryDbContext context, ILogger<SuppliersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Suppliers.OrderBy(s => s.Name).ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Phone")] Supplier supplier)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to save supplier {Name}", supplier.Name);
                ModelState.AddModelError("", "Could not save the supplier. Please try again.");
            }
        }
        return View(supplier);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier == null) return NotFound();

        return View(supplier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Phone")] Supplier supplier)
    {
        if (id != supplier.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Suppliers.AnyAsync(e => e.Id == supplier.Id)) return NotFound();
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update supplier {Id}", supplier.Id);
                ModelState.AddModelError("", "Could not save the changes. Please try again.");
            }
        }
        return View(supplier);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var supplier = await _context.Suppliers.FirstOrDefaultAsync(m => m.Id == id);
        if (supplier == null) return NotFound();

        return View(supplier);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier != null)
        {
            try
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete supplier {Id}", id);
                TempData["Error"] = "This supplier cannot be deleted because supplies are linked to it.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}