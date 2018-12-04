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
        public CustomJsonResult GetDetails(string operater, string merchantId, string id)
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


                var organizations = CurrentDb.Organization.Where(m => m.PId == obBatchAllocate.BelongOrganizationId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();
                if (organizations.Count > 0)
                {
                    foreach (var item in organizations)
                    {
                        var sysUser = CurrentDb.SysUser.Where(m => m.Id == item.HeaderId).FirstOrDefault();
                        if (sysUser != null)
                        {
                            ret.BelongUsers.Add(new RetObBatchAllocateTaskGetDetails.BelongUser { UserId = sysUser.Id, UserName = string.Format("{0}:{1}（{2}）", item.FullName, sysUser.FullName, sysUser.UserName), OrganizationId = item.Id });
                        }
                    }
                }
                else
                {

                    var sysMerchantUsers = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.OrganizationId == obBatchAllocate.BelongOrganizationId).ToList();

                    foreach (var sysMerchantUser in sysMerchantUsers)
                    {
                        ret.BelongUsers.Add(new RetObBatchAllocateTaskGetDetails.BelongUser { UserId = sysMerchantUser.Id, UserName = string.Format("{0}（{1}）", sysMerchantUser.FullName, sysMerchantUser.UserName), OrganizationId = sysMerchantUser.OrganizationId });
                    }

                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
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

                var soureUser = CurrentDb.SysUser.Where(m => m.Id == obBatchAllocate.BelongUserId).FirstOrDefault();

                var belongUsers = rop.BelongUsers.Where(m => m.AllocatedCount > 0).ToList();

                foreach (var item in belongUsers)
                {
                    var belongUser = CurrentDb.SysUser.Where(m => m.Id == item.UserId).FirstOrDefault();
                    var belongOrganization=CurrentDb.Organization.Where(m => m.Id== item.OrganizationId).FirstOrDefault();

                    var new_Allocate = new ObBatchAllocate();
                    new_Allocate.Id = GuidUtil.New();
                    new_Allocate.PId = obBatchAllocate.Id;
                    new_Allocate.MerchantId = obBatchAllocate.MerchantId;
                    new_Allocate.ObBatchId = obBatchAllocate.ObBatchId;
                    new_Allocate.DataCount = item.AllocatedCount;
                    new_Allocate.AllocatedCount = 0;
                    new_Allocate.UnAllocatedCount = item.AllocatedCount;
                    new_Allocate.UsedCount = 0;
                    new_Allocate.UnUsedCount = 0;
                    new_Allocate.BelongUserId = item.UserId;
                    new_Allocate.BelongUserName= string.Format("{0}机构：{1}({2})", belongOrganization.FullName, belongUser.FullName, belongUser.UserName);
                    new_Allocate.BelongOrganizationId = item.OrganizationId;
                    new_Allocate.Creator = operater;
                    new_Allocate.CreateTime = this.DateTime;
                    new_Allocate.SoureName = string.Format("上级分配人：{0}（{1}）", soureUser.FullName, soureUser.UserName);
                    CurrentDb.ObBatchAllocate.Add(new_Allocate);



                    List<ObCustomer> obCustomers = new List<ObCustomer>();


                    if (rop.Mode == Enumeration.ObBatchAllocateTaskAllocateMode.Random)
                    {
                        //随机分配
                        obCustomers = CurrentDb.ObCustomer.Where(x => x.BelongUserId == obBatchAllocate.BelongUserId).OrderBy(x => Guid.NewGuid()).Take(item.AllocatedCount).ToList();
                    }
                    else if (rop.Mode == Enumeration.ObBatchAllocateTaskAllocateMode.Filter)
                    {
                        //过滤分配
                        obCustomers = CurrentDb.ObCustomer.Where(x =>
                                     x.BelongUserId == obBatchAllocate.BelongUserId
                                     &&
                                     (rop.Filters.CarPlateNo == null || x.CarPlateNo.Contains(rop.Filters.CarPlateNo)) &&
                                     (rop.Filters.CarModel == null || x.CarModel.Contains(rop.Filters.CarModel)) &&
                                     (rop.Filters.CarEngineNo == null || x.CarEngineNo.Contains(rop.Filters.CarEngineNo)) &&
                                     (rop.Filters.CsrAddress == null || x.CsrAddress.Contains(rop.Filters.CsrAddress)) &&
                                     (rop.Filters.CsrPhoneNumber == null || x.CsrPhoneNumber.Contains(rop.Filters.CsrPhoneNumber)) &&
                                     (rop.Filters.CsrName == null || x.CsrName.Contains(rop.Filters.CsrName)) &&
                                     (rop.Filters.CsrIdNumber == null || x.CsrIdNumber.Contains(rop.Filters.CsrIdNumber)) &&
                                     (rop.Filters.CarInsLastCompany == null || x.CarInsLastCompany.Contains(rop.Filters.CarInsLastCompany))).OrderBy(x => Guid.NewGuid()).Take(item.AllocatedCount).ToList();
                    }

                    foreach (var obCustomer in obCustomers)
                    {
                        obCustomer.BelongUserId = item.UserId;
                        obCustomer.BelongOrganizationId = item.OrganizationId;
                        obCustomer.ObBatchAllocateId = new_Allocate.Id;
                        obCustomer.Mender = operater;
                        obCustomer.MendTime = this.DateTime;


                        var obCustomerBelongTrack = new ObCustomerBelongTrack();
                        obCustomerBelongTrack.Id = GuidUtil.New();
                        obCustomerBelongTrack.MerchantId = obCustomer.MerchantId;
                        obCustomerBelongTrack.ObBatchId = obCustomer.ObBatchId;
                        obCustomerBelongTrack.ObBatchDataId = obCustomer.ObBatchId;
                        obCustomerBelongTrack.ObCustomerId = obCustomer.Id;
                        obCustomerBelongTrack.BelongUserId = item.UserId;
                        obCustomerBelongTrack.Description = string.Format("分配给用户：{0}（{1}）", belongUser.FullName, belongUser.UserName);
                        obCustomerBelongTrack.Creator = operater;
                        obCustomerBelongTrack.CreateTime = this.DateTime;
                        CurrentDb.ObCustomerBelongTrack.Add(obCustomerBelongTrack);
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");
            }

            return result;
        }

    }
}
