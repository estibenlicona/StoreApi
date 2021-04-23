using AppStore.Core.Entities;
using AppStore.Core.Interfaces;
using AppStore.Infraestructure.Data;
using System.Threading.Tasks;

namespace AppStore.Infrastructure.Repositories
{
    public class UniOfWork : IUniOfWork
    {
        private readonly AppStoreDbContext _context;
        private readonly IRepository<Product> _productRepository;

        public UniOfWork(AppStoreDbContext context, IRepository<Product> productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public IRepository<Product> ProductRespository => _productRepository ?? new BaseRepository<Product>(_context);

        public void Dispose()
        {
            if(_context != null)
            {
                _context.Dispose();
            }
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
