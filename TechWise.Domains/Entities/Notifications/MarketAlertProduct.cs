namespace TechWise.Domains.Entities.Store
{
    public class MarketAlertProduct
    {
        public int Id { get; set; }
        public int MarketAlertId { get; set; }
        public int ProductId { get; set; }

        // Navigation
        public MarketAlert MarketAlert { get; set; }
        public Product Product { get; set; }
    }
}