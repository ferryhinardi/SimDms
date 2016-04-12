using SimDms.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SimDms.Web.Models
{
    public class LayoutContext : DbContext
    {

        // Override constructor
        public LayoutContext(string ConnString): base(ConnString) {}

        // Default constructor
        public LayoutContext()
            : base(MyHelpers.GetConnString("LayoutContext")) { }
        
        public IDbSet<SysUser> SysUsers { get; set; }
        public IDbSet<SysUserView> SysUserViews { get; set; }
        public IDbSet<SysRole> SysRoles { get; set; }
        public IDbSet<SysRoleMenu> SysRoleMenus { get; set; }
        public IDbSet<SysRoleUser> SysRoleUsers { get; set; }
        public IDbSet<SysSession> SysSessions { get; set; }
        public IDbSet<SysMenu> SysMenus { get; set; }
        public IDbSet<SysModule> SysModules { get; set; }
        public IDbSet<SysRoleModule> SysRoleModules { get; set; }
        public IDbSet<SysReport> SysReports { get; set; }
        public IDbSet<SysControlDms> SysControlDmses { get; set; }
        public IDbSet<SysUserProfitCenter> SysUserProfitCenters { get; set; }
        public IDbSet<LookUpDtl> LookUpDtls { get; set; }
        public IDbSet<OrganizationHdr> OrganizationHdrs { get; set; }

    }
}