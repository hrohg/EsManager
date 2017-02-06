using System;
using System.Collections.Generic;
using ES.DataAccess.Helpers;
using ES.DataAccess.Models;

namespace ES.Business.Managers
{
    public class MembersManager : ManagerBase
    {
        public static List<ESSharedProducts> GetProducts()
        {
            using (var db = GetDataContext())
            {
                try
                {
                    return null;
                }
                catch (Exception)
                {
                    return new List<ESSharedProducts>();
                }
                
            }
            
        } 
    }
}
