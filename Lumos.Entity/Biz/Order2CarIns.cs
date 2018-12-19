using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.Entity
{
    [Table("Order2CarIns")]
    public class Order2CarIns:Order
    {
        public string CustomerId { get; set; }
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }
        public decimal OfCompulsoryAmount { get; set; }
        public decimal OfTravelTaxAmount { get; set; }
        public decimal OfCommercialAmount { get; set; }
        public decimal OfTotalAmount { get; set; }
        public decimal UnCompulsoryAmount { get; set; }
        public decimal UnTravelTaxAmount { get; set; }
        public decimal UnCommercialAmount { get; set; }
        public decimal UnTotalAmount { get; set; }
        public string UnAuditComments { get; set; }
        public string UnOrderImgUrl { get; set; }
        public string UnPayImgUrl { get; set; }
        public string CarOwner { get; set; }
        public string CarOwnerPhoneNumber { get; set; }
        public string CarOwnerAddress { get; set; }
        public string CarOwnerIdNumber { get; set; }
        public DateTime? CarRegisterDate { get; set; }
        public string CarPlateNo { get; set; }
        public string CarModel { get; set; }
        public string CarEngineNo { get; set; }
        public string CarVin { get; set; }
        public string UnderwriterId { get; set; }
        public string UnderwriterName { get; set; }

    }
}
