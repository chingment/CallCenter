using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class CallResultRecordModel
    {
        public string ResultName { get; set; }
        public string NextCallTime { get; set; }
        public string Remark { get; set; }

        public string CreateTime { get; set; }
    }
}
