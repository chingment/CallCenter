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
            this.Salesman = new SalesmanModel();
           
            this.Customer = new ObCustomerModel();
            this.Car = new CarInfoModel();
            this.CarInsKinds = new List<CarInsPKindModel>();
        }

        public SalesmanModel Salesman { get; set; }
        public ObCustomerModel Customer { get; set; }
        public CarInfoModel Car { get; set; }
        public string CompanyName { get; set; }
        public string OfCompulsoryAmount { get; set; }
        public string OfTravelTaxAmount { get; set; }
        public string OfCommercialAmount { get; set; }
        public string OfTotalAmount { get; set; }
        public List<CarInsPKindModel> CarInsKinds { get; set; }

        public string UnAuditComments { get; set; }

        public string UnCompulsoryAmount { get; set; }
        public string UnTravelTaxAmount { get; set; }
        public string UnCommercialAmount { get; set; }
        public string UnTotalAmount { get; set; }
        public string UnOrderImgUrl { get; set; }
        public string UnPayImgUrl { get; set; }
    }
}
