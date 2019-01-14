using Lumos.Common;
using Lumos.DAL;
using Lumos.Entity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Transactions;

namespace Lumos.BLL.Biz
{
    public enum RedisMqHandleType
    {
        [Remark("未知")]
        Unknow = 0,
        [Remark("外呼数据批次导入")]
        ObBatch = 1
    }

    public class RedisMq4GlobalHandle
    {
        public RedisMqHandleType Type { get; set; }
        public object Pms { get; set; }
        private static readonly object lock_Handle = new object();
        public void Handle()
        {
            LogUtil.Info("type:" + this.Type.GetCnName() + ",pms: " + Newtonsoft.Json.JsonConvert.SerializeObject(this.Pms));

            lock (lock_Handle)
            {
                try
                {
                    switch (this.Type)
                    {
                        case RedisMqHandleType.ObBatch:
                            HandleByDataBatch(this.Pms.ToJsonObject<ObBatch>());
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error("消息队列，发生异常", ex);
                }
            }
        }


        private void HandleByDataBatch(ObBatch rop)
        {
            LumosDbContext CurrentDb = new LumosDbContext();

            var obBatch = CurrentDb.ObBatch.Where(m => m.Id == rop.Id && m.Status == Entity.Enumeration.DataBatchStatus.WaitHandle).FirstOrDefault();

            if (obBatch == null)
            {
                LogUtil.Info("找不到处理的批次");
                return;
            }

            obBatch.Status = Entity.Enumeration.DataBatchStatus.Handling;
            obBatch.Mender = GuidUtil.New();
            obBatch.MendTime = DateTime.Now;
            CurrentDb.SaveChanges();

            using (TransactionScope ts = new TransactionScope())
            {


                var obBatchAllocateId = GuidUtil.New();

                if (obBatch.SoureType == Entity.Enumeration.DataBatchSoureType.File)
                {
                    if (!string.IsNullOrEmpty(obBatch.FilePath))
                    {
                        if (File.Exists(obBatch.FilePath))
                        {
                            var belongUser = CurrentDb.SysUser.Where(m => m.Id == obBatch.BelongerId).FirstOrDefault();

                            FileStream fsRead = new FileStream(obBatch.FilePath, FileMode.Open);
                            HSSFWorkbook workbook = new HSSFWorkbook(fsRead);
                            ISheet sheet = workbook.GetSheetAt(0);
                            int rowCount = sheet.LastRowNum + 1;

                            int validCount = 0;
                            int inValidCount = 0;
                            for (int i = 1; i < rowCount; i++)
                            {
                                #region 数据去重处理

                                IRow row = sheet.GetRow(i);

                                var csrPhoneNumber = NPOIHelperUtil.GetCellValue(row.GetCell(8));

                                var obCustomer = CurrentDb.ObCustomer.Where(m => m.MerchantId == obBatch.MerchantId && m.CsrPhoneNumber == csrPhoneNumber).FirstOrDefault();

                                bool isValid = true;
                                string handleReport = "";
                                if (obCustomer == null)
                                {
                                    handleReport = "有效分配数据：未重复";
                                    isValid = true;
                                    validCount += 1;
                                }
                                else
                                {
                                    if (obCustomer.RecoveryTime >= DateTime.Now)
                                    {
                                        if (obCustomer.ObBatchId == obBatch.Id)
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


                                var obBatchData = new ObBatchData();
                                obBatchData.Id = GuidUtil.New();
                                obBatchData.MerchantId = obBatch.MerchantId;
                                obBatchData.ObBatchId = obBatch.Id;
                                obBatchData.CsrName = NPOIHelperUtil.GetCellValue(row.GetCell(6));
                                obBatchData.CsrPhoneNumber = csrPhoneNumber;
                                obBatchData.CsrAddress = NPOIHelperUtil.GetCellValue(row.GetCell(7));
                                obBatchData.CsrIdNumber = NPOIHelperUtil.GetCellValue(row.GetCell(3));
                                obBatchData.CarRegisterDate = CommonUtil.ConverToStartTime(NPOIHelperUtil.GetCellValue(row.GetCell(0)));
                                obBatchData.CarPlateNo = NPOIHelperUtil.GetCellValue(row.GetCell(1));
                                obBatchData.CarModel = NPOIHelperUtil.GetCellValue(row.GetCell(2));
                                obBatchData.CarEngineNo = NPOIHelperUtil.GetCellValue(row.GetCell(5));
                                obBatchData.CarVin = NPOIHelperUtil.GetCellValue(row.GetCell(4));
                                obBatchData.CarInsLastQzNo = NPOIHelperUtil.GetCellValue(row.GetCell(9));
                                obBatchData.CarInsLastSyNo = NPOIHelperUtil.GetCellValue(row.GetCell(9));
                                obBatchData.CarInsLastCompany = NPOIHelperUtil.GetCellValue(row.GetCell(12));
                                obBatchData.CarInsLastStartTime = CommonUtil.ConverToStartTime(NPOIHelperUtil.GetCellValue(row.GetCell(10)));
                                obBatchData.CarInsLastEndTime = CommonUtil.ConverToEndTime(NPOIHelperUtil.GetCellValue(row.GetCell(11)));
                                obBatchData.BusinessType = obBatch.BusinessType;
                                obBatchData.IsValid = isValid;
                                obBatchData.HandleReport = handleReport;
                                obBatchData.Creator = obBatch.Creator;
                                obBatchData.CreateTime = obBatch.CreateTime;
                                CurrentDb.ObBatchData.Add(obBatchData);
                                CurrentDb.SaveChanges();

                                if (isValid)
                                {
                                    obCustomer = new ObCustomer();
                                    obCustomer.Id = GuidUtil.New();
                                    obCustomer.MerchantId = obBatch.MerchantId;
                                    obCustomer.ObBatchId = obBatch.Id;
                                    obCustomer.ObBatchDataId = obBatchData.Id;
                                    obCustomer.ObBatchAllocateId = obBatchAllocateId;
                                    obCustomer.CsrName = obBatchData.CsrName;
                                    obCustomer.CsrPhoneNumber = csrPhoneNumber;
                                    obCustomer.CsrAddress = obBatchData.CsrAddress;
                                    obCustomer.CsrIdNumber = obBatchData.CsrIdNumber;
                                    obCustomer.CarRegisterDate = obBatchData.CarRegisterDate;
                                    obCustomer.CarPlateNo = obBatchData.CarPlateNo;
                                    obCustomer.CarModel = obBatchData.CarModel;
                                    obCustomer.CarEngineNo = obBatchData.CarEngineNo;
                                    obCustomer.CarVin = obBatchData.CarVin;
                                    obCustomer.CarInsLastQzNo = obBatchData.CarInsLastQzNo;
                                    obCustomer.CarInsLastSyNo = obBatchData.CarInsLastSyNo;
                                    obCustomer.CarInsLastCompany = obBatchData.CarInsLastCompany;
                                    obCustomer.CarInsLastStartTime = obBatchData.CarInsLastStartTime;
                                    obCustomer.CarInsLastEndTime = obBatchData.CarInsLastEndTime;
                                    obCustomer.ExpiryTime = obBatch.ExpiryTime;
                                    obCustomer.RecoveryTime = obBatch.RecoveryTime;
                                    obCustomer.FollowDelayDays = obBatch.FollowDelayDays;
                                    obCustomer.BelongerOrganizationId = obBatch.BelongerOrganizationId;
                                    obCustomer.BelongerId = obBatch.BelongerId;
                                    obCustomer.BusinessType = obBatch.BusinessType;
                                    obCustomer.Creator = obBatch.Creator;
                                    obCustomer.CreateTime = obBatch.CreateTime;
                                    CurrentDb.ObCustomer.Add(obCustomer);
                                    CurrentDb.SaveChanges();

                                    var obCustomerBelongTrack = new ObCustomerBelongTrack();
                                    obCustomerBelongTrack.Id = GuidUtil.New();
                                    obCustomerBelongTrack.MerchantId = obBatch.MerchantId;
                                    obCustomerBelongTrack.ObBatchId = obBatch.Id;
                                    obCustomerBelongTrack.ObBatchDataId = obBatchData.Id;
                                    obCustomerBelongTrack.ObCustomerId = obCustomer.Id;
                                    obCustomerBelongTrack.BelongerId = obBatch.BelongerId;
                                    obCustomerBelongTrack.Description = string.Format("分配给用户：{0}，姓名：{1}", belongUser.UserName, belongUser.FullName);
                                    obCustomerBelongTrack.Creator = obBatch.Creator;
                                    obCustomerBelongTrack.CreateTime = obBatch.CreateTime;
                                    CurrentDb.ObCustomerBelongTrack.Add(obCustomerBelongTrack);

                                }
                                #endregion
                            }

                            obBatch.DataCount = validCount + inValidCount;
                            obBatch.ValidCount = validCount;
                            obBatch.InValidCount = inValidCount;
                            obBatch.Status = Entity.Enumeration.DataBatchStatus.Complete;

                            //StringBuilder obBatchHandleReport = new StringBuilder();
                            //obBatchHandleReport.Append("导入数据总数有{0}条");
                            //obBatchHandleReport.Append("有效数据有{0}条");
                            //obBatchHandleReport.Append("无效数据{0}条");
                            //obBatchHandleReport.Append("导入数据总数为{0}条");
                            //obBatchHandleReport.Append("导入数据总数为{0}条");

                            //obBatch.HandleReport = obBatchHandleReport.ToString();

                            obBatch.Mender = GuidUtil.New();
                            obBatch.MendTime = DateTime.Now;

                            if (validCount > 0)
                            {
                                var obBatchAllocate = new ObBatchAllocate();
                                obBatchAllocate.Id = obBatchAllocateId;
                                obBatchAllocate.PId = GuidUtil.Empty();
                                obBatchAllocate.MerchantId = obBatch.MerchantId;
                                obBatchAllocate.ObBatchId = obBatch.Id;
                                obBatchAllocate.DataCount = validCount;
                                obBatchAllocate.AllocatedCount = 0;
                                obBatchAllocate.UnAllocatedCount = validCount;
                                obBatchAllocate.UsedCount = 0;
                                obBatchAllocate.Creator = obBatch.Creator;
                                obBatchAllocate.CreateTime = obBatch.CreateTime;
                                obBatchAllocate.BelongerId = obBatch.BelongerId;
                                obBatchAllocate.BelongerOrganizationId = obBatch.BelongerOrganizationId;
                                obBatchAllocate.SoureName = string.Format("数据文件:{0}", rop.SoureName);
                                CurrentDb.ObBatchAllocate.Add(obBatchAllocate);
                                CurrentDb.SaveChanges();
                            }
                        }

                        CurrentDb.SaveChanges();

                        ts.Complete();
                    }
                }
            }
        }

    }
}

