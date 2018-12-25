using Lumos;
using Lumos.BLL.Service.Merch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class FnController : Controller
    {

        [HttpPost]
        public CustomJsonResult Notify()
        {
            Stream stream = Request.InputStream;
            stream.Seek(0, SeekOrigin.Begin);
            string content = new StreamReader(stream).ReadToEnd();
            LogUtil.Info("接收通知内容:" + content);
            MerchServiceFactory.TelephoneControl.Notify(GuidUtil.New(), content);
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "通知成功", "");
        }
    }
}