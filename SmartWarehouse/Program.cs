using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using SmartWarehouse.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. SERVICES CONFIGURATION
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. IDENTITY CONFIGURATION (Penting: Menggunakan AddIdentity, bukan AddDefaultIdentity)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

//Akses prduct untuk admin
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/Index";

    options.LoginPath = "/Identity/Account/Login";
});

// 1.Daftarkan EmailService dasar Anda
builder.Services.AddTransient<IEmailService, EmailService>();

// 2. Daftarkan IdentityEmailSender sebagai implementasi IEmailSender
builder.Services.AddTransient<IEmailSender, IdentityEmailSender>();

builder.Services.AddTransient<IEmailService, EmailService>();
var app = builder.Build();

IWebHostEnvironment env = app.Environment;
RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");

// 3. SEEDING ROLES & ADMIN USER
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // Buat Roles jika belum ada
        string[] roleNames = { "Admin", "Operator" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Buat User Admin Default (Ganti email & password sesuai keinginan)
        string adminEmail = "admin@warehouse.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdmin = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
            var createAdmin = await userManager.CreateAsync(newAdmin, "Admin123!");
            if (createAdmin.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Terjadi error saat seeding database.");
    }
}

// 4. HTTP REQUEST PIPELINE
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Urutan Authentication harus sebelum Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();