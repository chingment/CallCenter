using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RetObCalloutCarInsGetUnderwritingOrder
    {
        public RetObCalloutCarInsGetUnderwritingOrder()
        {
            this.UnderwritingOrders = new List<UnderwritingOrder>();
        }

        public List<UnderwritingOrder> UnderwritingOrders { get; set; }

        public class UnderwritingOrder
        {
            public string Customer { get; set; }

            public string Tips { get; set; }

            public string Url { get; set; }

            public string Underwriter { get; set; }
        }
    }
}
