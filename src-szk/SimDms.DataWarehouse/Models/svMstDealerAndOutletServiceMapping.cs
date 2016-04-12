using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    [Table("svMstDealerAndOutletServiceMapping")]
    public class svMstDealerAndOutletServiceMapping
    {
        [Key]
        [Column(Order = 1)]
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public bool? isMD { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SeqNo { get; set; }
    }
}