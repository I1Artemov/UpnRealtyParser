using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UpnRealtyParser.Business.Repositories
{
    public class EFGenericRepo<TEntity, TContext> : IDisposable
        where TEntity : class
        where TContext : DbContext
    {
        protected TContext _context;
        protected DbSet<TEntity> _dbSet;
        protected bool disposed = false;

        public EFGenericRepo(TContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public void Add(TEntity item)
        {
            _dbSet.Add(item);
        }

        public async Task AddRangeAsync(List<TEntity> items)
        {
            await _dbSet.AddRangeAsync(items);
        }

        public void Delete(TEntity item)
        {
            _dbSet.Remove(item);
        }

        public TEntity Get(int id)
        {
            return _dbSet.Find(id);
        }

        public TEntity GetWithoutTracking(Func<TEntity, bool> condition)
        {
            return _dbSet.AsNoTracking().FirstOrDefault(condition);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbSet;
        }

        public IQueryable<TEntity> GetAllWithoutTracking()
        {
            return _dbSet.AsNoTracking();
        }

        public void Update(TEntity item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public List<TEntity> ApplyPagination(IQueryable<TEntity> allData, int skip, int pageSize)
        {
            return allData.Skip(skip).Take(pageSize).ToList();
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
