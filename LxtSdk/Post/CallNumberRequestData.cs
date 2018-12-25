using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{

    public class CallNumberRequestData
    {

        public CallNumberRequestData()
        {
       
        }

        public string Agent { get; set; }
        public string Seq { get; set; }
        public string UserData { get; set; }
        public string Callee { get; set; }


    }
}
