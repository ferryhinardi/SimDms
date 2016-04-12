using System;

namespace SimDms.DataWarehouse.Models
{
    public class OutstandingTraining
    {
        public string Department { get; set; }
        public string Position { get; set; }
        public string Grade { get; set; }
        public string GradeName { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public string TrainingDescription { get; set; }
        public int? ManPower { get; set; }
        public int? Trained { get; set; }
        public int? NotTrained { get; set; }
    }
}