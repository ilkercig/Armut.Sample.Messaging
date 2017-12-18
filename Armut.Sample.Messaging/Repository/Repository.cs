using Armut.Sample.Messaging.Data;
using Armut.Sample.Messaging.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Armut.Sample.Messaging.Repository
{
    public abstract class Repository<T> where T: ModelBase
    {
        protected readonly MessagingContext m_DbContext;

        public Repository(MessagingContext dbContext)
        {
            m_DbContext = dbContext;
        }

        public virtual T GetById(int id)
        {
            return m_DbContext.Set<T>().Find(id);
        }

        public virtual IEnumerable<T> List()
        {
            return m_DbContext.Set<T>().AsEnumerable();
        }

        public virtual IEnumerable<T> List(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return m_DbContext.Set<T>()
                   .Where(predicate)
                   .AsEnumerable();
        }

        public T Insert(T entity)
        {
            T result = m_DbContext.Set<T>().Add(entity).Entity;
            m_DbContext.SaveChanges();
            return result;
        }

        public void Update(T entity)
        {
            m_DbContext.Entry(entity).State = EntityState.Modified;
            m_DbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            m_DbContext.Set<T>().Remove(entity);
            m_DbContext.SaveChanges();
        }
    }

}
