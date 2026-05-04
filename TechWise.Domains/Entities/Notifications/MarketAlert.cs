namespace TechWise.Domains.Entities.Store
{
    public class MarketAlert
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CreatedByAdminId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<MarketAlertProduct> Products { get; set; }
            = new List<MarketAlertProduct>();
    }
}