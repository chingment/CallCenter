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


namespace WebMerch.Controllers
{
    public class CallRecordController : OwnBaseController
    {

        public ActionResult List()
        {
            return View();
        }

        public CustomJsonResult GetList(RupCallRecordGetList rup)
        {
            var accessUserIds = MerchServiceFactory.User.GetCanAccessUserIds(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId);

            var query = (from u in CurrentDb.CallRecord
                         where
u.MerchantId == this.CurrentMerchantId &&
 accessUserIds.Contains(u.SalesmanId)
                         select new { u.Id, u.CustomerId, u.CustomerName, u.SalesmanId, u.SalesmanName, u.RecordFile, u.TimeLength, u.PhoneNumber, u.CreateTime, u.RingTime, u.AnswerTime, u.ByeTime, u.StartTime });


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
                    RecordFile = item.RecordFile,
                    TimeLength = item.TimeLength,
                    PhoneNumber = item.PhoneNumber,
                    RingTime = item.RingTime.ToUnifiedFormatDateTime(),
                    AnswerTime = item.AnswerTime.ToUnifiedFormatDateTime(),
                    ByeTime = item.ByeTime.ToUnifiedFormatDateTime(),
                    StartTime = item.StartTime.ToUnifiedFormatDateTime(),
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }
    }
}