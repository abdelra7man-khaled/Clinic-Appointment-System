using Clinic.DataAccess.Data;
using Clinic.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Clinic.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        internal DbSet<T> _dbSet;
        public Repository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T> GetAsync(int id)
            => await _dbSet.FindAsync(id);

        public virtual IQueryable<T> Query()
            => _dbSet.AsQueryable();
        public virtual async Task AddAsync(T entity)
            => await _dbSet.AddAsync(entity);
        public virtual void Update(T entity)
            => _dbSet.Update(entity);
        public virtual void Remove(T entity)
            => _dbSet.Remove(entity);

        public virtual void RemoveRange(IEnumerable<T> entities)
            => _dbSet.RemoveRange(entities);
    }
}
