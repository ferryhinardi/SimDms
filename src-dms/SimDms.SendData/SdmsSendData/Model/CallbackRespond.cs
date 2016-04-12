using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sim.Dms.SendData.Model
{
    public class CallbackResponse
    {
        public bool Success { get; set; }
        public string CompanyCode { get; set; }
        public string DataType { get; set; }
        public string Message { get; set; }
        public string ResponseData { get; set; }
    }
}
