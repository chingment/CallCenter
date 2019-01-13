using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class CallResultRecordProvider : BaseProvider
    {
        public CustomJsonResult GetDetails(string operater, string merchantId, string id)
        {
            var ret = new RetCallResultRecordGetDetails();

            var callResultRecord = CurrentDb.CallResultRecord.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();

            if (callResultRecord != null)
            {
                ret.CustomerId = callResultRecord.CustomerId;
                ret.CustomerName = callResultRecord.CustomerName;
                ret.CustomerPhoneNumber = callResultRecord.CustomerPhoneNumber;
                ret.SalesmanName = callResultRecord.SalesmanName;
                ret.ResultName = callResultRecord.ResultName;
                ret.NextCallTime = callResultRecord.NextCallTime.ToUnifiedFormatDateTime();
                ret.Remark = callResultRecord.Remark;
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }
    }
}
