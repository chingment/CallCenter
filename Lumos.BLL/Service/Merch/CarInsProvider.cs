using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{
    public class CarInsProvider : BaseProvider
    {
        public CustomJsonResult GetDealtUnderwritingOrderDetails(string operater, string merchantId, string underwriterId, string orderId)
        {
            var ret = new RetCarInsGetDealtUnderwritingOrderDetails();

            var order2CarIns = CurrentDb.Order2CarIns.Where(m => m.MerchantId == merchantId && m.Id == orderId).FirstOrDefault();

            if (string.IsNullOrEmpty(order2CarIns.UnderwriterId))
            {
                if (order2CarIns.FollowStatus == Enumeration.OrderFollowStatus.CarInsWtUnderwrie)
                {
                    var underwriter = CurrentDb.SysMerchantUser.Where(m => m.Id == underwriterId).FirstOrDefault();
                    order2CarIns.FollowStatus = Enumeration.OrderFollowStatus.CarInsInUnderwrie;
                    order2CarIns.UnderwriterId = underwriter.Id;
                    order2CarIns.UnderwriterName = underwriter.FullName;
                    order2CarIns.Mender = operater;
                    order2CarIns.MendTime = this.DateTime;
                    CurrentDb.SaveChanges();

                    MerchServiceFactory.Customer.AddDealtTrack(operater, underwriter.FullName + "，已取单核保中", order2CarIns.MerchantId, order2CarIns.CustomerId, order2CarIns.Id, order2CarIns.FollowStatus);
                }
            }
            else
            {
                if (order2CarIns.FollowStatus == Enumeration.OrderFollowStatus.CarInsInUnderwrie)
                {
                    if (order2CarIns.UnderwriterId != underwriterId)
                    {

                    }
                }
            }


            ret.Salesman.Id = order2CarIns.SalesmanId;
            ret.Salesman.Name = order2CarIns.SalesmanName;



            ret.Customer.Name = order2CarIns.CarOwner;
            ret.Customer.PhoneNumber = order2CarIns.CarOwnerPhoneNumber;
            ret.Customer.Address = order2CarIns.CarOwnerAddress;
            ret.Customer.IdNumber = order2CarIns.CarOwnerIdNumber;

            ret.Car.RegisterDate = order2CarIns.CarRegisterDate.ToUnifiedFormatDate();
            ret.Car.PlateNo = order2CarIns.CarPlateNo;
            ret.Car.Model = order2CarIns.CarModel;
            ret.Car.EngineNo = order2CarIns.CarEngineNo;
            ret.Car.Vin = order2CarIns.CarVin;

            ret.OfCommercialAmount = order2CarIns.OfCommercialAmount.ToF2Price();
            ret.OfCompulsoryAmount = order2CarIns.OfCompulsoryAmount.ToF2Price();
            ret.OfTravelTaxAmount = order2CarIns.OfTravelTaxAmount.ToF2Price();
            ret.OfTotalAmount = order2CarIns.OfTotalAmount.ToF2Price();

            ret.UnAuditComments = order2CarIns.UnAuditComments;
            ret.UnCompulsoryAmount = order2CarIns.UnCompulsoryAmount.ToF2Price();
            ret.UnTravelTaxAmount = order2CarIns.UnTravelTaxAmount.ToF2Price();
            ret.UnCommercialAmount = order2CarIns.UnCommercialAmount.ToF2Price();
            ret.UnTotalAmount = order2CarIns.UnTotalAmount.ToF2Price();
            ret.UnOrderImgUrl = order2CarIns.UnOrderImgUrl;
            ret.UnPayImgUrl = order2CarIns.UnPayImgUrl;

            var order2CarInsKinds = CurrentDb.Order2CarInsKind.Where(m => m.OrderId == orderId).ToList();


            var carKinds = CurrentDb.CarInsKind.OrderBy(m => m.Priority).ToList();

            var carPKinds = carKinds.Where(m => m.PId == "0").ToList();

            foreach (var carPKind in carPKinds)
            {
                CarInsPKindModel carPInsKindModel = new CarInsPKindModel();
                carPInsKindModel.Id = carPKind.Id;
                carPInsKindModel.Name = carPKind.Name;

                var carCKinds = carKinds.Where(m => m.PId == carPKind.Id).ToList();

                foreach (var carCKind in carCKinds)
                {

                    var order2CarInsKind = order2CarInsKinds.Where(m => m.KindId == carCKind.Id).FirstOrDefault();

                    var carInsCKindModel = new CarInsCKindModel();
                    carInsCKindModel.Id = carCKind.Id;
                    carInsCKindModel.PId = carCKind.PId;
                    carInsCKindModel.Name = carCKind.Name;
                    carInsCKindModel.Type = carCKind.Type;
                    carInsCKindModel.CanWaiverDeductible = carCKind.CanWaiverDeductible;
                    carInsCKindModel.InputType = carCKind.InputType;
                    carInsCKindModel.InputUnit = carCKind.InputUnit;
                    if (!string.IsNullOrEmpty(carCKind.InputOption))
                    {
                        carInsCKindModel.InputOption = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(carCKind.InputOption);
                    }

                    carInsCKindModel.IsHasDetails = carCKind.IsHasDetails;

                    if (order2CarInsKind == null)
                    {
                        carInsCKindModel.IsSelected = false;
                        carInsCKindModel.InputValue = carCKind.InputValue;
                        carInsCKindModel.IsSelectedWaiverDeductible = carCKind.IsSelectedWaiverDeductible;
                    }
                    else
                    {
                        carInsCKindModel.IsSelected = true;
                        carInsCKindModel.InputValue = order2CarInsKind.KindValue;
                        carInsCKindModel.IsSelectedWaiverDeductible = order2CarInsKind.KindIsWaiverDeductible;
                        carInsCKindModel.Details = order2CarInsKind.KindDetails;
                        carPInsKindModel.Child.Add(carInsCKindModel);
                    }
                }

                ret.CarInsKinds.Add(carPInsKindModel);

            }

            ret.CarInsKinds = ret.CarInsKinds.Where(m => m.Child.Count > 0).ToList();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }


        public CustomJsonResult GetUnderwritingOrderDetails(string operater, string merchantId, string orderId)
        {
            var ret = new RetCarInsGetDealtUnderwritingOrderDetails();

            var order2CarIns = CurrentDb.Order2CarIns.Where(m => m.MerchantId == merchantId && m.Id == orderId).FirstOrDefault();


            ret.Salesman.Id = order2CarIns.SalesmanId;
            ret.Salesman.Name = order2CarIns.SalesmanName;

            ret.Customer.Name = order2CarIns.CarOwner;
            ret.Customer.PhoneNumber = order2CarIns.CarOwnerPhoneNumber;
            ret.Customer.Address = order2CarIns.CarOwnerAddress;
            ret.Customer.IdNumber = order2CarIns.CarOwnerIdNumber;

            ret.Car.RegisterDate = order2CarIns.CarRegisterDate.ToUnifiedFormatDate();
            ret.Car.PlateNo = order2CarIns.CarPlateNo;
            ret.Car.Model = order2CarIns.CarModel;
            ret.Car.EngineNo = order2CarIns.CarEngineNo;
            ret.Car.Vin = order2CarIns.CarVin;

            ret.OfCommercialAmount = order2CarIns.OfCommercialAmount.ToF2Price();
            ret.OfCompulsoryAmount = order2CarIns.OfCompulsoryAmount.ToF2Price();
            ret.OfTravelTaxAmount = order2CarIns.OfTravelTaxAmount.ToF2Price();
            ret.OfTotalAmount = order2CarIns.OfTotalAmount.ToF2Price();

            ret.UnAuditComments = order2CarIns.UnAuditComments;
            ret.UnCompulsoryAmount = order2CarIns.UnCompulsoryAmount.ToF2Price();
            ret.UnTravelTaxAmount = order2CarIns.UnTravelTaxAmount.ToF2Price();
            ret.UnCommercialAmount = order2CarIns.UnCommercialAmount.ToF2Price();
            ret.UnTotalAmount = order2CarIns.UnTotalAmount.ToF2Price();
            ret.UnOrderImgUrl = order2CarIns.UnOrderImgUrl;
            ret.UnPayImgUrl = order2CarIns.UnPayImgUrl;

            var order2CarInsKinds = CurrentDb.Order2CarInsKind.Where(m => m.OrderId == orderId).ToList();


            var carKinds = CurrentDb.CarInsKind.OrderBy(m => m.Priority).ToList();

            var carPKinds = carKinds.Where(m => m.PId == "0").ToList();

            foreach (var carPKind in carPKinds)
            {
                CarInsPKindModel carPInsKindModel = new CarInsPKindModel();
                carPInsKindModel.Id = carPKind.Id;
                carPInsKindModel.Name = carPKind.Name;

                var carCKinds = carKinds.Where(m => m.PId == carPKind.Id).ToList();

                foreach (var carCKind in carCKinds)
                {

                    var order2CarInsKind = order2CarInsKinds.Where(m => m.KindId == carCKind.Id).FirstOrDefault();

                    var carInsCKindModel = new CarInsCKindModel();
                    carInsCKindModel.Id = carCKind.Id;
                    carInsCKindModel.PId = carCKind.PId;
                    carInsCKindModel.Name = carCKind.Name;
                    carInsCKindModel.Type = carCKind.Type;
                    carInsCKindModel.CanWaiverDeductible = carCKind.CanWaiverDeductible;
                    carInsCKindModel.InputType = carCKind.InputType;
                    carInsCKindModel.InputUnit = carCKind.InputUnit;
                    if (!string.IsNullOrEmpty(carCKind.InputOption))
                    {
                        carInsCKindModel.InputOption = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(carCKind.InputOption);
                    }

                    carInsCKindModel.IsHasDetails = carCKind.IsHasDetails;

                    if (order2CarInsKind == null)
                    {
                        carInsCKindModel.IsSelected = false;
                        carInsCKindModel.InputValue = carCKind.InputValue;
                        carInsCKindModel.IsSelectedWaiverDeductible = carCKind.IsSelectedWaiverDeductible;
                    }
                    else
                    {
                        carInsCKindModel.IsSelected = true;
                        carInsCKindModel.InputValue = order2CarInsKind.KindValue;
                        carInsCKindModel.IsSelectedWaiverDeductible = order2CarInsKind.KindIsWaiverDeductible;
                        carInsCKindModel.Details = order2CarInsKind.KindDetails;
                        carPInsKindModel.Child.Add(carInsCKindModel);
                    }
                }

                ret.CarInsKinds.Add(carPInsKindModel);

            }

            ret.CarInsKinds = ret.CarInsKinds.Where(m => m.Child.Count > 0).ToList();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult DealtUnderwritingOrder(string operater, string merchantId, string underwriterId, RopCarInsDealtUnderwritingOrder rop)
        {
            var ret = new RetCarInsGetDealtUnderwritingOrderDetails();

            var order2CarIns = CurrentDb.Order2CarIns.Where(m => m.MerchantId == merchantId && m.Id == rop.OrderId).FirstOrDefault();


            if (order2CarIns.UnderwriterId != underwriterId)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "您没有权限");
            }


            switch (rop.Operate)
            {
                case Enumeration.OperateType.Save:
                    order2CarIns.UnCompulsoryAmount = rop.UnCompulsoryAmount;
                    order2CarIns.UnTravelTaxAmount = rop.UnTravelTaxAmount;
                    order2CarIns.UnCommercialAmount = rop.UnCommercialAmount;
                    order2CarIns.UnTotalAmount = rop.UnCompulsoryAmount + rop.UnTravelTaxAmount + rop.UnCommercialAmount;
                    order2CarIns.UnOrderImgUrl = rop.UnOrderImgUrl;
                    order2CarIns.UnPayImgUrl = rop.UnPayImgUrl;
                    order2CarIns.UnAuditComments = rop.UnAuditComments;
                    break;
                case Enumeration.OperateType.Submit:
                    order2CarIns.UnCompulsoryAmount = rop.UnCompulsoryAmount;
                    order2CarIns.UnTravelTaxAmount = rop.UnTravelTaxAmount;
                    order2CarIns.UnCommercialAmount = rop.UnCommercialAmount;
                    order2CarIns.UnTotalAmount = rop.UnCompulsoryAmount + rop.UnTravelTaxAmount + rop.UnCommercialAmount;
                    order2CarIns.UnOrderImgUrl = rop.UnOrderImgUrl;
                    order2CarIns.UnPayImgUrl = rop.UnPayImgUrl;
                    order2CarIns.UnAuditComments = rop.UnAuditComments;
                    order2CarIns.Status = Enumeration.OrderStatus.WaitPay;
                    order2CarIns.FollowStatus = Enumeration.OrderFollowStatus.CarInsAlUnderwrie;
                    MerchServiceFactory.Customer.AddDealtTrack(operater, order2CarIns.UnderwriterName + ",核保完成", order2CarIns.MerchantId, order2CarIns.CustomerId, order2CarIns.Id, order2CarIns.FollowStatus);
                    break;
                default:
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未知道操作");

            }

            order2CarIns.Mender = operater;
            order2CarIns.MendTime = this.DateTime;
            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "提交成功", ret);
        }

        public CustomJsonResult GetKind(string operater, string merchantId)
        {
            CustomJsonResult result = new CustomJsonResult();
            List<CarInsPKindModel> carInsPKindModels = new List<CarInsPKindModel>();

            var carKinds = CurrentDb.CarInsKind.OrderBy(m => m.Priority).ToList();

            var carPKinds = carKinds.Where(m => m.PId == "0").ToList();

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
                    carInsCKindModel.IsSelectedWaiverDeductible = carCKind.IsSelectedWaiverDeductible;
                    carInsCKindModel.InputType = carCKind.InputType;
                    carInsCKindModel.InputUnit = carCKind.InputUnit;
                    carInsCKindModel.InputValue = carCKind.InputValue;
                    if (!string.IsNullOrEmpty(carCKind.InputOption))
                    {
                        carInsCKindModel.InputOption = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(carCKind.InputOption);
                    }

                    carInsCKindModel.IsHasDetails = carCKind.IsHasDetails;
                    carInsCKindModel.IsSelected = carCKind.IsSelected;

                    carPInsKindModel.Child.Add(carInsCKindModel);
                }

                carInsPKindModels.Add(carPInsKindModel);

            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功", new { carInsKinds = carInsPKindModels });
        }


        public CustomJsonResult SubmitUnderwriting(string operater, string merchantId, string salesmanId, RopObCalloutCarInsSubmitUnderwriting rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var callRecordCount = CurrentDb.CallRecord.Where(m => m.MerchantId == merchantId && m.SalesmanId == salesmanId && m.CustomerId == rop.CustomerId).Count();
                if (callRecordCount == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "系统检测到没有外呼客户，不能提交核保");
                }

                var salesman = CurrentDb.SysMerchantUser.Where(m => m.Id == salesmanId).FirstOrDefault();

                var obCustomer = CurrentDb.ObCustomer.Where(m => m.Id == rop.CustomerId).FirstOrDefault();

                var order2CarIns = CurrentDb.Order2CarIns.Where(m => m.CustomerId == rop.CustomerId && (m.FollowStatus == Enumeration.OrderFollowStatus.CarInsWtUnderwrie || m.FollowStatus == Enumeration.OrderFollowStatus.CarInsInUnderwrie)).FirstOrDefault();

                if (order2CarIns != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该客户已在核保中，不能重复提交，请耐心等候");
                }

                order2CarIns = new Order2CarIns();

                order2CarIns.Id = GuidUtil.New();
                order2CarIns.Sn = SnUtil.Build(Enumeration.BizSnType.Order2CarIns, merchantId);
                order2CarIns.Type = Enumeration.OrderType.CarIns;
                order2CarIns.MerchantId = merchantId;
                order2CarIns.CustomerId = rop.CustomerId;
                order2CarIns.CompanyId = "";
                order2CarIns.CompanyName = "";
                order2CarIns.OfCompulsoryAmount = rop.OfCompulsoryAmount;
                order2CarIns.OfTravelTaxAmount = rop.OfTravelTaxAmount;
                order2CarIns.OfCommercialAmount = rop.OfCommercialAmount;
                order2CarIns.OfTotalAmount = rop.OfCompulsoryAmount + rop.OfTravelTaxAmount + rop.OfCommercialAmount;
                order2CarIns.CarOwner = obCustomer.CsrName;
                order2CarIns.CarOwnerPhoneNumber = obCustomer.CsrPhoneNumber;
                order2CarIns.CarOwnerAddress = obCustomer.CsrAddress;
                order2CarIns.CarOwnerIdNumber = obCustomer.CsrIdNumber;
                order2CarIns.CarRegisterDate = obCustomer.CarRegisterDate;
                order2CarIns.CarPlateNo = obCustomer.CarPlateNo;
                order2CarIns.CarModel = obCustomer.CarModel;
                order2CarIns.CarEngineNo = obCustomer.CarEngineNo;
                order2CarIns.CarVin = obCustomer.CarVin;
                order2CarIns.PayWay = Enumeration.OrderPayWay.Unknow;
                order2CarIns.Status = Enumeration.OrderStatus.Submitted;
                order2CarIns.FollowStatus = Enumeration.OrderFollowStatus.CarInsWtUnderwrie;
                order2CarIns.SubmitTime = this.DateTime;
                order2CarIns.SalesmanId = salesman.Id;
                order2CarIns.SalesmanName = salesman.FullName;
                order2CarIns.Creator = operater;
                order2CarIns.CreateTime = this.DateTime;
                CurrentDb.Order2CarIns.Add(order2CarIns);

                foreach (var kind in rop.Kinds)
                {
                    var order2CarInsKind = new Order2CarInsKind();
                    order2CarInsKind.Id = GuidUtil.New();
                    order2CarInsKind.OrderId = order2CarIns.Id;
                    order2CarInsKind.KindId = kind.Id;
                    order2CarInsKind.KindValue = kind.Value;
                    order2CarInsKind.KindIsWaiverDeductible = kind.IsWaiverDeductible;
                    order2CarInsKind.CreateTime = this.DateTime;
                    order2CarInsKind.Creator = operater;
                    CurrentDb.Order2CarInsKind.Add(order2CarInsKind);
                }

                MerchServiceFactory.Customer.AddDealtTrack(operater, "提交报价单，等待取单核保", order2CarIns.MerchantId, order2CarIns.CustomerId, order2CarIns.Id, order2CarIns.FollowStatus);

                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "提交成功");
            }

            return result;
        }


        public CustomJsonResult GetUnderwritingOrder(string operater, string merchantId, string userId)
        {
            var accessUserIds = MerchServiceFactory.User.GetCanAccessUserIds(operater, merchantId, userId);

            var order2CarInss = CurrentDb.Order2CarIns.Where(m => m.MerchantId == merchantId &&
                           accessUserIds.Contains(m.SalesmanId)).ToList();


            CustomJsonResult result = new CustomJsonResult();


            var ret = new RetObCalloutCarInsGetUnderwritingOrder();


            foreach (var order2CarIns in order2CarInss)
            {

                string customer = string.Format("{0}[{1}]", order2CarIns.CarOwner, order2CarIns.CarPlateNo);
                string tips = "";
                switch (order2CarIns.FollowStatus)
                {
                    case Enumeration.OrderFollowStatus.CarInsWtUnderwrie:
                        tips = "等待后勤人员取单核保";
                        break;
                    case Enumeration.OrderFollowStatus.CarInsInUnderwrie:
                        tips = "后勤人员核保中";
                        break;
                    case Enumeration.OrderFollowStatus.CarInsAlUnderwrie:
                        tips = "核保已经完成";
                        break;
                }
                ret.UnderwritingOrders.Add(new RetObCalloutCarInsGetUnderwritingOrder.UnderwritingOrder { Customer = customer, Tips = tips });
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }


    }
}
