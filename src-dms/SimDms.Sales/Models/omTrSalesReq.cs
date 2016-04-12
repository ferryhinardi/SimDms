using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.Sales.Models
{
    [Table("omTrSalesReq")]
    public class omTrSalesReq
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ReqNo { get; set; }
        public DateTime? ReqDate { get; set; }
        public string ReffNo { get; set; }
        public DateTime? ReffDate { get; set; }
        public string StatusFaktur { get; set; }
        public string SubDealerCode { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public bool IsLocked { get; set; }
        public string LockedBy { get; set; }
        public DateTime? LockedDate { get; set; }
        public DateTime ApproveDate { get; set; }
        public bool? isCBU { get; set; }
    }

    [Table("omTrSalesFPolRevision")]
    public class omTrSalesFPolRevision
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string RevisionNo { get; set; }
        public DateTime? RevisionDate { get; set; }
        public int? RevisionSeq { get; set; }
        public string RevisionCode { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; }
        public string FakturPolisiName { get; set; }
        public string FakturPolisiAddress1 { get; set; }
        public string FakturPolisiAddress2 { get; set; }
        public string FakturPolisiAddress3 { get; set; }
        public string PostalCode { get; set; }
        public string PostalCodeDesc { get; set; }
        public string FakturPolisiCity { get; set; }
        public string FakturPolisiTelp1 { get; set; }
        public string FakturPolisiTelp2 { get; set; }
        public string FakturPolisiHP { get; set; }
        public DateTime? FakturPolisiBirthday { get; set; }
        public string FakturPolisiNo { get; set; }
        public DateTime? FakturPolisiDate { get; set; }
        public string FakturPolisiArea { get; set; }
        public string IDNo { get; set; }
        public bool IsCityTransport { get; set; }
        public bool? IsProject { get; set; }
        public string SubDealerCode { get; set; }
        public string Status { get; set; }
        public int SendCounter { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
    }

    public class inquiryTrSalesReqView
    {
        public string ReqNo { get; set; }
        public string ReqDate { get; set; }
        public string ReffNo { get; set; }
        public string ReffDate { get; set; }
        public string StatusFaktur { get; set; }
        public string SubDealerCode { get; set; }
        public string SubDealerName { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
    }

    public class SalesReqView
    {
        public string ReqNo { get; set; }
        public DateTime? ReqDate { get; set; }
        public string ReffNo { get; set; }
        public DateTime? ReffDate { get; set; }
        public string StatusFaktur { get; set; }
        public string Faktur { get; set; }
        public string SubDealerCode { get; set; }
        public string CustomerName { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
        public bool? isCBU { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; } 
    }

    public class SubDealerLookup
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ProfitCenter { get; set; }
    }

    public class SalesFPolRevisiView 
    {
        public string RevisiNo { get; set; }
        public string RevisiDate { get; set; }
        public string RevisiStatus { get; set; }
        public string ReqNo { get; set; }
        public DateTime? ReqDate { get; set; }
        public string ReffNo { get; set; }
        public DateTime? ReffDate { get; set; }
        public string StatusFaktur { get; set; }
        public string Faktur { get; set; }
        public string SubDealerCode { get; set; }
        public string CustomerName { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public string Stat { get; set; }
        public bool? isCBU { get; set; }
        public string ChassisCode { get; set; }
        public decimal ChassisNo { get; set; } 
    }
}