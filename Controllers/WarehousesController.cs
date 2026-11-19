using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class WarehousesController : Controller
{
    private readonly ApplicationDbContext _context;

    public WarehousesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: WAREHOUSES
    public async Task<IActionResult> Index()
    {
        return View(await _context.Warehouses.ToListAsync());
    }

    // GET: WAREHOUSES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(m => m.Id == id);
        if (warehouse == null)
        {
            return NotFound();
        }

        return View(warehouse);
    }

    // GET: WAREHOUSES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: WAREHOUSES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Address")] Warehouse warehouse)
    {
        if (ModelState.IsValid)
        {
            _context.Add(warehouse);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(warehouse);
    }

    // GET: WAREHOUSES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
        {
            return NotFound();
        }
        return View(warehouse);
    }

    // POST: WAREHOUSES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,Address")] Warehouse warehouse)
    {
        if (id != warehouse.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(warehouse);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseExists(warehouse.Id))
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
        return View(warehouse);
    }

    // GET: WAREHOUSES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(m => m.Id == id);
        if (warehouse == null)
        {
            return NotFound();
        }

        return View(warehouse);
    }

    // POST: WAREHOUSES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse != null)
        {
            _context.Warehouses.Remove(warehouse);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool WarehouseExists(int? id)
    {
        return _context.Warehouses.Any(e => e.Id == id);
    }
}
