using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("CallResultCode")]
    public class CallResultCode
    {
        [Key]
        public string Code { get; set; }
        public string FCode { get; set; }
        public string Name { get; set; }
        public Enumeration.BusinessType BusinessType { get; set; }
        public int Priority { get; set; }
        public bool IsAllowNextCall { get; set; }
        public bool IsInValidData { get; set; }
    }
}
