using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using SmartWarehouse.Models;
using SmartWarehouse.Services;

namespace SmartWarehouse.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ProductController(ApplicationDbContext context, IEmailService emailService) : Controller
    {
        // Menampilkan daftar stok gudang
        public async Task<IActionResult> Index()
        {
            var products = await context.Products.ToListAsync();
            return View(products);
        }

        // Form Tambah Barang
        [Authorize(Roles ="Admin")]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                context.Add(product);
                await context.SaveChangesAsync();

                // CATAT KE MOVEMENT
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

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (ModelState.IsValid)
            {
                context.Update(product);
                await context.SaveChangesAsync();

                if (product.StockQuantity < 10)
                {
                    string subject = $"🚨 PERINGATAN STOK RENDAH: {product.Name}";
                    string message = $@"<h3>Stok Kritis!</h3>
                                <p>Barang <b>{product.Name}</b> (SKU: {product.SKU}) saat ini hanya tersisa <b>{product.StockQuantity}</b> unit.</p>
                                <p>Segera lakukan pemesanan ulang (Restock).</p>";

                    await emailService.SendEmailAsync("yayansky65@gmail.com", subject, message);
                }

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await context.Products.FindAsync(id);
            if (product != null)
            {
                context.Products.Remove(product);
                await context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DownloadStockReport()
        {
            var products = await context.Products.ToListAsync();

            // Ini akan memanggil view "Index" tapi merendernya sebagai PDF
            return new ViewAsPdf("Index", products)
            {
                FileName = $"Stock_Report_{DateTime.Now:yyyyMMdd}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--footer-center [page]/[toPage]"
            };
        }

        
    }
}
