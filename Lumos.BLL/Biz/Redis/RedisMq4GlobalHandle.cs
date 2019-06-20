using Lumos.Common;
using Lumos.DAL;
using Lumos.Entity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
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
            var CurrentDb = new LumosDbContext();

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

            var belongUser = CurrentDb.SysUser.Where(m => m.Id == obBatch.BelongerId).FirstOrDefault();

            try
            {
                TransactionOptions transactionOption = new TransactionOptions();
                transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                transactionOption.Timeout = new TimeSpan(0, 10, 0);

                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, transactionOption))
                {
                    var tsCurrentDb = new LumosDbContext();

                    var obBatchAllocateId = GuidUtil.New();

                    var tsObBatch = tsCurrentDb.ObBatch.Where(m => m.Id == rop.Id).FirstOrDefault();

                    List<ObBatchData> obBatchDatas = new List<ObBatchData>();
                    List<ObCustomer> obCustomers = new List<ObCustomer>();
                    List<ObCustomerBelongTrack> obCustomerBelongTracks = new List<ObCustomerBelongTrack>();
                    List<ObBatchAllocate> obBatchAllocates = new List<Entity.ObBatchAllocate>();

                    if (tsObBatch.SoureType == Entity.Enumeration.DataBatchSoureType.File)
                    {
                        if (!string.IsNullOrEmpty(tsObBatch.FilePath))
                        {
                            if (File.Exists(tsObBatch.FilePath))
                            {

                                FileStream fsRead = new FileStream(tsObBatch.FilePath, FileMode.Open);
                                HSSFWorkbook workbook = new HSSFWorkbook(fsRead);
                                ISheet sheet = workbook.GetSheetAt(0);
                                int rowCount = sheet.LastRowNum + 1;

                                int validCount = 0;
                                int inValidCount = 0;
                                for (int i = 1; i < rowCount; i++)
                                {
                                    #region 数据去重处理

                                    IRow row = sheet.GetRow(i);

                                    string csrPhoneNumber = null;
                                    string csrName = null;
                                    string csrAddress = null;
                                    string csrIdNumber = null;
                                    string csrCompany = null;
                                    DateTime? carRegisterDate = null;
                                    string carPlateNo = null;
                                    string carModel = null;
                                    string carEngineNo = null;
                                    string carVin = null;
                                    string carInsLastQzNo = null;
                                    string carInsLastSyNo = null;
                                    string carInsLastCompany = null;
                                    DateTime? carInsLastStartTime = null;
                                    DateTime? carInsLastEndTime = null;


                                    if (tsObBatch.ImportFileTmpl == "1")
                                    {
                                        csrPhoneNumber = NPOIHelperUtil.GetCellValue(row.GetCell(1));
                                        csrName = NPOIHelperUtil.GetCellValue(row.GetCell(0));
                                        csrAddress = NPOIHelperUtil.GetCellValue(row.GetCell(2));
                                        csrCompany = NPOIHelperUtil.GetCellValue(row.GetCell(3));
                                    }
                                    else if (tsObBatch.ImportFileTmpl == "2")
                                    {
                                        csrPhoneNumber = NPOIHelperUtil.GetCellValue(row.GetCell(8));
                                        csrName = NPOIHelperUtil.GetCellValue(row.GetCell(6));
                                        csrAddress = NPOIHelperUtil.GetCellValue(row.GetCell(7));
                                        csrIdNumber = NPOIHelperUtil.GetCellValue(row.GetCell(3));
                                        carRegisterDate = CommonUtil.ConverToStartTime(NPOIHelperUtil.GetCellValue(row.GetCell(0)));
                                        carPlateNo = NPOIHelperUtil.GetCellValue(row.GetCell(1));
                                        carModel = NPOIHelperUtil.GetCellValue(row.GetCell(2));
                                        carEngineNo = NPOIHelperUtil.GetCellValue(row.GetCell(5));
                                        carVin = NPOIHelperUtil.GetCellValue(row.GetCell(4));
                                        carInsLastQzNo = NPOIHelperUtil.GetCellValue(row.GetCell(9));
                                        carInsLastSyNo = NPOIHelperUtil.GetCellValue(row.GetCell(9));
                                        carInsLastCompany = NPOIHelperUtil.GetCellValue(row.GetCell(12));
                                        carInsLastStartTime = CommonUtil.ConverToStartTime(NPOIHelperUtil.GetCellValue(row.GetCell(10)));
                                        carInsLastEndTime = CommonUtil.ConverToEndTime(NPOIHelperUtil.GetCellValue(row.GetCell(11)));
                                    }

                                    if (!string.IsNullOrEmpty(csrPhoneNumber))
                                    {
                                        //ObCustomer obCustomer = null;
                                        var obCustomer = tsCurrentDb.ObCustomer.Where(m => m.MerchantId == obBatch.MerchantId && m.CsrPhoneNumber == csrPhoneNumber).FirstOrDefault();

                                        bool isValid = true;
                                        string handleReport = "";
                                        if (obCustomer == null)
                                        {
                                            var obCustomer1 = obBatchDatas.Where(m => m.CsrPhoneNumber == csrPhoneNumber).FirstOrDefault();
                                            if (obCustomer1 == null)
                                            {
                                                handleReport = "有效分配数据：未重复";
                                                isValid = true;
                                                validCount += 1;
                                            }
                                            else
                                            {
                                                handleReport = "无效分配数据:与本批次重复";
                                                isValid = false;
                                                inValidCount += 1;
                                            }
                                        }
                                        else
                                        {
                                            if (obCustomer.RecoveryTime >= DateTime.Now)
                                            {
                                                if (obCustomer.ObBatchId == tsObBatch.Id)
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
                                        obBatchData.MerchantId = tsObBatch.MerchantId;
                                        obBatchData.ObBatchId = tsObBatch.Id;
                                        obBatchData.CsrName = csrName;
                                        obBatchData.CsrPhoneNumber = csrPhoneNumber;
                                        obBatchData.CsrAddress = csrAddress;
                                        obBatchData.CsrIdNumber = csrIdNumber;
                                        obBatchData.CsrCompany = csrCompany;
                                        obBatchData.CarRegisterDate = carRegisterDate;
                                        obBatchData.CarPlateNo = carPlateNo;
                                        obBatchData.CarModel = carModel;
                                        obBatchData.CarEngineNo = carEngineNo;
                                        obBatchData.CarVin = carVin;
                                        obBatchData.CarInsLastQzNo = carInsLastQzNo;
                                        obBatchData.CarInsLastSyNo = carInsLastSyNo;
                                        obBatchData.CarInsLastCompany = carInsLastCompany;
                                        obBatchData.CarInsLastStartTime = carInsLastStartTime;
                                        obBatchData.CarInsLastEndTime = carInsLastEndTime;
                                        obBatchData.BusinessType = tsObBatch.BusinessType;
                                        obBatchData.IsValid = isValid;
                                        obBatchData.HandleReport = handleReport;
                                        obBatchData.Creator = tsObBatch.Creator;
                                        obBatchData.CreateTime = tsObBatch.CreateTime;
                                        

                                        obBatchDatas.Add(obBatchData);

                                        if (isValid)
                                        {
                                            obCustomer = new ObCustomer();
                                            obCustomer.Id = GuidUtil.New();
                                            obCustomer.MerchantId = tsObBatch.MerchantId;
                                            obCustomer.ObBatchId = tsObBatch.Id;
                                            obCustomer.ObBatchDataId = obBatchData.Id;
                                            obCustomer.ObBatchAllocateId = obBatchAllocateId;
                                            obCustomer.CsrName = obBatchData.CsrName;
                                            obCustomer.CsrPhoneNumber = csrPhoneNumber;
                                            obCustomer.CsrAddress = obBatchData.CsrAddress;
                                            obCustomer.CsrIdNumber = obBatchData.CsrIdNumber;
                                            obCustomer.CsrCompany = obBatchData.CsrCompany;
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
                                            obCustomer.ExpiryTime = tsObBatch.ExpiryTime;
                                            obCustomer.RecoveryTime = tsObBatch.RecoveryTime;
                                            obCustomer.FollowDelayDays = tsObBatch.FollowDelayDays;
                                            obCustomer.BelongerOrganizationId = tsObBatch.BelongerOrganizationId;
                                            obCustomer.BelongerId = tsObBatch.BelongerId;
                                            obCustomer.BusinessType = tsObBatch.BusinessType;
                                            obCustomer.Creator = tsObBatch.Creator;
                                            obCustomer.CreateTime = tsObBatch.CreateTime;
                                            //tsCurrentDb.ObCustomer.Add(obCustomer);

                                            obCustomers.Add(obCustomer);

                                            var obCustomerBelongTrack = new ObCustomerBelongTrack();
                                            obCustomerBelongTrack.Id = GuidUtil.New();
                                            obCustomerBelongTrack.MerchantId = tsObBatch.MerchantId;
                                            obCustomerBelongTrack.ObBatchId = tsObBatch.Id;
                                            obCustomerBelongTrack.ObBatchDataId = obBatchData.Id;
                                            obCustomerBelongTrack.ObCustomerId = obCustomer.Id;
                                            obCustomerBelongTrack.BelongerId = tsObBatch.BelongerId;
                                            obCustomerBelongTrack.Description = string.Format("分配给用户：{0}，姓名：{1}", belongUser.UserName, belongUser.FullName);
                                            obCustomerBelongTrack.Creator = tsObBatch.Creator;
                                            obCustomerBelongTrack.CreateTime = tsObBatch.CreateTime;
                                            //tsCurrentDb.ObCustomerBelongTrack.Add(obCustomerBelongTrack);

                                            obCustomerBelongTracks.Add(obCustomerBelongTrack);
                                        }

                                    }

                                    #endregion
                                }

                                tsObBatch.DataCount = validCount + inValidCount;
                                tsObBatch.ValidCount = validCount;
                                tsObBatch.InValidCount = inValidCount;
                                tsObBatch.Status = Entity.Enumeration.DataBatchStatus.Complete;

                                //StringBuilder obBatchHandleReport = new StringBuilder();
                                //obBatchHandleReport.Append("导入数据总数有{0}条");
                                //obBatchHandleReport.Append("有效数据有{0}条");
                                //obBatchHandleReport.Append("无效数据{0}条");
                                //obBatchHandleReport.Append("导入数据总数为{0}条");
                                //obBatchHandleReport.Append("导入数据总数为{0}条");

                                //obBatch.HandleReport = obBatchHandleReport.ToString();

                                tsObBatch.Mender = GuidUtil.New();
                                tsObBatch.MendTime = DateTime.Now;

                                if (validCount > 0)
                                {
                                    var obBatchAllocate = new ObBatchAllocate();
                                    obBatchAllocate.Id = obBatchAllocateId;
                                    obBatchAllocate.PId = GuidUtil.Empty();
                                    obBatchAllocate.MerchantId = tsObBatch.MerchantId;
                                    obBatchAllocate.ObBatchId = tsObBatch.Id;
                                    obBatchAllocate.DataCount = validCount;
                                    obBatchAllocate.AllocatedCount = 0;
                                    obBatchAllocate.UnAllocatedCount = validCount;
                                    obBatchAllocate.UsedCount = 0;
                                    obBatchAllocate.Creator = tsObBatch.Creator;
                                    obBatchAllocate.CreateTime = tsObBatch.CreateTime;
                                    obBatchAllocate.BelongerId = tsObBatch.BelongerId;
                                    obBatchAllocate.BelongerOrganizationId = tsObBatch.BelongerOrganizationId;
                                    obBatchAllocate.SoureName = string.Format("数据文件:{0}", rop.SoureName);
                                    //tsCurrentDb.ObBatchAllocate.Add(obBatchAllocate);

                                    obBatchAllocates.Add(obBatchAllocate);
                                }
                            }

                            tsCurrentDb.BulkInsert(obBatchDatas);
                            tsCurrentDb.BulkInsert(obCustomers);
                            tsCurrentDb.BulkInsert(obCustomerBelongTracks);
                            tsCurrentDb.BulkInsert(obBatchAllocates);

                            tsCurrentDb.BulkSaveChanges();
                            //tsCurrentDb.SaveChanges();
                            ts.Complete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("消息队列处理Excel文件异常", ex);
                obBatch.Status = Entity.Enumeration.DataBatchStatus.Failure;
                obBatch.Description = "处理Excel文件异常";
                obBatch.Mender = GuidUtil.New();
                obBatch.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
            }
        }

    }
}

