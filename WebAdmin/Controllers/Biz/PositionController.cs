using Lumos;
using Lumos.BLL;
using Lumos.BLL.Service.Admin;
using Lumos.Common;
using Lumos.DAL.AuthorizeRelay;
using Lumos.Entity;
using Lumos.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace WebAdmin.Controllers.Sys
{
    [OwnAuthorize(SysPermissionCode.业务系统管理)]
    public class PositionController : OwnBaseController
    {
        #region 视图
        public ViewResult List()
        {
            return View();
        }
        #endregion

        #region 方法

        public CustomJsonResult GetAll()
        {
            object obj = Newtonsoft.Json.JsonConvert.DeserializeObject(ConvertToZTreeJson2(CurrentDb.MchPosition.ToArray(), "id", "pid", "name", "role"));
            return Json(ResultType.Success, obj);
        }

        public CustomJsonResult GetDetails(string positionId)
        {
            return AdminServiceFactory.Position.GetDetails(this.CurrentUserId, positionId);
        }

        public CustomJsonResult GetPositionMenus(string positionId)
        {
            var positionMenus = AdminServiceFactory.Position.GetPositionMenus(this.CurrentUserId, positionId);
            var isCheckedIds = from p in positionMenus select p.Id;
            object obj = Newtonsoft.Json.JsonConvert.DeserializeObject(ConvertToZTreeJson(CurrentDb.MchMenu.OrderByDescending(m => m.Priority).ToArray(), "id", "pid", "name", "menu", isCheckedIds.ToArray()));
            return Json(ResultType.Success, obj);

        }

        [HttpPost]
        public CustomJsonResult SavePositionMenu(string positionId, string[] menuIds)
        {
            return AdminServiceFactory.Position.SavePositionMenu(this.CurrentUserId, positionId, menuIds);
        }

        #endregion
    }
}