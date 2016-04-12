using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class UnitInfo
    {
        public long? No { get; set; }
        public string JobType { get; set; }
        public int? Unit { get; set; }
        public int? Invoice { get; set; }
        public int? Task { get; set; }
    }
}