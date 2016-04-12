namespace eXpressAPI.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DOC_DETAIL")]
    public partial class DocDetail : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        public string DocNo { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(2)]
        public string No { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(50)]
        public string FileId { get; set; }

        [StringLength(50)]
        public string FileType { get; set; }

        [StringLength(250)]
        public string Keyword { get; set; }

        [DefaultValue("0")]
        public bool IsDisplay { get; set; }

        [DefaultValue("0")]
        public bool IsPrivate { get; set; }
    }
}
