﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SimDms.Common.Models
{
    [Table("sysUserProfitCenter")]
    public class SysUserProfitCenter
    {
        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }
        public string ProfitCenter { get; set; }
    }

    public class UserProfitCenter {
        public string ProfitCenterCode { get; set; }
    }
}