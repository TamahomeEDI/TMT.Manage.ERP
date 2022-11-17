using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TMT.ERP.BusinessLogic.Interfaces;
using TMT.ERP.DataAccess.Repositories;
using TMT.ERP.DataAccess.Model;
using CommonLib;

namespace TMT.ERP.BusinessLogic.Managers
{
    public class Manager : IManager
    {
        private static object _lock = new object();
        private string contextKey;
        public bool IsDisposed { get; private set; }
        protected ErpEntities DbContext { get; private set; }
        protected UnitOfWork UnitOfWork { get; private set; }
        protected IRepository Repository { get; private set; }

        public bool LazyLoadingEnabled
        {
            get { return DbContext.Configuration.LazyLoadingEnabled; }
            set { DbContext.Configuration.LazyLoadingEnabled = value; }
        }

        public Manager(string contextKey = null, Manager manager = null)
        {
            if (manager == null)
            {
                this.contextKey = contextKey;
                if (this.contextKey == null)
                    this.contextKey = ConfigurationManager.AppSettings["HttpContextDbContextKey"];
                Initialize();
            }
            else
            {
                this.DbContext = manager.DbContext;
                this.UnitOfWork = manager.UnitOfWork;
                this.Repository = manager.Repository;
            }
        }

        private void Initialize()
        {
            lock (_lock)
            {
                //Use 1 and only context per request for web-based app
                if (DbContext == null && HttpContext.Current != null && !string.IsNullOrEmpty(contextKey))
                {
                    var context = HttpContext.Current.Items[contextKey] as ErpEntities;
                    if (context == null)
                    {
                        context = new ErpEntities();
                        HttpContext.Current.Items[contextKey] = context;
                    }
                    DbContext = context;
                }
                else
                {
                    DbContext = new ErpEntities();
                }

                UnitOfWork = new UnitOfWork(DbContext);
                Repository = UnitOfWork.CreateRepository();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!IsDisposed)
            {
                if (DbContext != null)
                {
                    //Disable lazy loading as context is gonna be disposed
                    DbContext.Configuration.LazyLoadingEnabled = false;
                    DbContext.Dispose();

                    IsDisposed = true;
                    DbContext = null;

                    //Remove context key from HttpRequest
                    if (HttpContext.Current != null)
                    {
                        HttpContext.Current.Items.Remove(contextKey);
                    }
                }
            }
        }

        #endregion

        #region IManager Members

        public void ReflectEntityState(Entity entity)
        {
            //Disable lazy loading temporary to make sure EF doesn't reload entities unexpectedly
            DbContext.Configuration.LazyLoadingEnabled = false;
            SaveEntityStateRecursive(entity);
            DbContext.Configuration.LazyLoadingEnabled = true;
        }

