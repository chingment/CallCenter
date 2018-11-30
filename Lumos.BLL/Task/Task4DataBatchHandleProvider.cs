using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Task
{
    public class Task4DataBatchHandleProvider : BaseProvider, IJob
    {
        public void Execute(IJobExecutionContext context)
        {

            var dataBatchs = CurrentDb.DataBatch.Where(m => m.Status == Entity.Enumeration.DataBatchStatus.WaitHandle).ToList();

            foreach (var dataBatch in dataBatchs)
            {
                dataBatch.Status = Entity.Enumeration.DataBatchStatus.Handling;
                dataBatch.Mender = GuidUtil.New();
                dataBatch.MendTime = this.DateTime;
                CurrentDb.SaveChanges();


            }
        }
    }
}
