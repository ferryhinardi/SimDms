using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("SvFtir")]
    public class SvFtir
    {
        [Key]
        [Column(Order = 1)]
        public string FtirNo { get; set; }
        public string ScanNo { get; set; }
        public DateTime? FtirDate { get; set; }
        public DateTime? FtirEventDate { get; set; }
        public string FtirMaker { get; set; }
        public string FtirCategory { get; set; }
        public DateTime? FtirRegDate { get; set; }
        public string DealerName { get; set; }
        public string VinNo { get; set; }
        public string Model { get; set; }
        public string Machine { get; set; }
        public string TransmNo { get; set; }
        public string TitleCategory { get; set; }
        public string TitleName { get; set; }
        public int? Odometer { get; set; }
        public int? UsageTime { get; set; }
        public bool? IsAvailPartDmg { get; set; }
        public bool? IsReportToSis { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string NotSendingCategory { get; set; }
        public string EvtInfoA01 { get; set; }
        public bool? EvtInfoA02 { get; set; }
        public string EvtInfoA03 { get; set; }
        public string EvtInfoA04 { get; set; }
        public string EvtInfoA05 { get; set; }
        public string EvtInfoA06 { get; set; }
        public int? EvtInfoA07 { get; set; }
        public string EvtInfoA08 { get; set; }
        public string EvtInfoB01 { get; set; }
        public bool? EvtInfoB01Chk01 { get; set; }
        public bool? EvtInfoB01Chk02 { get; set; }
        public bool? EvtInfoB01Chk03 { get; set; }
        public bool? EvtInfoB01Chk04 { get; set; }
        public bool? EvtInfoB01Chk05 { get; set; }
        public bool? EvtInfoB01Chk06 { get; set; }
        public bool? EvtInfoB01Chk07 { get; set; }
        public bool? EvtInfoB01Chk08 { get; set; }
        public bool? EvtInfoB01Chk09 { get; set; }
        public bool? EvtInfoB01Chk10 { get; set; }
        public bool? EvtInfoB01Chk11 { get; set; }
        public bool? EvtInfoB01Chk12 { get; set; }
        public bool? EvtInfoB01Chk13 { get; set; }
        public bool? EvtInfoB01Chk14 { get; set; }
        public bool? EvtInfoB01Chk15 { get; set; }
        public bool? EvtInfoB01Chk16 { get; set; }
        public bool? EvtInfoB01Chk17 { get; set; }
        public bool? EvtInfoB01Chk18 { get; set; }
        public bool? EvtInfoB01Chk19 { get; set; }
        public bool? EvtInfoB01Chk20 { get; set; }
        public string EvtInfoB02 { get; set; }
        public string EvtInfoB0201 { get; set; }
        public int? EvtInfoB0202A { get; set; }
        public int? EvtInfoB0202B { get; set; }
        public string EvtInfoB0202C { get; set; }
        public string EvtInfoB0203 { get; set; }
        public string EvtInfoB0301 { get; set; }
        public int? EvtInfoB0302A { get; set; }
        public int? EvtInfoB0302B { get; set; }
        public string EvtInfoB0303 { get; set; }
        public int? EvtInfoB0401 { get; set; }
        public int? EvtInfoB0402 { get; set; }
        public string EvtInfoB0403 { get; set; }
        public string EvtInfoB0404 { get; set; }
        public int? EvtInfoB0501 { get; set; }
        public int? EvtInfoB0502 { get; set; }
        public string EvtInfoB0503 { get; set; }
        public string EvtInfoC01 { get; set; }
        public bool? EvtInfoC01Chk01 { get; set; }
        public bool? EvtInfoC01Chk02 { get; set; }
        public bool? EvtInfoC01Chk03 { get; set; }
        public bool? EvtInfoC01Chk04 { get; set; }
        public bool? EvtInfoC01Chk05 { get; set; }
        public bool? EvtInfoC01Chk06 { get; set; }
        public bool? EvtInfoC01Chk07 { get; set; }
        public bool? EvtInfoC01Chk08 { get; set; }
        public bool? EvtInfoC01Chk09 { get; set; }
        public bool? EvtInfoC01Chk10 { get; set; }
        public bool? EvtInfoC01Chk11 { get; set; }
        public bool? EvtInfoC01Chk12 { get; set; }
        public bool? EvtInfoC01Chk13 { get; set; }
        public bool? EvtInfoC01Chk14 { get; set; }
        public bool? EvtInfoC01Chk15 { get; set; }
        public bool? EvtInfoC01Chk16 { get; set; }
        public bool? EvtInfoC01Chk17 { get; set; }
        public bool? EvtInfoC01Chk18 { get; set; }
        public bool? EvtInfoC01Chk19 { get; set; }
        public bool? EvtInfoC01Chk20 { get; set; }
        public bool? EvtInfoC02Chk01 { get; set; }
        public bool? EvtInfoC02Chk02 { get; set; }
        public bool? EvtInfoC02Chk03 { get; set; }
        public bool? EvtInfoC03Chk01 { get; set; }
        public bool? EvtInfoC03Chk02 { get; set; }
        public bool? EvtInfoC03Chk03 { get; set; }
        public bool? EvtInfoC03Chk04 { get; set; }
        public bool? EvtInfoC04Chk01 { get; set; }
        public bool? EvtInfoC04Chk02 { get; set; }
        public bool? EvtInfoC04Chk03 { get; set; }
        public bool? EvtInfoC04Chk04 { get; set; }
        public int? EvtInfoC05 { get; set; }
        public bool? EvtInfoC06Chk01 { get; set; }
        public bool? EvtInfoC06Chk02 { get; set; }
        public bool? EvtInfoC06Chk03 { get; set; }
        public bool? EvtInfoC06Chk04 { get; set; }
        public int? EvtInfoC0701 { get; set; }
        public decimal? EvtInfoC0702 { get; set; }
        public string EvtInfoC0801 { get; set; }
        public string EvtInfoC0802 { get; set; }
        public string EvtInfoC0803 { get; set; }
        public string EvtInfoC0804 { get; set; }
        public string EvtInfoC0805 { get; set; }
        public string EvtInfoC0806 { get; set; }
        public string EvtInfoC0807 { get; set; }
        public string EvtInfoC0808 { get; set; }
        public string EvtInfoC0809 { get; set; }
        public string EvtInfoC0901 { get; set; }
        public int? EvtInfoC0902 { get; set; }
        public bool? EvtInfoC0902Chk01 { get; set; }
        public bool? EvtInfoC0902Chk02 { get; set; }
        public bool? EvtInfoC0902Chk03 { get; set; }
        public bool? EvtInfoC0902Chk04 { get; set; }
        public bool? EvtInfoC0902Chk05 { get; set; }
        public bool? EvtInfoC0902Chk06 { get; set; }
        public bool? EvtInfoC0902Chk07 { get; set; }
        public bool? EvtInfoC0902Chk08 { get; set; }
        public bool? EvtInfoC0903Chk01 { get; set; }
        public bool? EvtInfoC0903Chk02 { get; set; }
        public bool? EvtInfoC0903Chk03 { get; set; }
        public bool? EvtInfoC0903Chk04 { get; set; }
        public bool? EvtInfoC0903Chk05 { get; set; }
        public bool? EvtInfoC0903Chk06 { get; set; }
        public bool? EvtInfoC0904Chk01 { get; set; }
        public bool? EvtInfoC0904Chk02 { get; set; }
        public bool? EvtInfoC0904Chk03 { get; set; }
        public bool? EvtInfoC0904Chk04 { get; set; }
        public bool? EvtInfoC0904Chk05 { get; set; }
        public bool? EvtInfoC0904Chk06 { get; set; }
        public bool? EvtInfoC0904Chk07 { get; set; }
        public string EvtInfoC090501 { get; set; }
        public string EvtInfoC090502 { get; set; }
        public string EvtInfoC090503 { get; set; }
        public string EvtInfoC090504 { get; set; }
        public string EvtInfoC090601 { get; set; }
        public int? EvtInfoC090602 { get; set; }
        public string DealerCode { get; set; }
        public string OutletCode { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? PartReceivedDate { get; set; }
        public string FileID1 { get; set; }
        public string FileID2 { get; set; }
        public string FileID3 { get; set; }
        public string FileID4 { get; set; }
        public string FileID5 { get; set; }
        public string FileName1 { get; set; }
        public string FileName2 { get; set; }
        public string FileName3 { get; set; }
        public string FileName4 { get; set; }
        public string FileName5 { get; set; }
        public string AirWayBillNo { get; set; }
        public string DikirimVia { get; set; }

    }

    public class SvFtir2
    {
        [Key]
        public string FtirNo { get; set; }
        public string DealerCode { get; set; }
        public string OutletCode { get; set; }
        public DateTime? FtirDate { get; set; }
        public DateTime? FtirEventDate { get; set; }
        public string FtirMaker { get; set; }
        public string FtirCategory { get; set; }
        public DateTime? FtirRegDate { get; set; }
        public string DealerName { get; set; }
        public string VinNo { get; set; }
        public string Model { get; set; }
        public string Machine { get; set; }
        public string TransmNo { get; set; }
        public string TitleCategory { get; set; }
        public string TitleName { get; set; }
        public int? Odometer { get; set; }
        public int? UsageTime { get; set; }
        public bool? IsAvailPartDmg { get; set; }
        public bool? IsReportToSis { get; set; }
        public DateTime? EstimatedDelivery { get; set; }
        public string NotSendingCategory { get; set; }

    }

    public class VinModel
    {
        public string Model { get; set; }
        public string Engine { get; set; }
        public string Transmision { get; set; }
    }

    //public class FtirFileInfo
    //{
    //    [Key]
    //    [Column(Order = 1)]
    //    public string FileID { get; set; }
    //    public string FileName { get; set; }
    //    public string FileType { get; set; }
    //    public int FileSize { get; set; }
    //}
}