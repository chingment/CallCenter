using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos.Entity;

namespace Lumos.BLL
{
    public interface ITelephoneControlSdk
    {
        CustomJsonResult CallNumber(string customer, string password, string account, string csrId, string csrPhoneNumber);
        CustomJsonResult Hangup(string customer, string password, string account);
        CustomJsonResult Login(string customer, string password, string acccount);
        CustomJsonResult Logout(string customer, string password, string account);
        Enumeration.TelePhoneStatus GetStatus(string customer, string password, string account);
    }
}
