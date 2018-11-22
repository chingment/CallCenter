using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{

    [Table("SysMerchantUser")]
    public class SysMerchantUser : SysUser
    {
        public string MerchantId { get; set; }
        public Enumeration.PositionType PositionType { get; set; }
        public Enumeration.BusinessType BusinessType { get; set; }
    }
}
