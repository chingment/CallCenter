using Lumos.DAL;
using Lumos.Entity;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{



    public class UserProvider : BaseProvider
    {

        public List<string> GetCanAccessUserIds(string merchantId, string id)
        {
            List<string> userIds = new List<string>();
            var user = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();
            if (user == null)
                return userIds;
            var position = CurrentDb.SysPosition.Where(m => m.Id == user.PositionId).FirstOrDefault();
            if (position == null)
                return userIds;

            if (position.DataRightType == Enumeration.DataRightType.Self)
            {
                userIds.Add(user.Id);
            }
            else if (position.DataRightType == Enumeration.DataRightType.Organization)
            {
                List<Organization> organizations = MerchServiceFactory.Organization.GetSons(merchantId, user.OrganizationId);

                List<string> organizationIds = organizations.Select(m => m.Id).ToList<string>();

                var users = CurrentDb.SysMerchantUser.Where(m => organizationIds.Contains(m.OrganizationId)).ToList();

                foreach (var m_user in users)
                {
                    userIds.Add(m_user.Id);
                }
            }

            return userIds;
        }

        public CustomJsonResult GetDetails(string operater, string merchantId, string id)
        {
            var ret = new RetUserGetDetails();

            var user = CurrentDb.SysMerchantUser.Where(m => m.Id == id).FirstOrDefault();
            if (user != null)
            {
                ret.UserName = user.UserName ?? ""; ;
                ret.FullName = user.FullName ?? ""; ;
                ret.Email = user.Email ?? ""; ;
                ret.PhoneNumber = user.PhoneNumber ?? "";
                ret.OrganizationId = user.OrganizationId;
                ret.PositionId = user.PositionId;
                ret.Status = user.Status;
                ret.TeleSeatId = user.TeleSeatId;
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult GetPersonalInfo(string operater, string merchantId, string id)
        {
            var ret = new RetUserGetPersonalInfo();

            var user = CurrentDb.SysMerchantUser.Where(m => m.Id == id).FirstOrDefault();
            if (user != null)
            {
                ret.UserName = user.UserName ?? ""; ;
                ret.FullName = user.FullName ?? ""; ;
                ret.Email = user.Email ?? ""; ;
                ret.PhoneNumber = user.PhoneNumber ?? "";
                ret.PositionName = user.PositionId.GetCnName();

                var organization = CurrentDb.Organization.Where(m => m.Id == user.OrganizationId).FirstOrDefault();
                if (organization != null)
                {
                    ret.OrganizationName = organization.FullName ?? "";
                }

                var teleSeat = CurrentDb.TeleSeat.Where(m => m.Id == user.TeleSeatId).FirstOrDefault();

                if (teleSeat != null)
                {
                    ret.TeleSeatAccount = teleSeat.Account ?? "";
                    ret.TeleSeatPassword = teleSeat.Password ?? "";
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult Add(string operater, string merchantId, RopUserAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var merchant = CurrentDb.Merchant.Where(m => m.Id == merchantId).FirstOrDefault();

            string userName = string.Format("{0}{1}", merchant.SimpleCode, rop.UserName);

            var isExistUserName = CurrentDb.SysUser.Where(m => m.UserName == userName).FirstOrDefault();
            if (isExistUserName != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该用户名（{0}）已被使用", userName));
            }

            string userId = "";
            using (TransactionScope ts = new TransactionScope())
            {
                var user = new SysMerchantUser();
                user.Id = GuidUtil.New();
                userId = user.Id;
                user.UserName = userName;
                user.FullName = rop.FullName;
                user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                user.Email = rop.Email;
                user.PhoneNumber = rop.PhoneNumber;
                user.BelongSite = Enumeration.BelongSite.Merchant;
                user.IsDelete = false;
                user.IsCanDelete = true;
                user.Status = Enumeration.UserStatus.Normal;
                user.MerchantId = merchantId;
                user.OrganizationId = rop.OrganizationId;
                user.PositionId = rop.PositionId;
                user.TeleSeatId = rop.TeleSeatId;
                user.Creator = operater;
                user.CreateTime = DateTime.Now;
                user.RegisterTime = DateTime.Now;
                user.Status = Enumeration.UserStatus.Normal;
                user.SecurityStamp = Guid.NewGuid().ToString().Replace("-", "");
                CurrentDb.SysMerchantUser.Add(user);

                var obTakeDataLimit = new ObTakeDataLimit();
                obTakeDataLimit.Id = GuidUtil.New();
                obTakeDataLimit.MerchantId = merchantId;
                obTakeDataLimit.SalesmanId = user.Id;
                obTakeDataLimit.TaskQuantity = merchant.ObTakeDataPeriodQuantity;
                obTakeDataLimit.UnTakeQuantity = merchant.ObTakeDataPeriodQuantity;
                obTakeDataLimit.TakedQuantity = 0;
                obTakeDataLimit.Creator = operater;
                obTakeDataLimit.CreateTime = this.DateTime;
                CurrentDb.ObTakeDataLimit.Add(obTakeDataLimit);

                var position = CurrentDb.SysPosition.Where(m => m.Id == rop.PositionId).FirstOrDefault();

                if (position.IsOrganizationHeader)
                {
                    var organization = CurrentDb.Organization.Where(m => m.Id == rop.OrganizationId).FirstOrDefault();

                    if (!string.IsNullOrEmpty(organization.HeaderId))
                    {
                        var header = CurrentDb.SysMerchantUser.Where(m => m.Id == organization.HeaderId).FirstOrDefault();

                        return new CustomJsonResult(ResultType.Failure, ResultCode.FailureNeedReplaceTips, string.Format("该机构已存在负责人（{0}[{1}]），如需替换，请先更改其职位", header.FullName, header.UserName));
                    }

                    organization.HeaderId = user.Id;

                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

            }

            if (result.Result == ResultType.Success)
            {
                UserDataCacheUtil.Add(merchantId, userId);
            }

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchantId, RopUserEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();

            var userModel = new UserModel();

            using (TransactionScope ts = new TransactionScope())
            {
                var user = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.Id == rop.Id).FirstOrDefault();

                if (!string.IsNullOrEmpty(rop.Password))
                {
                    user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }

                if (!string.IsNullOrEmpty(rop.TeleSeatId))
                {
                    var teleSeatIsUse = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.TeleSeatId == rop.TeleSeatId && m.Id != rop.Id).FirstOrDefault();
                    if (teleSeatIsUse != null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该话机号已经被使用");
                    }
                }

             
                if (user.PositionId != rop.PositionId)
                {
                    if (user.PositionId == Enumeration.SysPositionId.MerchantTSR)
                    {
                        var unTakeCount = CurrentDb.ObCustomer.Where(m => m.BelongerId == user.Id && m.IsTake == false).Count();
                        if (unTakeCount > 0)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "更换职位需要把未取的数据（" + unTakeCount + "）条转移到同组的其他成员");
                        }
                    }
                }

                user.FullName = rop.FullName;
                user.Email = rop.Email;
                user.PhoneNumber = rop.PhoneNumber;
                user.OrganizationId = rop.OrganizationId;
                user.PositionId = rop.PositionId;
                user.TeleSeatId = rop.TeleSeatId;
                user.Status = rop.Status;
                user.MendTime = DateTime.Now;
                user.Mender = operater;

                var position = CurrentDb.SysPosition.Where(m => m.Id == rop.PositionId).FirstOrDefault();

                var organization = CurrentDb.Organization.Where(m => m.Id == rop.OrganizationId).FirstOrDefault();

                if (position.IsOrganizationHeader)
                {
                    if (!string.IsNullOrEmpty(organization.HeaderId))
                    {
                        if (organization.HeaderId != user.Id)
                        {
                            var header = CurrentDb.SysMerchantUser.Where(m => m.Id == organization.HeaderId).FirstOrDefault();

                            return new CustomJsonResult(ResultType.Failure, ResultCode.FailureNeedReplaceTips, string.Format("该机构已存在负责人（{0}[{1}]），如需替换，请先更改其职位", header.FullName, header.UserName));
                        }
                    }

                    organization.HeaderId = user.Id;

                }
                else
                {
                    if (organization.HeaderId == user.Id)
                    {
                        organization.HeaderId = null;
                    }
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                //userModel.UserId = user.Id;
                //userModel.FullName = user.FullName;
                //userModel.OrganizationId = user.OrganizationId;
                //userModel.PositionId = user.PositionId;
                //userModel.TeleSeatAccount = user.TeleSeatAccount;
                //userModel.TeleSeatPassword = user.TeleSeatPassword;

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            if (result.Result == ResultType.Success)
            {
                //UserDataCacheUtil.Edit(merchantId, rop.Id);
            }

            return result;


        }

        public CustomJsonResult Delete(string operater, string merchantId, string id)
        {


            var user = CurrentDb.SysUser.Find(id);

            if (user == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到用户");


            if (!user.IsCanDelete)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("不允许删除用户（{0}）", user.UserName));
            }

            user.IsDelete = true;
            user.Mender = operater;
            user.MendTime = DateTime.Now;

            CurrentDb.SaveChanges();


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");
        }

        public List<string> GetPermissions(string operater, string merchantId, string id)
        {
            List<string> list = new List<string>();


            var adminUser = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();
            if (adminUser == null)
                return list;


            var model = (from sysMenuPermission in CurrentDb.SysMenuPermission
                         where
                             (from sysRoleMenu in CurrentDb.SysRoleMenu
                              where
                              (from sysPositionRole in CurrentDb.SysPositionRole
                               where sysPositionRole.PositionId == adminUser.PositionId
                               select sysPositionRole.RoleId)
                              .Contains(sysRoleMenu.RoleId)
                              select sysRoleMenu.MenuId).Contains(sysMenuPermission.MenuId)
                         select sysMenuPermission.PermissionId).Distinct();

            if (model != null)
            {
                list = model.ToList();
            }

            return list;
        }

        public CustomJsonResult RunHeartbeatPacket(string operater, string merchantId, string id)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetRunHeartbeatPacket();

            UserDataCacheUtil.SetLastAccessTime(merchantId, id, DateTime.Now);

            var userModel = UserDataCacheUtil.Get(merchantId, id);

            ret.WorkStatusName = userModel.WorkStatusName;
            ret.TelePhoneStatusName = userModel.TelePhoneStatusName;

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

    }
}
