using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.DataAccess.Repositories
{
    public interface IRepository
    {
        void Add(object entity);
        void Delete(object entity);
        void Update(object entity);
        void Attach(object entity);
        void Detach(object entity);
        void ChangeState(object entity, EntityState state);
        EntityState GetState(object entity);
        void Save();
    }
}
