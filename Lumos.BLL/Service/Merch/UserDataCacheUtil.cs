using Lumos.DAL;
using Lumos.Redis;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class UserDataCacheUtil
    {
        private static readonly string key_list = "userDataCache";

        private static bool IsExists(string merchantId, string userId)
        {
            return RedisManager.Db.HashExists(key_list, userId);
        }

        public static bool Add(string merchantId, string userId)
        {
            var isFlag = false;

            LumosDbContext db = new LumosDbContext();

            var sysMerchantUser = db.SysMerchantUser.Where(m => m.Id == userId).FirstOrDefault();

            if (sysMerchantUser == null)
                return false;

            UserModel p_user = new UserModel();
            p_user.UserId = sysMerchantUser.Id;
            p_user.UserName = sysMerchantUser.UserName;
            p_user.FullName = sysMerchantUser.FullName;
            p_user.MerchantId = sysMerchantUser.MerchantId;
            p_user.OrganizationId = sysMerchantUser.OrganizationId;
            p_user.PositionId = sysMerchantUser.PositionId;
            p_user.WorkStatus = Enumeration.WorkStatus.Unknow;
            p_user.TeleSeatId = sysMerchantUser.TeleSeatId;
            var teleSeat = db.TeleSeat.Where(m => m.Id == sysMerchantUser.TeleSeatId).FirstOrDefault();
            if (teleSeat != null)
            {
                p_user.TeleSeatAccount = teleSeat.Account;
                p_user.TeleSeatPassword = teleSeat.Password;
                p_user.TelePhoneStatus = Enumeration.TelePhoneStatus.Unknow;
            }

            isFlag = RedisManager.Db.HashSet(key_list, userId, p_user.ToJsonString(), StackExchange.Redis.When.Always);

            return isFlag;
        }

        public static bool Edit(string merchantId, string userId,string fullName,string organizationId, Enumeration.SysPositionId positionId,string teleSeatId)
        {
            if (!IsExists(merchantId, userId))
            {
                var isAddSuccessed = Add(merchantId, userId);
                if (!isAddSuccessed)
                    return false;
            }

            var l_user = RedisManager.Db.HashGet(key_list, userId).ToString().ToJsonObject<UserModel>();
            l_user.FullName = fullName;
            l_user.OrganizationId = organizationId;
            l_user.PositionId = positionId;
            l_user.TeleSeatId = teleSeatId;

            var isFlag = RedisManager.Db.HashSet(key_list, userId, l_user.ToJsonString(), StackExchange.Redis.When.Always);
            return isFlag;

        }

        public static bool SetLastAccessTime(string merchantId, string userId, DateTime lastAccessTime)
        {
            if (!IsExists(merchantId, userId))
            {
                var isAddSuccessed = Add(merchantId, userId);
                if (!isAddSuccessed)
                    return false;
            }

            var l_user = RedisManager.Db.HashGet(key_list, userId).ToString().ToJsonObject<UserModel>();
            l_user.LastAccessTime = lastAccessTime;
            l_user.TimeOutTime = lastAccessTime.AddSeconds(30);
            var isFlag = RedisManager.Db.HashSet(key_list, userId, l_user.ToJsonString(), StackExchange.Redis.When.Always);

            return isFlag;
        }

        public static bool Edit(UserModel user)
        {
            var isFlag = RedisManager.Db.HashSet(key_list, user.UserId, user.ToJsonString(), StackExchange.Redis.When.Always);
            return isFlag;
        }

        public static List<UserModel> GetList(string merchantId, string userId)
        {
            var accessUserIds = MerchServiceFactory.User.GetCanAccessUserIds(merchantId, userId);
            var list_users = GetList().Where(m => m.MerchantId == merchantId && accessUserIds.Contains(m.UserId)).ToList();
            return list_users;
        }

        public static UserModel Get(string merchantId, string userId)
        {
            var userModel = new UserModel();

            var user = GetList().Where(m => m.MerchantId == merchantId && m.UserId == userId).FirstOrDefault();
            if (user != null)
            {
                userModel = user;
            }

            return userModel;
        }

        public static List<UserModel> GetList()
        {
            var list_users = new List<UserModel>();
            var hs_users = RedisManager.Db.HashGetAll(key_list);
            foreach (var hs_user in hs_users)
            {
                var user = hs_user.Value.ToString().ToJsonObject<UserModel>();

                //if (user.LastAccessTime.AddMinutes(1) > DateTime.Now)
                //{
                //    user.WorkStatus = Enumeration.WorkStatus.OnLine;
                //    user.WorkStatusName = "在线";
                //}
                //else
                //{
                //    user.WorkStatus = Enumeration.WorkStatus.OffLine;
                //    user.WorkStatusName = "离线";
                //}

                //switch (user.TelePhoneStatus)
                //{
                //    case Enumeration.TelePhoneStatus.IDLE:
                //        user.TelePhoneStatusName = "空闲";
                //        break;
                //    case Enumeration.TelePhoneStatus.CallOut:
                //        user.TelePhoneStatusName = "正在外呼通话中";
                //        break;
                //    case Enumeration.TelePhoneStatus.CallIn:
                //        user.TelePhoneStatusName = "正在呼入通话中";
                //        break;
                //    case Enumeration.TelePhoneStatus.Ringing:
                //        user.TelePhoneStatusName = "正在响铃中";
                //        break;
                //    case Enumeration.TelePhoneStatus.Process:
                //        user.TelePhoneStatusName = "正在整理中";
                //        break;
                //    default:
                //        user.TelePhoneStatusName = "未就绪";
                //        break;
                //}

                list_users.Add(user);
            }

            return list_users;
        }
    }
}
