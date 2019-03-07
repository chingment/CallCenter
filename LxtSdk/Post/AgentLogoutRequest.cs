using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public class AgentLogoutRequest : IApiPostRequest<AgentLogoutRequestResult>
    {
        public AgentLogoutRequest(AgentLogoutRequestData postdata)
        {
            this.PostData = postdata;
        }

        public string Customer { get; set; }
        public string Password { get; set; }
        public Object PostData { get; set; }

        public string ApiUrl
        {
            get
            {
                return "http://39.108.86.40:80/openapi/V2.0.6/AgentLogout";
            }
        }
    }
}
