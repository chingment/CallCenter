﻿using LxtSdk;
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

        public CustomJsonResult CallNumber(string customer,string password,string account, string seq, string csrPhoneNumber)
        {
            CustomJsonResult result = new CustomJsonResult();

            var requestData = new CallNumberRequestData();
            requestData.Agent = account;
            requestData.Seq = seq;
            requestData.UserData = seq;
            requestData.Callee = csrPhoneNumber;

            var request = new CallNumberRequest(requestData);
            request.Customer = customer;
            request.Password = password;
            var requestResult = _api.DoPost(request);


            return result;
        }
        public CustomJsonResult Hangup(string customer, string password, string account)
        {
            CustomJsonResult result = new CustomJsonResult();

            var requestData = new HangupCallRequestData();
            requestData.Agent = account;
            requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");
            requestData.UserData = "";


            var request = new HangupCallRequest(requestData);
            request.Customer = customer;
            request.Password = password;
            var requestResult = _api.DoPost(request);


            return result;
        }
        public CustomJsonResult Login(string customer, string password, string account)
        {

            CustomJsonResult result = new CustomJsonResult();

            var telephoneStatus = GetStatus(customer, password, account);
            if (telephoneStatus == Enumeration.TelePhoneStatus.Unknow)
            {
                var requestData = new AgentLoginRequestData();
                requestData.Agent = account;
                requestData.Seq = SnUtil.Build(Enumeration.BizSnType.TelphoneControlSeq, "");
                requestData.UserData = "";

                var request = new AgentLoginRequest(requestData);
                request.Customer = customer;
                request.Password = password;
                var requestResult = _api.DoPost(request);

            }

            return result;
        }
        public CustomJsonResult Logout(string customer, string password, string account)
        {

            CustomJsonResult result = new CustomJsonResult();


            var requestData = new AgentLogoutRequestData();
            requestData.Agent = account;
            requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");
            requestData.UserData = "";

            var request = new AgentLogoutRequest(requestData);
            request.Customer = customer;
            request.Password = password;
            var requestResult = _api.DoPost( request);

            return result;
        }
        public Enumeration.TelePhoneStatus GetStatus(string customer, string password, string account)
        {
            Enumeration.TelePhoneStatus telephoneStatus = Entity.Enumeration.TelePhoneStatus.Unknow;

            var requestData = new GetAgentStatusRequestData();
            requestData.Agent = account;
            requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");
            requestData.UserData = "";

            var request = new GetAgentStatusRequest(requestData);
            request.Customer = customer;
            request.Password = password;
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
                    telephoneStatus = Enumeration.TelePhoneStatus.IDLE;
                    break;
                case "CALL_OUT":
                    telephoneStatus = Enumeration.TelePhoneStatus.CallOut;
                    break;
                case "CALL_IN":
                    telephoneStatus = Enumeration.TelePhoneStatus.CallIn;
                    break;
                case "RINGING":
                    telephoneStatus = Enumeration.TelePhoneStatus.Ringing;
                    break;
                case "PROCESS":
                    telephoneStatus = Enumeration.TelePhoneStatus.Process;
                    break;
                default:
                    telephoneStatus = Enumeration.TelePhoneStatus.Unknow;
                    break;
            }



            return telephoneStatus;
        }
        public CustomJsonResult GetRecordList(string customer, string password, string startKey)
        {
            CustomJsonResult result = new CustomJsonResult();


            var requestData = new GetRecordListRequestData();
            requestData.StartKey = startKey;
            requestData.Seq = SnUtil.Build(Entity.Enumeration.BizSnType.TelphoneControlSeq, "");

            var request = new GetRecordListRequest(requestData);
            request.Customer = customer;
            request.Password = password;
            var requestResult = _api.DoPost(request);

            return result;
        }
    }
}
