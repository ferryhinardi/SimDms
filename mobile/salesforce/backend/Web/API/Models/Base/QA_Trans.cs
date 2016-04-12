namespace eXpressAPI.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QA_HEADER")]
    public partial class QaHeader : BaseEntityObject
    {
        [StringLength(32)]
        public string DocNo { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Initial { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(32)]
        public string AutoNo { get; set; }

        [Display(Name = "Doc Date")]
        public DateTime DocDate { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [StringLength(20)]
        public string Customer { get; set; }

        [StringLength(30)]
        [Display(Name = "Type")]
        public string TypeId { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(20)]
        [Display(Name = "Doc Type")]
        public string DocType { get; set; }

        [StringLength(20)]
        public string Sales { get; set; }

        [StringLength(20)]
        public string Contact { get; set; } 

        [StringLength(20)]
        public string Project { get; set; }  

        [StringLength(20)]
        public string Revisi { get; set; }

        [DefaultValue("1")]
        public bool Draft { get; set; }  

        public double Discount { get; set; }

        [StringLength(500)]
        [Display(Name = "Header Message")]
        public string HeaderPrint { get; set; }

        [StringLength(500)]
        [Display(Name = "Footer Message")]
        public string FooterPrint { get; set; }

        [DefaultValue("0")]
        public bool Closed { get; set; }

        [DefaultValue("0")]
        [Display(Name = "Close Status")]
        public bool CloseStatus { get; set; }

        [Display(Name = "Close Date")]
        public DateTime? CloseDate { get; set; }

        [StringLength(2000)]
        [Display(Name = "Close Description")]
        public string CloseDescription { get; set; }

        [Display(Name = "Reff No")]
        public string ReffNo { get; set; }

        public string Link { get; set; }  
 
        public string Currency { get; set; }   
        public double? SubTotal { get; set; }
        public double? DiscountPrice { get; set; }
        public double? GrandTotal { get; set; }

        [NotMapped]
        [StringLength(2000)]
        public string ListDiscount { get; set; }

        [NotMapped]
        public bool DocType2 { get; set; }
    }

    
    [Table("QA_DETAIL")]
    public partial class QaDetail : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string DocNo { get; set; }

        [Key]
        [Column(Order = 2)]
        [DefaultValue("0")]
        public bool Draft { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(3)]
        public string No { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(2)]
        public string SpecCategoryCode { get; set; }
        
        [StringLength(100)]
        public string SpecCategory { get; set; }

        //public int GroupNo { get; set; }

        //[StringLength(100)]
        //public string GroupType { get; set; }  

        [StringLength(30)]
        public string ItemId { get; set; }

        [StringLength(100)]
        public string Refference { get; set; }

        [StringLength(150)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Value { get; set; }

        [StringLength(10)]
        public string Currency { get; set; }

        [StringLength(20)]
        public string UOM { get; set; }

        //[Column(TypeName = "money")]
        [DefaultValue("0")]
        public decimal Price { get; set; }

        //[Column(TypeName = "money")]
        [DefaultValue("0")]
        public decimal Qty { get; set; }

        [DefaultValue("0")]
        public decimal Disc { get; set; }  

        //[Column(TypeName = "money")]
        [DefaultValue("0")]
        public double Total { get; set; }
    }


    [Table("QA_DISCOUNT")]
    public partial class QaDiscount : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string TypeId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Currency { get; set; }

        public double? Discount { get; set; }

        [StringLength(750)]
        public string Description { get; set; }

        public double? Total { get; set; }
    }

    public class DiscountList
    {
        public string Currency { get; set; }
        public double Discount { get; set; }
    }

    [Table("vw_quotation")]
    public partial class VwQuotation : BaseEntityObject
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CompanyCode { get; set; }

        [StringLength(32)]
        public string DocNo { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Initial { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(32)]
        public string AutoNo { get; set; }

        [Key]
        [Column(Order = 4)]
        public DateTime DocDate { get; set; }

        [Key]
        [Column(Order = 5)]
        public DateTime DueDate { get; set; }

        [StringLength(20)]
        public string Customer { get; set; }

        [StringLength(30)]
        public string TypeId { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(20)]
        public string DocType { get; set; }

        [StringLength(20)]
        public string Sales { get; set; }

        [StringLength(20)]
        public string Contact { get; set; }

        [StringLength(20)]
        public string Project { get; set; }

        [StringLength(20)]
        public string Revisi { get; set; }

        [Key]
        [Column(Order = 6)]
        public bool Draft { get; set; }

        [Key]
        [Column(Order = 7)]
        public double Discount { get; set; }

        [StringLength(500)]
        public string HeaderPrint { get; set; }

        [StringLength(500)]
        public string FooterPrint { get; set; }

        [Key]
        [Column(Order = 8)]
        public bool Closed { get; set; }

        [Key]
        [Column(Order = 9)]
        public bool CloseStatus { get; set; }

        public DateTime? CloseDate { get; set; }

        [StringLength(2000)]
        public string CloseDescription { get; set; }

        public string ReffNo { get; set; }

        public string Link { get; set; }

        public string Currency { get; set; }

        [Key]
        [Column(Order = 10)]
        public double SubTotal { get; set; }

        [Key]
        [Column(Order = 11)]
        public double DiscountPrice { get; set; }

        [Key]
        [Column(Order = 12)]
        public double GrandTotal { get; set; }

        [Key]
        [Column(Order = 13)]
        public bool Status { get; set; }

        [Key]
        [Column(Order = 14)]
        public bool IsDeleted { get; set; }

        [StringLength(32)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(32)]
        public string ChangeBy { get; set; }

        public DateTime? ChangeDate { get; set; }

        [StringLength(32)]
        public string Model { get; set; }

        [StringLength(100)]
        public string TypeName { get; set; }

        [StringLength(100)]
        public string ProjectName { get; set; }

        [StringLength(50)]
        public string Location { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        [StringLength(10)]
        public string Abbr { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        [StringLength(75)]
        public string City { get; set; }

        [StringLength(8)]
        public string ZIP { get; set; }

        [StringLength(20)]
        public string PhoneNo { get; set; }

        [StringLength(20)]
        public string FaxNo { get; set; }

        [StringLength(50)]
        public string Website { get; set; }

        [StringLength(60)]
        public string ContactName { get; set; }

        [StringLength(50)]
        public string ContactPosition { get; set; }

        [StringLength(100)]
        public string ContactEmail { get; set; }

        [StringLength(50)]
        public string ContactHP { get; set; }

        [StringLength(50)]
        public string ContactPhone { get; set; }

        [StringLength(60)]
        public string SalesName { get; set; }

        [StringLength(100)]
        public string SalesEMail { get; set; }

        [StringLength(50)]
        public string SalesPosition { get; set; }

        [StringLength(50)]
        public string SalesPhone { get; set; }

        [StringLength(50)]
        public string SalesHP { get; set; }
    }
}
