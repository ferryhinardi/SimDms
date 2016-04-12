using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class Report
    {
        [Key]
        [Column(Order = 1)]
        public string ReportID { get; set; }
        public string ReportPath { get; set; }
        public string ReportSource { get; set; }
        public string ReportProc { get; set; }
        public string ReportDeviceID { get; set; }
        public string ReportInfo { get; set; }
        public string DefaultParam { get; set; }
        public string Refference { get; set; }
        public string IsLanscape { get; set; }
    }

    public class ReportDevice
    {
        [Key]
        [Column(Order = 1)]
        public string ReportDeviceID { get; set; }
        public string DeviceFormat { get; set; }
        public bool IsLanscape { get; set; }
        public int ViewerWidth { get; set; }
        public int ViewerHeight { get; set; }
    }

    public class GnRpMst004
    {
        public String CustomerName { get; set; }
        public String Address1 { get; set; }
        public String Address2 { get; set; }
        public String Address3 { get; set; }
        public String Address4 { get; set; }
        public String PhoneNo { get; set; }
        public String HPNo { get; set; }
        public String BasicModel { get; set; }
        public Decimal? ProductionYear { get; set; }
        public Decimal? ChassisNo { get; set; }
        public String PoliceRegNo { get; set; }
        public DateTime? FirstServiceDate { get; set; }
        public DateTime? LastServiceDate { get; set; }
        public Int32? Kunjungan { get; set; }
        public String Keterangan { get; set; }
    }

    //report service claim detail list batch browse
    public class BatchBrowse
    {
        public string BatchNo { get; set; }
        public DateTime? BatchDate { get; set; }
        public string ReceiptNo { get; set; }
        public string ReceiptDate { get; set; }
        public string FPJNo { get; set; }
        public string FPJDate { get; set; }
        public string FPJGovNo { get; set; }
        public decimal? LotNo { get; set; }
        public decimal? ProcessSeq { get; set; }
        public decimal? TotalNoOfItem { get; set; }
        public decimal? TotalClaimAmt { get; set; }
        public decimal? OtherCompensationAmt { get; set; }
    }
}