using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.DataAccess.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private DbContext dbContext;               

        public UnitOfWork(DbContext context)
        {
            dbContext = context;
        }

        public IGenericRepository<T> CreateRepository<T>() where T : class
        {
            return new GenericRepository<T>(dbContext);
        }

        public IRepository CreateRepository()
        {
            return new Repository(dbContext);
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (dbContext != null)
                dbContext.Dispose();
        }

        #endregion
    }
}
