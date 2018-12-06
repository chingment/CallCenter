using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RopObCalloutSaveCallRecored
    {
        public string ObCustomerId { get; set; }

        public string ResultCode { get; set; }

        public DateTime? NextCallTime { get; set; }

        public string Remark { get; set; }

        public string TakerId { get; set; }
    }
}
