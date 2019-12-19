using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using noHRforIT.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace noHRforIT.Business.Services
{
    public abstract class BaseService<T> where T : class, new()
    {
        private readonly ApplicationDbContext _dbContext;

        public BaseService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public virtual int Count()
        {
            return _dbContext.Set<T>().Count();
        }
        public virtual IEnumerable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }

            return query.AsEnumerable();
        }

        public T GetSingle(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public T GetSingleWithConstraint(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().FirstOrDefault(predicate);
        }

        public T GetSingleWithConstraint(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            foreach (var property in includeProperties)
            {
                query = query.Include(property);
            }

            return query.Where(predicate).FirstOrDefault();
        }

        public virtual IEnumerable<T> FindByPredicate(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate);
        }

        public virtual void Add(T entity)
        {
            EntityEntry dbEntityEntry = _dbContext.Entry<T>(entity);
            _dbContext.Set<T>().Add(entity);
        }

        public virtual void Update(T entity)
        {
            EntityEntry dbEntityEntry = _dbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Modified;
        }
        public virtual void Delete(T entity)
        {
            EntityEntry dbEntityEntry = _dbContext.Entry<T>(entity);
            dbEntityEntry.State = EntityState.Deleted;
        }

        public virtual void DeleteByPredicate(Expression<Func<T, bool>> predicate)
        {
            IEnumerable<T> entities = _dbContext.Set<T>().Where(predicate);

            foreach (var entity in entities)
            {
                _dbContext.Entry<T>(entity).State = EntityState.Deleted;
            }
        }

        public virtual void Commit()
        {
            _dbContext.SaveChanges();
        }
    }
}
