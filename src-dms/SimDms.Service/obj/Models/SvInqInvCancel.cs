using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class SvInqInvCancel
    {
        public Boolean? IsSelected { get; set; }
        public String InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public String JobOrderNo { get; set; }
        public DateTime? JobOrderDate { get; set; }
        public String FPJNo { get; set; }
        public String FPJGovNo { get; set; }
        public String FPJGovNo1 { get; set; }
        public String JobType { get; set; }
        public Decimal? LaborDppAmt { get; set; }
        public Decimal? PartsDppAmt { get; set; }
        public Decimal? MaterialDppAmt { get; set; }
        public Decimal? TotalSrvAmt { get; set; }
    }

    public class SvInqInvCancelSubDtl
    {
        public String CompanyCode { get; set; }
        public String BranchCode { get; set; }
        public String DocNo { get; set; }
        public Int64? SeqNo { get; set; }
        public DateTime? DocDate { get; set; }
        public String ProfitCenterCode { get; set; }
        public DateTime? AccDate { get; set; }
        public String AccountNo { get; set; }
        public String JournalCode { get; set; }
        public String TypeJournal { get; set; }
        public String ApplyTo { get; set; }
        public Decimal? AmountDb { get; set; }
        public Decimal? AmountCr { get; set; }
        public String TypeTrans { get; set; }
        public String BatchNo { get; set; }
        public Int32? BatchDate { get; set; }
        public String StatusFlag { get; set; }
        public String CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String LastUpdBy { get; set; }
        public DateTime? LastUpdDate { get; set; }
        public Decimal? AmountDbOld { get; set; }
        public Decimal? AmountCrOld { get; set; }
        public String Description { get; set; }
    }
}