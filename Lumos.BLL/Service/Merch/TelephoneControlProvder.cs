using Lumos.Entity;
using Lumos.Redis;
using LxtSdk;
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
        public CustomJsonResult Login(string operater, string merchantId, string salesmanId, string account)
        {
            CustomJsonResult result = new CustomJsonResult();

            if (string.IsNullOrEmpty(account))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置外呼话机号码，请联系系统管理员");
            }

            var merchant = CurrentDb.Merchant.Where(m => m.Id == merchantId).FirstOrDefault();

            SdkFactory.Lxt.Login(merchant.LxtApiCustomer, merchant.LxtApiPassword, account);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "签入成功");

            return result;
        }

        public CustomJsonResult Logout(string operater, string merchantId, string salesmanId, string account)
        {
            CustomJsonResult result = new CustomJsonResult();

            if (string.IsNullOrEmpty(account))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置外呼话机号码，请联系系统管理员");
            }

            var merchant = CurrentDb.Merchant.Where(m => m.Id == merchantId).FirstOrDefault();

            SdkFactory.Lxt.Logout(merchant.LxtApiCustomer, merchant.LxtApiPassword, account);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "签出成功");

            return result;
        }

        public CustomJsonResult CallCustomer(string operater, string merchantId, string salesmanId, string customerId, string account)
        {
            CustomJsonResult result = new CustomJsonResult();

            if (string.IsNullOrEmpty(account))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置外呼话机号码，请退出，在登陆界面点击设置，进行设置");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var salesman = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.Id == salesmanId).FirstOrDefault();
                if (salesman == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "用户信息无效");
                }

                var obCustomer = CurrentDb.ObCustomer.Where(m => m.Id == customerId).FirstOrDefault();
                if (obCustomer == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统找不到该客户信息");
                }

                var merchant = CurrentDb.Merchant.Where(m => m.Id == merchantId).FirstOrDefault();

                var telePhoneStatus = SdkFactory.Lxt.GetStatus(merchant.LxtApiCustomer, merchant.LxtApiPassword, account);
                if (telePhoneStatus == Enumeration.TelePhoneStatus.Unknow)
                {
                    SdkFactory.Lxt.Login(merchant.LxtApiCustomer, merchant.LxtApiPassword, account);

                    telePhoneStatus = SdkFactory.Lxt.GetStatus(merchant.LxtApiCustomer, merchant.LxtApiPassword, account);
                }

                if (telePhoneStatus != Enumeration.TelePhoneStatus.IDLE)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "话机正在忙，请稍后再试");
                }

                var callRecord = new CallRecord();
                callRecord.Id = GuidUtil.New();
                callRecord.Sn = SnUtil.Build(Enumeration.BizSnType.TelphoneControlSeq, salesmanId);
                callRecord.MerchantId = merchantId;
                callRecord.CustomerId = obCustomer.Id;
                callRecord.CustomerName = obCustomer.CsrName;
                callRecord.SalesmanId = salesmanId;
                callRecord.SalesmanName = salesman.FullName;
                callRecord.TeleSeatAccount = account;
                callRecord.CustomerPhoneNumber = obCustomer.CsrPhoneNumber;
                callRecord.Remark = "";
                callRecord.Creator = operater;
                callRecord.CreateTime = this.DateTime;
                CurrentDb.CallRecord.Add(callRecord);

                SdkFactory.Lxt.CallNumber(merchant.LxtApiCustomer, merchant.LxtApiPassword, account, callRecord.Sn, obCustomer.CsrPhoneNumber);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");

                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;
        }

        public CustomJsonResult HangupCustomer(string operater, string merchantId, string salesmanId, string customerId, string account)
        {
            CustomJsonResult result = new CustomJsonResult();

            if (string.IsNullOrEmpty(account))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未配置外呼话机号码，请联系系统管理员");
            }

            var merchant = CurrentDb.Merchant.Where(m => m.Id == merchantId).FirstOrDefault();

            SdkFactory.Lxt.Hangup(merchant.LxtApiCustomer, merchant.LxtApiPassword, account);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "挂机成功");

            return result;
        }

        private DateTime? GetTime(string timeStamp)
        {
            if (timeStamp == "")
                return null;
            if (timeStamp == "0")
                return null;

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);

            return dtStart.Add(toNow);
        }

        public void Notify(string operater, string content)
        {
            string sn = "";
            var notifyType = GetNotifyType(content, out sn);
            LogUtil.Info("通知的类型为:" + notifyType);
            var callRecord = CurrentDb.CallRecord.Where(m => m.Sn == sn).FirstOrDefault();
            if (callRecord != null)
            {
                switch (notifyType)
                {
                    case "callstart":
                        var callstart = content.ToJsonObject<NotifyResultByCallStart>();
                        if (callstart != null)
                        {

                        }
                        break;
                    case "callend":
                        var callend = content.ToJsonObject<NotifyResultByCallEnd>();
                        if (callend != null)
                        {

                        }
                        break;
                    case "billing":
                        var billing = content.ToJsonObject<NotifyResultByBilling>();
                        if (billing != null)
                        {
                            callRecord.StartTime = GetTime(billing.Notify.StartTime);
                            callRecord.ByeTime = GetTime(billing.Notify.ByeTime);
                            callRecord.AnswerTime = GetTime(billing.Notify.AnswerTime);
                            callRecord.RingTime = GetTime(billing.Notify.RingTime);
                            callRecord.CustomerPhoneNumber = billing.Notify.Callee;
                            callRecord.RecordFile = billing.Notify.RecordFile;
                            callRecord.Service = billing.Notify.Service;
                            callRecord.TimeLength = billing.Notify.TimeLength;
                            CurrentDb.SaveChanges();
                        }
                        break;
                }

            }
        }

        public string GetNotifyType(string content, out string sn)
        {
            string type = "";


            try
            {
                var jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>(content);
                if (jObject != null)
                {
                    if (jObject["notify"] != null)
                    {
                        var s1 = jObject["notify"];
                        var s2 = s1["type"];
                        if (s2 != null)
                        {
                            sn = s1["userData"].ToString();
                            return s2.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            sn = null;
            return type;
        }

    }
}
