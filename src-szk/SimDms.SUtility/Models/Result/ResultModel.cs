using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.SUtility.Models.Result
{
    public class ResultModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string innerMessage { get; set; }
        public object data { get; set; }
    }
}