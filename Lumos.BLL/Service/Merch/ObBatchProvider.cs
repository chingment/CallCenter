using Lumos.BLL.Biz;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{
    public class ObBatchProvider : BaseProvider
    {
        public CustomJsonResult GetDetails(string operater, string merchantId, string id)
        {
            var ret = new RetObBatchGetDetails();

            var obBatch = CurrentDb.ObBatch.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();

            if (obBatch != null)
            {
                ret.Id = obBatch.Id ?? ""; ;
                ret.Code = obBatch.Code ?? ""; ;
                ret.Name = obBatch.Name ?? ""; ;
                ret.SoureName = obBatch.SoureName ?? "";
                ret.ValidCount = obBatch.ValidCount;
                ret.InValidCount = obBatch.InValidCount;
                ret.ExpiryTime = obBatch.ExpiryTime.ToUnifiedFormatDate() ?? "";
                ret.RecoveryTime = obBatch.RecoveryTime.ToUnifiedFormatDate() ?? "";
                ret.HandleReport = obBatch.HandleReport ?? "";
                ret.FollowDelayDays = obBatch.FollowDelayDays;
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult AddByFile(string operater, string merchantId, RopObBatchAddByFile rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var isExistCode = CurrentDb.ObBatch.Where(m => m.Code == rop.Code).FirstOrDefault();
            if (isExistCode != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该批次号（{0}）已被使用", rop.Code));
            }


            var obBatch = new ObBatch();
            obBatch.Id = GuidUtil.New();
            obBatch.MerchantId = merchantId;
            obBatch.Code = rop.Code;
            obBatch.Name = rop.Name;
            obBatch.BizType = rop.BizType;
            obBatch.SoureType = Enumeration.DataBatchSoureType.File;
            obBatch.ExpiryTime = this.DateTime.AddDays(rop.ExpiryDays);
            obBatch.FollowDelayDays = rop.FollowDelayDays;
            obBatch.RecoveryTime = obBatch.ExpiryTime.AddDays(rop.ExpiryDays);
            obBatch.SoureName = rop.FileName;
            obBatch.FilePath = rop.FilePath;
            obBatch.BelongId = rop.BelongId;
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
