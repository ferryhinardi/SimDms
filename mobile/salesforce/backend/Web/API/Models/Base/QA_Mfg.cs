namespace eXpressAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QA_MFG")]
    public partial class Mfg : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(70)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Abbr { get; set; }

        [StringLength(250)]
        public string Description { get; set; }
    }
}
