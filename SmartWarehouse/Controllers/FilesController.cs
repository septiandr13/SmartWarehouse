using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Models;

namespace SmartWarehouse.Controllers
{
    [Authorize]
    public class FilesController(ApplicationDbContext context, IWebHostEnvironment env) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await context.FileDocuments.OrderByDescending(f => f.UploadDate).ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return Json(new { success = false, message = "File kosong" });

            try
            {
                string uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                var doc = new FileDocument
                {
                    FileName = file.FileName,
                    StoredFileName = uniqueFileName,
                    FilePath = "/uploads/" + uniqueFileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    UserName = User.Identity.Name
                };

                context.FileDocuments.Add(doc);
                await context.SaveChangesAsync();

                // Kembalikan JSON saat berhasil agar Dropzone tahu proses selesai
                return Json(new { success = true, fileName = file.FileName });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var doc = await context.FileDocuments.FindAsync(id);
            if (doc == null) return NotFound();

            // 1. Hapus File Fisik
            string fullPath = Path.Combine(env.WebRootPath, doc.FilePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath)) System.IO.File.Delete(fullPath);

            // 2. Hapus Record Database
            context.FileDocuments.Remove(doc);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
