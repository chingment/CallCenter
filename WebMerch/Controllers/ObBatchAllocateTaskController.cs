using Lumos;
using Lumos.BLL.Service.Merch;
using Lumos.Common;
using Lumos.Entity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebMerch.Controllers
{
    public class ObBatchAllocateTaskController : OwnBaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Allocate()
        {
            return View();
        }

        public CustomJsonResult GetDetails(string id)
        {
            return MerchServiceFactory.ObBatchAllocateTask.GetDetails(this.CurrentUserId, this.CurrentMerchantId, id);
        }

        public CustomJsonResult GetList(RupDataBatchGetListByCarIns rup)
        {
            var query = (from u in CurrentDb.ObBatchAllocateTask
                         join b in CurrentDb.ObBatch on u.ObBatchId equals b.Id
                         where
                         u.BelongUserId == this.CurrentUserId
                         &&
                         u.MerchantId == this.CurrentMerchantId
                         select new { u.Id, ObBatchName = b.Name, ObBatchCode = b.Code, u.DataCount, u.AllocatedCount, u.UnAllocatedCount, u.UnUsedCount, u.UsedCount, u.CreateTime });

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
                    ObBatchName = item.ObBatchName,
                    ObBatchCode = item.ObBatchCode,
                    DataCount = item.DataCount,
                    AllocatedCount = item.AllocatedCount,
                    UnAllocatedCount = item.UnAllocatedCount,
                    UnUsedCount = item.UnUsedCount,
                    UsedCount = item.UsedCount,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }
    }
}