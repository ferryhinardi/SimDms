using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class Qa2RowModel
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string StatusKonsumenDescE { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string RefferenceDesc1 { get; set; }
        public string SalesModelReport { get; set; }
        public string RespondenName { get; set; }
        public DateTime BirthDate { get; set; }
        public string RespondenAge { get; set; }
        public string RespondenGender { get; set; }
        public string LookUpValueName { get; set; }
        public string IsCredit { get; set; }
        public string Installment { get; set; }
        public string OccupationDescE { get; set; }
        public string ProductSourceDescE { get; set; }
        public string TestDrivedescE { get; set; }
        public string RespondenStatusDescE { get; set; }
        public string FirstTimeDescE { get; set; }
        public string IsReplacementMerkDescE { get; set; }
        public string IsReplacementMerkOthers { get; set; }
        public string IsReplacementType { get; set; }
        public string IsReplacementYear { get; set; }
        public string IsReplacementReasonDescE { get; set; }
        public string ComparisonDescE { get; set; }
        public string AspectBrand { get; set; }
        public string AspectEngine { get; set; }
        public string AspectExterior { get; set; }
        public string AspectInterior { get; set; }
        public string AspectPrice { get; set; }
        public string AspectAfterSales { get; set; }
        public string AspectOutlet { get; set; }
        public string AspectResalePrice { get; set; }
        public string AspectOthersDetail { get; set; }
        public string AspectOthers { get; set; }

        public string PembelianAtasNamaDescE { get; set; }
        public string JenisUsaha { get; set; }
        public string PurposeDescE { get; set; }
        public string PeriodDescE { get; set; }
        public string RenovationDescE { get; set; }

        public string EmployeeName{get;set;}
        public string OccupationOthers{get;set;}
        public string ProductSourceDetail {get;set;}
        public string IsAdditionalTotal{get;set;}
        public string IsReplacementReasonOthers{get;set;}
        public string ComparisonOthers{get;set;}
        public string PurposeOthers{get;set;}
    }
}