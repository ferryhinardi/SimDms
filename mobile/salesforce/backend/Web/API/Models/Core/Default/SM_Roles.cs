namespace eXpressAPI
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SM_Roles")]
    public partial class Roles
    {
        [Key]
        [StringLength(20)]
        public string RoleId { get; set; }

        [StringLength(32)]
        public string Name { get; set; }

        public int StatusRole { get; set; }

        public int RoleByGrade { get; set; }

        public int RoleByLocation { get; set; }

        public int RoleByOutsource { get; set; }

        public int RoleByCompany { get; set; }

        public int IsSalary { get; set; }

        public int RoleByOrganization { get; set; }
        
        [StringLength(32)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(32)]
        public string ChangeBy { get; set; }

        public DateTime? ChangeDate { get; set; }
    }
}
