﻿using Lumos.BLL;
using Lumos.BLL.Service.Admin;
using Lumos.BLL.Service.Merch;
using Lumos.Entity;
using Lumos.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMerch
{
    public static class OwnRequest
    {
        public const string SESSION_NAME = "WebMerch_SessionName";

        public static string GetCurrentUserId()
        {
            var userInfo = GetUserInfo();
            if (userInfo == null)
                return "0";

            return userInfo.UserId;
        }

        public static string GetCurrentMerchantId()
        {
            var userInfo = GetUserInfo();
            if (userInfo == null)
                return "0";

            return userInfo.MerchantId;
        }

        public static UserInfo GetUserInfo()
        {

            UserInfo userInfo = SSOUtil.GetUserInfo(GetAccessToken());

            return userInfo;
        }

        public static string GetAccessToken()
        {
            var context = HttpContext.Current;
            var request = context.Request;
            var response = context.Response;

            var token = request.Cookies[OwnRequest.SESSION_NAME];
            if (token == null)
                return null;

            return token.Value;
        }

        public static bool IsLogin()
        {
            if (GetUserInfo() == null)
                return false;

            return true;
        }

        public static string GetUserNameWithSymbol()
        {
            var userInfo = GetUserInfo();
            if (userInfo == null)
                return "";

            string userName = userInfo.UserName;

            char[] c = userName.ToCharArray();
            string s1 = "";
            string s2 = "";
            if (userName.Length > 4)
            {
                s1 = userName.Substring(0, 2);
                s2 = userName.Substring(userName.Length - 2, 2);
                userName = s1 + "***" + s2;
            }

            return userName;
        }

        public static bool IsInMenu(string url)
        {

            return true;
        }

        public static bool IsInPermission(string[] permissions)
        {
            var userInfo = GetUserInfo();
            if (userInfo == null)
                return false;

            List<string> listPermissions = MerchServiceFactory.User.GetPermissions(userInfo.UserId, userInfo.MerchantId, userInfo.UserId);
            if (listPermissions == null)
                return false;
            if (listPermissions.Count < 1)
                return false;

            bool isHas = false;
            foreach (var permission in listPermissions)
            {
                foreach (var m in permissions)
                {
                    if (permission.Trim() == m.Trim())
                    {
                        isHas = true;
                        break;
                    }
                }
                if (isHas)
                {
                    break;
                }
            }

            return isHas;
        }

        public static void Postpone()
        {
            var userInfo = GetUserInfo();
            if (userInfo != null)
            {
                SSOUtil.Postpone(GetAccessToken());
            }

        }

        public static void Quit()
        {

            SSOUtil.Quit(GetAccessToken());

            var context = HttpContext.Current;
            var request = context.Request;
            var response = context.Response;
            HttpCookie cookie_session = request.Cookies[OwnRequest.SESSION_NAME];
            if (cookie_session != null)
            {
                TimeSpan ts = new TimeSpan(-1, 0, 0, 0);
                cookie_session.Expires = DateTime.Now.Add(ts);
                response.AppendCookie(cookie_session);
            }



        }
    }
}