using System.ComponentModel.DataAnnotations;

namespace SmartWarehouse.Models
{
    public class WarehouseBin
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Zone { get; set; }
        public required string RackName { get; set; }
        public bool IsOccupied { get; set; }
    }
}
