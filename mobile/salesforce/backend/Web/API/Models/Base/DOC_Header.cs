namespace eXpressAPI.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DOC_HEADER")]
    public partial class DocHeader : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string DocNo { get; set; }

        public DateTime DocDate { get; set; }

        [StringLength(150)]
        public string Title { get; set; }

        public string DocType { get; set; }

        public string DeptId { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(250)]
        public string Keyword { get; set; }

        [StringLength(36)]
        public string ReffNo { get; set; }

        [DefaultValue("0")]
        public bool IsPrivate { get; set; }
    }
}
