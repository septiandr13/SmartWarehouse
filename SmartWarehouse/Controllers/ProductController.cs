using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using SmartWarehouse.Models;
using SmartWarehouse.Services;

namespace SmartWarehouse.Controllers
{
    // Hapus [Route] dan [ApiController] agar fitur View (HTML) kembali normal
    public class ProductController(ApplicationDbContext context, IEmailService emailService) : Controller
    {
        // 1. Menampilkan Daftar Stok
        public async Task<IActionResult> Index()
        {
            var products = await context.Products.ToListAsync();
            return View(products);
        }

        // 2. Form Tambah Barang (GET)
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View();

        // 3. Proses Tambah Barang (POST)
        [HttpPost]
        [ValidateAntiForgeryToken] // Keamanan standar form MVC
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                context.Add(product);
                await context.SaveChangesAsync();

                // Catat ke Stock Movement
                var movement = new StockMovement
                {
                    ProductId = product.Id,
                    QuantityChanged = product.StockQuantity,
                    Type = "Inbound",
                    Remarks = "Stok awal produk baru",
                    UserName = User.Identity?.Name
                };
                context.StockMovements.Add(movement);
                await context.SaveChangesAsync();
                //send email new product
                string subject = $"ADA STOCK BARU MASUK NIH: {product.Name}";
                string message = $@"<h3>Segera infokan ke tim yang lain!</h3>
                                        <p>Barang <b>{product.Name}</b> (SKU: {product.SKU}) Stock baru masuk <b>{product.StockQuantity}</b> unit.</p>";
                await emailService.SendEmailAsync("yayansky65@gmail.com", subject, message);

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // 4. Form Edit Barang (GET)
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null) return NotFound();
            var product = await context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // 5. Proses Edit Barang (POST)
        [HttpPost]
        [Authorize(Roles ="Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(product);
                    await context.SaveChangesAsync();

                   
                    // Cek Stok Rendah untuk Email Notifikasi
                    if (product.StockQuantity < 10)
                    {
                        string subject = $"🚨 PERINGATAN STOK RENDAH: {product.Name}";
                        string message = $@"<h3>Stok Kritis!</h3>
                                         <p>Barang <b>{product.Name}</b> (SKU: {product.SKU}) saat ini tersisa <b>{product.StockQuantity}</b> unit.</p>";

                        await emailService.SendEmailAsync("yayansky65@gmail.com", subject, message);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!context.Products.Any(e => e.Id == product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // 6. Proses Hapus Barang (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var product = await context.Products.FindAsync(id);
            if (product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 7. Cetak Report PDF
        public async Task<IActionResult> DownloadStockReport()
        {
            var products = await context.Products.ToListAsync();
            return new ViewAsPdf("Index", products)
            {
                FileName = $"Stock_Report_{DateTime.Now:yyyyMMdd}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--footer-center [page]/[toPage]"
            };
        }

        // 8. API Pendukung untuk Scanner Barcode (Tetap bekerja via AJAX)
        [HttpGet]
        public async Task<IActionResult> GetNameBySKU(string sku)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.SKU == sku);
            if (product != null)
            {
                return Json(new { success = true, name = product.Name });
            }
            return Json(new { success = false, message = "Produk baru" });
        }
    }
}