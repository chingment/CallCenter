using Lumos;
using Lumos.BLL.Service.Merch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class ObTakeDataLimitController : OwnBaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Adjust()
        {
            return View();
        }

        public CustomJsonResult GetList(RupUserGetList rup)
        {
            var query = (from t in CurrentDb.ObTakeDataLimit
                         join u in CurrentDb.SysMerchantUser
                         on t.SalesmanId equals u.Id

                         where (rup.UserName == null || u.UserName.Contains(rup.UserName)) &&
                         (rup.FullName == null || u.FullName.Contains(rup.FullName)) &&
                         u.IsDelete == false &&
                         u.IsCanDelete == true &&
                         u.MerchantId == this.CurrentMerchantId
                         select new { t.Id, u.UserName, u.FullName, t.TaskQuantity, t.UnTakeQuantity, t.TakedQuantity,t.CreateTime });

            //if (!string.IsNullOrEmpty(rup.OrganizationId))
            //{
            //    query = query.Where(m => m.OrganizationId == rup.OrganizationId);
            //}

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
                    UserName = item.UserName,
                    FullName = item.FullName,
                    TaskQuantity = item.TaskQuantity,
                    UnTakeQuantity = item.UnTakeQuantity,
                    TakedQuantity = item.TakedQuantity
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        public CustomJsonResult GetDetails(string id)
        {
            return MerchServiceFactory.ObTakeDataLimit.GetDetails(this.CurrentUserId, this.CurrentMerchantId, id);
        }

        [HttpPost]
        public CustomJsonResult Adjust(RopObTakeDataLimitAdjust rop)
        {
            return MerchServiceFactory.ObTakeDataLimit.Adjust(this.CurrentUserId, this.CurrentMerchantId, rop);
        }
    }
}