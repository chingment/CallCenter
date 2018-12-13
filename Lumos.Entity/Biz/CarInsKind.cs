using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{
    [Table("CarInsKind")]
    public class CarInsKind
    {
        [Key]
        public string Id { get; set; }
        public string PId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }

        [MaxLength(128)]
        public string AliasName { get; set; }

        public bool CanWaiverDeductible { get; set; }

        public bool IsSelectedWaiverDeductible { get; set; }

        public Enumeration.CarKindType Type { get; set; }

        public Enumeration.CarKindInputType InputType { get; set; }

        [MaxLength(128)]
        public string InputUnit { get; set; }

        [MaxLength(2048)]
        public string InputValue { get; set; }

        [MaxLength(2048)]
        public string InputOption { get; set; }

        public bool IsHasDetails { get; set; }

        public int Priority { get; set; }

        public string Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public string Mender { get; set; }

        public DateTime? MendTime { get; set; }

        public bool IsSelected { get; set; }

    }
}
