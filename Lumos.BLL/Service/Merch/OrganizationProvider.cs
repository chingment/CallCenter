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

        private List<Organization> GetFathers(string merchantId, string id)
        {
            var sysOrganizations = CurrentDb.Organization.Where(m => m.MerchantId == merchantId).ToList();

            var list = new List<Organization>();
            var list2 = list.Concat(GetFatherList(sysOrganizations, id));
            return list2.OrderBy(m => m.Dept).ToList();
        }


        public IEnumerable<Organization> GetFatherList(IList<Organization> list, string pId)
        {
            var query = list.Where(p => p.Id == pId).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetFatherList(list, t.PId)));
        }

        public List<Organization> GetSons(string merchantId, string id)
        {
            var sysOrganizations = CurrentDb.Organization.Where(m => m.MerchantId == merchantId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();
            var sysOrganization = sysOrganizations.Where(p => p.Id == id).ToList();
            var list2 = sysOrganization.Concat(GetSonList(sysOrganizations, id));
            return list2.ToList();
        }

        private IEnumerable<Organization> GetSonList(IList<Organization> list, string pId)
        {
            var query = list.Where(p => p.PId == pId).OrderBy(m => m.Priority).ToList();
            return query.ToList().Concat(query.ToList().SelectMany(t => GetSonList(list, t.Id)));
        }

        public CustomJsonResult GetDetails(string operater, string merchantId, string id)
        {
            var ret = new RetOrganizationGetDetails();
            var organization = CurrentDb.Organization.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();

            if (organization == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "操作失败", ret);
            }

            if (organization != null)
            {
                ret.Id = organization.Id ?? "";
                ret.Name = organization.Name ?? "";
                ret.Description = organization.Description ?? "";
                ret.Status = organization.Status;
                var header = CurrentDb.SysMerchantUser.Where(m => m.Id == organization.HeaderId).FirstOrDefault();
                if (header != null)
                {
                    ret.HeaderId = header.Id;
                    ret.HeaderUserName = header.UserName;
                    ret.HeaderFullName = header.FullName;
                }
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }


        public CustomJsonResult Add(string operater, string merchantId, RopOrganizationAdd rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var fathters = GetFathers(merchantId, rop.PId);

                if(fathters==null)
                {
                    LogUtil.Error("fathters is null");
                }

                int dept = fathters.Count;
                var isExists = CurrentDb.Organization.Where(m => m.MerchantId == merchantId && m.PId == rop.PId && m.Name == rop.Name && m.Dept == dept && m.IsDelete == false).FirstOrDefault();
                if (isExists != null)
                {
                    return new CustomJsonResult(ResultType.Failure, "该名称在同一级别已经存在");
                }

                if (dept > 2)
                {
                    return new CustomJsonResult(ResultType.Failure, "目前机构级别，仅支持三级");
                }

                string fullName = "";
                foreach (var item in fathters)
                {
                    fullName += item.Name + "-";
                }

                fullName += rop.Name;

                var organization = new Organization();
                organization.Id = GuidUtil.New();
                organization.MerchantId = merchantId;
                organization.PId = rop.PId;
                organization.Name = rop.Name;
                organization.FullName = fullName;
                organization.Description = rop.Description;
                organization.Status = Enumeration.OrganizationStatus.Valid;
                organization.Dept = dept;
                //organization.HeaderId = rop.HeaderId;
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
            using (TransactionScope ts = new TransactionScope())
            {
                var organization = CurrentDb.Organization.Where(m => m.MerchantId == merchantId && m.Id == rop.Id).FirstOrDefault();

                if (organization == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }

                var fathters = GetFathers(merchantId, organization.PId);
                int dept = fathters.Count;
                var isExists = CurrentDb.Organization.Where(m => m.MerchantId == merchantId && m.PId == organization.PId && m.Name == rop.Name && m.Dept == dept && m.Id != rop.Id && m.IsDelete == false).FirstOrDefault();
                if (isExists != null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("保存失败，该名称({0})已被同一级别使用", rop.Name));
                }

                string fullName = "";
                foreach (var item in fathters)
                {
                    fullName += item.Name + "-";
                }

                fullName += rop.Name;


                var sons = GetSons(merchantId, rop.Id);

                foreach (var item in sons)
                {
                    var l_fathters = GetFathers(merchantId, item.Id);

                    string l_fullName = "";
                    foreach (var l_fathter in l_fathters)
                    {
                        l_fullName += l_fathter.Name + "-";
                    }

                    if (!string.IsNullOrEmpty(l_fullName))
                    {
                        l_fullName = l_fullName.Substring(0, l_fullName.Length - 1);
                    }

                    item.FullName = l_fullName;

                }


                organization.Name = rop.Name;
                organization.FullName = fullName;
                organization.Dept = dept;
                organization.Status = rop.Status;
                organization.Description = rop.Description;
                //organization.HeaderId = rop.HeaderId;
                organization.Mender = operater;
                organization.MendTime = DateTime.Now;
                CurrentDb.SaveChanges();
                ts.Complete();
                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
            }

        }

        public CustomJsonResult Delete(string operater, string merchantId, string id)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var organization = CurrentDb.Organization.Where(m => m.Id == id).FirstOrDefault();

                if (organization == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "数据为空");
                }

                if (organization.Dept == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("所选机构（{0}）不允许删除", organization.Name));
                }


                var sons = GetSons(merchantId, id).ToList();

                foreach (var item in sons)
                {

                    var organizationUserCount = CurrentDb.SysMerchantUser.Where(m => m.OrganizationId == item.Id).Count();

                    if (organizationUserCount > 0)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该机构及子机构存在关联用户，若删除，请先将所用户关联移除");
                    }

                    item.IsDelete = true;
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功"); ;
            }

            return result;
        }

        public CustomJsonResult EditSort(string operater, string merchantId, RopOrganizationEditSort rop)
        {
            if (rop != null)
            {
                if (rop.Dics != null)
                {
                    foreach (var item in rop.Dics)
                    {
                        string id = item.Id;
                        int priority = item.Priority;
                        var organization = CurrentDb.Organization.Where(m => m.MerchantId == merchantId && m.Id == id).FirstOrDefault();
                        if (organization != null)
                        {
                            organization.Priority = priority;
                            CurrentDb.SaveChanges();
                        }
                    }
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");

        }

    }
}
