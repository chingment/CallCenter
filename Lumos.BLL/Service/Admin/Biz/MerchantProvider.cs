using Lumos.DAL;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Admin
{
    public class MerchantProvider : BaseProvider
    {
        public CustomJsonResult GetDetails(string pOperater, string merchantId)
        {
            var ret = new RetMerchantGetDetails();
            var sysMerchantUser = CurrentDb.SysMerchantUser.Where(m => m.Id == merchantId).FirstOrDefault();
            if (sysMerchantUser != null)
            {
                ret.Id = sysMerchantUser.Id ?? ""; ;
                ret.UserName = sysMerchantUser.UserName ?? ""; ;

                var merchantInfo = CurrentDb.Merchant.Where(m => m.MerchantId == merchantId).FirstOrDefault();
                ret.MerchantName = merchantInfo.Name ?? ""; ;
                ret.ContactAddress = merchantInfo.ContactAddress ?? "";
                ret.ContactName = merchantInfo.ContactName ?? "";
                ret.ContactPhone = merchantInfo.ContactPhone ?? "";
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult Add(string pOperater, RopMerchantAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                var isExistsSysUser = CurrentDb.SysUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();

                if (isExistsSysUser != null)
                {
                    return new CustomJsonResult(ResultType.Failure, "账号已经存在");
                }

                var sysMerchatUser = new SysMerchantUser();
                sysMerchatUser.Id = GuidUtil.New();
                sysMerchatUser.UserName = string.Format("M{0}", rop.UserName);
                sysMerchatUser.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                sysMerchatUser.SecurityStamp = Guid.NewGuid().ToString();
                sysMerchatUser.RegisterTime = this.DateTime;
                sysMerchatUser.CreateTime = this.DateTime;
                sysMerchatUser.Creator = pOperater;
                sysMerchatUser.Status = Enumeration.UserStatus.Normal;
                sysMerchatUser.BelongSite = Enumeration.BelongSite.Merchant;

                sysMerchatUser.MerchantId = sysMerchatUser.Id;
                CurrentDb.SysMerchantUser.Add(sysMerchatUser);
                CurrentDb.SaveChanges();

                var merchant = new Merchant();
                merchant.Id = GuidUtil.New();
                merchant.MerchantId = sysMerchatUser.Id;
                merchant.Name = rop.MerchantName;
                merchant.ContactName = rop.ContactName;
                merchant.ContactPhone = rop.ContactPhone;
                merchant.ContactAddress = rop.ContactAddress;
                merchant.BusinessType = Enumeration.BusinessType.CarIns;
                merchant.CreateTime = this.DateTime;
                merchant.Creator = pOperater;
                CurrentDb.Merchant.Add(merchant);
                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, "新建成功");

            }

            return result;
        }

        public CustomJsonResult Edit(string pOperater, RopMerchantEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();
            using (TransactionScope ts = new TransactionScope())
            {
                var merchant = CurrentDb.Merchant.Where(m => m.MerchantId == rop.Id).FirstOrDefault();
                merchant.ContactName = rop.ContactName;
                merchant.ContactPhone = rop.ContactPhone;
                merchant.ContactAddress = rop.ContactAddress;
                merchant.MendTime = this.DateTime;
                merchant.Mender = pOperater;

                if (!string.IsNullOrEmpty(rop.Password))
                {
                    var sysMerchantUser = CurrentDb.SysMerchantUser.Where(m => m.Id == merchant.MerchantId).FirstOrDefault();
                    sysMerchantUser.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, "保存成功");
            }

            return result;
        }
    }
}
