namespace eXpressAPI
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SM_RoleMenu")]
    public partial class RoleMenu
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string RoleId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string MenuId { get; set; }

        [DefaultValue("0")]
        public int Status { get; set; }

        [DefaultValue("0")]
        public int IsEditable { get; set; }

        [StringLength(20)]
        [DefaultValue("1111111111")]
        public string AllowInfo { get; set; }  

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(100)]
        public string ChangeBy { get; set; }

        public DateTime? ChangeDate { get; set; }
    }
}
