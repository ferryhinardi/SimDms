using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace SimDms.ConsoleApps.Models
{
    public class DataLogContext : DbContext
    {
        public IDbSet<HrEmployeeLog> HrEmployeeLogs { get; set; }
        public IDbSet<HrEmployeeAchievementLog> HrEmployeeAchievementLogs { get; set; }
    }
}
