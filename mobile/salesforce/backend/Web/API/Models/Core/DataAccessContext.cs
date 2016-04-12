using System;
using System.Data.Entity;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace eXpressAPI.Models
{
    public partial class DataAccessContext : DbContext
    {
        public int AutoSaveChanges(string userId, string companyId)
        {
            int n = 0;

            DateTime saveTime = DateTime.Now;

            foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                string createddate = "";
                string createdby = "";
                string companyid = "";

                foreach (var propName in entry.CurrentValues.PropertyNames)
                {
                    if ((propName.ToLower() == "createddate") || (propName.ToLower() == "createdate"))
                    {
                        createddate = propName;
                    }
                    else if ((propName.ToLower() == "createdby"))
                    {
                        createdby = propName;
                    }
                    else if ((propName.ToLower() == "companycode"))
                    {
                        companyid = propName;
                    }
                }

                if (createddate != "")
                {
                    entry.Property(createddate).CurrentValue = saveTime;
                }
                if (createdby != "")
                {
                    entry.Property(createdby).CurrentValue = userId;
                }
                if (companyid != "" && entry.Property(companyid).CurrentValue == null)
                {
                    entry.Property(companyid).CurrentValue = companyId;
                }
            }

            foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
            {
                string changedate = "";
                string changeby = "";

                foreach (var propName in entry.CurrentValues.PropertyNames)
                {
                    if ((propName.ToLower() == "changedate") || (propName.ToLower() == "modifieddate"))
                    {
                        changedate = propName;
                    }
                    else if ((propName.ToLower() == "changeby") || (propName.ToLower() == "modifiedby"))
                    {
                        changeby = propName;
                    }
                }

                if (changedate != "")
                {
                    entry.Property(changedate).CurrentValue = saveTime;
                }
                if (changeby != "")
                {
                    entry.Property(changeby).CurrentValue = userId;
                }
            }

            n = base.SaveChanges();
            return n;
        }

        public int SaveChanges(string userId)
        {
            int n = 0;

            DateTime saveTime = DateTime.Now;


            foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                string createddate = "";
                string createdby = "";
                string companyid = "";

                foreach (var propName in entry.CurrentValues.PropertyNames)
                {
                    if ((propName.ToLower() == "createddate") || (propName.ToLower() == "createdate"))
                    {
                        createddate = propName;
                    }
                    else if ((propName.ToLower() == "createdby"))
                    {
                        createdby = propName;
                    }
                    else if ((propName.ToLower() == "companyid"))
                    {
                        companyid = propName;
                    }
                }

                if (createddate != "")
                {
                    entry.Property(createddate).CurrentValue = saveTime;
                }
                if (createdby != "")
                {
                    entry.Property(createdby).CurrentValue = userId;
                }

            }

            foreach (var entry in this.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
            {
                string changedate = "";
                string changeby = "";

                foreach (var propName in entry.CurrentValues.PropertyNames)
                {
                    if ((propName.ToLower() == "changeddate") || (propName.ToLower() == "modifieddate"))
                    {
                        changedate = propName;
                    }
                    else if ((propName.ToLower() == "changedby") || (propName.ToLower() == "modifiedby"))
                    {
                        changeby = propName;
                    }
                }

                if (changedate != "")
                {
                    entry.Property(changedate).CurrentValue = saveTime;
                }
                if (changeby != "")
                {
                    entry.Property(changeby).CurrentValue = userId;
                }
            }


            n = base.SaveChanges();
            return n;
        }

        public bool IsEditable(string MenuId, string RoleId)
        {
            bool success = false;

            var IsRoleMenu = this.RoleMenus.Find(RoleId, MenuId);

            if (IsRoleMenu != null)
            {
                success = (IsRoleMenu.IsEditable == 1);
            }

            return success;
        }

        public string IsPermission(string MenuId, string RoleId)
        {
            string success = "0000000000";
            var IsRoleMenu = this.RoleMenus.Find(RoleId, MenuId);
            if (IsRoleMenu != null)
            {
                success = (IsRoleMenu.AllowInfo);
            }
            return success;
        }       

        public string ExecScalar(string SQL)
        {
            string ret = "";

            try
            {
                ret = this.Database.SqlQuery<string>(SQL).Single();
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ex.Message));
            }

            return ret;

        }

        public DataTable GetTableInfo(string sql)
        {
            using (var connection = new SqlConnection(this.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand(sql, connection);

                command.Connection.Open();
                command.CommandTimeout = 3600;
                DataTable table = new DataTable();

                table.Load(command.ExecuteReader());

                command.Connection.Close();

                return table;
            }

            return null;

        }

        //static DataAccessContext()
        //{
        //    //Database.SetInitializer<DataContext>(null);
        //}

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Conventions.Remove<IncludeMetadataConvention>();
        //}

        // This is overridden to prevent someone from calling SaveChanges without specifying the user making the change
        //public override int SaveChanges()
        //{
        //    throw new InvalidOperationException("User ID must be provided");
        //}

        //public int SaveChanges(string userId)
        //{

        //    int n = 0;

        //    // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
        //    //foreach (var ent in this.ChangeTracker.Entries().Where(p => p.State == System.Data.Entity.EntityState.Added || p.State == System.Data.Entity.EntityState.Deleted || p.State == System.Data.Entity.EntityState.Modified))
        //    //{
        //    //    // For each changed record, get the audit record entries and add them
        //    //    foreach (AuditLog x in GetAuditRecordsForChange(ent, userId))
        //    //    {
        //    //        this.AuditLog.Add(x);
        //    //    }
        //    //}

 
        //    n = base.SaveChanges();

        //    // Call the original SaveChanges(), which will save both the changes made and the audit records
        //    return n;
        //}

        //private List<AuditLog> GetAuditRecordsForChange(DbEntityEntry dbEntry, string userId)
        //{
        //    List<AuditLog> result = new List<AuditLog>();

        //    DateTime changeTime = DateTime.UtcNow;

        //    // Get the Table() attribute, if one exists
        //    TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

        //    // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
        //    string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;

        //    // Get primary key value (If you have more than one key column, this will need to be adjusted)
        //    string keyName = dbEntry.Entity.GetType().GetProperties().Single(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

        //    if (dbEntry.State == System.Data.Entity.EntityState.Added)
        //    {
        //        // For Inserts, just add the whole record
        //        // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()
        //        result.Add(new AuditLog()
        //        {
        //            AuditLogID = Guid.NewGuid(),
        //            UserID = userId,
        //            EventDateUTC = changeTime,
        //            EventType = "A", // Added
        //            TableName = tableName,
        //            RecordID = dbEntry.CurrentValues.GetValue<object>(keyName).ToString(),  // Again, adjust this if you have a multi-column key
        //            ColumnName = "*ALL",    // Or make it nullable, whatever you want
        //            NewValue = (dbEntry.CurrentValues.ToObject() is IDescribableEntity) ? (dbEntry.CurrentValues.ToObject() as IDescribableEntity).Describe() : dbEntry.CurrentValues.ToObject().ToString()
        //        }
        //            );
        //    }
        //    else if (dbEntry.State == System.Data.Entity.EntityState.Deleted)
        //    {
        //        // Same with deletes, do the whole record, and use either the description from Describe() or ToString()
        //        result.Add(new AuditLog()
        //        {
        //            AuditLogID = Guid.NewGuid(),
        //            UserID = userId,
        //            EventDateUTC = changeTime,
        //            EventType = "D", // Deleted
        //            TableName = tableName,
        //            RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
        //            ColumnName = "*ALL",
        //            NewValue = (dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString()
        //        }
        //            );
        //    }
        //    else if (dbEntry.State == System.Data.Entity.EntityState.Modified)
        //    {
        //        foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
        //        {
        //            // For updates, we only want to capture the columns that actually changed
        //            if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
        //            {
        //                result.Add(new AuditLog()
        //                {
        //                    AuditLogID = Guid.NewGuid(),
        //                    UserID = userId,
        //                    EventDateUTC = changeTime,
        //                    EventType = "M",    // Modified
        //                    TableName = tableName,
        //                    RecordID = dbEntry.OriginalValues.GetValue<object>(keyName).ToString(),
        //                    ColumnName = propertyName,
        //                    OriginalValue = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
        //                    NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
        //                }
        //                    );
        //            }
        //        }
        //    }
        //    // Otherwise, don't do anything, we don't care about Unchanged or Detached entities

        //    return result;
        //}


        public virtual DbSet<RoleMenu> RoleMenus { get; set; }
        public virtual DbSet<RoleCompany> RoleCompanies { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<VwUser> VwUsers { get; set; }
        public virtual DbSet<CompanyProfile> CompanyProfile { get; set; }
        public virtual DbSet<Menus> Menus { get; set; }
        public virtual DbSet<ReportSession> ReportSessions { get; set; }
        //public virtual DbSet<AuditLog> AuditLog { get; set; }
    }
    

    public class RepositoryExp<T> : eXpressRepository<T> where T : class
    {
        private readonly DataAccessContext _dbContext;

        public RepositoryExp(DataAccessContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SaveResult Add(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Set<T>().Add(t);

            try
            {
                ret.Count = _dbContext.SaveChanges();
                ret.success = true;
            }
            catch (DbEntityValidationException ex)
            {
                ret.message = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        ret.message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }

                // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public async Task<SaveResult> AddAsync(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Set<T>().Add(t);

            try
            {
                ret.Count = await _dbContext.SaveChangesAsync();
                ret.success = true;
            }
            catch (DbEntityValidationException ex)
            {
                ret.message = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        ret.message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public async Task<SaveResult> RemoveAsync(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Entry(t).State = EntityState.Deleted;

            try
            {
                ret.Count = await _dbContext.SaveChangesAsync();
                ret.success = true;
            }
            catch (DbUpdateException ex)
            {
                ret.message = ex.InnerException.Message;
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public SaveResult Remove(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Entry(t).State = EntityState.Deleted;

            try
            {
                ret.Count = _dbContext.SaveChanges();
                ret.success = true;
            }
            catch (DbUpdateException ex)
            {
                ret.message = ex.InnerException.Message;
                // Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public SaveResult Update(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Entry(t).State = EntityState.Modified;

            try
            {
                ret.Count = _dbContext.SaveChanges();
                ret.success = true;
            }
            catch (DbEntityValidationException ex)
            {
                ret.message = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        ret.message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }

                //Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public async Task<SaveResult> UpdateAsync(T t)
        {
            SaveResult ret = new SaveResult(0, false);

            _dbContext.Entry(t).State = EntityState.Modified;

            try
            {
                ret.Count = await _dbContext.SaveChangesAsync();
                ret.success = true;
            }
            catch (DbEntityValidationException ex)
            {
                ret.message = "";
                foreach (DbEntityValidationResult item in ex.EntityValidationErrors)
                {
                    DbEntityEntry entry = item.Entry;
                    string entityTypeName = entry.Entity.GetType().Name;

                    foreach (DbValidationError subItem in item.ValidationErrors)
                    {
                        ret.message += string.Format("Error '{0}' occurred in {1} at {2}",
                                 subItem.ErrorMessage, entityTypeName, subItem.PropertyName);
                    }
                }

                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(ret.message));
            }

            return ret;
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Set<T>().CountAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(match);
        }

        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _dbContext.Set<T>().Where(match).ToListAsync();
        }
    }
}