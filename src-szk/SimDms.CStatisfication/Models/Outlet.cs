using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models
{
    public class Outlet
    {
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string DealerCode { get; set; }
        public string DealerName { get; set; }
    }
}