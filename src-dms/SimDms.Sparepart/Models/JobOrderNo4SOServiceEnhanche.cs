using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class JobOrderNo4SOServiceEnhanche
    {
        public Int64 NoUrut { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string ProductType { get; set; }
        public Int64 ServiceNo { get; set; }
        public string PartNo { get; set; }
        public string TipePart { get; set; }
        public string PartName { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public string TypeOfGoods { get; set; }
        public string BillType { get; set; }
        public decimal? QtyOrder { get; set; }
        public int QtySupply { get; set; }
        public int QtyBO { get; set; }
        public decimal? NetSalesAmt { get; set; }
        public decimal? DiscPct { get; set; }
    }

    public class SelectListPickingSlip {
        public Int64 No { get; set; }
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string PickingSlipNo { get; set; }
        public string PartNo { get; set; }
        public string PartNoOri { get; set; }
        public decimal? QtySupply { get; set; }
        public decimal? QtyPicked { get; set; }
        public decimal? QtyBill { get; set; }
        public string Status { get; set; }
        public string TransTypeDesc { get; set; }
        public string TransType { get; set; }
        public string LmpNo { get; set; }
        public string PickedBy { get; set; }
    }

    public class SelectSPKNoEnhance {
        public string JobOrderNo { get; set; }
        public DateTime JobOrderDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerCodeBill { get; set; }
        public string CustomerName { get; set; }
    }

    public class LampiranDocMode {
        public string JobOrderNo { get; set; }
        public string OrderNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime JobOrderDate { get; set; }
        public DateTime OrderDate { get; set; }
        public string TransType { get; set; }
        public ControlLmp Controls { get; set; }
        public List<JobOrderNo4SOServiceEnhanche> JOSrvEnhance { get; set; }
        public List<SelectListPickingSlip> ListPickingSlip { get; set; }
    }

    public class ControlLmp
    {
        public bool IsDisableTransType { get; set; }
        public bool IsDisableOrderDate { get; set; }
        public bool IsDisableJobOrderDate { get; set; }
        public bool IsDisableBtnPicking { get; set; }
        public bool IsDisableBtnUsageDocNo { get; set; }
        public bool IsDisableTxtUsageNo { get; set; }
        public bool IsDisableBtnLmp { get; set; }
        public bool IsDisableBtnPickedBy { get; set; }
        public bool IsDisableBtnProcLmp { get; set; }
        public string PickStatus { get; set; }
        public int Status { get; set; }
    }

    public class prePrint
    {
        public string LmpNo { get; set; }
        public string PickingSlipNo { get; set; }
        public string TypeOfGoods { get; set; }
        public string TransType { get; set; }
        public string SalesType { get; set; }
        public string DocNo { get; set; }
    }
}
    