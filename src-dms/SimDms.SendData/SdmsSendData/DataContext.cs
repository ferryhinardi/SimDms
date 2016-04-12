using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Sim.Dms.SendData.Model;

namespace Sim.Dms.SendData
{
    public class DataContext : DbContext
    {
        public IDbSet<SendScheduleDms> SendScheduleDmss { get; set; }
        //public IDbSet<Employee> Employees { get; set; }
    }
}
