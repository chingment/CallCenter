using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos.Entity;

namespace Lumos.BLL.Service.Merch
{

    public class UserModel
    {
        public string MerchantId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public Enumeration.WorkStatus WorkStatus { get; set; }
        public string WorkStatusName { get; set; }
        public string TelePhoneStatusName { get; set; }
        public string OrganizationId { get; set; }
        public Enumeration.SysPositionId PositionId { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime TimeOutTime { get; set; }
        public string TeleSeatId { get; set; }
        public string TeleSeatAccount { get; set; }
        public string TeleSeatPassword { get; set; }
        public Enumeration.TelePhoneStatus TelePhoneStatus { get; set; }
    }
}
