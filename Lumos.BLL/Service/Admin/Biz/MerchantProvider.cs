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
        public CustomJsonResult GetDetails(string operater, string id)
        {
            var ret = new RetMerchantGetDetails();
            var merchant = CurrentDb.Merchant.Where(m => m.Id == id).FirstOrDefault();
            if (merchant == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
            }

            var sysMerchantUser = CurrentDb.SysMerchantUser.Where(m => m.Id == merchant.UserId).FirstOrDefault();

            if (merchant == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
            }

            string[] importFileTmplsId = merchant.ImportFileTmpls.Split(',');
            string[] importFileTmplsNames = CurrentDb.ImportFileTmpl.Where(m => importFileTmplsId.Contains(m.Id)).Select(m => m.Name).ToArray();

            ret.Id = merchant.Id ?? ""; ;
            ret.UserName = sysMerchantUser.UserName ?? "";
            ret.Name = merchant.Name ?? "";
            ret.ContactAddress = merchant.ContactAddress ?? "";
            ret.ContactName = merchant.ContactName ?? "";
            ret.ContactPhone = merchant.ContactPhone ?? "";
            ret.SimpleCode = merchant.SimpleCode;
            ret.BusinessTypeName = merchant.BusinessType.GetCnName();
            ret.ImportFileTmplNames = string.Join(",", importFileTmplsNames);

            switch (merchant.ObTakeDataPeriodMode)
            {
                case Enumeration.ObTakeDataPeriodMode.Week:
                    ret.ObTakeDataPeriodMode = string.Format("按周，默认每周每人取量上限{0}条，如需调整请到数据限额调整页面", merchant.ObTakeDataPeriodQuantity);
                    break;
                case Enumeration.ObTakeDataPeriodMode.Day:
                    ret.ObTakeDataPeriodMode = string.Format("按天，默认每天每人取量上限{0}条，如需调整请到数据限额调整页面", merchant.ObTakeDataPeriodQuantity);
                    break;
                default:
                    ret.ObTakeDataPeriodMode = "未配置";
                    break;
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult Add(string operater, RopMerchantAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                var isExistsSysUser = CurrentDb.SysUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();

                if (isExistsSysUser != null)
                {
                    return new CustomJsonResult(ResultType.Failure, string.Format("该用户名（{0}）已经被使用", rop.UserName));
                }

                var isExistsMerchant = CurrentDb.Merchant.Where(m => m.SimpleCode == rop.SimpleCode).FirstOrDefault();

                if (isExistsMerchant != null)
                {
                    return new CustomJsonResult(ResultType.Failure, string.Format("该商户代码（{0}）已经被使用", rop.SimpleCode));
                }

                var sysPosition = CurrentDb.SysPosition.Where(m => m.BelongSite == Enumeration.BelongSite.Merchant && m.Id == Enumeration.SysPositionId.MerchantAdministrator).FirstOrDefault();
                if (sysPosition == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "初始角色未指定");
                }

                if (rop.BusinessType == Enumeration.BusinessType.Unknow)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未指定业务类型");
                }

                if (rop.ObTakeDataPeriodMode == Enumeration.ObTakeDataPeriodMode.Unknow)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未指定业务量类型");
                }

                if (rop.ImportFileTmpls == null || rop.ImportFileTmpls.Length == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "未指定导入模板");
                }

                string organizationId = GuidUtil.New();
                string merchantId = GuidUtil.New();
                var sysMerchatUser = new SysMerchantUser();
                sysMerchatUser.Id = GuidUtil.New();
                sysMerchatUser.UserName = string.Format("M{0}", rop.UserName);
                sysMerchatUser.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                sysMerchatUser.SecurityStamp = Guid.NewGuid().ToString();
                sysMerchatUser.FullName = rop.ContactName;
                sysMerchatUser.RegisterTime = this.DateTime;
                sysMerchatUser.Status = Enumeration.UserStatus.Normal;
                sysMerchatUser.BelongSite = Enumeration.BelongSite.Merchant;
                sysMerchatUser.IsCanDelete = false;
                sysMerchatUser.CreateTime = this.DateTime;
                sysMerchatUser.Creator = operater;
                sysMerchatUser.MerchantId = merchantId;
                sysMerchatUser.OrganizationId = organizationId;
                sysMerchatUser.PositionId = sysPosition.Id;
                CurrentDb.SysMerchantUser.Add(sysMerchatUser);


                var merchant = new Merchant();
                merchant.Id = merchantId;
                merchant.UserId = sysMerchatUser.Id;
                merchant.Name = rop.MerchantName;
                merchant.ContactName = rop.ContactName;
                merchant.ContactPhone = rop.ContactPhone;
                merchant.ContactAddress = rop.ContactAddress;
                merchant.SimpleCode = rop.SimpleCode;
                merchant.CreateTime = this.DateTime;
                merchant.Creator = operater;
                merchant.BusinessType = rop.BusinessType;
                merchant.ObTakeDataPeriodMode = rop.ObTakeDataPeriodMode;
                merchant.ObTakeDataPeriodQuantity = rop.ObTakeDataPeriodQuantity;
                merchant.ImportFileTmpls = string.Join(",", rop.ImportFileTmpls);
                CurrentDb.Merchant.Add(merchant);



                var organization = new Organization();
                organization.Id = organizationId;
                organization.PId = GuidUtil.Empty();
                organization.Name = "总公司";
                organization.Dept = 0;
                organization.IsDelete = false;
                organization.Status = Enumeration.OrganizationStatus.Valid;
                organization.Priority = 0;
                organization.MerchantId = merchantId;
                organization.FullName = organization.Name;
                organization.Creator = operater;
                organization.CreateTime = this.DateTime;

                CurrentDb.Organization.Add(organization);

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, "新建成功");

            }

            return result;
        }

        public CustomJsonResult Edit(string operater, RopMerchantEdit rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {

                var merchant = CurrentDb.Merchant.Where(m => m.Id == rop.Id).FirstOrDefault();

                if (merchant == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }

                var sysMerchantUser = CurrentDb.SysMerchantUser.Where(m => m.Id == merchant.UserId).FirstOrDefault();
                if (sysMerchantUser == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }

                if (!string.IsNullOrEmpty(rop.Password))
                {
                    sysMerchantUser.PasswordHash = PassWordHelper.HashPassword(rop.Password);
                }

                sysMerchantUser.FullName = rop.ContactName;
                sysMerchantUser.MendTime = this.DateTime;
                sysMerchantUser.Mender = operater;


                merchant.ContactName = rop.ContactName;
                merchant.ContactPhone = rop.ContactPhone;
                merchant.ContactAddress = rop.ContactAddress;
                merchant.MendTime = this.DateTime;
                merchant.Mender = operater;


                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, "保存成功");
            }

            return result;
        }

    }
}
