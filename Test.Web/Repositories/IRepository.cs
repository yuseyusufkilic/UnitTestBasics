using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Test.Web.Repositories
{
    public interface IRepository<T> where T:class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(int id);
        Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);

        //update delete asenkron hali yok. çünkü memory'e bir şey yansıtılmıyor stateleri değişiyor.
        

    }
}
