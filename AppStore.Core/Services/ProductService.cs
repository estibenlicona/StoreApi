using AppStore.Core.Entities;
using AppStore.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IUniOfWork _uniOfWork;

        public ProductService(IUniOfWork uniOfWork)
        {
            _uniOfWork = uniOfWork;
        }

        public IEnumerable<Product> List()
        {
            return  _uniOfWork.ProductRespository.SelectAll();
        }

        public async Task<Product> Get(int Id)
        {
            return await _uniOfWork.ProductRespository.Select(Id);
        }

        public async Task Create(Product entity)
        {
            await _uniOfWork.ProductRespository.Insert(entity);
            await _uniOfWork.SaveChangesAsync();
        }

        public async Task<bool> Edit(Product entity)
        {
            _uniOfWork.ProductRespository.Update(entity);
            await _uniOfWork.SaveChangesAsync();
            return true; // Machete
        }

        public async Task<bool> Remove(int Id)
        {
            await _uniOfWork.ProductRespository.Delete(Id);
            await _uniOfWork.SaveChangesAsync();
            return true; // Machete
        }
    }
}
