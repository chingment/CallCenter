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
        public string CompulsoryAmount { get; set; }
        public string TravelTaxAmount { get; set; }
        public string CommercialAmount { get; set; }
        public string TotalAmount { get; set; }
        public List<CarInsPKindModel> CarInsKinds { get; set; }
    }
}
