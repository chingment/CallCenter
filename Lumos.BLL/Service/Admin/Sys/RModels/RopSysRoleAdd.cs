using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos.Entity;

namespace Lumos.BLL.Service.Admin
{
    public class RopSysRoleAdd
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Enumeration.BelongSite BelongSite { get; set; }
    }
}
