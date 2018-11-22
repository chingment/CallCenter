using Lumos.DAL.AuthorizeRelay;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public class BizMenuProvider : BaseProvider
    {
        public List<Permission> GetPermissionList(BizPermissionCode pPermissionCode)
        {
            Type t = pPermissionCode.GetType();
            List<Permission> list = new List<Permission>();
            //SysPermission p = new SysPermission();
            //p.Id = "0";
            //p.Name = "权限集合";
            //p.PId = "";
            //list.Add(p);
            list = GetBasePermissionList(t, list);
            return list;
        }

        private List<Permission> GetBasePermissionList(Type pType, List<Permission> pSysPermissionList)
        {
            if (pType.Name != "Object")
            {
                System.Reflection.FieldInfo[] properties = pType.GetFields();
                foreach (System.Reflection.FieldInfo property in properties)
                {
                    string pId = "0";
                    object[] typeAttributes = property.GetCustomAttributes(false);
                    foreach (PermissionCodeAttribute attribute in typeAttributes)
                    {
                        pId = attribute.PId;
                    }
                    object id = property.GetValue(null);
                    string name = property.Name;
                    Permission model = new Permission();
                    model.Id = id.ToString();
                    model.Name = name;
                    pSysPermissionList.Add(model);
                }
                pSysPermissionList = GetBasePermissionList(pType.BaseType, pSysPermissionList);
            }
            return pSysPermissionList;
        }

        public CustomJsonResult GetPermissions(string pOperater)
        {
            var ret = new RetBizMenuGetPermissions();

            ret.Permissions = GetPermissionList(new BizPermissionCode());

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult GetDetails(string pOperater, string pMenuId)
        {
            var ret = new RetBizMenuGetDetails();

            var menu = CurrentDb.BizMenu.Where(m => m.Id == pMenuId).FirstOrDefault();

            ret.MenuId = menu.Id;
            ret.Name = menu.Name;
            ret.Url = menu.Url;
            ret.Description = menu.Description;
            ret.BusinessType = menu.BusinessType;

            var menuPermission = CurrentDb.BizMenuPermission.Where(u => u.MenuId == pMenuId).ToList();
            var permissionIdIds = (from p in menuPermission select p.PermissionId).ToArray();

            ret.PermissionIds = permissionIdIds;

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult Add(string pOperater, RopBizMenuAdd rop)
        {
            var menu = new BizMenu();
            menu.Id = GuidUtil.New();
            menu.Name = rop.Name;
            menu.Url = rop.Url;
            menu.Description = rop.Description;
            menu.PId = rop.PMenuId;
            menu.IsCanDelete = true;
            menu.BusinessType = rop.BusinessType;
            menu.Creator = pOperater;
            menu.CreateTime = DateTime.Now;
            CurrentDb.BizMenu.Add(menu);
            CurrentDb.SaveChanges();

            if (rop.PermissionIds != null)
            {
                foreach (var id in rop.PermissionIds)
                {
                    CurrentDb.BizMenuPermission.Add(new BizMenuPermission { Id = GuidUtil.New(), MenuId = menu.Id, PermissionId = id, Creator = pOperater, CreateTime = DateTime.Now });
                }
            }

            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "添加成功");

        }


        public CustomJsonResult Edit(string pOperater, RopBizMenuEdit rop)
        {

            var menu = CurrentDb.BizMenu.Where(m => m.Id == rop.MenuId).FirstOrDefault();

            menu.Name = rop.Name;
            menu.Url = rop.Url;
            menu.Description = rop.Description;
            menu.BusinessType = rop.BusinessType;
            menu.Mender = pOperater;
            menu.MendTime = DateTime.Now;

            var menuPermission = CurrentDb.BizMenuPermission.Where(r => r.MenuId == rop.MenuId).ToList();
            foreach (var m in menuPermission)
            {
                CurrentDb.BizMenuPermission.Remove(m);
            }


            if (rop.PermissionIds != null)
            {
                foreach (var id in rop.PermissionIds)
                {
                    CurrentDb.BizMenuPermission.Add(new BizMenuPermission { Id = GuidUtil.New(), MenuId = menu.Id, PermissionId = id, Creator = pOperater, CreateTime = DateTime.Now });
                }
            }

            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");


        }


        public CustomJsonResult Delete(string pOperater, string[] pMenuIds)
        {
            if (pMenuIds != null)
            {
                foreach (var id in pMenuIds)
                {
                    var menu = CurrentDb.BizMenu.Where(m => m.Id == id).FirstOrDefault();

                    if (!menu.IsCanDelete)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("所选菜单（{0}）不允许删除", menu.Name));
                    }

                    CurrentDb.BizMenu.Remove(menu);

                    var positionMenus = CurrentDb.BizPositionMenu.Where(r => r.MenuId == id).ToList();
                    foreach (var positionMenu in positionMenus)
                    {
                        CurrentDb.BizPositionMenu.Remove(positionMenu);
                    }

                    CurrentDb.SaveChanges();

                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "删除成功");
        }

        public CustomJsonResult EditSort(string pOperater, RopBizMenuEditSort rop)
        {
            if (rop != null)
            {
                if (rop.Dics != null)
                {
                    foreach (var item in rop.Dics)
                    {
                        string menuId = item.MenuId;
                        int priority = item.Priority;
                        var model = CurrentDb.BizMenu.Where(m => m.Id == menuId).FirstOrDefault();
                        if (model != null)
                        {
                            model.Priority = priority;
                            CurrentDb.SaveChanges();
                        }
                    }
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");

        }
    }
}
