using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using System.Transactions;
using SimDms.Common;
using SimDms.Common.Models;
using SimDms.Common.DcsWs;
using System.Diagnostics;
using SimDms.PreSales.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class CouponController : BaseController
    {
        public JsonResult itsUserDefault()
        {
            var outlet = ctx.PmBranchOutlets.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault().OutletID;
            var outletname = ctx.OrganizationDtls.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault().BranchName;
            var list = ctx.HrEmployees.Where(p => p.Department == "SALES").Select(p => new { p.CompanyCode, p.EmployeeID, p.EmployeeName, p.RelatedUser, p.Position, p.TeamLeader, p.IdentityNo });
            var eid = list.Where(p => p.CompanyCode == CompanyCode && p.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            var Branch = ctx.HrEmployeeMutations.Where(a => a.CompanyCode == CompanyCode && a.EmployeeID == eid.EmployeeID && a.IsDeleted == false).OrderByDescending(b => b.CreatedDate).FirstOrDefault();
            var g = ctx.CoProfiles.Find(CompanyCode, BranchCode);
            var sis = ctx.HrEmployeeSales.Where(a => a.CompanyCode == CompanyCode && a.EmployeeID == eid.EmployeeID).FirstOrDefault();
            var bCoo = false;
            if (eid != null)
            {
                if (eid.Position == "COO" || eid.Position == "CEO" || eid.Position == "GM")
                {
                    bCoo = true;
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        UserID = CurrentUser.UserId,
                        EmployeeID = eid.EmployeeID,
                        EmployeeName = eid.EmployeeName,
                        Position = eid.Position,
                        SISID = sis.SalesID,
                        IdentityNo = eid.IdentityNo,
                        isCOO = bCoo,
                        Branch = Branch == null ? null : Branch.BranchCode,
                        ProductType = g.ProductType,
                        cmpCode = CompanyCode,
                        OutletCode = BranchCode,
                        outletID = outlet,
                        outletName = outletname,
                        UpperLvl = eid.TeamLeader
                    }
                });
            }
            else
            {
                return Json(new { success = false, message = "User belum link dengan salesman" });
            }
        }

        public JsonResult browse()
        {
            var employee = ctx.HrEmployees.Where(a => a.CompanyCode == CompanyCode && a.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            var qry = from p in ctx.pmKDPCoupon
                      join q in ctx.PmKdps
                        on new { p.CompanyCode, p.InquiryNumber } equals new { q.CompanyCode, q.InquiryNumber }
                      join r in ctx.HrEmployees
                        on new { q.CompanyCode, q.EmployeeID } equals new { r.CompanyCode, r.EmployeeID }
                      join s in ctx.HrEmployeeSales
                        on new { q.CompanyCode, q.EmployeeID } equals new { s.CompanyCode, s.EmployeeID }
                      join t in ctx.CoProfiles on q.BranchCode equals t.BranchCode into leftJoin
                      from t in leftJoin.DefaultIfEmpty()
                      where p.CompanyCode == CompanyCode
                      select new
                      {
                          p.CompanyCode,
                          p.InquiryNumber,
                          p.CoupunNumber,
                          p.NamaProspek,
                          p.ProspekIdentityNo,
                          q.TelpRumah,
                          q.AlamatProspek,
                          p.TestDriveDate,
                          p.Email,
                          q.EmployeeID,
                          r.EmployeeName,
                          r.IdentityNo,
                          s.SalesID,
                          //CompanyName,
                          //BranchName,
                          t.CompanyGovName,
                          t.CompanyName,
                          p.Remark
                      };

            if (employee.Position == "S")
            {
                qry = qry.Where(p => p.EmployeeID.Equals(employee.EmployeeID));
            }
            if (employee.Position == "SC" || employee.Position == "SH")
            {
                var subordinates = ctx.HrEmployees.Where(x => x.TeamLeader == employee.EmployeeID && x.PersonnelStatus == "1").Select(x => x.EmployeeID).ToList();
                qry = qry.Where(x => subordinates.Contains(x.EmployeeID));
            }

            return Json(qry.AsQueryable().toKG());
        }

        public JsonResult InqNumber()
        {
            var employee = ctx.HrEmployees.Where(a => a.CompanyCode == CompanyCode && a.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            var Coupon = ctx.pmKDPCoupon.Select(a=>a.InquiryNumber).ToList();
            var TipeKendaraan = ctx.pmMstCoupon.Where(a => a.CompanyCode == CompanyCode && a.isActive == true && a.BeginPeriod <= DateTime.Now && a.EndPeriod >= DateTime.Now)
                                .Select(a => a.TipeKendaraan).ToList();
            var Variant = ctx.pmMstCoupon.Where(a => a.CompanyCode == CompanyCode && a.isActive == true && a.BeginPeriod <= DateTime.Now && a.EndPeriod >= DateTime.Now)
                                .Select(a => a.Variant).ToList();
            var qry = from p in ctx.PmKdps
                      join q in ctx.HrEmployees
                        on new { p.CompanyCode, p.EmployeeID } equals new { q.CompanyCode, q.EmployeeID }
                      join r in ctx.HrEmployeeSales
                        on new { q.CompanyCode, q.EmployeeID } equals new { r.CompanyCode, r.EmployeeID }
                      where p.CompanyCode == CompanyCode
                      && p.BranchCode == BranchCode
                      && p.TestDrive == "10"
                      && (p.LastProgress == "P" || p.LastProgress == "HP" || p.LastProgress == "SPK")
                      select new
                      {
                          p.InquiryNumber,
                          p.InquiryDate,
                          p.NamaProspek,
                          p.TelpRumah,
                          p.AlamatProspek,
                          p.EmployeeID,
                          q.EmployeeName,
                          q.IdentityNo,
                          r.SalesID,
                          p.TipeKendaraan,
                          p.Variant,
                          TestDrive = p.TestDrive == "10" ? "YA" : "TIDAK"
                      };

            if (employee.Position == "S") {
                qry = qry.Where(p => p.EmployeeID.Equals(employee.EmployeeID) && !Coupon.Contains(p.InquiryNumber)
                        && TipeKendaraan.Contains(p.TipeKendaraan) && Variant.Contains(p.Variant));
            }
            if (employee.Position == "SC" || employee.Position == "SH") {
                var subordinates = ctx.HrEmployees.Where(x => x.TeamLeader == employee.EmployeeID && x.PersonnelStatus == "1").Select(x => x.EmployeeID).ToList();
                qry = qry.Where(x => subordinates.Contains(x.EmployeeID) && !Coupon.Contains(x.InquiryNumber)
                        && TipeKendaraan.Contains(x.TipeKendaraan) && Variant.Contains(x.Variant));
            }

            return Json(qry.AsQueryable().toKG());
        }

        public JsonResult Save(pmKDPCoupon model)
        {
            string msg = "";
            var record = ctx.pmKDPCoupon.Where(a=> a.CompanyCode == CompanyCode && a.InquiryNumber == model.InquiryNumber && a.CoupunNumber == model.CoupunNumber).FirstOrDefault();

            if (record == null)
            {
                record = new pmKDPCoupon
                {
                    CompanyCode = CompanyCode,
                    InquiryNumber = model.InquiryNumber,
                    CoupunNumber = model.CoupunNumber,
                    NamaProspek = model.NamaProspek,
                    EmployeeID = model.EmployeeID,
                    ProspekIdentityNo = model.ProspekIdentityNo,
                    Email = model.Email,
                    TestDriveDate = model.TestDriveDate,
                    Remark = model.Remark == null ? "" : model.Remark,
                    ProcessFlag = false,
                    ProcessDate = Convert.ToDateTime("1900-01-01"),
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now,
                };
                ctx.pmKDPCoupon.Add(record);
                msg = "New Coupon added...";
            }
            else
            {
                    record.CoupunNumber = model.CoupunNumber;
                    record.ProspekIdentityNo = model.ProspekIdentityNo;
                    record.Email = model.Email;
                    record.TestDriveDate = model.TestDriveDate;
                    record.Remark = model.Remark == null ? "" : model.Remark;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    msg = "Coupon updated";
            }

            try
            {
                ctx.SaveChanges();
                return Json(new { success = true, message = msg, data = record });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}