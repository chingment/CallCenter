﻿using Lumos.DAL;
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

        public CustomJsonResult GetDetails(string pOperater, string userId)
        {
            var ret = new RetUserGetDetails();
            var user = CurrentDb.SysMerchantUser.Where(m => m.Id == userId).FirstOrDefault();
            if (user != null)
            {
                ret.UserName = user.UserName ?? ""; ;
                ret.FullName = user.FullName ?? ""; ;
                ret.Email = user.Email ?? ""; ;
                ret.PhoneNumber = user.PhoneNumber ?? "";
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult Add(string pOperater, RopUserAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            var isExistUserName = CurrentDb.SysUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();
            if (isExistUserName != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该用户名（{0}）已被使用", rop.UserName));
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var user = new SysMerchantUser();
                user.Id = GuidUtil.New();
                user.UserName = string.Format("Up{0}", rop.UserName);
                user.FullName = rop.FullName;
                user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                user.Email = rop.Email;
                user.PhoneNumber = rop.PhoneNumber;
                user.BelongSite = Enumeration.BelongSite.Merchant;
                user.IsDelete = false;
                user.IsCanDelete = true;
                user.Status = Enumeration.UserStatus.Normal;
                user.Creator = pOperater;
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

        public CustomJsonResult Edit(string pOperater, RopUserEdit rop)
        {

            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var user = CurrentDb.SysMerchantUser.Where(m => m.Id == rop.UserId).FirstOrDefault();
                if (!string.IsNullOrEmpty(rop.Password))
                {
                    user.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }
                user.FullName = rop.FullName;
                user.Email = rop.Email;
                user.PhoneNumber = rop.PhoneNumber;
                user.MendTime = DateTime.Now;
                user.Mender = pOperater;
                CurrentDb.SaveChanges();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

                CurrentDb.SaveChanges();
                ts.Complete();
            }
            return result;


        }


        public CustomJsonResult Delete(string pOperater, string[] pUserIds)
        {
            if (pUserIds == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到用户");

            if (pUserIds.Length <= 0)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到用户");


            foreach (string userId in pUserIds)
            {
                SysUser user = CurrentDb.SysUser.Find(userId);

                if (!user.IsCanDelete)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("不允许删除用户（{0}）", user.UserName));
                }

                user.IsDelete = true;
                user.Mender = pOperater;
                user.MendTime = DateTime.Now;

                CurrentDb.SaveChanges();
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");
        }
    }
}
