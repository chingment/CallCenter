using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{
    [Table("CustomerDealtTrack")]
    public class CustomerDealtTrack
    {
        [Key]
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string OrderId { get; set; }
        public Enumeration.OrderFollowStatus OrderFollowStatus { get; set; }
        public string DealterId { get; set; }
        public string DealterName { get; set; }
        public DateTime DealtTime { get; set; }
        public string Remarks { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
