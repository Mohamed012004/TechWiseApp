using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Contracts;
using TechWise.Persistence.Identity.Contexts;

namespace TechWise.Persistence.Identity.Repositories
{
    public class GenericRepository<T>(IdentityStoreDbContext _context)
        : IGenericRepository<T> where T : class
    {
        protected readonly DbSet<T> _dbSet = _context.Set<T>();

        public async Task<T?> GetByIdAsync(int id)
            => await _dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);

        public void Update(T entity)
            => _dbSet.Update(entity);

        public void Delete(T entity)
            => _dbSet.Remove(entity);

        public void DeleteRange(IEnumerable<T> entities)
            => _dbSet.RemoveRange(entities);
    }
}