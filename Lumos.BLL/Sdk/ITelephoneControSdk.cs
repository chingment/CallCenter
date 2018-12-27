﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lumos.Entity.Enumeration;

namespace Lumos.BLL
{
    public interface ITelephoneControlSdk
    {
        CustomJsonResult CallNumber(string account, string csrId, string csrPhoneNumber);
        CustomJsonResult Hangup(string account);
        CustomJsonResult Login(string acccount);
        CustomJsonResult Logout(string account);
        TelSeatPhoneStatus GetStatus(string account);
    }
}
