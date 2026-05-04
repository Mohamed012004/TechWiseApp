using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;

namespace TechWise.Persistence.Store.Repositories
{
    public class ProductRepository(StoreDbContext _context) : IProductRepository
    {
        public async Task<(List<Product> Items, int TotalCount)> GetProductsAsync(
            string? category, string? brand,
            decimal? minPrice, decimal? maxPrice,
            string? search, int pageNumber, int pageSize)
        {
            var query = _context.Products
                .Include(p => p.Specs)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category.ToLower());

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(p => p.Brand.ToLower() == brand.ToLower());

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.Title.Contains(search) ||
                                         p.Brand.Contains(search));

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Specs)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();
        }
    }
}