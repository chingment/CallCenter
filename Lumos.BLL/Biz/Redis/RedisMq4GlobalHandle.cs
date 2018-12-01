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
                                dataBatchDetail.CreateTime = DateTime.Now;
                                CurrentDb.DataBatchDetails.Add(dataBatchDetail);
                                CurrentDb.SaveChanges();
                            }

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
