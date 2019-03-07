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
    public class Task4Tim2TelePhoneControlProvider : BaseProvider, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {


                var users = UserDataCacheUtil.GetList();

                foreach (var user in users)
                {
                    if (user.TimeOutTime > DateTime.Now)
                    {
                        user.WorkStatus = Enumeration.WorkStatus.OnLine;
                    }
                    else
                    {
                        user.WorkStatus = Enumeration.WorkStatus.OffLine;
                    }

                    user.TelePhoneStatus = SdkFactory.Lxt.GetStatus(user.TeleSeatApiCustomer, user.TeleSeatApiPassword, user.TeleSeatAccount);

                    LogUtil.Info(string.Format("用户:{0},在线状态:{1},话机状态:{2}", user.UserName, user.WorkStatus.GetCnName(), user.TelePhoneStatus.GetCnName()));

                    UserDataCacheUtil.Update(user);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("检查电话控件任务发生异常", ex);
            }

        }
    }
}
