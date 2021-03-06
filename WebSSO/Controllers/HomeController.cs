﻿using Lumos.Common;
using Lumos.Entity;
using Lumos.Web.Mvc;
using Lumos.BLL;
using System.Web.Mvc;
using Lumos.Session;
using System;
using Lumos;
using log4net;
using System.Reflection;
using Lumos.Redis;
using Lumos.Web;
using Lumos.BLL.Biz;
using Lumos.BLL.Service.Admin;
using System.Web;
using System.Linq;


namespace WebSSO.Controllers
{

    public class HomeController : OwnBaseController
    {
        public readonly string sesionKeyLoginVerifyCode = "sesionKeyLoginVerifyCode";
        [AllowAnonymous]
        public ActionResult Login()
        {
            Session[sesionKeyLoginVerifyCode] = null;
            return View();
        }

        public ActionResult Set()
        {
            return View();
        }

        /// <summary>
        /// 登录方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public CustomJsonResult Login(RopLogin rop)
        {
            RetLogin ret = new RetLogin();

            if (Session[sesionKeyLoginVerifyCode] == null)
            {
                return Json(ResultType.Failure, ret, "验证码超时");
            }

            if (Session[sesionKeyLoginVerifyCode].ToString() != rop.VerifyCode)
            {
                return Json(ResultType.Failure, ret, "验证码不正确");
            }

            var result = AdminServiceFactory.AuthorizeRelay.SignIn(rop.UserName, rop.Password, CommonUtil.GetIP(), Enumeration.LoginType.Website);

            if (result.ResultType == Enumeration.LoginResult.Failure)
            {

                if (result.ResultTip == Enumeration.LoginResultTip.UserNotExist || result.ResultTip == Enumeration.LoginResultTip.UserPasswordIncorrect)
                {
                    return Json(ResultType.Failure, ret, "用户名或密码不正确");
                }

                if (result.ResultTip == Enumeration.LoginResultTip.UserDisabled)
                {
                    return Json(ResultType.Failure, ret, "账户被禁用");
                }

                if (result.ResultTip == Enumeration.LoginResultTip.UserDeleted)
                {
                    return Json(ResultType.Failure, ret, "账户被删除");
                }
            }

            string host = "";
            string returnUrl = "";

            var _this = this;
            switch (result.User.BelongSite)
            {
                case Enumeration.BelongSite.Admin:
                    host = System.Configuration.ConfigurationManager.AppSettings["custom:WebAdminUrl"];
                    //returnUrl = string.Format("{0}?returnUrl={1}", host, model.ReturnUrl);
                    returnUrl = string.Format("{0}", host);
                    break;
                case Enumeration.BelongSite.Merchant:
                    host = System.Configuration.ConfigurationManager.AppSettings["custom:WebMerchUrl"];
                    //returnUrl = string.Format("{0}?returnUrl={1}", host, model.ReturnUrl);
                    returnUrl = string.Format("{0}", host);
                    break;
            }


            UserInfo userInfo = new UserInfo();
            userInfo.UserId = result.User.UserId;
            userInfo.UserName = result.User.UserName;
            userInfo.MerchantId = result.User.MerchantId;

            string token = GuidUtil.New();
            SSOUtil.SetUserInfo(token, userInfo);

            ret.Url = string.Format("{0}?token={1}", returnUrl, token);

            return Json(ResultType.Success, ret, "登录成功");

        }


        [AllowAnonymous]
        public ActionResult GetImgVerifyCode(string name)
        {
            VerifyCodeHelper v = new VerifyCodeHelper();
            v.CodeSerial = "0,1,2,3,4,5,6,7,8,9";
            string code = v.CreateVerifyCode(); //取随机码 
            v.CreateImageOnPage(code, ControllerContext.HttpContext);   //输出图片
            Session[name] = code;   //Session 取出验证码
            Response.End();
            return null;
        }

        [HttpPost]
        [AllowAnonymous]
        public CustomJsonResult Set(string simpleCode, string account, string password)
        {

            var merchant = CurrentDb.Merchant.Where(m => m.SimpleCode == simpleCode).FirstOrDefault();
            if (merchant == null)
            {
                return Json(ResultType.Failure, "更新失败，不存在该商户");

            }

            var teleSeat = CurrentDb.TeleSeat.Where(m => m.Account == account && m.MerchantId == merchant.Id).FirstOrDefault();
            if (teleSeat == null)
            {
                return Json(ResultType.Failure, "更新失败，不存在该话机账号");
            }

            if (teleSeat.Password != password)
            {
                return Json(ResultType.Failure, "更新失败，话机账号对应的密码不正确");
            }

            this.Response.Cookies.Add(new HttpCookie("teleSeatSimpleCode", simpleCode));
            this.Response.Cookies.Add(new HttpCookie("teleSeatAccount", account));
            this.Response.Cookies.Add(new HttpCookie("teleSeatPassword", password));

            return Json(ResultType.Success, "更新成功");
        }

    }
}