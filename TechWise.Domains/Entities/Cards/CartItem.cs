namespace TechWise.Domains.Entities.Store
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;

        // Navigation
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}