﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("CallResultRecord")]
    public class CallResultRecord
    {
        [Key]
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string CustomerId { get; set; }
        public string SalesmanId { get; set; }
        public string ResultCode { get; set; }
        public DateTime? NextCallTime { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}