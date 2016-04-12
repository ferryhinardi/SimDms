using System;

namespace SimDms.PreSales.Models.Result
{
    public class ValidationResult
    {
        public bool? Status { get; set; }
        public string Message { get; set; }
    }

    public class ITSValidationResult
    {
        public bool? Status { get; set; }
        public string Message { get; set; }
        public string SalesCoordinator { get; set; }
        public string SalesHead { get; set; }
        public string BranchManager { get; set; }
        public string GeneralManager { get; set; }
        public string Grade { get; set; }
    }
}