using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Sales.Models
{

    public class Jurnal
    {
        public decimal? year { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? DateFromHide { get; set; }
        public DateTime? DateToHide { get; set; }
        public string Status { get; set; }
        public string Transaksi { get; set; }
        public string Trans { get; set; }

    }

    public class JurnalPurchase
    {
         public bool isSelected { get; set; }
         public string CompanyCode { get; set; }
         public string BranchCode { get; set; }
         public string TypeJournal { get; set; }
         public string DocNo { get; set; }
         public DateTime? DocDate { get; set; }
         public string RefNo { get; set; }
         public string RefCode { get; set; }
         public string RefName { get; set; }
         public string Status { get; set; }
            
    }

    public class JurnalInventory
    {
        public bool isSelected { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TypeJournal { get; set; }
        public string DocNo { get; set; }
        public DateTime? DocDate { get; set; }
        public string ReffNo { get; set; }
        public string BranchCodeFrom { get; set; }
        public string BranchCodeTo { get; set; }
        public string Status { get; set; }

    }

    public class GetJurnal
    {
        public String SeqCode { get; set; }
        public String TypeTrans { get; set; }
        public String AccountNo { get; set; }
        public String AccDescription { get; set; }
        public Decimal? AmountDb { get; set; }
        public Decimal? AmountCr { get; set; }
    }
}