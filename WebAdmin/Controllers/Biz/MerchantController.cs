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

        public CustomJsonResult GetDetails(string merchantId)
        {
            return AdminServiceFactory.Merchant.GetDetails(this.CurrentUserId, merchantId);
        }

        public CustomJsonResult GetList(RupMerchantGetList rup)
        {
            string name = rup.Name.ToSearchString();
            var query = (from m in CurrentDb.SysMerchantUser
                         join u in CurrentDb.MerchantInfo on m.Id equals u.MerchantId
                         where
                                 (name.Length == 0 || u.Name.Contains(name))
                         select new { m.Id, m.UserName, u.BusinessType, MerchantName = u.Name, u.ContactName, u.ContactAddress, u.ContactPhone, m.CreateTime });

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = 10;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            List<object> list = new List<object>();

            foreach (var item in query)
            {

                list.Add(new
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    MerchantName = item.MerchantName,
                    ContactName = item.ContactName,
                    ContactPhone = item.ContactPhone,
                    CreateTime = item.CreateTime,
                    ContactAddress = item.ContactAddress,
                    BusinessType = item.BusinessType.GetCnName()
                });


            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

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