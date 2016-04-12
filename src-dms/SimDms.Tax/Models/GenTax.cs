using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Tax.Models
{
    public class GenTax
    {
        public Int64 No { get; set; }
        public bool chkSelect { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProfitCenter { get; set; }
        public string FPJGovNo { get; set; }
        public string FPJGovDate { get; set; }
        public string DocNo { get; set; }
        public string DocDate { get; set; }
        public string CustName { get; set; }
        public string InvNo { get; set; }
    }

    public class FakturPajak 
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProfitCenter { get; set; }
        public string optionSRGL { get; set; }
        public string optionPrintFormat { get; set; }
        public string optionPrePrint { get; set; }
        public string optionSRK { get; set; }
        public string optionPPFF { get; set; }
        public string optionFullHalf { get; set; }
        public string rbPrePrinted { get; set; } 
        public string SignName { get; set; }
        public string JobTitle { get; set; }
        public decimal? StatusCoret { get; set; }
        public bool isShowSparePart { get; set; } 
        public bool isALL { get; set; }
        public bool isDP { get; set; }
        public bool isHargaJual { get; set; }
        public bool isPenggantian { get; set; }
        public bool isTermijn { get; set; }
        public bool isShowJasaMaterial { get; set; }
        public bool isShowPotongan { get; set; }
        public string FakturNoFrom { get; set; }
        public string FakturNoTo { get; set; } 
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; } 
    }
}