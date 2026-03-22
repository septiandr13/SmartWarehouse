using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SmartWarehouse.Controllers
{
    // Gunakan Primary Constructor untuk menyuntikkan (inject) Logger dan DbContext sekaligus
    public class HomeController(ILogger<HomeController> logger, ApplicationDbContext context) : Controller
    {
        public IActionResult Index()
        {
            // Ambil 5 produk dengan stok tertinggi
            var topProducts = context.Products
                .OrderByDescending(p => p.StockQuantity)
                .Take(5)
                .Select(p => new { p.Name, p.StockQuantity })
                .ToList();

            // Serialisasi ke JSON agar bisa dibaca JavaScript
            ViewBag.ChartLabels = JsonSerializer.Serialize(topProducts.Select(p => p.Name));
            ViewBag.ChartData = JsonSerializer.Serialize(topProducts.Select(p => p.StockQuantity));
            ViewBag.RecentMovements = context.StockMovements
                    .Include(m => m.Product)
                    .OrderByDescending(m => m.DateOccurred)
                    .Take(5)
                    .ToList();

            // Data kartu stats lainnya tetap sama
            ViewBag.TotalProducts = context.Products.Count();
            ViewBag.TotalStock = context.Products.Sum(p => (int?)p.StockQuantity) ?? 0;
            ViewBag.LowStockCount = context.Products.Count(p => p.StockQuantity < 10);

            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}