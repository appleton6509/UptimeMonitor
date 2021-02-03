using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IRepository<T> where T : IBaseModel
    {
        public Task<int> PutAsync(Guid id, T model);
        public Task<int> PostAsync(T model);
        public T Get(Guid id);
        public List<T> GetAll();
        public Task<int> DeleteAsync(Guid id);
        public bool Exists(Guid id);
        public Guid UserId();

    }
}
