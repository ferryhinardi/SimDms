namespace eXpressAPI.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QA_SPECS")]
    public partial class SPECS : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string No { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string TypeId { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(30)]
        public string ItemId { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(2)]
        public string SpecCategoryCode { get; set; }

        [StringLength(80)]
        public string SpecCategory { get; set; }

        [NotMapped]
        public string ItemName { get; set; }
        //public int GroupNo { get; set; }

        //[StringLength(100)]
        //public string GroupType { get; set; }

        [StringLength(100)]
        public string Refference { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(250)]
        public string Value { get; set; }

        [StringLength(10)]
        public string Currency { get; set; }

        [StringLength(20)]
        public string UOM { get; set; }

        [Column(TypeName = "money")]
        [DefaultValue("0")]
        public decimal? Price { get; set; }

        [Column(TypeName = "money")]
        [DefaultValue("0")]
        public decimal? Qty { get; set; }

        [Column(TypeName = "money")]
        [DefaultValue("0")]
        public decimal? Discount { get; set; }  

        [Column(TypeName = "money")]
        [DefaultValue("0")]
        public decimal? Total { get; set; }

        public bool? IsHeader { get; set; }

        [StringLength(50)]
        public string GroupHeader { get; set; }

        public bool? Printable { get; set; }
    }

    [Table("QA_SPECS_INFO")]
    public partial class SPECINFO : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(3)]
        public string GroupNo { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(30)]
        public string TypeId { get; set; }

        [StringLength(80)]
        public string SpecCategory { get; set; }

        [StringLength(100)]
        public string GroupType { get; set; }
    }

    public class SearchSPEC
    {
        public string typeid { get; set; }
        public string specid { get; set; }
        public string search { get; set; }
    }
}
