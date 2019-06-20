
using log4net;
using Lumos.BLL;
using Lumos.Common;
using Lumos.DAL;
using Lumos.Entity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebAppApi.Controllers
{
    public class HomeController : Controller
    {
        private string key = "test";
        private string secret = "6ZB97cdVz211O08EKZ6yriAYrHXFBowC";
        private long timespan = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;
        private string host = "";

        Dictionary<string, string> model = new Dictionary<string, string>();

        public ActionResult Index()
        {
            //object isTest = ConfigurationManager.AppSettings["custom:IsTest"];
            //if (isTest == null)
            //{
            //    isTest = "false";
            //}

            //host = "https://demo.res.17fanju.com";

            //if (isTest.ToString() == "false")
            //{
            //    host = "https://demo.res.17fanju.com";
            //}
            //else
            //{
            //     host = "http://localhost:16665";
            //}

            //string clientId = "00000000000000000000000000000000";
            //string storeId = "be9ae32c554d4942be4a42fa48446210";
            //model.Add("获取全局数据", GlobalDataSet(clientId, storeId, DateTime.Parse("2018-04-09 15:14:28")));
 
            return View(model);
        }

        public static string stringSort(string str)
        {
            char[] chars = str.ToCharArray();
            List<string> lists = new List<string>();
            foreach (char s in chars)
            {
                lists.Add(s.ToString());
            }
            lists.Sort();//sort默认是从小到大的。显示123456789      

            str = "";
            foreach (string item in lists)
            {
                str += item;
            }
            return str;
        }
         
    }
}