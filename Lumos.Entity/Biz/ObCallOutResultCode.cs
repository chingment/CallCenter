using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("ObCallOutResultCode")]
    public class ObCallOutResultCode
    {
        [Key]
        public string Code { get; set; }
        public string FCode { get; set; }
        public string Name { get; set; }
        public Enumeration.DataBizType DataBizType { get; set; }

        public int Priority { get; set; }
    }
}
