using System;
using AppStore.Core.Interfaces;
using AppStore.Core.Entities;
using System.Threading.Tasks;

namespace AppStore.Core.Interfaces
{
    public interface IUniOfWork : IDisposable
    {
        IRepository<Product> ProductRespository { get; }
        
        void SaveChanges();

        Task SaveChangesAsync();
    }
}