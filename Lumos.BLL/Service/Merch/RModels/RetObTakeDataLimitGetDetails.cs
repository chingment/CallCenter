using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RetObTakeDataLimitGetDetails
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int TaskQuantity { get; set; }
        public int UnTakeQuantity { get; set; }
        public int TakedQuantity { get; set; }
    }
}
