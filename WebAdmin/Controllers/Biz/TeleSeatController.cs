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
            string account = rup.Account.ToSearchString();
            var query = (from m in CurrentDb.TeleSeat
                         join u in CurrentDb.Merchant on m.MerchantId equals u.Id
                         where
                                 (account.Length == 0 || m.Account.Contains(account))
                                 &&
                                   (rup.MerchantId == null || m.MerchantId.Contains(rup.MerchantId))
                         select new { m.Id, MerchantName = u.Name, m.Account, m.Password, m.CreateTime });

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
                });


            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }
    }
}