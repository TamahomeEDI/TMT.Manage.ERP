using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib;

namespace TMT.ERP.DataAccess.Repositories
{
    public class Repository : IRepository
    {
        protected DbContext DbContext { get; private set; }
        protected ObjectContext ObjContext { get { return ((IObjectContextAdapter)DbContext).ObjectContext; } }

        protected internal Repository(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            DbContext = context;
            ObjContext.CommandTimeout = 120;
        }


        #region IRepository Members

        public void Add(object entity)
        {
            ChangeState(entity, System.Data.EntityState.Added);
        }

        public void Delete(object entity)
        {
            ChangeState(entity, System.Data.EntityState.Deleted);
        }

        public void Update(object entity)
        {
            ChangeState(entity, System.Data.EntityState.Modified);
        }

        public void Attach(object entity)
        {
            ChangeState(entity, System.Data.EntityState.Unchanged);
        }

        public void Detach(object entity)
        {
            ChangeState(entity, EntityState.Detached);
        }

        public void ChangeState(object entity, EntityState state)
        {
            if (entity != null)
                DbContext.Entry(entity).State = state;
        }

        public EntityState GetState(object entity)
        {
            return DbContext.Entry(entity).State;
        }

        public void Save()
        {
            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
