# Inventory Management System
ASP.NET Core MVC web application for tracking products, stock levels,
supplies and orders.

## Features
- Registration and login (ASP.NET Core Identity)
- Product, category, warehouse and supplier management
- Stock tracking with low stock highlighting
- Orders (reduce stock) and supplies (increase stock)
- Automatic supply request when stock drops below the minimum
- Stock and sales reports with CSV export

## Tech stack
ASP.NET Core MVC, Entity Framework Core (Code First), SQL Server LocalDB,
Dependency Injection, async/await, xUnit, Bootstrap, JavaScript

## How to run
1. Install Visual Studio with the ASP.NET workload and SQL Server LocalDB.
2. Clone the repository and open InventorySystem.sln.
3. Check the connection string in appsettings.json.
4. In Package Manager Console run: Update-Database
5. Press F5.