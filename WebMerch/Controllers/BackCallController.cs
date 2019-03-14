using Lumos;
using Lumos.BLL.Service.Merch;
using Lumos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class BackCallController : OwnBaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Handle()
        {
            return View();
        }

        public CustomJsonResult GetListByToday(RupCallResultRecordGetList rup)
        {
            var accessUserIds = MerchServiceFactory.User.GetCanAccessUserIds(this.CurrentMerchantId, this.CurrentUserId);

            var query = (from u in CurrentDb.CallResultRecord
                         where
u.MerchantId == this.CurrentMerchantId &&
 accessUserIds.Contains(u.SalesmanId) &&
 (rup.CustomerName == null || u.CustomerName.Contains(rup.CustomerName)) &&
 (rup.PhoneNumber == null || u.CustomerPhoneNumber.Contains(rup.PhoneNumber))
                         select new { u.Id, u.CustomerId, u.CustomerName, u.SalesmanId, u.SalesmanName, u.ResultName, u.ResultCode, u.CustomerPhoneNumber, u.CreateTime, u.NextCallTime, u.Remark });


            DateTime start = DateTime.Parse(CommonUtil.ConverToShortDateStart(DateTime.Now));
            DateTime end = DateTime.Parse(CommonUtil.ConverToShortDateEnd(DateTime.Now));

            query = query.Where(m => m.NextCallTime >= start && m.NextCallTime <= end);


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
                    CustomerId = item.CustomerId,
                    CustomerName = item.CustomerName,
                    SalesmanId = item.SalesmanId,
                    SalesmanName = item.SalesmanName,
                    ResultName = item.ResultName,
                    ResultCode = item.ResultCode,
                    CustomerPhoneNumber = item.CustomerPhoneNumber,
                    NextCallTime = item.NextCallTime.ToUnifiedFormatDateTime(),
                    Remark = item.Remark,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }
    }
}