using Lumos;
using Lumos.BLL;
using Lumos.BLL.Biz;
using Lumos.BLL.Service.Merch;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class OrganizationController : OwnBaseController
    {
        public ViewResult List()
        {
            return View();
        }

        public ViewResult Add()
        {
            return View();
        }

        public CustomJsonResult GetTreeList(int pId)
        {

            var arr = CurrentDb.Organization.Where(m => m.MerchantId == this.CurrentUserId && m.IsDelete == false).OrderByDescending(m => m.Priority).ToArray();

            object json = ConvertToZTreeJson(arr, "id", "pid", "name", "menu");
            return Json(ResultType.Success, json);

        }

        public CustomJsonResult GetDetails(string organizationId)
        {
            return MerchServiceFactory.Organization.GetDetails(this.CurrentUserId, this.CurrentUserId, organizationId);
        }

        [HttpPost]
        public CustomJsonResult Add(RopOrganizationAdd rop)
        {
            return MerchServiceFactory.Organization.Add(this.CurrentUserId, this.CurrentUserId, rop);
        }

        [HttpPost]
        public CustomJsonResult Edit(RopOrganizationEdit rop)
        {
            return MerchServiceFactory.Organization.Edit(this.CurrentUserId, this.CurrentUserId, rop);
        }

        [HttpPost]
        public CustomJsonResult Delete(string[] organizationIds)
        {
            return MerchServiceFactory.Organization.Delete(this.CurrentUserId, this.CurrentUserId, organizationIds);
        }

        public CustomJsonResult GetUserList(RupUserGetList rup)
        {

            string name = rup.Name.ToSearchString();
            var query = (from p in CurrentDb.SysMerchantUser

                         where
                         p.OrganizationId == rup.OrganizationId &&
   (name.Length == 0 || p.FullName.Contains(name))
   && p.MerchantId == this.CurrentUserId
                         select new { p.Id, p.OrganizationId, p.UserName, p.FullName, p.CreateTime });

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = 10;
            query = query.OrderBy(r => r.Id).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();
            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    OrganizationId = item.OrganizationId,
                    UserName = item.UserName,
                    FullName = item.UserName,
                    CreateTime = item.CreateTime,
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity);
        }
    }
}
