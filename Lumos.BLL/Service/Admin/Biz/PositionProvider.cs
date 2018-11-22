﻿using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public class PositionProvider : BaseProvider
    {
        public CustomJsonResult GetDetails(string pOperater, string positionId)
        {
            var ret = new RetPositionGetDetails();

            var position = CurrentDb.Position.Where(m => m.Id == positionId).FirstOrDefault();

            ret.PositionId = position.Id;
            ret.Name = position.Name;
            ret.Description = position.Description;


            var positionMenus = from c in CurrentDb.BizMenu
                                where
                                    (from o in CurrentDb.PositionMenu where o.PositionId == positionId select o.MenuId).Contains(c.Id)
                                select c;



            var menuIds = (from p in positionMenus select p.Id).ToArray();

            ret.MenuIds = menuIds;

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public List<BizMenu> GetPositionMenus(string pOperater, string pPositionId)
        {
            var model = from c in CurrentDb.BizMenu
                        where
                            (from o in CurrentDb.PositionMenu where o.PositionId == pPositionId select o.MenuId).Contains(c.Id)
                        select c;

            return model.ToList();
        }

        public CustomJsonResult SavePositionMenu(string pOperater, string pPositionId, string[] pMenuIds)
        {
            var position = CurrentDb.Position.Where(m => m.Id == pPositionId).FirstOrDefault();

            List<PositionMenu> positionMenuList = CurrentDb.PositionMenu.Where(r => r.PositionId == pPositionId).ToList();

            foreach (var positionMenu in positionMenuList)
            {
                CurrentDb.PositionMenu.Remove(positionMenu);
            }


            if (pMenuIds != null)
            {
                foreach (var menuId in pMenuIds)
                {
                    CurrentDb.PositionMenu.Add(new PositionMenu { Id = GuidUtil.New(), PositionId = pPositionId, PositionType = position.Type, MenuId = menuId, Creator = pOperater, CreateTime = DateTime.Now });
                }
            }

            CurrentDb.SaveChanges();


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "保存成功");
        }

    }
}
