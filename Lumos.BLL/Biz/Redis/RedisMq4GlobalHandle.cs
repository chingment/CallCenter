using Lumos.Common;
using Lumos.DAL;
using Lumos.Entity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Transactions;

namespace Lumos.BLL.Biz
{
    public enum RedisMqHandleType
    {
        [Remark("未知")]
        Unknow = 0,
        [Remark("数据批次")]
        DataBatch = 1
    }

    public class RedisMq4GlobalHandle
    {
        public RedisMqHandleType Type { get; set; }
        public object Pms { get; set; }
        private static readonly object lock_Handle = new object();
        public void Handle()
        {
            LogUtil.Info("type:" + this.Type.GetCnName());
            //LogUtil.Info("pms:"+)
            lock (lock_Handle)
            {
                try
                {
                    switch (this.Type)
                    {
                        case RedisMqHandleType.DataBatch:
                            HandleByDataBatch(this.Pms.ToJsonObject<DataBatch>());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error("消息队列，发生异常", ex);
                }
            }
        }


        private void HandleByDataBatch(DataBatch rop)
        {
            LumosDbContext CurrentDb = new LumosDbContext();

            var dataBatch = CurrentDb.DataBatch.Where(m => m.Id == rop.Id && m.Status == Entity.Enumeration.DataBatchStatus.WaitHandle).FirstOrDefault();

            if (dataBatch == null)
            {
                LogUtil.Info("找不到处理的批次");
                return;
            }

            dataBatch.Status = Entity.Enumeration.DataBatchStatus.Handling;
            dataBatch.Mender = GuidUtil.New();
            dataBatch.MendTime = DateTime.Now;
            CurrentDb.SaveChanges();

            //Thread.Sleep(10000);

            using (TransactionScope ts = new TransactionScope())
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

                            int validCount = 0;
                            int inValidCount = 0;
                            for (int i = 1; i < rowCount; i++)
                            {
                                IRow row = sheet.GetRow(i);

                                var csrPhoneNumber = NPOIHelperUtil.GetCellValue(row.GetCell(8));

                                var l_DataBatchDetail = CurrentDb.DataBatchDetails.Where(m => m.MerchantId == dataBatch.MerchantId && m.CsrPhoneNumber == csrPhoneNumber).FirstOrDefault();

                                bool isValid = true;
                                string handleReport = "";
                                if (l_DataBatchDetail == null)
                                {
                                    handleReport = "有效分配数据：未重复";
                                    isValid = true;
                                    validCount += 1;
                                }
                                else
                                {
                                    if (l_DataBatchDetail.RecoveryTime >= DateTime.Now)
                                    {
                                        if (l_DataBatchDetail.DataBatchId == dataBatch.Id)
                                        {
                                            handleReport = "无效分配数据:与本批次重复";
                                        }
                                        else
                                        {
                                            handleReport = "无效分配数据:与其他批次重复，未到回收时间";
                                        }

                                        isValid = false;
                                        inValidCount += 1;
                                    }
                                    else
                                    {
                                        handleReport = "有效分配数据:数据重复，已到回收时间";
                                        isValid = true;
                                        validCount += 1;
                                    }
                                }


                                var dataBatchDetail = new DataBatchDetails();
                                dataBatchDetail.Id = GuidUtil.New();
                                dataBatchDetail.MerchantId = dataBatch.MerchantId;
                                dataBatchDetail.DataBatchId = dataBatch.Id;
                                dataBatchDetail.CsrName = NPOIHelperUtil.GetCellValue(row.GetCell(6));
                                dataBatchDetail.CsrPhoneNumber = csrPhoneNumber;
                                dataBatchDetail.CsrAddress = NPOIHelperUtil.GetCellValue(row.GetCell(7));
                                dataBatchDetail.CsrIdNumber = NPOIHelperUtil.GetCellValue(row.GetCell(3));
                                dataBatchDetail.CarRegisterDate = NPOIHelperUtil.GetCellValue(row.GetCell(0));
                                dataBatchDetail.CarPlateNo = NPOIHelperUtil.GetCellValue(row.GetCell(1));
                                dataBatchDetail.CarModel = NPOIHelperUtil.GetCellValue(row.GetCell(2));
                                dataBatchDetail.CarEngineNo = NPOIHelperUtil.GetCellValue(row.GetCell(5));
                                dataBatchDetail.CarVin = NPOIHelperUtil.GetCellValue(row.GetCell(4));
                                dataBatchDetail.CarInsLastQzNo = NPOIHelperUtil.GetCellValue(row.GetCell(9));
                                dataBatchDetail.CarInsLastSyNo = NPOIHelperUtil.GetCellValue(row.GetCell(9));
                                dataBatchDetail.CarInsLastCompany = NPOIHelperUtil.GetCellValue(row.GetCell(12));
                                dataBatchDetail.CarInsLastStartTime = NPOIHelperUtil.GetCellValue(row.GetCell(10));
                                dataBatchDetail.CarInsLastEndTime = NPOIHelperUtil.GetCellValue(row.GetCell(11));
                                dataBatchDetail.ExpiryTime = dataBatch.ExpiryTime;
                                dataBatchDetail.RecoveryTime = dataBatch.RecoveryTime;
                                dataBatchDetail.FollowDelayDays = dataBatch.FollowDelayDays;
                                dataBatchDetail.IsValid = isValid;
                                dataBatchDetail.HandleReport = handleReport;
                                dataBatchDetail.Creator = GuidUtil.New();
                                dataBatchDetail.CreateTime = DateTime.Now;
                                CurrentDb.DataBatchDetails.Add(dataBatchDetail);
                                CurrentDb.SaveChanges();
                            }

                            dataBatch.ValidCount = validCount;
                            dataBatch.InValidCount = inValidCount;
                            dataBatch.Status = Entity.Enumeration.DataBatchStatus.Complete;
                            dataBatch.Mender = GuidUtil.New();
                            dataBatch.MendTime = DateTime.Now;
                            CurrentDb.SaveChanges();
                            ts.Complete();
                        }
                    }
                }
            }

        }
    }
}
