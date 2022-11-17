using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TMT.ERP.DataAccess;
using TMT.ERP.DataAccess.Repositories;
using TMT.ERP.BusinessLogic.Interfaces;
using TMT.ERP.DataAccess.Model;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;

namespace TMT.ERP.BusinessLogic.Managers
{
    /// <summary>
    /// Generic manager implement the basic methods to manage entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericManager<T> : Manager, IGenericManager<T> where T : Entity
    {
        protected IGenericRepository<T> GenericRepository { get; private set; }

        public GenericManager(string contextKey = null, Manager manager = null)
            : base(contextKey: contextKey, manager: manager)
        {
            GenericRepository = UnitOfWork.CreateRepository<T>();
        }

        #region IGenericManager<T> Members

        public void SetTrackingOption(bool noTracking)
        {
            GenericRepository.SetTrackingOption(noTracking);
        }

        public virtual int Count()
        {
            return GenericRepository.Count();
        }

        public virtual IList<T> GetPaged(int maximumRows, int startRowIndex, out int totalRowCount, string sortExpression, Expression<Func<T, bool>> filter = null, Expression<Func<T, object>>[] includeProperties = null)
        {
            return GenericRepository.GetPaged(maximumRows, startRowIndex, out totalRowCount, sortExpression, filter, includeProperties);
        }

        public virtual IList<T> GetPaged(int maximumRows, int startRowIndex, out int totalRowCount, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, Expression<Func<T, bool>> filter = null, Expression<Func<T, object>>[] includeProperties = null)
        {
            return GenericRepository.GetPaged(maximumRows, startRowIndex, out totalRowCount, orderBy, filter, includeProperties);
        }

        public virtual IList<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Expression<Func<T, object>>[] includeProperties = null)
        {
            return GenericRepository.Get(filter, orderBy, includeProperties);
        }

        public virtual IList<T> Get(string query, params object[] parameters)
        {
            return GenericRepository.Get(query, parameters);
        }

        public virtual T FindById(object id)
        {
            return GenericRepository.FindById(id);
        }

        public virtual T Find(Expression<Func<T, bool>> where, Expression<Func<T, object>>[] includeProperties = null)
        {
            return GenericRepository.FirstOrDefault(where, includeProperties);
        }

        public virtual void Add(T entity)
        {
            base.Add(entity);
        }

        public virtual void Delete(T entity)
        {
            base.Delete(entity);
        }

        public virtual void Update(T entity)
        {
            base.Update(entity);
        }

        public virtual void Attach(T entity)
        {
            base.Attach(entity);
        }

        #endregion
    }
}
