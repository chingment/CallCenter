using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public class GetRecordListRequestResult
    {
        public string Seq { get; set; }

        public Response Response { get; set; }

        public List<RecordModel> Record { get; set; }


        public class RecordModel
        {

            public string Key { get; set; }
            public string Caller { get; set; }
            public string Callee { get; set; }
            public string Filename { get; set; }
            public string Type { get; set; }
            public string Agent { get; set; }
            public string StartTime { get; set; }
            public string Duration { get; set; }
        }
    }
}
