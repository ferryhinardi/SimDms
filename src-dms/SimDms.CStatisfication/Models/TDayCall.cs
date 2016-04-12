using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models
{
    [Table("CsTDayCall")]
    public class TDayCall
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Chassis { get; set; }
        public bool? IsDeliveredA { get; set; }
        public bool? IsDeliveredB { get; set; }
        public bool? IsDeliveredC { get; set; }
        public bool? IsDeliveredD { get; set; }
        public bool? IsDeliveredE { get; set; }
        public bool? IsDeliveredF { get; set; }
        public bool? IsDeliveredG { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int Status { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string Reason { get; set; }
    }

    public class TDayCallModel
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Chassis { get; set; }
        public bool? IsDeliveredA { get; set; }
        public bool? IsDeliveredB { get; set; }
        public bool? IsDeliveredC { get; set; }
        public bool? IsDeliveredD { get; set; }
        public bool? IsDeliveredE { get; set; }
        public bool? IsDeliveredF { get; set; }
        public bool? IsDeliveredG { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int Status { get; set; }
        public DateTime? FinishDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public DateTime? BirthDate { get; set; }
        public string AddPhone1 { get; set; }
        public string AddPhone2 { get; set; }
        public string ReligionCode { get; set; }
        public string Reason { get; set; }
    }

    [Table("CsTDayCallView")]
    public class TDayCallView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CustomerCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string Chassis { get; set; }
        public bool? IsDeliveredA { get; set; }
        public bool? IsDeliveredB { get; set; }
        public bool? IsDeliveredC { get; set; }
        public bool? IsDeliveredD { get; set; }
        public bool? IsDeliveredE { get; set; }
        public bool? IsDeliveredF { get; set; }
        public bool? IsDeliveredG { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public int Status { get; set; }
        public DateTime? FinishDate { get; set; }
        public DateTime? InputDate { get; set; }

        public string StatusInfo { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string BranchCode { get; set; }
        public string Engine { get; set; }
        public string CarType { get; set; }
        public string Color { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public string PoliceRegNo { get; set; }

        public DateTime? BirthDate { get; set; }
        public string AddPhone1 { get; set; }
        public string AddPhone2 { get; set; }
        public string ReligionCode { get; set; }
    }

    [Table("CsLkuTDaysCallView")]
    public class CsLkuTDayCallView
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string HPNo { get; set; }
        public string AddPhone1 { get; set; }
        public string AddPhone2 { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Outstanding { get; set; }
        public DateTime? BirthDate { get; set; }
        public string CarType { get; set; }
        public string Color { get; set; }
        public string PoliceRegNo { get; set; }
        public string Chassis { get; set; }
        public string Engine { get; set; }
        public string SONo { get; set; }
        public string SalesmanCode { get; set; }
        public string SalesmanName { get; set; }
        public string Salesman { get; set; }
        public string ReligionCode { get; set; }
        public string SalesModelCode { get; set; }
        public decimal? SalesModelYear { get; set; }
        public DateTime? DODate { get; set; }
        public DateTime? BPKDate { get; set; }
        public string IsDeliveredA { get; set; }
        public string IsDeliveredB { get; set; }
        public string IsDeliveredC { get; set; }
        public string IsDeliveredD { get; set; }
        public string IsDeliveredE { get; set; }
        public string IsDeliveredF { get; set; }
        public string IsDeliveredG { get; set; }
        public string Comment { get; set; }
        public string Additional { get; set; }
        public string Reason { get; set; }
        public int? Status { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }

    public class CsDlvryOutstanding
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string PoliceRegNo { get; set; }
        public string Chassis { get; set; }
        public DateTime? BPKDate { get; set; }
    }
}


