using Lumos.Entity;
using Lumos.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lumos.Common;
using Lumos;
using Lumos.BLL.Service.Admin;

namespace WebAdmin.Controllers.Biz
{
    [OwnAuthorize(AdminPermissionCode.业务系统管理)]
    public class MerchantController : OwnBaseController
    {

        public ViewResult List()
        {
            return View();
        }

        public ViewResult ListBySelect()
        {
            return View();
        }

        public ViewResult Add()
        {
            return View();
        }

        public ViewResult Edit()
        {
            return View();
        }

        public CustomJsonResult GetDetails(string id)
        {
            return AdminServiceFactory.Merchant.GetDetails(this.CurrentUserId, id);
        }

        public CustomJsonResult GetList(RupMerchantGetList rup)
        {
            string name = rup.Name.ToSearchString();
            var query = (from m in CurrentDb.Merchant
                         join u in CurrentDb.SysMerchantUser on m.UserId equals u.Id
                         where
                                 (name.Length == 0 || m.Name.Contains(name))
                         select new { m.Id, u.UserName, m.BusinessType, m.Name, m.ContactName, m.ContactAddress, m.ContactPhone, m.CreateTime });

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
                    Name = item.Name,
                    ContactName = item.ContactName,
                    ContactPhone = item.ContactPhone,
                    ContactAddress = item.ContactAddress,
                    BusinessType = item.BusinessType.GetCnName(),
                    CreateTime = item.CreateTime,
                });


            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public CustomJsonResult Add(RopMerchantAdd rop)
        {
            return AdminServiceFactory.Merchant.Add(this.CurrentUserId, rop);
        }

        [HttpPost]

        public CustomJsonResult Edit(RopMerchantEdit rop)
        {
            return AdminServiceFactory.Merchant.Edit(this.CurrentUserId, rop);
        }

    }
}