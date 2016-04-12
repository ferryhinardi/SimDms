using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Transactions;
using ClosedXML;
using ClosedXML.Excel;
using System.Data;
using DocumentFormat.OpenXml;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using GemBox.Spreadsheet;
using System.Globalization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class SPKExhibitionController : BaseController
    {
        CultureInfo ci = new CultureInfo("ID-id");

        public JsonResult Default()
        {
            return Json(new
            {
                success = true,
                data = new
                {
                    InquiryDate = DateTime.Now,
                    QuantityInquiry = 1,
                    SPKDate = DateTime.Now,
                    Date = DateTime.Now,
                    DateFrom = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    DateTo = DateTime.Now,
                    LastProgress = "SPK"
                }
            });
        }

        public IEnumerable<dynamic> CarMappingList()
        {
            var qry = from g in ctx.PmGroupTypeSeqs
                      join m in ctx.OmMstModels on new { g.GroupCode, g.TypeCode } equals new { m.GroupCode, m.TypeCode } into m2
                      from m in m2.DefaultIfEmpty()
                      join c in ctx.OmMstModelColours on m.SalesModelCode equals c.SalesModelCode into c2
                      from c in c2.DefaultIfEmpty()
                      where c.ColourCode != null 
                             && m.Status == "1"
                             && m.CompanyCode == "0000000"
                      orderby g.GroupCode, g.TypeCode, c.ColourCode
                      select new
                      {
                          TipeKendaraan = g.GroupCode,
                          Variant = g.TypeCode,
                          TransmissionType = m != null ? m.TransmissionType : null,
                          ColorCode = c != null ? c.ColourCode : null,
                          ColorName = (from r in ctx.OmMstRefferences
                                       where r.RefferenceType == "COLO" && r.RefferenceCode == c.ColourCode
                                       select r.RefferenceDesc1).FirstOrDefault()
                      };

            return qry;
        }

        #region "Get Master Data"
        public JsonResult GetGrade(string Company, string id = "")
        {
            var model = ctx.HrEmployees.Find(Company, id);
            return Json(model.Grade);
        }

        public JsonResult GetSalesCoordinator(string Company, string id)
        {
            
            var model = ctx.HrEmployees.Find(Company, id);
            var NikSC = ctx.HrEmployees.Find(Company, model.TeamLeader);
            if (NikSC != null)
            {
                return Json(NikSC.EmployeeID);

            }
            else
            {
                return Json(null);
            }
        }

        public JsonResult GetSalesHead(string Company, string id)
        {
            var model = ctx.HrEmployees.Find(Company, id);
            if (model != null)
            {
                var teamLead = model == null ? "" : model.TeamLeader;
                var NikSH = ctx.HrEmployees.Find(Company, teamLead);
                return Json(NikSH == null ? "" : NikSH.EmployeeID);
            }
            else
            {
                return Json(null);
            }
            
        }

        public JsonResult GetOutletBySales(string Company, string Employee)
        {
            var emp = ctx.HrEmployeeMutations.OrderByDescending(p => p.CreatedDate)
                .Where(p => p.CompanyCode == Company && p.EmployeeID == Employee  && p.IsJoinDate == true)
                .FirstOrDefault();
            var list = ctx.GnMstDealerOutletMappings.OrderBy(p => p.OutletCode).Where(p => p.DealerCode == Company & p.isActive == true).FirstOrDefault();
            var outletCode = "";
            if (emp != null)
            {
                list = ctx.GnMstDealerOutletMappings.OrderBy(p => p.OutletCode).Where(p => p.DealerCode == Company && p.isActive == true && p.OutletCode == emp.BranchCode).FirstOrDefault();
                if (list != null)
                {
                    outletCode = list.OutletCode;
                }
                else outletCode = emp.BranchCode;

                return Json(outletCode);
            }
            else
            {
                outletCode = ctx.Database.SqlQuery<string>("select dbo.GetBranchCodeByEmp('" + Company + "','" + Employee + "')").FirstOrDefault();

                return Json(outletCode);
            }
        }

        public JsonResult CarTypes()
        {
            var data = ctx.Database.SqlQuery<MyComboList>("Exec uspfn_getCarTypes" ).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

            //return Json(CarMappingList().Select(p => new
            //{
            //    value = p.TipeKendaraan,
            //    text = p.TipeKendaraan
            //}).OrderBy(p=> p.value).Distinct());
        }

        public JsonResult CarVariants(string id = "")
        {

            var data = ctx.Database.SqlQuery<MyComboList>("Exec uspfn_getCarvariants '" + id + "'  " ).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

            //var list = from g in ctx.PmGroupTypeSeqs
            //           join m in ctx.OmMstModels on new { g.GroupCode, g.TypeCode } equals new { m.GroupCode, m.TypeCode } into m2
            //           from m in m2.DefaultIfEmpty()
            //           where g.GroupCode == id
            //                && m.CompanyCode == "0000000"
            //           select new
            //           {
            //               value = m.TypeCode,
            //               text = m.TypeCode
            //           };

            //return Json(list.Distinct());
            //return Json(CarMappingList().Where(p => p.TipeKendaraan == id).Select(p => new { 
            //    value = p.Variant, 
            //    text = p.Variant
            //}).Distinct());
        }

        public JsonResult Transmissions()
        {
            var cartype = Request["CarType"];
            var carvari = Request["CarVariant"];

            var data = ctx.Database.SqlQuery<MyComboList>("Exec uspfn_getCarTransmissionTypes '" + cartype + "','" + carvari + "'").ToList();
            return Json(data, JsonRequestBehavior.AllowGet);

            //var list = from c in ctx.OmMstModels
            //           join l in ctx.GnMstLookUpDtls on c.TransmissionType equals l.LookUpValue
            //           where l.CodeID == "TRTY" && c.GroupCode == cartype && c.TypeCode == carvari
            //                && c.CompanyCode == "0000000"
            //           select new
            //           {
            //               value = c.TransmissionType,
            //               text = c.TransmissionType + " - " + l.LookUpValueName
            //           };

            //return Json(list.Distinct());
        }

        public JsonResult ModelColors()
        {
            var cartype = Request["CarType"];
            var carvari = Request["CarVariant"];
            var cartran = Request["CarTrans"];

            var data = ctx.Database.SqlQuery<MyComboList>("Exec uspfn_getCarColors '" + cartype + "', '" + carvari + "','"  + cartran + "'").ToList();

            return Json(data, JsonRequestBehavior.AllowGet);

            //var list = from c in ctx.OmMstModels
            //           join l in ctx.OmMstModelColours on c.SalesModelCode equals l.SalesModelCode
            //           join k in ctx.OmMstRefferences on l.ColourCode equals k.RefferenceCode
            //           where k.RefferenceType == "COLO" && c.GroupCode == cartype && c.TypeCode == carvari
            //                && c.CompanyCode == "0000000"
            //           select new
            //           {
            //               value = l.ColourCode,
            //               text = l.ColourCode + " - " + k.RefferenceDesc1
            //           };

            //return Json(list.Distinct());
        }

        #endregion

        #region "Private Method"
        private void GenerateExcel(XLWorkbook wb, DataTable dt, int lastRow, bool isCustomHeader = false, bool isShowSummary = false)
        {
            var ws = wb.Worksheet(1);
            var tmpLastRow = lastRow;

            int iCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                iCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    Type typ = dr[dc.ColumnName].GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            val = Convert.ToDateTime(val).ToString("dd-MMM-yyyy",ci).Insert(0,"'");
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,###";
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, iCol).Style.NumberFormat.Format = "#,###.0";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }
                            ws.Cell(lastRow + 1, iCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (tmpLastRow == lastRow)
                    {
                        ws.Cell(lastRow, iCol).Value = dc.ColumnName;
                        ws.Cell(lastRow, iCol).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, iCol).Style.Font.SetBold().Font.SetFontSize(10);

                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow + 1, iCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, iCol).Value = val;
                    }

                    iCol++;
                }

                lastRow++;
            }

            if (isShowSummary)
            {
            }

            var rngTable = ws.Range(tmpLastRow, 1, lastRow, iCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin);


            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();

            wb.PageOptions.SetPageOrientation(XLPageOrientation.Landscape);
            wb.PageOptions.SetPaperSize(XLPaperSize.LegalExtraPaper);
            wb.PageOptions.SetScale(100);
            wb.PageOptions.SetScaleHFWithDocument(true);
            wb.PageOptions.SetPagesWide(100);
            wb.PageOptions.SetHorizontalDpi(200);
            wb.PageOptions.SetPagesTall(100);
            wb.PageOptions.FitToPages(100, 100);
        }
        
        private ActionResult ShowReport(XLWorkbook wb, string fileName, string Device){
            if (Device == "desktop")
            {
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                    Response.Close();

                    return File(Response.OutputStream, Response.ContentType);
                }
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    wb.SaveAs(ms);
                    ms.Position = 0;

                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                        ExcelFile ef = ExcelFile.Load(ms, LoadOptions.XlsxDefault);
                        ef.Save(ms2, SaveOptions.HtmlDefault);
                        ms2.Position = 0;
                        ms.Close();
                        ms2.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                        Response.Close();
                        ms2.Close();

                        return new FileStreamResult(Response.OutputStream, "text/html");
                    }
                }
            }
        }
        #endregion

        #region "CRUD"

        public JsonResult Get()
        {
            try
            {
                var company = Request["CompanyCode"];
                var branch = Request["BranchCode"];
                var inquno = Int32.Parse(Request["InquiryNumber"]);

                var qry = from p in ctx.PmKDPExhibitions
                          join j1 in ctx.GnMstLookUpDtls on new { p.CompanyCode, CodeID = "ITLR", LookUpValue = p.LostCaseReasonID } equals new { j1.CompanyCode, j1.CodeID, j1.LookUpValue } into join1
                          from q in join1.DefaultIfEmpty()
                          join j2 in ctx.OmMstRefferences on new { p.CompanyCode, p.ColourCode, RefferenceType = "COLO" } equals new { j2.CompanyCode, ColourCode = j2.RefferenceCode, j2.RefferenceType } into join2
                          from r in join2.DefaultIfEmpty()
                          join j3 in ctx.HrEmployees on new { p.CompanyCode, p.SpvEmployeeID } equals new { j3.CompanyCode, SpvEmployeeID = j3.EmployeeID } into join3
                          from s in join3.DefaultIfEmpty()
                          join j4 in ctx.GnMstLookUpDtls on new { CompanyCode = "0000000", CodeID = "CITY", LookUpValue = p.CityID }
                                equals new { j4.CompanyCode, j4.CodeID, j4.LookUpValue } into join4
                          from t in join4.DefaultIfEmpty()
                          join j5 in ctx.PmKDPAdditionalExhibitions on new { p.CompanyCode, p.BranchCode, p.InquiryNumber, GiftRefferenceCode = "GIFT" } equals new { j5.CompanyCode, j5.BranchCode, j5.InquiryNumber, j5.GiftRefferenceCode } into join5
                          from u in join5.DefaultIfEmpty()
                          join j6 in ctx.HrEmployees on new { p.CompanyCode, p.EmployeeID } equals new { j6.CompanyCode, j6.EmployeeID } into join6
                          from v in join6.DefaultIfEmpty()
                          where
                          p.CompanyCode == company
                          &&
                          p.BranchCode == branch
                          &&
                          p.InquiryNumber == inquno
                          select new
                          {
                              p.InquiryNumber,
                              p.BranchCode,
                              p.CompanyCode,
                              p.EmployeeID,
                              p.SpvEmployeeID,
                              p.InquiryDate,
                              p.OutletID,
                              p.StatusProspek,
                              p.PerolehanData,
                              p.NamaProspek,
                              p.AlamatProspek,
                              p.TelpRumah,
                              p.CityID,
                              p.NamaPerusahaan,
                              p.AlamatPerusahaan,
                              p.Jabatan,
                              p.Handphone,
                              p.Faximile,
                              p.Email,
                              p.TipeKendaraan,
                              p.Variant,
                              p.Transmisi,
                              p.ColourCode,
                              p.CaraPembayaran,
                              p.TestDrive,
                              p.QuantityInquiry,
                              p.LastProgress,
                              p.LastUpdateStatus,
                              p.SPKDate,
                              u.SPKNo,
                              Hadiah = u.GiftRefferenceValue,
                              p.LostCaseDate,
                              p.LostCaseCategory,
                              p.LostCaseReasonID,
                              p.LostCaseOtherReason,
                              p.LostCaseVoiceOfCustomer,
                              p.Leasing,
                              p.DownPayment,
                              p.Tenor,
                              p.MerkLain,
                              v.Grade,
                              Variantstext = p.Variant,
                              Transmisistext = p.Transmisi,
                              ColourCodestext = r.RefferenceDesc1,
                              LostCaseReasonIDstext = q.LookUpValueName,
                              SpvEmployeeName = s.EmployeeName,
                              LeasingCode = p.Leasing,
                              CityName = t.LookUpValueName,
                              StatusVehicle = u.StatusVehicle,
                              BrandCode = u.OthersBrand,
                              ModelName = u.OthersType,
                              NikSales = p.EmployeeID,
                              NikSC = p.SpvEmployeeID,
                              NikSalesstext = (v.Position == "S" ? v.EmployeeName : "--  --"),
                              u.ShiftCode
                          };

                var data = from p in ctx.PmActivities
                           where p.CompanyCode == CompanyCode
                           && p.BranchCode == branch
                           && p.InquiryNumber == inquno
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

                if (qry.Count() > 0)
                {
                    return Json(new { success = true, data = qry.FirstOrDefault(), list = data.ToList() });
                }
                else
                {
                    return Json(new { success = false, message = "data not found..." });
                }
            }
            catch (Exception ex)
            {
                {
                    return Json(new { success = false, message = (ex.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message });
                }
            }
        }

        public JsonResult Save(PmKDPExhibition model)
        {
            var CekData = ctx.PmKDPAdditionalExhibitions.Where(a => a.CompanyCode == model.CompanyCode && a.BranchCode == model.BranchCode && a.SPKNo == model.SPKNo).FirstOrDefault();
            var DealerName = ctx.GnMstDealerMappings.Where(a => a.DealerCode == model.CompanyCode).FirstOrDefault().DealerName;
            var OutletName = ctx.GnMstDealerOutletMappings.Where(a => a.DealerCode == model.CompanyCode && a.OutletCode == model.BranchCode).FirstOrDefault().OutletName;
                
            if (CekData != null)
            {
                return Json(new { success = false, message = "No SPK " + model.SPKNo + " Sudah ada di Dealer " + DealerName + ", Outlate " + OutletName + " !" });
            }

            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted, Timeout = new TimeSpan(0, 15, 0) }))
                {
                    bool bolNew = false;

                    if (model.BranchCode == null )
                    {
                        model.BranchCode = ctx.Database.SqlQuery<string>("select dbo.GetBranchCodeByEmp('" + model.CompanyCode + "','" + model.NikSales + "')").FirstOrDefault();
                    }

                    model.AlamatProspek = string.IsNullOrEmpty(model.AlamatProspek) ? "" :
                        model.AlamatProspek.Length > 200 ? model.AlamatProspek.Substring(0, 200) : model.AlamatProspek;
                    model.AlamatPerusahaan = string.IsNullOrEmpty(model.AlamatPerusahaan) ? "" :
                        model.AlamatPerusahaan.Length > 200 ? model.AlamatPerusahaan.Substring(0, 200) : model.AlamatPerusahaan;

                    //Validasi hanya berdasarkan Inquiry Number saja kareana user boleh merubah Dealer dan Outlet  
                    var record = ctx.PmKDPExhibitions.ToList().Find(p => p.InquiryNumber == model.InquiryNumber);
                    //var record = ctx.PmKDPExhibitions.Find(model.InquiryNumber);
                    //, model.BranchCode, model.CompanyCode);
                    var userID = CurrentUser.Username; //CurrentUser.UserId;
                    var currentDate = DateTime.Now;
                    model.LastProgress = "SPK"; //Default adalah SPK
                     
                    if (record == null)
                    {
                        var newNumber = 1;

                        PmKDPExhibition pmkdp = ctx.PmKDPExhibitions.FirstOrDefault();
                        if (pmkdp != null)
                        {
                            newNumber = ctx.PmKDPExhibitions.Select(p => p.InquiryNumber).Max() + 1;
                        };
                        record = new PmKDPExhibition
                        {
                            InquiryNumber = newNumber,
                            CompanyCode = model.CompanyCode,
                            BranchCode = model.BranchCode,
                            CreatedBy = userID,
                            CreationDate = currentDate
                        };
                        ctx.PmKDPExhibitions.Add(record);
                        bolNew = true;
                    }

                    var outlet = ctx.PmBranchOutlets.Where(p => p.CompanyCode == model.CompanyCode && p.BranchCode == model.BranchCode).FirstOrDefault();
                    record.OutletID = (outlet == null) ? "" : outlet.OutletID;
                    record.EmployeeID = model.NikSales;
                    record.SpvEmployeeID = model.NikSC ?? "";
                    record.InquiryDate = model.InquiryDate;
                    record.StatusProspek = model.StatusProspek;
                    record.PerolehanData = model.PerolehanData;
                    record.NamaProspek = model.NamaProspek;
                    record.AlamatProspek = model.AlamatProspek;
                    record.TelpRumah = model.TelpRumah;
                    record.CityID = model.CityID;
                    record.NamaPerusahaan = model.NamaPerusahaan;
                    record.AlamatPerusahaan = model.AlamatPerusahaan;
                    record.Jabatan = model.Jabatan;
                    record.Handphone = model.Handphone;
                    record.Faximile = model.Faximile;
                    record.Email = model.Email;
                    record.TipeKendaraan = model.TipeKendaraan;
                    record.Variant = model.Variant;
                    record.Transmisi = model.Transmisi;
                    record.ColourCode = model.ColourCode;
                    record.CaraPembayaran = model.CaraPembayaran;

                    if (model.CaraPembayaran == "20")
                    {
                        record.Leasing = model.Leasing;
                        record.DownPayment = model.DownPayment;
                        record.Tenor = model.Tenor;
                    }
                    else
                    {
                        record.Leasing = "";
                        record.DownPayment = "";
                        record.Tenor = "";
                    }

                    record.TestDrive = model.TestDrive;
                    record.QuantityInquiry = model.QuantityInquiry;
                    record.LastProgress = model.LastProgress;
                    record.LastUpdateStatus = currentDate;


                    //Validasi hanya berdasarkan Inquiry Number saja kareana user boleh merubah Dealer dan Outlet  
                    var seqs = ctx.PmStatusHistoryExhibitions.Where(p => p.InquiryNumber == record.InquiryNumber);
                    //var seqs = from p in ctx.PmStatusHistoryExhibitions
                    //           where p.InquiryNumber == record.InquiryNumber
                    //           //&& p.CompanyCode == record.CompanyCode
                    //           //&& p.BranchCode == record.BranchCode
                    //           select p.SequenceNo;

                    var newseqno = (seqs.Count() > 0) ? seqs.Max(p => p.SequenceNo) + 1 : 1;
                    var hist = new PmStatusHistoryExhibition
                    {
                        InquiryNumber = record.InquiryNumber,
                        SequenceNo = newseqno,
                        LastProgress = record.LastProgress,
                        UpdateDate = record.LastUpdateStatus,
                        UpdateUser = userID
                    };
                    hist.CompanyCode = record.CompanyCode;
                    hist.BranchCode = record.BranchCode;

                    ctx.PmStatusHistoryExhibitions.Add(hist);
                    ctx.SaveChanges();

                    if (model.CompanyCode != record.CompanyCode)
                    {
                        string sql = string.Format("UPDATE pmStatusHistoryExhibition SET CompanyCode = '{0}', BranchCode = '{1}' " +
                            "WHERE InquiryNumber = {2} AND SequenceNo = {3}", model.CompanyCode, model.BranchCode, model.InquiryNumber, newseqno);
                        ctx.Database.ExecuteSqlCommand(sql);

                        ctx.SaveChanges();
                    }

                    record.SPKDate = model.SPKDate;
                    record.LostCaseDate = model.LostCaseDate;
                    record.LostCaseCategory = model.LostCaseCategory;
                    record.LostCaseReasonID = model.LostCaseReasonID;
                    record.LostCaseOtherReason = model.LostCaseOtherReason;
                    record.LostCaseVoiceOfCustomer = model.LostCaseVoiceOfCustomer;
                    record.MerkLain = model.MerkLain ?? "";
                    record.LastUpdateBy = userID;
                    record.LastUpdateDate = currentDate;

                    var giftRefferenceCode = "GIFT";
                    var giftRefferenceDesc = ctx.GnMstLookUpDtls.Where(
                            p => p.CompanyCode == "0000000" && p.CodeID == giftRefferenceCode && p.LookUpValue == model.Hadiah)
                            .FirstOrDefault().LookUpValueName;

                    //Validasi hanya berdasarkan Inquiry Number saja kareana user boleh merubah Dealer dan Outlet  
                    var oKdpAdd = ctx.PmKDPAdditionalExhibitions.ToList().Find(p => p.InquiryNumber == record.InquiryNumber);
                    //var oKdpAdd = ctx.PmKDPAdditionalExhibitions.Find(record.InquiryNumber);
                    //record.CompanyCode, record.BranchCode,
                    if (oKdpAdd == null)
                    {
                        oKdpAdd = new PmKdpAdditionalExhibition
                        {
                            InquiryNumber = record.InquiryNumber,
                            CompanyCode = record.CompanyCode,
                            BranchCode = record.BranchCode,
                            CreatedBy = record.LastUpdateBy,
                            CreatedDate = record.LastUpdateDate,
                        };
                        ctx.PmKDPAdditionalExhibitions.Add(oKdpAdd);
                    }

                    oKdpAdd.SPKNo = model.SPKNo;
                    oKdpAdd.SPKDate = model.SPKDate;
                    oKdpAdd.GiftRefferenceCode = giftRefferenceCode;
                    oKdpAdd.GiftRefferenceValue = model.Hadiah;
                    oKdpAdd.GiftRefferenceDesc = giftRefferenceDesc;
                    oKdpAdd.ShiftCode = model.ShiftCode;
                    //
                    oKdpAdd.StatusVehicle = model.StatusVehicle;
                    oKdpAdd.OthersBrand = model.BrandCode;
                    oKdpAdd.OthersType = model.ModelName;
                    oKdpAdd.LastUpdateBy = record.LastUpdateBy;
                    oKdpAdd.LastUpdateDate = record.LastUpdateDate;

                    ctx.SaveChanges();

                    if (!bolNew)
                    {
                        if (model.CompanyCode != record.CompanyCode)
                        {
                            string sql = string.Format("UPDATE pmKDPExhibition SET CompanyCode = '{0}', BranchCode = '{1}' " +
                                "WHERE InquiryNumber = {2}", model.CompanyCode, model.BranchCode, model.InquiryNumber);
                            ctx.Database.ExecuteSqlCommand(sql);

                            sql = string.Format("UPDATE pmKDPAdditionalExhibition SET CompanyCode = '{0}', BranchCode = '{1}' " +
                                "WHERE InquiryNumber = {2}", model.CompanyCode, model.BranchCode, model.InquiryNumber);
                            ctx.Database.ExecuteSqlCommand(sql);

                            ctx.SaveChanges();
                        }
                    }

                    GenerateInsPmKdp(record, oKdpAdd, hist, bolNew);
                    tranScope.Complete();


                    return Json(new
                    {
                        success = true,
                        message = "Nomor SPK: \"" + model.SPKNo + "\" berhasil disimpan ke database...",
                        data = new { InquiryNumber = record.InquiryNumber, CompanyCode = record.CompanyCode, BranchCode = record.BranchCode, LastUpdateStatus = record.LastUpdateDate }
                    });
                }
            }
            catch (Exception ex)
            {

                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                string innerEx = (ex.InnerException == null) ? ex.Message :
                    (ex.InnerException.InnerException == null) ? ex.Message : ex.InnerException.InnerException.Message;
                {
                    return Json(new { 
                        success = false,
                        message = (ex.InnerException == null) ? 
                        " Periksa kembali inputan Anda!" + " Error Message:" + ex.Message :
                        " Periksa kembali inputan Anda!" + " Error Message:" + innerEx});
                }
            }
        }

        #endregion

        private string GetUserEx(string UserLogin)
        {
            var str = "EX_" + UserLogin;

            if (str.Length > 15)
            {
                return str.Substring(0, 15);
            }
            else
                return str;
        }

        #region "SPK Exhibition Report"
        public ActionResult RawdataSPKExhibitionByInquiryDate(DateTime DateFrom, DateTime DateTo, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_PmRawdataSPKExhibitionByInquiryDate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@DateTo", DateTo.ToString("yyyyMMdd"));

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "RawdataSPKExhibitionByInquiryDate_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("SPKExhibition_InquiryDate");

            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            lastRow++;
            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Rawdata SPK Exhibition by Inquiry Date";
            lastRow++;
            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Cell("A" + lastRow).Value = "Period: " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            lastRow++;
            ws.Cell("A" + lastRow).Value = "";
            lastRow++;

            GenerateExcel(wb, dt, lastRow, false, false);
            return ShowReport(wb, fileName, Device);
        }

        public ActionResult RawdataSPKExhibitionBySPKDate(DateTime DateFrom, DateTime DateTo, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_PmRawdataSPKExhibitionBySPKDate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@DateTo", DateTo.ToString("yyyyMMdd"));

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "RawdataSPKExhibitionBySPKDate_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("SPKExhibition_SPKDate");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Rawdata SPK Exhibition by SPK Date";
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Cell("A" + lastRow).Value = "Period: " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            lastRow++;
            ws.Cell("A" + lastRow).Value = "";
            lastRow++;

            GenerateExcel(wb, dt, lastRow, false, false);
            return ShowReport(wb, fileName, Device);
        }

        public ActionResult RekapSPKExhibitionCustomer(DateTime DateFrom, DateTime DateTo, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_PmRekapSPKExhibitionCustomer";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@DateTo", DateTo.ToString("yyyyMMdd"));

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "RekapSPKExhibitionCustomer_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("RekapSPKExhibitionCustomer");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Rekap SPK Per Customer";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Period: " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            lastRow++;
            ws.Cell("A" + lastRow).Value = "";
            lastRow++;

            GenerateExcel(wb, dt, lastRow, false, false);
            return ShowReport(wb, fileName, Device);
        }

        public ActionResult RekapSPKExhibitionCustomerByInquiryDate(DateTime DateFrom, DateTime DateTo, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_PmRekapSPKExhibitionCustomerByInquiryDate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@DateTo", DateTo.ToString("yyyyMMdd"));

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "RekapSPKExhibitionCustomerByInquiryDate_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("RekapSPKExhiCustByInquiryDate");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Rekap SPK Per Customer By Inquiry Date";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Period: " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            lastRow++;
            ws.Cell("A" + lastRow).Value = "";
            lastRow++;

            GenerateExcel(wb, dt, lastRow, false, false);
            return ShowReport(wb, fileName, Device);
        }

        public ActionResult RekapSPKExhibitionCustomerBySPKDate(DateTime DateFrom, DateTime DateTo, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_PmRekapSPKExhibitionCustomerBySPKDate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@DateTo", DateTo.ToString("yyyyMMdd"));

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "RekapSPKExhiCustBySPKDate_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("RekapSPKExhiCustBySPKDate");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Rekap SPK Per Customer By SPK Date";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Period: " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            lastRow++;
            ws.Cell("A" + lastRow).Value = "";
            lastRow++;

            GenerateExcel(wb, dt, lastRow, false, false);
            return ShowReport(wb, fileName, Device);
        }

        public ActionResult SummarySPKExhibitionByDate(DateTime DateFrom, DateTime DateTo, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_PmSummarySPKExhibitionBySpkDate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@DateTo", DateTo.ToString("yyyyMMdd"));

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "SummarySPKExhibitionBySpkDate_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("SumSPKExhibitionBySpkDate");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Summary SPK By Spk Date";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Period: " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            lastRow++;
            ws.Cell("A" + lastRow).Value = "";
            lastRow++;

            var tmpLastRow = lastRow;
            int lastCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                lastCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    if (tmpLastRow < lastRow)
                    {
                        if (dc.ColumnName != "DEALER")
                        {
                            val = Convert.ToInt32(dr[dc.ColumnName]);
                        }
                    }
                    Type typ = val.GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow + 1, lastCol).Style.DateFormat.Format = "dd-MMM";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \"_);_(@_)";
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0.##_);_(* (#,##0.##);_(* \" - \"_);_(@_)";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }

                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (tmpLastRow == lastRow)
                    {
                        ws.Cell(lastRow, lastCol).Value = dc.ColumnName.Length > 6 ? dc.ColumnName.Substring(0, 7) : dc.ColumnName;
                        ws.Cell(lastRow, lastCol).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, lastCol).Style.Font.SetBold().Font.SetFontSize(10);
                        if (lastCol > 2)
                        {
                            if (lastCol % 2 == 0)
                            {
                                var rng = ws.Range(5, lastCol - 1, 5, lastCol);
                                rng.Merge();
                            }
                        }

                        ws.Cell(lastRow + 1, lastCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, lastCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow + 1, lastCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, lastCol).Value = val;
                    }

                    lastCol++;
                }

                lastRow++;
            }
            lastRow++;

            // Set LastRow Total
            ws.Cell(lastRow, 1).SetValue("TOTAL").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).Font.SetBold();
            var rngTable = ws.Range(tmpLastRow + 2, 2, lastRow, lastCol - 1);
            var R0 = ((rngTable.RowCount() - 1) * -1);
            var C0 = 0;
            var R1 = -1;
            var C1 = 0;
            string formula = string.Format("SUM(R[{0}]C[{1}]:R[{2}]C[{3}])", R0, C0, R1, C1);
            rngTable.LastRow().FormulaR1C1 =formula;            
            rngTable.LastRow().DataType = XLCellValues.Number;
            rngTable.LastRow().Style.Font.SetBold();

            // Remark this for Actived Formula.
            //for (int i = 1; i <= rngTable.LastRow().CellCount(); i++)
            //{
            //    rngTable.LastRow().Cell(i).Value = rngTable.LastRow().Cell(i).Value;
            //}
            //

            // Set Custom Header & Border Table
            rngTable = ws.Range(tmpLastRow, 1, lastRow, lastCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Font.SetFontSize(10);

            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();

            rngTable = ws.Range(tmpLastRow + 1, 1, tmpLastRow + 1, lastCol - 1);
            rngTable.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(XLColor.Yellow)
                .Border.SetTopBorder(XLBorderStyleValues.None);

            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, 1);
            rngTable.Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            var xlColor = XLColor.FromArgb(153, 51, 102);
            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow, 1);
            rngTable.Style
                .Fill.SetBackgroundColor(xlColor)
                .Font.SetFontColor(XLColor.White);

            xlColor = XLColor.FromArgb(255, 102, 0);
            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow, 2);
            rngTable.Style
                .Fill.SetBackgroundColor(xlColor);

            xlColor = XLColor.FromArgb(255, 255, 103);
            rngTable = ws.Range(tmpLastRow, 3, tmpLastRow + 1, lastCol - 1);
            rngTable.Style
                .Fill.SetBackgroundColor(xlColor);

            xlColor = XLColor.FromArgb(255, 255, 0);
            rngTable = ws.Range(tmpLastRow + 2, 2, lastRow, 2);
            rngTable.Style
                .Fill.SetBackgroundColor(xlColor);
            //

            return ShowReport(wb, fileName, Device);
        }

        public ActionResult SummarySPKExhibitionByInquiryDate(DateTime DateFrom, DateTime DateTo, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_PmSummarySPKExhibitionByInquiryDate";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@DateTo", DateTo.ToString("yyyyMMdd"));

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "SummarySPKExhibitionByDate_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("SumSPKExhibitionByInqDate");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Summary SPK By Inquiry Date";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Period: " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            lastRow++;
            ws.Cell("A" + lastRow).Value = "";
            lastRow++;

            var tmpLastRow = lastRow;
            int lastCol = 1;
            foreach (DataRow dr in dt.Rows)
            {
                lastCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    if (tmpLastRow < lastRow)
                    {
                        if (dc.ColumnName != "DEALER")
                        {
                            val = Convert.ToInt32(dr[dc.ColumnName]);
                        }
                    }
                    Type typ = val.GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow + 1, lastCol).Style.DateFormat.Format = "dd-MMM";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \"_);_(@_)";
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0.##_);_(* (#,##0.##);_(* \" - \"_);_(@_)";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }

                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    if (tmpLastRow == lastRow)
                    {
                        ws.Cell(lastRow, lastCol).Value = dc.ColumnName.Length > 6 ? dc.ColumnName.Substring(0, 7) : dc.ColumnName;
                        ws.Cell(lastRow, lastCol).Style.Fill.SetBackgroundColor(XLColor.Yellow).Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(lastRow, lastCol).Style.Font.SetBold().Font.SetFontSize(10);
                        if (lastCol > 2)
                        {
                            if (lastCol % 2 == 0)
                            {
                                var rng = ws.Range(5, lastCol - 1, 5, lastCol);
                                rng.Merge();
                            }
                        }

                        ws.Cell(lastRow + 1, lastCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, lastCol).Value = val;
                    }
                    else
                    {
                        ws.Cell(lastRow + 1, lastCol).Style.Font.SetFontSize(10);
                        ws.Cell(lastRow + 1, lastCol).Value = val;
                    }

                    lastCol++;
                }

                lastRow++;
            }
            lastRow++;

            // Set LastRow Total
            ws.Cell(lastRow, 1).SetValue("TOTAL").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).Font.SetBold();
            var rngTable = ws.Range(tmpLastRow + 2, 2, lastRow, lastCol - 1);
            var R0 = ((rngTable.RowCount() - 1) * -1);
            var C0 = 0;
            var R1 = -1;
            var C1 = 0;
            string formula = string.Format("SUM(R[{0}]C[{1}]:R[{2}]C[{3}])", R0, C0, R1, C1);
            rngTable.LastRow().FormulaR1C1 = formula;
            rngTable.LastRow().DataType = XLCellValues.Number;
            rngTable.LastRow().Style.Font.SetBold();

            // Remark this for Actived Formula.
            //for (int i = 1; i <= rngTable.LastRow().CellCount(); i++)
            //{
            //    rngTable.LastRow().Cell(i).Value = rngTable.LastRow().Cell(i).Value;
            //}
            //

            // Set Custom Header & Border Table
            rngTable = ws.Range(tmpLastRow, 1, lastRow, lastCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Font.SetFontSize(10);

            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();

            rngTable = ws.Range(tmpLastRow + 1, 1, tmpLastRow + 1, lastCol - 1);
            rngTable.Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(XLColor.Yellow)
                .Border.SetTopBorder(XLBorderStyleValues.None);

            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, 1);
            rngTable.Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);

            var xlColor = XLColor.FromArgb(153, 51, 102);
            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow, 1);
            rngTable.Style
                .Fill.SetBackgroundColor(xlColor)
                .Font.SetFontColor(XLColor.White);

            xlColor = XLColor.FromArgb(255, 102, 0);
            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow, 2);
            rngTable.Style
                .Fill.SetBackgroundColor(xlColor);

            xlColor = XLColor.FromArgb(255, 255, 103);
            rngTable = ws.Range(tmpLastRow, 3, tmpLastRow + 1, lastCol - 1);
            rngTable.Style
                .Fill.SetBackgroundColor(xlColor);

            xlColor = XLColor.FromArgb(255, 255, 0);
            rngTable = ws.Range(tmpLastRow + 2, 2, lastRow, 2);
            rngTable.Style
                .Fill.SetBackgroundColor(xlColor);
            //

            return ShowReport(wb, fileName, Device);
        }
        
        public ActionResult DailySPKReportPerDealer(DateTime Date, string ShiftCode, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_PmDailySPKReportPerDealer";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Date", Date.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@ShiftCode", ShiftCode);

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "DailySPKReportPerDealer_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("DailySPKReportPerDealer");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Daily SPK Report Per Dealer";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Per " + Date.ToString("dd MMMM yyyy") + " Shift: " + ShiftCode;
            lastRow++;

            var tmpLastRow = lastRow + 1;
            int lastCol = 1;
            int firstCell = 1;
            int lastCell = 0;
            int rowGrouping = 5;
            string tmpHeaderGroupText = "";
            var xlColor = XLColor.FromArgb(209, 209, 105);
            int colorR = xlColor.Color.R;
            int colorG = xlColor.Color.G;
            int colorB = xlColor.Color.B;

            foreach (DataRow dr in dt.Rows)
            {
                lastCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    var headerGroupText = dc.ColumnName.Split(new string[] { "|" }, StringSplitOptions.None)[0];
                    if (tmpLastRow < lastRow)
                    {
                        if (dc.ColumnName != "DEALER")
                        {
                            val = string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])) ? Convert.ToInt32("0") : Convert.ToInt32(dr[dc.ColumnName]);
                        }
                    }

                    if (lastRow == rowGrouping)
                    {
                        if (lastCol > 2)
                        {
                            if (lastCol == 3)
                            {
                                tmpHeaderGroupText = headerGroupText;
                                firstCell = lastCol;
                                lastCell = lastCol - 1;
                            }
                            if (tmpHeaderGroupText != headerGroupText)
                            {
                                tmpHeaderGroupText = headerGroupText;
                                colorR = colorR < 220 ? colorR + 25 : 50;
                                colorG = colorG < 230 ? colorG + 10 : 70;
                                colorB = colorB < 210 ? colorB + 40 : 40;
                                xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                rg.Merge().Style
                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                    .Fill.SetBackgroundColor(xlColor);

                                rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                rg.Style
                                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                    .Fill.SetBackgroundColor(xlColor);

                                firstCell = lastCol;
                                lastCell++;
                            }
                            else
                            {
                                lastCell++;
                                if (lastCell == dt.Columns.Count)
                                {
                                    colorR = colorR < 220 ? colorR + 25 : 50;
                                    colorG = colorG < 230 ? colorG + 10 : 70;
                                    colorB = colorB < 210 ? colorB + 40 : 40;
                                    xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                    var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                    rg.Merge().Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                    rg.Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }
                            }
                        }
                    }

                    Type typ = val.GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow + 1, lastCol).Style.DateFormat.Format = "dd-MMM";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \"  \"_);_(@_)";
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0.##_);_(* (#,##0.##);_(* \"  \"_);_(@_)";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }

                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    ws.Cell(lastRow + 1, lastCol).Style.Font.SetFontSize(10);
                    ws.Cell(lastRow + 1, lastCol).Value = val;
                    lastCol++;
                }

                lastRow++;
            }
            lastRow++;


            // Set LastRow Total
            ws.Cell(lastRow, 1).SetValue("TOTAL").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).Font.SetBold();
            var rngTable = ws.Range(tmpLastRow + 2, 2, lastRow, lastCol - 1);
            var R0 = ((rngTable.RowCount() - 1) * -1);
            var C0 = 0;
            var R1 = -1;
            var C1 = 0;
            string formula = string.Format("SUM(R[{0}]C[{1}]:R[{2}]C[{3}])", R0, C0, R1, C1);
            rngTable.LastRow().FormulaR1C1 = formula;
            rngTable.LastRow().Style.Font.SetBold();
            rngTable.LastRow().Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \"_);_(@_)";
            // Remark this for Actived Formula.
            //for (int i = 1; i <= rngTable.LastRow().CellCount(); i++)
            //{
            //    rngTable.LastRow().Cell(i).Value = rngTable.LastRow().Cell(i).Value;
            //}
            //

            // Set Custom Header & Border Table
            rngTable = ws.Range(tmpLastRow, 1, lastRow, lastCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Font.SetFontSize(10);

            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();
            ws.Rows().Style.Alignment.SetWrapText();
            ws.Rows().AdjustToContents();

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Style
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Row(tmpLastRow + 1).Height = 38;

            xlColor = XLColor.FromArgb(153, 51, 102);
            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, 1);
            rngTable.Value = "DEALER";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(xlColor)
                .Font.SetFontColor(XLColor.White);

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Value = "TOTAL";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, lastCol - 1);
            rngTable.Style.Font.SetBold();
            //

            return ShowReport(wb, fileName, Device);
        }

        public ActionResult DailySPKReportPerDealerByInquiryDate(DateTime Date, string ShiftCode, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_PmDailySPKReportPerDealerByInquiryDateNew";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Date", Date.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@ShiftCode", ShiftCode);

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "DailySPKReportPerDealerByInquiryDate_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("DailySPKRptDealerByInqDate");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Daily SPK Report Per Dealer By Inquiry Date";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Per " + Date.ToString("dd MMMM yyyy") + " Shift: " + ShiftCode;
            lastRow++;

            var tmpLastRow = lastRow + 1;
            int lastCol = 1;
            int firstCell = 1;
            int lastCell = 0;
            int rowGrouping = 5;
            string tmpHeaderGroupText = "";
            var xlColor = XLColor.FromArgb(209, 209, 105);
            int colorR = xlColor.Color.R;
            int colorG = xlColor.Color.G;
            int colorB = xlColor.Color.B;

            foreach (DataRow dr in dt.Rows)
            {
                lastCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    var headerGroupText = dc.ColumnName.Split(new string[] { "|" }, StringSplitOptions.None)[0];
                    if (tmpLastRow < lastRow)
                    {
                        if (dc.ColumnName != "DEALER")
                        {
                            val = string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])) ? Convert.ToInt32("0") : Convert.ToInt32(dr[dc.ColumnName]);
                        }
                    }

                    if (lastRow == rowGrouping)
                    {
                        if (lastCol > 2)
                        {
                            if (lastCol == 3)
                            {
                                tmpHeaderGroupText = headerGroupText;
                                firstCell = lastCol;
                                lastCell = lastCol - 1;
                            }
                            if (tmpHeaderGroupText != headerGroupText)
                            {
                                var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                if (tmpHeaderGroupText.Length > 10 && tmpHeaderGroupText.Substring(0, 9).ToString() == "SUB TOTAL")
                                {
                                    tmpHeaderGroupText = headerGroupText;
                                    //xlColor = XLColor.FromArgb(128, 128, 128);
                                    rg = ws.Range(rowGrouping, firstCell, rowGrouping + 1, lastCell);
                                    rg.Merge()
                                        .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }
                                else
                                {
                                    tmpHeaderGroupText = headerGroupText;
                                    colorR = colorR < 220 ? colorR + 25 : 50;
                                    colorG = colorG < 230 ? colorG + 10 : 70;
                                    colorB = colorB < 210 ? colorB + 40 : 40;
                                    xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                    rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                    rg.Merge().Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                    rg.Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }

                                firstCell = lastCol;
                                lastCell++;

                                if (lastCell == dt.Columns.Count)
                                {
                                    if (tmpHeaderGroupText.Length > 10 && tmpHeaderGroupText.Substring(0, 9).ToString() == "SUB TOTAL")
                                    {
                                        //xlColor = XLColor.FromArgb(128, 128, 128);
                                        rg = ws.Range(rowGrouping, firstCell, rowGrouping + 1, lastCell);
                                        rg.Merge()
                                            .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                            .Fill.SetBackgroundColor(xlColor);
                                    }
                                }
                            }
                            else
                            {
                                lastCell++;
                                if (lastCell == dt.Columns.Count)
                                {
                                    colorR = colorR < 220 ? colorR + 25 : 50;
                                    colorG = colorG < 230 ? colorG + 10 : 70;
                                    colorB = colorB < 210 ? colorB + 40 : 40;
                                    xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                    var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                    rg.Merge().Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                    rg.Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }
                            }
                        }
                    }

                    Type typ = val.GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow + 1, lastCol).Style.DateFormat.Format = "dd-MMM";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \"  \"_);_(@_)";
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0.##_);_(* (#,##0.##);_(* \"  \"_);_(@_)";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }

                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    ws.Cell(lastRow + 1, lastCol).Style.Font.SetFontSize(10);
                    ws.Cell(lastRow + 1, lastCol).Value = val;
                    lastCol++;
                }

                lastRow++;
            }
            lastRow++;


            // Set LastRow Total
            ws.Cell(lastRow, 1).SetValue("TOTAL").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).Font.SetBold();
            var rngTable = ws.Range(tmpLastRow + 2, 2, lastRow, lastCol - 1);
            var R0 = ((rngTable.RowCount() - 1) * -1);
            var C0 = 0;
            var R1 = -1;
            var C1 = 0;
            string formula = string.Format("SUM(R[{0}]C[{1}]:R[{2}]C[{3}])", R0, C0, R1, C1);
            rngTable.LastRow().FormulaR1C1 = formula;
            rngTable.LastRow().Style.Font.SetBold();
            rngTable.LastRow().Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \"_);_(@_)";
            // Remark this for Actived Formula.
            //for (int i = 1; i <= rngTable.LastRow().CellCount(); i++)
            //{
            //    rngTable.LastRow().Cell(i).Value = rngTable.LastRow().Cell(i).Value;
            //}
            //

            // Set Custom Header & Border Table
            rngTable = ws.Range(tmpLastRow, 1, lastRow, lastCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Font.SetFontSize(10);

            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();
            ws.Rows().Style.Alignment.SetWrapText();
            ws.Rows().AdjustToContents();

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Style
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Row(tmpLastRow + 1).Height = 38;

            xlColor = XLColor.FromArgb(153, 51, 102);
            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, 1);
            rngTable.Value = "DEALER";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(xlColor)
                .Font.SetFontColor(XLColor.White);

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Value = "TOTAL";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, lastCol - 1);
            rngTable.Style.Font.SetBold();
            //

            return ShowReport(wb, fileName, Device);
        }

        public ActionResult DailySPKReportPerDealerBySPKDate(DateTime Date, string ShiftCode, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_PmDailySPKReportPerDealerBySPKDateNew";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@Date", Date.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@ShiftCode", ShiftCode);

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "DailySPKReportPerDealerBySPKDate_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("DailySPKRptDealerBySPKDate");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Daily SPK Report Per Dealer By SPK Date";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Per " + Date.ToString("dd MMMM yyyy") + " Shift: " + ShiftCode;
            lastRow++;

            var tmpLastRow = lastRow + 1;
            int lastCol = 1;
            int firstCell = 1;
            int lastCell = 0;
            int rowGrouping = 5;
            string tmpHeaderGroupText = "";
            var xlColor = XLColor.FromArgb(209, 209, 105);
            int colorR = xlColor.Color.R;
            int colorG = xlColor.Color.G;
            int colorB = xlColor.Color.B;

            foreach (DataRow dr in dt.Rows)
            {
                lastCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    var headerGroupText = dc.ColumnName.Split(new string[] { "|" }, StringSplitOptions.None)[0];
                    if (tmpLastRow < lastRow)
                    {
                        if (dc.ColumnName != "DEALER")
                        {
                            val = string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])) ? Convert.ToInt32("0") : Convert.ToInt32(dr[dc.ColumnName]);
                        }
                    }

                    if (lastRow == rowGrouping)
                    {
                        if (lastCol > 2)
                        {
                            if (lastCol == 3)
                            {
                                tmpHeaderGroupText = headerGroupText;
                                firstCell = lastCol;
                                lastCell = lastCol - 1;
                            }
                            if (tmpHeaderGroupText != headerGroupText)
                            {
                                var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                if (tmpHeaderGroupText.Length > 10 && tmpHeaderGroupText.Substring(0, 9).ToString() == "SUB TOTAL")
                                {
                                    tmpHeaderGroupText = headerGroupText;
                                    //xlColor = XLColor.FromArgb(128, 128, 128);
                                    rg = ws.Range(rowGrouping, firstCell, rowGrouping + 1, lastCell);
                                    rg.Merge()
                                        .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }
                                else
                                {
                                    tmpHeaderGroupText = headerGroupText;
                                    colorR = colorR < 220 ? colorR + 25 : 50;
                                    colorG = colorG < 230 ? colorG + 10 : 70;
                                    colorB = colorB < 210 ? colorB + 40 : 40;
                                    xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                    rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                    rg.Merge().Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                    rg.Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }

                                firstCell = lastCol;
                                lastCell++;

                                if (lastCell == dt.Columns.Count)
                                {
                                    if (tmpHeaderGroupText.Length > 10 && tmpHeaderGroupText.Substring(0, 9).ToString() == "SUB TOTAL")
                                    {
                                        //xlColor = XLColor.FromArgb(128, 128, 128);
                                        rg = ws.Range(rowGrouping, firstCell, rowGrouping + 1, lastCell);
                                        rg.Merge()
                                            .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                            .Fill.SetBackgroundColor(xlColor);
                                    }
                                }
                            }
                            else
                            {
                                lastCell++;
                                if (lastCell == dt.Columns.Count)
                                {
                                    colorR = colorR < 220 ? colorR + 25 : 50;
                                    colorG = colorG < 230 ? colorG + 10 : 70;
                                    colorB = colorB < 210 ? colorB + 40 : 40;
                                    xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                    var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                    rg.Merge().Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                    rg.Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }
                            }
                        }
                    }

                    Type typ = val.GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            ws.Cell(lastRow + 1, lastCol).Style.DateFormat.Format = "dd-MMM";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \"  \"_);_(@_)";
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0.##_);_(* (#,##0.##);_(* \"  \"_);_(@_)";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }

                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    ws.Cell(lastRow + 1, lastCol).Style.Font.SetFontSize(10);
                    ws.Cell(lastRow + 1, lastCol).Value = val;
                    lastCol++;
                }

                lastRow++;
            }
            lastRow++;


            // Set LastRow Total
            ws.Cell(lastRow, 1).SetValue("TOTAL").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).Font.SetBold();
            var rngTable = ws.Range(tmpLastRow + 2, 2, lastRow, lastCol - 1);
            var R0 = ((rngTable.RowCount() - 1) * -1);
            var C0 = 0;
            var R1 = -1;
            var C1 = 0;
            string formula = string.Format("SUM(R[{0}]C[{1}]:R[{2}]C[{3}])", R0, C0, R1, C1);
            rngTable.LastRow().FormulaR1C1 = formula;
            rngTable.LastRow().Style.Font.SetBold();
            rngTable.LastRow().Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \"_);_(@_)";
            // Remark this for Actived Formula.
            //for (int i = 1; i <= rngTable.LastRow().CellCount(); i++)
            //{
            //    rngTable.LastRow().Cell(i).Value = rngTable.LastRow().Cell(i).Value;
            //}
            //

            // Set Custom Header & Border Table
            rngTable = ws.Range(tmpLastRow, 1, lastRow, lastCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Font.SetFontSize(10);

            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();
            ws.Rows().Style.Alignment.SetWrapText();
            ws.Rows().AdjustToContents();

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Style
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Row(tmpLastRow + 1).Height = 38;

            xlColor = XLColor.FromArgb(153, 51, 102);
            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, 1);
            rngTable.Value = "DEALER";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(xlColor)
                .Font.SetFontColor(XLColor.White);

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Value = "TOTAL";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, lastCol - 1);
            rngTable.Style.Font.SetBold();
            //

            return ShowReport(wb, fileName, Device);
        }

        public ActionResult SummarySPKReportBySpkDatePerShift(DateTime DateFrom, DateTime DateTo, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_PmSummarySPKReportBySpkDatePerShiftNew";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@DateTo", DateTo.ToString("yyyyMMdd"));

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "SummarySPKReportBySpkDatePerShift_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("SumSPKReportBySpkDatePerShift");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Summary SPK By Spk Date Per Shift";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Period: " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            lastRow++;
            ws.Cell("A" + lastRow).Value = "";
            lastRow++;

            var tmpLastRow = lastRow + 1;
            int lastCol = 1;
            int firstCell = 1;
            int lastCell = 0;
            int rowGrouping = 6;
            string tmpHeaderGroupText = "";
            var xlColor = XLColor.FromArgb(209, 209, 105);
            int colorR = xlColor.Color.R;
            int colorG = xlColor.Color.G;
            int colorB = xlColor.Color.B;

            var dicGroupType = new Dictionary<int, int>();
            foreach (DataRow dr in dt.Rows)
            {
                lastCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    var headerGroupText = dc.ColumnName.Split(new string[] { "|" }, StringSplitOptions.None)[0];
                    if (tmpLastRow < lastRow)
                    {
                        if (dc.ColumnName != "TANGGAL")
                        {
                            if (dc.ColumnName != "SHIFT")
                            {
                                val = string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])) ? Convert.ToInt32("0") : Convert.ToInt32(dr[dc.ColumnName]);
                            }
                        }
                    }

                    if (lastRow == rowGrouping)
                    {
                        if (lastCol > 2 + 1)
                        {
                            if (lastCol == (3 + 1))
                            {
                                tmpHeaderGroupText = headerGroupText;
                                firstCell = lastCol;
                                lastCell = lastCol - 1;
                            }
                            if (tmpHeaderGroupText != headerGroupText)
                            {
                                var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                if (tmpHeaderGroupText.Length > 10 && tmpHeaderGroupText.Substring(0, 9).ToString() == "SUB TOTAL")
                                {
                                    tmpHeaderGroupText = headerGroupText;
                                    //xlColor = XLColor.FromArgb(128, 128, 128);
                                    rg = ws.Range(rowGrouping, firstCell, rowGrouping + 1, lastCell);
                                    rg.Merge()
                                        .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }
                                else
                                {
                                    tmpHeaderGroupText = headerGroupText;
                                    colorR = colorR < 220 ? colorR + 25 : 50;
                                    colorG = colorG < 230 ? colorG + 10 : 70;
                                    colorB = colorB < 210 ? colorB + 40 : 40;
                                    xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                    rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                    rg.Merge().Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                    rg.Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }

                                firstCell = lastCol;
                                lastCell++;

                                if (lastCell == dt.Columns.Count)
                                {
                                    if (tmpHeaderGroupText.Length > 10 && tmpHeaderGroupText.Substring(0, 9).ToString() == "SUB TOTAL")
                                    {
                                        //xlColor = XLColor.FromArgb(128, 128, 128);
                                        rg = ws.Range(rowGrouping, firstCell, rowGrouping + 1, lastCell);
                                        rg.Merge()
                                            .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                            .Fill.SetBackgroundColor(xlColor);
                                    }
                                }
                            }
                            else
                            {
                                lastCell++;
                                var colCount = dt.Columns.Count;
                                if (lastCell == colCount)
                                {
                                    colorR = colorR < 220 ? colorR + 25 : 50;
                                    colorG = colorG < 230 ? colorG + 10 : 70;
                                    colorB = colorB < 210 ? colorB + 40 : 40;
                                    xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                    var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                    rg.Merge().Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                    rg.Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    dicGroupType.Add(lastCell + 1, lastCell + 1);
                                }
                            }
                        }
                    }

                    Type typ = val.GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            val = Convert.ToDateTime(val).ToString("dd MMM yyyy", ci).Insert(0, "'");    
                        //ws.Cell(lastRow + 1, lastCol).Style.DateFormat.Format = "dd MMMM yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \"_);_(@_)";
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0.##_);_(* (#,##0.##);_(* \" - \"_);_(@_)";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }

                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    ws.Cell(lastRow + 1, lastCol).Style.Font.SetFontSize(10);
                    ws.Cell(lastRow + 1, lastCol).Value = val;

                    lastCol++;
                }

                lastRow++;
                if (lastRow > 7)
                {
                    if (lastRow % 2 == 1)
                    {
                        var rng = ws.Range(lastRow - 1, 1, lastRow, 1);
                        rng.Merge();
                    }
                }
            }
            lastRow++;

            // Set LastRow Total
            ws.Cell(lastRow, 1).SetValue("TOTAL").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).Font.SetBold();
            var rngTable = ws.Range(tmpLastRow + 2, 2 + 1, lastRow, lastCol - 1);
            var R0 = ((rngTable.RowCount() - 1) * -1);
            var C0 = 0;
            var R1 = -1;
            var C1 = 0;
            string formula = string.Format("SUM(R[{0}]C[{1}]:R[{2}]C[{3}])", R0, C0, R1, C1);
            rngTable.LastRow().FormulaR1C1 = formula;
            rngTable.LastRow().Style.Font.SetBold();

            // Remark this for Actived Formula.
            //for (int i = 1; i <= rngTable.LastRow().CellCount(); i++)
            //{
            //    rngTable.LastRow().Cell(i).Value = rngTable.LastRow().Cell(i).Value;
            //}
            //

            // Set Custom Header & Border Table
            rngTable = ws.Range(tmpLastRow, 1, lastRow, lastCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Font.SetFontSize(10);
            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();
            ws.Rows().Style.Alignment.SetWrapText();
            ws.Rows().AdjustToContents();
            ws.Row(tmpLastRow + 1).Height = 38;

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Style
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            xlColor = XLColor.FromArgb(153, 51, 102);
            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, 1);
            rngTable.Value = "TANGGAL";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(xlColor)
                .Font.SetFontColor(XLColor.White);

            rngTable = ws.Range(tmpLastRow + 2, 1, lastRow - 1, 1);
            rngTable.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Value = "SHIFT";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(xlColor)
                .Font.SetFontColor(XLColor.White);

            rngTable = ws.Range(tmpLastRow + 2, 2, lastRow - 1, 2);
            rngTable.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 7;

            rngTable = ws.Range(tmpLastRow, 3, tmpLastRow + 1, 3);
            rngTable.Merge().Value = "TOTAL";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            xlColor = XLColor.FromArgb(255, 255, 0);
            rngTable = ws.Range(tmpLastRow + 2, 3, lastRow, 3);
            rngTable.Style.Fill.SetBackgroundColor(xlColor);

            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, lastCol - 1);
            rngTable.Style.Font.SetBold();
            //

            ws.Rows(5, 5).Delete();

            if (Device == "desktop")
            {
                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
            else
            {
                Stream ms = new MemoryStream();
                wb.PageOptions.SetPageOrientation(XLPageOrientation.Landscape);
                wb.SaveAs(ms);
                ms.Position = 0;

                SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                Stream ms2 = new MemoryStream();
                ExcelFile.Load(ms, LoadOptions.XlsxDefault).
                    Save(ms2, SaveOptions.HtmlDefault);

                ms2.Position = 0;
                ms.Close();
                ms.Dispose();
                ms = null;
                FileStreamResult fsr = new FileStreamResult(ms2, "text/html");
                
                return fsr; 
                //File(ms, "application/pdf", fileName + ".pdf");
            }
        }

        public ActionResult SummarySPKReportByInquiryDatePerShift(DateTime DateFrom, DateTime DateTo, string Device)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_PmSummarySPKReportByInquiryDatePerShiftNew";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@DateFrom", DateFrom.ToString("yyyyMMdd"));
            cmd.Parameters.AddWithValue("@DateTo", DateTo.ToString("yyyyMMdd"));

            DataTable dt = new DataTable("datTable1");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);

            DateTime now = DateTime.Now;
            string fileName = "SummarySPKReportByInqDatePerShift_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");
            int lastRow = 1;
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("SumSPKReportByInqDatePerShift");

            ws.Columns("1").Width = 30;
            ws.Cell("A" + lastRow).Style.Font.SetFontSize(7);
            ws.Cell("A" + lastRow).Value = "Printed on Date: " + now.ToString("dd-MM-yyyy") + " / Time: " + now.ToShortTimeString();
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(10);
            ws.Cell("A" + lastRow).Value = "Summary SPK By Inquiry Date Per Shift";
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            lastRow++;

            ws.Cell("A" + lastRow).Style.Font.SetBold().Font.SetFontSize(8);
            ws.Range("A" + lastRow, "D" + lastRow).Merge();
            ws.Cell("A" + lastRow).Value = "Period: " + DateFrom.ToString("dd-MMM-yyyy") + " s/d " + DateTo.ToString("dd-MMM-yyyy");
            lastRow++;
            ws.Cell("A" + lastRow).Value = "";
            lastRow++;

            var tmpLastRow = lastRow + 1;
            int lastCol = 1;
            int firstCell = 1;
            int lastCell = 0;
            int rowGrouping = 6;
            string tmpHeaderGroupText = "";
            var xlColor = XLColor.FromArgb(209, 209, 105);
            int colorR = xlColor.Color.R;
            int colorG = xlColor.Color.G;
            int colorB = xlColor.Color.B;

            foreach (DataRow dr in dt.Rows)
            {
                lastCol = 1;
                foreach (DataColumn dc in dt.Columns)
                {
                    var val = dr[dc.ColumnName];
                    var headerGroupText = dc.ColumnName.Split(new string[] { "|" }, StringSplitOptions.None)[0];
                    if (tmpLastRow < lastRow)
                    {
                        if (dc.ColumnName != "TANGGAL")
                        {
                            if (dc.ColumnName != "SHIFT")
                            {
                                val = string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])) ? Convert.ToInt32("0") : Convert.ToInt32(dr[dc.ColumnName]);
                            }
                        }
                    }

                    if (lastRow == rowGrouping)
                    {
                        if (lastCol > 2 + 1)
                        {
                            if (lastCol == (3 + 1))
                            {
                                tmpHeaderGroupText = headerGroupText;
                                firstCell = lastCol;
                                lastCell = lastCol - 1;
                            }
                            if (tmpHeaderGroupText != headerGroupText)
                            {
                                var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                if (tmpHeaderGroupText.Length > 10 && tmpHeaderGroupText.Substring(0, 9).ToString() == "SUB TOTAL")
                                {
                                    tmpHeaderGroupText = headerGroupText;
                                    //xlColor = XLColor.FromArgb(128, 128, 128);
                                    rg = ws.Range(rowGrouping, firstCell, rowGrouping + 1, lastCell);
                                    rg.Merge()
                                        .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }
                                else
                                {
                                    tmpHeaderGroupText = headerGroupText;
                                    colorR = colorR < 220 ? colorR + 25 : 50;
                                    colorG = colorG < 230 ? colorG + 10 : 70;
                                    colorB = colorB < 210 ? colorB + 40 : 40;
                                    xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                    rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                    rg.Merge().Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                    rg.Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }

                                firstCell = lastCol;
                                lastCell++;

                                if (lastCell == dt.Columns.Count)
                                {
                                    if (tmpHeaderGroupText.Length > 10 && tmpHeaderGroupText.Substring(0, 9).ToString() == "SUB TOTAL")
                                    {
                                        //xlColor = XLColor.FromArgb(128, 128, 128);
                                        rg = ws.Range(rowGrouping, firstCell, rowGrouping + 1, lastCell);
                                        rg.Merge()
                                            .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                            .Fill.SetBackgroundColor(xlColor);
                                    }
                                }
                            }
                            else
                            {
                                lastCell++;
                                var rg = ws.Range(rowGrouping, firstCell, rowGrouping, lastCell);
                                if (lastCell == dt.Columns.Count)
                                {
                                    colorR = colorR < 220 ? colorR + 25 : 50;
                                    colorG = colorG < 230 ? colorG + 10 : 70;
                                    colorB = colorB < 210 ? colorB + 40 : 40;
                                    xlColor = XLColor.FromArgb(colorR, colorG, colorB);
                                    rg.Merge().Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);

                                    rg = ws.Range(rowGrouping + 1, firstCell, rowGrouping + 1, lastCell);
                                    rg.Style
                                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                                        .Fill.SetBackgroundColor(xlColor);
                                }
                            }
                        }
                    }

                    Type typ = val.GetType();
                    switch (Type.GetTypeCode(typ))
                    {
                        case TypeCode.DateTime:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            val = Convert.ToDateTime(val).ToString("dd MMM yyyy", ci).Insert(0, "'");
                            //ws.Cell(lastRow + 1, lastCol).Style.DateFormat.Format = "dd MMMM yyyy";
                            break;
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0_);_(* (#,##0);_(* \" - \"_);_(@_)";
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            break;
                        case TypeCode.Decimal:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                            ws.Cell(lastRow + 1, lastCol).Style.NumberFormat.Format = "_(* #,##0.##_);_(* (#,##0.##);_(* \" - \"_);_(@_)";
                            break;
                        case TypeCode.Boolean:
                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            break;
                        default:
                            if (string.IsNullOrWhiteSpace(val.ToString()) == false)
                            {
                                val = val.ToString().Substring(0, 1) == "0" ? val.ToString().Insert(0, "'") : val;
                            }

                            ws.Cell(lastRow + 1, lastCol).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                            break;
                    };

                    ws.Cell(lastRow + 1, lastCol).Style.Font.SetFontSize(10);
                    ws.Cell(lastRow + 1, lastCol).Value = val;

                    lastCol++;
                }

                lastRow++;
                if (lastRow > 7)
                {
                    if (lastRow % 2 == 1)
                    {
                        var rng = ws.Range(lastRow - 1, 1, lastRow, 1);
                        rng.Merge();
                    }
                }
            }
            lastRow++;

            // Set LastRow Total
            ws.Cell(lastRow, 1).SetValue("TOTAL").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right).Font.SetBold();
            var rngTable = ws.Range(tmpLastRow + 2, 2 + 1, lastRow, lastCol - 1);
            var R0 = ((rngTable.RowCount() - 1) * -1);
            var C0 = 0;
            var R1 = -1;
            var C1 = 0;
            string formula = string.Format("SUM(R[{0}]C[{1}]:R[{2}]C[{3}])", R0, C0, R1, C1);
            rngTable.LastRow().FormulaR1C1 = formula;
            rngTable.LastRow().Style.Font.SetBold();

            // Remark this for Actived Formula.
            //for (int i = 1; i <= rngTable.LastRow().CellCount(); i++)
            //{
            //    rngTable.LastRow().Cell(i).Value = rngTable.LastRow().Cell(i).Value;
            //}
            //

            // Set Custom Header & Border Table
            rngTable = ws.Range(tmpLastRow, 1, lastRow, lastCol - 1);
            rngTable.Style
                .Border.SetTopBorder(XLBorderStyleValues.Thin)
                .Border.SetBottomBorder(XLBorderStyleValues.Thin)
                .Border.SetLeftBorder(XLBorderStyleValues.Thin)
                .Border.SetRightBorder(XLBorderStyleValues.Thin)
                .Font.SetFontSize(10);
            ws.Columns().Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
            ws.Columns().Style.Alignment.SetWrapText();
            ws.Columns().AdjustToContents();
            ws.Rows().Style.Alignment.SetWrapText();
            ws.Rows().AdjustToContents();
            ws.Row(tmpLastRow + 1).Height = 38;

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Style
                .Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            xlColor = XLColor.FromArgb(153, 51, 102);
            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, 1);
            rngTable.Value = "TANGGAL";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(xlColor)
                .Font.SetFontColor(XLColor.White);

            rngTable = ws.Range(tmpLastRow + 2, 1, lastRow - 1, 1);
            rngTable.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable = ws.Range(tmpLastRow, 2, tmpLastRow + 1, 2);
            rngTable.Merge().Value = "SHIFT";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Fill.SetBackgroundColor(xlColor)
                .Font.SetFontColor(XLColor.White);

            rngTable = ws.Range(tmpLastRow + 2, 2, lastRow - 1, 2);
            rngTable.Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            ws.Column(1).Width = 20;
            ws.Column(2).Width = 7;

            rngTable = ws.Range(tmpLastRow, 3, tmpLastRow + 1, 3);
            rngTable.Merge().Value = "TOTAL";
            rngTable.Merge()
                .Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            xlColor = XLColor.FromArgb(255, 255, 0);
            rngTable = ws.Range(tmpLastRow + 2, 3, lastRow, 3);
            rngTable.Style.Fill.SetBackgroundColor(xlColor);

            rngTable = ws.Range(tmpLastRow, 1, tmpLastRow + 1, lastCol - 1);
            rngTable.Style.Font.SetBold();
            //

            ws.Rows(5, 5).Delete();

            if (Device == "desktop")
            {
                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
            else
            {
                Stream ms = new MemoryStream();
                wb.PageOptions.SetPageOrientation(XLPageOrientation.Landscape);
                wb.SaveAs(ms);
                ms.Position = 0;

                SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
                Stream ms2 = new MemoryStream();
                ExcelFile.Load(ms, LoadOptions.XlsxDefault).
                    Save(ms2, SaveOptions.HtmlDefault);

                ms2.Position = 0;
                ms.Close();
                ms.Dispose();
                ms = null;
                FileStreamResult fsr = new FileStreamResult(ms2, "text/html");

                return fsr;
                //File(ms, "application/pdf", fileName + ".pdf");
            }
        }

        #endregion

        #region "Eksekutif Summary"

        public JsonResult Summary()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_SpkeSummary";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@DateFrom", Request["DateFrom"]);
            cmd.Parameters.AddWithValue("@DateTo", Request["DateTo"]);
            cmd.Parameters.AddWithValue("@InqType", Request["SummaryType"]);
            
            try
            {
                DataSet ds = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                return Json(new { success = true, row = GetJsonRow(ds.Tables[0]), data = GetJson(ds.Tables[1]) }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Generate Insert pmKdp Script
        public void GenerateInsPmKdp(PmKDPExhibition model, PmKdpAdditionalExhibition model2, PmStatusHistoryExhibition model3, bool bolNew)
        {
            string sqls = "declare @inqNo int, @branchCode varchar(15), @leader varchar(32) " + System.Environment.NewLine +
                "set @branchCode = (select top 1 BranchCode from HrEmployeeMutation where CompanyCode = '" + model.CompanyCode + "' and EmployeeID = '" + model.EmployeeID + "' order by MutationDate desc)" + System.Environment.NewLine;

            sqls += "select @leader=(select top 1 TeamLeader from HrEmployee where EmployeeId = '" + model.EmployeeID + "') " + System.Environment.NewLine;
            
            if (bolNew)
            {
                sqls += "begin tran " + System.Environment.NewLine +
                    "set @inqNo = (select isnull(max(InquiryNumber), 0) + 1 from pmKDP with (tablockx))" + System.Environment.NewLine +

                    "INSERT INTO [dbo].[pmKDP] ([InquiryNumber], [CompanyCode],  [BranchCode],[EmployeeID], [SpvEmployeeID], [InquiryDate], [OutletID], [StatusProspek], [PerolehanData], [NamaProspek], [AlamatProspek], [TelpRumah], [CityID], [NamaPerusahaan], [AlamatPerusahaan], [Jabatan], [Handphone], [Faximile], [Email], [TipeKendaraan], [Variant], [Transmisi], [ColourCode], [CaraPembayaran], [TestDrive], [QuantityInquiry], [LastProgress], [LastUpdateStatus], [SPKDate], [LostCaseDate], [LostCaseCategory], [LostCaseReasonID], [LostCaseOtherReason], [LostCaseVoiceOfCustomer], [CreationDate], [CreatedBy], [LastUpdateBy], [LastUpdateDate], [Leasing], [DownPayment], [Tenor], [MerkLain])" + System.Environment.NewLine +
                    "VALUES (@inqNo,'" + model.CompanyCode +
                        "',@branchCode,'" + model.EmployeeID +
                        "',@leader,'" + model.InquiryDate +
                        "','" + model.OutletID + "','" + model.StatusProspek +
                        "','" + model.PerolehanData + "','" + model.NamaProspek +
                        "','" + model.AlamatProspek + "','" + model.TelpRumah +
                        "','" + model.CityID + "','" + model.NamaPerusahaan +
                        "','" + model.AlamatPerusahaan + "','" + model.Jabatan +
                        "','" + model.Handphone + "','" + model.Faximile +
                        "','" + model.Email + "','" + model.TipeKendaraan +
                        "','" + model.Variant + "','" + model.Transmisi +
                        "','" + model.ColourCode + "','" + model.CaraPembayaran +
                        "','" + model.TestDrive + "','" + model.QuantityInquiry +
                        "','" + model.LastProgress + "','" + model.LastUpdateStatus +
                        "','" + model.SPKDate + "','" + model.LostCaseDate +
                        "','" + model.LostCaseCategory + "','" + model.LostCaseReasonID +
                        "','" + model.LostCaseOtherReason + "','" + model.LostCaseVoiceOfCustomer +
                        "','" + model.CreationDate + "','" + GetUserEx(model.CreatedBy)  +
                        "','" + GetUserEx(model.LastUpdateBy) + "','" + model.LastUpdateDate +
                        "','" + model.Leasing + "','" + model.DownPayment +
                        "','" + model.Tenor + "','" + model.MerkLain +
                        "')" + System.Environment.NewLine +

                    "commit tran" + System.Environment.NewLine +

                    "INSERT INTO [dbo].[pmKdpAdditionalExhibition] ([CompanyCode], [BranchCode], [InquiryNumber], [StatusVehicle], [OthersBrand], [OthersType], [CreatedBy], [CreatedDate], [LastUpdateBy], [LastUpdateDate], [SPKNo], [GiftRefferenceCode], [GiftRefferenceValue], [GiftRefferenceDesc], [ShiftCode])" + System.Environment.NewLine +
                    "VALUES ('" +
                        model2.CompanyCode + "'," +
                        "@branchCode" + "," +
                        "@inqNo" + ",'" +
                        model2.StatusVehicle + "','" +
                        model2.OthersBrand + "','" +
                        model2.OthersType + "','" +
                       GetUserEx(  model2.CreatedBy)  + "','" +
                        model2.CreatedDate + "','" +
                       GetUserEx(  model2.LastUpdateBy)  + "','" +
                        model2.LastUpdateDate + "','" +
                        model2.SPKNo + "','" +
                        model2.GiftRefferenceCode + "','" +
                        model2.GiftRefferenceValue + "','" +
                        model2.GiftRefferenceDesc + "','" +
                        model2.ShiftCode +
                        "')" + System.Environment.NewLine +
                    "INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])" + System.Environment.NewLine +
                    "VALUES (@inqNo,'" +
                        model3.CompanyCode + "'," +
                        "@branchCode" + ",'" +
                        "1" + "','" +
                        "P" + "','" +
                        model3.UpdateDate + "','" +
                        GetUserEx(  model3.UpdateUser)  +
                        "')" + System.Environment.NewLine +
                    "INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])" + System.Environment.NewLine +
                    "VALUES (@inqNo,'" +
                        model3.CompanyCode + "'," +
                        "@branchCode" + ",'" +
                        "2" + "','" +
                        "HP" + "','" +
                        model3.UpdateDate + "','" +
                        GetUserEx( model3.UpdateUser)  +
                        "')" + System.Environment.NewLine +
                    "INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])" + System.Environment.NewLine +
                    "VALUES (@inqNo,'" +
                        model3.CompanyCode + "'," +
                        "@branchCode" + ",'" +
                        "3" + "','" +
                        "SPK" + "','" +
                        model3.UpdateDate + "','" +
                       GetUserEx( model3.UpdateUser)  +
                        "')" + System.Environment.NewLine;
            }
            else
            {
                sqls += "UPDATE [dbo].[pmKDP]" + System.Environment.NewLine +
                "SET " +
                    "EmployeeID = '" + model.EmployeeID +
                    "', SpvEmployeeID = @leader, InquiryDate = '" + model.InquiryDate +
                    "', OutletID = '" + model.OutletID +
                    "', StatusProspek = '" + model.StatusProspek +
                    "', PerolehanData = '" + model.PerolehanData +
                    "', NamaProspek = '" + model.NamaProspek +
                    "', AlamatProspek = '" + model.AlamatProspek +
                    "', TelpRumah = '" + model.TelpRumah +
                    "', CityID = '" + model.CityID +
                    "', NamaPerusahaan = '" + model.NamaPerusahaan +
                    "', AlamatPerusahaan = '" + model.AlamatPerusahaan +
                    "', Jabatan = '" + model.Jabatan +
                    "', Handphone = '" + model.Handphone +
                    "', Faximile = '" + model.Faximile +
                    "', Email = '" + model.Email +
                    "', TipeKendaraan = '" + model.TipeKendaraan +
                    "', Variant = '" + model.Variant +
                    "', Transmisi = '" + model.Transmisi +
                    "', ColourCode = '" + model.ColourCode +
                    "', CaraPembayaran = '" + model.CaraPembayaran +
                    "', TestDrive = '" + model.TestDrive +
                    "', QuantityInquiry = '" + model.QuantityInquiry +
                    "', LastProgress = '" + model.LastProgress +
                    "', LastUpdateStatus = '" + model.LastUpdateStatus +
                    "', SPKDate = '" + model.SPKDate +
                    "', LostCaseDate = '" + model.LostCaseDate +
                    "', LostCaseCategory = '" + model.LostCaseCategory +
                    "', LostCaseReasonID = '" + model.LostCaseReasonID +
                    "', LostCaseOtherReason = '" + model.LostCaseOtherReason +
                    "', LostCaseVoiceOfCustomer = '" + model.LostCaseVoiceOfCustomer +
                    "', LastUpdateBy = '" + GetUserEx(  model.LastUpdateBy)  +
                    "', LastUpdateDate = '" + model.LastUpdateDate +
                    "', Leasing = '" + model.Leasing +
                    "', DownPayment = '" + model.DownPayment +
                    "', Tenor = '" + model.Tenor +
                    "', MerkLain = '" + model.MerkLain +
                    "'" + System.Environment.NewLine +
                "WHERE InquiryNumber = " + model.InquiryNumber + System.Environment.NewLine +
                    "AND CompanyCode = '" + model3.CompanyCode +
                    "' AND  BranchCode = @branchCode" + System.Environment.NewLine +
                
                //"GO" + System.Environment.NewLine +
                "UPDATE [dbo].[pmKdpAdditionalExhibition]" + System.Environment.NewLine +
                "SET " +
                    "StatusVehicle = '" + model2.StatusVehicle +
                    "', OthersBrand = '" + model2.OthersBrand +
                    "', OthersType = '" + model2.OthersType +
                    "', LastUpdateBy = '" + GetUserEx(  model2.LastUpdateBy)  +
                    "', LastUpdateDate = '" + model2.LastUpdateDate +
                    "', SPKNo = '" + model2.SPKNo +
                    "', GiftRefferenceCode = '" + model2.GiftRefferenceCode +
                    "', GiftRefferenceValue = '" + model2.GiftRefferenceValue +
                    "', GiftRefferenceDesc = '" + model2.GiftRefferenceDesc +
                    "', ShiftCode = '" + model2.ShiftCode +
                    "'" + System.Environment.NewLine +
                "WHERE InquiryNumber = " + model.InquiryNumber + System.Environment.NewLine +
                    "AND CompanyCode = '" + model3.CompanyCode +
                    "' AND  BranchCode = @branchCode" + System.Environment.NewLine +

                //"GO" + System.Environment.NewLine +
                "UPDATE [dbo].[pmStatusHistory]" + System.Environment.NewLine +
                "SET " +
                    "LastProgress = 'P" +
                    "', UpdateDate = '" + model3.UpdateDate +
                    "', UpdateUser = '" + GetUserEx( model3.UpdateUser)  +
                    "'" + System.Environment.NewLine +
                "WHERE InquiryNumber = " + model.InquiryNumber + System.Environment.NewLine +
                    "AND CompanyCode = '" + model3.CompanyCode +
                    "' AND  BranchCode = @branchCode" +
                    " AND  SequenceNo = '1'" + System.Environment.NewLine +

                //"GO" + System.Environment.NewLine +
                "UPDATE [dbo].[pmStatusHistory]" + System.Environment.NewLine +
                "SET " +
                    "LastProgress = 'HP" +
                    "', UpdateDate = '" + model3.UpdateDate +
                    "', UpdateUser = '" + GetUserEx( model3.UpdateUser)  +
                    "'" + System.Environment.NewLine +
                "WHERE InquiryNumber = " + model.InquiryNumber + System.Environment.NewLine +
                    "AND CompanyCode = '" + model3.CompanyCode +
                    "' AND  BranchCode = @branchCode" +
                    " AND  SequenceNo = '2'" + System.Environment.NewLine +

                //"GO" + System.Environment.NewLine +
                "UPDATE [dbo].[pmStatusHistory]" + System.Environment.NewLine +
                "SET " +
                    "LastProgress = 'SPK" +
                    "', UpdateDate = '" + model3.UpdateDate +
                    "', UpdateUser = '" + GetUserEx( model3.UpdateUser)  +
                    "'" + System.Environment.NewLine +
                "WHERE InquiryNumber = " + model.InquiryNumber + System.Environment.NewLine +
                    "AND CompanyCode = '" + model3.CompanyCode +
                    "' AND  BranchCode = @branchCode" +
                    " AND  SequenceNo = '3'";
                //"set @inqNo = @inqNo + 1" + System.Environment.NewLine
            }
            GenerateSQL(new SysSQLGateway() { TaskNo = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + model.InquiryNumber, TaskName = model.CompanyCode + "_" + model.SPKNo, SQL = sqls, DealerCode = model.CompanyCode });
        }

        public void GenerateInsPmKdpv2(PmKDPExhibition model, PmKdpAdditionalExhibition model2, PmStatusHistoryExhibition model3)
        {
            FileStream fs1 = GetOrCreateFile();
            StreamWriter writer = new StreamWriter(fs1);
            StreamReader reader = new StreamReader(fs1);

            reader.ReadToEnd();
            long size = fs1.Length;
            if (size < 1)
            {
                writer.WriteLine("declare @inqNo int");
                writer.WriteLine("set @inqNo = (select isnull(max(InquiryNumber), 0) + 1 from pmKDP)");
                writer.WriteLine();
            }
            writer.WriteLine("INSERT INTO [dbo].[pmKDP] ([InquiryNumber], [BranchCode], [CompanyCode], [EmployeeID], [SpvEmployeeID], [InquiryDate], [OutletID], [StatusProspek], [PerolehanData], [NamaProspek], [AlamatProspek], [TelpRumah], [CityID], [NamaPerusahaan], [AlamatPerusahaan], [Jabatan], [Handphone], [Faximile], [Email], [TipeKendaraan], [Variant], [Transmisi], [ColourCode], [CaraPembayaran], [TestDrive], [QuantityInquiry], [LastProgress], [LastUpdateStatus], [SPKDate], [LostCaseDate], [LostCaseCategory], [LostCaseReasonID], [LostCaseOtherReason], [LostCaseVoiceOfCustomer], [CreationDate], [CreatedBy], [LastUpdateBy], [LastUpdateDate], [Leasing], [DownPayment], [Tenor], [MerkLain])");
            writer.WriteLine("VALUES (@inqNo,'" + model.CompanyCode +
                "','" + model.BranchCode + "','" + model.EmployeeID +
                "',(select top 1 TeamLeader from HrEmployee where EmployeeId = '" + 
                        model.EmployeeID + "'),'" + model.InquiryDate +
                "','" + model.OutletID + "','" + model.StatusProspek +
                "','" + model.PerolehanData + "','" + model.NamaProspek +
                "','" + model.AlamatProspek + "','" + model.TelpRumah +
                "','" + model.CityID + "','" + model.NamaPerusahaan +
                "','" + model.AlamatPerusahaan + "','" + model.Jabatan +
                "','" + model.Handphone + "','" + model.Faximile +
                "','" + model.Email + "','" + model.TipeKendaraan +
                "','" + model.Variant + "','" + model.Transmisi +
                "','" + model.ColourCode + "','" + model.CaraPembayaran +
                "','" + model.TestDrive + "','" + model.QuantityInquiry +
                "','" + model.LastProgress + "','" + model.LastUpdateStatus +
                "','" + model.SPKDate + "','" + model.LostCaseDate +
                "','" + model.LostCaseCategory + "','" + model.LostCaseReasonID +
                "','" + model.LostCaseOtherReason + "','" + model.LostCaseVoiceOfCustomer +
                "','" + model.CreationDate + "','" + model.CreatedBy +
                "','" + model.LastUpdateBy + "','" + model.LastUpdateDate +
                "','" + model.Leasing + "','" + model.DownPayment +
                "','" + model.Tenor + "','" + model.MerkLain +
                "')");
            writer.WriteLine();
            writer.WriteLine("INSERT INTO [dbo].[pmKdpAdditional] ([CompanyCode], [BranchCode], [InquiryNumber], [StatusVehicle], [OthersBrand], [OthersType], [CreatedBy], [CreatedDate], [LastUpdateBy], [LastUpdateDate], [SPKNo], [GiftRefferenceCode], [GiftRefferenceValue], [GiftRefferenceDesc], [ShiftCode])");
            writer.WriteLine("VALUES ('" +
                model2.CompanyCode + "','" +
                model2.BranchCode + "'," +
                "@inqNo" + ",'" +
                model2.StatusVehicle + "','" +
                model2.OthersBrand + "','" +
                model2.OthersType + "','" +
                model2.CreatedBy + "','" +
                model2.CreatedDate + "','" +
                model2.LastUpdateBy + "','" +
                model2.LastUpdateDate + "','" +
                model2.SPKNo + "','" +
                model2.GiftRefferenceCode + "','" +
                model2.GiftRefferenceValue + "','" +
                model2.GiftRefferenceDesc + "','" +
                model2.ShiftCode +
                "')");
            writer.WriteLine();
            writer.WriteLine("INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])");
            writer.WriteLine("VALUES (@inqNo,'" +
                model3.CompanyCode + "','" +
                model3.BranchCode + "','" +
                "1" + "','" +
                "P" + "','" +
                model3.UpdateDate + "','" +
                model3.UpdateUser +
                "')");
            writer.WriteLine();
            writer.WriteLine("INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])");
            writer.WriteLine("VALUES (@inqNo,'" +
                model3.CompanyCode + "','" +
                model3.BranchCode + "','" +
                "2" + "','" +
                "HP" + "','" +
                model3.UpdateDate + "','" +
                model3.UpdateUser +
                "')");
            writer.WriteLine();
            writer.WriteLine("INSERT INTO [dbo].[pmStatusHistory] ([InquiryNumber], [CompanyCode], [BranchCode], [SequenceNo], [LastProgress], [UpdateDate], [UpdateUser])");
            writer.WriteLine("VALUES (@inqNo,'" +
                model3.CompanyCode + "','" +
                model3.BranchCode + "','" +
                "3" + "','" +
                "SPK" + "','" +
                model3.UpdateDate + "','" +
                model3.UpdateUser +
                "')");
            writer.WriteLine();
            writer.WriteLine("set @inqNo = @inqNo + 1");
            writer.WriteLine();
            writer.Close();

        }

        public FileStream GetOrCreateFile()
        {
            int i = 1;
            int maxSize = 25500000;
            bool emptyFilefound = false;
            string path = @"D:\ex" + i + ".sql";
            FileStream fs1 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            long size = fs1.Length;

            //jika file sudah ada dan masi bisa diisi
            if (size < maxSize)
            {
                emptyFilefound = true;
            }

            //cari nama file yg kosong
            while (!emptyFilefound)
            {
                //jika file sudah ada
                if (size > maxSize - 1)
                {
                    fs1.Close();
                    fs1.Dispose();
                    i++;
                    path = @"D:\ex" + i + ".sql";
                    fs1 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    size = fs1.Length;
                }
                else //ketemu yg kosong
                    emptyFilefound = true;
            };
            return fs1;
        }
        #endregion
    }
}
