using Lumos.DAL;
using Lumos.Entity;
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
                CurrentDb.SaveChanges();


                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "新建成功");

                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchantId, RopUserEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();

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
                user.MendTime = DateTime.Now;
                user.Mender = operater;
                CurrentDb.SaveChanges();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

                CurrentDb.SaveChanges();
                ts.Complete();
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
    }
}
