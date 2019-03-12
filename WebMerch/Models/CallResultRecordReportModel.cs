using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMerch
{
    public class CallResultRecordReportModel: ReportModel
    {
        public string SalesmanName { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string ResultCode { get; set; }
    }
}