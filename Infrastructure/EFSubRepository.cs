using System.Data.Entity;
using System.Linq;

namespace Infrastructure
{
    public class EFSubRepository<T> where T : class
    {
        private readonly DbContext dbContext;

        public EFSubRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Add(T entity)
        {
            dbContext.Set<T>().Add(entity);
        }

        public void Remove(T entity)
        {
            dbContext.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            dbContext.Entry(entity).State = EntityState.Modified;
        }

        public T Get(int id)
        {
            return dbContext.Set<T>().Find(id);
        }

        public IQueryable<T> GetAll()
        {
            return dbContext.Set<T>();
        }
    }
}
