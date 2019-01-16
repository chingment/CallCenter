using log4net;
using Lumos;
using Lumos.BLL.Task;
using System;
using System.Threading;
using Topshelf;

namespace Task4Mq2Global
{
    public sealed class ServiceRunner : ServiceControl, ServiceSuspend
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ServiceRunner));


        private string ServiceName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings.Get("ServiceName");
            }
        }

        public ServiceRunner()
        {

        }

        public bool Start(HostControl hostControl)
        {
            new Thread(Go).Start();
            _logger.Info(string.Format("{0} Start", ServiceName));
            return true;
        }

        void Go()
        {
            try
            {
                string taskProvider = System.Configuration.ConfigurationManager.AppSettings["custom:Task4Provider"];
                Task4Factory.Launcher.Launch(taskProvider);
            }
            catch (Exception ex)
            {
                LogUtil.Error("异常错误", ex);
            }
        }

        public bool Stop(HostControl hostControl)
        {
            _logger.Info(string.Format("{0} Stop", ServiceName));
            return true;
        }

        public bool Continue(HostControl hostControl)
        {

            _logger.Info(string.Format("{0} Continue", ServiceName));
            return true;
        }

        public bool Pause(HostControl hostControl)
        {
            _logger.Info(string.Format("{0} Pause", ServiceName));
            return true;
        }

    }
}
