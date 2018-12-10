using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RopObCalloutCarInsSubmitInsure
    {
        public string CustomerId { get; set; }

        public decimal CommercialPrice { get; set; }

        public decimal TravelTaxPrice { get; set; }

        public decimal CompulsoryPrice { get; set; }
    }
}
