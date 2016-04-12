using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.DataWarehouse.Models
{
    public class QaRowModel
    {
        public string Area { get; set; }
        public string CompanyName { get; set; }
        public string BranchName { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ChassisCode { get; set; }
        public string ChassisNo { get; set; }
        public string RespondenName { get; set; }
        public string RespondenAge { get; set; }
        public string RespondenGender { get; set; }
        public string IsCredit { get; set; }
        public string SalesModelReport { get; set; }
        public string RespondenStatusDescE { get; set; }
        public string IsReplacementMerkDescE { get; set; }
        public string IsReplacementTypeDescE { get; set; }
        public string IsReplacementReasonDescE { get; set; }
        public string IsAdditionalSuzuki { get; set; }
        public string IsAdditionalDaihatsu { get; set; }
        public string IsAdditionalMitsubishi { get; set; }
        public string IsAdditionalOther { get; set; }
        public string IsAdditionalTotal { get; set; }
        public string LoadCapacityDescE { get; set; }
        public string AnnualDriveDescE { get; set; }
        public string FirstTimeDescE { get; set; }
        public string OccupationDescE { get; set; }
        public string OccupationDetailDescE { get; set; }
        public string PassengerCarDescE { get; set; }
        public string PassengerCarUnitDescE { get; set; }
        public string IsPassengerCarToyota { get; set; }
        public string IsPassengerCarHonda { get; set; }
        public string IsPassengerCarDaihatsu { get; set; }
        public string IsPassengerCarSuzuki { get; set; }
        public string IsPassengerCarOthers { get; set; }
        public string MotorCycleDescE { get; set; }
        public string StatusKonsumenDescE { get; set; }
        public string RespondenPhone { get; set; }
    }
}