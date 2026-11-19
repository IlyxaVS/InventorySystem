using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Fills the Category and Warehouse dropdowns (shows names, stores ids)
    private void LoadLists(int? categoryId = null, int? warehouseId = null)
    {
        ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", categoryId);
        ViewBag.WarehouseId = new SelectList(_context.Warehouses, "Id", "Name", warehouseId);
    }

    // GET: Products
    public async Task<IActionResult> Index()
    {
        var products = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Warehouse);
        return View(await products.ToListAsync());
    }

    // GET: Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Warehouse)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null) return NotFound();

        return View(product);
    }

    // GET: Products/Create
    public IActionResult Create()
    {
        LoadLists();
        return View();
    }

    // POST: Products/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name,Sku,Price,Quantity,MinQuantity,CategoryId,WarehouseId")] Product product)
    {
        if (await _context.Products.AnyAsync(p => p.Sku == product.Sku))
        {
            ModelState.AddModelError("Sku", "A product with this SKU already exists.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        LoadLists(product.CategoryId, product.WarehouseId);
        return View(product);
    }

    // GET: Products/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        LoadLists(product.CategoryId, product.WarehouseId);
        return View(product);
    }

    // POST: Products/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Name,Sku,Price,Quantity,MinQuantity,CategoryId,WarehouseId")] Product product)
    {
        if (id != product.Id) return NotFound();

        if (await _context.Products.AnyAsync(p => p.Sku == product.Sku && p.Id != product.Id))
        {
            ModelState.AddModelError("Sku", "A product with this SKU already exists.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        LoadLists(product.CategoryId, product.WarehouseId);
        return View(product);
    }

    // GET: Products/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Warehouse)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null) return NotFound();

        return View(product);
    }

    // POST: Products/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}