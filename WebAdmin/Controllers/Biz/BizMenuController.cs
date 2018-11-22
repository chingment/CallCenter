using Lumos;
using Lumos.BLL.Service.Admin;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAdmin.Controllers.Biz
{
    [OwnAuthorize(SysPermissionCode.业务系统管理)]
    public class BizMenuController : OwnBaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Sort()
        {
            return View();
        }

        public CustomJsonResult GetPermissions()
        {
            return AdminServiceFactory.BizMenu.GetPermissions(this.CurrentUserId);
        }


        public CustomJsonResult GetDetails(string menuId)
        {
            return AdminServiceFactory.BizMenu.GetDetails(this.CurrentUserId, menuId);
        }

        public CustomJsonResult GetAll(string pMenuId)
        {
            MchMenu[] arr;
            if (pMenuId == "0")
            {
                arr = CurrentDb.MchMenu.OrderByDescending(m => m.Priority).ToArray();
            }
            else
            {
                arr = CurrentDb.MchMenu.Where(m => m.PId == pMenuId).OrderByDescending(m => m.Priority).ToArray();
            }

            object json = ConvertToZTreeJson(arr, "id", "pid", "name", "menu");
            return Json(ResultType.Success, json);
        }


        [HttpPost]
        [OwnNoResubmit]
        public CustomJsonResult Add(RopBizMenuAdd rop)
        {
            return AdminServiceFactory.BizMenu.Add(this.CurrentUserId, rop);
        }

        [HttpPost]
        public CustomJsonResult Edit(RopBizMenuEdit rop)
        {
            return AdminServiceFactory.BizMenu.Edit(this.CurrentUserId, rop);

        }

        [HttpPost]
        public CustomJsonResult Delete(string[] menuIds)
        {
            return AdminServiceFactory.BizMenu.Delete(this.CurrentUserId, menuIds);
        }

        [HttpPost]
        public CustomJsonResult EditSort(RopBizMenuEditSort rop)
        {
            return AdminServiceFactory.BizMenu.EditSort(this.CurrentUserId, rop);
        }
    }
}