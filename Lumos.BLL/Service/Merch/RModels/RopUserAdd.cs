﻿using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RopUserAdd
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string OrganizationId { get; set; }

        public Enumeration.SysPositionId PositionId { get; set; }

        public string TeleSeatId { get; set; }
    }
}
