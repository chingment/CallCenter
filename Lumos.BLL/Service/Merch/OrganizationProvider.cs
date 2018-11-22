using Lumos.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Lumos.BLL.Service.Merch
{
    public class OrganizationProvider : BaseProvider
    {
        public CustomJsonResult GetDetails(string pOperater, string pMerchantId, string pSubjectId)
        {
            var ret = new RetOrganizationGetDetails();
            var organization = CurrentDb.MchOrganization.Where(m => m.MerchantId == pMerchantId && m.Id == pSubjectId).FirstOrDefault();
            if (organization != null)
            {
                ret.OrganizationId = organization.Id ?? "";
                ret.Name = organization.Name ?? "";
                ret.Description = organization.Description ?? "";
                ret.Status = organization.Status;
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }


        public CustomJsonResult Add(string pOperater, string pMerchantId, RopOrganizationAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var isExistOrganization = CurrentDb.MchOrganization.Where(m => m.MerchantId == pMerchantId && m.Name == rop.Name).FirstOrDefault();
                if (isExistOrganization != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在");
                }

                var organization = new MchOrganization();
                organization.Id = GuidUtil.New();
                organization.MerchantId = pMerchantId;
                organization.PId = rop.PId;
                organization.Name = rop.Name;
                organization.Description = rop.Description;
                organization.Status = Enumeration.OrganizationStatus.Valid;
                organization.Creator = pOperater;
                organization.CreateTime = DateTime.Now;
                CurrentDb.MchOrganization.Add(organization);
                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "添加成功");
            }

            return result;
        }

        public CustomJsonResult Edit(string pOperater, string pMerchantId, RopOrganizationEdit rop)
        {
            var organization = CurrentDb.MchOrganization.Where(m => m.Id == rop.OrganizationId).FirstOrDefault();

            var isExistlOrganization = CurrentDb.MchOrganization.Where(m => m.MerchantId == pMerchantId && m.Id != organization.Id && m.Name == rop.Name).FirstOrDefault();
            if (isExistlOrganization != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在");
            }

            organization.Name = rop.Name;
            organization.Status = rop.Status;
            organization.Description = rop.Description;
            organization.Mender = pOperater;
            organization.MendTime = DateTime.Now;
            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

        }

        public CustomJsonResult Delete(string pOperater, string pMerchantId, string[] pOrganizationIds)
        {
            if (pOrganizationIds != null)
            {
                foreach (var id in pOrganizationIds)
                {
                    var organization = CurrentDb.MchOrganization.Where(m => m.Id == id).FirstOrDefault();
                    if (organization != null)
                    {
                        organization.IsDelete = true;

                        var sysMerchantUsers = CurrentDb.SysMerchantUser.Where(m => m.OrganizationId == id).ToList();

                        foreach (var sysMerchantUser in sysMerchantUsers)
                        {
                            sysMerchantUser.OrganizationId = null;
                        }

                        CurrentDb.SaveChanges();
                    }
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");
        }

    }
}
