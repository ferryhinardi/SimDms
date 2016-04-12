using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sim.Dms.SendData.Model
{
    [Table("GnMstSendScheduleDms")]
    public class SendScheduleDms
    {
        [Key]
        [Column(Order = 1)]
        public string CompanyCode { get; set; }
        [Key]
        [Column(Order = 2)]
        public string DataType { get; set; }
        public DateTime? LastSendDate { get; set; }
        public int SegmentNo { get; set; }
        public string SPName { get; set; }
        public int Priority { get; set; }
        public char Status { get; set; }
        public string ColumnLastUpdate { get; set; }
    }
}
