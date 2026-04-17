using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Models;
public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<WarehouseBin> Bins { get; set; }
    public DbSet<StockMovement> StockMovements { get; set; }
    public DbSet<AppSetting> AppSettings { get; set; }
    public DbSet<FileDocument> FileDocuments { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}