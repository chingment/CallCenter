using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public class AgentQueryStatusRequest : IApiPostRequest<AgentQueryStatusRequestResult>
    {

        public AgentQueryStatusRequest(AgentQueryStatusRequestData postdata)
        {
            this.PostData = postdata;
        }


        public Object PostData { get; set; }

        public string ApiUrl
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/media/uploadnews";
            }
        }

    }
}
