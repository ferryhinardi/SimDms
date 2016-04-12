using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sim.Dms.SendData.Model
{
    public class GnMstScheduleData
    {
        public string UniqueID { get; set; }
        public string CompanyCode { get; set; }
        public string DataType { get; set; }
        public int Segment { get; set; }
        public string Data { get; set; }
        public DateTime LastSendDate { get; set; }
        public char Status { get; set; }
    }
}
