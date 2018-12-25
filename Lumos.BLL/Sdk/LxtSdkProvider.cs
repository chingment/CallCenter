using LxtSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL
{
    public class LxtSdkProvider : ITelephoneControlSdk
    {
        private LxtApi _api = new LxtApi();

        public CustomJsonResult CallNumber(string account, string seq, string csrPhoneNumber)
        {
            CustomJsonResult result = new CustomJsonResult();

            var requestData = new CallNumberRequestData();
            requestData.Agent = account;
            requestData.Seq = seq;
            requestData.UserData = seq;
            requestData.Callee = csrPhoneNumber;

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
        public TelephoneStatus GetStatus(string account)
        {
            TelephoneStatus telephoneStatus = TelephoneStatus.Unknow;

            var requestData = new GetAgentStatusRequestData();
            requestData.Agent = account;
            requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");
            requestData.UserData = "";

            var request = new GetAgentStatusRequest(requestData);

            var requestResult = _api.DoPost(request);

            if (requestResult == null)
            {
                return telephoneStatus;
            }

            if (requestResult.Data == null)
            {
                return telephoneStatus;
            }

            if (requestResult.Data.Response == null)
            {
                return telephoneStatus;
            }

            switch (requestResult.Data.Response.ServerStatus)
            {
                case "IDLE":
                    telephoneStatus = TelephoneStatus.IDLE;
                    break;
                case "CALL_OUT":
                    telephoneStatus = TelephoneStatus.CallOut;
                    break;
                case "CALL_IN":
                    telephoneStatus = TelephoneStatus.CallIn;
                    break;
                case "RINGING":
                    telephoneStatus = TelephoneStatus.Ringing;
                    break;
                case "PROCESS":
                    telephoneStatus = TelephoneStatus.Process;
                    break;
                default:
                    telephoneStatus = TelephoneStatus.Unknow;
                    break;
            }



            return telephoneStatus;
        }
    }
}
