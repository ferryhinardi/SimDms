using SimDms.Common;
using SimDms.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.CStatisfication.Models
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


        public int SaveChanges()
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

        public IDbSet<Holiday> Holidays { get; set; }
        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysRoleUser> SysRoleUsers { get; set; }
        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<HrEmployee> HrEmployees { get; set; }

        public IDbSet<TDayCall> TDayCalls { get; set; }
        public IDbSet<CustHoliday> CustHolidays { get; set; }

        public IDbSet<CustomerView> CustomerViews { get; set; }
        public IDbSet<CustomerBuyView> CustomerBuyViews { get; set; }
        public IDbSet<TDayCallView> TDayCallViews { get; set; }
        public IDbSet<CsStnkExt> StnkExtensions { get; set; }
        public IDbSet<CustHolidayView> CustHolidayViews { get; set; }
        //public IDbSet<CustomerBirthday> CustomerBirthdays { get; set; }
        public IDbSet<CsCustBirthDay> CsCustBirthDays { get; set; }
        //public IDbSet<CsCustBirthdayView> CsCustBirthdayViews { get; set; }
        public IDbSet<CsCustRelation> CsCustRelations { get; set; }
        public IDbSet<CsCustBpkb> CustBPKBs { get; set; }
        public IDbSet<CsCustFeedback> CsCustFeedbacks { get; set; }
        public IDbSet<CsReview> CsReviews { get; set; }
        //public IDbSet<CsCustomerVehicle> CsCustomerVehicles { get; set; }
        public IDbSet<CsSetting> CsSettings { get; set; }
        public IDbSet<CsCustData> CsCustDatas { get; set; }
        public IDbSet<CsBpkbRetrievalInformation> CsBpkbRetrievalInformations { get; set; }

        public IDbSet<CsLkuTDayCallView> CsLkuTDayCallViews { get; set; }
        public IDbSet<CsLkuStnkExtensionView> CsLkuStnkExtensionViews { get; set; }
        public IDbSet<CsLkuBpkbReminderView> CsLkuBpkbReminderViews { get; set; }
        public IDbSet<CsLkuFeedbackView> CsLkuFeedbackViews { get; set; }
        public IDbSet<CsLkuBirthdayView> CsLkuBirthdayViews { get; set; }

        public IDbSet<GnMstOrganizationDtl> GnMstOrganizationDtls { get; set; }
        public IDbSet<GnMstOrganizationHdr> GnMstOrganizationHdrs { get; set; }
        public IDbSet<GnMstCustomer> GnMstCustomers { get; set; }
        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
    }
}