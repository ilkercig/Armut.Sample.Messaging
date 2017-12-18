using Armut.Sample.Messaging.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Armut.Sample.Messaging.Repository
{
    public interface IRepository<T> where T : ModelBase
    {
        T GetById(int id);
        IEnumerable<T> List();
        IEnumerable<T> List(Expression<Func<T, bool>> predicate);
        T Insert(T entity);
        void Delete(T entity);
        void Update(T entity);
    }
}