        private void SaveEntityStateRecursive(Entity entity)
        {
            //Only change state if entity's state is not unchanged
            if (entity.EntityState != System.Data.EntityState.Unchanged)
            {
                //Reset tracking log
                entity.IsTracked = false;
                //Change state
                Repository.ChangeState(entity, entity.EntityState);
                //Set entity's state to unchanged to prevent recursive references
                entity.EntityState = System.Data.EntityState.Unchanged;
                //Loop through entity's properties
                foreach (var prop in entity.GetType().GetProperties())
                {
                    //Pay no attention to read-only property
                    if (!prop.CanWrite)
                        continue;

                    //If property is enumerable
                    if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                    {
                        //Get property values
                        IEnumerable propValues = prop.GetValue(entity) as IEnumerable;
                        if (propValues != null)
                        {
                            var values = propValues.OfType<Entity>().ToList();
                            for (int i = 0; i < values.Count(); i++)
                            {
                                var value = values[i];
                                //Check if enumerable value is Entity
                                if (typeof(Entity).IsAssignableFrom(value.GetType()))
                                {
                                    SaveEntityStateRecursive(value as Entity);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Check if property is Entity
                        if (typeof(Entity).IsAssignableFrom(prop.PropertyType))
                        {
                            Entity value = prop.GetValue(entity) as Entity;
                            if (value != null)
                            {
                                SaveEntityStateRecursive(value as Entity);
                            }
                        }
                    }
                }
            }
        }

        public void DetachEntity(Entity entity)
        {
            //Disable lazy loading temporary to make sure EF doesn't reload entities unexpectedly
            DbContext.Configuration.LazyLoadingEnabled = false;
            DetachEntityRecursive(entity);
            DbContext.Configuration.LazyLoadingEnabled = true;
        }

        private void DetachEntityRecursive(Entity entity)
        {
            if (entity == null || entity.EntityState == EntityState.Detached)
                return;

            //Set entity's state to detach to prevent recursive references
            entity.EntityState = System.Data.EntityState.Detached;
            //Loop through entity's properties
            foreach (var prop in entity.GetType().GetProperties())
            {
                //Pay no attention to read-only property
                if (!prop.CanWrite)
                    continue;

                //If property is enumerable
                if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    //Get property values
                    IEnumerable propValues = prop.GetValue(entity) as IEnumerable;
                    if (propValues != null)
                    {
                        var values = propValues.OfType<Entity>().ToList();
                        for (int i = 0; i < values.Count(); i++)
                        {
                            var value = values[i];
                            //Check if enumerable value is Entity
                            if (typeof(Entity).IsAssignableFrom(value.GetType()))
                            {
                                DetachEntityRecursive(value as Entity);
                            }
                        }
                    }
                }
                else
                {
                    //Check if property is Entity
                    if (typeof(Entity).IsAssignableFrom(prop.PropertyType))
                    {
                        Entity value = prop.GetValue(entity) as Entity;
                        if (value != null)
                        {
                            DetachEntityRecursive(value as Entity);
                        }
                    }
                }
            }

            //Detach the entity last to prevent losing of referenced entities
            Repository.Detach(entity);
        }

        public string TrackChanges(Entity entity)
        {
            LazyLoadingEnabled = false;
            string delimiter = Environment.NewLine;
            string indent = "\t";
            string changes = TrackChangesRecursive(entity, delimiter, indent);
            LazyLoadingEnabled = true;
            return changes;
        }

        private string TrackChangesRecursive(Entity entity, string delimiter, string indent)
        {
            if (entity.IsTracked)
                return string.Empty;

            entity.IsTracked = true;

            string changes = string.Empty;
            string format;

            switch (entity.EntityState)
            {
                case System.Data.EntityState.Added: format = "{0}{1} : {3}"; changes += TrackPropertiesChanged(entity, format, delimiter, indent); break;
                case System.Data.EntityState.Modified: format = "{0}{1} : {2} => {3}"; changes += TrackPropertiesChanged(entity, format, delimiter, indent); break;
                case System.Data.EntityState.Deleted: changes = "Deleted " + System.Data.Objects.ObjectContext.GetObjectType(entity.GetType()).Name + " : " + entity.ToString() + delimiter; break;
                case System.Data.EntityState.Detached: break;
                case System.Data.EntityState.Unchanged: break;
            }

            //entity.IsTracked = false;

            return changes;
        }

        private string TrackPropertiesChanged(Entity entity, string format, string delimiter, string indent)
        {
            string action = entity.EntityState.ToString();
            string entityName = System.Data.Objects.ObjectContext.GetObjectType(entity.GetType()).Name;
            string changes = string.Format("{0} {1}{2}", action, entityName, delimiter);
            bool changeFlag = false;

            //Track changes for simple property 
            foreach (var modifiedProperty in entity.ModifiedProperties)
            {
                var oldValue = GetPropertyValue(modifiedProperty.Value.Item1);
                var newValue = GetPropertyValue(modifiedProperty.Value.Item2);

                if (string.IsNullOrWhiteSpace(oldValue) && string.IsNullOrWhiteSpace(newValue))
                    continue;

                changes += string.Format(format, indent, modifiedProperty.Key, oldValue, newValue);
                changes += delimiter;

                changeFlag = true;
            }

            //Track changes for reference property
            foreach (var refProperty in entity.GetType().GetProperties())
            {
                if (!refProperty.CanWrite)
                    continue;
                //If property is collection
                if (typeof(IEnumerable<Entity>).IsAssignableFrom(refProperty.PropertyType))
                {
                    var propValue = refProperty.GetValue(entity);
                    if (propValue != null)
                    {
                        foreach (Entity item in (propValue as IEnumerable<Entity>))
                        {
                            if (item != null)
                            {
                                string recursiveChanges = TrackChangesRecursive(item, delimiter, indent);
                                changes += recursiveChanges;
                                if (!string.IsNullOrWhiteSpace(recursiveChanges))
                                    changeFlag = true;
                            }
                        }
                    }
                }
                else if (typeof(Entity).IsAssignableFrom(refProperty.PropertyType))
                {
                    var refEntity = refProperty.GetValue(entity) as Entity;
                    if (refEntity != null)
                    {
                        string recursiveChanges = TrackChangesRecursive(refEntity, delimiter, indent);
                        changes += recursiveChanges;
                        if (!string.IsNullOrWhiteSpace(recursiveChanges))
                            changeFlag = true;
                    }
                }
            }

            return changeFlag ? changes : string.Empty;
        }

        private string GetPropertyValue(object prop)
        {
            if (prop == null)
                return string.Empty;
            if (prop is Entity)
            {
                return (prop as Entity).EntityState == System.Data.EntityState.Unchanged ? prop.ToString() : string.Empty;
            }
            else
            {
                return prop.ToString();
            }
        }

        public void Add(object entity)
        {
            Repository.Add(entity);
        }

        public void Delete(object entity)
        {
            Repository.Delete(entity);
        }

        public void Update(object entity)
        {
            Repository.Update(entity);
        }

        public void Attach(object entity)
        {
            Repository.Attach(entity);
        }

        public void Detach(object entity)
        {
            Repository.Detach(entity);
        }

        public EntityState GetState(object entity)
        {
            return Repository.GetState(entity);
        }

        public void Save()
        {
            Repository.Save();
        }

        #endregion
    }
}
