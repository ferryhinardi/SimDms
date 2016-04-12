using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class JobOrder
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public long ServiceNo { get; set; }
        public string ServiceType { get; set; }
        public string JobOrderNo { get; set; }
        public string EstimationNo { get; set; }
        public string BookingNo { get; set; }
    }

    [Table("SvJobOrderView")]
    public class JobOrderView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ProductType { get; set; }
        [Key]
        [Column(Order = 4)]
        public long ServiceNo { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string EstimationNo { get; set; }
        public DateTime? EstimationDate { get; set; }
        public string InvoiceNo { get; set; }
        public string BookingNo { get; set; }
        public DateTime? BookingDate { get; set; }
        public string PoliceRegNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string Chassis { get; set; }
        public string EngineCode { get; set; }
        public decimal EngineNo { get; set; }
        public string Engine { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string CustomerCode { get; set; }
        public string Customer { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerBill { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerNameBill { get; set; }
        public string CustomerBillAddress { get; set; }
        public string NPWPNo { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public decimal Odometer { get; set; }
        public string JobType { get; set; }
        public string ForemanID { get; set; }
        public string MechanicID { get; set; }
        public string ServiceStatus { get; set; }
        public string ServiceType { get; set; }
        public string StatusTask { get; set; }
        public string ServiceStatusDesc { get; set; }
        public string ServiceRequestDesc { get; set; }
        public string ForemanName { get; set; }
        public string MechanicName { get; set; }
        public decimal? TotalSrvAmount { get; set; }
    }

    public class DiscountLookup
    {
        public int SeqNo { get;set; }
        public string DiscountType { get; set; }
        public decimal LaborDiscPct { get; set; }
        public decimal PartDiscPct { get; set; }
        public decimal MaterialDiscPct { get; set; }
    }
}