using SimDms.Common;
using SimDms.Common.Models;
using System;
using System.Data.Entity;
using System.Web;
using System.Data.Entity.Validation;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Linq;
using SimDms.Sparepart.Models;

namespace SimDms.General.Models
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

            //Elmah.ErrorSignal.FromCurrentContext().Raise(innerException);
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
            //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
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
                throw HandleDataValidationException(vex);
            }
            catch (DbUpdateException dbu)
            {
                throw HandleDataUpdateException(dbu);
            }

            return n;
        }

        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysRoleUser> SysRoleUsers { get; set; }
        public IDbSet<SysUserView> SysUserViews { get; set; }
        public IDbSet<SysRole> SysRoles { get; set; }
        public IDbSet<SysMenu> SysMenus { get; set; }
        public IDbSet<SysRoleMenu> SysRoleMenus { get; set; }
        public IDbSet<SysMessageBoard> SysMessageBoards { get; set; }
        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<CoProfileFinance> CoProfileFinance { get; set; }
        public IDbSet<SimDms.Sparepart.Models.GnMstCoProfileSpare> CoProfileSpare { get; set; }
        public IDbSet<SimDms.Sparepart.Models.GnMstCoProfileService> CoProfileService { get; set; }
        public IDbSet<SimDms.Sparepart.Models.GnMstCoProfileSales>CoProfileSales { get; set; }
        
        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
        public IDbSet<OrganizationHdr> OrganizationHdrs { get; set; }
        public IDbSet<SysModule> SysModules { get; set; }
        public IDbSet<SysModuleView> SysModuleViews { get; set; }
        public IDbSet<SysRoleModule> SysRoleModules { get; set; }
        public IDbSet<GnMstOrganizationDtl> GnMstOrganizationDtls { get; set; }
        public IDbSet<GnMstCustomerUtility> GnMstCustomerUtilities { get; set; }
        public IDbSet<GnMstCustomer> GnMstCustomers { get; set; }
        public IDbSet<GnMstCustomerDealer> GnMstCustomerDealers { get; set; }
        public IDbSet<GnMstZipCode> GnMstZipCode { get; set; }
        public IDbSet<GnMstCustomerClass> GnMstCustomerClasses { get; set; }
        //public IDbSet<GnMstTax> GnMstTaxes { get; set; }
        public IDbSet<GnMstCollector> GnMstCollectors { get; set; }
        public IDbSet<HrEmployeeView> HrEmployeeViews { get; set; }
        public IDbSet<OmMstRefference> OmMstRefferences { get; set; }
        public IDbSet<GnMstCustomerDealerProfitCenter> GnMstCustomerDealerProfitCenters { get; set; }
        public IDbSet<ProfitCenter> ProfitCenters { get; set; }
        public IDbSet<GnMstCustomerDisc> GnMstCustomerDiscs { get; set; }
        public IDbSet<GnMstCustomerBank> GnMstCustomerBanks { get; set; }
        public IDbSet<SysUserProfitCenter> SysUserProfitCenters { get; set; }
        public IDbSet<GnMstSupplier> GnMstSuppliers { get; set; }
        public IDbSet<GnMstSupplierClass> GnMstSupplierClasses { get; set; }
        public IDbSet<GnMstSupplierBank> GnMstSupplierBanks { get; set; }
        public IDbSet<GnMstSupplierProfitCenter> GnMstSupplierProfitCenters { get; set; }
        public IDbSet<GnMstSupplierUtility> GnMstSupplierUtilities { get; set; }
        public IDbSet<GnMstSegmentAcc> GnMstSegmentAccs { get; set; }
        public IDbSet<gnMstAccount> GnMstAccounts { get; set; }
        public IDbSet<GnMstDocument> GnMstDocuments { get; set; }
        public IDbSet<gnMSTEmployee> employees { get; set; }
        public IDbSet<GnMstSignature> GnMstSignatures { get; set; }
        public IDbSet<LookUpHdr> GnMstLookUpHdrs { get; set; }
        public IDbSet<GnMstFPJSignDate> GnMstFPJSignDates { get; set; }
        public IDbSet<GnMstFPJSeqNo> GnMstFPJSeqNos { get; set; }
        //public IDbSet<GnMstCoProfileSpare> CoProfileSpare { get; set; }
        //public IDbSet<GnMstCoProfileService> CoProfileService { get; set; }
        //public IDbSet<GnMstCoProfileSales> CoProfileSales { get; set; }
        //public IDbSet<GnMstCoProfileFinance> CoProfileFinance { get; set; }
        public IDbSet<GnMstEmployeeTraining> employeeTrainings { get; set; }
        public IDbSet<Tax> Taxs { get; set; }
        public IDbSet<GnMstCalender> Calenders { get; set; }
        public IDbSet<Periode> Periodes { get; set; }
        public IDbSet<GnMstReminder> GnMstReminders { get; set; }
        public IDbSet<GnMstApproval> GnMstApprovals { get; set; }
        public IDbSet<SysReportSettings> SysReportSettings { get; set; }

        public IDbSet<SysParameter> SysParameters { get; set; } 
    } 
}