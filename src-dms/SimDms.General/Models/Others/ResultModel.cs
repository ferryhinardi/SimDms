using System;

namespace SimDms.General.Models.Others
{
    public class ResultModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string details { get; set; }
        public object data { get; set; }
    }
}