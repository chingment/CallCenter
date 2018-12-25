using Autofac;
using Lumos.BLL;
using LxtSdk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{


    public sealed class Cat
    {
        public event EventHandler Calling;

        public void Call()
        {
            Console.WriteLine("猫叫了...");
            if (Calling != null)//检查是否有事件注册  
                Calling(this, EventArgs.Empty);//调用事件注册的方法  
        }
    }

    public sealed class Mouser
    {
        public void Escape()
        {
            Console.WriteLine("老鼠逃跑了...");
        }

    }

    public sealed class Master
    {
        //发生猫叫时惊醒  
        public void Wakeup(object sender, EventArgs e)
        {
            Console.WriteLine("主人惊醒了...");
        }
    }


    public class A
    {
        public static int a = 1;
        public A()
        {
            a++;
            Console.WriteLine("A.a=" + a);
            Console.WriteLine("我是A");
        }

        public virtual void Show()
        {
            Console.WriteLine("我是A.Show");
        }

        public virtual void Cry()
        {
            Console.WriteLine("我是A.Cry");
        }
    }

    public class B : A
    {
        public B()
        {
            a++;
            Console.WriteLine("B.a=" + a);
            Console.WriteLine("我是B");
        }

        public override void Show()
        {
            //base.Show();
            Console.WriteLine("我是B.Show");
        }

        public new void Cry()
        {
            Console.WriteLine("我是B.Cry");
        }
    }

    public class Test
    {
        public void Show()
        {
            Console.WriteLine("FileName");
        }
    }

    public class Test2
    {
        public Test Test { get; set; }

        public void Show()
        {
            if (Test != null)
            {
                Test.Show();
            }
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Address { get; set; }
    }

    public interface IPersonRepository
    {
        IEnumerable<Person> GetAll();
        Person Get(int id);
        Person Add(Person item);
        bool Update(Person item);
        bool Delete(int id);
    }

    public class PersonRepository : IPersonRepository
    {
        List<Person> person = new List<Person>();

        public PersonRepository()
        {
            Add(new Person { Id = 1, Name = "joye.net1", Age = 18, Address = "中国上海" });
            Add(new Person { Id = 2, Name = "joye.net2", Age = 18, Address = "中国上海" });
            Add(new Person { Id = 3, Name = "joye.net3", Age = 18, Address = "中国上海" });
        }
        public IEnumerable<Person> GetAll()
        {
            return person;
        }
        public Person Get(int id)
        {
            return person.Find(p => p.Id == id);
        }
        public Person Add(Person item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            person.Add(item);
            return item;
        }
        public bool Update(Person item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            int index = person.FindIndex(p => p.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            person.RemoveAt(index);
            person.Add(item);
            return true;
        }
        public bool Delete(int id)
        {
            person.RemoveAll(p => p.Id == id);
            return true;
        }
    }

    class Program
    {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串 </returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));//转换为字节
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();//实例化数据加密标准
                MemoryStream mStream = new MemoryStream();//实例化内存流
                //将数据流链接到加密转换的流
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
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

        public static string GetMD52(string myString)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.Unicode.GetBytes(myString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("X");
            }

            return byte2String;
        }


        static void Main(string[] args)
        {

            //var s = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject>("{\"custome\":\"C112\",\"notify\":{\"type\":\"billing\"}}");


           // var s1 = s["notify"];
           // var s2=s1["type"]

            string EncryptStr = EncryptDES("{csrId:\"bba32e80f0e3461eb064437b35240c83\",\"a\":\"bba32e80f0e3461eb064437b35240c83\"}", "ssssssss");  //返回加密后的字符串
            string DecryptStr = DecryptDES(EncryptStr, "ssssssss");//解密字符串


            ////var p1 = new AgentQueryStatusRequestData();
            ////p1.Agent = "1308000";
            ////p1.Seq = "112323";
            ////p1.UserData = "";

            ////var req1 = new AgentQueryStatusRequest(p1);
            //var lxt = new LxtApi();
            ////var lxtResult = lxt.DoPost(req1);


            ////var p2 = new AgentLoginRequestData();
            ////p2.Agent = "1308000";
            ////p2.Seq = "112323";
            ////p2.UserData = "";

            ////var req2 = new AgentLoginRequest(p2);

            ////var lxtResult2 = lxt.DoPost(req2);


            //var p3 = new CallNumberRequestData();
            //p3.Agent = "1308000";
            //p3.Seq = "112323";
            ////p3.UserData = "";
            //p3.Callee = "15989287032";

            //var req3 = new CallNumberRequest(p3);

            //var lxtResult3 = lxt.DoPost(req3);

            //int num = 100;
            //Thread[] th = new Thread[num];
            //for (int i = 0; i < num; i++)
            //{
            //    Thread n = new Thread(DoWork);
            //    th[i] = n;
            //    th[i].Start();
            //}
        }

        public static void DoWork()
        {


            //builder.RegisterModule(new LoggingModule());
            //builder.RegisterType<Test>();

            var s = new Test2();

            s.Test.Show();

            var builder = new ContainerBuilder();
            builder.RegisterType<Test>().As<Test>();
            //builder.RegisterControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
            var container = builder.Build();
            //DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //builder.Register(t => new Test()).As<Test>();
            //builder.RegisterType<Test2>();
            //var container = builder.Build();

            Test2 test2 = container.Resolve<Test2>();
            //Test2 ee = new Test2();
            test2.Show();

            //string sn = SnUtil.Build(Lumos.Entity.Enumeration.BizSnType.Order2StockIn, "111");
            //Console.WriteLine("sn:" + sn);
        }
    }
}

