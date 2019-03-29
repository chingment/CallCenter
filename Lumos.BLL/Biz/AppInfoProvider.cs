using Lumos.Entity;
using Lumos.WeiXinSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Biz
{
    public class AppInfoProvider : BaseProvider
    {
        public string GetSecret(string pAppId)
        {
            var appInfo = CurrentDb.SysAppInfo.Where(m => m.AppId == pAppId).FirstOrDefault();
            if (appInfo == null)
                return null;

            return appInfo.AppSecret;
        }

        public WxAppInfoConfig Get(string pAppId)
        {
            var sysAppInfo = CurrentDb.SysAppInfo.Where(m => m.AppId == pAppId).FirstOrDefault();
            if (sysAppInfo == null)
                return null;

            var appInfo = new WxAppInfoConfig();
            appInfo.AppId = sysAppInfo.AppId;
            appInfo.AppSecret = sysAppInfo.AppSecret;
            appInfo.PayMchId = sysAppInfo.AppWxPayMchId;
            appInfo.PayKey = sysAppInfo.AppWxPayKey;
            appInfo.PayResultNotifyUrl = sysAppInfo.AppWxPayResultNotifyUrl;
            appInfo.Oauth2RedirectUrl = sysAppInfo.AppWxOauth2RedirectUrl;
            appInfo.NotifyEventUrlToken = sysAppInfo.AppWxNotifyEventUrlToken;
            return appInfo;
        }


    }
}
