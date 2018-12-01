using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{
    public class DataBatchProvider : BaseProvider
    {
        public CustomJsonResult AddByFile(string operater, string merchantId, RopDataBatchAddByFile rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var isExistCode = CurrentDb.DataBatch.Where(m => m.Code == rop.Code).FirstOrDefault();
            if (isExistCode != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该批次号（{0}）已被使用", rop.Code));
            }


            var bataBatch = new DataBatch();
            bataBatch.Id = GuidUtil.New();
            bataBatch.MerchantId = merchantId;
            bataBatch.Code = rop.Code;
            bataBatch.Name = rop.Name;
            bataBatch.BizType = rop.BizType;
            bataBatch.SoureType = Enumeration.DataBatchSoureType.File;
            bataBatch.SoureName = rop.FileName;
            bataBatch.FilePath = rop.FilePath;
            bataBatch.Status = Enumeration.DataBatchStatus.WaitHandle;
            bataBatch.Creator = operater;
            bataBatch.CreateTime = this.DateTime;

            CurrentDb.DataBatch.Add(bataBatch);
            CurrentDb.SaveChanges();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

            return result;
        }
    }
}
