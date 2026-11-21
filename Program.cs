using InventorySystem.Data;
using InventorySystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace InventorySystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var identityConnection = builder.Configuration.GetConnectionString("IdentityConnection")
                ?? throw new InvalidOperationException("Connection string 'IdentityConnection' not found.");
            var inventoryConnection = builder.Configuration.GetConnectionString("InventoryConnection")
                ?? throw new InvalidOperationException("Connection string 'InventoryConnection' not found.");

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(identityConnection));
            builder.Services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(inventoryConnection));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<AppIdentityDbContext>();
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IInventoryService, InventoryService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}").WithStaticAssets();
            app.MapRazorPages().WithStaticAssets();
            app.Run();
        }
    }
}
