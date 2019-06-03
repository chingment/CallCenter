using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public class RetObBatchGetDetails
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string SoureName { get; set; }

        public int ValidCount { get; set; }

        public int InValidCount { get; set; }

        public string ExpiryTime { get; set; }

        public string RecoveryTime { get; set; }

        public int FollowDelayDays { get; set; }

        public string HandleReport { get; set; }

        public string BelongerName { get; set; }

        public int DataCount { get; set; }

        public Entity.Enumeration.BusinessType BusinessType { get; set; }

    }
}
