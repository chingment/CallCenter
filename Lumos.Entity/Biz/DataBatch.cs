using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("DataBatch")]
    public class DataBatch
    {
        [Key]
        public string Id { get; set; }

        public string MerchantId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public Enumeration.DataBatchSoureType SoureType { get; set; }
        public string SoureName { get; set; }
        public string FilePath { get; set; }
        public Enumeration.DataBatchBizType BizType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ValidCount { get; set; }
        public int InValidCount { get; set; }
        public Enumeration.DataBatchStatus Status { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public string Mender { get; set; }

        public DateTime? MendTime { get; set; }
    }
}
