using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class VORBrowse
    {
        public long ServiceNo { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public string JobDelayCode { get; set; }
        public string JobDelayDesc { get; set; }
        public string JobReasonDesc { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsSparepart { get; set; }
        public string PoliceRegNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public decimal? Odometer { get; set; }
        public string CustomerCode { get; set; }
        public string  CustomerName { get; set; }
        public string JobType { get; set; }
        public string JobTypeDesc { get; set; }
        public string ForemanID { get; set; }
        public string ForemanName { get; set; }
        public string MechanicID { get; set; }
        public string MechanicName { get; set; }

        public string Customer { get; set; }
        public string SA { get; set; }
        public string FM { get; set; }
        public string JobTypes { get; set; }
    }

    public class JobOrderBrowse
    {
        public long ServiceNo { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public string PoliceRegNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string TransmissionType { get; set; }
        public decimal? Odometer { get; set; }
        public string ChassisCode { get; set; }
        public decimal? ChassisNo { get; set; }
        public string EngineCode { get; set; }
        public decimal? EngineNo { get; set; }
        public string ColorCode { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerCodeBillName { get; set; }
        public string JobType { get; set; }
        public string JobTypeDesc { get; set; }
        public string ForemanID { get; set; }
        public string ForemanName { get; set; }
        public string MechanicID { get; set; }
        public string MechanicName { get; set; }
        public string ServiceStatus { get; set; }

        public string Pelanggan { get; set; }
        public string Pembayar { get; set; }
    }

    public class JobDelayBrowse
    {
        public string JobDelayCode { get; set; }
        public string JobDelayDesc { get; set; }
    }

    public class GridMechanic
    {
        public string MechanicID { get; set; }
        public string MechanicName { get; set; }
    }

    public class VORPart
    {
        public long? SeqNo { get; set; }
        public string POSNo { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public decimal? PartQty { get; set; }
    }

    [Table("svTrnSrvNoVOR")]
    public class svTrnSrvNoVOR
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}