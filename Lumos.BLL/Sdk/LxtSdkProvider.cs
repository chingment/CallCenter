using LxtSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos.Entity;

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
        public CustomJsonResult Hangup(string account)
        {
            CustomJsonResult result = new CustomJsonResult();

            var requestData = new HangupCallRequestData();
            requestData.Agent = account;
            requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");
            requestData.UserData = "";


            var request = new HangupCallRequest(requestData);

            var requestResult = _api.DoPost(request);


            return result;
        }
        public CustomJsonResult Login(string account)
        {

            CustomJsonResult result = new CustomJsonResult();

            var telephoneStatus = GetStatus(account);
            if (telephoneStatus == Enumeration.TelSeatPhoneStatus.Unknow)
            {
                var requestData = new AgentLoginRequestData();
                requestData.Agent = account;
                requestData.Seq = SnUtil.Build(Enumeration.BizSnType.TelphoneControlSeq, "");
                requestData.UserData = "";

                var request = new AgentLoginRequest(requestData);

                var requestResult = _api.DoPost(request);

            }

            return result;
        }
        public CustomJsonResult Logout(string account)
        {

            CustomJsonResult result = new CustomJsonResult();


            var requestData = new AgentLogoutRequestData();
            requestData.Agent = account;
            requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");
            requestData.UserData = "";

            var request = new AgentLogoutRequest(requestData);

            var requestResult = _api.DoPost(request);

            return result;
        }
        public Enumeration.TelSeatPhoneStatus GetStatus(string account)
        {
            Enumeration.TelSeatPhoneStatus telephoneStatus = Entity.Enumeration.TelSeatPhoneStatus.Unknow;

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
                    telephoneStatus = Enumeration.TelSeatPhoneStatus.IDLE;
                    break;
                case "CALL_OUT":
                    telephoneStatus = Enumeration.TelSeatPhoneStatus.CallOut;
                    break;
                case "CALL_IN":
                    telephoneStatus = Enumeration.TelSeatPhoneStatus.CallIn;
                    break;
                case "RINGING":
                    telephoneStatus = Enumeration.TelSeatPhoneStatus.Ringing;
                    break;
                case "PROCESS":
                    telephoneStatus = Enumeration.TelSeatPhoneStatus.Process;
                    break;
                default:
                    telephoneStatus = Enumeration.TelSeatPhoneStatus.Unknow;
                    break;
            }



            return telephoneStatus;
        }
    }
}
