using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        ret.BelongUsers.Add(new RetObBatchAllocateTaskGetDetails.BelongUser { Id = sysUser.Id, Name = string.Format("{0}:{1}（{2}）", item.FullName, sysUser.UserName, sysUser.FullName), OrganizationId = item.Id });
                    }
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }
    }
}
