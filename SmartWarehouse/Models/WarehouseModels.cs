using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public required string SKU { get; set; }
        [Required]
        public required string Name { get; set; }
        public int StockQuantity { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    public class WarehouseBin
    {
        public int Id { get; set; }
        public required string Zone { get; set; } // Contoh: A1, B2
        public required string RackName { get; set; }
        public bool IsOccupied { get; set; }
    }
    // Models/AppSetting.cs
    public class AppSetting
    {
        public int Id { get; set; }
        public string Key { get; set; } // Contoh: "WarehouseName"
        public string Value { get; set; } // Contoh: "Gudang Utama Jakarta"
    }
}
