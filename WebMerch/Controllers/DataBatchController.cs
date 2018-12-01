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

        public ActionResult Details()
        {
            return View();
        }

        public CustomJsonResult GetDetails(string id)
        {
            return MerchServiceFactory.DataBatch.GetDetails(this.CurrentUserId, this.CurrentMerchantId, id);
        }

        public CustomJsonResult GetList(RupDataBatchGetList rup)
        {
            var query = (from u in CurrentDb.DataBatch
                         where (rup.Code == null || u.Code.Contains(rup.Code))
                         &&
                         u.MerchantId == this.CurrentMerchantId
                         select new { u.Id, u.Code, u.Name, u.SoureName, u.SoureType, u.Description, u.ValidCount, u.InValidCount, u.Status, u.CreateTime });

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
                    ValidCount = item.ValidCount,
                    InValidCount = item.InValidCount,
                    StatusName = item.Status.GetCnName(),
                    Description = item.Description,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        public CustomJsonResult GetListByCarIns(RupDataBatchGetListByCarIns rup)
        {
            var query = (from u in CurrentDb.DataBatchDetails
                         where
                         u.DataBatchId == rup.DataBatchId
                         &&
                         u.MerchantId == this.CurrentMerchantId
                         select new { u.Id, u.CsrName, u.CsrPhoneNumber, u.CsrAddress, u.CsrIdNumber, u.CarRegisterDate, u.CarPlateNo, u.CarModel, u.CarEngineNo, u.CarVin, u.CarInsLastQzNo, u.CarInsLastSyNo, u.CarInsLastCompany, u.CarInsLastStartTime, u.CarInsLastEndTime, u.CreateTime, u.IsValid, u.HandleReport });

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
                    CsrName = item.CsrName,
                    CsrPhoneNumber = item.CsrPhoneNumber,
                    CsrAddress = item.CsrAddress,
                    CsrIdNumber = item.CsrIdNumber,
                    CarRegisterDate = item.CarRegisterDate,
                    CarPlateNo = item.CarPlateNo,
                    CarModel = item.CarModel,
                    CarEngineNo = item.CarEngineNo,
                    CarVin = item.CarVin,
                    IsValid = item.IsValid,
                    CarInsLastQzNo = item.CarInsLastQzNo,
                    CarInsLastCompany = item.CarInsLastCompany,
                    CarInsLastStartTime = item.CarInsLastStartTime,
                    CarInsLastEndTime = item.CarInsLastEndTime,
                    HandleReport = item.HandleReport,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        [HttpPost]
        public CustomJsonResult AddByFile(RopDataBatchAddByFile rop)
        {
            if (Request.Files.Count == 0)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择上传文件");

            var file = Request.Files[0];

            string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();

            if (fileExtension != ".xls")
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "文件格式类型不符合，必须为.xls文件");
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

            switch (rop.BizType)
            {
                case Enumeration.DataBatchBizType.CarIns:
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
            result = MerchServiceFactory.DataBatch.AddByFile(this.CurrentUserId, this.CurrentMerchantId, rop);

            return result;
        }
    }
}