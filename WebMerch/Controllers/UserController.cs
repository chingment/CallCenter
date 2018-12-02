﻿using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using Lumos.Entity;
using Lumos.DAL.AuthorizeRelay;
using Lumos.Common;
using Lumos.Web.Mvc;
using Lumos.DAL;
using Lumos.BLL;
using Lumos;
using Lumos.BLL.Service.Merch;

namespace WebMerch.Controllers
{
    public class UserController : OwnBaseController
    {

        #region 视图

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
        #endregion

        public CustomJsonResult GetList(RupUserGetList rup)
        {
            var list = (from u in CurrentDb.SysMerchantUser
                        where (rup.UserName == null || u.UserName.Contains(rup.UserName)) &&
                        (rup.FullName == null || u.FullName.Contains(rup.FullName)) &&
                        u.IsDelete == false &&
                        u.MerchantId == this.CurrentMerchantId
                        select new { u.Id, u.UserName, u.FullName, u.Email, u.PhoneNumber, u.CreateTime, u.IsDelete });

            int total = list.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = 10;
            list = list.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list };

            return Json(ResultType.Success, pageEntity, "");
        }

        public CustomJsonResult GetDetails(string id)
        {
            return MerchServiceFactory.User.GetDetails(this.CurrentUserId,this.CurrentMerchantId, id);
        }

        [HttpPost]
        public CustomJsonResult Add(RopUserAdd rop)
        {
            return MerchServiceFactory.User.Add(this.CurrentUserId, this.CurrentMerchantId, rop);
        }

        [HttpPost]
        public CustomJsonResult Edit(RopUserEdit rop)
        {
            return MerchServiceFactory.User.Edit(this.CurrentUserId, this.CurrentMerchantId, rop);
        }

        [HttpPost]
        public CustomJsonResult Delete(string id)
        {
            return MerchServiceFactory.User.Delete(this.CurrentUserId, this.CurrentMerchantId, id);
        }

    }
}