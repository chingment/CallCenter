using Lumos.BLL;
using Lumos.BLL.Service.Admin;
using Lumos.Entity;
using Lumos.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAdmin
{
    public static class OwnRequest
    {
        public const string SESSION_NAME = "WebAdmin_SessionName";


        public static string GetCurrentUserId()
        {
            var userInfo = GetUserInfo();
            if (userInfo == null)
                return "";

            return userInfo.UserId;
        }

        public static UserInfo GetUserInfo()
        {
            UserInfo userInfo = null;

            var context = HttpContext.Current;
            var request = context.Request;
            var response = context.Response;

            var token = request.Cookies[OwnRequest.SESSION_NAME];
            if (token == null)
                return null;

            userInfo = SSOUtil.GetUserInfo(token.Value);

            //userInfo = new UserInfo();
            //userInfo.Token = "1";
            //userInfo.UserId = 1000;
            //userInfo.UserName = "admin";

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
            if (userName.Length >= 5)
            {
                s1 = userName.Substring(0, 2);
                s2 = userName.Substring(userName.Length - 2, 2);
                userName = s1 + "***" + s2;
            }

            return userName;
        }


        public static bool IsInPermission(string[] permissions)
        {
            var userId = GetCurrentUserId();

            List<string> listPermissions = AdminServiceFactory.SysAdminUser.GetPermissions(userId, userId);
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

            SSOUtil.Postpone(GetAccessToken());


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