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
using System.Text.RegularExpressions;

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

        public static string GetMD5(string material)
        {
            if (string.IsNullOrEmpty(material))
                throw new ArgumentOutOfRangeException();



            byte[] result = Encoding.Default.GetBytes(material);    //tbPass为输入密码的文本框  
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            string s_output = BitConverter.ToString(output).Replace("-", "");

            return s_output;
        }

        public static string DeUnicode(string str)
        {
            //最直接的方法Regex.Unescape(str);
            Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
            return reg.Replace(str, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        }

        public BaseRequstResult<T> DoPost<T>(IApiPostRequest<T> request)
        {
            BaseRequestData postData = new BaseRequestData();

            string customer = "C112";
            string password = "002D5B9BB96F585E7FA85AB1BADE244B30744608";
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

            LogUtil.Info(string.Format("FengNiaoSdk-PostUrl->{0}", realServerUrl));
            LogUtil.Info(string.Format("FengNiaoSdk-PostData->{0}", str_PostData));
            string responseString = webUtils.DoPost(realServerUrl, str_PostData);

            responseString = DeUnicode(responseString);


            LogUtil.Info(string.Format("FengNiaoSdk-PostResult->{0}", responseString));
            BaseRequstResult<T> rsp = JsonConvert.DeserializeObject<BaseRequstResult<T>>(responseString);
            return rsp;
        }

    }
}
