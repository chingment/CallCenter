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
                ret.CustomerName = callResultRecord.CustomerName;
                ret.CustomerPhoneNumber = callResultRecord.CustomerPhoneNumber;
                ret.SalesmanName = callResultRecord.SalesmanName;
                ret.ResultName = callResultRecord.ResultName;
                ret.NextCallTime = callResultRecord.NextCallTime.ToUnifiedFormatDateTime();
                ret.Remark = callResultRecord.Remark;


                var callRecords = (from u in CurrentDb.CallRecord.Where(m => m.CustomerId == callResultRecord.CustomerId).OrderByDescending(m => m.CreateTime) select new { u.Id, u.CustomerId, u.CustomerName, u.SalesmanId, u.SalesmanName, u.RecordFile, u.TimeLength, u.CustomerPhoneNumber, u.CreateTime, u.RingTime, u.AnswerTime, u.ByeTime, u.StartTime }).ToList();
                foreach (var item in callRecords)
                {
                    string recordFile = "";

                    if (!string.IsNullOrEmpty(item.RecordFile))
                    {
                        recordFile = "http://39.108.86.40" + item.RecordFile;
                    }

                    ret.CallRecords.Add(new
                    {
                        Id = item.Id,
                        CustomerId = item.CustomerId,
                        CustomerName = item.CustomerName,
                        SalesmanId = item.SalesmanId,
                        SalesmanName = item.SalesmanName,
                        RecordFile = recordFile,
                        TimeLength = item.TimeLength,
                        CustomerPhoneNumber = item.CustomerPhoneNumber,
                        RingTime = item.RingTime.ToUnifiedFormatDateTime(),
                        AnswerTime = item.AnswerTime.ToUnifiedFormatDateTime(),
                        ByeTime = item.ByeTime.ToUnifiedFormatDateTime(),
                        StartTime = item.StartTime.ToUnifiedFormatDateTime(),
                    });

                }

            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }
    }
}
