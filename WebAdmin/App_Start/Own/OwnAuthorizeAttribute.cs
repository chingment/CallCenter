﻿using System;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Lumos;
using Lumos.Web;

namespace WebAdmin
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class OwnAuthorizeAttribute : ActionFilterAttribute
    {
        public string[] _permissions;

        public OwnAuthorizeAttribute(params string[] permissions)
        {
            if (permissions != null)
            {
                if (permissions.Length > 0)
                {
                    _permissions = permissions;
                }
            }

        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            if (skipAuthorization)
            {
                return;
            }

            var request = filterContext.RequestContext.HttpContext.Request;
            var response = filterContext.RequestContext.HttpContext.Response;
            bool isAjaxRequest = request.IsAjaxRequest();
            string userAgent = request.UserAgent;
            string returnUrl = "";

            if (isAjaxRequest)
            {
                if (request.UrlReferrer != null)
                {
                    returnUrl = request.UrlReferrer.AbsoluteUri;
                    //LogUtil.Info("request.UrlReferrer.AbsoluteUri:" + request.UrlReferrer.AbsoluteUri);
                }
                else
                {
                    returnUrl = request.Url.AbsoluteUri;
                    //LogUtil.Info("request.Url.AbsoluteUri:" + request.Url.AbsoluteUri);
                }
            }

            //LogUtil.Info("returnUrl:" + returnUrl);


            if (request.HttpMethod == "POST")
            {
                if (request.IsAjaxRequest())
                {
                    var antiForgeryCookie = request.Cookies[AntiForgeryConfig.CookieName];

                    var cookieValue = antiForgeryCookie != null
                        ? antiForgeryCookie.Value
                        : null;
                    //从cookies 和 Headers 中 验证防伪标记
                    //这里可以加try-catch
                    AntiForgery.Validate(cookieValue, request.Headers["__RequestVerificationToken"]);
                }
            }


            string token = request.QueryString["token"];

            if (token != null)
            {
                HttpCookie cookie_session = request.Cookies[OwnRequest.SESSION_NAME];
                if (cookie_session != null)
                {
                    cookie_session.Value = token;
                    response.AppendCookie(cookie_session);
                }
                else
                {
                    response.Cookies.Add(new HttpCookie(OwnRequest.SESSION_NAME, token));
                }
            }




            var userInfo = OwnRequest.GetUserInfo();

            if (userInfo == null)
            {
                //LogUtil.Info("userInfo: is null");

                MessageBox messageBox = new MessageBox();
                messageBox.No = Guid.NewGuid().ToString();
                messageBox.Type = MessageBoxTip.Failure;
                messageBox.Title = "温馨提示";
                messageBox.GoToUrl = OwnWebSettingUtils.GetLoginPage(returnUrl);
                messageBox.Content = "请先<a href=\"javascript:void(0)\" onclick=\"window.top.location.href='" + OwnWebSettingUtils.GetLoginPage(returnUrl) + "'\">登录</a>后打开";
                messageBox.IsTop = true;

                if (isAjaxRequest)
                {
                    CustomJsonResult jsonResult = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, messageBox.Title, messageBox);
                    //jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    filterContext.Result = jsonResult;
                    filterContext.Result.ExecuteResult(filterContext);
                    filterContext.HttpContext.Response.End();
                }
                else
                {
                    filterContext.Result = new ViewResult { ViewName = "MessageBox", MasterName = "_Layout", ViewData = new ViewDataDictionary { Model = messageBox } };
                }

                return;
            }

            if (_permissions != null)
            {
                MessageBox messageBox = new MessageBox();
                messageBox.No = Guid.NewGuid().ToString();
                messageBox.Type = MessageBoxTip.Warn;
                messageBox.Title = "温馨提示";
                messageBox.Content = "您没有权限";

                bool isHasPermission = OwnRequest.IsInPermission(_permissions);

                if (!isHasPermission)
                {
                    //LogUtil.Info("userInfo isHasPermission : false");

                    if (isAjaxRequest)
                    {
                        CustomJsonResult jsonResult = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, messageBox.Title, messageBox);
                        //jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                        filterContext.Result = jsonResult;
                        filterContext.Result.ExecuteResult(filterContext);
                        filterContext.HttpContext.Response.End();
                    }
                    else
                    {
                        filterContext.Result = new ViewResult { ViewName = "MessageBox", MasterName = "_Layout", ViewData = new ViewDataDictionary { Model = messageBox } };
                    }

                    return;
                }
            }

            OwnRequest.Postpone();

            //LogUtil.Info("check over");

            base.OnActionExecuting(filterContext);
        }
    }

}