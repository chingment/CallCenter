using Lumos;
using Lumos.BLL.Service.Merch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class DataBatchController : OwnBaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult AddByFile()
        {
            return View();
        }

        public CustomJsonResult GetList(RupDataBatchGetList rup)
        {
            var query = (from u in CurrentDb.DataBatch
                         where (rup.Code == null || u.Code.Contains(rup.Code))
                         select new { u.Id, u.Code, u.Name, u.SoureName, u.SoureType, u.StartTime, u.EndTime, u.Description, u.ValidCount, u.InValidCount, u.Status, u.CreateTime });

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
                    Code = item.Code,
                    Name = item.Name,
                    SoureName = item.SoureName,
                    SoureTypeName = item.SoureType.GetCnName(),
                    StartTime = item.StartTime.ToUnifiedFormatDate(),
                    EndTime = item.EndTime.ToUnifiedFormatDate(),
                    ValidCount = item.ValidCount,
                    InValidCount = item.InValidCount,
                    StatusName = item.Status.GetCnName(),
                    Description = item.Description
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public CustomJsonResult AddByFile(RopDataBatchAddByFile rop)
        {
            var s = this;
            var file1 = Request.Files[0];
            var result = new CustomJsonResult();


            return result;
        }
    }
}