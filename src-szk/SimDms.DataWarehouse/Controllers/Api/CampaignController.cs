using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using System.Data.SqlClient;
using System.Data;
using SimDms.DataWarehouse.Models;
using System.Web.Script.Serialization;
using System.Threading;
using OfficeOpenXml;
using SimDms.DataWarehouse.Helpers;
using EP = SimDms.DataWarehouse.Helpers.EPPlusHelper;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Globalization;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class CampaignController : BaseController
    {
        public JsonResult ComplainCodeList()
        {
            //var qry = ctx.ReffServices.Where(x => x.RefferenceType == "COMPLNCD").AsQueryable();
            var qry = (from p in ctx.ReffServices
                      where p.RefferenceType == "COMPLNCD"
                      select new
                      {
                          p.RefferenceCode,
                          p.Description,
                          p.DescriptionEng
                      }).Distinct();
            return Json(qry.KGrid());
        }

        public JsonResult ComplainCampaign(string pType)
        {
            var qry = (from p in ctx.svMstCampaigns
                       join cc in ctx.ReffServices on new { RefferenceType = "COMPLNCD", RefferenceCode = p.ComplainCode } equals new { cc.RefferenceType, cc.RefferenceCode }
                       where p.ProductType == pType
                       select new
                       {
                           p.ComplainCode,
                           Description = cc.Description,
                           DescriptionEng = cc.DescriptionEng
                       }).Distinct();
            return Json(qry.KGrid());
        }

        public JsonResult DefectCampaign(string pType, string cCode)
        {
            var qry = (from p in ctx.svMstCampaigns
                       join dc in ctx.ReffServices on new { RefferenceType = "DEFECTCD", RefferenceCode = p.DefectCode } equals new { dc.RefferenceType, dc.RefferenceCode }
                       where p.ProductType == pType && p.ComplainCode == cCode
                       select new
                       {
                           p.DefectCode,
                           Description = dc.Description,
                           DescriptionEng = dc.DescriptionEng
                       }).Distinct();
            return Json(qry.KGrid());
        }

        public JsonResult ChassisCampaign(string pType, string cCode, string dCode)
        {
            var qry = (from p in ctx.svMstCampaigns
                       where p.ProductType == pType && p.ComplainCode == cCode && p.DefectCode == dCode
                       select new
                       {
                           p.ChassisCode,
                           ChassisStartNo = p.ChassisStartNo,
                           ChassisEndNo = p.ChassisEndNo
                       });
            return Json(qry.KGrid());
        }

        public JsonResult OperationCampaign(string pType, string cCode, string dCode, string chassisCode, string chassisStartNo, string chassisEndNo )
        {
            Int32 csNo  = chassisStartNo == "" ? 0 : Convert.ToInt32(chassisStartNo);
            Int32 ceNo = chassisEndNo == "" ? 0 : Convert.ToInt32(chassisEndNo);

            var qry = (from p in ctx.svMstCampaigns
                       join x in ctx.svMstTasks on new { p.OperationNo } equals new { x.OperationNo }
                       where p.ProductType == pType && p.ComplainCode == cCode && p.DefectCode == dCode && p.ChassisCode == chassisCode && p.ChassisStartNo == csNo && p.ChassisEndNo == ceNo
                       select new
                       {
                           p.OperationNo,
                           OperationName = x.Description,
                           p.Description
                       }).Distinct();
            return Json(qry.KGrid());
        }

        public JsonResult DefectCodeList()
        {
            //var qry = ctx.ReffServices.Where(x => x.RefferenceType == "DEFECTCD").AsQueryable();
            var qry = (from p in ctx.ReffServices
                      where p.RefferenceType == "DEFECTCD"
                      select new
                      {
                          p.RefferenceCode,
                          p.Description,
                          p.DescriptionEng
                      }).Distinct();
            return Json(qry.KGrid());
        }

        public JsonResult OperationList()
        {
            var qry = (from p in ctx.svMstTasks
                      //where p.CompanyCode == CurrentUser.DealerCode
                      select new
                      {
                          p.OperationNo,
                          p.Description,
                          IsActive = p.IsActive ? "Aktif" : "Tidak Aktif"
                      }).Distinct();
            return Json(qry.KGrid());
        }

        public JsonResult Browse()
        {
            var qry = (from p in ctx.svMstCampaigns
                       join cc in ctx.ReffServices on new { RefferenceType = "COMPLNCD", RefferenceCode = p.ComplainCode } equals new { cc.RefferenceType, cc.RefferenceCode }
                       join dc in ctx.ReffServices on new { RefferenceType = "DEFECTCD", RefferenceCode = p.DefectCode } equals new { dc.RefferenceType, dc.RefferenceCode }
                       join opn in ctx.svMstTasks on new {  OperationNo = p.OperationNo } equals new { opn.OperationNo }
                       select new
                       {
                           p.ComplainCode,
                           ComplainName = cc.Description,
                           p.DefectCode,
                           DefectName = dc.Description,
                           p.ChassisCode,
                           p.ChassisStartNo,
                           p.ChassisEndNo,
                           p.OperationNo,
                           OperationName = opn.Description,
                           p.CloseDate,
                           p.Description,
                           IsActive = p.IsActive ? "Aktif" : "Tidak Aktif"
                       }).Distinct();
            return Json(qry.KGrid());
        }

        public JsonResult campaignsave(string Model)
        {
            string Uid = "";
            if (CurrentUser.Username.Length > 15)
            {
                Uid = CurrentUser.Username.Substring(1, 15);
            }
            else
            {
                Uid = CurrentUser.Username;
            }
            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<mdlCampaign> lstData = ser.Deserialize<List<mdlCampaign>>(Model);

                if (lstData != null && lstData.Count > 0)
                {
                    foreach (var item in lstData)
                    {
                        string Qry = String.Format(@"uspfn_CampaignSave '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}'",
                            item.ProductType, item.ComplainCode, item.ComplainName, item.DefectCode, item.DefectName, item.ChassisCode, item.ChassisStartNo, item.ChassisEndNo, item.OperationNo, item.OperationName, item.ClosedDate, item.Description, item.IsActive, Uid);

                        ctx.Database.ExecuteSqlCommand(Qry);
                        return Json(new { success = true, message = "Data Saved" });
                    }
                }
                return Json(new { success = false, message = "Data gagal disimpan!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult bulkSave(string Data)
        {
            string Uid = "";
            if (CurrentUser.Username.Length > 15)
            {
                Uid = CurrentUser.Username.Substring(1, 15);
            }
            else
            {
                Uid = CurrentUser.Username;
            }
            try
            {
                JavaScriptSerializer ser = new JavaScriptSerializer();
                List<mdlCampaign> lstData = ser.Deserialize<List<mdlCampaign>>(Data);
                if (lstData != null && lstData.Count > 0)
                {
                    foreach (var item in lstData)
                    {
                        string Qry = String.Format(@"uspfn_CampaignSave '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}'",
                        item.ProductType, item.ComplainCode, "",item.DefectCode, "",item.ChassisCode, item.ChassisStartNo, item.ChassisEndNo, item.OperationNo, "", item.ClosedDate.ToString("yyyy-MM-dd"), item.Description, item.IsActive, Uid);

                        ctx.Database.ExecuteSqlCommand(Qry);
                    }
                    return Json(new { success = true, message="Data Saved" });
                }
                return Json(new { success = false, message = "Data gagal disimpan!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult doSend(string pType)
        {
            try
            {
                GenerateInsCampaign(pType);
                return Json(new { success = true, message = "Proses Kirim Selesai!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        private void GenerateInsCampaign(string pType)
        {
            int j = 0;
            string sqls = "";
            string kirimKe = pType == "2W" ? "ALLR2" : "ALL";

            var dtCampaign = (from a in ctx.svMstCampaigns
                              where a.ProductType == pType && a.IsLocked == false
                              select new
                              {
                                  a.ProductType,
                                  a.ComplainCode,
                                  a.DefectCode,
                                  a.ChassisCode,
                                  a.ChassisStartNo,
                                  a.ChassisEndNo,
                                  a.OperationNo,
                                  a.CloseDate,
                                  a.Description,
                                  a.IsActive,
                                  a.CreatedBy,
                                  a.CreatedDate,
                                  a.LastupdateBy,
                                  a.LastupdateDate
                              });

            if (dtCampaign.Count() > 0)
            {
                sqls = "declare @cmpCode varchar(20) select top 1 @cmpCode=Companycode from gnMstOrganizationHdr" + System.Environment.NewLine;
                sqls += "declare @svMstCampaign table(" + System.Environment.NewLine +
                       "CompanyCode Varchar(15), ProductType Varchar(15), ComplainCode Varchar(15), DefectCode Varchar(15), ChassisCode Varchar(15), ChassisStartNo Integer, ChassisEndNo Integer," + System.Environment.NewLine +
                       "OperationNo Varchar(25), CloseDate DateTime, Description Varchar(100), IsActive Bit, CreatedBy Varchar(25), CreatedDate DateTime, LastupdateBy Varchar(25)," + System.Environment.NewLine +
                       "LastupdateDate DateTime, IsLocked Bit)" + System.Environment.NewLine;


                sqls += "insert into @svMstCampaign" + System.Environment.NewLine;

                dtCampaign.ToList().ForEach(y =>
                {
                    j += 1;
                    if (j == 1)
                    {
                        sqls += "select @cmpCode,'" + y.ProductType + "','" + y.ComplainCode + "','" + y.DefectCode + "','" + y.ChassisCode + "','" + y.ChassisStartNo +
                                "','" + y.ChassisEndNo + "','" + y.OperationNo + "','" + y.CloseDate + "','" + y.Description + "','" + y.IsActive + "','" + y.CreatedBy + "','" + y.CreatedDate +
                                "','" + y.LastupdateBy + "','" + y.LastupdateDate + "','0'" + System.Environment.NewLine;
                    }
                    else
                    {
                        sqls += "union select @cmpCode,'" + y.ProductType + "','" + y.ComplainCode + "','" + y.DefectCode + "','" + y.ChassisCode + "','" + y.ChassisStartNo +
                                "','" + y.ChassisEndNo + "','" + y.OperationNo + "','" + y.CloseDate + "','" + y.Description + "','" + y.IsActive + "','" + y.CreatedBy + "','" + y.CreatedDate +
                                "','" + y.LastupdateBy + "','" + y.LastupdateDate + "','0'" + System.Environment.NewLine;
                    }

                    ctx.Database.ExecuteSqlCommand("UPDATE svMstCampaign SET IsLocked='true', LockingBy='SendToAllDealer' WHERE " +
                                                    "ProductType='" + y.ProductType + "' AND ComplainCode='" + y.ComplainCode + "' AND DefectCode='" + y.DefectCode + "' AND ChassisCode='" + y.ChassisCode + "' " +
                                                    "AND ChassisStartNo='" + y.ChassisStartNo + "' AND ChassisEndNo='" + y.ChassisEndNo + "' AND OperationNo='" + y.OperationNo + "' AND Description='" + y.Description + "'");

                });

                sqls += "declare @CompanyCode varchar(15), @ProductType varchar(15), @ComplainCode varchar(15), @DefectCode varchar(15), @ChassisCode varchar(15), @ChassisStartNo integer, @ChassisEndNo integer, @OperationNo varchar(15), " + System.Environment.NewLine +
                        "@CloseDate datetime, @Description varchar(100), @IsActive bit, @CreatedBy varchar(25), @CreatedDate datetime, @LastupdateBy varchar(25), @LastupdateDate varchar(25), @IsLocked bit" + System.Environment.NewLine;
                sqls += "declare cur Cursor for " + System.Environment.NewLine;
                sqls += "select CompanyCode, ProductType, ComplainCode, DefectCode, ChassisCode, ChassisStartNo, ChassisEndNo, OperationNo, CloseDate, Description, IsActive, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, IsLocked from @svMstCampaign " + System.Environment.NewLine;

                sqls += "open cur" + System.Environment.NewLine;
                sqls += "fetch next from cur into @CompanyCode, @ProductType, @ComplainCode, @DefectCode, @ChassisCode, @ChassisStartNo, @ChassisEndNo, @OperationNo, @CloseDate, @Description, @IsActive, @CreatedBy, @CreatedDate, @LastupdateBy, @LastupdateDate, @IsLocked" + System.Environment.NewLine;
                sqls += "while @@Fetch_Status = 0" + System.Environment.NewLine;
                sqls += "begin" + System.Environment.NewLine;
                sqls += "if not exists (select top 1 1 from svMstCampaign where CompanyCode = @CompanyCode and ProductType = @ProductType and ComplainCode = @ComplainCode and" + System.Environment.NewLine;
                sqls += "DefectCode = @DefectCode and ChassisCode = @ChassisCode and ChassisStartNo = @ChassisStartNo and ChassisEndNo = @ChassisEndNo)" + System.Environment.NewLine;
                sqls += "begin" + System.Environment.NewLine;
                sqls += "insert into svMstCampaign (CompanyCode, ProductType, ComplainCode, DefectCode, ChassisCode, ChassisStartNo, ChassisEndNo, OperationNo, CloseDate, Description, IsActive, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, IsLocked)" + System.Environment.NewLine +
                        "values(@CompanyCode, @ProductType, @ComplainCode, @DefectCode, @ChassisCode, @ChassisStartNo, @ChassisEndNo, @OperationNo, @CloseDate, @Description, @IsActive, @CreatedBy, @CreatedDate, @LastupdateBy, @LastupdateDate, @IsLocked)" + System.Environment.NewLine;
                sqls += "end" + System.Environment.NewLine;
                sqls += "else" + System.Environment.NewLine;
                sqls += "begin" + System.Environment.NewLine;
                sqls += "if not exists (select top 1 1 from svMstCampaignHist where CompanyCode = @CompanyCode and ProductType = @ProductType and ComplainCode = @ComplainCode and" + System.Environment.NewLine;
                sqls += "DefectCode = @DefectCode and ChassisCode = @ChassisCode and ChassisStartNo = @ChassisStartNo and ChassisEndNo = @ChassisEndNo)" + System.Environment.NewLine;
                sqls += "begin" + System.Environment.NewLine;
                sqls += "insert into svMstCampaignHist (CompanyCode, ProductType, ComplainCode, DefectCode, ChassisCode, ChassisStartNo, ChassisEndNo, OperationNo, CloseDate, Description, IsActive, CreatedBy, CreatedDate, LastupdateBy, LastupdateDate, IsLocked)" + System.Environment.NewLine +
                        "values(@CompanyCode, @ProductType, @ComplainCode, @DefectCode, @ChassisCode, @ChassisStartNo, @ChassisEndNo, @OperationNo, @CloseDate, @Description, @IsActive, @CreatedBy, @CreatedDate, @LastupdateBy, @LastupdateDate, @IsLocked)" + System.Environment.NewLine;
                sqls += "end" + System.Environment.NewLine;
                sqls += "update svMstCampaign Set" + System.Environment.NewLine;
                sqls += "ComplainCode = @ComplainCode, DefectCode = @DefectCode, ChassisCode = @ChassisCode, ChassisStartNo = @ChassisStartNo, ChassisEndNo = @ChassisEndNo," + System.Environment.NewLine;
                sqls += "OperationNo = @OperationNo, CloseDate = @CloseDate, Description = @Description, IsActive = @IsActive, LastupdateBy = @LastupdateBy, LastupdateDate = @LastupdateDate" + System.Environment.NewLine;
                sqls += "where CompanyCode = @CompanyCode and ProductType = @ProductType and ComplainCode = @ComplainCode and DefectCode = @DefectCode and ChassisCode = @ChassisCode and ChassisStartNo = @ChassisStartNo and ChassisEndNo = @ChassisEndNo" + System.Environment.NewLine;
                sqls += "end" + System.Environment.NewLine;
                sqls += "fetch next from cur into @CompanyCode, @ProductType, @ComplainCode, @DefectCode, @ChassisCode, @ChassisStartNo, @ChassisEndNo, @OperationNo, @CloseDate, @Description, @IsActive, @CreatedBy, @CreatedDate, @LastupdateBy, @LastupdateDate, @IsLocked" + System.Environment.NewLine;
                sqls += "end" + System.Environment.NewLine;
                sqls += "close cur" + System.Environment.NewLine;
                sqls += "deallocate cur";

                GenerateSQL(new SysSQLGateway() { TaskNo = DateTime.Now.ToString("yyyyMMddHHmmss") + "_Campaign", TaskName = "Campaign_" + DateTime.Now.ToString("yyyyMMddHHmmss"), SQL = sqls, DealerCode = kirimKe }); //DealerCode : Target Server atau Database
            }
       }


        public JsonResult doExport()
        {
            var pType = Request.Params["ProductType"];
            var complainCode = Request.Params["ComplainCode"];
            var defectCode = Request.Params["DefectCode"];
            var chassisCode = Request.Params["ChassisCode"];
            var chassisStartNo = Request.Params["ChassisStartNo"];
            var chassisEndNo = Request.Params["ChassisEndNo"];
            var operationNo = Request.Params["OperationNo"];
            var descrip = Request.Params["Description"];
            var query = @"exec uspfn_GetCampaignClaim @ProductType, @ComplainCode, @DefectCode, @ChassisCode, @ChassisStartNo, @ChassisEndNo, @OperationNo, @Description";

            var parameters = new[]
                {
                    new SqlParameter("@ProductType", pType),
                    new SqlParameter("@ComplainCode", complainCode),
                    new SqlParameter("@DefectCode", defectCode),
                    new SqlParameter("@ChassisCode", chassisCode),
                    new SqlParameter("@ChassisStartNo", chassisStartNo),
                    new SqlParameter("@ChassisEndNo", chassisEndNo),
                    new SqlParameter("@OperationNo", operationNo),
                    new SqlParameter("@Description", descrip)
                };

            var result = ctx.MultiResultSetSqlQuery(query, parameters);
            var message = "";
            var complain = complainCode == "" ? "ALL" : complainCode;
            var defect = defectCode == "" ? "ALL" : defectCode;
            var chCode = "";
            var chNoFromTo = "";
            if (chassisCode == "") {
                chCode = "ALL";
                chNoFromTo = "ALL";
            }
            else
            {
                chCode = chassisCode;
                chNoFromTo = chassisStartNo + " - " + chassisEndNo;
            }

            var opsNo = operationNo == "" ? "ALL" : operationNo;
            var descr = descrip == "" ? "ALL" : descrip;

            try
            {
                var package = new ExcelPackage();
                package = GenerateExcel(package, pType, complain, defect, chCode, chNoFromTo, opsNo, descr, result);
                var content = package.GetAsByteArray();
                var guid = Guid.NewGuid().ToString();
                TempData.Add(guid, content);

                return Json(new { message = message, value = guid });
            }
            catch (Exception e)
            {
                message = e.Message;
                return Json(new { message = message });
            }
        }

        private static ExcelPackage GenerateExcel(ExcelPackage package, string pType, string complain, string defect, string chassisCode, string chassisNoFromTo, string operationNo, string descr, MultiResultSetReader result)
        {
            var sheet = package.Workbook.Worksheets.Add("CampaignClaimReport");
            var z = sheet.Cells[1, 1];
            var data = result.ResultSetFor<CampaignClaim>().ToList();

            #region -- Constants --
            const int
                rTitle = 1,
                rPType = 2,
                rComplain = 3,
                rDefect = 4,
                rChassisCode = 5,
                rChassisNo = 6,
                rOperationNo = 7,
                rDescription = 8,
                rHeader1 = 9,
                rHeader2 = 10,
                rData = 11,

                cStart = 1,
                cNo = 1,
                cJobOrder = 2,
                cJobOrderDate = 3,
                cChassisCode = 4,
                cChassisNo = 5,
                cBasicModel = 6,
                cDealerCode = 7,
                cDealerName = 8,
                cEnd = 8;

            double
                wNo = EP.GetTrueColWidth(5),
                wJobOrder = EP.GetTrueColWidth(18),
                wVin = EP.GetTrueColWidth(22),
                wDCode = EP.GetTrueColWidth(18),
                wDDesc = EP.GetTrueColWidth(65);

            const string
                fCustom = "_(* #,##0_);_(* (#,##0);_(* \"-\"_);_(@_)";
            #endregion

            sheet.Column(cNo).Width = wNo;
            sheet.Column(cJobOrder).Width = wJobOrder;
            sheet.Column(cJobOrderDate).Width = wJobOrder;
            sheet.Column(cChassisCode).Width = wVin;
            sheet.Column(cChassisNo).Width = wVin;
            sheet.Column(cBasicModel).Width = wVin;
            sheet.Column(cDealerCode).Width = wDCode;
            sheet.Column(cDealerName).Width = wDDesc;

            #region -- Title --
            z.Address = EP.GetRange(rTitle, cStart, rTitle, cEnd);
            z.Value = "Campaign Claim Report";
            z.Style.Font.Bold = true;
            z.Style.Font.Size = 14f;
            z.Merge = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            z.Address = EP.GetRange(rPType, cNo, rPType, cJobOrder);
            z.Value = "Product Type";
            z.Merge = true;
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rPType, cJobOrderDate);
            z.Value = ": " + pType;
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rComplain, cNo, rComplain, cJobOrder);
            z.Value = "Complain";
            z.Merge = true;
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rComplain, cJobOrderDate);
            if (complain == "ALL")
            {
                z.Value = ": " + complain;
            }
            else
            {
                z.Value = ": " + data[0].Complain;
            }
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rDefect, cNo, rDefect, cJobOrder);
            z.Value = "Defect";
            z.Merge = true;
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDefect, cJobOrderDate);
            if (defect == "ALL")
            {
                z.Value = ": " + defect;
            }
            else
            {
                z.Value = ": " + data[0].Defect;
            }
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rChassisCode, cNo, rChassisCode, cJobOrder);
            z.Value = "Chassis Code";
            z.Merge = true;
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rChassisCode, cJobOrderDate);
            if (chassisCode == "ALL")
            {
                z.Value = ": " + chassisCode;
            }
            else
            {
                z.Value = ": " + data[0].ChassisCode;
            }
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rChassisNo, cNo, rChassisNo, cJobOrder);
            z.Value = "Chassis No (From-To)";
            z.Merge = true;
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rChassisNo, cJobOrderDate);
            if (chassisNoFromTo == "ALL")
            {
                z.Value = ": " + chassisNoFromTo;
            }
            else
            {
                z.Value = ": " + data[0].ChassisNoFromTo;
            }
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rOperationNo, cNo, rOperationNo, cJobOrder);
            z.Value = "Operation No";
            z.Merge = true;
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rOperationNo, cJobOrderDate);
            if (operationNo == "ALL")
            {
                z.Value = ": " + operationNo;
            }
            else
            {
                z.Value = ": " + data[0].OperationNo;
            }
            z.Style.Font.Bold = true;

            z.Address = EP.GetRange(rDescription, cNo, rDescription, cJobOrder);
            z.Value = "Description";
            z.Merge = true;
            z.Style.Font.Bold = true;
            z.Address = EP.GetCell(rDescription, cJobOrderDate);
            if (descr == "ALL")
            {
                z.Value = ": " + descr;
            }
            else
            {
                z.Value = ": " + data[0].Description;
            }
            z.Style.Font.Bold = true;
            #endregion

            #region -- Header --
            z.Address = EP.GetRange(rHeader1, cStart, rHeader2, cEnd);
            z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(197, 217, 241));
            z.Style.Font.Bold = true;
            z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            z.Address = EP.GetRange(rHeader1, cNo, rHeader2, cNo);
            z.Value = "No";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cJobOrder, rHeader2, cJobOrder);
            z.Value = "Job Order No";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cJobOrderDate, rHeader2, cJobOrderDate);
            z.Value = "Job Order Date";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cChassisCode, rHeader1, cChassisNo);
            z.Value = "Vehicle";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cChassisCode);
            z.Value = "Chassis Code";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetCell(rHeader2, cChassisNo);
            z.Value = "Chassis No";
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cBasicModel, rHeader2, cBasicModel);
            z.Value = "Basic Model";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cDealerCode, rHeader2, cDealerCode);
            z.Value = "Dealer Code";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            z.Address = EP.GetRange(rHeader1, cDealerName, rHeader2, cDealerName);
            z.Value = "Dealer Name";
            z.Merge = true;
            z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            #endregion

            #region -- Data --
            if (data.Count == 0) return package;
            for (int i = 0; i < data.Count; i++)
            {
                var row = rData + i;

                var items = new List<ExcelCellItem>
                {
                    new ExcelCellItem { Column = cNo, Value = i + 1 },
                    new ExcelCellItem { Column = cJobOrder, Value = data[i].JobOrderNo },
                    new ExcelCellItem { Column = cJobOrderDate, Value = data[i].JobOrderDate.ToString("dd/MM/yyyy") },
                    new ExcelCellItem { Column = cChassisCode, Value = data[i].ChassisCode },
                    new ExcelCellItem { Column = cChassisNo, Value = data[i].ChassisNo },
                    new ExcelCellItem { Column = cBasicModel, Value = data[i].BasicModel },
                    new ExcelCellItem { Column = cDealerCode, Value = data[i].BranchCode },
                    new ExcelCellItem { Column = cDealerName, Value = data[i].DealerName },
                };

                foreach (var item in items)
                {
                    z.Address = EP.GetCell(row, item.Column);
                    z.Value = item.Value;
                    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
                    z.Style.Numberformat.Format = item.Format != null ? item.Format : fCustom;
                }
            }
            #endregion

            //#region -- Total --
            //var rTotal = data.Count + rData;
            //z.Address = EP.GetRange(rTotal, cStart, rTotal, cEnd);
            //z.Style.Fill.PatternType = ExcelFillStyle.Solid;
            //z.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(234, 241, 221));
            //z.Style.Font.Bold = true;
            ////z.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ////z.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            //z.Address = EP.GetRange(rTotal, cPeriode, rTotal, cOutlet);
            //z.Value = "TOTAL";
            //z.Merge = true;
            //z.Style.Border.BorderAround(ExcelBorderStyle.Thin);

            //var sums = new List<ExcelCellItem>
            //{
            //    //new ExcelCellItem { Column = cPeriode, Value = "TOTAL" },
            //    //new ExcelCellItem { Column = cOutlet, Value = "" },
            //    new ExcelCellItem { Column = cDoData, Value = total[0].TotDoData },
            //    new ExcelCellItem { Column = cDelivery, Value = total[0].TotDeliveryDate },
            //    new ExcelCellItem { Column = cTdCallByDO, Value = total[0].TotTDaysCallData },
            //    new ExcelCellItem { Column = cTdCallByInput, Value = total[0].TotTDaysCallByInput },
            //};

            //foreach (var sum in sums)
            //{
            //    z.Address = EP.GetCell(rTotal, sum.Column);
            //    z.Value = sum.Value;
            //    z.Style.Border.BorderAround(ExcelBorderStyle.Thin);
            //    z.Style.Numberformat.Format = sum.Format != null ? sum.Format : fCustom;
            //}

            //sheet.Row(rTotal).Height = hTotal;

            //#endregion

            return package;
        }

        public FileContentResult DownloadExcelFile(string key, string fileName)
        {
            var content = TempData.FirstOrDefault(x => x.Key == key).Value as byte[];
            TempData.Clear();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AppendHeader("content-disposition", "attachment; filename=" + fileName + ".xlsx");
            Response.Buffer = true;
            Response.BinaryWrite(content);
            Response.End();
            return File(content, contentType, "");
        }

        private class CampaignClaim
        {
            public string ProductType { get; set; }
            public string Complain { get; set; }
            public string Defect { get; set; }
            public string ChassisCode { get; set; }
            public string ChassisNoFromTo { get; set; }
            public string OperationNo { get; set; }
            public string Description { get; set; }
            public string JobOrderNo { get; set; }
            public DateTime JobOrderDate { get; set; }
            public Decimal ChassisNo {get; set; }
            public string BasicModel { get; set; }
            public string BranchCode { get; set; }
            public string DealerName { get; set; }
        }

    }
}