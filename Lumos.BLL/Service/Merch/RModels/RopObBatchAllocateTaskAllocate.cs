using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos.Entity;

namespace Lumos.BLL.Service.Merch
{
    public class RopObBatchAllocateTaskAllocate
    {
        public RopObBatchAllocateTaskAllocate()
        {
            this.BelongUsers = new List<BelongUser>();
            this.Filters = new RupObCustomerGetList();
        }

        public string Id { get; set; }

        public List<BelongUser> BelongUsers { get; set; }

        public Enumeration.ObBatchAllocateTaskAllocateMode Mode { get; set; }

        public class BelongUser
        {
            public string UserId { get; set; }

            public string OrganizationId { get; set; }

            public int AllocatedCount { get; set; }
        }

        public RupObCustomerGetList Filters { get; set; }
    }
}
