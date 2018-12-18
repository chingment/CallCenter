using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lumos.BLL.Service.Merch
{
    public class RopCarInsDealtUnderwritingOrder
    {
        public string OrderId { get; set; }
        public string UnderwriterAuditComments { get; set; }

        public Entity.Enumeration.OperateType Operate { get; set; }

    }
}
