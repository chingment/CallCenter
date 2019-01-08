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
    
    public class ObBatchController : OwnBaseController
    {
        public ActionResult List()
        {
            return View();
        }

        public ActionResult AddByFile()
        {
            return View();
        }

        public ActionResult Details()
        {
            return View();
        }


        public CustomJsonResult GetDetails(string id)
        {
            return MerchServiceFactory.ObBatch.GetDetails(this.CurrentUserId, this.CurrentMerchantId, id);
        }

        public CustomJsonResult GetList(RupObBatchGetList rup)
        {
            var query = (from u in CurrentDb.ObBatch
                         where (rup.Code == null || u.Code.Contains(rup.Code))
                         &&
                         u.MerchantId == this.CurrentMerchantId
                         select new { u.Id, u.Code, u.Name, u.SoureType, u.SoureName, u.Description, u.DataCount, u.ValidCount, u.InValidCount, u.Status, u.CreateTime });

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
                    SoureName = string.Format("（{0}）{1}", item.SoureType.GetCnName(), item.SoureName),
                    DataCount = item.Status != Enumeration.DataBatchStatus.Complete ? "-" : item.DataCount.ToString(),
                    ValidCount = item.Status != Enumeration.DataBatchStatus.Complete ? "-" : item.ValidCount.ToString(),
                    InValidCount = item.Status != Enumeration.DataBatchStatus.Complete ? "-" : item.InValidCount.ToString(),
                    StatusName = item.Status.GetCnName(),
                    Description = item.Description,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public CustomJsonResult AddByFile(RopObBatchAddByFile rop)
        {
            if (Request.Files.Count == 0)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择上传文件");

            var file = Request.Files[0];

            string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();

            if (fileExtension != ".xls")
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "文件格式类型不符合，必须为.xls文件");
            }

            //var belongOrganization = CurrentDb.Organization.Where(m => m.Dept == 0 && m.MerchantId == this.CurrentMerchantId).FirstOrDefault();

            //if (belongOrganization == null)
            //{
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请先设置机构");
            //}


            var belongUser = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == this.CurrentMerchantId && m.Id == rop.BelongerId).FirstOrDefault();

            if (string.IsNullOrEmpty(belongUser.OrganizationId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("数据分配人[{0}]，未设置所属机构", belongUser.FullName));
            }

            var result = new CustomJsonResult();

            HSSFWorkbook workbook = new HSSFWorkbook(file.InputStream);
            ISheet sheet = workbook.GetSheetAt(0);
            if (sheet == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel工作簿为空");
            }

            IRow sheet_DataTitle = sheet.GetRow(0);

            if (sheet_DataTitle == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "Excel工作簿为空");
            }

            ExcelFormatCheckUtil excelFormatCheckUtil = new ExcelFormatCheckUtil(sheet);

            switch (rop.DataBizType)
            {
                case Enumeration.DataBizType.CarIns:
                    #region 车险模板
                    excelFormatCheckUtil.AddCheckCellIsString(0, "初登日期", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(1, "车牌", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(2, "厂牌型号", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(3, "身份证", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(4, "车架号", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(5, "发动机号", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(6, "车主", 1, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(7, "地址", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(8, "电话", 1, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(9, "保单号", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(10, "起保日期", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(11, "终保日期", 0, 200);
                    excelFormatCheckUtil.AddCheckCellIsString(12, "保险公司", 0, 200);
                    #endregion
                    break;
                default:
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "业务类型不符合");
            }

            excelFormatCheckUtil.StartCheck();

            if (excelFormatCheckUtil.ErrorPoint.Count > 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, excelFormatCheckUtil.ErrorMessage, excelFormatCheckUtil.ErrorPoint);
            }


            string fileDir = Server.MapPath("~/Upload/Files/DataBatch/");
            string fileName = GuidUtil.New();
            string filePath = string.Format("{0}{1}{2}", fileDir, fileName, fileExtension);

            FileStream fs = System.IO.File.Create(filePath);
            workbook.Write(fs);
            fs.Close();

            rop.FileName = file.FileName;
            rop.FilePath = filePath;
            rop.BelongerId = belongUser.Id;
            rop.BelongerName = string.Format("{0}机构负责人：{1}({2})", belongUser.FullName, belongUser.FullName, belongUser.UserName);
            rop.BelongerOrganizationId = belongUser.OrganizationId;
            result = MerchServiceFactory.ObBatch.AddByFile(this.CurrentUserId, this.CurrentMerchantId, rop);

            return result;
        }

        public CustomJsonResult GetDataList(RupObBatchDataGetList rup)
        {
            var query = (from u in CurrentDb.ObBatchData
                         where
                         u.ObBatchId == rup.ObBatchId
                         &&
                         u.MerchantId == this.CurrentMerchantId
                         select new { u.Id, u.CsrName, u.CsrPhoneNumber, u.CsrAddress, u.CsrIdNumber, u.CarRegisterDate, u.CarPlateNo, u.CarModel, u.CarEngineNo, u.CarVin, u.CarInsLastQzNo, u.CarInsLastSyNo, u.CarInsLastCompany, u.CarInsLastStartTime, u.CarInsLastEndTime, u.CreateTime, u.IsValid, u.HandleReport });

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = 10;
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
                    IsValid = item.IsValid,
                    CarInsLastQzNo = item.CarInsLastQzNo,
                    CarInsLastCompany = item.CarInsLastCompany,
                    CarInsLastStartTime = item.CarInsLastStartTime.ToUnifiedFormatDate(),
                    CarInsLastEndTime = item.CarInsLastEndTime.ToUnifiedFormatDate(),
                    HandleReport = item.HandleReport,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

    }
}