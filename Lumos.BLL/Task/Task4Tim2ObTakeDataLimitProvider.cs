using Lumos.BLL.Service.Merch;
using Lumos.Entity;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Task
{
    public class Task4Tim2ObTakeDataLimitProvider : BaseProvider, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                var merchants = CurrentDb.Merchant.ToList();
                foreach (var merchant in merchants)
                {
                    var obTakeDataLimits = CurrentDb.ObTakeDataLimit.Where(m => m.MerchantId == merchant.Id).ToList();

                    foreach (var obTakeDataLimit in obTakeDataLimits)
                    {
                        string date = DateTime.Now.ToUnifiedFormatDate();
                        var isExist = CurrentDb.ObTakeDataDayLog.Where(m => m.MerchantId == obTakeDataLimit.MerchantId && m.SalesmanId == obTakeDataLimit.SalesmanId && m.Date == date).FirstOrDefault();
                        if (isExist == null)
                        {
                            var obTakeDataDayLog = new ObTakeDataDayLog();
                            obTakeDataDayLog.Id = GuidUtil.New();
                            obTakeDataDayLog.Date = date;
                            obTakeDataDayLog.MerchantId = obTakeDataLimit.MerchantId;
                            obTakeDataDayLog.SalesmanId = obTakeDataLimit.SalesmanId;
                            obTakeDataDayLog.TaskQuantity = obTakeDataLimit.TaskQuantity;
                            obTakeDataDayLog.UnTakeQuantity = obTakeDataLimit.UnTakeQuantity;
                            obTakeDataDayLog.TakedQuantity = obTakeDataLimit.TakedQuantity;
                            obTakeDataDayLog.CreateTime = DateTime.Now;
                            obTakeDataDayLog.Creator = GuidUtil.Empty();
                            CurrentDb.ObTakeDataDayLog.Add(obTakeDataDayLog);
                            CurrentDb.SaveChanges();

                            switch (merchant.ObTakeDataPeriodMode)
                            {
                                case Enumeration.ObTakeDataPeriodMode.Day:
                                    obTakeDataLimit.UnTakeQuantity = obTakeDataLimit.TaskQuantity;
                                    obTakeDataLimit.TakedQuantity = 0;
                                    CurrentDb.SaveChanges();
                                    break;
                                case Enumeration.ObTakeDataPeriodMode.Week:
                                    if (DateTime.DayOfWeek == DayOfWeek.Sunday)
                                    {
                                        obTakeDataLimit.UnTakeQuantity = obTakeDataLimit.TaskQuantity;
                                        obTakeDataLimit.TakedQuantity = 0;
                                        CurrentDb.SaveChanges();
                                    }

                                    break;
                            }
                        }
                    }

                    CurrentDb.SaveChanges();
                }

                LogUtil.Info("任务执行完成:" + DateTime.Now.ToUnifiedFormatDate());
            }
            catch (Exception ex)
            {
                LogUtil.Error("发生异常", ex);
            }

        }
    }
}
