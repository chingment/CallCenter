using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class CustomerProvider:BaseProvider
    {
        public void AddDealtTrack(string dealterId,string remarks,string merchantId, string customerId,string orderId, Entity.Enumeration.OrderFollowStatus orderFollowStatus)
        {
            var customerDealtTrack = new CustomerDealtTrack();
            customerDealtTrack.Id = GuidUtil.New();
            customerDealtTrack.MerchantId = merchantId;
            customerDealtTrack.CustomerId = customerId;
            customerDealtTrack.OrderId = orderId;
            customerDealtTrack.OrderFollowStatus = orderFollowStatus;
            customerDealtTrack.Remarks = remarks;
            customerDealtTrack.DealterId = dealterId;
            customerDealtTrack.DealtTime = this.DateTime;
            customerDealtTrack.CreateTime = this.DateTime;
            customerDealtTrack.Creator = dealterId;
            CurrentDb.CustomerDealtTrack.Add(customerDealtTrack);
            CurrentDb.SaveChanges();
        }
    }
}
