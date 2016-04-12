using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class SPKNoService:CustomerDetailsTagih
    {
        public string PoliceRegNo { get; set; }
        public string ServiceBookNo { get; set; }
        public string BasicModel { get; set; }
        public string ServiceStatus { get; set; }
        public string ColorCode { get; set; }
        public decimal EngineNo { get; set; }
        public string JobOrderNo { get; set; }
        public DateTime JobOrderDate { get; set; }
        public string VIN { get; set; }
        public string LmpNo { get; set; }
        public DateTime LmpDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public DateTime ReferenceDate { get; set; }
        public string ReferenceNo { get; set; }
        public string ReturnNo { get; set; }
    }

    public class ReturnSSDetails
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartNo { get; set; }
        public string PartNoOriginal { get; set; }
        public string DocNo { get; set; }
        public decimal QtyLmp { get; set; }
        public decimal QtyBill { get; set; }
        public string ReturnNo { get; set; }
        public string WarehouseCode { get; set; }
        public string LmpNo { get; set; }
        public DateTime LmpDate { get; set; }
    }

    public class ReturnSumAmtSS
    {
        public string ReturnNo { get; set; }
        public decimal TotReturQty { get; set; }
        public decimal TotReturAmt { get; set; }
        public decimal TotDiscAmt { get; set; }
        public decimal TotDPPAmt { get; set; }
        public decimal TotCostAmt { get; set; }
        public decimal TotPPNAmt { get; set; }
        public decimal TotFinalReturAmt { get; set; }
    }

    public class SelectCostPrice
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string PartNo { get; set; }
        public string SupplierCode { get; set; }
        public decimal OnHand { get; set; }
        public decimal CostPrice { get; set; }
    }
}
