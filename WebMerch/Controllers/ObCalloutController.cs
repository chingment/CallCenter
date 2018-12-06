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

        public CustomJsonResult TakeData()
        {
            return MerchServiceFactory.ObCallout.TakeData(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId);
        }

        public CustomJsonResult SaveCallRecored(RopObCalloutSaveCallRecored rop)
        {
            rop.TakerId = this.CurrentUserId;

            return MerchServiceFactory.ObCallout.SaveCallRecored(this.CurrentUserId, this.CurrentMerchantId, rop);
        }
    }
}