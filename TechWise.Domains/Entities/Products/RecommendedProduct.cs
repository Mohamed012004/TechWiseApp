namespace TechWise.Domains.Entities.Store
{
    public class RecommendedProduct
    {
        public int Id { get; set; }
        public string UserId { get; set; }        // GUID for AppUser
        public int ProductId { get; set; }
        public int UserReqNumber { get; set; }
        public string? OperationType { get; set; } // Recommendation / Upgrade
        public string? Brand { get; set; }
        public string? Category { get; set; }
        public string? Purpose { get; set; }
        public string? Budget { get; set; }
        public string? ProductName { get; set; }
        public string? Reason { get; set; }

        // Navigation Property
        public Product Product { get; set; }
    }
}