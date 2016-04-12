using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.PreSales.Models;
using SimDms.Common;
using SimDms.Common.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class GridController : BaseController
    {
        private class CboItem
        {
            public string value { get; set; }
            public string text { get; set; }
            public string Position { get; set; } 
        }
        public JsonResult Cities()
        {
            var fltCode = Request["fltCityCode"];
            var fltName = Request["fltCityName"];
            var qry = from p in ctx.LookUpDtls
                      where p.CompanyCode == CompanyCode
                      && p.CodeID == "CITY"
                      select new
                      {
                          CityCode = p.LookUpValue,
                          CityName = p.LookUpValueName
                      };

            if (!string.IsNullOrWhiteSpace(fltCode)) qry = qry.Where(p => p.CityCode.Contains(fltCode));
            if (!string.IsNullOrWhiteSpace(fltName)) qry = qry.Where(p => p.CityName.Contains(fltName));

            return Json(qry.toKG());
        }

        public JsonResult InputKDP(int InquiryNumber)
        {
            var Branch = ctx.PmKdps.Where(a => a.InquiryNumber == InquiryNumber).FirstOrDefault().BranchCode;
            var qry = string.Format(@"select e.CompanyName BranchName, a.InquiryNumber,a.BranchCode,a.TipeKendaraan,a.Variant,a.Transmisi,a.ColourCode, ISNULL(d.RefferenceDesc1,'') ColourName
	            ,a.NamaProspek,a.EmployeeID,a.SpvEmployeeID,b.EmployeeName,UPPER(a.PerolehanData) PerolehanData,a.LastProgress,a.QuantityInquiry,ISNULL(c.LookUpValue, '') TestDrive
	            ,a.TelpRumah,a.NamaPerusahaan,a.AlamatProspek,a.AlamatPerusahaan,a.Handphone,a.InquiryDate,a.CityID,ISNULL(c1.LookUpValueName,'') CityName
	            ,a.Jabatan,a.CaraPembayaran,a.DownPayment,a.Faximile,a.LastUpdateStatus,a.SPKDate,a.Leasing,a.StatusProspek,a.Tenor,a.Email,a.LostCaseDate
	            ,a.LostCaseCategory,a.LostCaseOtherReason,a.LostCaseReasonID,a.LostCaseVoiceOfCustomer,a.MerkLain,b.Grade,b1.EmployeeID LeaderID,b1.EmployeeName LeaderName
				,f.StatusVehicle, f.OthersBrand BrandCode, f.OthersType ModelName
            from pmKDP a
            inner join HrEmployee b
            on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
            inner join HrEmployee b1
            on b.CompanyCode = b1.CompanyCode and b.TeamLeader = b1.EmployeeID
            left join gnMstLookUpDtl c
            on a.CompanyCode = c.CompanyCode and a.TestDrive = c.LookUpValue and c.CodeID = 'PMOP'
            left join gnMstLookUpDtl c1
            on a.CompanyCode = c.CompanyCode and a.CityID = c1.LookUpValue and c1.CodeID = 'CITY'
            left join omMstRefference d
            on a.CompanyCode = d.CompanyCode and a.ColourCode = d.RefferenceCode and RefferenceType = 'COLO'
            inner join gnMstCoProfile e
            on a.CompanyCode = e.CompanyCode and a.BranchCode = e.BranchCode
			left JOIN pmKdpAdditional f
			ON a.CompanyCode = f.CompanyCode and a.BranchCode = f.BranchCode AND a.InquiryNumber = f.InquiryNumber
            where a.CompanyCode = '{0}'
            and a.BranchCode = '{1}'
            and a.InquiryNumber = '{2}'
            ", CompanyCode, Branch, InquiryNumber);
            var result = ctx.Database.SqlQuery<InputKDP>(qry);

            var GroupCode = result.FirstOrDefault().TipeKendaraan;
            var TypeCode = result.FirstOrDefault().Variant;
            var ColourCode = result.FirstOrDefault().ColourCode;
            var Transmisi = result.FirstOrDefault().Transmisi;

            var Variant = ctx.GroupTypes
                   .Where(x => (x.CompanyCode == CompanyCode && x.GroupCode == GroupCode))
                   .Select(x => new { value = x.TypeCode, text = x.TypeCode }).Distinct().ToList();

            var Transmission = ctx.MstModels
                .Where(x => (x.CompanyCode == CompanyCode && x.GroupCode == GroupCode
                       && x.TypeCode == TypeCode))
                   .Select(x => new { value = x.TransmissionType, text = x.TransmissionType }).Distinct().ToList();

            //var ModelColour = ctx.MstRefferences
            //    .Where(x => (x.CompanyCode == CompanyCode && x.RefferenceType == "COLO"
            //           && x.RefferenceCode == ColourCode))
            //       .Select(x => new { value = x.RefferenceCode, text = x.RefferenceDesc1 }).ToList();
            var ModelColour = (from p in ctx.MstModelColours
                      join q in ctx.MstModels on
                      new { p.CompanyCode, p.SalesModelCode } equals
                      new { q.CompanyCode, q.SalesModelCode }
                      join r in ctx.MstRefferences on
                      new { p.CompanyCode, ColourCode = p.ColourCode, GroupCode = "COLO" } equals
                      new { r.CompanyCode, ColourCode = r.RefferenceCode, GroupCode = r.RefferenceType }
                      where p.CompanyCode == CompanyCode
                      && q.GroupCode == GroupCode
                      && q.TypeCode == TypeCode
                      && q.TransmissionType == Transmisi
                      select new
                      {
                          value = p.ColourCode,
                          text = r.RefferenceDesc1
                      }).Distinct().ToList();

            return Json(new { success = true, data = result, Variant = Variant, Transmission = Transmission, ModelColour = ModelColour });
        }

        public JsonResult KdpBrowse()
        {
            var qry = from p in ctx.PmKdps
                      join q in ctx.HrEmployees on new { p.CompanyCode, p.EmployeeID } equals new { q.CompanyCode, q.EmployeeID }
                      join q1 in ctx.HrEmployees on new { q.CompanyCode, q.TeamLeader } equals new { q1.CompanyCode, TeamLeader = q1.EmployeeID }
                      join j1 in ctx.MstRefferences on new { p.CompanyCode, p.ColourCode, RefferenceType = "COLO" } equals new { j1.CompanyCode, ColourCode = j1.RefferenceCode, j1.RefferenceType } into join1
                      from r in join1.DefaultIfEmpty()
                      join j2 in ctx.LookUpDtls on new { p.CompanyCode, p.TestDrive, CodeID = "PMOP" } equals new { j2.CompanyCode, TestDrive = j2.LookUpValue, j2.CodeID } into join2
                      from s in join2.DefaultIfEmpty()
                      join j3 in ctx.LookUpDtls on new { p.CompanyCode, p.CityID, CodeID = "CITY" } equals new { j3.CompanyCode, CityID = j3.LookUpValue, j3.CodeID } into join3
                      from t in join3.DefaultIfEmpty()
                      join j4 in ctx.PmKdpAdditionals on new { p.CompanyCode, p.BranchCode, p.InquiryNumber } equals new { j4.CompanyCode, j4.BranchCode, j4.InquiryNumber } into join4
                      from u in join4.DefaultIfEmpty()
                      where p.CompanyCode == CompanyCode
                      && p.BranchCode == BranchCode
                      select new
                      {
                          p.InquiryNumber,
                          p.BranchCode,
                          p.TipeKendaraan,
                          p.Variant,
                          p.Transmisi,
                          p.ColourCode,
                          ColourName = r.RefferenceDesc1,
                          p.NamaProspek,
                          p.EmployeeID,
                          p.SpvEmployeeID,
                          q.EmployeeName,
                          p.PerolehanData,
                          p.LastProgress,
                          p.QuantityInquiry,
                          TestDrive = s.LookUpValue,
                          p.TelpRumah,
                          p.NamaPerusahaan,
                          p.AlamatProspek,
                          p.AlamatPerusahaan,
                          p.Handphone,
                          p.InquiryDate,
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
                          LeaderID = q1.EmployeeID,
                          LeaderName = q1.EmployeeName,
                          u.StatusVehicle,
                          u.OthersBrand,
                          u.OthersType,
                      };

            return Json(qry.toKG());
        }

        public JsonResult KdpList()
        {
            var fltCar = Request["fltCar"];
            var fltVar = Request["fltVar"];
            var fltTrn = Request["fltTrn"];
            var fltCus = Request["fltCus"];
            var fltSls = Request["fltSls"];
            var fltSta = Request["fltSta"];
            var nikSales = Request["NikSales"];
            var nikSC = Request["NikSC"];
            var nikSH = Request["NikSH"];

            var employeeID = ctx.HrEmployees.Where(x => x.RelatedUser==CurrentUser.UserId).Select(x => x.EmployeeID).FirstOrDefault();
            var CheckEmp = ctx.HrEmployees.Where(x => x.RelatedUser == CurrentUser.UserId).FirstOrDefault();

            if (CheckEmp.Position == "SH" || CheckEmp.Position == "SC")
            {
                nikSales = "";
            }
            var subordinates = ctx.HrEmployees.Where(x => x.TeamLeader==employeeID && x.PersonnelStatus == "1").Select(x => x.EmployeeID).ToList();
            //var salesman = ctx.HrEmployees.Where(a => subordinates.Contains(a.TeamLeader) == true).Select(a => a.EmployeeID).ToList();

            //var listemp = ctx.HrEmployees.Where(x => subordinates.Contains(x.TeamLeader)).Select(x => x.EmployeeID).ToList();

            var qry = from p in ctx.PmKdps
                      join q in ctx.HrEmployees on new { p.CompanyCode, p.EmployeeID } equals new { q.CompanyCode, q.EmployeeID }
                      join j1 in ctx.MstRefferences on new { p.CompanyCode, p.ColourCode, RefferenceType = "COLO" } equals new { j1.CompanyCode, ColourCode = j1.RefferenceCode, j1.RefferenceType } into join1
                      from r in join1.DefaultIfEmpty()
                      join j2 in ctx.LookUpDtls on new { p.CompanyCode, p.TestDrive, CodeID = "PMOP" } equals new { j2.CompanyCode, TestDrive = j2.LookUpValue, j2.CodeID } into join2
                      from s in join2.DefaultIfEmpty()
                      join j3 in ctx.LookUpDtls on new { p.CompanyCode, p.CityID, CodeID = "CITY" } equals new { j3.CompanyCode, CityID = j3.LookUpValue, j3.CodeID } into join3
                      from t in join3.DefaultIfEmpty()
                      join j4 in ctx.PmKdpAdditionals on new { p.CompanyCode, p.BranchCode, p.InquiryNumber } equals new { j4.CompanyCode, j4.BranchCode, j4.InquiryNumber } into join4
                      from u in join4.DefaultIfEmpty()
                      where p.CompanyCode == CompanyCode
                      && p.BranchCode == BranchCode
                      select new
                      {
                          p.InquiryNumber,
                          p.BranchCode,
                          p.TipeKendaraan,
                          p.Variant,
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
                          TestDrive = s.LookUpValue,
                          p.TelpRumah,
                          p.NamaPerusahaan,
                          p.AlamatProspek,
                          p.AlamatPerusahaan,
                          p.Handphone,
                          p.InquiryDate,
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
                          u.OthersType
                      };

            
            if ((!string.IsNullOrEmpty(nikSH)) && string.IsNullOrEmpty(nikSales))
            {

                //qry = qry.Where(x => salesman.Contains(x.EmployeeID) == true); 
                //qry = qry.Where(x => listemp.Contains(x.EmployeeID) == true).OrderBy(v => v.InquiryNumber);
                var sc = ctx.HrEmployees.Where(x => subordinates.Contains(x.TeamLeader) == true).Select(x => x.EmployeeID).ToList();
                qry = qry.Where(x => sc.Contains(x.EmployeeID) == true || subordinates.Contains(x.EmployeeID) == true);

            }
            else
            if ((!string.IsNullOrEmpty(nikSC)) && string.IsNullOrEmpty(nikSales))
            {
                //qry = qry.Where(x => listemp.Contains(x.EmployeeID) == true).OrderBy(v => v.InquiryNumber);
                qry = qry.Where(x => subordinates.Contains(x.EmployeeID) == true);           
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(nikSales))
                {
                    qry = qry.Where(p => p.EmployeeID.Equals(nikSales));
                }
                else
                {
                    var emplList = ctx.HrEmployees.Where(p => p.CompanyCode == CompanyCode && p.TeamLeader == nikSC).Select(p => p.EmployeeID).ToList();
                    qry = qry.Where(p => emplList.Contains(p.EmployeeID));
                }
            }

            if (!string.IsNullOrWhiteSpace(fltCar)) qry = qry.Where(p => p.TipeKendaraan.Contains(fltCar));
            if (!string.IsNullOrWhiteSpace(fltVar)) qry = qry.Where(p => p.Variant.Contains(fltVar));
            if (!string.IsNullOrWhiteSpace(fltTrn)) qry = qry.Where(p => p.Transmisi.Contains(fltTrn));
            if (!string.IsNullOrWhiteSpace(fltCus)) qry = qry.Where(p => p.NamaProspek.Contains(fltCus));
            if (!string.IsNullOrWhiteSpace(fltSls)) qry = qry.Where(p => p.EmployeeName.Contains(fltSls));
            if (!string.IsNullOrWhiteSpace(fltSta)) qry = qry.Where(p => p.LastProgress.Contains(fltSta));

            return Json(qry.toKG());
        }

        public JsonResult ModelList()
        {
            var fltBrand = Request["fltBrand"];
            var fltModel = Request["fltModel"];
            var fltStatu = Request["fltStatu"];

            var qry = from p in ctx.ItsMstModels
                      select new
                      {
                          p.ModelType,
                          p.Variant,
                          p.BrandCode,
                          p.ModelName,
                          p.isSuzukiClass
                      };

            if (!string.IsNullOrWhiteSpace(fltBrand)) qry = qry.Where(p => p.BrandCode.Contains(fltBrand));
            if (!string.IsNullOrWhiteSpace(fltModel)) qry = qry.Where(p => p.ModelName.Contains(fltModel));
            if (!string.IsNullOrWhiteSpace(fltStatu))
            {
                if ((new string[] { "B", "D" }).Contains(fltStatu))
                {
                    qry = qry.Where(p => p.isSuzukiClass == true);
                }

                if ((new string[] { "C", "E" }).Contains(fltStatu))
                {
                    qry = qry.Where(p => p.isSuzukiClass == false);
                }
            }

            return Json(qry.toKG());
        }

        public JsonResult SMlist()
        {
            var nikSC = Request["NikSC"];
            var nikSH = Request["NikSH"];
            List<string> str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode).Select(x => x.EmployeeID).ToList();

            var qry = from p in ctx.HrEmployees 
                      where p.CompanyCode ==  CompanyCode
                      && p.Position == "S"
                      && p.Department =="sales"
                      && p.PersonnelStatus == "1"
                      select new
                      {
                          p.EmployeeID,
                          p.EmployeeName,
                          p.Position,
                          p.TeamLeader
                      };
            //if (!string.IsNullOrWhiteSpace(nikSC)) str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode && x.TeamLeader == nikSC).Select(x => x.EmployeeID).ToList();
            //if (!string.IsNullOrWhiteSpace(nikSH)) str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode && x.TeamLeader == nikSH).Select(x => x.EmployeeID).ToList();
            if (!string.IsNullOrWhiteSpace(nikSC)) qry = qry.Where(p => p.TeamLeader == nikSC);
            if (!string.IsNullOrWhiteSpace(nikSH)) qry = qry.Where(p => p.TeamLeader == nikSH);

            return Json(qry.AsQueryable().toKG());
        }

        public JsonResult SHlist()
        {
            var qry = from p in ctx.HrEmployees
                      join q in ctx.HrEmployeeMutations
                        on new { p.CompanyCode, p.EmployeeID } equals new { q.CompanyCode, q.EmployeeID}
                      where p.CompanyCode == CompanyCode && q.BranchCode == BranchCode
                      && p.Position == "SH"
                      && p.Department == "sales"
                      && p.PersonnelStatus == "1"
                      select new
                      {
                          p.EmployeeID,
                          p.EmployeeName,
                          p.Position,
                          p.TeamLeader
                      };

            return Json(qry.AsQueryable().toKG());
        }

        public JsonResult SMlist1() 
        {
            var nikSH = Request["cmbSH"];
            List<string> str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode).Select(x => x.EmployeeID).ToList();
            var qry = from p in ctx.HrEmployees
                      where p.CompanyCode == CompanyCode
                      && p.Department == "sales"
                      && p.PersonnelStatus == "1"
                      select new
                      {
                          p.EmployeeID,
                          p.EmployeeName,
                          p.Position,
                          p.TeamLeader
                      };
            if (!string.IsNullOrWhiteSpace(nikSH))
            {
                str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode && x.TeamLeader == nikSH).Select(x => x.EmployeeID).ToList();
            }
            qry = qry.Where(p => str.Contains(p.TeamLeader) || str.Contains(p.EmployeeID));
            return Json(qry.AsQueryable().toKG());
        }

        public JsonResult BranchManager() 
        {
            //var positionID = Request["PositionID"];
            //var employeeID = Request["EmployeeID"];

            //List<string> str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode).Select(x => x.EmployeeID).ToList();
            var qrystr = string.Format(@"select a.EmployeeID as value, a.EmployeeName as text, 'BM' Position
                                        from HrEmployee a
                                        where a.CompanyCode = '{0}'
                                        and a.Department = 'SALES'
                                        and a.Position = 'BM'
                                        and a.PersonnelStatus = '1'
                                        group by a.EmployeeID, a.EmployeeName  
                                        union                          
                                        select a.EmployeeID as value, b.EmployeeName as text, 'BM' Position
                                        from HrEmployeeAdditionalJob a
                                        inner join HrEmployee b
                                        on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
                                        where a.Position <> b.Position
                                        and a.Position = 'BM'
                                        and a.CompanyCode = '{0}'
                                        and a.Department = 'SALES'
                                        and b.PersonnelStatus = '1'", CompanyCode);

            var Branch = ctx.Database.SqlQuery<CboItem>(qrystr).AsQueryable();
            var qry = from p in (ctx.Database.SqlQuery<CboItem>(qrystr))
                      select new
                      {
                          EmployeeID = p.value,
                          EmployeeName = p.text,
                          TitleName = p.Position 
                      };

            return Json(qry.AsQueryable().toKG());
        }

        public JsonResult SalesHead() 
        {
            var branchManager = Request["BranchManager"];
            var cmbbm = Request["cmbBM"]; 
            //List<string> str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode).Select(x => x.EmployeeID).ToList();
            //jabatan asli n additional
            var oriPosition = ctx.HrEmployees.Where(p => p.EmployeeID == cmbbm).FirstOrDefault();
            var adiPosition = ctx.HrEmployeeAdditionalJobs.Where(p => p.EmployeeID == cmbbm).FirstOrDefault();
            var qry = from p in ctx.HrEmployees
                      where p.CompanyCode == CompanyCode
                      && (p.Position == "SH" || p.Position == "SC")
                      && p.Department == "sales"
                      && p.PersonnelStatus == "1"
                      select new
                      {
                          p.EmployeeID,
                          p.EmployeeName,
                          TitleName = p.Position,
                          p.TeamLeader
                      };

            if (oriPosition != null && adiPosition != null)
            {
                if (oriPosition.Position + adiPosition.Position == "BMSH")
                {
                    qry = (from p in ctx.HrEmployees
                           join q in ctx.HrEmployeeAdditionalJobs
                             on new { p.CompanyCode, p.EmployeeID } equals new { q.CompanyCode, q.EmployeeID }
                           where p.CompanyCode == CompanyCode
                           && (p.Position == "SH" || p.Position == "SC"
                           || q.Position == "SH")
                           && p.Department == "sales"
                           && p.PersonnelStatus == "1"
                           select new
                           {
                               p.EmployeeID,
                               p.EmployeeName,
                               TitleName = q.Position,
                               p.TeamLeader
                           }).Distinct();
                }
            }
            
            if (!string.IsNullOrWhiteSpace(branchManager)) qry = qry.Where(p => p.TeamLeader == branchManager || p.EmployeeID == branchManager);
            if (!string.IsNullOrWhiteSpace(cmbbm)) qry = qry.Where(p => p.TeamLeader == cmbbm || p.EmployeeID == cmbbm);
            return Json(qry.AsQueryable().toKG());
        }

        public JsonResult Salesman() 
        {
            var nikSH = Request["SalesHead"];
            var nikSC = Request["SalesCoor"];
            var cmbsh = Request["cmbSH"];
            var cmbsc = Request["cmbSC"];
            List<string> str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode).Select(x => x.EmployeeID).ToList();

            var qry = from p in ctx.HrEmployees
                      where p.CompanyCode == CompanyCode
                      && p.Department == "sales"
                      && p.PersonnelStatus == "1"
                      select new
                      {
                          p.EmployeeID,
                          p.EmployeeName,
                          p.Position,
                          p.TeamLeader
                      };
            if (!string.IsNullOrWhiteSpace(nikSH))
            {
                str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode && x.TeamLeader == nikSH).Select(x => x.EmployeeID).ToList();
            }
            if (!string.IsNullOrWhiteSpace(nikSC))
            {
                str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode && x.TeamLeader == nikSC).Select(x => x.EmployeeID).ToList();
            }
            if (!string.IsNullOrWhiteSpace(cmbsh))
            {
                str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode && x.TeamLeader == cmbsh).Select(x => x.EmployeeID).ToList();
            }
            if (!string.IsNullOrWhiteSpace(cmbsc))
            {
                str = ctx.HrEmployees.Where(x => x.CompanyCode == CompanyCode && x.TeamLeader == cmbsc).Select(x => x.EmployeeID).ToList();
            }
            qry = qry.Where(p => str.Contains(p.TeamLeader) || str.Contains(p.EmployeeID));
            return Json(qry.AsQueryable().toKG());
        }

   }
}
