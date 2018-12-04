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
    public class ObBatchAllocateController : OwnBaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public CustomJsonResult GetDetails(string id)
        {
            return MerchServiceFactory.ObBatchAllocate.GetDetails(this.CurrentUserId, this.CurrentMerchantId, id);
        }

        public CustomJsonResult GetCanAllocateList(RupObBatchAllocateTaskGetList rup)
        {
            var query = (from u in CurrentDb.ObBatchAllocate
                         join b in CurrentDb.ObBatch on u.ObBatchId equals b.Id
                         where
                         u.BelongUserId == this.CurrentUserId
                         &&
                         u.MerchantId == this.CurrentMerchantId &&
                           (rup.Code == null || b.Code.Contains(rup.Code))
                         select new { u.Id, ObBatchName = b.Name, u.SoureName, ObBatchCode = b.Code, u.DataCount, u.AllocatedCount, u.UnAllocatedCount, u.UnUsedCount, u.UsedCount, u.CreateTime });

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
                    SoureName = item.SoureName,
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

        public CustomJsonResult GetAlreadyAllocateList(RupObBatchAllocateTaskGetList rup)
        {

            var obBatchAllocateIds = CurrentDb.ObBatchAllocate.Where(m => m.BelongUserId == this.CurrentUserId).Select(m => m.Id).ToArray();

            var query = (from u in CurrentDb.ObBatchAllocate
                         join b in CurrentDb.ObBatch on u.ObBatchId equals b.Id
                         where
                         obBatchAllocateIds.Contains(u.PId)
                         &&
                         u.MerchantId == this.CurrentMerchantId &&
                           (rup.Code == null || b.Code.Contains(rup.Code))
                         select new { u.Id, ObBatchName = b.Name, u.SoureName, ObBatchCode = b.Code, u.DataCount, u.AllocatedCount, u.UnAllocatedCount, u.UnUsedCount, u.UsedCount, u.CreateTime, u.BelongUserName });

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
                    SoureName = item.SoureName,
                    BelongUserName = item.BelongUserName,
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

        [HttpPost]
        public CustomJsonResult Add(RopObBatchAllocateAdd rop)
        {
            return MerchServiceFactory.ObBatchAllocate.Add(this.CurrentUserId, this.CurrentMerchantId, rop);
        }

    }
}