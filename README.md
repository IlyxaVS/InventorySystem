# Inventory Management System
ASP.NET Core MVC web application for tracking products, stock levels, supplies and orders.

## Features
- Registration and login (ASP.NET Core Identity), all pages require authentication
- Product, category, warehouse and supplier management (add, edit, delete)
- Stock tracking with low stock highlighting
- Orders (reduce stock) and supplies (increase stock when received)
- Automatic supply request when the stock drops below the minimum level
- Stock and sales reports with CSV export
- Home page dashboard with key numbers

## Tech stack
ASP.NET Core MVC, Entity Framework Core (Code First), SQL Server LocalDB, Dependency Injection, async/await, xUnit, Bootstrap, JavaScript
Run all tests from the repository root with `dotnet test`.

## Architecture
- Models: entities (Product, Category, Warehouse, Supplier, Supply, Order)
- Data: two database contexts
  - `AppIdentityDbContext`: users (ASP.NET Core Identity), database `InventoryIdentityDb`
  - `InventoryDbContext`: products, warehouses, supplies and orders, database `InventoryDb`
- Services: business logic (`InventoryService`: placing orders, receiving supplies, automatic supply requests)
- Controllers and Views: MVC layer
- InventorySystem.Tests: unit tests for the service (xUnit, in-memory database)

## How to run
1. Install Visual Studio with the ASP.NET workload and SQL Server LocalDB.
2. Clone the repository and open `InventorySystem.slnx`.
3. Check the connection strings `IdentityConnection` and `InventoryConnection` in `appsettings.json`.
4. Open Package Manager Console (Default project: InventorySystem) and create both databases:
```
Update-Database -Context AppIdentityDbContext
Update-Database -Context InventoryDbContext
```
5. Press F5 and register a new user.

## How it works
1. Create categories, warehouses and suppliers.
2. Add products with a unique SKU (product code), a price, the current stock and a minimum stock level.
3. Create an order: the stock decreases. If it drops below the minimum, a supply request with status Pending is created automatically.
4. When goods arrive, press Receive on the Supplies page: the stock increases.
5. Open Reports to see the stock value and sales for a period (CSV export is available).