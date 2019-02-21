using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RupCallResultRecordGetList : RupBaseGetList
    {
        public  string SalesmanName { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }

        public string ResultCode { get; set; }
    }
}
