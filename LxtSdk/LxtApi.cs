using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using log4net;
using System.Reflection;
using Lumos;
using System.Security.Cryptography;
using Newtonsoft.Json.Serialization;

namespace LxtSdk
{


    public class LxtApi
    {
        public LxtApi()
        {

        }

        public string BuildDigest(string customer, string password, string timestamp, string seq)
        {
            string digest = "";

            digest = GetMD5(customer + "@" + timestamp + "@" + seq + "@" + password);

            return digest;
        }

        public static string GetMD5(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;
        }

        public BaseRequstResult<T> DoPost<T>(IApiPostRequest<T> request)
        {
            BaseRequestData postData = new BaseRequestData();

            string customer = "C112";
            string password = "CD5B3D64915D7BFDBFA319B32B30CD2E530FAA6D";
            string timestamp = ((long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds).ToString();

            ThreadSafeRandom ran = new ThreadSafeRandom();

            string seq = ran.Next(100000, 999999).ToString();
            string digest = BuildDigest(customer, password, timestamp, seq);


            postData.Authentication.Customer = customer;
            postData.Authentication.Timestamp = timestamp;
            postData.Authentication.Agent = "";
            postData.Authentication.Seq = seq;
            postData.Authentication.Digest = digest;

            postData.Param.Lang = "zh_CN|en_US";
            postData.Param.Debug = "true";

            postData.Request = request.PostData;

            string realServerUrl = request.ApiUrl;
            WebUtils webUtils = new WebUtils();


            JsonSerializerSettings jsonSerializerSettings=new JsonSerializerSettings();
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            string str_PostData = JsonConvert.SerializeObject(postData, jsonSerializerSettings);

            LogUtil.Info(string.Format("WeiXinSdk-Post->{0}", postData));
            string responseString = webUtils.DoPost(realServerUrl, str_PostData);
            LogUtil.Info(string.Format("WeiXinSdk-Result->{0}", responseString));
            BaseRequstResult<T> rsp = JsonConvert.DeserializeObject<BaseRequstResult<T>>(responseString);
            return rsp;
        }

    }
}
