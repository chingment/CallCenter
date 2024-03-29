﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("ObBatch")]
    public class ObBatch
    {
        [Key]
        public string Id { get; set; }

        public string MerchantId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Enumeration.DataBatchSoureType SoureType { get; set; }
        public string SoureName { get; set; }
        public string FilePath { get; set; }
        public string ImportFileTmpl { get; set; }
        public Enumeration.BusinessType BusinessType { get; set; }
        public int DataCount { get; set; }
        public int ValidCount { get; set; }
        public int InValidCount { get; set; }
        public Enumeration.DataBatchStatus Status { get; set; }

        public int ExpiryDays { get; set; }
        public DateTime ExpiryTime { get; set; }

        public int RecoveryDays { get; set; }
        public DateTime RecoveryTime { get; set; }
        public int FollowDelayDays { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }

        public string HandleReport { get; set; }

        public string BelongerId { get; set; }

        public string BelongerName { get; set; }

        public string BelongerOrganizationId { get; set; }
    }
}
