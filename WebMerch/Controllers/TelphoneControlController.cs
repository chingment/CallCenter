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
        [HttpPost]
        public CustomJsonResult Login()
        {
            return MerchServiceFactory.TelephoneControl.Login(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId);
        }

        [HttpPost]
        public CustomJsonResult CallCustomer(string customerId)
        {
            return MerchServiceFactory.TelephoneControl.CallCustomer(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, customerId);
        }

        [HttpPost]
        public CustomJsonResult HangupCustomer(string customerId)
        {
            return MerchServiceFactory.TelephoneControl.HangupCustomer(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, customerId);
        }
    }
}