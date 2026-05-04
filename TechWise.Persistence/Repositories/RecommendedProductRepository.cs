using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Domains.Entities.Store;

namespace TechWise.Persistence.Store.Repositories
{
    public class RecommendedProductRepository(StoreDbContext _context)
        : IRecommendedProductRepository
    {
        public async Task<List<RecommendedProduct>> GetUserRecommendationsAsync(
            string userId, string? operationType)
        {
            var query = _context.RecommendedProducts
                .Include(r => r.Product)
                    .ThenInclude(p => p.Specs)
                .Where(r => r.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(operationType))
                query = query.Where(r => r.OperationType == operationType);

            return await query
                .OrderByDescending(r => r.UserReqNumber)
                .ToListAsync();
        }

        public async Task<RecommendedProduct?> GetRecommendedProductDetailsAsync(
            string userId, int productId, int userReqNumber)
        {
            return await _context.RecommendedProducts
                .Include(r => r.Product)
                    .ThenInclude(p => p.Specs)
                .FirstOrDefaultAsync(r =>
                    r.UserId == userId &&
                    r.ProductId == productId &&
                    r.UserReqNumber == userReqNumber);
        }


        public async Task<RecommendedProduct?> GetLatestRecommendationAsync(
            string userId, int productId)
        {
            return await _context.RecommendedProducts
                .Include(r => r.Product)
                    .ThenInclude(p => p.Specs)
                .Where(r => r.UserId == userId && r.ProductId == productId)
                .OrderByDescending(r => r.UserReqNumber)
                .FirstOrDefaultAsync();
        }


    }
}