﻿using Lumos.Entity;
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
        public CustomJsonResult GetDealtUnderwritingOrderDetails(string operater, string merchantId, string orderId)
        {
            var ret = new RetCarInsGetDealtUnderwritingOrderDetails();

            var order2CarIns = CurrentDb.Order2CarIns.Where(m => m.MerchantId == merchantId && m.Id == orderId).FirstOrDefault();


            ret.Customer.Name = order2CarIns.CarOwner;
            ret.Customer.PhoneNumber = order2CarIns.CarOwnerPhoneNumber;
            ret.Customer.Address = order2CarIns.CarOwnerAddress;
            ret.Customer.IdNumber = order2CarIns.CarOwnerIdNumber;

            ret.Car.RegisterDate = order2CarIns.CarRegisterDate.ToUnifiedFormatDate();
            ret.Car.PlateNo = order2CarIns.CarPlateNo;
            ret.Car.Model = order2CarIns.CarModel;
            ret.Car.EngineNo = order2CarIns.CarEngineNo;
            ret.Car.Vin = order2CarIns.CarVin;

            ret.CommercialAmount = order2CarIns.CommercialAmount.ToF2Price();
            ret.CompulsoryAmount = order2CarIns.CompulsoryAmount.ToF2Price();
            ret.TravelTaxAmount = order2CarIns.TravelTaxAmount.ToF2Price();
            ret.TotalAmount = order2CarIns.TotalAmount.ToF2Price();


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


        public CustomJsonResult CarInsGetKind(string operater, string merchantId)
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


        public CustomJsonResult CarInsSubmitUnderwriting(string operater, string merchantId, string belongerId, RopObCalloutCarInsSubmitUnderwriting rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var obCustomer = CurrentDb.ObCustomer.Where(m => m.Id == rop.ObCustomerId).FirstOrDefault();

                var order2CarIns = CurrentDb.Order2CarIns.Where(m => m.ObCustomerId == rop.ObCustomerId && (m.FollowStatus == Enumeration.OrderFollowStatus.CarInsWtUnderwrie || m.FollowStatus == Enumeration.OrderFollowStatus.CarInsInUnderwrie)).FirstOrDefault();

                if (order2CarIns != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该客户已在核保中，不能重复提交，请耐心等候");
                }

                order2CarIns = new Order2CarIns();

                order2CarIns.Id = GuidUtil.New();
                order2CarIns.Sn = SnUtil.Build(Enumeration.BizSnType.Order2CarIns, merchantId);
                order2CarIns.Type = Enumeration.OrderType.CarIns;
                order2CarIns.MerchantId = merchantId;
                order2CarIns.ObCustomerId = rop.ObCustomerId;
                order2CarIns.CompanyId = "";
                order2CarIns.CompanyName = "";
                order2CarIns.CompulsoryAmount = rop.CompulsoryAmount;
                order2CarIns.TravelTaxAmount = rop.TravelTaxAmount;
                order2CarIns.CommercialAmount = rop.CommercialAmount;
                order2CarIns.TotalAmount = rop.CompulsoryAmount + rop.TravelTaxAmount + rop.CommercialAmount;
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
                order2CarIns.BelongerId = belongerId;
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

                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "提交成功");
            }

            return result;
        }


        public CustomJsonResult CarInsGetUnderwritingOrder(string operater, string merchantId, string userId)
        {
            var accessUserIds = MerchServiceFactory.User.GetCanAccessUserIds(operater, merchantId, userId);

            var order2CarInss = CurrentDb.Order2CarIns.Where(m => m.MerchantId == merchantId &&
                           accessUserIds.Contains(m.BelongerId)).ToList();


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