﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("SysMerchantUser")]
    public class SysMerchantUser : SysUser
    {
        public string MerchantId { get; set; }

        public string OrganizationId { get; set; }

        public Enumeration.SysPositionId PositionId { get; set; }

        public string TeleSeatId { get; set; }
    }
}
