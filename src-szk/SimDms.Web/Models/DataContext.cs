using SimDms.DataWarehouse.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SimDms.Web.Models
{
    public class DataContext : DbContext
    {

        // Override constructor
        public DataContext(string ConnString): base(ConnString) {}

        // Default constructor
        public DataContext()
            : base(MyHelpers.GetConnString("DataContext")) { }

        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysRole> SysRoles { get; set; }
        public IDbSet<SysRoleUser> SysRoleUsers { get; set; }
        public IDbSet<SysRoleMenu> SysRoleMenus { get; set; }
        public IDbSet<SysMenu> SysMenus { get; set; }
        public IDbSet<SysModule> SysModules { get; set; }
        public IDbSet<SysSession> SysSessions { get; set; }
        public IDbSet<SysRoleModule> SysRoleModules { get; set; }
        public IDbSet<RegisteredUser> RegisteredUsers { get; set; }
        public IDbSet<SysReport> SysReports { get; set; }

        public IDbSet<LastTransDateInfo> TransactionDateInfo { get; set; }
        public IDbSet<ReportSession> ReportSessions { get; set; }
        //public IDbSet<SysLogger> SysLoggers { get; set; }
    }
}