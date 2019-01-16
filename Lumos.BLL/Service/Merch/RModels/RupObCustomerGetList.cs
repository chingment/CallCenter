using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RupObCustomerGetList : RupObBatchGetList
    {
        public string ObBatchAllocateId { get; set; }
        public string CsrName { get; set; }
        public string CsrPhoneNumber { get; set; }
        public string CsrAddress { get; set; }
        public string CsrIdNumber { get; set; }
        public string CsrCompany { get; set; }
        public string CarInsLastCompany { get; set; }
        public string CarPlateNo { get; set; }
        public string CarModel { get; set; }
        public string CarEngineNo { get; set; }
        public DateTime? CarRegisterDateStart { get; set; }
        public DateTime? CarRegisterDateEnd { get; set; }
        public DateTime? CarInsLastStartTime { get; set; }
        public DateTime? CarInsLastEndTime { get; set; }

    }
}
