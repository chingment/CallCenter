using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.Entity
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public string Id { get; set; }
        public string Sn { get; set; }
        public string MerchantId { get; set; }
        public Enumeration.OrderType Type { get; set; }
        public decimal ChargeAmount { get; set; }
        public Enumeration.OrderPayWay PayWay { get; set; }
        public Enumeration.OrderStatus Status { get; set; }
        public Enumeration.OrderFollowStatus FollowStatus { get; set; }
        public DateTime SubmitTime { get; set; }
        public DateTime? PayTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public DateTime? CancleTime { get; set; }
        public string Remarks { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
