﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("SysRoleMenu")]
    public class SysRoleMenu
    {
        public string Id { get; set; }
        [Key]
        [Column(Order = 1)]
        public string RoleId { get; set; }
        [Key]
        [Column(Order = 2)]
        public string MenuId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
