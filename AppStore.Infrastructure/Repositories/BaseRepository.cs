using AppStore.Core.Entities;
using AppStore.Core.Interfaces;
using AppStore.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Infrastructure.Repositories
{
    public class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        private DbSet<T> _entities;

        public BaseRepository(AppStoreDbContext context)
        {
            _entities = context.Set<T>();
        }

        public IEnumerable<T> SelectAll()
        {
            return _entities.AsEnumerable();
        }

        public async Task<T> Select(int Id)
        {
            return await _entities.FindAsync(Id);
        }

        public async Task Insert(T entity)
        {
            await _entities.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _entities.Update(entity);
        }

        public async Task Delete(int id)
        {
            T entity = await Select(id);
            _entities.Remove(entity);
        }

    }
}
