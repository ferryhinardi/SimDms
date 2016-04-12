namespace eXpressAPI.Models
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("QA_CUSTOMERS")]
    public partial class Customer : BaseCompany
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(80)]
        public string Email { get; set; }

        [DefaultValue("0")]
        public decimal Discount { get; set; }
    }

    [Table("QA_PROSPEK")]
    public partial class Prospek : BaseCompany
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }
    }

    [Table("SM_COMPANY_PROFILE")]
    public partial class CompanyProfile : BaseCompany
    {
        [Key]
        [StringLength(20)]
        public string Code { get; set; }
    }

    [Table("QA_VENDORS")]
    public partial class Vendor : BaseCompany
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string Code { get; set; }
    }
}
