using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public required string SKU { get; set; }
        [Required]
        public required string Name { get; set; }
        public int StockQuantity { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
