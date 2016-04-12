using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    public class ItsWithStatusAndTestDrive
    {
        public string Dealer { get; set; }
        public string Abbr { get; set; }
        public string Date { get; set; }
        public string Model { get; set; }
        public string Var { get; set; }
        public int? INQ { get; set; }
        public int? InqTestDrive { get; set; }
        public int? SPK { get; set; }
        public int? SPKTestDrive { get; set; }
        public int? LOST { get; set; }
    }
}