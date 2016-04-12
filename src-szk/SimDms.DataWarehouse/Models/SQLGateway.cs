using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.DataWarehouse.Models
{
     [Table("SysSQLGateway")]
    public class SysSQLGateway
    {
        [Key]
        public string TaskNo { get; set; }
        public string TaskName { get; set; }
        public string DealerCode { get; set; }
        public string SQL { get; set; }
        public string FileName { get; set; }

        [DefaultValue(0)]
        public int Status { get; set; }

        public string UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ExecuteTime { get; set; }
    }


}