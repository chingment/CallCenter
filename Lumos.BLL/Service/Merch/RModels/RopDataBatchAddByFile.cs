using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RopDataBatchAddByFile
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public Enumeration.DataBatchBizType BizType { get; set; }
    }

}
