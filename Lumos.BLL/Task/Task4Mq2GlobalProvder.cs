using Lumos.BLL.Biz;
using Lumos.Entity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lumos.BLL.Task
{
    public class Task4Mq2GlobalProvder : BaseProvider, ITask
    {
        public CustomJsonResult Run()
        {
            CustomJsonResult result = new CustomJsonResult();


            RedisMq4GlobalProvider redisMq = new RedisMq4GlobalProvider();
            while (true)
            {
                try
                {
                    var handleObj = redisMq.Pop();
                    if (handleObj == null)
                    {
                        Console.WriteLine("无处理信息，休息100毫秒");
                        Thread.Sleep(1000);
                        continue;
                    }


                    Thread thread = new Thread(new ThreadStart(handleObj.Handle));
                    thread.Start();

                    Console.WriteLine("正在处理信息，休息100毫秒");
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "," + ex.StackTrace);
                    Thread.Sleep(1000);
                }

            }

        }
    }
}
