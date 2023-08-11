using System.Linq.Expressions;

namespace Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {

        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);

        void DetachAll();

    }
}
