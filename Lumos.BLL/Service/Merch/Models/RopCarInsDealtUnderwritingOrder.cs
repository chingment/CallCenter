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
        public string UnAuditComments { get; set; }

        public Entity.Enumeration.OperateType Operate { get; set; }
        public string UnPayImgUrl { get; set; }
        public string UnOrderImgUrl { get; set; }
        public decimal UnCompulsoryAmount { get; set; }
        public decimal UnTravelTaxAmount { get; set; }
        public decimal UnCommercialAmount { get; set; }

    }
}
