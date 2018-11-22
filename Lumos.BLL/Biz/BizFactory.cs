using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Biz
{
    public class BizFactory
    {
        public static SmsProvider Sms
        {
            get
            {
                return new SmsProvider();
            }
        }
        public static WxUserProvider WxUser
        {
            get
            {
                return new WxUserProvider();
            }
        }

        public static AppInfoProvider AppInfo
        {
            get
            {
                return new AppInfoProvider();
            }
        }

    }
}
