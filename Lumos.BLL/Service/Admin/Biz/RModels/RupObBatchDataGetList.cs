﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public class RupObBatchDataGetList : RupBaseGetList
    {
        public string ObBatchId { get; set; }

        public string MerchantId { get; set; }
    }
}
