namespace eXpressAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QA_CATEGORIES")]
    public partial class Category : BaseEntityObject
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