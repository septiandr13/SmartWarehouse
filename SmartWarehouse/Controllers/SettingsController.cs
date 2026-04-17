using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//[Authorize(Roles = "Admin")]
public class SettingsController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var settings = await context.AppSettings.ToListAsync();
        return View(settings);
    }

    [HttpPost]
    public async Task<IActionResult> Update(int id, string value)
    {
        var setting = await context.AppSettings.FindAsync(id);
        if (setting != null)
        {
            setting.Value = value;
            await context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}