using Lumos;
using Lumos.BLL.Service.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAdmin.Controllers.Biz
{
    [OwnAuthorize(AdminPermissionCode.业务系统管理)]
    public class TeleSeatController : OwnBaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public CustomJsonResult GetList(RupTeleSeatGetList rup)
        {

            var query = (from m in CurrentDb.TeleSeat
                         join u in CurrentDb.Merchant on m.MerchantId equals u.Id
                         join s in CurrentDb.SysMerchantUser on m.Id equals s.TeleSeatId into temp
                         from tt in temp.DefaultIfEmpty()
                         where
                          (rup.Account == null || m.Account.Contains(rup.Account)) &&
                         (rup.UserName == null || tt.UserName.Contains(rup.UserName)) &&
                                   (rup.MerchantId == null || m.MerchantId.Contains(rup.MerchantId))
                         select new { m.Id, MerchantName = u.Name, m.Account, m.Password, m.CreateTime, tt.UserName, tt.FullName, UserId = tt.Id });

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
                    MerchantName = item.MerchantName,
                    Account = item.Account,
                    Password = item.Password,
                    CreateTime = item.CreateTime,
                    StatusName = item.UserId == null ? "未使用" : "已使用",
                    UserName = item.UserId == null ? "" : string.Format("{0}({1})", item.UserName, item.FullName)
                });


            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }
    }
}