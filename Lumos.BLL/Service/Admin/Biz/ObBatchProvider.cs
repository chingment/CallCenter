using Lumos.BLL.Biz;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Admin
{
    public class ObBatchProvider : BaseProvider
    {

        public CustomJsonResult GetDetails(string operater, string id)
        {
            var ret = new RetObBatchGetDetails();

            var obBatch = CurrentDb.ObBatch.Where(m => m.Id == id).FirstOrDefault();

            if (obBatch != null)
            {
                ret.Id = obBatch.Id ?? ""; ;
                ret.Code = obBatch.Code ?? ""; ;
                ret.Name = obBatch.Name ?? ""; ;
                ret.SoureName = string.Format("（{0}）{1}", obBatch.SoureType.GetCnName(), obBatch.SoureName);
                ret.DataCount = obBatch.DataCount;
                ret.ValidCount = obBatch.ValidCount;
                ret.InValidCount = obBatch.InValidCount;
                ret.ExpiryTime = obBatch.ExpiryTime.ToUnifiedFormatDate() ?? "";
                ret.RecoveryTime = obBatch.RecoveryTime.ToUnifiedFormatDate() ?? "";
                ret.HandleReport = obBatch.HandleReport ?? "";
                ret.FollowDelayDays = obBatch.FollowDelayDays;
                ret.BelongerName = obBatch.BelongerName;
                ret.BusinessType = obBatch.BusinessType;
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult AddByFile(string operater, RopObBatchAddByFile rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var isExistCode = CurrentDb.ObBatch.Where(m => m.MerchantId == rop.MerchantId && m.Code == rop.Code).FirstOrDefault();
            if (isExistCode != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该批次号（{0}）已被使用", rop.Code));
            }

            var merchant = CurrentDb.Merchant.Where(m => m.Id == rop.MerchantId).FirstOrDefault();


            var obBatch = new ObBatch();
            obBatch.Id = GuidUtil.New();
            obBatch.MerchantId = rop.MerchantId;
            obBatch.Code = rop.Code;
            obBatch.Name = rop.Name;
            obBatch.BusinessType = merchant.BusinessType;
            obBatch.ImportFileTmpl = rop.ImportFileTmpl;
            obBatch.SoureType = Enumeration.DataBatchSoureType.File;
            obBatch.ExpiryDays = rop.ExpiryDays;
            obBatch.ExpiryTime = this.DateTime.AddDays(rop.ExpiryDays);
            obBatch.FollowDelayDays = rop.FollowDelayDays;
            obBatch.RecoveryDays = rop.ExpiryDays;
            obBatch.RecoveryTime = obBatch.ExpiryTime.AddDays(rop.ExpiryDays);
            obBatch.SoureName = rop.FileName;
            obBatch.FilePath = rop.FilePath;
            obBatch.BelongerId = rop.BelongerId;
            obBatch.BelongerName = rop.BelongerName;
            obBatch.BelongerOrganizationId = rop.BelongerOrganizationId;
            obBatch.Status = Enumeration.DataBatchStatus.WaitHandle;
            obBatch.Creator = operater;
            obBatch.CreateTime = this.DateTime;

            CurrentDb.ObBatch.Add(obBatch);
            CurrentDb.SaveChanges();

            RedisMqFactory.Global.Push(RedisMqHandleType.ObBatch, obBatch);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

            return result;
        }

    }
}
