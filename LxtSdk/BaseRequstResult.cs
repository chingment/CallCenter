using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public class BaseRequstResult<T>
    {

        public T Data { get; set; }

        public string Log { get; set; }

        public ResultModel Result { get; set; }

        public class ResultModel
        {
            public string Error { get; set; }
            public string Msg { get; set; }
        }
    }
}
