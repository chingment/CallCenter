using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.Entity
{
    [Table("ObCustomerBelongTrack")]
    public class ObCustomerBelongTrack
    {
        [Key]
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string ObBatchId { get; set; }
        public string ObBatchDataId { get; set; }
        public string ObCustomerId { get; set; }
        public string BelongId { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
