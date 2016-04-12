using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SimDms.Common.Models;
using SimDms.Common;
using System.Data.Entity.Validation;
using System.Text;
using System.Data.Entity.Infrastructure;

namespace SimDms.Tax.Models
{
    public partial class DataContext : DbContext
    {
        public DataContext(string ConnString)
            : base(ConnString)
        {
        }

        public DataContext() :
            base(MyHelpers.GetConnString("DataContext"))
        {

        }

        public DateTime CurrentTime
        {
            get
            {
                return this.Database.SqlQuery<DateTime>("select getdate()").FirstOrDefault();
            }
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
                throw HandleDataValidationException(vex);
            }
            catch (DbUpdateException dbu)
            {
                throw HandleDataUpdateException(dbu);
            }

            return n;
        }

        public string CurrentUser { get; set; }

        public IDbSet<CoProfile> CoProfiles { get; set; }
        public IDbSet<CoProfileFinance> CoProfileFinances { get; set; }

        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
        public IDbSet<LookUpHdr> LookUpHdrs { get; set; } 
        public IDbSet<OrganizationHdr> OrganizationHdrs { get; set; }
        public IDbSet<OrganizationDtl> OrganizationDtls { get; set; }

        public IDbSet<Period> Periods { get; set; }

        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysUserProfitCenter> SysUserProfitCenters { get; set; }
        public IDbSet<Supplier> Suppliers { get; set; }
        public IDbSet<SupplierProfitCenter> SupplierProfitCenters { get; set; }
        public IDbSet<TxFpjConfig> TxFpjConfigs { get; set; } 
        public IDbSet<gnTaxIn> gnTaxIn { get; set; }
        public IDbSet<GnMstSignature> gnMstSignatures { get; set; }
        public IDbSet<gnGenerateTax> gnGenerateTax { get; set; }

        public IDbSet<GnTaxInHistory> GnTaxInHistories { get; set; }
        public IDbSet<GnTaxOut> GnTaxOuts { get; set; }
        public IDbSet<GnTaxOutHistory> GnTaxOutHistories { get; set; }
        public IDbSet<GnTaxPPN> GnTaxPPns { get; set; }


        public IDbSet<GnMstCustomer> GnMstCustomer { get; set; }

        public IDbSet<svTrnFakturPajak> svTrnFakturPajak { get; set; }
        public IDbSet<SysMessage> SysMsgs { get; set; }
    }   
}