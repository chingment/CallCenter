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
        public ActionResult TeleMarket()
        {
            return View();
        }

        public ActionResult CarInsUnderwritingOrderDetails()
        {
            return View();
        }

        public CustomJsonResult TakeData()
        {
            return MerchServiceFactory.ObCallout.TakeData(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId);
        }

        public CustomJsonResult SaveCallResultRecord(RopObCalloutSaveCallResultRecord rop)
        {
            return MerchServiceFactory.ObCallout.SaveCallResultRecord(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, rop);
        }

        public CustomJsonResult GetInitConfig()
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

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", new { TaskQuantity = obTakeDataLimit.TaskQuantity, UnTakeQuantity = obTakeDataLimit.UnTakeQuantity, TakedQuantity = obTakeDataLimit.TakedQuantity });
        }
    }
}