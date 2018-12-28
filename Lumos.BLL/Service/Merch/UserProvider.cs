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

            var merchant = CurrentDb.Merchant.Where(m => m.Id == merchantId).FirstOrDefault();

            ret.SimpleCode = merchant.SimpleCode;

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
                var organization = CurrentDb.Organization.Where(m => m.Id == user.OrganizationId).FirstOrDefault();
                ret.OrganizationName = organization.FullName;
                ret.PositionName = user.PositionId.GetCnName();
                ret.TeleSeatAccount = user.TeleSeatAccount ?? "";
                ret.TeleSeatPassword = user.TeleSeatPassword ?? "";
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult Add(string operater, string merchantId, RopUserAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            var isExistUserName = CurrentDb.SysUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();
            if (isExistUserName != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该用户名（{0}）已被使用", rop.UserName));
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var merchant = CurrentDb.Merchant.Where(m => m.Id == merchantId).FirstOrDefault();

                var user = new SysMerchantUser();
                user.Id = GuidUtil.New();
                user.UserName = string.Format("{0}{1}", merchant.SimpleCode, rop.UserName);
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
                obTakeDataLimit.UnContactQuantity = 0;
                obTakeDataLimit.TargetQuantity = 0;
                obTakeDataLimit.InValidQuantity = 0;
                obTakeDataLimit.Creator = operater;
                obTakeDataLimit.CreateTime = this.DateTime;
                CurrentDb.ObTakeDataLimit.Add(obTakeDataLimit);

                CurrentDb.SaveChanges();


                var sysPosition = CurrentDb.SysPosition.Where(m => m.Id == rop.PositionId).FirstOrDefault();

                if (sysPosition.IsOrganizationHeader)
                {
                    var organization = CurrentDb.Organization.Where(m => m.Id == rop.OrganizationId).FirstOrDefault();

                    if (organization.HeaderId != user.Id)
                    {
                        var header = CurrentDb.SysMerchantUser.Where(m => m.Id == organization.HeaderId).FirstOrDefault();
                        if (header.Status == Enumeration.UserStatus.Normal)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.FailureNeedReplaceTips, string.Format("该机构的负责人（{0}[{1}]）的账号正常使用中，如需替换负责人，请先将账号设置无效", header.FullName, header.UserName));
                        }

                    }

                    organization.HeaderId = user.Id;

                }

                UserDataCacheUtil.Add(user.MerchantId, user.Id);

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

            }

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchantId, RopUserEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();


            var userModel = new UserModel();

            using (TransactionScope ts = new TransactionScope())
            {
                var user = CurrentDb.SysMerchantUser.Where(m => m.Id == rop.Id).FirstOrDefault();
                if (!string.IsNullOrEmpty(rop.Password))
                {
                    user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }

                user.FullName = rop.FullName;
                user.Email = rop.Email;
                user.PhoneNumber = rop.PhoneNumber;
                user.OrganizationId = rop.OrganizationId;
                user.PositionId = rop.PositionId;
                user.Status = rop.Status;
                user.MendTime = DateTime.Now;
                user.Mender = operater;

                CurrentDb.SaveChanges();


                var sysPosition = CurrentDb.SysPosition.Where(m => m.Id == rop.PositionId).FirstOrDefault();
                var organization = CurrentDb.Organization.Where(m => m.Id == rop.OrganizationId).FirstOrDefault();

                if (sysPosition.IsOrganizationHeader)
                {

                    if (!string.IsNullOrEmpty(organization.HeaderId))
                    {
                        if (organization.HeaderId != user.Id)
                        {

                            var header = CurrentDb.SysMerchantUser.Where(m => m.Id == organization.HeaderId).FirstOrDefault();
                            if (header != null)
                            {
                                if (!rop.IsReplacePosition)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.FailureNeedReplaceTips, string.Format("该机构已存在负责人（{0}[{1}]），若确定，他的职位将改为普通人员？", header.FullName, header.UserName));
                                }
                                else
                                {
                                    header.PositionId = Enumeration.SysPositionId.MerchantStaff;
                                }
                            }

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

                userModel.UserId = user.Id;
                userModel.FullName = user.FullName;
                userModel.OrganizationId = user.OrganizationId;
                userModel.PositionId = user.PositionId;
                userModel.TeleSeatAccount = user.TeleSeatAccount;
                userModel.TeleSeatPassword = user.TeleSeatPassword;

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

            }

            if (result.Result == ResultType.Success)
            {
                UserDataCacheUtil.Edit(merchantId, rop.Id, userModel);
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

        public void SetLastAccessTime(string operater, string merchantId, string id, DateTime lastAccessTime)
        {
            UserDataCacheUtil.SetLastAccessTime(merchantId, id, lastAccessTime);
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
