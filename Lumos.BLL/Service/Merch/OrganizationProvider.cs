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
        public CustomJsonResult GetDetails(string operater, string merchantId, string id)
        {
            var ret = new RetOrganizationGetDetails();
            var organization = CurrentDb.Organization.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();
            if (organization != null)
            {
                ret.OrganizationId = organization.Id ?? "";
                ret.Name = organization.Name ?? "";
                ret.Description = organization.Description ?? "";
                ret.Status = organization.Status;
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }


        public CustomJsonResult Add(string operater, string merchantId, RopOrganizationAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var isExistOrganization = CurrentDb.Organization.Where(m => m.MerchantId == merchantId && m.Name == rop.Name).FirstOrDefault();
                if (isExistOrganization != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在");
                }

                var organization = new Organization();
                organization.Id = GuidUtil.New();
                organization.MerchantId = merchantId;
                organization.PId = rop.PId;
                organization.Name = rop.Name;
                organization.Description = rop.Description;
                organization.Status = Enumeration.OrganizationStatus.Valid;
                organization.Creator = operater;
                organization.CreateTime = DateTime.Now;
                CurrentDb.Organization.Add(organization);
                CurrentDb.SaveChanges();
                ts.Complete();
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "添加成功");
            }

            return result;
        }

        public CustomJsonResult Edit(string operater, string merchantId, RopOrganizationEdit rop)
        {
            var organization = CurrentDb.Organization.Where(m => m.Id == rop.OrganizationId).FirstOrDefault();

            var isExistlOrganization = CurrentDb.Organization.Where(m => m.MerchantId == merchantId && m.Id != organization.Id && m.Name == rop.Name).FirstOrDefault();
            if (isExistlOrganization != null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "名称已存在");
            }

            organization.Name = rop.Name;
            organization.Status = rop.Status;
            organization.Description = rop.Description;
            organization.Mender = operater;
            organization.MendTime = DateTime.Now;
            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

        }

        public CustomJsonResult Delete(string operater, string merchantId, string id)
        {
            //if (pOrganizationIds != null)
            //{
            //    foreach (var id in pOrganizationIds)
            //    {
            //        var organization = CurrentDb.Organization.Where(m => m.Id == id).FirstOrDefault();
            //        if (organization != null)
            //        {
            //            organization.IsDelete = true;

            //            var sysMerchantUsers = CurrentDb.SysMerchantUser.Where(m => m.OrganizationId == id).ToList();

            //            foreach (var sysMerchantUser in sysMerchantUsers)
            //            {
            //                sysMerchantUser.OrganizationId = null;
            //            }

            //            CurrentDb.SaveChanges();
            //        }
            //    }
            //}

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");
        }

    }
}
