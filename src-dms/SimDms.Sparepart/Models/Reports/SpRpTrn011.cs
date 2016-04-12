using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models.Reports
{
    public class SpRpTrn011
    {
        public string FPJNo { get; set; }
        public DateTime? FPJDate { get; set; }
        public string InvoiceNo { get; set; }
        public string PickingSlipNo { get; set; }
        public string fakturFPJGovNo { get; set; }
        public string OrderNo { get; set; }
        public string CustomerCode { get; set; }
        public DateTime? DueDate { get; set; }
        public decimal? TotDiscAmt { get; set; }
        public decimal? TotDppAmt { get; set; }
        public decimal? TotPPNAmt { get; set; }
        public decimal? TotFinalSalesAmt { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string NPWPNo { get; set; }
        public string DocNo { get; set; }
        public string PartNo { get; set; }
        public string PartNoOriginal { get; set; }
        public decimal? QtyBill { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? DiscPct { get; set; }
        public decimal? DiscAmt { get; set; }
        public decimal? NetRetailPrice { get; set; }
        public decimal? NetSalesAmt { get; set; }
        public string PartName { get; set; }
        public string TOPC { get; set; }
        public string CITY { get; set; }
        public decimal? TaxPct { get; set; }
        public string PrintSeq { get; set; }
        //public string PrintSeq { get; set; }
        public string TypeOfGoods { get; set; }
        public string CompanyName { get; set; }
        public string AddressCo1 { get; set; }
        public string AddressCo2 { get; set; }
        public string AddressCo3 { get; set; }
        public string Remark { get; set; }
        public string SignName { get; set; }
        public string TitleSign { get; set; }
        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string Note3 { get; set; }


    }
}
