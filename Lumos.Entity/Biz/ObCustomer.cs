using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("ObCustomer")]
    public class ObCustomer
    {
        [Key]
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string ObBatchId { get; set; }
        public string ObBatchDataId { get; set; }
        public string CsrName { get; set; }
        public string CsrPhoneNumber { get; set; }
        public string CsrAddress { get; set; }
        public string CsrIdNumber { get; set; }
        public string CarRegisterDate { get; set; }
        public string CarPlateNo { get; set; }
        public string CarModel { get; set; }
        public string CarEngineNo { get; set; }
        public string CarVin { get; set; }
        public string CarInsLastQzNo { get; set; }
        public string CarInsLastSyNo { get; set; }
        public string CarInsLastCompany { get; set; }
        public string CarInsLastStartTime { get; set; }
        public string CarInsLastEndTime { get; set; }
        public string BelongId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public DateTime ExpiryTime { get; set; }
        public DateTime RecoveryTime { get; set; }
        public int FollowDelayDays { get; set; }
    }
}
