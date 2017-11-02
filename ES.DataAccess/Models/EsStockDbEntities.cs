using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ES.DataAccess.Models
{
    public partial class EsStockDbEntities
    {
        public EsStockDbEntities(string connectionString): base(connectionString)
        {

        }
    }
}
