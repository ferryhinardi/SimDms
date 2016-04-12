using System;

namespace SimDms.DataWarehouse.Models
{
    public class ReviewSfm
    {
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public int? Trainee { get; set; }
        public int? Silver { get; set; }
        public int? Gold { get; set; }
        public int? Platinum { get; set; }
        public int? TotalSalesman { get; set; }
        public int? SC { get; set; }
        public int? SH { get; set; }
        public int? BM { get; set; }
        public int? TotalSCSHBM { get; set; }
    }
}