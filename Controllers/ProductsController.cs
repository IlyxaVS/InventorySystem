using InventorySystem.Data;
using InventorySystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ProductsController : Controller
{
    private readonly InventoryDbContext _context;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(InventoryDbContext context, ILogger<ProductsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    private void LoadLists(int? categoryId = null, int? warehouseId = null)
    {
        ViewBag.CategoryId = new SelectList(_context.Categories.OrderBy(c => c.Name), "Id", "Name", categoryId);
        ViewBag.WarehouseId = new SelectList(_context.Warehouses.OrderBy(w => w.Name), "Id", "Name", warehouseId);
    }

    private void ValidateNumbers(Product product)
    {
        if (product.Price < 0)
            ModelState.AddModelError("Price", "Price cannot be negative.");
        if (product.Quantity < 0)
            ModelState.AddModelError("Quantity", "Stock cannot be negative.");
        if (product.MinQuantity < 0)
            ModelState.AddModelError("MinQuantity", "Minimum stock cannot be negative.");
    }

    public async Task<IActionResult> Index()
    {
        var products = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Warehouse)
            .OrderBy(p => p.Name);
        return View(await products.ToListAsync());
    }

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

    public IActionResult Create()
    {
        LoadLists();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name,Sku,Price,Quantity,MinQuantity,CategoryId,WarehouseId")] Product product)
    {
        ValidateNumbers(product);

        if (await _context.Products.AnyAsync(p => p.Sku == product.Sku))
        {
            ModelState.AddModelError("Sku", "A product with this SKU already exists.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to save product {Sku}", product.Sku);
                ModelState.AddModelError("", "Could not save the product. Check that the SKU is unique and that the category and warehouse exist.");
            }
        }

        LoadLists(product.CategoryId, product.WarehouseId);
        return View(product);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        LoadLists(product.CategoryId, product.WarehouseId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Name,Sku,Price,Quantity,MinQuantity,CategoryId,WarehouseId")] Product product)
    {
        if (id != product.Id) return NotFound();

        ValidateNumbers(product);

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
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Products.AnyAsync(e => e.Id == product.Id)) return NotFound();
                throw;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to update product {Id}", product.Id);
                ModelState.AddModelError("", "Could not save the changes. Check that the SKU is unique and that the category and warehouse exist.");
            }
        }

        LoadLists(product.CategoryId, product.WarehouseId);
        return View(product);
    }

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

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            try
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Failed to delete product {Id}", id);
                TempData["Error"] = "This product cannot be deleted because orders or supplies refer to it.";
            }
        }
        return RedirectToAction(nameof(Index));
    }
}