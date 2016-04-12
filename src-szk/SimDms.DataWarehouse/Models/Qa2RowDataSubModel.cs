using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("qa2TrQuestionnaireSub")]
    public class Qa2RowDataSubModel
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string BranchCode { get; set; }
        [Key]
        [Column(Order = 3)]
        public string ChassisCode { get; set; }
        [Key]
        [Column(Order = 4)]
        public string ChassisNo { get; set; }

        public string IsAdditionalMerkCode { get; set; }
        public string IsAdditionalMerkDescI { get; set; }
        public string IsAdditionalMerkDescE { get; set; }
        public string IsAdditionalMerkOthers { get; set; }
        public string IsAdditionalType { get; set; }
        public Decimal IsAdditionalYear { get; set; }
    }
}