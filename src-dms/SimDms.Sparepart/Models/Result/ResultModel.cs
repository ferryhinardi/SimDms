using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.Models.Result
{
    public class ResultModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public object data { get; set; }
    }
}
