using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using SimDms.Common.Models;
using SimDms.Common;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Text;
using System.Data.SqlClient;
using TracerX;
using System.Data.Entity.Core;

namespace SimDms.Sparepart.Models
{
    public partial class DataContext : DbContext
    {
        // Override constructor
        public DataContext(string ConnString): base(ConnString) {}
        // Default constructor
        public DataContext()
            : base(MyHelpers.GetConnString("DataContext"))
        {          
        
        } 

        public DateTime CurrentTime
        {
            get {
                return this.Database.SqlQuery<DateTime>("select getdate()").FirstOrDefault();
            }
        }

        public string CurrentUser { get;  set; }

        public  int SaveChanges()
        {
            foreach (var x in ChangeTracker.Entries<BaseTable>())
            {
                switch (x.State)
                { 
                    case EntityState.Added:
                        x.Entity.SetCreatedBy(CurrentUser, CurrentTime);
                        break;
                    case EntityState.Modified:
                        x.Entity.SetModifiedBy(CurrentUser, CurrentTime);
                        break;
                }
            }

            try
            {
                return base.SaveChanges();
            }
            catch (DbUpdateException e)
            {
                var innerEx = e.InnerException;

                while (innerEx.InnerException != null)
                    innerEx = innerEx.InnerException;

                UpdateException updateException = (UpdateException)e.InnerException;
                SqlException sqlException = (SqlException)updateException.InnerException;
                var sb = new StringBuilder();

                foreach (SqlError error in sqlException.Errors)
                {
                    sb.AppendLine(error.Message);
                }

                string s = sb.ToString();
                MyLogger.Log.Info(s);
                throw new Exception(s);
            }
            catch (DbEntityValidationException e)
            {
                var sb = new StringBuilder();

                foreach (var entry in e.EntityValidationErrors)
                {
                    foreach (var error in entry.ValidationErrors)
                    {
                        sb.AppendLine(string.Format("{0}-{1}-{2}",
                            entry.Entry.Entity,
                            error.PropertyName,
                            error.ErrorMessage
                            )
                        );
                    }
                }

                string s = sb.ToString();
                MyLogger.Log.Info(s);
                throw new Exception(s);
            }
        }

        public IDbSet<SpMstItemLoc> SpMstItemLocs { get; set; }
        public IDbSet<SpMstItemlocView> SpMstItemlocViews { get; set; }
        public IDbSet<SpMstItemLocItemLookupView> SpMstItemLocItemLookupViews { get; set; }
        public IDbSet<spMstItem> spMstItems { get; set; }
        public IDbSet<SysMessage> SysMsgs { get; set; }
        public IDbSet<MasterItemInfo> MasterItemInfos { get; set; }
        public IDbSet<MstSupplierProfitCenter> MstSupplierProfitCenters { get; set; }
        public IDbSet<MstCustomerProfitCenter> MstCustomerProfitCenters { get; set; }
        public IDbSet<SpMstItemModel> SpMstItemModels { get; set; }

        public DbSet<spMasterPartLookup> spMasterPartLookups { get; set; }
        public DbSet<sp_spMstSalesTargetDtlview> spMstSalesTargetDtlviews { get; set; }
        public DbSet<GnMstCoProfileSpare> GnMstCoProfileSpares { get; set; }
        public DbSet<spTrnPClaimHdr> spTrnPClaimHdrs { get; set; }
        public DbSet<SpTrnPRcvHdr> SpTrnPRcvHdrs { get; set; }
        public DbSet<SpGridPartNo> SpGridPartNos { get; set; }
        public IDbSet<CoProfileService> CoProfileServices { get; set; }
        public IDbSet<SpTrnPClaimDtl> SpTrnPClaimDtls { get; set; }
        
    }
}