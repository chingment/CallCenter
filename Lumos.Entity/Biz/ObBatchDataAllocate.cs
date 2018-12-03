﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.Entity
{
    [Table("ObBatchDataAllocate")]
    public class ObBatchDataAllocate
    {
        [Key]
        public string Id { get; set; }
        public string PId { get; set; }
        public string MerchantId { get; set; }
        public string ObBatchId { get; set; }
        public int DataCount { get; set; }
        public int AllocatedCount { get; set; }
        public int UnAllocatedCount { get; set; }
        public int UsedCount { get; set; }
        public int UnUsedCount { get; set; }
        public string BelongId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
