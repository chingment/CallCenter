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
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Monitor()
        {
            return View();
        }

        public CustomJsonResult GetMonitor(RupUserGetList rup)
        {
            var list = UserDataCacheUtil.GetList(this.CurrentMerchantId, this.CurrentUserId);

            var data = new { telSeats = list };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", data);
        }


        public CustomJsonResult GetList(RupTeleSeatGetList rup)
        {
            var query = (from u in CurrentDb.TeleSeat
                         join m in CurrentDb.SysMerchantUser on u.Id equals m.TeleSeatId into temp
                         from tt in temp.DefaultIfEmpty()
                         where (rup.Account == null || u.Account.Contains(rup.Account)) &&
                         (rup.UserName == null || tt.UserName.Contains(rup.UserName))

                         select new { u.Id, u.Account, u.Password, tt.UserName, tt.FullName, u.CreateTime, UserId = tt.Id });

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = 10;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    Account = item.Account,
                    Password = item.Password,
                    StatusName = item.UserId == null ? "未使用" : "已使用",
                    UserName = item.UserId == null ? "" : string.Format("{0}({1})", item.UserName, item.FullName),
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }
    }
}