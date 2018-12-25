using Lumos.Entity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{
    public class TelephoneControlProvder : BaseProvider
    {
        public CustomJsonResult Login(string operater, string merchantId, string userId)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var sysMerchantUser = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.Id == userId).FirstOrDefault();


                SdkFactory.Lxt.Login(sysMerchantUser.TelSeatAccount);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

                //CurrentDb.SaveChanges();
                //ts.Complete();
            }

            return result;
        }

        public CustomJsonResult CallCustomer(string operater, string merchantId, string salesmanId, string customerId)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var sysMerchantUser = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.Id == salesmanId).FirstOrDefault();
                if (sysMerchantUser == null)
                {
                    new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "用户信息无效");
                }

                if (string.IsNullOrEmpty(sysMerchantUser.TelSeatAccount))
                {
                    new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置外呼电话信息，请联系系统管理员");
                }

                var obCustomer = CurrentDb.ObCustomer.Where(m => m.Id == customerId).FirstOrDefault();
                if (obCustomer == null)
                {
                    new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统找不到该客户信息");
                }

                string telSeatAccount = sysMerchantUser.TelSeatAccount;

                var telephoneStatus = SdkFactory.Lxt.GetStatus(telSeatAccount);

                if (telephoneStatus != TelephoneStatus.IDLE)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该话机状态为" + telephoneStatus.GetCnName());
                }

                var callRecord = new CallRecord();
                callRecord.Id = GuidUtil.New();
                callRecord.Sn = SnUtil.Build(Enumeration.BizSnType.TelphoneControlSeq, salesmanId);
                callRecord.MerchantId = merchantId;
                callRecord.CustomerId = obCustomer.Id;
                callRecord.SalesmanId = salesmanId;
                callRecord.Remark = "";
                callRecord.Creator = operater;
                callRecord.CreateTime = this.DateTime;
                CurrentDb.CallRecord.Add(callRecord);

                SdkFactory.Lxt.CallNumber(telSeatAccount, callRecord.Sn, obCustomer.CsrPhoneNumber);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;
        }

        public CustomJsonResult HangupCustomer(string operater, string merchantId, string salesmanId, string customerId)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var sysMerchantUser = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.Id == salesmanId).FirstOrDefault();
                if (sysMerchantUser == null)
                {
                    new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "用户信息无效");
                }

                if (string.IsNullOrEmpty(sysMerchantUser.TelSeatAccount))
                {
                    new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置外呼电话信息，请联系系统管理员");
                }

                var obCustomer = CurrentDb.ObCustomer.Where(m => m.Id == customerId).FirstOrDefault();
                if (obCustomer == null)
                {
                    new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统找不到该客户信息");
                }

                string telSeatAccount = sysMerchantUser.TelSeatAccount;

                SdkFactory.Lxt.Hangup(telSeatAccount);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;
        }

        public void Notify(string operater, string content)
        {
            var notifyType = GetNotifyType(content);
            LogUtil.Info("通知的类型为:" + notifyType);
            switch (notifyType)
            {
                case "callend":
                    break;
                case "callstart":
                    break;
                case "billing":
                    break;
            }
        }

        public string GetNotifyType(string content)
        {
            string type = "";

            try
            {
                var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(content);
                if (jObject != null)
                {
                    if (jObject["notify"] != null)
                    {
                        var s2 = jObject["type"];
                        if (s2 != null)
                        {
                            return s2.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return type;
        }
    }
}
