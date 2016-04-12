namespace eXpressAPI
{
    using eXpressAPI.Models;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SM_Users")]
    public partial class Users
    {
        [Key]
        [StringLength(20)]
        public string UserId { get; set; }

        [StringLength(20)]
        public string RoleId { get; set; }

        [StringLength(32)]
        public string Code { get; set; }

        [StringLength(64)]
        public string Name { get; set; }

        [Column(TypeName = "numeric")]
        public int? StatusUsers { get; set; }

        [StringLength(64)]
        public string Pass { get; set; }

        [Column(TypeName = "numeric")]
        public int? IsLogin { get; set; }

        public DateTime? LoginDate { get; set; }

        [StringLength(32)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(32)]
        public string ChangeBy { get; set; }
        
        public DateTime? ChangeDate { get; set; }

        public string Contact { get; set; }

        [NotMapped]
        public string Google { get; set; }
    }

    [Table("vw_users")]
    public partial class VwUser : Users
    {
        public int IsSalary { get; set; }
    }

    [Table("Config_Numbers")]
    public partial class ConfigNumber : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(90)]
        public string Name { get; set; }

        [StringLength(5)]
        public string Prefix { get; set; }

        public int LastNumber { get; set; }

        [StringLength(6)]
        public string LastYM { get; set; }

        [StringLength(25)]
        public string Format { get; set; }

        public int Digit { get; set; }

        public bool UseYear { get; set; }

        public bool UseMonth { get; set; }
    }
}
