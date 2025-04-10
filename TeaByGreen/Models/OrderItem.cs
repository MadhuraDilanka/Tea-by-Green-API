namespace TeaByGreen.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // ✅ Mark these as optional
        public Product? Product { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }

        // 🔧 Add this line
        public decimal PriceAtOrderTime { get; set; }
    }


}
