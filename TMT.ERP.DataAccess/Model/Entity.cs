using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMT.ERP.DataAccess.Model.Audit;
using CommonLib;
using System.Collections;

namespace TMT.ERP.DataAccess.Model
{
    public abstract class Entity : INotifyPropertyChangedExtended
    {
        public System.Data.EntityState EntityState { get; set; }
        public readonly Dictionary<string, Tuple<object, object>> ModifiedProperties;
        public bool IsTracked { get; set; }

        protected Entity()
        {
            EntityState = System.Data.EntityState.Unchanged;
            PropertyChanged += Entity_PropertyChanged;
            ModifiedProperties = new Dictionary<string, Tuple<object, object>>();
        }

        void Entity_PropertyChanged(object sender, PropertyChangedExtendedEventArgs e)
        {
            if (EntityState == System.Data.EntityState.Deleted || EntityState == System.Data.EntityState.Detached)
                return;

            if (e.OldValue.IsNullOrDefault() && EntityState != System.Data.EntityState.Added)
                return;

            //if (e.OldValue is Entity || e.NewValue is Entity)
            //    return;

            if (e.PropertyName.EndsWith("ID", StringComparison.OrdinalIgnoreCase))
                return;

            ModifiedProperties[e.PropertyName] = Tuple.Create<object, object>(
                ModifiedProperties.ContainsKey(e.PropertyName) ? ModifiedProperties[e.PropertyName].Item1 : e.OldValue,
                e.NewValue);
        }

        protected void SetNotifyingProperty<T>(ref T field, T value, [CallerMemberName]string caller = null)
        {
            if ((field == null || !field.Equals(value)) && !(field == null && value == null))
            {
                T oldValue = field;
                field = value;
                OnPropertyChanged(this, new PropertyChangedExtendedEventArgs(caller, oldValue, value));
            }
        }

        public virtual void OnPropertyChanged(object sender, PropertyChangedExtendedEventArgs e)
        {
            PropertyChangedExtendedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(sender, e);
        }

        #region INotifyPropertyChangedExtended Members

        public event PropertyChangedExtendedEventHandler PropertyChanged;

        #endregion
    }
}
