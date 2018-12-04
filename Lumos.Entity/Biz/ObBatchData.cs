using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.Entity
{
    [Table("ObBatchData")]
    public class ObBatchData
    {
        [Key]
        public string Id { get; set; }
        public string MerchantId { get; set; }
        public string ObBatchId { get; set; }
        public string CsrName { get; set; }
        public string CsrPhoneNumber { get; set; }
        public string CsrAddress { get; set; }
        public string CsrIdNumber { get; set; }
        public DateTime? CarRegisterDate { get; set; }
        public string CarPlateNo { get; set; }
        public string CarModel { get; set; }
        public string CarEngineNo { get; set; }
        public string CarVin { get; set; }
        public string CarInsLastQzNo { get; set; }
        public string CarInsLastSyNo { get; set; }
        public string CarInsLastCompany { get; set; }
        public DateTime? CarInsLastStartTime { get; set; }
        public DateTime? CarInsLastEndTime { get; set; }
        public bool IsValid { get; set; }
        public string HandleReport { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
