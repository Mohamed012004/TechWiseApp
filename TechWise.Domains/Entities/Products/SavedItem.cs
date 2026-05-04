namespace TechWise.Domains.Entities.Store
{
    public class SavedItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Product Product { get; set; }
    }
}