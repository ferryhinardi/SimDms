using System;

namespace SimDms.DataWarehouse.Models
{
    public class ScShBmTraining
    {
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public int? SC { get; set; }
        public int? SH { get; set; }
        public int? BM { get; set; }
        public int? SCBasic { get; set; }
        public int? SCAdvance { get; set; }
        public int? SHBasic { get; set; }
        public int? SHIntermediate { get; set; }
        public int? SHAdvance { get; set; }
        public int? BMBasic { get; set; }
        public int? BMIntermediate { get; set; }
        public int? BMAdvance { get; set; }
    }
}