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
        public IActionResult Index(Guid? productId, DateTime? startDate, DateTime? endDate, int? month, int? year)
        {
            var movements = context.StockMovements
                .Include(m => m.Product)
                .AsQueryable();

            // FILTER PRODUCT
            if (productId.HasValue)
                movements = movements.Where(m => m.ProductId == productId);

            // FILTER TANGGAL RANGE
            if (startDate.HasValue)
                movements = movements.Where(m => m.DateOccurred >= startDate.Value);

            if (endDate.HasValue)
                movements = movements.Where(m => m.DateOccurred <= endDate.Value);

            // FILTER BULAN
            if (month.HasValue)
                movements = movements.Where(m => m.DateOccurred.Month == month);

            // FILTER TAHUN
            if (year.HasValue)
                movements = movements.Where(m => m.DateOccurred.Year == year);

            var chartData = movements
            .GroupBy(m => m.DateOccurred.Date)
            .Select(g => new
            {
                Date = g.Key,
                Inbound = g.Where(x => x.Type == "Inbound").Sum(x => x.QuantityChanged),
                Outbound = g.Where(x => x.Type == "Outbound").Sum(x => Math.Abs(x.QuantityChanged))
            })
            .OrderBy(x => x.Date)
            .ToList();

            // FORMAT KE JSON
            ViewBag.ChartLabels = JsonSerializer.Serialize(
                chartData.Select(x => x.Date.ToString("dd MMM"))
            );

            ViewBag.InboundData = JsonSerializer.Serialize(
                chartData.Select(x => x.Inbound)
            );

            ViewBag.OutboundData = JsonSerializer.Serialize(
                chartData.Select(x => x.Outbound)
            );
            // ======================
            // DATA ACTIVITY
            // ======================
            var recentMovements = movements
                .OrderByDescending(m => m.DateOccurred)
                .Take(10)
                .ToList();

            ViewBag.RecentMovements = recentMovements;

            // ======================
            // CHART (ikut filter)
            // ======================
            var topProducts = context.Products
                .Select(p => new
                {
                    p.Name,
                    Stock = p.StockQuantity
                })
                .OrderByDescending(p => p.Stock)
                .Take(5)
                .ToList();

            ViewBag.ChartLabels = JsonSerializer.Serialize(topProducts.Select(p => p.Name));
            ViewBag.ChartData = JsonSerializer.Serialize(topProducts.Select(p => p.Stock));

            // ======================
            // KPI (optional: bisa difilter juga)
            // ======================
            ViewBag.TotalProducts = context.Products.Count();
            ViewBag.TotalStock = context.Products.Sum(p => (int?)p.StockQuantity) ?? 0;
            ViewBag.LowStockCount = context.Products.Count(p => p.StockQuantity < 10);

            // ======================
            // DROPDOWN DATA
            // ======================
            ViewBag.Products = context.Products.ToList();

            // ======================
            // SIMPAN STATE FILTER
            // ======================
            ViewBag.ProductId = productId;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.Month = month;
            ViewBag.Year = year;

            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Export(Guid? productId, DateTime? startDate, DateTime? endDate, int? month, int? year)
        {
            var movements = context.StockMovements
                .Include(m => m.Product)
                .AsQueryable();

            // APPLY FILTER (SAMA PERSIS)
            if (productId.HasValue)
                movements = movements.Where(m => m.ProductId == productId);

            if (startDate.HasValue)
                movements = movements.Where(m => m.DateOccurred >= startDate);

            if (endDate.HasValue)
                movements = movements.Where(m => m.DateOccurred <= endDate);

            if (month.HasValue)
                movements = movements.Where(m => m.DateOccurred.Month == month);

            if (year.HasValue)
                movements = movements.Where(m => m.DateOccurred.Year == year);

            var data = movements
                .OrderByDescending(m => m.DateOccurred)
                .ToList();

            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var ws = workbook.Worksheets.Add("Warehouse Report");

            // HEADER
            ws.Cell(1, 1).Value = "Tanggal";
            ws.Cell(1, 2).Value = "Produk";
            ws.Cell(1, 3).Value = "Tipe";
            ws.Cell(1, 4).Value = "Qty";

            // DATA
            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].DateOccurred;
                ws.Cell(i + 2, 2).Value = data[i].Product?.Name;
                ws.Cell(i + 2, 3).Value = data[i].Type;
                ws.Cell(i + 2, 4).Value = data[i].QuantityChanged;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "WarehouseReport.xlsx"
            );
        }
    }
}