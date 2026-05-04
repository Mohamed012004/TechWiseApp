using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;

namespace TechWise.Persistence.Store.Repositories
{
    public class AdminRepository(StoreDbContext _context) : IAdminRepository
    {
        // ===== Stats =====
        public async Task<int> GetTotalOrdersAsync()
            => await _context.Orders.CountAsync();

        public async Task<int> GetTodayOrdersAsync()
            => await _context.Orders
                .CountAsync(o => o.CreatedAt.Date == DateTime.UtcNow.Date);

        public async Task<decimal> GetTotalRevenueAsync()
            => await _context.Orders
                .Where(o => o.IsPaid)
                .SumAsync(o => o.Total);

        public async Task<int> GetTotalReviewsAsync()
            => await _context.Reviews.CountAsync();

        // ===== Products =====
        public async Task<List<Product>> GetAllProductsAsync(
            string? category, string? brand, int pageNumber, int pageSize)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category.ToLower());

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(p => p.Brand.ToLower() == brand.ToLower());

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalProductsAsync()
            => await _context.Products.CountAsync();

        public async Task<Product?> GetProductByIdAsync(int id)
            => await _context.Products.FindAsync(id);

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // ===== Orders =====
        public async Task<List<Order>> GetAllOrdersAsync(
            int pageNumber, int pageSize)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
            => await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

        public async Task UpdateOrderAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // ===== Reviews =====
        public async Task<List<Review>> GetAllReviewsAsync(
            int pageNumber, int pageSize)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
            => await _context.Reviews
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == reviewId);

        public async Task DeleteReviewAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}