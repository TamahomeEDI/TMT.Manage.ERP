using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using LinqKit;

namespace TMT.ERP.DataAccess.Repositories
{
    public class GenericRepository<T> : Repository, IGenericRepository<T> where T : class
    {
        protected DbSet<T> DbSet { get; private set; }
        private bool noTracking = false;

        protected internal GenericRepository(DbContext context) :
            base(context)
        {
            DbSet = DbContext.Set<T>();
        }


        #region IGenericRepository<T> Members

        public void SetTrackingOption(bool noTracking)
        {
            this.noTracking = noTracking;
        }

        public virtual int Count()
        {
            return DbSet.Count();
        }

        public virtual IList<T> GetPaged(int maximumRows, int startRowIndex, out int totalRowCount, string sortExpression, Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includeProperties)
        {
            totalRowCount = filter == null ? DbSet.Count() : DbSet.AsExpandable().Where(filter).Count();

            IQueryable<T> query = DbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (filter != null)
            {
                query = query.AsExpandable<T>().Where(filter);
            }

            if (noTracking)
            {
                query = query.AsNoTracking<T>();
            }

            return query.OrderBy(sortExpression).Skip(startRowIndex).Take(maximumRows).ToList();
        }

        public virtual IList<T> GetPaged(int maximumRows, int startRowIndex, out int totalRowCount, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, Expression<Func<T, bool>> filter = null, Expression<Func<T, object>>[] includeProperties = null)
        {
            totalRowCount = filter == null ? DbSet.Count() : DbSet.AsExpandable().Where(filter).Count();

            IQueryable<T> query = DbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (filter != null)
            {
                query = query.AsExpandable<T>().Where(filter);
            }

            if (noTracking)
            {
                query = query.AsNoTracking<T>();
            }

            return orderBy(query).Skip(startRowIndex).Take(maximumRows).ToList();
        }

        public virtual IList<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (filter != null)
            {
                query = query.AsExpandable().Where(filter);
            }

            if (noTracking)
            {
                query = query.AsNoTracking<T>();
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual IList<T> Get(string query, params object[] parameters)
        {
            var result = DbSet.SqlQuery(query, parameters);

            if (noTracking)
            {
                result = result.AsNoTracking();
            }

            return result.ToList();
        }

        public virtual T FindById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> where, Expression<Func<T, object>>[] includeProperties = null)
        {
            IQueryable<T> query = DbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (noTracking)
            {
                query = query.AsNoTracking<T>();
            }

            return query.SingleOrDefault(where);
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> where, Expression<Func<T, object>>[] includeProperties = null)
        {

            IQueryable<T> query = DbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (noTracking)
            {
                query = query.AsNoTracking<T>();
            }

            return query.FirstOrDefault(where);
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
