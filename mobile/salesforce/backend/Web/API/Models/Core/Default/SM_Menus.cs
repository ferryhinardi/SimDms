namespace eXpressAPI
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SM_Menus")]
    public partial class Menus
    {
        [Key]
        [StringLength(20)]
        public string MenuId { get; set; }

        [StringLength(32)]
        public string Name { get; set; }

        [StringLength(20)]
        public string Parent { get; set; }

        public int MenuLevel { get; set; }

        public int Seq { get; set; }

        public int StatusMenu { get; set; }

        [StringLength(64)]
        public string Link { get; set; }
        
        [StringLength(50)]
        public string MenuPict { get; set; }

        [StringLength(32)]
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        [StringLength(32)]
        public string ChangeBy { get; set; }

        public DateTime? ChangeDate { get; set; }
    }
}
