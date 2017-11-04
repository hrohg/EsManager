using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Data.Models
{
    public class MemberRoleModel : ModelBase
    {
        private bool _isSelected;

        #region External properties
        public long Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value;
                RaisePropertyChanged("IsSelected"); 
            }
        }

        #endregion External properties

        #region Contructors
        
        #endregion Constructors
    }
}
