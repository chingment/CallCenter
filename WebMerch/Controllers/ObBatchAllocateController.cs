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

        public ActionResult AddByCarIns()
        {
            return View();
        }

        public ActionResult AddByCommon()
        {
            return View();
        }

        public CustomJsonResult GetDetails(string id)
        {
            return MerchServiceFactory.ObBatchAllocate.GetDetails(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, id);
        }

        public CustomJsonResult GetCanAllocateList(RupObBatchAllocateTaskGetList rup)
        {
            var query = (from u in CurrentDb.ObBatchAllocate
                         join b in CurrentDb.ObBatch on u.ObBatchId equals b.Id
                         where
                         u.BelongerId == this.CurrentUserId
                         && u.AllocaterId != this.CurrentUserId
                         &&
                         u.MerchantId == this.CurrentMerchantId &&
                           (rup.Code == null || b.Code.Contains(rup.Code))

                         select new { u.Id, u.IsStopAllocate, ObBatchName = b.Name, u.SoureName, ObBatchCode = b.Code, u.DataCount, u.AllocatedCount, u.UnAllocatedCount, u.UsedCount, u.CreateTime, b.BusinessType });

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
                    UnUsedCount = item.AllocatedCount - item.UsedCount,
                    UsedCount = item.UsedCount,
                    BusinessType = item.BusinessType,
                    IsStopAllocate = item.IsStopAllocate,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        public CustomJsonResult GetAlreadyAllocateList(RupObBatchAllocateTaskGetList rup)
        {

            var obBatchAllocateIds = CurrentDb.ObBatchAllocate.Where(m => m.BelongerId == this.CurrentUserId).Select(m => m.Id).ToArray();

            var query = (from u in CurrentDb.ObBatchAllocate
                         join b in CurrentDb.ObBatch on u.ObBatchId equals b.Id
                         where
                         obBatchAllocateIds.Contains(u.PId)
                         &&
                         u.MerchantId == this.CurrentMerchantId &&
                           (rup.Code == null || b.Code.Contains(rup.Code))
                         select new { u.Id, ObBatchName = b.Name, u.SoureName, u.AllocateMode, ObBatchCode = b.Code, u.DataCount, u.AllocatedCount, u.UnAllocatedCount, u.UsedCount, u.CreateTime, u.BelongerName, u.Description, u.Filters });

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
                    BelongerName = item.BelongerName,
                    ObBatchName = item.ObBatchName,
                    ObBatchCode = item.ObBatchCode,
                    AllocateMode = item.AllocateMode.GetCnName(),
                    DataCount = item.DataCount,
                    AllocatedCount = item.AllocatedCount,
                    UnAllocatedCount = item.UnAllocatedCount,
                    UnUsedCount = item.DataCount - item.UsedCount,
                    UsedCount = item.UsedCount,
                    Description = item.Description,
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


        public CustomJsonResult GetObCustomerListByCarIns(RupObCustomerGetList rup)
        {


            var query = (from u in CurrentDb.ObCustomer
                         where
                         u.ObBatchAllocateId == rup.ObBatchAllocateId
                         &&
                         u.MerchantId == this.CurrentMerchantId &&

                         (rup.CarPlateNo == null || u.CarPlateNo.Contains(rup.CarPlateNo)) &&
                         (rup.CarModel == null || u.CarModel.Contains(rup.CarModel)) &&
                         (rup.CarEngineNo == null || u.CarEngineNo.Contains(rup.CarEngineNo)) &&
                         (rup.CsrAddress == null || u.CsrAddress.Contains(rup.CsrAddress)) &&
                         (rup.CsrPhoneNumber == null || u.CsrPhoneNumber.Contains(rup.CsrPhoneNumber)) &&
                         (rup.CsrName == null || u.CsrName.Contains(rup.CsrName)) &&
                         (rup.CsrIdNumber == null || u.CsrIdNumber.Contains(rup.CsrIdNumber)) &&
                         (rup.CarInsLastCompany == null || u.CarInsLastCompany.Contains(rup.CarInsLastCompany)) &&
                         (rup.CarRegisterDateStart == null || u.CarRegisterDate >= rup.CarRegisterDateStart) &&
                         (rup.CarRegisterDateEnd == null || u.CarRegisterDate <= rup.CarRegisterDateEnd) &&
                         (rup.CarInsLastStartTime == null || u.CarInsLastStartTime >= rup.CarInsLastStartTime) &&
                         (rup.CarInsLastEndTime == null || u.CarInsLastEndTime <= rup.CarInsLastEndTime)

                         select new { u.Id, u.CsrName, u.CsrPhoneNumber, u.CsrAddress, u.CsrIdNumber, u.CarRegisterDate, u.CarPlateNo, u.CarModel, u.CarEngineNo, u.CarVin, u.CarInsLastQzNo, u.CarInsLastSyNo, u.CarInsLastCompany, u.CarInsLastStartTime, u.CarInsLastEndTime, u.CreateTime });

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = rup.PageSize;
            query = query.OrderByDescending(r => r.Id).Skip(pageSize * (pageIndex)).Take(pageSize);


            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    CsrName = item.CsrName,
                    CsrPhoneNumber = item.CsrPhoneNumber,
                    CsrAddress = item.CsrAddress,
                    CsrIdNumber = item.CsrIdNumber,
                    CarRegisterDate = item.CarRegisterDate.ToUnifiedFormatDate(),
                    CarPlateNo = item.CarPlateNo,
                    CarModel = item.CarModel,
                    CarEngineNo = item.CarEngineNo,
                    CarVin = item.CarVin,
                    CarInsLastQzNo = item.CarInsLastQzNo,
                    CarInsLastCompany = item.CarInsLastCompany,
                    CarInsLastStartTime = item.CarInsLastStartTime.ToUnifiedFormatDate(),
                    CarInsLastEndTime = item.CarInsLastEndTime.ToUnifiedFormatDate(),
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        public CustomJsonResult GetObCustomerListByCommon(RupObCustomerGetList rup)
        {


            var query = (from u in CurrentDb.ObCustomer
                         where
                         u.ObBatchAllocateId == rup.ObBatchAllocateId
                         &&
                         u.MerchantId == this.CurrentMerchantId &&
                         (rup.CsrPhoneNumber == null || u.CsrPhoneNumber.Contains(rup.CsrPhoneNumber)) &&
                         (rup.CsrName == null || u.CsrName.Contains(rup.CsrName))

                         select new { u.Id, u.CsrName, u.CsrPhoneNumber, u.CsrAddress, u.CsrCompany, u.CreateTime });

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = rup.PageSize;
            query = query.OrderByDescending(r => r.Id).Skip(pageSize * (pageIndex)).Take(pageSize);


            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    CsrName = item.CsrName,
                    CsrPhoneNumber = item.CsrPhoneNumber,
                    CsrAddress = item.CsrAddress,
                    CsrCompany = item.CsrCompany,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

    }
}