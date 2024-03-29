﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RetObCalloutTakeData
    {
        public string ObCustomerId { get; set; }

        public RetObCalloutTakeData()
        {
            this.Customer = new ObCustomerModel();
            this.Car = new CarInfoModel();
            this.CallResultRecords = new List<CallResultRecordModel>();
        }

        public ObCustomerModel Customer { get; set; }
        public CarInfoModel Car { get; set; }

        public List<CallResultRecordModel> CallResultRecords { get; set; }
    }
}
