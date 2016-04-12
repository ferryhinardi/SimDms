namespace eXpressAPI.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QA_ITEMS")]
    public partial class Item : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string Code { get; set; }

        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }        
        
        [StringLength(250)]
        public string Description2 { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(32)]
        public string Model { get; set; }

        [StringLength(70)]
        public string Mfg { get; set; }

        [StringLength(20)]
        public string Class { get; set; }

        [StringLength(20)]
        public string UOM { get; set; }

        [StringLength(20)]
        public string PartNo { get; set; }

        [StringLength(20)]
        public string MachineType { get; set; }

        [DefaultValue("0")]
        public bool Package { get; set; }
    }


    [Table("QA_ITEM_TYPES")]
    public partial class ItemType : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string Code { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }        
        
        [StringLength(250)]
        public string Description2 { get; set; }

        [StringLength(50)]
        public string Category { get; set; }

        [StringLength(32)]
        public string Model { get; set; }

        [StringLength(20)]
        public string OUM { get; set; }

        [StringLength(10)]
        public string Currency { get; set; }

        [Column(TypeName = "money")]
        [DefaultValue("0")]
        public decimal Price { get; set; }
    } 


    [Table("vw_items")]
    public partial class VwItem : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(30)]
        public string Code { get; set; }
        [StringLength(150)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Description { get; set; }        
        
        [StringLength(250)]
        public string Description2 { get; set; }
        [StringLength(50)]
        public string Category { get; set; }
        [StringLength(32)]
        public string Model { get; set; }
        [StringLength(70)]
        public string Mfg { get; set; }
        [StringLength(20)]
        public string Class { get; set; }
        [StringLength(20)]
        public string UOM { get; set; }
        [StringLength(20)]
        public string PartNo { get; set; }
        [StringLength(20)]
        public string MachineType { get; set; }
        [DefaultValue("0")]
        public bool Package { get; set; }


        public string UomName { get; set; }

        public string CategoryName { get; set; }

        public string ClassName { get; set; }

        public string Manufacturer { get; set; }
    }

     [Table("vw_item_types")]
     public partial class VwItemType : BaseEntityObject
     {
         [Key]
         [Column(Order = 1)]
         [StringLength(30)]
         public string Code { get; set; }
         [StringLength(100)]
         public string Name { get; set; }
         [StringLength(250)]
         public string Description { get; set; }
         [StringLength(250)]
         public string Description2 { get; set; }
         [StringLength(50)]
         public string Category { get; set; }
         [StringLength(32)]
         public string Model { get; set; }
         [StringLength(20)]
         public string OUM { get; set; }
         [StringLength(10)]
         public string Currency { get; set; }
         [Column(TypeName = "money")]
         [DefaultValue("0")]
         public decimal Price { get; set; }

         public string CategoryName { get; set; }
     }
}
