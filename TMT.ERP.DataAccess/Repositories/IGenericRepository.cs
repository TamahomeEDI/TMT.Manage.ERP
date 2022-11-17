using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.DataAccess.Repositories
{
    public interface IGenericRepository<T> : IRepository
    {
        void SetTrackingOption(bool noTracking);
        int Count();
        IList<T> GetPaged(int maximumRows, int startRowIndex, out int totalRowCount, string sortExpression, Expression<Func<T, bool>> filter = null, Expression<Func<T, object>>[] includeProperties = null);
        IList<T> GetPaged(int maximumRows, int startRowIndex, out int totalRowCount, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, Expression<Func<T, bool>> filter = null, Expression<Func<T, object>>[] includeProperties = null);
        IList<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Expression<Func<T, object>>[] includeProperties = null);
        IList<T> Get(string query, params object[] parameters);
        T FindById(object id);
        T SingleOrDefault(Expression<Func<T, bool>> where, Expression<Func<T, object>>[] includeProperties = null);
        T FirstOrDefault(Expression<Func<T, bool>> where, Expression<Func<T, object>>[] includeProperties = null);
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        void Attach(T entity);
    }
}
