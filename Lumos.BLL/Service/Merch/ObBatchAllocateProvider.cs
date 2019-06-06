using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{
    public class ObBatchAllocateProvider : BaseProvider
    {
        public CustomJsonResult GetDetails(string operater, string merchantId, string allocaterId, string id)
        {
            var ret = new RetObBatchAllocateTaskGetDetails();

            var obBatchAllocate = CurrentDb.ObBatchAllocate.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();

            if (obBatchAllocate != null)
            {
                var obBatch = CurrentDb.ObBatch.Where(m => m.Id == obBatchAllocate.ObBatchId).FirstOrDefault();

                ret.Id = obBatchAllocate.Id ?? ""; ;
                ret.ObBatchId = obBatch.Id;
                ret.ObBatchCode = obBatch.Code ?? ""; ;
                ret.ObBatchName = obBatch.Name ?? ""; ;
                ret.UnAllocatedCount = obBatchAllocate.UnAllocatedCount;


                var organizations = CurrentDb.Organization.Where(m => m.PId == obBatchAllocate.BelongerOrganizationId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();
                if (organizations.Count > 0)
                {
                    foreach (var item in organizations)
                    {
                        var sysUser = CurrentDb.SysUser.Where(m => m.Id == item.HeaderId && m.Id != allocaterId).FirstOrDefault();
                        if (sysUser != null)
                        {
                            ret.BelongUsers.Add(new RetObBatchAllocateTaskGetDetails.BelongUser { UserId = sysUser.Id, UserName = string.Format("{0}:{1}（{2}）", item.FullName, sysUser.FullName, sysUser.UserName), OrganizationId = item.Id });
                        }
                    }
                }
                else
                {

                    var sysMerchantUsers = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.OrganizationId == obBatchAllocate.BelongerOrganizationId && m.Id != allocaterId).ToList();

                    foreach (var sysMerchantUser in sysMerchantUsers)
                    {
                        ret.BelongUsers.Add(new RetObBatchAllocateTaskGetDetails.BelongUser { UserId = sysMerchantUser.Id, UserName = string.Format("{0}（{1}）", sysMerchantUser.FullName, sysMerchantUser.UserName), OrganizationId = sysMerchantUser.OrganizationId });
                    }

                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        private string GetFilters(RupObCustomerGetList filters)
        {
            StringBuilder sb = new StringBuilder();

            if (filters != null)
            {
                if (!string.IsNullOrEmpty(filters.CsrName))
                {
                    sb.Append("客户名称:" + filters.CsrName + ",");
                }
                if (!string.IsNullOrEmpty(filters.CsrPhoneNumber))
                {
                    sb.Append("电话号码:" + filters.CsrPhoneNumber + ",");
                }
                if (!string.IsNullOrEmpty(filters.CsrIdNumber))
                {
                    sb.Append("身份证号:" + filters.CsrIdNumber + ",");
                }

                if (!string.IsNullOrEmpty(filters.CsrAddress))
                {
                    sb.Append("地址:" + filters.CsrAddress + ",");
                }

                if (!string.IsNullOrEmpty(filters.CarInsLastCompany))
                {
                    sb.Append("保险公司:" + filters.CarInsLastCompany + ",");
                }
                if (!string.IsNullOrEmpty(filters.CarPlateNo))
                {
                    sb.Append("车牌号码:" + filters.CarPlateNo + ",");
                }
                if (!string.IsNullOrEmpty(filters.CarEngineNo))
                {
                    sb.Append("发动号:" + filters.CarEngineNo + ",");
                }

                if (!string.IsNullOrEmpty(filters.CarModel))
                {
                    sb.Append("厂牌:" + filters.CarModel + ",");
                }

                if (filters.CarRegisterDateStart != null)
                {
                    sb.Append("初登日期开始:" + filters.CarRegisterDateStart + ",");
                }
                if (filters.CarRegisterDateEnd != null)
                {
                    sb.Append("初登日期结束:" + filters.CarRegisterDateEnd + ",");
                }

                if (filters.CarInsLastStartTime != null)
                {
                    sb.Append("起保开始:" + filters.CarInsLastStartTime + ",");
                }

                if (filters.CarInsLastEndTime != null)
                {
                    sb.Append("起保结束:" + filters.CarInsLastEndTime + ",");
                }
            }


            return sb.ToString();
        }

        public CustomJsonResult Add(string operater, string merchantId, RopObBatchAllocateAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var obBatchAllocate = CurrentDb.ObBatchAllocate.Where(m => m.Id == rop.Id).FirstOrDefault();
                if (obBatchAllocate == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "此任务ID未找到");
                }

                if (rop.BelongUsers == null || rop.BelongUsers.Count <= 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "分配的对象为空");
                }

                int allocatedCount = rop.BelongUsers.Sum(m => m.AllocatedCount);

                if (allocatedCount == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请输入分配的数量");
                }

                if (allocatedCount > obBatchAllocate.UnAllocatedCount)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "分配的总数量大于批次的数量");
                }

                obBatchAllocate.AllocatedCount += allocatedCount;
                obBatchAllocate.UnAllocatedCount -= allocatedCount;
                obBatchAllocate.Mender = operater;
                obBatchAllocate.MendTime = this.DateTime;

                var soureUser = CurrentDb.SysUser.Where(m => m.Id == obBatchAllocate.BelongerId).FirstOrDefault();

                var belongUsers = rop.BelongUsers.Where(m => m.AllocatedCount > 0).ToList();

                foreach (var item in belongUsers)
                {
                    var belongUser = CurrentDb.SysUser.Where(m => m.Id == item.UserId).FirstOrDefault();
                    var belongOrganization = CurrentDb.Organization.Where(m => m.Id == item.OrganizationId).FirstOrDefault();

                    var new_Allocate = new ObBatchAllocate();
                    new_Allocate.Id = GuidUtil.New();
                    new_Allocate.PId = obBatchAllocate.Id;
                    new_Allocate.MerchantId = obBatchAllocate.MerchantId;
                    new_Allocate.ObBatchId = obBatchAllocate.ObBatchId;
                    new_Allocate.DataCount = item.AllocatedCount;
                    new_Allocate.AllocatedCount = 0;
                    new_Allocate.UnAllocatedCount = item.AllocatedCount;
                    new_Allocate.UsedCount = 0;
                    new_Allocate.BelongerId = item.UserId;
                    new_Allocate.BelongerName = string.Format("{0}机构：{1}({2})", belongOrganization.FullName, belongUser.FullName, belongUser.UserName);
                    new_Allocate.BelongerOrganizationId = item.OrganizationId;
                    new_Allocate.AllocaterId = soureUser.Id;
                    new_Allocate.Creator = operater;
                    new_Allocate.CreateTime = this.DateTime;
                    new_Allocate.SoureName = string.Format("上级分配人：{0}（{1}）", soureUser.FullName, soureUser.UserName);
                    new_Allocate.AllocateMode = rop.Mode;
                    new_Allocate.Description = rop.Description;

                    List<ObCustomer> obCustomers = new List<ObCustomer>();


                    if (rop.Mode == Enumeration.ObBatchAllocateMode.Random)
                    {
                        //随机分配
                        obCustomers = CurrentDb.ObCustomer.Where(x => x.ObBatchId == obBatchAllocate.ObBatchId && x.BelongerId == obBatchAllocate.BelongerId).OrderBy(x => Guid.NewGuid()).Take(item.AllocatedCount).ToList();
                    }
                    else if (rop.Mode == Enumeration.ObBatchAllocateMode.Filter)
                    {
                        new_Allocate.Filters = GetFilters(rop.Filters);
                        //过滤分配
                        obCustomers = CurrentDb.ObCustomer.Where(x =>
                                     x.ObBatchId == obBatchAllocate.ObBatchId &&
                                     x.BelongerId == obBatchAllocate.BelongerId
                                     &&
                                     (rop.Filters.CarPlateNo == null || x.CarPlateNo.Contains(rop.Filters.CarPlateNo)) &&
                                     (rop.Filters.CarModel == null || x.CarModel.Contains(rop.Filters.CarModel)) &&
                                     (rop.Filters.CarEngineNo == null || x.CarEngineNo.Contains(rop.Filters.CarEngineNo)) &&
                                     (rop.Filters.CsrAddress == null || x.CsrAddress.Contains(rop.Filters.CsrAddress)) &&
                                     (rop.Filters.CsrPhoneNumber == null || x.CsrPhoneNumber.Contains(rop.Filters.CsrPhoneNumber)) &&
                                     (rop.Filters.CsrName == null || x.CsrName.Contains(rop.Filters.CsrName)) &&
                                     (rop.Filters.CsrIdNumber == null || x.CsrIdNumber.Contains(rop.Filters.CsrIdNumber)) &&
                                     (rop.Filters.CarInsLastCompany == null || x.CarInsLastCompany.Contains(rop.Filters.CarInsLastCompany)) &&
                                     (rop.Filters.CarRegisterDateStart == null || x.CarRegisterDate >= rop.Filters.CarRegisterDateStart) &&
                                     (rop.Filters.CarRegisterDateEnd == null || x.CarRegisterDate <= rop.Filters.CarRegisterDateEnd) &&
                                     (rop.Filters.CarInsLastStartTime == null || x.CarInsLastStartTime >= rop.Filters.CarInsLastStartTime) &&
                                     (rop.Filters.CarInsLastEndTime == null || x.CarInsLastEndTime <= rop.Filters.CarInsLastEndTime)
                                     ).OrderBy(x => Guid.NewGuid()).Take(item.AllocatedCount).ToList();
                    }

                    if (obCustomers.Count != item.AllocatedCount)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "分配的总数量与库存数据不一致");
                    }

                    foreach (var obCustomer in obCustomers)
                    {
                        obCustomer.BelongerId = item.UserId;
                        obCustomer.BelongerOrganizationId = item.OrganizationId;
                        obCustomer.ObBatchAllocateId = new_Allocate.Id;
                        obCustomer.Mender = operater;
                        obCustomer.MendTime = this.DateTime;
                        CurrentDb.SaveChanges();

                        var obCustomerBelongTrack = new ObCustomerBelongTrack();
                        obCustomerBelongTrack.Id = GuidUtil.New();
                        obCustomerBelongTrack.MerchantId = obCustomer.MerchantId;
                        obCustomerBelongTrack.ObBatchId = obCustomer.ObBatchId;
                        obCustomerBelongTrack.ObBatchDataId = obCustomer.ObBatchId;
                        obCustomerBelongTrack.ObCustomerId = obCustomer.Id;
                        obCustomerBelongTrack.BelongerId = item.UserId;
                        obCustomerBelongTrack.Description = string.Format("分配给用户：{0}（{1}）", belongUser.FullName, belongUser.UserName);
                        obCustomerBelongTrack.Creator = operater;
                        obCustomerBelongTrack.CreateTime = this.DateTime;
                        CurrentDb.ObCustomerBelongTrack.Add(obCustomerBelongTrack);
                    }

                    CurrentDb.ObBatchAllocate.Add(new_Allocate);
                }


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");
            }

            return result;
        }



        public void a(string operater, string obBatchAllocateId, int count)
        {
            var obBatchAllocate = CurrentDb.ObBatchAllocate.Where(m => m.Id == obBatchAllocateId).FirstOrDefault();

            var pObBatchAllocates = CurrentDb.ObBatchAllocate.Where(m => m.PId == obBatchAllocateId).ToList();

            if (pObBatchAllocates.Count > 0)
            {
                foreach (var item in pObBatchAllocates)
                {
                    int unAllocatedCount = item.UnAllocatedCount;

                    if (unAllocatedCount > 0)
                    {

                        item.RestoreCount = unAllocatedCount;
                        item.UnAllocatedCount = unAllocatedCount;

                        var obCustomers = CurrentDb.ObCustomer.Where(m => m.ObBatchAllocateId == item.Id && m.BelongerId == item.BelongerId && m.IsUseCall == false && m.IsTake == false).Take(unAllocatedCount).ToList();
                        if (obCustomers.Count > 0)
                        {

                            var belongUser = CurrentDb.SysMerchantUser.Where(m => m.Id == obBatchAllocate.AllocaterId).FirstOrDefault();
                            var belongOrganization = CurrentDb.Organization.Where(m => m.Id == belongUser.OrganizationId).FirstOrDefault();

                            foreach (var obCustomer in obCustomers)
                            {
                                obCustomer.BelongerId = belongUser.Id;
                                obCustomer.BelongerOrganizationId = belongUser.OrganizationId;
                                obCustomer.ObBatchAllocateId = item.Id;
                                obCustomer.Mender = operater;
                                obCustomer.MendTime = this.DateTime;
                                CurrentDb.SaveChanges();

                                var obCustomerBelongTrack = new ObCustomerBelongTrack();
                                obCustomerBelongTrack.Id = GuidUtil.New();
                                obCustomerBelongTrack.MerchantId = obCustomer.MerchantId;
                                obCustomerBelongTrack.ObBatchId = obCustomer.ObBatchId;
                                obCustomerBelongTrack.ObBatchDataId = obCustomer.ObBatchId;
                                obCustomerBelongTrack.ObCustomerId = obCustomer.Id;
                                obCustomerBelongTrack.BelongerId = belongUser.Id;
                                obCustomerBelongTrack.Description = string.Format("还原给用户：{0}（{1}）", belongUser.FullName, belongUser.UserName);
                                obCustomerBelongTrack.Creator = operater;
                                obCustomerBelongTrack.CreateTime = this.DateTime;
                                CurrentDb.ObCustomerBelongTrack.Add(obCustomerBelongTrack);
                            }
                            CurrentDb.SaveChanges();
                        }
                    }

                    a(operater, item.Id, count);
                }
            }
            else
            {
                var obCustomers = CurrentDb.ObCustomer.Where(m => m.ObBatchAllocateId == obBatchAllocate.Id && m.BelongerId == obBatchAllocate.BelongerId && m.IsUseCall == false && m.IsTake == false).Take(count).ToList();
                if (obCustomers.Count > 0)
                {

                    var belongUser = CurrentDb.SysMerchantUser.Where(m => m.Id == obBatchAllocate.AllocaterId).FirstOrDefault();
                    var belongOrganization = CurrentDb.Organization.Where(m => m.Id == belongUser.OrganizationId).FirstOrDefault();

                    foreach (var obCustomer in obCustomers)
                    {
                        obCustomer.BelongerId = belongUser.Id;
                        obCustomer.BelongerOrganizationId = belongUser.OrganizationId;
                        obCustomer.ObBatchAllocateId = obBatchAllocate.Id;
                        obCustomer.Mender = operater;
                        obCustomer.MendTime = this.DateTime;
                        CurrentDb.SaveChanges();

                        var obCustomerBelongTrack = new ObCustomerBelongTrack();
                        obCustomerBelongTrack.Id = GuidUtil.New();
                        obCustomerBelongTrack.MerchantId = obCustomer.MerchantId;
                        obCustomerBelongTrack.ObBatchId = obCustomer.ObBatchId;
                        obCustomerBelongTrack.ObBatchDataId = obCustomer.ObBatchId;
                        obCustomerBelongTrack.ObCustomerId = obCustomer.Id;
                        obCustomerBelongTrack.BelongerId = belongUser.Id;
                        obCustomerBelongTrack.Description = string.Format("还原给用户：{0}（{1}）", belongUser.FullName, belongUser.UserName);
                        obCustomerBelongTrack.Creator = operater;
                        obCustomerBelongTrack.CreateTime = this.DateTime;
                        CurrentDb.ObCustomerBelongTrack.Add(obCustomerBelongTrack);
                    }
                    CurrentDb.SaveChanges();
                }
            }

        }
        public CustomJsonResult Restore(string operater, string merchantId, string obBatchAllocateId)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                var obBatchAllocate = CurrentDb.ObBatchAllocate.Where(m => m.Id == obBatchAllocateId).FirstOrDefault();

                int unAllocatedCount = obBatchAllocate.UnAllocatedCount - obBatchAllocate.RestoreCount;

                var pObBatchAllocate = CurrentDb.ObBatchAllocate.Where(m => m.Id == obBatchAllocate.PId).FirstOrDefault();


                if (unAllocatedCount > 0)
                {
                    a(operater, obBatchAllocateId, unAllocatedCount);

                    obBatchAllocate.RestoreCount = unAllocatedCount;

                    pObBatchAllocate.UnAllocatedCount += unAllocatedCount;
                    pObBatchAllocate.AllocatedCount -= unAllocatedCount;

                    CurrentDb.SaveChanges();
                    ts.Complete();

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "还原成功");
                }
                else
                {
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "没有可还原的数据");
                }




            }

            return result;
        }
    }
}
