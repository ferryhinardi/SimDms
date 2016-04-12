using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimDms.General.Models
{
    public class EmployeeMutationView
    {
        public string CompanyCode { get; set; }
        public string EmployeeID { get; set; }
        public string BranchCode { get; set; }
        public string MutationTo { get; set; }
        public string PersonnelStatus { get; set; }
        public string userID { get; set; }
    }
}