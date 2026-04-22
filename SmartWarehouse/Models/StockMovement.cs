namespace SmartWarehouse.Models
{
    public class StockMovement
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public int QuantityChanged { get; set; } // Bisa positif (masuk) atau negatif (keluar)
        public string Type { get; set; } = "Inbound"; // Inbound, Outbound, Adjustment
        public string? Remarks { get; set; }
        public string? UserName { get; set; }
        public DateTime DateOccurred { get; set; } = DateTime.Now;
    }
}
