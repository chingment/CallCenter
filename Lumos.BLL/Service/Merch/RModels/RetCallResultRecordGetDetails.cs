using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RetCallResultRecordGetDetails
    {
        public RetCallResultRecordGetDetails()
        {
            this.CallRecords = new List<object>();
        }

        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string SalesmanName { get; set; }

        public string ResultName { get; set; }
        public string NextCallTime { get; set; }
        public string Remark { get; set; }

        public List<object> CallRecords { get; set; }
    }
}
