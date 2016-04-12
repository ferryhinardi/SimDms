namespace eXpressAPI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SM_RoleCompany")]
    public partial class RoleCompany
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string RoleId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string CompanyId { get; set; }

        [StringLength(1)]
        public string Status { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(100)]
        public string ChangeBy { get; set; }

        public DateTime? ChangeDate { get; set; }
    }
}
