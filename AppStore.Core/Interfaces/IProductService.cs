using AppStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Core.Interfaces
{
    public interface IProductService
    {
        IEnumerable<Product> List();

        Task<Product> Get(int Id);

        Task Create(Product product);

        Task<bool> Edit(Product product);

        Task<bool> Remove(int Id);
    }
}
