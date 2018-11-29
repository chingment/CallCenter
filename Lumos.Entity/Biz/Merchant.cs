﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.Entity
{
    [Table("Merchant")]
    public class Merchant
    {
        [Key]
        public string Id { get; set; }

        public string SimpleCode { get; set; }

        public string UserId { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }

        public string ContactPhone { get; set; }

        public string ContactAddress { get; set; }

        public Enumeration.BusinessType BusinessType { get; set; }

        public string Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public string Mender { get; set; }

        public DateTime? MendTime { get; set; }
    }
}