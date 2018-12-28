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
    }
}