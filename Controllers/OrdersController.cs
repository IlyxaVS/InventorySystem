using InventorySystem.Data;
using InventorySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class OrdersController : Controller
{
    private readonly InventoryDbContext _context;
    private readonly IInventoryService _inventory;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(InventoryDbContext context, IInventoryService inventory, ILogger<OrdersController> logger)
    {
        _context = context;
        _inventory = inventory;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(o => o.Product)
            .OrderByDescending(o => o.Date)
            .ToListAsync();

        return View(orders);
    }

    public IActionResult Create()
    {
        ViewBag.Products = new SelectList(_context.Products.OrderBy(p => p.Name), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int productId, int quantity)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            await _inventory.PlaceOrderAsync(productId, quantity, userId);
            return RedirectToAction(nameof(Index));
        }
        catch (InventoryException ex)
        {
            ViewBag.Error = ex.Message;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Failed to save order for product {ProductId}", productId);
            ViewBag.Error = "Could not save the order. Please try again.";
        }

        ViewBag.Products = new SelectList(_context.Products.OrderBy(p => p.Name), "Id", "Name", productId);
        return View();
    }
}