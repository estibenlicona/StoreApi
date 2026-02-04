using Microsoft.EntityFrameworkCore;
using AppStore.Core.Entities;
using AppStore.Infrastructure.Data.Config;

namespace AppStore.Infraestructure.Data
{
    public class AppStoreDbContext : DbContext
    {
        public AppStoreDbContext(DbContextOptions<AppStoreDbContext> options)
            : base(options)
        {
            
        }

        public virtual DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ProductConfig());
        }
    }
}
