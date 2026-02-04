using AppStore.Core.Entities;
using AppStore.Core.Interfaces;
using AppStore.Infraestructure.Data;
using System.Threading.Tasks;

namespace AppStore.Infrastructure.Repositories
{
    public class UniOfWork : IUniOfWork, IDisposable
    {
        private readonly AppStoreDbContext _context;
        private readonly IRepository<Product> _productRepository;
        private bool _disposed = false;

        public UniOfWork(AppStoreDbContext context, IRepository<Product> productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public IRepository<Product> ProductRespository => _productRepository ?? new BaseRepository<Product>(_context);

        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                _disposed = true;
            }
        }

        ~UniOfWork()
        {
            Dispose(false);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
