using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    public class PositionMenu
    {
        public string Id { get; set; }
        [Key]
        [Column(Order = 1)]
        public Enumeration.PositionType PositionType { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MenuId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
