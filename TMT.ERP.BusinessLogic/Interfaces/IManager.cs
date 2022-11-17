using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMT.ERP.DataAccess.Model;

namespace TMT.ERP.BusinessLogic.Interfaces
{
    public interface IManager : IDisposable
    {
        void ReflectEntityState(Entity entity);
        void DetachEntity(Entity entity);
        string TrackChanges(Entity entity);
        void Add(object entity);
        void Delete(object entity);
        void Update(object entity);
        void Attach(object entity);
        void Detach(object entity);
        EntityState GetState(object entity);
        void Save();
    }
}
