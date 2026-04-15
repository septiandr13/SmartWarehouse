using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    // Menampilkan daftar semua user
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users.ToListAsync();
        return View(users);
    }

    // Menghapus User
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user != null && user.Email != "admin@warehouse.com") 
        {
            await userManager.DeleteAsync(user);
        }
        return RedirectToAction(nameof(Index));
    }
}