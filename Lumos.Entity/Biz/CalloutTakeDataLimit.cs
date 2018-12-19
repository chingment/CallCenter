using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("CalloutTakeDataLimit")]
    public class CalloutTakeDataLimit
    {
        [Key]
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string SalesmanId { get; set; }
        public int TaskQuantity { get; set; }
        public int UnTakeQuantity { get; set; }
        public int TakedQuantity { get; set; }
        public int UnContactQuantity { get; set; }
        public int TargetQuantity { get; set; }
        public int InValidQuantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
