using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{
    public class ObBatchAllocateTaskProvider : BaseProvider
    {
        public CustomJsonResult GetDetails(string operater, string merchantId, string id)
        {
            var ret = new RetObBatchAllocateTaskGetDetails();

            var obBatchDataAllocate = CurrentDb.ObBatchAllocateTask.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();

            if (obBatchDataAllocate != null)
            {
                var obBatch = CurrentDb.ObBatch.Where(m => m.Id == obBatchDataAllocate.ObBatchId).FirstOrDefault();

                ret.Id = obBatchDataAllocate.Id ?? ""; ;
                ret.ObBatchId = obBatch.Id;
                ret.ObBatchCode = obBatch.Code ?? ""; ;
                ret.ObBatchName = obBatch.Name ?? ""; ;
                ret.UnAllocatedCount = obBatchDataAllocate.UnAllocatedCount;


                var organizations = CurrentDb.Organization.Where(m => m.PId == obBatchDataAllocate.BelongOrganizationId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

                foreach (var item in organizations)
                {
                    var sysUser = CurrentDb.SysUser.Where(m => m.Id == item.HeaderId).FirstOrDefault();
                    if (sysUser != null)
                    {
                        ret.BelongUsers.Add(new RetObBatchAllocateTaskGetDetails.BelongUser { UserId = sysUser.Id, UserName = string.Format("{0}:{1}（{2}）", item.FullName, sysUser.UserName, sysUser.FullName), OrganizationId = item.Id });
                    }
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }


        public CustomJsonResult Allocate(string operater, string merchantId, RopObBatchAllocateTaskAllocate rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {
                var obBatchAllocateTask = CurrentDb.ObBatchAllocateTask.Where(m => m.Id == rop.Id).FirstOrDefault();
                if (obBatchAllocateTask == null)
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

                if (allocatedCount > obBatchAllocateTask.UnAllocatedCount)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "分配的总数量大于批次的数量");
                }


                obBatchAllocateTask.UnAllocatedCount -= allocatedCount;
                obBatchAllocateTask.Mender = operater;
                obBatchAllocateTask.MendTime = this.DateTime;

                foreach (var item in rop.BelongUsers)
                {
                    var new_task = new ObBatchAllocateTask();
                    new_task.Id = GuidUtil.New();
                    new_task.PId = obBatchAllocateTask.Id;
                    new_task.MerchantId = obBatchAllocateTask.MerchantId;
                    new_task.ObBatchId = obBatchAllocateTask.ObBatchId;
                    new_task.DataCount = allocatedCount;
                    new_task.AllocatedCount = 0;
                    new_task.UnAllocatedCount = item.AllocatedCount;
                    new_task.UsedCount = 0;
                    new_task.UnUsedCount = 0;
                    new_task.BelongUserId = item.UserId;
                    new_task.BelongOrganizationId = item.OrganizationId;
                    new_task.Creator = operater;
                    new_task.CreateTime = this.DateTime;
                    CurrentDb.ObBatchAllocateTask.Add(new_task);


                    var obCustomers = CurrentDb.ObCustomer.Where(x => x.BelongUserId == obBatchAllocateTask.BelongUserId).OrderBy(x => Guid.NewGuid()).Take(item.AllocatedCount).ToList();

                    foreach (var obCustomer in obCustomers)
                    {
                        obCustomer.BelongUserId = item.UserId;
                        obCustomer.BelongOrganizationId = item.OrganizationId;
                        obCustomer.ObBatchAllocateTaskId = new_task.Id;
                        obCustomer.Mender = operater;
                        obCustomer.MendTime = this.DateTime;
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
