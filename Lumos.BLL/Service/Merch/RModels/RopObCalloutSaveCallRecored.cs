using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RopObCalloutSaveCallResultRecord
    {
        public string CustomerId { get; set; }

        public string ResultCode { get; set; }

        public DateTime? NextCallTime { get; set; }

        public string Remark { get; set; }
    }
}
