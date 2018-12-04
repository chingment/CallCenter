using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos.Entity;

namespace Lumos.BLL.Service.Merch
{
    public class RopObBatchAllocateAdd
    {
        public RopObBatchAllocateAdd()
        {
            this.BelongUsers = new List<BelongUser>();
            this.Filters = new RupObCustomerGetList();
        }

        public string Id { get; set; }

        public List<BelongUser> BelongUsers { get; set; }

        public Enumeration.ObBatchAllocateMode Mode { get; set; }

        public string Description { get; set; }

        public class BelongUser
        {
            public string UserId { get; set; }

            public string OrganizationId { get; set; }

            public int AllocatedCount { get; set; }
        }

        public RupObCustomerGetList Filters { get; set; }
    }
}
