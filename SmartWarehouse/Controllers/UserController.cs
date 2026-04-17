using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();

        var userRoles = new List<(IdentityUser user, string role)>();

        foreach (var user in users)
        {
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User";
            userRoles.Add((user, role));
        }

        return View(userRoles);
    }

    // ================= CREATE USER =================
    [HttpPost]
    public async Task<IActionResult> Create(string email, string password, string role)
    {
        var user = new IdentityUser { UserName = email, Email = email };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);
        }

        return RedirectToAction(nameof(Index));
    }

    // ================= UPDATE ROLE =================
    [HttpPost]
    public async Task<IActionResult> UpdateRole(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user != null && user.Email != "admin@warehouse.com")
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            await _userManager.AddToRoleAsync(user, role);
        }

        return RedirectToAction(nameof(Index));
    }

    // ================= DELETE =================
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user != null && user.Email != "admin@warehouse.com")
        {
            await _userManager.DeleteAsync(user);
        }

        return RedirectToAction(nameof(Index));
    }
}