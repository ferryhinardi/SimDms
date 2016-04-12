using System;

namespace SimDms.DataWarehouse.Models
{
    public class SalesmanTraining
    {
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public int? Trainee { get; set; }
        public int? Silver { get; set; }
        public int? Gold { get; set; }
        public int? Platinum { get; set; }
        public int? TotalSalesman { get; set; }
        public int? GoldTerminated { get; set; }
        public int? PlatinumTerminated { get; set; }
        public int? STDP1 { get; set; }
        public int? STDP2 { get; set; }
        public int? STDP3 { get; set; }
        public int? STDP4 { get; set; }
        public int? STDP5 { get; set; }
        public int? STDP6 { get; set; }
        public int? STDP7 { get; set; }
        public int? TotalSTDP { get; set; }
        public int? SPSSilver { get; set; }
        public int? SPSGold { get; set; }
        public int? SPSPlatinum { get; set; }

    }
}