using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.Entity
{
    [Table("Order2CarInsKind")]
    public class Order2CarInsKind
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string KindId { get; set; }
        public string KindValue { get; set; }
        public string KindDetails { get; set; }
        public bool KindIsWaiverDeductible { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
