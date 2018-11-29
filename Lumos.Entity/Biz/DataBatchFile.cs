using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("DataBatchFile")]
    public class DataBatchFile
    {
        [Key]
        public string Id { get; set; }

        public string Code { get; set; }

        public string FilePath { get; set; }

        public Enumeration.DataBatchFileType FileType { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public Enumeration.DataBatchFileStatus Status { get; set; }

        public string Description { get; set; }

        public string Creator { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
