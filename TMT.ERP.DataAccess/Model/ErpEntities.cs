using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMT.ERP.DataAccess.Model
{
    public partial class ErpEntities
    {
        public ErpEntities(System.Data.Common.DbConnection conn)
            : base(conn, true)
        {

        }
    }
}
