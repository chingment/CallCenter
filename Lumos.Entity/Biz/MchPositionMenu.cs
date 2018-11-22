using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("MchPositionMenu")]
    public class MchPositionMenu
    {
        public string Id { get; set; }
        [Key]
        [Column(Order = 1)]
        public string PositionId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MenuId { get; set; }

        public Enumeration.PositionType PositionType { get; set; }

        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
