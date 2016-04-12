using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.Absence.Models
{
    public class DataContext : DbContext
    {
        public DataContext(string ConnString)
            : base(ConnString)
        {
        }

        public DataContext() :
            base(MyHelpers.GetConnString("DataContext"))
        {

        }


        protected Exception HandleDataUpdateException(DbUpdateException exception)
        {
            Exception innerException = exception.InnerException;

            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }
            
            Elmah.ErrorSignal.FromCurrentContext().Raise(innerException);
            return innerException;
        }

        protected Exception HandleDataValidationException(DbEntityValidationException exception)
        {
            var stringBuilder = new StringBuilder();

            foreach (DbEntityValidationResult result in exception.EntityValidationErrors)
            {
                foreach (DbValidationError error in result.ValidationErrors)
                {
                    stringBuilder.AppendFormat("{0} [{1}]: {2}",
                        result.Entry.Entity.ToString().Split('.').Last(), error.PropertyName, error.ErrorMessage);
                    stringBuilder.AppendLine();
                }
            }

            var ex = new Exception(stringBuilder.ToString().Trim());
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            return ex;
        }


        public override int SaveChanges()
        {
            int n = 0;

            try
            {
                n = base.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                throw  HandleDataValidationException(vex);
            }
            catch (DbUpdateException dbu)
            {
                throw  HandleDataUpdateException(dbu);
            }

            return n;
        }

        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysRole> SysRoles { get; set; }
        public IDbSet<OrganizationHdr> OrganizationHdrs { get; set; }
        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<Holiday> Holidays { get; set; }
        public IDbSet<Shift> Shifts { get; set; }
        public IDbSet<GnMstOrgGroup> GnMstOrgGroups { get; set; }
        public IDbSet<GnMstCoProfile> GnMstCoProfiles { get; set; }
        public IDbSet<GnMstPosition> GnMstPositions { get; set; }
        public IDbSet<GnMstLookupDtl> GnMstLookupDtls { get; set; }
        public IDbSet<GnMstZipCode> GnMstZipCodes { get; set; }
        public IDbSet<HrEmployee> HrEmployees { get; set; }
        public IDbSet<HrEmployeeSales> HrEmployeeSales { get; set; }
        public IDbSet<HrEmployeeService> HrEmployeeService { get; set; }
        public IDbSet<HrEmployeeMutation> HrEmployeeMutations { get; set; }
        public IDbSet<HrEmployeeView> HrEmployeeViews { get; set; }
        public IDbSet<OrgGroup> OrgGroups { get; set; }
        public IDbSet<Position> Positions { get; set; }
        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
        public IDbSet<HrTrnAttendanceFileDtl> HrTrnAttendanceFileDtls { get; set; }
        public IDbSet<HrTrnAttendanceFileHdrView> HrTrnAttendanceFileHdrViews { get; set; }
        public IDbSet<HrTrnAttendanceFileDtlView> HrTrnAttendanceFileDtlViews { get; set; }
        public IDbSet<HrEmployeeShift> HrEmployeeShifts { get; set; }
        public IDbSet<HrEmployeeShiftView> HrEmployeeShiftView { get; set; }
        public IDbSet<HrUploadedFile> HrUploadedFiles { get; set; }
        public IDbSet<HrTrnAttendanceFileHdr> HrTrnAttendanceFileHdrs { get; set; }
        public IDbSet<HrEmployeeEducation> HrEmployeeEducations { get; set; }
        public IDbSet<HrEmployeeExperience> HrEmployeeExperiences { get; set; }
        public IDbSet<HrEmployeeAdditionalJob> HrEmployeeAdditionalJobs { get; set; }
        public IDbSet<HrEmployeeAdditionalBranch> HrEmployeeAdditionalBranches { get; set; }
        public IDbSet<HrEmployeeAchievement> HrEmployeeAchievements { get; set; }
        public IDbSet<HrEmployeeVehicle> HrEmployeeVehicles { get; set; }
        public IDbSet<HrEmployeeTraining> HrEmployeeTrainings { get; set; }
        public IDbSet<HrEmployeeTrainingView> HrEmployeeTrainingViews { get; set; }
        public IDbSet<HrMstTraining> HrMstTrainings { get; set; }
        public IDbSet<HrDepartmentTraining> HrDepartmentTraining { get; set; }
        public IDbSet<HrEmployeeAchievementView> HrEmployeeAchievementViews { get; set; }
        public IDbSet<HrLookupMapping> HrLookupMappings { get; set; }
        public IDbSet<HrLookupView> HrLookupViews { get; set; }
        public IDbSet<GnMstDealerMapping> GnMstDealerMappings { get; set; }
        public IDbSet<GnMstDealerOutletMapping> GnMstDealerOutletMappings { get; set; }
        public IDbSet<SysLog> SysLogs { get; set; }
        public IDbSet<PmKdp> PmKdps { get; set; }
        public IDbSet<DealerInfo> DealerList { get; set; }

    }

    public class DocumentContext : DbContext
    {
        public IDbSet<GnMstEmployeeDocument> GnMstEmployeeDocuments { get; set; }
        public IDbSet<HrAbsenceFile> HrAbsenceFiles { get; set; }
    }

}