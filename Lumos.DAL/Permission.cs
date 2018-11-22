using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Lumos.Entity
{
    public class Permission
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PId { get; set; }
        public string Description { get; set; }
    }
}
