using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ES.DataAccess.Models;

namespace ES.Business.Managers
{
    public class ProductsManager : ManagerBase
    {
        public static List<ESSharedProducts> GetEsSharedProductses()
        {
            using (var db = GetDataContext())
            {
                try
                {
                    return db.ESSharedProducts.ToList();
                }
                catch (Exception)
                {
                    return new List<ESSharedProducts>();
                }

            }
        }
    }
}
