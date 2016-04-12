using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class InqKdpCouponView
    {
        public string Area { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TestDriveDate { get; set; }
        public string CoupunNumber { get; set; }
        public string NamaProspek { get; set; }
        public int ProspekIdentityNo { get; set; }
        public string AlamatProspek { get; set; }
        public string TelpRumah { get; set; }
        public string Email { get; set; }
        public string EmployeeName { get; set; }
        public string SalesID { get; set; }
        public string IdentityNo { get; set; }
        public string OutletName { get; set; }
        public string OutletArea { get; set; }
        public string Remark { get; set; }
    }
}