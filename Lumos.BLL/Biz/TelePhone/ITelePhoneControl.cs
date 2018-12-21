using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public interface ITelePhoneControl
    {
        CustomJsonResult CallNumber(string number);
        CustomJsonResult Handup();
        CustomJsonResult Login();
        CustomJsonResult Logout();
        CustomJsonResult QueryStatus();
    }
}
