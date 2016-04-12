using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Document.Models
{
    public class MenuModel
    {
        public int MenuID { get; set; }
        public int MenuLevel { get; set; }
        public int MenuSeq { get; set; }
        public int MenuHeader { get; set; }
        public string ContentID { get; set; }
        public List<MenuModel> MenuItems { get; set; }
    }
}