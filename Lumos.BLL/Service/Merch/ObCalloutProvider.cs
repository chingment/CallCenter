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


                var taker = CurrentDb.SysMerchantUser.Where(m => m.Id == salesmanId).FirstOrDefault();

                if (taker.Status != Enumeration.UserStatus.Normal)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该账号已被禁用");
                }

                var obCustomer = CurrentDb.ObCustomer.Where(m => m.MerchantId == merchantId && m.IsUseCall == false && m.SalesmanId == salesmanId).FirstOrDefault();
                if (obCustomer == null)
                {

                    var calloutTakeDataLimit = CurrentDb.CalloutTakeDataLimit.Where(m => m.MerchantId == merchantId && m.SalesmanId == salesmanId).FirstOrDefault();
                    if (calloutTakeDataLimit == null || calloutTakeDataLimit.UnTakeQuantity <= 0)
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

                        obBatchAllocate.UnUsedCount = obBatchAllocate.AllocatedCount - obBatchAllocate.UsedCount;
                        obBatchAllocate.Mender = operater;
                        obBatchAllocate.MendTime = this.DateTime;
                    }

                    calloutTakeDataLimit.TakedQuantity += 1;
                    calloutTakeDataLimit.UnTakeQuantity -= 1;


                    CurrentDb.SaveChanges();
                    ts.Complete();
                }

                ret.ObCustomerId = obCustomer.Id;

                ret.Customer.Name = obCustomer.CsrName;
                ret.Customer.PhoneNumber = obCustomer.CsrPhoneNumber;
                ret.Customer.IdNumber = obCustomer.CsrIdNumber;
                ret.Customer.Address = obCustomer.CsrAddress;


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

        public CustomJsonResult SaveCallRecored(string operater, string merchantId, string salesmanId, RopObCalloutSaveCallRecored rop)
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

                obCustomer.IsUseCall = true;
                obCustomer.UseCallTime = this.DateTime;

                var callRecord = new CallRecord();
                callRecord.Id = GuidUtil.New();
                callRecord.MerchantId = merchantId;
                callRecord.SalesmanId = salesmanId;
                callRecord.CustomerId = rop.CustomerId;
                callRecord.ResultCode = rop.ResultCode;
                callRecord.NextCallTime = rop.NextCallTime;
                callRecord.Remark = rop.Remark;
                callRecord.Creator = operater;
                callRecord.CreateTime = this.DateTime;
                CurrentDb.CallRecord.Add(callRecord);


                var resultCodePrefix = rop.ResultCode.Substring(0, 1);

                var calloutTakeDataLimit = CurrentDb.CalloutTakeDataLimit.Where(m => m.MerchantId == merchantId && m.SalesmanId == salesmanId).FirstOrDefault();

                switch (resultCodePrefix)
                {
                    case "1":
                        calloutTakeDataLimit.UnContactQuantity += 1;
                        break;
                    case "2":
                        calloutTakeDataLimit.TargetQuantity += 1;
                        break;
                    case "3":
                        calloutTakeDataLimit.InValidQuantity += 1;
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
