using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SimDms.SUtility.Models;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Text;
using SimDms.SUtility.Models.Others;

namespace SimDms.SUtility.Controllers
{
    public class DataContext : DbContext
    {

        // Override constructor
        public DataContext(string ConnString): base(ConnString) {}

        // Default constructor
        public DataContext()
            : base(MyHelpers.GetConnString("DataContext")) { }


        protected Exception HandleDataUpdateException(DbUpdateException exception)
        {
            Exception innerException = exception.InnerException;

            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }

            return new Exception(innerException.Message);
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

            return new Exception(stringBuilder.ToString().Trim());
        }


        public int SaveChanges()
        {
            int svRet = 0;

            try
            {
                svRet = base.SaveChanges();
            }
            catch (DbEntityValidationException vex)
            {
                var exception1 = HandleDataValidationException(vex);
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception1);
            }
            catch (DbUpdateException dbu)
            {
                var exception = HandleDataUpdateException(dbu);
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
            }

            return svRet;
        }

        public IDbSet<Company> Companies { get; set; }
        public IDbSet<HrEmployee> HrEmployees { get; set; }
        public IDbSet<HrEmployeeAchievement> HrEmployeeAchievements { get; set; }
        public IDbSet<HrEmployeeMutation> HrEmployeeMutations { get; set; }
        public IDbSet<HrEmployeeSales> HrEmployeeSales { get; set; }

        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysUserView> SysUserViews { get; set; }
        public IDbSet<SysRole> SysRoles { get; set; }
        public IDbSet<SysModule> SysModules { get; set; }
        public IDbSet<SysRoleModule> SysRoleModules { get; set; }
        public IDbSet<SysRoleModuleView> SysRoleModuleViews { get; set; }
        public IDbSet<SysMenu> SysMenus { get; set; }
        public IDbSet<SysRoleMenu> SysRoleMenus { get; set; }
        public IDbSet<SysRoleMenuView> SysRoleMenuViews { get; set; }
        public IDbSet<SysRoleUser> SysRoleUsers { get; set; }
        public IDbSet<SysMenuView> SysMenuViews { get; set; }


        public IDbSet<SvTrnService> SvTrnServices { get; set; }
        public IDbSet<SvTrnInvoice> SvTrnInvoices { get; set; }
        public IDbSet<SvHstSzkMsi> SvHstSzkMsies { get; set; }
        public IDbSet<svMstRefferenceService> svMstRefferenceService { get; set; }
        public IDbSet<OmTrSalesSoVin> OmTrSalesSoVins { get; set; } 

        public IDbSet<PmKdp> PmKdps { get; set; }
        public IDbSet<PmActivity> PmActivities { get; set; }
        public IDbSet<PmStatusHistory> PmStatusHistories { get; set; }

        public IDbSet<GnMstCustDealer> GnMstCustDealers { get; set; }
        public IDbSet<GnMstCustDealerDtl> GnMstCustDealerDtls { get; set; }

        public IDbSet<CsTDayCall> CsTDayCalls { get; set; }
        public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        public IDbSet<CsCustBpkb> CsCustBpkbs { get; set; }
        public IDbSet<CsCustData> CsCustDatas { get; set; }
        public IDbSet<CsCustFeedback> CsCustFeedbacks { get; set; }
        public IDbSet<CsCustHoliday> CsCustHolidays { get; set; }
        public IDbSet<CsMstHoliday> CsMstHolidays { get; set; }
        public IDbSet<CsCustRelation> CsCustRelations { get; set; }
        public IDbSet<CsStnkExt> CsStnkExts { get; set; }
        public IDbSet<CsCustomerVehicle> CsCustomerVehicles { get; set; }
        public IDbSet<gnMstCustomer> gnMstCustomers { get; set; }
        public IDbSet<svMstCustomerVehicle> svMstCustomerVehicles { get; set; }
        public IDbSet<omTrSalesBPK> omTrSalesBPKs { get; set; }
        public IDbSet<omTrSalesDO> omTrSalesDOs { get; set; }
        public IDbSet<omTrSalesDODetail> omTrSalesDODetails { get; set; }
        public IDbSet<omTrSalesInvoice> omTrSalesInvoices { get; set; }
        public IDbSet<omTrSalesInvoiceVin> omTrSalesInvoiceVins { get; set; }
        public IDbSet<omTrSalesSO> omTrSalesSOs { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        public IDbSet<DealerInfo> DealerInfos { get; set; }
        public IDbSet<OutletInfo> OutletInfos { get; set; }
        public IDbSet<pmHstITS> pmHstITSes { get; set; }
        public IDbSet<omHstInquirySales> omHstInquirySalesses { get; set; }
        public IDbSet<DmsUpload> DmsUploads { get; set; }
        public IDbSet<DmsDownload> DmsDownloads { get; set; }
        public IDbSet<DmsDownloadFileView> DmsDownloadFileViews { get; set; }
        public IDbSet<DmsUploadFileView> DmsUploadFileViews { get; set; }

        public IDbSet<SysSQLGateway> SQLGateway { get; set; }

    }

    public class DataDealerContext : DbContext
    {

        // Override constructor
        public DataDealerContext(string ConnString): base(ConnString) {}

        // Default constructor
        public DataDealerContext()
            : base(MyHelpers.GetConnString("DataDealerContext")) { }

        public IDbSet<TokenAccess> TokenAccesses { get; set; }
        public IDbSet<GnMstScheduleData> GnMstScheduleDatas { get; set; }
        public IDbSet<SysDataType> SysDataTypes { get; set; }
    }

    public class MyHelpers
    {        

        public static string GetConnString(string cfgName)
        {
            //string MyAppPath = HttpContext.Current.Request.ApplicationPath.ToString();

            //if (MyAppPath.Length > 1)
            //{
            //    var IsMultipleApp = System.Configuration.ConfigurationManager.AppSettings["MultipleApp"] ?? "0";
            //    if (Convert.ToBoolean(IsMultipleApp))
            //    {
            //        cfgName += MyAppPath.Replace(@"/", "_");
            //    }
            //}

            string cnStr = System.Configuration.ConfigurationManager.ConnectionStrings[cfgName].ToString();
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                cnStr += "-" + HttpContext.Current.User.Identity.Name;
            }

            //MyLogger.Info("Conn String: " + cnStr);
            return cnStr;
        }
    }

}