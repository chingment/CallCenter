using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RopObCalloutCarInsSubmitUnderwriting
    {
        public string ObCustomerId { get; set; }

        public decimal TravelTaxAmount { get; set; }

        public decimal CompulsoryAmount { get; set; }

        public decimal CommercialAmount { get; set; }

        public List<CarInsKindModel> Kinds { get; set; }

        public class CarInsKindModel
        {
            public string Id { get; set; }

            public bool IsWaiverDeductible { get; set; }

            public string Value { get; set; }

            public string Details { get; set; }

        }

    }
}
