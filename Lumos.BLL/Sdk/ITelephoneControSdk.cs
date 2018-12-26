using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public enum TelephoneStatus
    {

        [Remark("未知")]
        Unknow = 0,
        [Remark("空闲")]
        IDLE = 1,
        [Remark("呼出接通")]
        CallOut = 2,
        [Remark("呼入接通")]
        CallIn = 3,
        [Remark("振铃")]
        Ringing = 4,
        [Remark("整理中")]
        Process = 5

    }
    public interface ITelephoneControlSdk
    {
        CustomJsonResult CallNumber(string account, string csrId, string csrPhoneNumber);
        CustomJsonResult Hangup(string account);
        CustomJsonResult Login(string acccount);
        CustomJsonResult Logout(string account);
        TelephoneStatus GetStatus(string account);
    }
}
