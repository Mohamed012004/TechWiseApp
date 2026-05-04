using TechWise.Domains.Entities.Store;

namespace TechWise.Domains.Contracts
{
    public interface IProductRepository
    {
        Task<(List<Product> Items, int TotalCount)> GetProductsAsync(
            string? category,
            string? brand,
            decimal? minPrice,
            decimal? maxPrice,
            string? search,
            int pageNumber,
            int pageSize);

        Task<Product?> GetProductByIdAsync(int id);
        Task<List<string>> GetCategoriesAsync();
    }
}