using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.DataWarehouse.Models;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class LookUpGridController : BaseController
    {
        public JsonResult Cities()
        {

            var fltText = Request["CityID"]; 
            var companyCode = Request["CompanyCode"];

            var qry = ctx.GnMstLookUpDtls.Where(p => p.CodeID == "CITY").Select(q => new
            {
                CityCode = q.LookUpValue,
                CityName = q.LookUpValueName
            });

            if (string.IsNullOrWhiteSpace(companyCode) == false)
            {
                qry = ctx.GnMstLookUpDtls.Where(p => p.CompanyCode == companyCode && p.CodeID == "CITY").Select(q => new
                {
                    CityCode = q.LookUpValue,
                    CityName = q.LookUpValueName
                });
            }

            var qryCities = qry.Where(p => p.CityCode.Contains(fltText));
            if (qryCities.ToArray().Count() == 0)
            {
                qryCities = qry.Where(p => p.CityName.Contains(fltText));
                if (qryCities.ToArray().Count() == 0)
                {
                    qryCities = qry;
                }
            }

            return Json(qryCities.Distinct().KGrid());
        }

        public JsonResult KdpList()
        {
            var company = Request["CompanyCode"];
            var branch = Request["BranchCode"];
            var nikSales = Request["NikSales"];
            var nikSC = Request["NikSC"];

            var testDrive = ctx.GnMstLookUpDtls.Where(p => p.CodeID == "PMOP").Select(
                    p => new { 
                        CodeID = p.CodeID,
                        LookUpValue = p.LookUpValue,
                        LookUpValueName = p.LookUpValueName
                }).Distinct();

            var DealerAbbreviation = ctx.GnMstDealerMappings.OrderBy(p => p.DealerAbbreviation).Where(p => (p.GroupNo == 100 || p.GroupNo == 105) 
                && p.DealerCode.Substring(4, 1) == "4" && p.isActive == true)
                .Select(p => new { 
                    DealerCode = p.DealerCode,
                    DealerName = p.DealerCode + " - " + p.DealerAbbreviation
                });

            var OutletAbbreviation = ctx.GnMstDealerOutletMappings.OrderBy(p => p.OutletAbbreviation).Where(p => p.isActive == true)
                .Select(p => new
                {
                    DealerCode = p.DealerCode,
                    OutletCode = p.OutletCode,
                    OutletName = p.OutletCode + " - " + p.OutletAbbreviation
                });

            var DealeOutleMapping = from p in DealerAbbreviation
                join q in OutletAbbreviation on new { p.DealerCode }
                    equals new { q.DealerCode }
                select new
                {
                    CompanyCode = p.DealerCode,
                    BranchCode = q.OutletCode,
                    DealerName = p.DealerCode + " - " + p.DealerName,
                    OutletName = q.DealerCode + " - " + q.OutletName
                };

            var employeeID = ctx.HrEmployees.Where(x => x.RelatedUser == CurrentUser.Username).Select(x => x.EmployeeID).FirstOrDefault();
            var subordinates = ctx.HrEmployees.Where(x => x.TeamLeader == employeeID).Select(x => x.EmployeeID).ToList();
            var qry = from p in ctx.PmKDPExhibitions
                join pd in ctx.PmKDPAdditionalExhibitions on new { p.CompanyCode, p.InquiryNumber, p.BranchCode, GiftRefferenceCode = "GIFT" }
                    equals new { pd.CompanyCode, pd.InquiryNumber, pd.BranchCode, pd.GiftRefferenceCode }
                join o in DealeOutleMapping on new { p.CompanyCode, p.BranchCode }
                    equals new { o.CompanyCode, o.BranchCode }
                join q in ctx.HrEmployees on new { p.CompanyCode, p.EmployeeID }
                    equals new { q.CompanyCode, q.EmployeeID }
                join j1 in ctx.OmMstRefferences on new
                { //p.CompanyCode,
                    p.ColourCode,
                    RefferenceType = "COLO"
                }
                    equals new
                    { //j1.CompanyCode, 
                        ColourCode = j1.RefferenceCode,
                        j1.RefferenceType
                    } into join1
                from r in join1.DefaultIfEmpty()
                join j2 in testDrive on new { p.TestDrive }
                    equals new { TestDrive = j2.LookUpValue } into join2
                from s in join2.DefaultIfEmpty()
                select new
                {
                    p.InquiryNumber,
                    p.InquiryDate,
                    p.CompanyCode,
                    o.DealerName,
                    o.OutletName,
                    p.BranchCode,
                    p.TipeKendaraan,
                    p.Variant,
                    p.Transmisi,
                    p.ColourCode,
                    ColourName = r.RefferenceDesc1,
                    p.NamaProspek,
                    p.EmployeeID,
                    q.EmployeeName,
                    p.PerolehanData,
                    p.LastProgress,
                    p.QuantityInquiry,
                    TestDrive = s.LookUpValueName,
                    pd.SPKNo,
                    SPKDate = p.SPKDate,
                    Hadiah = pd.GiftRefferenceValue,
                    pd.ShiftCode
                };

            qry = qry.Where(x => x.InquiryDate.Value.Year == 2015);

            qry = qry.OrderByDescending(p => p.InquiryNumber);
            return Json(qry.KGrid());
        }

        public JsonResult LookUpPartNo()
        {
            var PartNo = Request.Params["PartNo"];
            var qry = ctx.Database.SqlQuery<LiveStockPart>("exec uspfn_spLiveStockPartParam @PartNo=@p0", PartNo).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult LookUpPartNo2()
        {
            var PartNo = Request.Params["PartNo"];
            var qry = ctx.Database.SqlQuery<LiveStockPart>("exec uspfn_spLiveStockPartParam2 @PartNo=@p0", PartNo).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult LoopUpModel()
        {
            var PartNo = Request.Params["PartNo"];
            var qry = ctx.Database.SqlQuery<ModelLiveStockPart>("exec uspfn_spLiveStockPartLookUpModel @PartNo=@p0", PartNo).ToList();
            return Json(qry);
        }

        public JsonResult LookUpQuestionnaire()
        {
            var qry = ctx.Database.SqlQuery<QaModel>("exec uspfn_spGetQuestionnaires @companyCode=@p0, @branchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult LookUpQaCarModel()
        {
            var qry = ctx.Database.SqlQuery<QaModelLookUp>("exec uspfn_SpGetQaModel @companyCode=@p0, @branchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult LookUpQa2CarModel()
        {
            var qry = ctx.Database.SqlQuery<Qa2ModelLookUp>("exec uspfn_SpGetQa2Model @companyCode=@p0, @branchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult LookUpQuestionnaire2()
        {
            var qry = ctx.Database.SqlQuery<Qa2Model>("exec uspfn_spGetQuestionnaires2 @companyCode=@p0, @branchCode=@p1", CompanyCode, BranchCode).AsQueryable();

            return Json(qry.KGrid());
        }

        public JsonResult IndentList()
        {
            var qry = from p in ctx.pmIndents
                      join q in ctx.HrEmployees on new { p.CompanyCode, p.EmployeeID } equals new { q.CompanyCode, q.EmployeeID }
                      join j1 in ctx.OmMstRefferences on new { p.ColourCode, RefferenceType = "COLO" } equals new { ColourCode = j1.RefferenceCode, j1.RefferenceType } into join1
                      from r in join1.DefaultIfEmpty()
                      join j2 in ctx.GnMstLookUpDtls on new { p.TestDrive, CodeID = "PMOP", CompanyCode = "0000000" } equals new { TestDrive = j2.LookUpValue, j2.CodeID, j2.CompanyCode } into join2
                      from s in join2.DefaultIfEmpty()
                      join j3 in ctx.GnMstLookUpDtls on new { p.CityID, CodeID = "CITY", CompanyCode = "0000000" } equals new { CityID = j3.LookUpValue, j3.CodeID, j3.CompanyCode } into join3
                      from t in join3.DefaultIfEmpty()
                      join j4 in ctx.pmIndentAdditionals on new { p.CompanyCode, p.BranchCode, p.IndentNumber } equals new { j4.CompanyCode, j4.BranchCode, j4.IndentNumber } into join4
                      from u in join4.DefaultIfEmpty()
                      where p.CompanyCode == CompanyCode
                      select new
                      {
                          DealerName = ctx.GnMstDealerMappings.Where(a => a.DealerCode == p.CompanyCode).FirstOrDefault().DealerAbbreviation,
                          OutletName = ctx.GnMstDealerOutletMappings.Where(a => a.DealerCode == p.CompanyCode && a.OutletCode == p.BranchCode).FirstOrDefault().OutletAbbreviation,
                          p.IndentNumber,
                          p.IndentDate,
                          p.BranchCode,
                          p.TipeKendaraan,
                          p.Variant,
                          Transmision = ctx.GnMstLookUpDtls.Where(a => a.CompanyCode == "0000000" && a.CodeID == "TRTY" && a.LookUpValue == p.Transmisi).FirstOrDefault().LookUpValueName,
                          p.Transmisi,
                          p.ColourCode,
                          ColourName = r.RefferenceDesc1,
                          p.NamaProspek,
                          p.EmployeeID,
                          p.SpvEmployeeID,
                          q.EmployeeName,
                          PerolehanData = p.PerolehanData.ToUpper(),
                          p.LastProgress,
                          p.QuantityInquiry,
                          TestDrive = s.LookUpValueName,
                          p.TelpRumah,
                          p.NamaPerusahaan,
                          p.AlamatProspek,
                          p.AlamatPerusahaan,
                          p.Handphone,
                          p.CityID,
                          CityName = t.LookUpValueName,
                          p.Jabatan,
                          p.CaraPembayaran,
                          p.DownPayment,
                          p.Faximile,
                          p.LastUpdateStatus,
                          p.SPKDate,
                          p.Leasing,
                          p.StatusProspek,
                          p.Tenor,
                          p.Email,
                          p.LostCaseDate,
                          p.LostCaseCategory,
                          p.LostCaseOtherReason,
                          p.LostCaseReasonID,
                          p.LostCaseVoiceOfCustomer,
                          p.MerkLain,
                          q.Grade,
                          u.StatusVehicle,
                          u.OthersBrand,
                          u.OthersType,
                          p.DeliveryMonth,
                          p.DeliveryYear,
                          PeriodeDelivery = p.DeliveryYear +"-"+ p.DeliveryMonth
                      };
            qry = qry.Distinct();
            qry = qry.OrderByDescending(p => p.IndentNumber);
            return Json(qry.KGrid());
        }

        public JsonResult CsReviews()
        {
            string qry = @"exec uspfn_LookupCsReviews @GroupNo=@p0, @CompanyCode=@p1, @BranchCode=@p2, @Plan=@p3";
            var GroupNo = Request["GroupArea"];
            var CompanyCode = Request["CompanyCode"];
            var BranchCode = Request["BranchCode"];
            var Plan = Request["Plan"];
            var data = ctx.Database.SqlQuery<CsReviewModel>(qry, GroupNo, CompanyCode, BranchCode, Plan).AsQueryable();
            return Json(data.KGrid());
        }

        [HttpPost]
        public JsonResult WarrantyParts()
        {
            var queryable = ctx.MstItemInfos.Where(a=>a.CompanyCode == "6006406").AsQueryable();

            string partNo = Request["partNo"];

            if (string.IsNullOrEmpty(partNo) == false)
            {
                    queryable = queryable.Where(x => partNo.Contains(x.PartNo) == false);
            }

            return Json(GeLang.DataTables<SpMstItemInfo>.Parse(queryable, Request));
        }
    }
}
