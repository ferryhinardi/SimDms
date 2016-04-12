using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace SimDms.Document.Models
{
    public class DataContext :DbContext
    {
        public IDbSet<SysHelp> SysHelps { get; set; }
        public IDbSet<SysHelpContent> SysHelpContents { get; set; }
        public IDbSet<SysDocumentImage> SysDocumentImages { get; set; }
        public IDbSet<DocImage> DocImage { get; set; }
    }

    public class DMSContext : DbContext
    {

    }
}