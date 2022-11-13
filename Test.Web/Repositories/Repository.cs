using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Web.Repositories
{
    public class Repository<T>:IRepository<T> where T: class
    {
        private UnitTestMVCDBContext _context;
        private readonly DbSet<T> _dbSet; //entity'nin tablosunu tutuyor bu arkadaş.

        public Repository(UnitTestMVCDBContext context)
        {
            _context = context;
            _dbSet=_context.Set<T>();
            
        }

        public async Task Create(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<System.Collections.Generic.IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Update(T entity)
        {
            _context.Entry<T>(entity).State= EntityState.Modified;
            //_dbSet.Update(entity); // yukarda modified dediğimiz için zaten, aslında böyle yazmaya gerek yok.
            _context.SaveChanges();

        }
    }
}
