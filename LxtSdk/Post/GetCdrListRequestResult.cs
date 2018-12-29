using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
   public class GetCdrListRequestResult
    {
        public string Seq { get; set; }

        public ResponseModel Response { get; set; }


        public class ResponseModel
        {
            public ResponseModel()
            {

            }

            public string Seq { get; set; }

            public string UserData { get; set; }

            public Result Result { get; set; }

            public string Total { get; set; }
            public List<CdrModel> Cdr { get; set; }
        }

        public class CdrModel
        {
            public string Key { get; set; }
            public string Agent { get; set; }
            public string ServiceType { get; set; }
        }
    }
}
