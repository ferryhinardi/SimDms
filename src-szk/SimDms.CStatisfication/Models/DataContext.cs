using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SimDms.CStatisfication.Models
{
    public class DataContext : DbContext
    {
        public IDbSet<Customer> Customers { get; set; }
        public IDbSet<TDayCall> TDayCalls { get; set; }
    }
}