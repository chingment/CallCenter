using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.Entity
{
    [Table("ImportFileTmpl")]
    public class ImportFileTmpl
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
