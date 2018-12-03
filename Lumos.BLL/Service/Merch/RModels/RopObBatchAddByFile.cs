using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RopObBatchAddByFile
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public int RecoveryDays { get; set; }

        public int ExpiryDays { get; set; }

        public int FollowDelayDays { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public Enumeration.DataBatchBizType BizType { get; set; }

        public string BelongId { get; set; }
    }

}
