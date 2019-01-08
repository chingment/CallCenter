using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("CallRecord")]
    public class CallRecord
    {
        [Key]
        public string Id { get; set; }
        public string Sn { get; set; }
        public string MerchantId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string SalesmanId { get; set; }
        public string SalesmanName { get; set; }
        public string TeleSeatAccount { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? RingTime { get; set; }
        public DateTime? AnswerTime { get; set; }
        public DateTime? ByeTime { get; set; }
        public string RecordFile { get; set; }
        public int Service { get; set; }
        public int TimeLength { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
