using Lumos.Entity;
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

        public CustomJsonResult TakeData(string operater, string merchantId, string takerId)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var ret = new RetObCalloutTakeData();

                var obCustomer = CurrentDb.ObCustomer.Where(m => m.MerchantId == merchantId && m.IsUseCall == false && m.TakerId == takerId).FirstOrDefault();
                if (obCustomer == null)
                {
                    obCustomer = CurrentDb.ObCustomer.Where(m => m.MerchantId == merchantId && m.IsTake == false && m.BelongUserId == takerId).OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                    if (obCustomer == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "没有可外呼的数据");
                    }

                    obCustomer.TakerId = takerId;
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

        public CustomJsonResult SaveCallRecored(string operater, string merchantId, RopObCalloutSaveCallRecored rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var obCustomer = CurrentDb.ObCustomer.Where(m => m.MerchantId == merchantId && m.IsTake == true && m.TakerId == rop.TakerId && m.Id == rop.CustomerId).FirstOrDefault();
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
                callRecord.CustomerId = rop.CustomerId;
                callRecord.ResultCode = rop.ResultCode;
                callRecord.NextCallTime = rop.NextCallTime;
                callRecord.Remark = rop.Remark;
                callRecord.Creator = operater;
                callRecord.CreateTime = this.DateTime;
                CurrentDb.CallRecord.Add(callRecord);
                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }
            return result;
        }


        public CustomJsonResult CarInsGetKind(string operater, string merchantId)
        {
            CustomJsonResult result = new CustomJsonResult();
            List<CarInsPKindModel> carInsPKindModels = new List<CarInsPKindModel>();

            var carKinds = CurrentDb.CarInsKind.OrderByDescending(m => m.Priority).ToList();

            var carPKinds = carKinds.Where(m => m.PId == 0).ToList();

            foreach (var carPKind in carPKinds)
            {
                CarInsPKindModel carPInsKindModel = new CarInsPKindModel();
                carPInsKindModel.Id = carPKind.Id;
                carPInsKindModel.Name = carPKind.Name;

                var carCKinds = carKinds.Where(m => m.PId == carPKind.Id).ToList();

                foreach (var carCKind in carCKinds)
                {
                    var carInsCKindModel = new CarInsCKindModel();
                    carInsCKindModel.Id = carCKind.Id;
                    carInsCKindModel.PId = carCKind.PId;
                    carInsCKindModel.Name = carCKind.Name;
                    carInsCKindModel.Type = carCKind.Type;
                    carInsCKindModel.CanWaiverDeductible = carCKind.CanWaiverDeductible;
                    carInsCKindModel.IsWaiverDeductible = carCKind.IsWaiverDeductible;
                    carInsCKindModel.InputType = carCKind.InputType;
                    carInsCKindModel.InputUnit = carCKind.InputUnit;
                    carInsCKindModel.InputValue = carCKind.InputValue;
                    if (!string.IsNullOrEmpty(carCKind.InputOption))
                    {
                        carInsCKindModel.InputOption = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(carCKind.InputOption);
                    }

                    carInsCKindModel.IsHasDetails = carCKind.IsHasDetails;
                    carInsCKindModel.IsCheck = carCKind.IsCheck;

                    carPInsKindModel.Child.Add(carInsCKindModel);
                }

                carInsPKindModels.Add(carPInsKindModel);

            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功", new { carInsKinds = carInsPKindModels });
        }


        public CustomJsonResult CarInsSubmitInsure(string operater, string merchantId, RopObCalloutCarInsSubmitInsure rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            return result;
        }
    }
}
