using DAL;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Core
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly KlaContext _context;
        protected readonly DbSet<TEntity> _entities;

        

        public Repository(KlaContext context)
        {
            _context = context;
            _entities = _context.Set<TEntity>();
        }


        public IEnumerable<TEntity> GetAll()
        {
            // .AsNoTracking()
            return _entities.AsNoTracking().ToList();
            
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.AsNoTracking().Where(predicate).ToList();
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.SingleOrDefault(predicate);
        }

        public void Add(TEntity entity)
        {
            _entities.Add(entity);

        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            _entities.AddRange(entities);

        }
    }

}
