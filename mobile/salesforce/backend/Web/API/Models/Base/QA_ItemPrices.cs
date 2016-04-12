namespace eXpressAPI.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QA_ITEM_PRICES")]
    public partial class ItemPrice : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string ItemId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string Description { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string UOM { get; set; }
        
        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string Currency { get; set; }

        [Column(TypeName = "money")]
        [DefaultValue("0")]
        public decimal Price { get; set; }

        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }

    [Table("vw_prices")]
    public partial class VwPrice : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string ItemId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(100)]
        public string Description { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(20)]
        public string UOM { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(10)]
        public string Currency { get; set; }

        public string CurrName { get; set; }

        public string LastRef { get; set; }

        public string LastCurr { get; set; }

        [Column(TypeName = "money")]
        [DefaultValue("0")]
        public decimal Price { get; set; }

        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }

    public class EntryItemPrice
    {
        public string ItemId { get; set; }
        public string Description { get; set; }
        public string UOM { get; set; }
        public string Currency { get; set; }
        public decimal Price { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string LastRef { get; set; }
        public string LastCurr { get; set; }
        public string CurrName { get; set; }
    }
}
