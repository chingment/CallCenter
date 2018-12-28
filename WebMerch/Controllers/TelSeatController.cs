using Lumos;
using Lumos.BLL.Service.Merch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class TelSeatController : OwnBaseController
    {
        // GET: TelSeat
        public ActionResult List()
        {
            return View();
        }

        public CustomJsonResult GetList(RupUserGetList rup)
        {
            var list = UserDataCacheUtil.GetList(this.CurrentMerchantId,this.CurrentUserId);

            var data = new { telSeats = list };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", data);
        }
    }
}