using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("TeleSeat")]
    public class TeleSeat
    {
        [Key]
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
