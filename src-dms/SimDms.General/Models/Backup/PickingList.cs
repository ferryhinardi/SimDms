using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class PickingList
    {
        public string PickingSlipNo { get; set; }
        public DateTime PickingSlipDate { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string ProductType { get; set; }
        public decimal TotDPPAmt { get; set; }
        public decimal TotPPNAmt { get; set; }
        public decimal TotFinalSalesAmt { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string TransType { get; set; }
        public string CustomerCodeShip { get; set; }
        public string CustomerCodeBill { get; set; }
        public decimal TotSalesAmt { get; set; }
        public decimal TotSalesQty { get; set; }
    }

    public class PickingListBrowse
    {
        public string PickingSlipNo { get; set; }
        public DateTime PickingSlipDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
    }

    public class PickingListHdrLkp
    {
        public string PickingSlipNo { get; set; }
        public DateTime PickingSlipDate { get; set; }
        public string Remark { get; set; }        
    }


    public class CustSOPickingList
    {
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string ProfitCenter { get; set; }
    }

    public class CustOrderDetails
    {
        public string DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string PaymentName { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerCode { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime ReferenceDate { get; set; }
        public bool chkSelect { get; set; }
        public string Remark { get; set; }
    }

    public class PickingPartOrderDetail
    {
        public Int64 NoUrut { get; set; }
        public string DocNo { get; set; }
        public string PartNo { get; set; }
        public string PartNoOriginal { get; set; }
        public int? SupSeq { get; set; }
        public int? PTSeq { get; set; }
        public decimal QtyBill { get; set; }
        public decimal QtyBoSupply { get; set; }
        public string ExPickingSlipNo { get; set; }
        public decimal QtyPick { get; set; }
        public decimal QtyPicked { get; set; }
        public decimal QtyBOOutstd { get; set; }
        public decimal QtyOrder { get; set; }
    }

    public class PickingPartOrderDetailAS
    {
        public Int64 NoUrut { get; set; }
        public string DocNo { get; set; }
        public string PartNo { get; set; }
        public string PartNoOriginal { get; set; }
        public int? SupSeq { get; set; }
        public int? PTSeq { get; set; }
        public int? QtyBill { get; set; }
        public int? QtyBoSupply { get; set; }
        public string ExPickingSlipNo { get; set; }
        public decimal QtyPick { get; set; }
        public decimal QtyPicked { get; set; }
        public decimal QtyBOOutstd { get; set; }
        public decimal QtyOrder { get; set; }
    }
    public class Modifikasi
    {
        public string ID { get; set; }
        public string InterChangeCode { get; set; }
    }

    public class PickingHdrBrowse
    {
        public string PickingSlipNo { get; set; }
        public DateTime PickingSlipDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string SalesType { get; set; }
        public string TransType { get; set; }
        public string PickedBy { get; set; }
        public string PickedByName { get; set; }
        public string  Remark { get; set; }
    }

}
