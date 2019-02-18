using Lumos;
using Lumos.BLL.Service.Merch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class TelphoneControlController : OwnBaseController
    {

        public string Account
        {
            get
            {
                if (Request.Cookies["teleSeatAccount"] == null)
                    return null;
                return Request.Cookies["teleSeatAccount"].Value.ToString();
            }
        }
        [HttpPost]
        public CustomJsonResult Login()
        {
            return MerchServiceFactory.TelephoneControl.Login(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, this.Account);
        }

        [HttpPost]
        public CustomJsonResult Logout()
        {
            return MerchServiceFactory.TelephoneControl.Logout(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, this.Account);
        }

        [HttpPost]
        public CustomJsonResult CallCustomer(string customerId)
        {
            return MerchServiceFactory.TelephoneControl.CallCustomer(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, customerId, this.Account);
        }

        [HttpPost]
        public CustomJsonResult HangupCustomer(string customerId)
        {
            return MerchServiceFactory.TelephoneControl.HangupCustomer(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, customerId, this.Account);
        }
    }
}