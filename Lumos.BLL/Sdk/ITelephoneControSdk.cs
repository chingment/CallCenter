using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public interface ITelephoneControSdk
    {
        CustomJsonResult CallNumber(string account, string csrId, string csrPhoneNumber);
        CustomJsonResult Handup();
        CustomJsonResult Login(string acccount);
        CustomJsonResult Logout();
        CustomJsonResult GetStatus(string account );
    }
}
