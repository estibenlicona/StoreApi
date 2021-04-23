using AppStore.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppStore.Core.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> SelectAll();

        Task<T> Select(int Id);

        Task Insert(T entity);

        void Update(T entity);

        Task Delete(int Id);
    }
}
