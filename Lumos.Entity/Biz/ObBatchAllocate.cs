﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.Entity
{
    [Table("ObBatchAllocate")]
    public class ObBatchAllocate
    {
        [Key]
        public string Id { get; set; }
        public string PId { get; set; }
        public string MerchantId { get; set; }
        public string ObBatchId { get; set; }
        public string SoureName { get; set; }
        public int DataCount { get; set; }
        public int AllocatedCount { get; set; }
        public int UnAllocatedCount { get; set; }
        public int UsedCount { get; set; }
        public int UnUsedCount { get; set; }
        public string BelongUserId { get; set; }
        public string BelongUserName { get; set; }
        public string BelongOrganizationId { get; set; }
        public string Allocater { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }

        public Enumeration.ObBatchAllocateMode AllocateMode { get; set; }

        public string Description { get; set; }

        public string Filters { get; set; }
    }
}