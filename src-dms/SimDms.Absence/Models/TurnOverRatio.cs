using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Absence.Models
{
    public class TurnOverRatio
    {
        public String DealerAbbreviation { get; set; }
        public String OutletAbbreviation { get; set; }
        public Decimal? Ratio { get; set; }
        public Int32? StartEmployeeCount { get; set; }
        public Int32? EmployeeIn { get; set; }
        public Int32? StartPlatinum { get; set; }
        public Int32? StartGold { get; set; }
        public Int32? StartSilver { get; set; }
        public Int32? StartTrainee { get; set; }
        public Int32? LoyalCount { get; set; }
        public Int32? EndEmployeeCount { get; set; }
        public Int32? EmployeeOut { get; set; }
        public Int32? EndPlatinum { get; set; }
        public Int32? EndGold { get; set; }
        public Int32? EndSilver { get; set; }
        public Int32? EndTrainee { get; set; }
    }
}