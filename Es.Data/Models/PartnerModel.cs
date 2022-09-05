using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Es.Data.Models
{
    public class EsPartnerModel
    {
        public int? MemberId { get; private set; }
        public string CardNumber { get; set; }
        public string CardNumberFormating { get; set; }

        public EsPartnerModel(int? memberId)
        {
            MemberId = memberId;
        }
    }
}
