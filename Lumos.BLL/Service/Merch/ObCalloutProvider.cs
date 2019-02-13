﻿using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{
    public class ObCalloutProvider : BaseProvider
    {

        private List<ObBatchAllocate> GetFatherObBatchAllocates(string merchantId, string id)
        {
            var obBatchAllocates = CurrentDb.ObBatchAllocate.Where(m => m.MerchantId == merchantId).ToList();

            var list = new List<ObBatchAllocate>();
            var list2 = list.Concat(GetFatherObBatchAllocateList(obBatchAllocates, id));
            return list2.ToList();
        }


        public IEnumerable<ObBatchAllocate> GetFatherObBatchAllocateList(IList<ObBatchAllocate> list, string pId)
        {
            var query = list.Where(p => p.Id == pId).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetFatherObBatchAllocateList(list, t.PId)));
        }

        public CustomJsonResult TakeData(string operater, string merchantId, string salesmanId)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var ret = new RetObCalloutTakeData();

                //var s = CurrentDb.ObBatchAllocate.Where(m => m.BelongerId == "1a1f58e9595544b3be41da174b34513f").FirstOrDefault();

                //System.Threading.Thread.Sleep(1000000);

                DateTime dateEnd1 = DateTime.Parse(DateTime.Now.ToUnifiedFormatDate() + " 23:40:00");
                DateTime dateEnd2 = DateTime.Parse(DateTime.Now.ToUnifiedFormatDate() + " 23:59:59");

                if (DateTime.Now >= dateEnd1 && DateTime.Now <= dateEnd2)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统在该时段维护中");
                }

                var taker = CurrentDb.SysMerchantUser.Where(m => m.Id == salesmanId).FirstOrDefault();

                if (taker.Status != Enumeration.UserStatus.Normal)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该账号已被禁用");
                }

                var obCustomer = CurrentDb.ObCustomer.Where(m => m.MerchantId == merchantId && m.IsUseCall == false && m.SalesmanId == salesmanId).FirstOrDefault();
                if (obCustomer == null)
                {

                    var obTakeDataLimit = CurrentDb.ObTakeDataLimit.Where(m => m.MerchantId == merchantId && m.SalesmanId == salesmanId).FirstOrDefault();
                    if (obTakeDataLimit == null || obTakeDataLimit.UnTakeQuantity <= 0)
                    {

                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "您当前的外号任务量已经用完");
                    }


                    obCustomer = CurrentDb.ObCustomer.Where(m => m.MerchantId == merchantId && m.IsTake == false && m.BelongerId == salesmanId).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                    if (obCustomer == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "没有可外呼的数据");
                    }

                    obCustomer.SalesmanId = salesmanId;
                    obCustomer.IsTake = true;
                    obCustomer.TakeTime = this.DateTime;
                    obCustomer.IsUseCall = false;
                    obCustomer.Mender = operater;
                    obCustomer.MendTime = this.DateTime;

                    var obBatchAllocates = GetFatherObBatchAllocates(merchantId, obCustomer.ObBatchAllocateId);

                    foreach (var obBatchAllocate in obBatchAllocates)
                    {
                        if (obBatchAllocate.Id == obCustomer.ObBatchAllocateId)
                        {
                            obBatchAllocate.AllocatedCount += 1;
                            obBatchAllocate.UnAllocatedCount -= 1;
                            obBatchAllocate.UsedCount += 1;
                        }
                        else
                        {
                            obBatchAllocate.UsedCount += 1;
                        }

                        //obBatchAllocate.UnUsedCount = obBatchAllocate.AllocatedCount - obBatchAllocate.UsedCount;
                        obBatchAllocate.Mender = operater;
                        obBatchAllocate.MendTime = this.DateTime;
                    }

                    obTakeDataLimit.TakedQuantity += 1;
                    obTakeDataLimit.UnTakeQuantity -= 1;


                    CurrentDb.SaveChanges();
                    ts.Complete();
                }

                ret.ObCustomerId = obCustomer.Id;

                ret.Customer.Name = obCustomer.CsrName;
                ret.Customer.PhoneNumber = obCustomer.CsrPhoneNumber;
                ret.Customer.IdNumber = obCustomer.CsrIdNumber;
                ret.Customer.Address = obCustomer.CsrAddress;
                ret.Customer.Company = obCustomer.CsrCompany;

                ret.Car.RegisterDate = obCustomer.CarRegisterDate.ToUnifiedFormatDate();
                ret.Car.PlateNo = obCustomer.CarPlateNo;
                ret.Car.Model = obCustomer.CarModel;
                ret.Car.EngineNo = obCustomer.CarEngineNo;
                ret.Car.Vin = obCustomer.CarVin;
                ret.Car.InsLastQzNo = obCustomer.CarInsLastQzNo;
                ret.Car.InsLastSyNo = obCustomer.CarInsLastSyNo;
                ret.Car.InsLastCompany = obCustomer.CarInsLastCompany;
                ret.Car.InsTime = obCustomer.CarInsLastStartTime.ToUnifiedFormatDate() + " 至 " + obCustomer.CarInsLastEndTime.ToUnifiedFormatDate();



                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "取出成功", ret);
            }

            return result;

        }

        public CustomJsonResult SaveCallResultRecord(string operater, string merchantId, string salesmanId, RopObCalloutSaveCallResultRecord rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var obCustomer = CurrentDb.ObCustomer.Where(m => m.MerchantId == merchantId && m.IsTake == true && m.SalesmanId == salesmanId && m.Id == rop.CustomerId).FirstOrDefault();
                if (obCustomer == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "客户资料不存在");
                }

                if (obCustomer.IsUseCall)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "已经保存通话记录");
                }


                var callRecordCount = CurrentDb.CallRecord.Where(m => m.MerchantId == merchantId && m.SalesmanId == salesmanId && m.CustomerId == rop.CustomerId).Count();
                if (callRecordCount == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统检测到没有外呼客户，不能保存通话记录");
                }

                obCustomer.IsUseCall = true;
                obCustomer.UseCallTime = this.DateTime;


                var salesman = CurrentDb.SysMerchantUser.Where(m => m.Id == salesmanId).FirstOrDefault();

                var callResultCode = CurrentDb.CallResultCode.Where(m => m.Code == rop.ResultCode).FirstOrDefault();

                var callResultRecord = new CallResultRecord();
                callResultRecord.Id = GuidUtil.New();
                callResultRecord.MerchantId = merchantId;
                callResultRecord.SalesmanId = salesmanId;
                callResultRecord.CustomerId = rop.CustomerId;
                callResultRecord.ResultCode = rop.ResultCode;
                callResultRecord.ResultName = callResultCode.Name;
                callResultRecord.NextCallTime = rop.NextCallTime;
                callResultRecord.CustomerId = obCustomer.Id;
                callResultRecord.CustomerName = obCustomer.CsrName;
                callResultRecord.CustomerPhoneNumber = obCustomer.CsrPhoneNumber;
                callResultRecord.SalesmanId = salesman.Id;
                callResultRecord.SalesmanName = salesman.FullName;
                callResultRecord.Remark = rop.Remark;
                callResultRecord.Creator = operater;
                callResultRecord.CreateTime = this.DateTime;
                CurrentDb.CallResultRecord.Add(callResultRecord);


                var resultCodePrefix = rop.ResultCode.Substring(0, 1);

                var obTakeDataLimit = CurrentDb.ObTakeDataLimit.Where(m => m.MerchantId == merchantId && m.SalesmanId == salesmanId).FirstOrDefault();

                switch (resultCodePrefix)
                {
                    case "1":
                        //obTakeDataLimit.UnContactQuantity += 1;
                        break;
                    case "2":
                        //obTakeDataLimit.TargetQuantity += 1;
                        break;
                    case "3":
                        //obTakeDataLimit.InValidQuantity += 1;
                        break;
                    default:
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存成功失败,未知通话结果");
                }


                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }
            return result;
        }

        public CustomJsonResult GetDealtTrack(string operater, string merchantId, string customerId)
        {
            CustomJsonResult result = new CustomJsonResult();

            var customerDealtTracks = CurrentDb.CustomerDealtTrack.Where(m => m.CustomerId == customerId).OrderByDescending(m => m.DealtTime).ToList();

            var tracks = new List<CustomerDealtTrackModel>();

            foreach (var item in customerDealtTracks)
            {
                string url = "";
                if (item.OrderFollowStatus == Enumeration.OrderFollowStatus.CarInsAlUnderwrie)
                {
                    url = "/ObCallout/CarInsUnderwritingOrderDetails?orderId=" + item.OrderId;
                }

                tracks.Add(new CustomerDealtTrackModel { Message = item.Remarks, DealtTime = item.DealtTime.ToUnifiedFormatDateTime(), Url = url });

            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功", tracks);

            return result;
        }

    }
}
