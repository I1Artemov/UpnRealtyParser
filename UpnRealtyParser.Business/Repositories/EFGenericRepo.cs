using Microsoft.EntityFrameworkCore;
using System;

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
