using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.Entity
{
    [Table("Order2CarIns")]
    public class Order2CarIns:Order
    {
        public string ObCustomerId { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public decimal CompulsoryAmount { get; set; }
        public decimal TravelTaxAmount { get; set; }
        public decimal CommercialAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string CarOwner { get; set; }
        public string CarOwnerPhoneNumber { get; set; }
        public string CarOwnerAddress { get; set; }
        public string CarOwnerIdNumber { get; set; }
        public DateTime? CarRegisterDate { get; set; }
        public string CarPlateNo { get; set; }
        public string CarModel { get; set; }
        public string CarEngineNo { get; set; }
        public string CarVin { get; set; }
    }
}
