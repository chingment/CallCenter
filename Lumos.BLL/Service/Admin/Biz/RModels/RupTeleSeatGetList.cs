﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public class RupTeleSeatGetList : RupBaseGetList
    {
        public string Account { get; set; }

        public string MerchantId { get; set; }

        public string UserName { get; set; }
    }
}