namespace TeaByGreen.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> Items { get; set; }
        public decimal TotalAmount { get; set; }

        // 🔧 Add this line
        public int? UserId { get; set; }

        // 🔧 Optional: Navigation property
        public User? User { get; set; }
    }

}
