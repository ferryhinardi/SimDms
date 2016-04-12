using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
    [Table("CsSettings")]
    public class CsSetting
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string SettingCode { get; set; }
        public string SettingDesc { get; set; }
        public string SettingParam1 { get; set; }
        public string SettingParam2 { get; set; }
        public string SettingParam3 { get; set; }
        public string SettingParam4 { get; set; }
        public string SettingParam5 { get; set; }
        public string SettingLink1 { get; set; }
        public string SettingLink2 { get; set; }
        public string SettingLink3 { get; set; }
        public string IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}