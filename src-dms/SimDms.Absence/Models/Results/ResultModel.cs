using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models.Results
{
    public class ResultModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public object data { get; set; }
    }
}