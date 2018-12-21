using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public class Response
    {
        public string Seq { get; set; }

        public string UserData { get; set; }

        public Result Result { get; set; }
    }

    public class Result
    {
        public string Error { get; set; }

    }


    public class AgentQueryStatusRequestResult
    {
        public string Seq { get; set; }

        public Response Response { get; set; }


    }
}
