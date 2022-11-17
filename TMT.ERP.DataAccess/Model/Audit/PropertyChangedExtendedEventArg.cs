using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.DataAccess.Model.Audit
{
    public class PropertyChangedExtendedEventArgs : PropertyChangedEventArgs
    {
        public virtual object OldValue { get; private set; }
        public virtual object NewValue { get; private set; }

        public PropertyChangedExtendedEventArgs(string propertyName, object oldValue, object newValue)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
