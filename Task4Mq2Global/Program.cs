using log4net;
using Lumos;
using Lumos.BLL;
using Lumos.BLL.Task;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Topshelf;

namespace Task4Mq2Global
{
    class Program
    {
        public static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log.InfoFormat("程序开始");

            HostFactory.Run(x =>
            {
                x.UseLog4Net();
                x.RunAsLocalSystem();
                x.Service<ServiceRunner>();
                x.SetDescription(string.Format("{0} Ver:{1}", System.Configuration.ConfigurationManager.AppSettings.Get("ServiceName"), System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));
                x.SetDisplayName(System.Configuration.ConfigurationManager.AppSettings.Get("ServiceDisplayName"));
                x.SetServiceName(System.Configuration.ConfigurationManager.AppSettings.Get("ServiceName"));
                x.EnablePauseAndContinue();
            });
        }




        //public static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //[DllImport("User32.dll", EntryPoint = "ShowWindow")]
        //private static extern bool ShowWindow(IntPtr hWnd, int type);

        //static void Main(string[] args)
        //{
        //    LogUtil.SetTrackId();
        //    LogUtil.Info("程序开始运行");

        //    ShowWindow(Process.GetCurrentProcess().MainWindowHandle, 2);//隐藏本dos窗体, 0: 后台执行；1:正常启动；2:最小化到任务栏；3:最大化

        //    try
        //    {
        //        string taskProvider = System.Configuration.ConfigurationManager.AppSettings["custom:Task4Provider"];
        //        Task4Factory.Launcher.Launch(taskProvider);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtil.Error("异常错误", ex);
        //    }

        //    LogUtil.Info("程序结束运行");
        //}
    }
}
