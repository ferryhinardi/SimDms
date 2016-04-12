using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.PreSales.Models;
using GeLang;
using System.Data.Entity.Infrastructure;
using SimDms.Common.Models;
//using SimDms.Common.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class ClnUpKdpController : BaseController
    {
        public ActionResult Default()
        {
            string salesHead = string.Empty;
            Position position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == p.BranchCode && p.UserId == CurrentUser.UserId).FirstOrDefault();
            if (position == null && NationalSLS == "0")
            {
                return Json(new { success = false, message = "Anda tidak mendapatkan akses untuk menu ini" });
            }
            else
            {
                if (position.PositionId == "20")
                {
                    string sql = string.Empty;
                    sql = GetEmployeeByPosition(position.PositionId, position.EmployeeID);
                    var salesCoord = ctx.Database.SqlQuery<InqEmployee>(sql).FirstOrDefault();
                    sql = GetParentPosition(salesCoord.UserID);
                    if ((from p in ctx.Database.SqlQuery<InqEmployee>(sql)
                         select new { value = p.EmployeeID, text = p.EmployeeName }).FirstOrDefault() == null)
                    {
                        return Json(new { success = false, message = "User belum mempunyai Sales Head di Master Team Members ! " });
                    }
                    else
                    {
                        salesHead = (from p in ctx.Database.SqlQuery<InqEmployee>(sql)
                                     select new { value = p.EmployeeID, text = p.EmployeeName }).FirstOrDefault().value.ToString();
                    }                    
                }
            }

            LookUpDtl lookupITSDate = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "CLNUP" && p.LookUpValue == "ITSDATE").FirstOrDefault();
            if (lookupITSDate == null)
            {
                return Json(new { success = false, message = "Program clean up tidak dapat digunakan. Setting ITS Date terlebih dahulu di Master Lookup." });
            }
            else
            {
                DateTime dt = DateTime.ParseExact(lookupITSDate.ParaValue, "yyyyMMdd", null);
                if (DateTime.Now >= dt)
                {
                    return Json(new { success = false, message = "Program clean up tidak dapat digunakan. ITS Date di Master Lookup lebih kecil dari tanggal hari ini." });
                }
            }

            LookUpDtl lookupPer = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "CLNUP" && p.LookUpValue == "PER").FirstOrDefault();
            bool bPerDate = false;
            if (lookupPer != null)
            {
                if (lookupPer.ParaValue == "1") bPerDate = true;
            }

            var Outlet = (from p in ctx.DealerOutletMappings
                          where p.DealerCode == CompanyCode && p.OutletCode == BranchCode
                          select p).FirstOrDefault();
            
            return Json(new
            {
                success = true,
                data = new
                {
                    CompanyCode = CompanyCode,
                    CompanyName = CompanyName,
                    BranchCode = BranchCode,
                    IsBranch = IsBranch,
                    DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    DateTo = DateTime.Now,
                    EmployeeID = (position == null) ? "" : position.EmployeeID,
                    NationalSLS = NationalSLS,
                    PerDate = DateTime.Now,
                    DisabledPerDate = bPerDate,
                    PositionID = (position == null) ? "" : position.PositionId,
                    Outlet = (Outlet == null) ? "" : Outlet.OutletCode,
                    SalesHead = (position == null) ? "" : salesHead
                }
            });
        }

        public ActionResult DetailFollowUp(int inquiryNumber, string lastProgress)
        {            
            return Json(new
            {
                success = true,
                data = new
                {                   
                    InquiryNumber = inquiryNumber,
                    ActivityDate = DateTime.Now,
                    ActivityType = "",
                    ActivityDetail = "",
                    NextFollowUpDate = DateTime.Now,
                    LastProgress = lastProgress
                }
            });
        }

        public JsonResult Outlets(string area, string dealer)
        {
            if (NationalSLS == "1")
            {
                var list = (from p in ctx.DealerOutletMappings
                            where p.DealerCode == CompanyCode
                            select new Outlet()
                            {
                                OutletCode = p.OutletCode,
                                OutletName = p.OutletName
                            }).Select(p => new { value = p.OutletCode, text = p.OutletName }).ToList();
                return Json(list);
            }
            else
            {
                var list = (from p in ctx.DealerOutletMappings
                            where p.DealerCode == CompanyCode && p.OutletCode == BranchCode
                            select new Outlet()
                            {
                                OutletCode = p.OutletCode,
                                OutletName = p.OutletName
                            }).Select(p => new { value = p.OutletCode, text = p.OutletName }).ToList();

                return Json(list);
            }
        }

        private string GetEmployeeByPosition(string positionID, string employeeID)
        {
            string sql = string.Format(@"SELECT 
                        a.EmployeeID, b.EmployeeName, a.UserID
                    FROM 
                        pmPosition a
                    LEFT JOIN gnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.EmployeeID = a.EmployeeID
                    WHERE
                        a.CompanyCode = '{0}' AND ((CASE WHEN '{1}' ='' THEN a.BranchCode END)<>'' 
                        OR (CASE WHEN '{1}' <>'' THEN a.BranchCode END) = '{1}') 
                        AND a.PositionId = '{2}'
                        AND (CASE WHEN '{3}' = '' THEN '' ELSE a.EmployeeID END) = '{3}'
                    ORDER BY b.EmployeeName", CompanyCode, BranchCode, positionID, employeeID);

            return sql;
        }

        public JsonResult ComboSalesman(string employeeID, string lookup, string dealer, string outlet, string positionID)
        {
            Position position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.EmployeeID == employeeID).FirstOrDefault();
            string sql = string.Empty;

//            if (NationalSLS == "1")
//            {
//                if (lookup == BRANCH_MANAGER)
//                {
//                    sql = string.Format(@"
//	                    select distinct BranchHead EmployeeID, BranchHead EmployeeName 
//                        from pmHstITS
//                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}' and (case when '{1}' = '' then '' else BranchCode end) = '{1}' and BranchHead != ''
//                        ", dealer == null ? string.Empty : dealer, outlet == null ? string.Empty : outlet);
//                }
//                else if (lookup == SALES_HEAD)
//                {
//                    sql = string.Format(@"
//	                    select distinct SalesHead EmployeeID, SalesHead EmployeeName 
//                        from pmHstITS
//                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
//                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
//                            and (case when '{2}' = '' then '' else BranchHead end) = '{2}'
//                            and SalesHead != ''"
//                           , dealer, outlet == null ? string.Empty : outlet, employeeID);
//                }
//                else if (lookup == SALES_COORDINATOR)
//                {
//                    sql = string.Format(@"
//	                    select distinct SalesCoordinator EmployeeID, SalesCoordinator EmployeeName 
//                        from pmHstITS
//                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
//                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
//                            and (case when '{2}' = '' then '' else SalesHead end) = '{2}'
//                            and SalesCoordinator != ''"
//                           , dealer, outlet == null ? string.Empty : outlet, employeeID);
//                }
//                else if (lookup == SALESMAN)
//                {
//                    sql = string.Format(@"
//	                    select distinct Wiraniaga EmployeeID, Wiraniaga EmployeeName 
//                        from pmHstITS
//                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
//                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
//                            and (case when '{2}' = '' then '' else SalesCoordinator end) = '{2}'
//                            and Wiraniaga != ''"
//                            , dealer, outlet == null ? string.Empty : outlet, employeeID);
//                }

//                var list = (from p in ctx.Database.SqlQuery<InqEmployee>(sql)
//                            select new { value = p.EmployeeID, text = p.EmployeeName }).ToList();

//                return Json(list);
//            }
//            else
//            {
                string msg = string.Empty;
                positionID = (position == null) ? "" : position.PositionId;
                if (positionID == "") return null;

                #region SALES_HEAD
                if (lookup == SALES_HEAD)
                {
                    if (positionID == COO)
                    {
                        sql = "select ' ' EmployeeID, '--SELECT ALL--' EmployeeName";
                        if (NationalSLS == "1")
                            sql = string.Format(@"
	                    select distinct SalesHead EmployeeID, SalesHead EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                            and (case when '{2}' = '' then '' else BranchHead end) = '{2}'
                            and SalesHead != ''"
                                , dealer, outlet == null ? string.Empty : outlet, employeeID);
                    }
                    else if (positionID == SALES_ADMIN)
                    {
                        sql = GetEmployeeByPosition(BRANCH_MANAGER, "");
                    }
                    else if (positionID == BRANCH_MANAGER)
                    {
                        sql = GetChildPosition(CurrentUser.UserId);
                    }
                    else if (positionID == SALES_HEAD)
                    {
                        sql = GetEmployeeByPosition(positionID, employeeID);
                    }
                    else if (positionID == SALES_COORDINATOR)
                    {
                        sql = GetEmployeeByPosition(positionID, employeeID);
                        var salesCoord = ctx.Database.SqlQuery<InqEmployee>(sql).FirstOrDefault();
                        sql = GetParentPosition(salesCoord.UserID);
                    }
                    else if (positionID == SALESMAN)
                    {
                        sql = GetEmployeeByPosition(positionID, "");
                        position = ctx.Positions.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.UserId == CurrentUser.UserId).FirstOrDefault();
                        var salesCoord = ctx.Database.SqlQuery<InqEmployee>(sql).ToList();
                        var salesCoordID = salesCoord.Where(p => p.EmployeeID == position.EmployeeID).ToList();

                        sql = GetParentPosition(salesCoordID[0].UserID);
                        var salesHead = ctx.Database.SqlQuery<InqEmployee>(sql).ToList();
                        if (salesHead.Count == 0)
                        {
                            msg = "User belum memiliki Sales Head di Master Team Members !";
                            return Json(new { data = "", message = msg });
                        }
                        else
                        {
                            sql = GetParentPosition(salesHead[0].UserID);
                        }
                    }
                }
                #endregion

                #region SALES_COORDINATOR
                else if (lookup == SALES_COORDINATOR)
                {
                    if (positionID == COO || positionID == SALES_ADMIN || positionID == BRANCH_MANAGER)
                    {
                        sql = "select ' ' EmployeeID, '--SELECT ALL--' EmployeeName";
                        if (NationalSLS == "1")
                            sql = string.Format(@"
	                    select distinct SalesCoordinator EmployeeID, SalesCoordinator EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                            and (case when '{2}' = '' then '' else SalesHead end) = '{2}'
                            and SalesCoordinator != ''"
                                , dealer, outlet == null ? string.Empty : outlet, employeeID);
                    }
                    else if (positionID == SALES_HEAD)
                    {
                        var userid = (from p in ctx.Positions
                                      where p.CompanyCode == CompanyCode &&
                                      p.BranchCode == BranchCode &&
                                      p.EmployeeID == employeeID
                                      select p).FirstOrDefault();

                        sql = GetChildPosition(userid.UserId);
                    }
                    else if (positionID == SALES_COORDINATOR)
                    {
                        sql = GetEmployeeByPosition(positionID, employeeID);
                    }
                    else if (positionID == SALESMAN)
                    {
                        sql = GetParentPosition(CurrentUser.UserId);
                    }
                }
                #endregion

                #region SALESMAN
                else if (lookup == SALESMAN)
                {
                    if (positionID == COO || positionID == SALES_ADMIN || positionID == BRANCH_MANAGER || positionID == SALES_HEAD)
                    {
                        sql = "select ' ' EmployeeID, '--SELECT ALL--' EmployeeName";
                        if (NationalSLS == "1")
                            sql = string.Format(@"
	                    select distinct Wiraniaga EmployeeID, Wiraniaga EmployeeName 
                        from pmHstITS
                        where (case when '{0}' = '' then '' else CompanyCode end) = '{0}'
                            and (case when '{1}' = '' then '' else BranchCode end) = '{1}'
                            and (case when '{2}' = '' then '' else SalesCoordinator end) = '{2}'
                            and Wiraniaga != ''"
                                , dealer, outlet == null ? string.Empty : outlet, employeeID);
                    }
                    else if (positionID == SALES_COORDINATOR)
                    {
                        var userid = (from p in ctx.Positions
                                      where p.CompanyCode == CompanyCode &&
                                      p.BranchCode == BranchCode &&
                                      p.EmployeeID == employeeID
                                      select p).FirstOrDefault();

                        sql = GetChildPosition(userid.UserId);

                        //sql = GetChildPosition(CurrentUser.UserId);
                    }
                    else if (positionID == SALESMAN)
                    {
                        sql = GetEmployeeByPosition(positionID, employeeID);
                    }
                }
                #endregion

                var list = (from p in ctx.Database.SqlQuery<InqEmployee>(sql)
                            select new { value = p.EmployeeID, text = p.EmployeeName }).ToList();

                //if (list.Count() == 0)
                //{
                //    if (lookup == BRANCH_MANAGER)
                //    {
                //        if (positionID == SALES_HEAD)
                //            msg = "User belum memiliki Sales Head di Master Team Members !";

                //        if (positionID == SALES_COORDINATOR || positionID == SALESMAN)
                //            msg = "User belum memiliki Branch Manager di Master Team Members !";
                //    }
                //    else if (lookup == SALES_HEAD)
                //    {
                //        if (positionID == COO)
                //            msg = "User belum memiliki Branch Manager di Master Team Members !";

                //        if (positionID == SALES_ADMIN || positionID == BRANCH_MANAGER || positionID == SALES_COORDINATOR || positionID == SALESMAN)
                //            msg = "User belum memiliki Sales Head di Master Team Members !";
                //    }
                //    else if (lookup == SALES_COORDINATOR)
                //    {
                //        if (positionID == SALES_HEAD || positionID == SALESMAN)
                //            msg = "User belum memiliki Sales Coordinator di Master Team Members !";
                //    }
                //    else if (lookup == SALESMAN)
                //    {
                //        if (positionID == SALES_COORDINATOR)
                //            msg = "User belum memiliki Salesman di Master Team Members !";
                //    }
                //}

                //if (msg != string.Empty)
                //    return Json(new { message = msg });
                //else
                    return Json(list);
            //}
        }

        private string GetParentPosition(string userID)
        {
            string sql = string.Format(@"SELECT 
                        a.employeeID, b.EmployeeName, c.OutletID, d.OutletName, e.UserID
                    FROM
                        PmMstTeamMembers a
                    LEFT JOIN GnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
                        AND b.EmployeeId = a.EmployeeID
                    LEFT JOIN PmPosition c
                        ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
                        AND c.UserID = '{2}'
                    LEFT JOIN PmBranchOutlets d
	                    ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
                        AND d.OutletID = c.OutletID
                    LEFT JOIN PmPosition e
                        ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode 
                        AND e.EmployeeID = a.EmployeeID
                    WHERE
                        a.CompanyCode = '{0}' AND a.BranchCode = '{1}' AND a.IsSupervisor = 1
                        AND TeamID = (SELECT TeamID FROM PmMStteamMembers WHERE CompanyCode ='{0}'
                        AND BranchCode = '{1}' AND EmployeeID = c.EmployeeID AND IsSupervisor = 0)", CompanyCode, BranchCode, userID);

            return sql;
        }

        private string GetChildPosition(string userID)
        {
            string sql = string.Format(@"SELECT 
                        a.EmployeeID, b.EmployeeName, c.OutletID, d.OutletName
                    FROM
                        PmMstTeamMembers a
                    LEFT JOIN GnMstEmployee b
                        ON b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode 
                        AND b.EmployeeId = a.EmployeeID
                    LEFT JOIN PmPosition c
                        ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode 
                        AND c.UserID = '{2}'
                    LEFT JOIN PmBranchOutlets d
	                    ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode 
                        AND d.OutletID = c.OutletID
                    WHERE
                        a.CompanyCode = '{0}'
                        AND ((CASE WHEN '{1}' = '' THEN a.BranchCode END) <> '' OR (CASE WHEN '{1}' <> '' THEN a.BranchCode END) = '{1}') 
                        AND a.IsSupervisor = 0 
                        AND TeamID = (SELECT TeamID FROM PmMstTeamMembers WHERE CompanyCode = '{0}'
                            AND ((CASE WHEN '{1}' = '' THEN BranchCode END) <> '' OR (CASE WHEN '{1}' <> '' THEN BranchCode END) = '{1}') 
                            AND EmployeeID = c.EmployeeID AND IsSupervisor = 1)", CompanyCode, BranchCode, userID);

            return sql;
        }

        public JsonResult InqClnUpKDP()
        {
            ((IObjectContextAdapter)ctx).ObjectContext.CommandTimeout = 6000;
            string branch = Request["Outlet"];
            DateTime perDate = Convert.ToDateTime(DateTime.Parse(Request["PerDate"]));
            string salesHead = Request["SalesHead"];
            string salesCo = Request["SalesCoord"];
            string salesman = Request["Salesman"];

            var qry = ctx.PmKdpClnUpViews.Where(p => p.CompanyCode == CompanyCode);
            if (!string.IsNullOrWhiteSpace(branch)) { qry = qry.Where(p => p.BranchCode.Contains(branch)); };
            qry = qry.Where(p => p.NextFollowUpDate <= perDate);
            if (!string.IsNullOrWhiteSpace(salesCo)) { qry = qry.Where(p => p.SpvEmployeeID.Contains(salesCo)); };
            if (!string.IsNullOrWhiteSpace(salesman)) { qry = qry.Where(p => p.EmployeeID.Contains(salesman)); };

            if (!string.IsNullOrWhiteSpace(salesHead))
            {
                var qryTeam = ctx.TeamMembers.Where(p => p.CompanyCode == CompanyCode);
                if (!string.IsNullOrWhiteSpace(branch)) { qryTeam = qryTeam.Where(p => p.BranchCode.Contains(branch)); };
                qryTeam = qryTeam.Where(p => p.EmployeeID == salesHead && p.IsSupervisor == true);
                var teamID = qryTeam.FirstOrDefault().TeamID;

                var qrySalesHead = ctx.TeamMembers.Where(p => p.CompanyCode == CompanyCode);
                if (!string.IsNullOrWhiteSpace(branch)) { qrySalesHead = qrySalesHead.Where(p => p.BranchCode.Contains(branch)); };
                qrySalesHead = qrySalesHead.Where(p => p.TeamID == teamID && p.IsSupervisor == false);
                var empSalesHead = qrySalesHead.Select(p => p.EmployeeID);

                qry = qry.Where(p => empSalesHead.Contains(p.SpvEmployeeID));
            }

            qry = qry.OrderBy(p => new { p.BranchCode, p.InquiryNumber }).Distinct();

            return Json(qry.KGrid());
        }

        public JsonResult InqClnUpKDPDtl()
        {
            string branchCode = Request["BranchCode"];
            int inquiryNumber = Convert.ToInt32(Request["InquiryNumber"]);

            var qry = ctx.PmActivites.Where(p =>
                       p.CompanyCode == CompanyCode &&
                       p.BranchCode == branchCode &&
                       p.InquiryNumber == inquiryNumber).Select(p => new { p.ActivityDate, p.ActivityDetail, p.ActivityType, p.NextFollowUpDate });

            return Json(new { data = qry.ToList(), total = qry.Count() });
        }

        public JsonResult ClnUpKDPDtl(int inquiryNumber)
        {
            var qry = ctx.PmActivites.Where(p =>
                       p.CompanyCode == CompanyCode &&
                       p.BranchCode == BranchCode &&
                       p.InquiryNumber == inquiryNumber).Select(p => new { p.ActivityDate, p.ActivityDetail, p.ActivityType, p.NextFollowUpDate });

            return Json(new { data = qry.ToList(), total = qry.Count() });
        }

        public int ClnUpKDP(string inquiryNumber, DateTime perDate)
        {
            string[] listInquiry = new string[]{ };
            listInquiry = inquiryNumber.ToString().Split(',');

            int result = 0;
            for (int i = 0; i < listInquiry.Count(); i++)
            {
                int inquiryNum = Int32.Parse(listInquiry[i]);
                PmKdp pmkdp = ctx.PmKdps.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.InquiryNumber == inquiryNum).FirstOrDefault();

                pmkdp.LastProgress = "LOST";
                pmkdp.LastUpdateStatus = pmkdp.LostCaseDate = pmkdp.LastUpdateDate = DateTime.Now;
                pmkdp.LostCaseCategory = "F";
                pmkdp.LostCaseReasonID = "70";
                pmkdp.LostCaseOtherReason = "CLEAN UP " + perDate.ToString("dd-MMM-yyyy");
                pmkdp.LastUpdateBy = "SYSTEM";

                result = ctx.SaveChanges();
            }            

            return result;            
        }

        public JsonResult ClnUpAllKDP()
        {
            string branch = Request["Outlet"];
            DateTime perDate = Convert.ToDateTime(DateTime.Parse(Request["PerDate"]));
            string salesHead = Request["SalesHead"];
            string salesCo = Request["SalesCoord"];
            string salesman = Request["Salesman"];

            var qry = ctx.PmKdpClnUpViews.Where(p => p.CompanyCode == CompanyCode);
            if (!string.IsNullOrWhiteSpace(branch)) { qry = qry.Where(p => p.BranchCode.Contains(branch)); };
            qry = qry.Where(p => p.NextFollowUpDate <= perDate);
            if (!string.IsNullOrWhiteSpace(salesCo)) { qry = qry.Where(p => p.SpvEmployeeID.Contains(salesCo)); };
            if (!string.IsNullOrWhiteSpace(salesman)) { qry = qry.Where(p => p.EmployeeID.Contains(salesman)); };

            if (!string.IsNullOrWhiteSpace(salesHead))
            {
                var qryTeam = ctx.TeamMembers.Where(p => p.CompanyCode == CompanyCode);
                if (!string.IsNullOrWhiteSpace(branch)) { qryTeam = qryTeam.Where(p => p.BranchCode.Contains(branch)); };
                qryTeam = qryTeam.Where(p => p.EmployeeID == salesHead && p.IsSupervisor == true);
                var teamID = qryTeam.FirstOrDefault().TeamID;

                var qrySalesHead = ctx.TeamMembers.Where(p => p.CompanyCode == CompanyCode);
                if (!string.IsNullOrWhiteSpace(branch)) { qrySalesHead = qrySalesHead.Where(p => p.BranchCode.Contains(branch)); };
                qrySalesHead = qrySalesHead.Where(p => p.TeamID == teamID && p.IsSupervisor == false);
                var empSalesHead = qrySalesHead.Select(p => p.EmployeeID);

                qry = qry.Where(p => empSalesHead.Contains(p.SpvEmployeeID));
            }
            qry = qry.OrderBy(p => new { p.BranchCode, p.InquiryNumber });            

            foreach (var item in qry.ToList())
            {
                PmKdp pmkdp = ctx.PmKdps.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == item.BranchCode && p.InquiryNumber == item.InquiryNumber).FirstOrDefault();

                pmkdp.LastProgress = "LOST";
                pmkdp.LastUpdateStatus = pmkdp.LostCaseDate = pmkdp.LastUpdateDate = DateTime.Now;
                pmkdp.LostCaseCategory = "F";
                pmkdp.LostCaseReasonID = "70";
                pmkdp.LostCaseOtherReason = "CLEAN UP " + perDate.ToString("dd-MMM-yyyy");
                pmkdp.LastUpdateBy = "SYSTEM";

                ctx.SaveChanges();
            }

            return Json(qry.KGrid());
        }

        public JsonResult ExportClnUpKDP(string Outlet, DateTime perDate, string SalesHead, string SalesCoord, string Salesman)
        {           
            var qry = ctx.PmKdpClnUpViews.Where(p => p.CompanyCode == CompanyCode);
            if (!string.IsNullOrWhiteSpace(Outlet)) { qry = qry.Where(p => p.BranchCode.Contains(Outlet)); };
            qry = qry.Where(p => p.NextFollowUpDate <= perDate);
            if (!string.IsNullOrWhiteSpace(SalesCoord)) { qry = qry.Where(p => p.SpvEmployeeID.Contains(SalesCoord)); };
            if (!string.IsNullOrWhiteSpace(Salesman)) { qry = qry.Where(p => p.EmployeeID.Contains(Salesman)); };

            if (!string.IsNullOrWhiteSpace(SalesHead))
            {
                var qryTeam = ctx.TeamMembers.Where(p => p.CompanyCode == CompanyCode);
                if (!string.IsNullOrWhiteSpace(Outlet)) { qryTeam = qryTeam.Where(p => p.BranchCode.Contains(Outlet)); };
                qryTeam = qryTeam.Where(p => p.EmployeeID == SalesHead && p.IsSupervisor == true);
                var teamID = qryTeam.FirstOrDefault().TeamID;

                var qrySalesHead = ctx.TeamMembers.Where(p => p.CompanyCode == CompanyCode);
                if (!string.IsNullOrWhiteSpace(Outlet)) { qrySalesHead = qrySalesHead.Where(p => p.BranchCode.Contains(Outlet)); };
                qrySalesHead = qrySalesHead.Where(p => p.TeamID == teamID && p.IsSupervisor == false);
                var empSalesHead = qrySalesHead.Select(p => p.EmployeeID);

                qry = qry.Where(p => empSalesHead.Contains(p.SpvEmployeeID));
            }
            qry = qry.OrderBy(p => new { p.BranchCode, p.InquiryNumber });

            RenderReport(Server.MapPath("~/Reports/rdlc/its/cleanup.rdlc"), "CleanUpITS", 10, 11.7, "excel", qry.ToList());
            return null;
        }

        public JsonResult SaveAct(PmActivity model, string lastProgress)
        {            
            var userID = CurrentUser.UserId;
            var currentDate = DateTime.Now;
            var numbers = from p in ctx.PmActivites
                            where p.CompanyCode == CompanyCode
                            && p.BranchCode == BranchCode
                            && p.InquiryNumber == model.InquiryNumber
                            select p.ActivityID;
            var newnumber = (numbers.Count() > 0) ? numbers.Max() + 1 : 1;
            //object returnMsg;
            var record = ctx.PmActivites.Find(CompanyCode, model.BranchCode,  model.InquiryNumber, newnumber);
            if (record == null)
            {
                record = new PmActivity
                {
                    CompanyCode = CompanyCode,
                    BranchCode = BranchCode,
                    InquiryNumber = model.InquiryNumber,
                    ActivityID = newnumber,
                    CreatedBy = userID,
                    CreationDate = currentDate,
                    ActivityDate = model.ActivityDate,
                    ActivityType = (String.IsNullOrEmpty(model.ActivityType)) ? "OTHER" : model.ActivityType,
                    ActivityDetail = (String.IsNullOrEmpty(model.ActivityDetail)) ? "PROSES CLEAN UP ITS" : model.ActivityDetail,
                    NextFollowUpDate = model.NextFollowUpDate,
                    LastUpdateBy = userID,
                    LastUpdateDate = currentDate
                };

                var pmKDP = (from p in ctx.PmKdps
                             where p.CompanyCode == CompanyCode
                             && p.BranchCode == BranchCode
                             && p.InquiryNumber == model.InquiryNumber
                             select p).FirstOrDefault();
                if (pmKDP != null)
                {
                    pmKDP.LastProgress = lastProgress;
                    pmKDP.LastUpdateBy = userID;
                    pmKDP.LastUpdateDate = DateTime.Now;
                }
                ctx.PmActivites.Add(record);
            }            

            if (String.IsNullOrEmpty(model.ActivityDetail) && String.IsNullOrEmpty(model.ActivityType))
            {
                 var isExist = (from p in ctx.PmActivites
                                where p.CompanyCode == CompanyCode
                                    && p.BranchCode == BranchCode
                                    && p.InquiryNumber == model.InquiryNumber
                                    && p.ActivityDetail == "PROSES CLEAN UP ITS"
                                select p).ToList();

                 if (isExist.Count > 0)
                 {
                     return Json(new { success = false, message = "Detail Activity untuk Proses Clean Up ITS sudah pernah dimasukkan" });
                 }                 
            }

            try
            {
                ctx.SaveChanges();

                var data = from p in ctx.PmActivites
                           where p.CompanyCode == record.CompanyCode
                           && p.BranchCode == record.BranchCode
                           && p.InquiryNumber == record.InquiryNumber
                           select new
                           {
                               p.CompanyCode,
                               p.BranchCode,
                               p.InquiryNumber,
                               p.ActivityID,
                               p.ActivityDate,
                               p.ActivityType,
                               p.ActivityDetail,
                               p.NextFollowUpDate,
                           };
                return Json(new { success = true, list = data.ToList() });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }                         
        }                    
    }
}
