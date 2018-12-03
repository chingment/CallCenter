using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RetObBatchAllocateTaskGetDetails
    {
        public RetObBatchAllocateTaskGetDetails()
        {
            this.BelongUsers = new List<BelongUser>();
        }

        public string Id { get; set; }
        public string ObBatchId { get; set; }
        public string ObBatchName { get; set; }
        public string ObBatchCode { get; set; }
        public int UnAllocatedCount { get; set; }

        public List<BelongUser> BelongUsers { get; set; }

        public class BelongUser
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public string OrganizationId { get; set; }
        }
    }
}
