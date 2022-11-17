using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.DataAccess.Repositories
{
    public static class ObjectContextExtension
    {
        public static string GetEntitySetName(this ObjectContext context, object entity)
        {
            Type entityType = ObjectContext.GetObjectType(entity.GetType());
            if (entityType == null)
                throw new InvalidOperationException("not an entity");

            EntityContainer container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);
            return (from entitySet in container.BaseEntitySets
                    where entitySet.ElementType.Name.Equals(entityType.Name)
                    select entitySet.Name).Single();
        }

        public static void AttachToOrGet(this ObjectContext context, string entitySetName, ref object entity)
        {
            ObjectStateEntry entry;
            // Track whether we need to perform an attach
            bool attach = false;
            if (
                context.ObjectStateManager.TryGetObjectStateEntry
                    (
                        context.CreateEntityKey(entitySetName, entity),
                        out entry
                    )
                )
            {
                // Re-attach if necessary
                attach = entry.State == EntityState.Detached;
                // Get the discovered entity to the ref
                entity = entry.Entity;
            }
            else
            {
                // Attach for the first time
                attach = true;
            }
            if (attach)
                context.AttachTo(entitySetName, entity);
        }
    }
}
