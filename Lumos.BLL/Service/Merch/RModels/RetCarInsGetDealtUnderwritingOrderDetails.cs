using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{

    public class RetCarInsGetDealtUnderwritingOrderDetails
    {
        public RetCarInsGetDealtUnderwritingOrderDetails()
        {
            this.Customer = new ObCustomerModel();
            this.Car = new CarInfoModel();
            this.CarInsKinds = new List<CarInsPKindModel>();
        }

        public ObCustomerModel Customer { get; set; }
        public CarInfoModel Car { get; set; }
        public string OfCompulsoryAmount { get; set; }
        public string OfTravelTaxAmount { get; set; }
        public string OfCommercialAmount { get; set; }
        public string OfTotalAmount { get; set; }
        public List<CarInsPKindModel> CarInsKinds { get; set; }
    }
}
