namespace eXpressAPI.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DOC_DEPARTMENT")]
    public partial class Department : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(10)]
        public string Abbr { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public DateTime? EffectiveDate { get; set; }
    }
    
    [Table("SM_Department")]
    public partial class Departement : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(32)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Parent { get; set; }

        public int Level { get; set; }
    }
}
