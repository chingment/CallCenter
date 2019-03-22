using Lumos;
using Lumos.BLL.Service.Merch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class ObCalloutController : OwnBaseController
    {
        public ActionResult TeleMarketByCommon()
        {
            return View();
        }

        public ActionResult TeleMarketByCarIns()
        {
            return View();
        }

        public ActionResult CarInsUnderwritingOrderDetails()
        {
            return View();
        }

        public CustomJsonResult TakeData(string customerId)
        {
            return MerchServiceFactory.ObCallout.TakeData(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, customerId);
        }

        public CustomJsonResult SkipData(string customerId)
        {
            return MerchServiceFactory.ObCallout.SkipData(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, customerId);
        }

        public CustomJsonResult SaveCallResultRecord(RopObCalloutSaveCallResultRecord rop)
        {
            return MerchServiceFactory.ObCallout.SaveCallResultRecord(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, rop);
        }

        public CustomJsonResult InitDataByTeleMarketByCarIns()
        {

            var carInsKinds = MerchServiceFactory.CarIns.GetKinds(this.CurrentUserId, this.CurrentMerchantId);
            var carInsCompanys = MerchServiceFactory.CarIns.GetCompanys(this.CurrentUserId, this.CurrentMerchantId);
            var ret = new { carInsKinds = carInsKinds, carInsCompanys = carInsCompanys };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult InitDataByTeleMarketByCommon()
        {

            var carInsKinds = MerchServiceFactory.CarIns.GetKinds(this.CurrentUserId, this.CurrentMerchantId);
            var carInsCompanys = MerchServiceFactory.CarIns.GetCompanys(this.CurrentUserId, this.CurrentMerchantId);
            var ret = new { carInsKinds = carInsKinds, carInsCompanys = carInsCompanys };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult CarInsGetUnderwritingOrderDetails(string orderId)
        {
            return MerchServiceFactory.CarIns.GetUnderwritingOrderDetails(this.CurrentUserId, this.CurrentMerchantId, orderId);
        }

        public CustomJsonResult CarInsSubmitUnderwriting(RopObCalloutCarInsSubmitUnderwriting rop)
        {
            return MerchServiceFactory.CarIns.SubmitUnderwriting(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, rop);
        }

        public CustomJsonResult GetDealtTrack(string customerId)
        {
            return MerchServiceFactory.ObCallout.GetDealtTrack(this.CurrentUserId, this.CurrentMerchantId, customerId);
        }

        public CustomJsonResult GetTaskDataStatus()
        {
            var obTakeDataLimit = CurrentDb.ObTakeDataLimit.Where(m => m.MerchantId == this.CurrentMerchantId && m.SalesmanId == this.CurrentUserId).FirstOrDefault();

            int taskQuantity = 0;
            int unTakeQuantity = 0;
            int takedQuantity = 0;
            if (obTakeDataLimit != null)
            {
                taskQuantity = obTakeDataLimit.TaskQuantity;
                unTakeQuantity = obTakeDataLimit.UnTakeQuantity;
                takedQuantity = obTakeDataLimit.TakedQuantity;
            }
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", new { TaskQuantity = taskQuantity, UnTakeQuantity = unTakeQuantity, TakedQuantity = takedQuantity });
        }


        public CustomJsonResult GetBookingCustomers()
        {

            var query = (from u in CurrentDb.CallResultRecord
                         where
u.MerchantId == this.CurrentMerchantId &&
u.SalesmanId == this.CurrentUserId &&
u.IsBackCalled == false &&
u.NextCallTime != null
                         select new { u.Id, u.CustomerId, u.CustomerName, u.SalesmanId, u.SalesmanName, u.ResultName, u.ResultCode, u.CustomerPhoneNumber, u.CreateTime, u.NextCallTime, u.Remark });


            query = query.OrderBy(r => r.CreateTime);



            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    CustomerId = item.CustomerId,
                    CustomerName = item.CustomerName,
                    ResultName = item.ResultName,
                    NextCallTime = item.NextCallTime.ToUnifiedFormatDateTime()
                });
            }

            return Json(ResultType.Success, olist, "");
        }
    }
}