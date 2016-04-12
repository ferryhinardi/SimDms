using MongoRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eXpressAPP.Codebase.DAL.Models.MongoDB
{
    public class CodeEditor : Entity 
    {
        public string html { get; set; }
        public string css { get; set; }
        public string js { get; set; }
        public string cfg { get; set; }
        public string filename { get; set; }
    }

    public class SearchParam
    {
        public string filename { get; set; }
    }
}