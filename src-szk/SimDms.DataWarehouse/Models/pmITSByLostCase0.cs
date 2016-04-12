using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("PmITSByLostCase0")]
    public class PmITSByLostCase0 : IPmITSByLostCase
    {
        [Key]
        [Column(Order=1)]
        public int? AreaCode { get; set; }
        public string Area { get; set; }
        [Key]
        [Column(Order = 2)]
        public string CompanyCode { get; set; }
        public string DealerAbbreviation { get; set; }
        [Key]
        [Column(Order = 3)]
        public string BranchCode { get; set; }
        public string OutletAbbreviation { get; set; }
        [Key]
        [Column(Order = 4)]
        public long InquiryNumber { get; set; }
        public DateTime? InquiryDate { get; set; }
        public DateTime? ProspectDate { get; set; }
        public DateTime? HotProspectDate { get; set; }
        public DateTime? SPKDate { get; set; }
        public DateTime? LostCaseDate { get; set; }
        public string StatusbeforeLOST { get; set; }
        public int? P_Outs { get; set; }
        public int? P_New { get; set; }
        public int? HP_Outs { get; set; }
        public int? HP_New { get; set; }
        public int? SPK_Outs { get; set; }
        public int? SPK_New { get; set; }
        public string TipeKendaraan { get; set; }
        public string Variant { get; set; }
        public string Transmisi { get; set; }
        public string ColourCode { get; set; }
        public string LostCaseCategory { get; set; }
        public string LostCaseCategoryDesc { get; set; }
        public string LostCaseReasonID { get; set; }
        public string LostCaseReasonDesc { get; set; }
        public string LostCaseOtherReason { get; set; }
        public string LostCaseVoiceOfCustomer { get; set; }
    }
}