namespace eXpressAPI.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QA_SPECCATEGORY")]
    public partial class SpecCategory : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [DefaultValue("0")]
        public int GroupNo { get; set; }

        [DefaultValue("0")]
        public int Seq { get; set; }

        [DefaultValue("0")]
        public bool Include { get; set; }
    }

    [Table("QA_CLASSIFICATION")]
    public partial class Classification : BaseEntityObject
    {

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
    }   

    [Table("QA_DETAIL_TYPE")]
    public partial class QDetailType : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
    }
}
