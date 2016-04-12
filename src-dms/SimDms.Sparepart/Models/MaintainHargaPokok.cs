using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models
{
    public class PartNoMntHargaPokok
    {
        public string PartNo { get; set; }
        public string ProductType { get; set; }
        public string CategoryName { get; set; }
        public string PartCategory { get; set; }
        public string PartName { get; set; }
        public string IsGenuinePart { get; set; }
        public string IsActive { get; set; }
        public decimal? OrderUnit { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string TipePart { get; set; }
        public string TypeOfGoods { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public decimal? QtyAvail { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
    }

    public class InquiryAvgMainRow
    {
        public string PartNo { get; set; }
        public string ProductType { get; set; }
        public string CategoryName { get; set; }
        public string PartCategory { get; set; }
        public string PartName { get; set; }
        public string IsGenuinePart { get; set; }
        public string IsActive { get; set; }
        public decimal? OrderUnit { get; set; }
        public decimal? Onhand { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string TypeOfGoods { get; set; }
        public string WarehouseCode { get; set; }
        public string LocationCode { get; set; }
        public decimal? QtyAvail { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? CostPrice { get; set; }
        public decimal? RetailPriceInclTax { get; set; }
    }

    public class MaintainSave
    {
        public string PartNo { get; set; }
        public decimal NewCostPrice { get; set; }
    }

    public class TypePart
    {
        public string LookUpValue { get; set; }
        public string LookUpValueName { get; set; }
    }

    public class SaveMaintainTypeOfGoods
    {
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string LookUpValue { get; set; }
        public string LookUpValueName { get; set; }
        public string TipePart { get; set; }
        public string TypeOfGoods { get; set; }
    }

    public class printing
    {
        public bool IsPeriod { get; set; }
        public decimal? MonthPeriod { get; set; }
        public string YearPeriod { get; set; }
        public bool IsType { get; set; }
        public string PartType { get; set; }
    }

    
}
