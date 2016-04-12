using System;

namespace SimDms.Sales.Models.Result
{
    public class ResultModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public object data { get; set; }
    }
}