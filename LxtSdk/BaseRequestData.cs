using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public class AuthenticationModel
    {

        public string Customer { get; set; }
        public string Agent { get; set; }
        public string Digest { get; set; }
        public string Seq { get; set; }
        public string Timestamp { get; set; }
    }

    public class BaseRequestData
    {
        public BaseRequestData()
        {
            this.Authentication = new AuthenticationModel();
            this.Param = new ParamModel();
        }
        public AuthenticationModel Authentication { get; set; }

        public ParamModel Param { get; set; }

        public Object Request { get; set; }


        public class ParamModel
        {
            public string Debug { get; set; }
            public string Lang { get; set; }
        }
    }
}
