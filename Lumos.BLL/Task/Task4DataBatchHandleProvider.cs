using Lumos.Entity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Task
{
    public class Task4DataBatchHandleProvider : BaseProvider, IJob
    {
        public void Execute(IJobExecutionContext context)
        {

            LogUtil.Info(this.DateTime.ToShortTimeString());

            var dataBatchsByWaitHandle = CurrentDb.DataBatch.Where(m => m.Status == Entity.Enumeration.DataBatchStatus.WaitHandle).ToList();

            foreach (var dataBatch in dataBatchsByWaitHandle)
            {
                dataBatch.Status = Entity.Enumeration.DataBatchStatus.Handling;
                dataBatch.Mender = GuidUtil.New();
                dataBatch.MendTime = this.DateTime;
                CurrentDb.SaveChanges();
            }

            var dataBatchsByHandling = CurrentDb.DataBatch.Where(m => m.Status == Entity.Enumeration.DataBatchStatus.Handling).ToList();

            foreach (var dataBatch in dataBatchsByHandling)
            {
                if (dataBatch.SoureType == Entity.Enumeration.DataBatchSoureType.File)
                {
                    if (!string.IsNullOrEmpty(dataBatch.FilePath))
                    {
                        if (File.Exists(dataBatch.FilePath))
                        {
                            FileStream fsRead = new FileStream(dataBatch.FilePath, FileMode.Open);
                            HSSFWorkbook workbook = new HSSFWorkbook(fsRead);
                            ISheet sheet = workbook.GetSheetAt(0);
                            int rowCount = sheet.LastRowNum + 1;
                            for (int i = 1; i < rowCount; i++)
                            {
                                IRow row = sheet.GetRow(i);

                                var dataBatchDetail = new DataBatchDetails();
                                dataBatchDetail.Id = GuidUtil.New();
                                dataBatchDetail.MerchantId = dataBatch.MerchantId;
                                dataBatchDetail.DataBatchId = dataBatch.Id;
                                dataBatchDetail.CsrName = "";
                                dataBatchDetail.CsrPhoneNumber = "";
                                dataBatchDetail.CsrAddress = "";
                                dataBatchDetail.CsrIdNumber = "";
                                dataBatchDetail.CarRegisterDate = "";
                                dataBatchDetail.CarPlateNo = "";
                                dataBatchDetail.CarModel = "";
                                dataBatchDetail.CarEngineNo = "";
                                dataBatchDetail.CarVin = "";
                                dataBatchDetail.CarInsLastQzNo = "";
                                dataBatchDetail.CarInsLastSyNo = "";
                                dataBatchDetail.CarInsLastCompany = "";
                                dataBatchDetail.CarInsLastStartTime = "";
                                dataBatchDetail.CarInsLastEndTime = "";
                                dataBatchDetail.Creator = GuidUtil.New();
                                dataBatchDetail.CreateTime = this.DateTime;
                                CurrentDb.DataBatchDetails.Add(dataBatchDetail);
                                CurrentDb.SaveChanges();
                            }

                            dataBatch.Status = Entity.Enumeration.DataBatchStatus.Complete;
                            dataBatch.Mender = GuidUtil.New();
                            dataBatch.MendTime = this.DateTime;
                            CurrentDb.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
