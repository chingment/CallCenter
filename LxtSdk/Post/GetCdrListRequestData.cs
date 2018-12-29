using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public class GetCdrListRequestData
    {
        public List<string> Agent { get; set; }
        public string Seq { get; set; }
        public string UserData { get; set; }

        public string ServerType { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Caller { get; set; }
        public string Callee { get; set; }
        public string TimeLengthMin { get; set; }
        public string TimeLengthMax { get; set; }
    }
}
