using DAL;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System.Linq.Expressions;
using Utility_LOG;

namespace Repository.Core
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly KlaContext _context;
        private readonly LogManager _log;
        protected readonly DbSet<TEntity> _entities;

        public Repository(KlaContext context, LogManager log)
        {
            _context = context;
            _log = log;
            _entities = _context.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                _log.LogEvent("fetching all entities", LogProviderType.File);
               
                return _entities.AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                _log.LogException("Error fetching all entities", ex, LogProviderType.File);
                throw new Exception("Error fetching all entities", ex);
            }
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                _log.LogEvent("fetching entities based on the predicate", LogProviderType.File);
                return _entities.AsNoTracking().Where(predicate).ToList();
            }
            catch (Exception ex)
            {
                _log.LogException("Error fetching entities based on the predicate", ex, LogProviderType.File);
                throw new Exception("Error fetching entities based on the predicate", ex);
            }
        }

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                _log.LogEvent("fetching single or default entity based on the predicate", LogProviderType.File);
                return _entities.SingleOrDefault(predicate);
            }
            catch (Exception ex)
            {
                _log.LogException("Error fetching single or default entity based on the predicate", ex, LogProviderType.File);
                throw new Exception("Error fetching single or default entity based on the predicate", ex);
            }
        }

        public void Add(TEntity entity)
        {
            try
            {
                _log.LogEvent("adding entity", LogProviderType.File);
                _entities.Add(entity);
            }
            catch (Exception ex)
            {
                _log.LogException("Error adding entity", ex, LogProviderType.File);
                throw new Exception("Error adding entity", ex);
            }
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            try
            {
                _log.LogEvent("adding a range of entities", LogProviderType.File);
                _entities.AddRange(entities);
            }
            catch (Exception ex)
            {
                _log.LogException("Error adding a range of entities", ex, LogProviderType.File);
                throw new Exception("Error adding a range of entities", ex);
            }
        }

        public void DetachAll()
        {
            try
            {
                foreach (var entry in _context.ChangeTracker.Entries())
                {
                    if (entry.Entity is TEntity)
                    {
                        entry.State = EntityState.Detached;
                    }
                }
            }
            catch (Exception ex)
            {

                _log.LogException("Error in DetachAll method", ex, LogProviderType.File);
                throw new Exception("Error in DetachAll method", ex);
            }

            
        }
    }

}
