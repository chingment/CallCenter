using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{

    public class NotifyResultByCallEnd
    {
        public NotifyResultByCallEnd()
        {
            this.Authentication = new AuthenticationModel();
            this.Notify = new NotifyContent();
        }

        public AuthenticationModel Authentication { get; set; }


        public NotifyContent Notify { get; set; }
        public class NotifyContent
        {


            public string Type { get; set; }
            public string Customer { get; set; }
            public string Agent { get; set; }
            public string Number { get; set; }
            public int Call_type { get; set; }
            public string Seq { get; set; }
            public string UserData { get; set; }
        }

     
    }
}
