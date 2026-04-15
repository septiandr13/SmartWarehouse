using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using SmartWarehouse.Models;
using SmartWarehouse.Services;

[Route("api/[controller]")]
[ApiController]
public class ProductController(ApplicationDbContext context, IEmailService emailService) : Controller
{
    // --- SEMBUNYIKAN DARI SWAGGER (Halaman Web) ---

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    [Authorize(Roles ="Admin")]
    public async Task<IActionResult> Index()
    {
        var products = await context.Products.ToListAsync();
        return View(products);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("create-page")]
    [Authorize(Roles = "Admin")]
    public IActionResult Create() => View();

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var product = await context.Products.FindAsync(id);
        if (product == null) return NotFound();
        return View(product);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("edit/{id}")]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (ModelState.IsValid)
        {
            context.Update(product);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(product);
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("delete/{id}")]
    [Authorize(Roles = "Admin")]
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

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("report")]
    public async Task<IActionResult> DownloadStockReport()
    {
        var products = await context.Products.ToListAsync();
        return new ViewAsPdf("Index", products);
    }


    // --- TAMPILKAN DI SWAGGER (Fitur API) ---

    /// <summary>
    /// Menambah produk baru ke database
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Product product)
    {
        if (ModelState.IsValid)
        {
            context.Add(product);
            await context.SaveChangesAsync();

            // Log Movement
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

            return Ok(new { success = true, message = "Produk berhasil ditambahkan" });
        }
        return BadRequest(ModelState);
    }

    /// <summary>
    /// Mencari nama produk berdasarkan barcode/SKU
    /// </summary>
    [HttpGet("GetNameBySKU")]
    public async Task<IActionResult> GetNameBySKU(string sku)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.SKU == sku);
        if (product != null)
        {
            return Ok(new { success = true, name = product.Name });
        }
        return NotFound(new { success = false, message = "Produk tidak ditemukan" });
    }
}