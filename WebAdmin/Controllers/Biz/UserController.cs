using System;
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
using System.Collections.Generic;

namespace WebAdmin.Controllers.Biz
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
            var query = (from u in CurrentDb.SysMerchantUser
                         join o in CurrentDb.Organization on u.OrganizationId equals o.Id
                         where (rup.UserName == null || u.UserName.Contains(rup.UserName)) &&
                         (rup.FullName == null || u.FullName.Contains(rup.FullName)) &&
                         u.IsDelete == false &&
                         u.IsCanDelete == true &&
                         u.MerchantId == rup.MerchantId
                         select new { u.Id, u.UserName, u.FullName, u.PositionId, OrganizationName = o.FullName, u.OrganizationId, u.Email, u.PhoneNumber, u.CreateTime, u.IsDelete, u.Status });

            if (!string.IsNullOrEmpty(rup.OrganizationId))
            {
                query = query.Where(m => m.OrganizationId == rup.OrganizationId);
            }

            if (rup.IsDataAllocater)
            {
                Enumeration.SysPositionId[] positionIds = new Enumeration.SysPositionId[] { Enumeration.SysPositionId.MerchantGM, Enumeration.SysPositionId.MerchantUM, Enumeration.SysPositionId.MerchantTL };
                query = query.Where(m => positionIds.Contains(m.PositionId) && m.Status == Enumeration.UserStatus.Normal);
            }

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
                    Email = item.Email,
                    OrganizationName = item.OrganizationName,
                    PositionName = item.PositionId.GetCnName(),
                    PhoneNumber = item.PhoneNumber,
                    StatusName = item.Status.GetCnName(),
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

    }
}