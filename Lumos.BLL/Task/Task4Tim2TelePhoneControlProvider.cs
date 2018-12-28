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
                    if (user.LastAccessTime.AddMinutes(1) > DateTime.Now)
                    {
                        user.WorkStatus = Enumeration.WorkStatus.OnLine;
                        user.WorkStatusName = "在线";
                    }
                    else
                    {
                        user.WorkStatus = Enumeration.WorkStatus.OffLine;
                        user.WorkStatusName = "离线";
                    }

                    user.TelePhoneStatus = SdkFactory.Lxt.GetStatus(user.TeleSeatAccount);

                    switch (user.TelePhoneStatus)
                    {
                        case Enumeration.TelePhoneStatus.IDLE:
                            user.TelePhoneStatusName = "空闲";
                            break;
                        case Enumeration.TelePhoneStatus.CallOut:
                            user.TelePhoneStatusName = "正在外呼通话中";
                            break;
                        case Enumeration.TelePhoneStatus.CallIn:
                            user.TelePhoneStatusName = "正在呼入通话中";
                            break;
                        case Enumeration.TelePhoneStatus.Ringing:
                            user.TelePhoneStatusName = "正在响铃中";
                            break;
                        case Enumeration.TelePhoneStatus.Process:
                            user.TelePhoneStatusName = "正在整理中";
                            break;
                        default:
                            user.TelePhoneStatusName = "未就绪";
                            break;
                    }


                    LogUtil.Info(string.Format("用户:{0},在线状态:{1},话机状态:{2}", user.UserName, user.WorkStatusName, user.TelePhoneStatusName));

                    UserDataCacheUtil.Edit(user.MerchantId, user.UserId, user);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("检查电话控件任务发生异常", ex);
            }

        }
    }
}
