using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SimDms.Common.Models
{
    public class CommonContext : DbContext
    {
        public CommonContext(string ConnString)
            : base(ConnString)
        {
        }

        public CommonContext() :
            base(MyHelpers.GetConnString("DataContext"))
        {

        }

        public IDbSet<SysParameter> Parameters { get; set; }
        public IDbSet<SysParam> sysParams { get; set; }
        public IDbSet<GnDcsUploadFile> GnDcsUploadFiles { get; set; }
        public IDbSet<SysUser> Users { get; set; }
        public IDbSet<SysUserProfitCenter> UserProfitCenters { get; set; }
        public IDbSet<CoProfile> MstCoProfiles { get; set; }
        public IDbSet<SysUserView> UserViews { get; set; }
        public IDbSet<LookUpDtl> MstLookUpDtls { get; set; }
        public IDbSet<SysFlatFileHdr> SysFlatFileHdrs { get; set; }
        public IDbSet<SysFlatFileDtl> SysFlatFileDtls { get; set; }
    }
}
