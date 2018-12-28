using Lumos.BLL.Biz;
using Lumos.BLL.Service.Admin;
using Lumos.BLL.Service.Merch;
using Lumos.Entity;
using Lumos.Redis;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lumos.BLL.Task
{

    public enum TimerTaskType
    {
        [Remark("未知")]
        Unknow = 0,
        [Remark("检查订单支付状态")]
        CheckOrderPay = 1
    }


    public class Task4Tim2GlobalData
    {
        public string Id { get; set; }
        public TimerTaskType Type { get; set; }
        public DateTime ExpireTime { get; set; }
        public object Data { get; set; }
    }

    public class Task4Tim2GlobalProvider : BaseProvider, IJob
    {
        private static readonly string key = "task4GlobalTimer";

        public void Enter(TimerTaskType type, DateTime expireTime, object data)
        {
            var d = new Task4Tim2GlobalData();
            d.Id = GuidUtil.New();
            d.Type = type;
            d.ExpireTime = expireTime;
            d.Data = data;
            RedisManager.Db.HashSetAsync(key, d.Id, Newtonsoft.Json.JsonConvert.SerializeObject(d), StackExchange.Redis.When.Always);
        }

        public void Exit(string id)
        {
            RedisManager.Db.HashDelete(key, id);
        }

        public static List<Task4Tim2GlobalData> GetList()
        {
            List<Task4Tim2GlobalData> list = new List<Task4Tim2GlobalData>();
            var hs = RedisManager.Db.HashGetAll(key);

            var d = (from i in hs select i).ToList();

            foreach (var item in d)
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<Task4Tim2GlobalData>(item.Value);
                list.Add(obj);
            }
            return list;
        }

        public void Execute(IJobExecutionContext context)
        {
            #region 检查支付状态
            try
            {
                var lists = GetList();
                LogUtil.Info(string.Format("共有{0}条记录需要检查状态", lists.Count));
                if (lists.Count > 0)
                {
                    LogUtil.Info(string.Format("开始执行订单查询,时间：{0}", this.DateTime));
                    foreach (var m in lists)
                    {
                        switch (m.Type)
                        {
                            case TimerTaskType.CheckOrderPay:
                                break;
                        }
                    }

                    LogUtil.Info(string.Format("结束执行订单查询,时间:{0}", this.DateTime));
                }

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
                LogUtil.Error("全局定时任务发生异常", ex);
            }
            #endregion
        }
    }
}
