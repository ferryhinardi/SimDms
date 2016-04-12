using System;

namespace SimDms.DataWarehouse.Models
{
    public class TurnOver
    {
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public int? Trainee { get; set; }
        public int? Silver { get; set; }
        public int? Gold { get; set; }
        public int? Platinum { get; set; }
        public int? TraineeTerminated { get; set; }
        public int? SilverTerminated { get; set; }
        public int? GoldTerminated { get; set; }
        public int? PlatinumTerminated { get; set; }
    }
}