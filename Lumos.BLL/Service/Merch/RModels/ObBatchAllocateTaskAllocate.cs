using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos.Entity;

namespace Lumos.BLL.Service.Merch
{
    public class ObBatchAllocateTaskAllocate
    {
        public string Id { get; set; }

        public List<BelongUser> BelongUsers { get; set; }

        public Enumeration.ObBatchAllocateTaskAllocateMode Mode { get; set; }

        public class BelongUser
        {
            public string Id { get; set; }

            public string OrganizationId { get; set; }

            public int AllocatedCount { get; set; }
        }
    }
}
