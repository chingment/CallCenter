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

        private static bool IsExists(string userId)
        {
            return RedisManager.Db.HashExists(key_list, userId);
        }

        public static bool Add(string userId)
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
            p_user.TeleSeatAccount = sysMerchantUser.TeleSeatAccount;
            p_user.TeleSeatPassword = sysMerchantUser.TeleSeatPassword;
            p_user.TelePhoneStatus = Enumeration.TelePhoneStatus.Unknow;
            p_user.WorkStatus = Enumeration.WorkStatus.Unknow;

            isFlag = RedisManager.Db.HashSet(key_list, userId, p_user.ToJsonString(), StackExchange.Redis.When.Always);

            return isFlag;
        }

        public static bool Edit(string userId, UserModel p_user)
        {
            if (!IsExists(userId))
            {
                var isAddSuccessed = Add(userId);
                if (!isAddSuccessed)
                    return false;
            }

            var l_user = RedisManager.Db.HashGet(key_list, userId).ToString().ToJsonObject<UserModel>();
            l_user.FullName = p_user.FullName;
            l_user.OrganizationId = p_user.OrganizationId;
            l_user.PositionId = p_user.PositionId;
            l_user.TeleSeatAccount = p_user.TeleSeatAccount;
            l_user.TeleSeatPassword = p_user.TeleSeatPassword;

            var isFlag = RedisManager.Db.HashSet(key_list, userId, l_user.ToJsonString(), StackExchange.Redis.When.Always);

            return isFlag;

        }

        public static bool SetLastAccessTime(string userId, DateTime lastAccessTime)
        {
            if (!IsExists(userId))
            {
                var isAddSuccessed = Add(userId);
                if (!isAddSuccessed)
                    return false;
            }

            var l_user = RedisManager.Db.HashGet(key_list, userId).ToString().ToJsonObject<UserModel>();
            l_user.LastAccessTime = lastAccessTime;

            var isFlag = RedisManager.Db.HashSet(key_list, userId, l_user.ToJsonString(), StackExchange.Redis.When.Always);

            return isFlag;
        }


        public static List<UserModel> GetList(string operater, string merchantId, string userId)
        {
            var accessUserIds = MerchServiceFactory.User.GetCanAccessUserIds(operater, merchantId, userId);

            var list_users = new List<UserModel>();

            var hs_users = RedisManager.Db.HashGetAll(key_list);

            foreach (var hs_user in hs_users)
            {
                var user = hs_user.Value.ToString().ToJsonObject<UserModel>();

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

                list_users.Add(user);
            }


            list_users = list_users.Where(m => m.MerchantId == merchantId && accessUserIds.Contains(m.UserId)).ToList();

            return list_users;


        }
    }
}
