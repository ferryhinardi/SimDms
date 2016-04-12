using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models.General
{
    public class ResultModel
    {
        public bool success { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public object data { get; set; }
    }
}