using System.Web.Mvc;
using System.Linq;
using Lumos.BLL;
using Lumos;
using System.Collections.Generic;
using Lumos.BLL.Service.Admin;
using Lumos.Entity;
using Lumos.BLL.Service.Merch;
using Lumos.Common;
using System;
using Lumos.Redis;

namespace WebMerch.Controllers
{
    public class HomeController : OwnBaseController
    {

        public ViewResult Index()
        {
            var incr = RedisManager.Db.StringIncrement("textIncr", 1);
            //MerchServiceFactory.User.SetLastAccessTime(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, DateTime.Now);

            //SdkFactory.Lxt.GetRecordList("0");
            //SdkFactory.Lxt.Logout("1308000");

            //RedisManager.Db.HashSetAsync(key, d.Id, Newtonsoft.Json.JsonConvert.SerializeObject(d), StackExchange.Redis.When.Always);

            //HttpUtil http = new HttpUtil();
            //string respon_data4 = http.HttpPostJson("http://112.74.179.185:8085/Fn/Notify", "dsdaddd", null);

            //BizFactory.ProductSku.InitSearchCache();


            //var tran = RedisManager.Db.CreateTransaction();

            //var productSkus = CurrentDb.ProductSku.Where(m => m.BarCode != null).ToList();

            //foreach (var item in productSkus)
            //{

            //    tran.HashSetAsync("h_productSkus", "barcode:" + item.BarCode + ",name:" + item.Name, Newtonsoft.Json.JsonConvert.SerializeObject(item));
            //    tran.SortedSetIncrementAsync("productSkus", item.BarCode, 1);
            //}
            //tran.ExecuteAsync();

            //string key = "85";
            //string key2 = "绿力";
            //var res = RedisManager.Db.Sort("productSkus", 0, -1);
            //var res2 = RedisManager.Db.HashGetAll("h_productSkus");



            //var list = (from i in res select i.ToString()).Where(x => x.Contains(key)).Take(10).ToList();

            //var list3 = (from i in res2 select i).Where(x => x.Name.ToString().Contains(key)).Take(10).ToList();
            //var list4 = (from i in res2 select i).Where(x => x.Name.ToString().Contains(key2)).Take(10).ToList();

            //var res3 = RedisManager.Db.HashValues("h_productSkus");

            //List<ProductSku> p = new List<ProductSku>();
            //if (res3 != null && res3.Length > 0)
            //{
            //    foreach (var item in res3)
            //    {
            //        var value = Newtonsoft.Json.JsonConvert.DeserializeObject<ProductSku>(item);
            //        p.Add(value);
            //    }
            //}

            //var list2 = (from i in p select i).Where(x => x.BarCode.Contains(key)).Take(10).ToList();

            return View();
        }

        public ViewResult Main()
        {
            //MerchServiceFactory.ObCallout.Get(this.CurrentUserId, this.CurrentMerchantId, "8f9e6852859d495ca16468c7f3f92c7a");
            return View();
        }

        public ViewResult ChangePassword()
        {
            return View();
        }
        public ViewResult PersonalInfo()
        {
            return View();
        }

        [HttpPost]
        public CustomJsonResult LogOff()
        {
            OwnRequest.Quit();
            var ret = new { url = OwnWebSettingUtils.GetLoginPage("") };
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "退出成功", ret);
        }

        [HttpPost]
        public CustomJsonResult ChangePassword(RopChangePassword rop)
        {
            var result = AdminServiceFactory.AuthorizeRelay.ChangePassword(this.CurrentUserId, this.CurrentUserId, rop.OldPassword, rop.NewPassword1, rop.NewPassword2);

            if (result.Result != ResultType.Success)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, result.Message);
            }

            OwnRequest.Quit();

            return Json(ResultType.Success, "点击<a href=\"" + OwnWebSettingUtils.GetLoginPage("") + "\">登录</a>");

        }

        public CustomJsonResult GetIndexData()
        {
            var ret = new IndexModel();

            ret.Title = OwnWebSettingUtils.GetWebName();
            ret.IsLogin = OwnRequest.IsLogin();

            if (ret.IsLogin)
            {
                ret.UserName = OwnRequest.GetUserNameWithSymbol();

                var merchant = CurrentDb.Merchant.Where(m => m.Id == this.CurrentMerchantId).FirstOrDefault();

                string businessType = "";
                switch (merchant.BusinessType)
                {
                    case Enumeration.BusinessType.CarIns:
                        businessType = "CarIns";
                        break;
                    case Enumeration.BusinessType.Common:
                        businessType = "Common";
                        break;
                }

                var merchantUser = CurrentDb.SysMerchantUser.Where(m => m.Id == this.CurrentUserId).FirstOrDefault();
                if (merchantUser != null)
                {
                    var menus = (from menu in CurrentDb.SysMenu where (from rolemenu in CurrentDb.SysRoleMenu where (from sysPositionRole in CurrentDb.SysPositionRole where sysPositionRole.PositionId == merchantUser.PositionId select sysPositionRole.RoleId).Contains(rolemenu.RoleId) select rolemenu.MenuId).Contains(menu.Id) && menu.BelongSite == Enumeration.BelongSite.Merchant && (menu.BusinessType == Enumeration.BusinessType.Unknow || menu.BusinessType == merchant.BusinessType) select menu).OrderBy(m => m.Priority).ToList();

                    var menuLevel1 = from c in menus where c.Dept == 1 select c;

                    foreach (var menuLevel1Child in menuLevel1)
                    {
                        var menuModel1 = new IndexModel.MenuModel();

                        menuModel1.Name = menuLevel1Child.Name;


                        var menuLevel2 = from c in menus where c.PId == menuLevel1Child.Id select c;

                        foreach (var menuLevel2Child in menuLevel2)
                        {
                            string url = menuLevel2Child.Url == null ? "" : menuLevel2Child.Url.Replace("{businessType}", businessType);

                            menuModel1.SubMenus.Add(new IndexModel.SubMenuModel { Name = menuLevel2Child.Name, Url = url });
                        }

                        ret.MenuNavByLeft.Add(menuModel1);
                    }
                }
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", ret);
        }

        public CustomJsonResult RunHeartbeatPacket()
        {
            return MerchServiceFactory.User.RunHeartbeatPacket(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId);
        }

        public CustomJsonResult GetPersonalInfo()
        {
            var model = MerchServiceFactory.User.GetPersonalInfo(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId);

            if (string.IsNullOrEmpty(model.TeleSeatAccount))
            {
                if (Request.Cookies["teleSeatAccount"] != null)
                {
                    model.TeleSeatAccount = Request.Cookies["teleSeatAccount"].Value.ToString();
                    model.TeleSeatPassword = Request.Cookies["teleSeatPassword"].Value.ToString();

                    var teleSeat = CurrentDb.TeleSeat.Where(m => m.Account == model.TeleSeatAccount).FirstOrDefault();
                    if (teleSeat != null)
                    {
                        model.TeleSeatDomain = teleSeat.Domain;
                    }
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", model);
        }

        public class IndexModel
        {
            public IndexModel()
            {
                this.MenuNavByLeft = new List<MenuModel>();
            }

            public string Title { get; set; }

            public bool IsLogin { get; set; }

            public string UserName { get; set; }

            public List<MenuModel> MenuNavByLeft { get; set; }

            public class MenuModel
            {
                public MenuModel()
                {
                    this.SubMenus = new List<SubMenuModel>();
                }

                public string Name { get; set; }

                public List<SubMenuModel> SubMenus { get; set; }
            }

            public class SubMenuModel
            {
                public string Url { get; set; }

                public string Name { get; set; }
            }
        }
    }
}