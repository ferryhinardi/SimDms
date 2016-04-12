using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sim.Dms.SendData.Model
{
    public class TokenAccess
    {
        public string CompanyCode { get; set; }
        public string TokenID { get; set; }
        public DateTime ExpiredDate { get; set; }
        public string ComputerName { get; set; }
    }
}
