namespace eXpressAPI.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QA_CONTACT")]
    public partial class Contact : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(10)]
        public string No { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Code { get; set; }

        [Index(IsUnique = true)] 
        [StringLength(30)]
        public string ContactId { get; set; }

        [StringLength(32)]
        public string ContactType { get; set; }

        [StringLength(60)]
        public string Name { get; set; }  

        [StringLength(50)]
        public string Position { get; set; }

        [StringLength(50)]
        public string WorkPhone { get; set; }

        [StringLength(50)]
        public string MobileNo { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(250)]
        public string Keterangan { get; set; }
    }

    [Table("QA_PROJECTS")]
    public partial class Project : BaseEntityObject
    {
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Location { get; set; }

        [StringLength(20)]
        public string CustomerCode { get; set; }

        [StringLength(20)]
        public string SalesCode { get; set; }

        [StringLength(20)]
        public string Contact { get; set; }

        public DateTime  RegisteredDate { get; set; }

        [StringLength(250)]
        public string Keterangan { get; set; }
    }

    [Table("vw_projects")]
    public partial class VwProject : BaseEntityObject
    {
        [Key]
        [Column(Order = 2)]
        [StringLength(20)]
        public string Code { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        public DateTime RegisteredDate { get; set; }
        [StringLength(250)]
        public string Keterangan { get; set; }
        [StringLength(50)]
        public string Location { get; set; }

        [StringLength(20)]
        public string CustomerCode { get; set; }
        [StringLength(100)]
        public string Customer { get; set; }
        [StringLength(10)]
        public string Abbr { get; set; }
        [StringLength(250)]
        public string Address { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(80)]
        public string Email { get; set; }

        [StringLength(60)]
        public string Sales { get; set; }
        [StringLength(50)]
        public string Workphone_s { get; set; }
        [StringLength(50)]
        public string Mobile_s { get; set; }
        [StringLength(100)]
        public string Email_s { get; set; }

        [StringLength(60)]
        public string Contact { get; set; }
        [StringLength(50)]
        public string Workphone_c { get; set; }
        [StringLength(50)]
        public string Mobile_c { get; set; }
        [StringLength(100)]
        public string Email_c { get; set; }
    }
}
