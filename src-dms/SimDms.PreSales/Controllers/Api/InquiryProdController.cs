using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers.Api
{
    public class InquiryProdController : BaseController
    {
        public JsonResult Default()
        {
            var SHID = String.Empty;
            var BMID = String.Empty;
            var SMID = String.Empty;
            var SHName = String.Empty;
            var BMName = String.Empty;
            var SMName = String.Empty;
            var bCoo = false; 
            //Position position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.UserId == CurrentUser.UserId).FirstOrDefault();
            var position = ctx.HrEmployees.Where(p => p.CompanyCode == CompanyCode && p.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            var branchcode = ctx.HrEmployeeMutations.Where(p => p.CompanyCode == CompanyCode && p.EmployeeID == position.EmployeeID && p.IsDeleted == false).OrderByDescending(a => a.CreatedDate).FirstOrDefault().BranchCode;
            if (position == null)
            {
                return Json(new { success = false, message = "Anda tidak mendapatkan akses untuk menu ini" });
            }

            if (position.Position == "COO" || position.Position == "CEO" || position.Position == "GM")
            {
                bCoo = true;
            }

            if (position.Position == "BM")
            {
                BMName = position.EmployeeName;
                BMID = position.EmployeeID;
            }

            var empBM = ctx.HrEmployees.FirstOrDefault();
            
            if (position.Position == "SH")
            {
                empBM = ctx.HrEmployees.Where(a => a.EmployeeID == position.TeamLeader).FirstOrDefault();
                SHName = position.EmployeeName;
                BMName = empBM.EmployeeName;
                SHID = position.EmployeeID;
                BMID = empBM.EmployeeID;
            }

            var empSH = ctx.HrEmployees.FirstOrDefault();
            if (position.Position == "S")
            {
                empSH = ctx.HrEmployees.Where(a => a.EmployeeID == position.TeamLeader).FirstOrDefault();
                empBM = ctx.HrEmployees.Where(a => a.EmployeeID == empSH.TeamLeader).FirstOrDefault();
                SHName = empSH.EmployeeName;
                BMName = empBM.EmployeeName;
                SHID = empSH.EmployeeID;
                BMID = empBM.EmployeeID;
                SMID = position.EmployeeID;
                SMName = position.EmployeeName;
            }

            string sql = string.Format("exec uspfn_gnInquiryBtn 'CABANG', '{0}', '', '2'", CompanyCode);
            var AreaList = (from p in ctx.Database.SqlQuery<SimDms.PreSales.Models.InquiryBtn>(sql) 
                              where p.GroupNo != "000"
                            select new SimDms.PreSales.Models.InquiryBtn()
                              {
                                  Area = p.Area
                              }).Select(p => new { value = p.Area, text = p.Area }).FirstOrDefault();

            var dealerList = (from p in ctx.Database.SqlQuery<SimDms.PreSales.Models.InquiryBtn>(sql)
                              where  p.GroupNo != "000"
                              select new SimDms.PreSales.Models.InquiryBtn()
                        {
                            DealerCode = p.DealerCode,
                            DealerName = p.DealerName
                        }).Select(p => new { value = p.DealerCode, text = p.DealerName }).FirstOrDefault();
          
            return Json(new
            {
                success = true,
                data = new
                    {
                        CompanyCode = CompanyCode,
                        CompanyName = CompanyName,
                        BranchCode = branchcode,
                        //BranchName = BranchName,
                        IsBranch = IsBranch,
                        DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                        DateTo = DateTime.Now,
                        PositionID = (position == null) ? "0" : position.Position,
                        EmployeeID = (position == null) ? "" : position.EmployeeID,
                        Salesman = SMID,
                        SalesmanName = SMName,
                        SalesHead = SHID ,
                        BranchManager = BMID,
                        SalesHeadName = SHName,
                        BranchManagerName = BMName ,
                        NationalSLS = NationalSLS,
                        Area = (NationalSLS == "0") ? AreaList.value : "",
                        Dealer = (NationalSLS == "0") ? dealerList.value : "",
                        Outlet = (IsBranch) ? branchcode : "",
                        IsGM = bCoo
                    }
            });

        }        
    }
}
