namespace eXpressAPI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

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
    }


}
