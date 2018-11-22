using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public class RopBizMenuAdd
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string PMenuId { get; set; }
        public Enumeration.BusinessType BusinessType { get; set; }
        public string[] PermissionIds { get; set; }
    }
}
