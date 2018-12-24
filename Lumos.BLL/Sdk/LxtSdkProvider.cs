using LxtSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public class LxtSdkProvider : ITelephoneControSdk
    {
        private LxtApi _api = new LxtApi();

        public CustomJsonResult CallNumber(string account,string callNumber)
        {
            CustomJsonResult result = new CustomJsonResult();

            var requestData = new CallNumberRequestData();
            requestData.Agent = account;
            requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");
            requestData.UserData = "";
            requestData.Callee = callNumber;

            var request = new CallNumberRequest(requestData);

            var requestResult = _api.DoPost(request);


            return result;
        }
        public CustomJsonResult Handup()
        {
            CustomJsonResult result = new CustomJsonResult();


            return result;
        }
        public CustomJsonResult Login(string account)
        {

            CustomJsonResult result = new CustomJsonResult();

            GetStatus(account);

            //var requestData = new AgentLoginRequestData();
            //requestData.Agent = account;
            //requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");
            //requestData.UserData = "";

            //var request = new AgentLoginRequest(requestData);

            //var requestResult = _api.DoPost(request);


            return result;
        }
        public CustomJsonResult Logout()
        {

            CustomJsonResult result = new CustomJsonResult();

            return result;
        }
        public CustomJsonResult GetStatus(string account)
        {
            CustomJsonResult result = new CustomJsonResult();

            var requestData = new GetAgentStatusRequestData();
            requestData.Agent = account;
            requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");
            requestData.UserData = "";

            var request = new GetAgentStatusRequest(requestData);

            var requestResult = _api.DoPost(request);


            return result;
        }
    }
}
