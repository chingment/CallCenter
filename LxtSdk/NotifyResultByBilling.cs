using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public class NotifyResultByBilling
    {
        public NotifyResultByBilling()
        {
            this.Authentication = new AuthenticationModel();
            this.Notify = new NotifyContent();
        }

        public AuthenticationModel Authentication { get; set; }

        public NotifyContent Notify { get; set; }
        public class NotifyContent
        {


            public string Type { get; set; }
            public string StartTime { get; set; }
            public string RingTime { get; set; }
            public string AnswerTime { get; set; }
            public string ByeTime { get; set; }
            public string StaffNo { get; set; }
            public string Callee { get; set; }
            public string Caller { get; set; }
            public string RecordFile { get; set; }
            public int Service { get; set; }
            public string Session { get; set; }
            public string Seq { get; set; }
            public string UserData { get; set; }
            public int TimeLength { get; set; }
        }
    }
}
