using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{
    public class ObTakeDataLimitProvider : BaseProvider
    {
        public CustomJsonResult GetDetails(string operater, string merchantId, string id)
        {
            var ret = new RetObTakeDataLimitGetDetails();

            var obTakeDataLimit = CurrentDb.ObTakeDataLimit.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();
            if (obTakeDataLimit != null)
            {
                ret.TaskQuantity = obTakeDataLimit.TaskQuantity;
                ret.UnTakeQuantity = obTakeDataLimit.UnTakeQuantity;
                ret.TakedQuantity = obTakeDataLimit.TakedQuantity;


                var salesman = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.Id == obTakeDataLimit.SalesmanId).FirstOrDefault();

                ret.UserName = salesman.UserName;
                ret.FullName = salesman.FullName;
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult Adjust(string operater, string merchantId, RopObTakeDataLimitAdjust rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var obTakeDataLimit = CurrentDb.ObTakeDataLimit.Where(m => m.MerchantId == merchantId && m.Id == rop.Id).FirstOrDefault();

                if (rop.AdjustQuantity == obTakeDataLimit.TaskQuantity)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，数量和之前一样");
                }

                if (rop.AdjustQuantity < obTakeDataLimit.TakedQuantity)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "保存失败，任务量不能小于已取量");
                }


                int quantity = 0;
                if (rop.AdjustQuantity > obTakeDataLimit.TaskQuantity)
                {
                    quantity = rop.AdjustQuantity - obTakeDataLimit.TaskQuantity;
                    obTakeDataLimit.UnTakeQuantity += quantity;
                }
                else
                {
                    quantity = obTakeDataLimit.TaskQuantity - rop.AdjustQuantity;
                    obTakeDataLimit.UnTakeQuantity -= quantity;
                }

                obTakeDataLimit.TaskQuantity = rop.AdjustQuantity;

                obTakeDataLimit.Mender = operater;
                obTakeDataLimit.MendTime = this.DateTime;
                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

            return result;
        }
    }
}
