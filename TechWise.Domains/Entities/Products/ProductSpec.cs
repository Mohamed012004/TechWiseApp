 
namespace TechWise.Domains.Entities.Store
{
    public class ProductSpec
    {
        public int Id { get; set; }
        public string SpecKey { get; set; }
        public string SpecValue { get; set; }

        // FK
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}