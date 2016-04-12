using ClosedXML.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimDms.DataWarehouse.Helpers;
using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class QuestionnaireController : BaseController
    {
        protected ResultModel InitializeResult()
        {
            return new ResultModel()
            {
                status = false,
                message = "",
                details = "",
                data = null,
                isValid = true
            };
        }

        [HttpPost]
        public JsonResult QaTransaction()
        {
            ResultModel result = InitializeResult();
            bool isupdated = false;
            bool isUpdateFailed = false;
            try
            {
                var salesmodelcode = Request["SalesModelCode"];
                var chassiscode = Request["ChassisCode"];
                var chassisno = Request["ChassisNo"];
                var namakonsumen = Request["NamaKonsumen"];
                var UmurKonsumen = Request["UmurKonsumen"];
                var JenisKelamin = Request["JenisKelamin"];
                var ContactNo = Request["ContactNo"];
                var CashOrCredit = Request["CashOrCredit"] == "1" ? true : false;
                var CreditInstalment = Request["CreditInstalment"];
                var AsVehicle = Request["AsVehicle"];
                var AsVehicleReplacedMerk = Request["AsVehicleReplacedMerk"];
                var AsVehicleReplacedType = Request["AsVehicleReplacedType"];
                var AsVehicleOtherReplacedMerk = Request["AsVehicleOtherReplacedMerk"];
                var AsVehicleOtherReplacedType = Request["AsVehicleOtherReplacedType"];
                var AsVehicleReplacedReason = Request["AsVehicleReplacedReason"];
                var AsVehicleReplacedReasonOther = Request["AsVehicleReplacedReasonOther"];
                var AsVehicleAdditionalSuzuki = Request["AsVehicleAdditionalSuzuki"];
                var AsVehicleAdditionalDaihatsu = Request["AsVehicleAdditionalDaihatsu"];
                var AsVehicleAdditionalMitsubishi = Request["AsVehicleAdditionalMitsubishi"];
                var AsVehicleAdditionalOthers = Request["AsVehicleAdditionalOthers"];
                var AsVehicleNew = Request["AsVehicleNew"];
                var AsVehicleNewOthers = Request["AsVehicleNewOther"];

                var LoadOneTrip = Request["LoadOneTrip"];
                var LongInKMAnnualTrip = Request["LongInKMAnnualTrip"];
                var OccupationPart = Request["OccupationPart"];
                var OccupationDetail = Request["OccupationDetail"];
                var OccupationPartOther = Request["OccupationPartOther"];
                var OccupationDetailOther = Request["OccupationDetailOther"];
                var PassengerCar = Request["PassengerCar"];
                var PassengerCarYes = Request["PassengerCarYes"];
                var MotorCycleExists = Request["MotorCycleExists"];

                var MerkPsgrCarToyota = (Request["MerkPsgrCar1"] ?? "") == "on" ? true : false;
                var TipePsgrCarToyota = Request["TipePsgrCar1"];
                var MerkPsgrCarHonda = (Request["MerkPsgrCar2"] ?? "") == "on" ? true : false;
                var TipePsgrCarHonda = Request["TipePsgrCar2"];
                var MerkPsgrCarDaihatsu = (Request["MerkPsgrCar3"] ?? "") == "on" ? true : false;
                var TipePsgrCarDaihatsu = Request["TipePsgrCar3"];
                var MerkPsgrCarSuzuki = (Request["MerkPsgrCar4"] ?? "") == "on" ? true : false;
                var TipePsgrCarSuzuki = Request["TipePsgrCar4"];
                var MerkPsgrCarOthers = (Request["MerkPsgrCar5"] ?? "") == "on" ? true : false;
                var MerkOthersPsgrCarOthers = Request["MerkOthersPsgrCar5"];
                var TipePsgrCarOthers = Request["TipePsgrCar5"];

                var StatusKonsumen = Request["StatusKonsumen"];

                #region old method (tabled) for passenger cars
                //var passengerCars = Request["passengerCars"];

                //JArray jsonObjArr = JArray.Parse(passengerCars);

                //DataTable _dt = new DataTable();
                //_dt.Columns.Add("FunctionType", typeof(string));
                //_dt.Columns.Add("FunctionCode", typeof(string));
                //_dt.Columns.Add("FunctionDesc", typeof(string));
                //_dt.Columns.Add("FunctionOthers", typeof(string));
                //_dt.Columns.Add("FunctionModel", typeof(string));

                //for (int i = 0; i < jsonObjArr.Count; i++)
                //{
                //    JObject jsonObj = (JObject)jsonObjArr[i];
                //    var merkCode = jsonObj["merk"].ToString().Substring(0, 1);
                //    var merk = jsonObj["merk"].ToString().Substring(3, jsonObj["merk"].ToString().Length - 3);
                //    var merkOthers = "";
                //    var tipe = jsonObj["tipe"];
                //    if (merkCode == "E")
                //    {
                //        //others
                //        merk = "Others";
                //        merkOthers = jsonObj["merk"].ToString().Substring(3, jsonObj["merk"].ToString().Length - 3);
                //    }
                //    _dt.Rows.Add("Passenger", merkCode, merk, merkOthers, tipe);
                //}

                #endregion

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spInsertTrQuestionnaire";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("companycode", CompanyCode ?? "");
                cmd.Parameters.AddWithValue("branchcode", BranchCode ?? "");
                cmd.Parameters.AddWithValue("salesmodelcode", salesmodelcode ?? "");
                cmd.Parameters.AddWithValue("chassiscode", chassiscode ?? "");
                cmd.Parameters.AddWithValue("chassisno", chassisno ?? "");
                cmd.Parameters.AddWithValue("rname", namakonsumen ?? "");
                cmd.Parameters.AddWithValue("rage", UmurKonsumen ?? "");
                cmd.Parameters.AddWithValue("rgender", JenisKelamin);
                cmd.Parameters.AddWithValue("rphone", ContactNo ?? "");
                cmd.Parameters.AddWithValue("iscredit", CashOrCredit);
                cmd.Parameters.AddWithValue("installment", string.IsNullOrEmpty(CreditInstalment) ? "0" : CreditInstalment);
                cmd.Parameters.AddWithValue("rstatuscode", AsVehicle ?? "");
                cmd.Parameters.AddWithValue("isremerkcode", AsVehicleReplacedMerk ?? "");
                cmd.Parameters.AddWithValue("isremerkothers", AsVehicleOtherReplacedMerk ?? "");
                cmd.Parameters.AddWithValue("isretypecode", AsVehicleReplacedType ?? "");
                cmd.Parameters.AddWithValue("isretypeothers", AsVehicleOtherReplacedType ?? "");
                cmd.Parameters.AddWithValue("isrereasoncode", AsVehicleReplacedReason ?? "");
                cmd.Parameters.AddWithValue("isrereasonothers", AsVehicleReplacedReasonOther ?? "");
                cmd.Parameters.AddWithValue("isaddsuzuki", string.IsNullOrEmpty(AsVehicleAdditionalSuzuki) ? DBNull.Value : (object)AsVehicleAdditionalSuzuki);
                cmd.Parameters.AddWithValue("isadddaihatsu", string.IsNullOrEmpty(AsVehicleAdditionalDaihatsu) ? DBNull.Value : (object)AsVehicleAdditionalDaihatsu);
                cmd.Parameters.AddWithValue("isaddmitsubishi", string.IsNullOrEmpty(AsVehicleAdditionalMitsubishi) ? DBNull.Value : (object)AsVehicleAdditionalMitsubishi);
                cmd.Parameters.AddWithValue("isaddothers", AsVehicleAdditionalOthers == "" ? DBNull.Value : (object)AsVehicleAdditionalOthers);
                cmd.Parameters.AddWithValue("loadcpcode", LoadOneTrip ?? "");
                cmd.Parameters.AddWithValue("annualdrcode", LongInKMAnnualTrip ?? "");
                cmd.Parameters.AddWithValue("occpcode", OccupationPart ?? "");
                cmd.Parameters.AddWithValue("occpothers", OccupationPartOther ?? "");
                cmd.Parameters.AddWithValue("occdetailcode", OccupationDetail ?? "");
                cmd.Parameters.AddWithValue("occdetailothers", OccupationDetailOther ?? "");
                cmd.Parameters.AddWithValue("pccode", PassengerCar ?? "");
                cmd.Parameters.AddWithValue("pcunitcode", PassengerCarYes ?? "");
                cmd.Parameters.AddWithValue("motorcode", MotorCycleExists ?? "");

                cmd.Parameters.AddWithValue("ispctoyota", MerkPsgrCarToyota);
                cmd.Parameters.AddWithValue("pctoyotatype", TipePsgrCarToyota);
                cmd.Parameters.AddWithValue("ispchonda", MerkPsgrCarHonda);
                cmd.Parameters.AddWithValue("pchondatype", TipePsgrCarHonda);
                cmd.Parameters.AddWithValue("ispcdaihatsu", MerkPsgrCarDaihatsu);
                cmd.Parameters.AddWithValue("pcdaihatsutype", TipePsgrCarDaihatsu);
                cmd.Parameters.AddWithValue("ispcsuzuki", MerkPsgrCarSuzuki);
                cmd.Parameters.AddWithValue("pcsuzukitype", TipePsgrCarSuzuki);
                cmd.Parameters.AddWithValue("ispcothers", MerkPsgrCarOthers);
                cmd.Parameters.AddWithValue("pcothersmerk", MerkOthersPsgrCarOthers);
                cmd.Parameters.AddWithValue("pcotherstype", TipePsgrCarOthers);
                cmd.Parameters.AddWithValue("asvehiclenew", AsVehicleNew ?? "");
                cmd.Parameters.AddWithValue("asvehiclenewothers", AsVehicleNewOthers ?? "");
                cmd.Parameters.AddWithValue("username", CurrentUser.Username ?? "");
                cmd.Parameters.AddWithValue("statuskonsumen", StatusKonsumen ?? "");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                isupdated = ds.Tables[0].Rows[0]["EndResult"].ToString() == "2" ? true : false;

                bool isValidData = ds.Tables[0].Rows[0]["EndResult"].ToString() == "0" ? false : true;

                isUpdateFailed = ds.Tables[0].Rows[0]["EndResult"].ToString() == "3" ? true : false;

                result.isValid = isValidData & !isUpdateFailed;

                result.message = ds.Tables[0].Rows[0]["EndResult"].ToString() == "0" ? "Data tidak berhasil karena tidak valid" : ds.Tables[0].Rows[0]["EndResult"].ToString() == "1" ? "Data has been saved" : ds.Tables[0].Rows[0]["EndResult"].ToString() == "2" ? "Data has been updated" : ds.Tables[0].Rows[0]["EndResult"].ToString() == "3" ? "Data tidak dapat diubah karena sudah diinput lewat dari sehari" : "";

                result.status = true;

                updateLastTrnInfo("Qa", "QaTrQuestionnaire");
            }
            catch (Exception ex)
            {
                result.message = isupdated ? "An Error Occurred While Trying to update data, please try again later or reload and try again!" : "An Error Occurred While Trying to save data, please try again later or reload and try again!";
                result.status = false;
                throw ex;
            }


            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public JsonResult populateEditQuestionnaire()
        {
            var chassiscode = Request["ChassisCode"];
            var chassisno = Request["ChassisNo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spGetQuestionnaireDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("chassiscode", chassiscode ?? "");
            cmd.Parameters.AddWithValue("chassisno", chassisno ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult removeQuestionnaire()
        {
            ResultModel result = InitializeResult();
            try
            {
                var chassiscode = Request["ChassisCode"];
                var chassisno = Request["ChassisNo"];

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spRemoveQuestionnaire";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("chassiscode", chassiscode ?? "");
                cmd.Parameters.AddWithValue("chassisno", chassisno ?? "");

                cmd.Connection.Open();

                cmd.ExecuteNonQuery();

                cmd.Connection.Close();

                result.message = "Data has been removed";
                result.status = true;
            }
            catch (Exception ex)
            {
                result.message = "An Error Occurred While Trying to remove data, please try again later or reload and try again!";
                result.status = false;
                throw ex;
            }

            return Json(result, JsonRequestBehavior.DenyGet);
        }

        public ActionResult QuestionnaireRekapProd()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var companyCode = CompanyCode;
            var branchCode = Request.Params["BranchCode"] == "-- SELECT All --" ? "%" : Request.Params["BranchCode"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "dd-MM-yyyy HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "dd-MM-yyyy HH:mm:ss", null);
            var reportOption = Request.Params["OutletOption"];

            List<IQueryable<QaProdModel>> listIqueryable = new List<IQueryable<QaProdModel>>();

            IQueryable<QaProdModel> z = null;

            var qry = z;

            var listbranchs = ctx.Database.SqlQuery<qaBranchModel>("exec uspfn_SpGetBranchesByCompanyCode @companyCode=@p0", companyCode).AsQueryable().OrderBy(p => p.value);

            if (reportOption == "1")
            {
                //Summary all outlets
                qry = ctx.Database.SqlQuery<QaProdModel>("exec uspfn_SpQuestionnaireProd @companyCode=@p0, @branchCode=@p1, @prodDateStart=@p2, @prodDateEnd=@p3", companyCode, branchCode, dateStart, dateEnd).AsQueryable();

                listIqueryable.Add(qry);
            }
            else
            {
                //Detail per outlet
                foreach (var row in listbranchs.OrderBy(p => p.value))
                {
                    var bcode = row.value;
                    qry = ctx.Database.SqlQuery<QaProdModel>("exec uspfn_SpQuestionnaireProd @companyCode=@p0, @branchCode=@p1, @prodDateStart=@p2, @prodDateEnd=@p3", companyCode, bcode, dateStart, dateEnd).AsQueryable();
                    listIqueryable.Add(qry);
                }
            }

            var wb = new XLWorkbook();

            if (listIqueryable != null && listIqueryable.Count > 0)
            {
                int count = 0;
                foreach (var item in listIqueryable)
                {
                    var currBranch = (branchCode != "" && branchCode != "%") ? listbranchs.Where(p => p.value == branchCode).FirstOrDefault() : listbranchs.ElementAt(count);
                    var ws = wb.Worksheets.Add(reportOption == "1" ? "Summary Rekapilasi" : currBranch.value);
                    int recNo = 1;

                    var rngTable = ws.Range("A1:C1");

                    //rngTable.Style
                    //    .Font.SetBold()
                    //    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                    //    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;
                    rngTable.Style.Font.FontColor = XLColor.Black;
                    //rngTable.Style.Fill.BackgroundColor = XLColor.LightGray;

                    rngTable.Merge();
                    rngTable.Value = "REKAPILASI";
                    rngTable.Style.Font.FontSize = 20;
                    rngTable.Style.Font.Underline = XLFontUnderlineValues.Single;
                    //rngTable.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    ws.Columns("1").Width = 10;
                    ws.Columns("2").Width = 75;
                    ws.Columns("3").Width = 27;

                    //ws.Cell("B" + recNo).Value = "REKAPILASI";
                    //ws.Cell("B" + recNo).Style.Font.FontSize = 30;
                    //ws.Cell("B" + recNo).Style.Font.Underline = XLFontUnderlineValues.Single;
                    //ws.Cell("B" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    recNo += 2;

                    ws.Cell("B" + recNo).Value = "Dealer : " + CompanyName;
                    ws.Cell("B" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("B" + recNo).Style.Font.Bold = true;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    recNo++;

                    ws.Cell("B" + recNo).Value = "Date : " + dateStart.ToString("dd MMM yyyy") + " s/d " + dateEnd.ToString("dd MMM yyyy");
                    ws.Cell("B" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("B" + recNo).Style.Font.Bold = true;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    recNo++;

                    ws.Cell("A" + recNo).Value = "";
                    ws.Cell("B" + recNo).Value = "Pertanyaan";
                    ws.Cell("C" + recNo).Value = "Jawaban";
                    ws.Cell("A" + recNo).Style.Font.FontSize = 11;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                    ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    recNo++;

                    ws.Cell("A" + recNo).Value = "";
                    ws.Cell("B" + recNo).Value = "";
                    ws.Cell("C" + recNo).Value = (reportOption == "1" && branchCode == "%") ? "SEMUA OUTLET" : (currBranch != null ? currBranch.CompanyName : "");
                    ws.Cell("A" + recNo).Style.Font.FontSize = 11;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                    //ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                    ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    recNo++;
                    //Spaces
                    ws.Cell("A" + recNo).Value = "";
                    ws.Cell("B" + recNo).Value = "";
                    ws.Cell("C" + recNo).Value = "";

                    recNo++;

                    foreach (var row in item)
                    {
                        ws.Cell("A" + recNo).Value = row.Num;
                        ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                        ws.Cell("B" + recNo).Value = row.QuestionAnswer;
                        ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                        if (row.Style == "LGA")
                        {
                            ws.Cell("B" + recNo).Style.Font.Bold = true;
                            ws.Cell("B" + recNo).Style.Fill.BackgroundColor = XLColor.LightGray;
                        }
                        else if (row.Style == "LG")
                        {
                            ws.Cell("B" + recNo).Style.Font.Bold = true;
                            ws.Cell("B" + recNo).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        }
                        else if (row.Style == "BL")
                        {
                            ws.Cell("B" + recNo).Style.Font.Bold = true;
                        }
                        else if (row.Style == "PI")
                        {
                            ws.Cell("B" + recNo).Style.Font.Bold = true;
                            ws.Cell("B" + recNo).Style.Fill.BackgroundColor = XLColor.LightPink;
                        }
                        else if (row.Style == "LB")
                        {
                            ws.Cell("B" + recNo).Style.Font.Bold = true;
                            ws.Cell("B" + recNo).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        }
                        else if (row.Style == "Y")
                        {
                            ws.Cell("B" + recNo).Style.Font.Bold = true;
                            ws.Cell("B" + recNo).Style.Fill.BackgroundColor = XLColor.Yellow;
                            ws.Cell("C" + recNo).Style.Font.Bold = true;
                            ws.Cell("C" + recNo).Style.Fill.BackgroundColor = XLColor.Yellow;
                        }

                        ws.Cell("C" + recNo).Value = row.Quantity;
                        ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        ws.Cell("A" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("B" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("C" + recNo).Style.Font.FontSize = 10;

                        recNo++;
                    }
                    count++;
                }
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult QuestionnaireRekapProdByTypeDealer()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_RekapByType" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var statusKonsumen = Request.Params["StatusKonsumen"] == "" ? "%" : Request.Params["StatusKonsumen"];
            var companyCode = CompanyCode ?? "";
            //var branchCode = Request.Params["BranchCode"] ?? "";
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);
            //var reportOption = Request.Params["OutletOption"];


            List<IQueryable<QaProdByTypeModel>> listIqueryable = new List<IQueryable<QaProdByTypeModel>>();

            IQueryable<QaProdByTypeModel> z = null;

            var qryz = z;

            var listbranchs = ctx.Database.SqlQuery<qaBranchModel>("exec uspfn_SpGetBranchesByCompanyCode @companyCode=@p0", companyCode).AsQueryable().OrderBy(p => p.value);

            foreach (var row in listbranchs.OrderBy(p => p.value))
            {
                var bcode = row.value;
                qryz = ctx.Database.SqlQuery<QaProdByTypeModel>("exec uspfn_SpGnQaRekapByType @companyCode=@p0, @branchCode=@p1, @dateStart=@p2, @dateEnd=@p3, @statusKonsumen=@p4", companyCode, bcode, dateStart, dateEnd, statusKonsumen).AsQueryable();
                listIqueryable.Add(qryz);
            }

            var wb = new XLWorkbook();

            if (listIqueryable != null && listIqueryable.Count > 0)
            {
                int count = 0;
                foreach (var qry in listIqueryable)
                {
                    var currBranch = listbranchs.ElementAt(count);
                    var ws = wb.Worksheets.Add(currBranch.value);

                    //qry = ctx.Database.SqlQuery<QaProdByTypeModel>("exec uspfn_SpGnQaRekapByType @companyCode=@p0, @branchCode=@p1, @dateStart=@p2, @dateEnd=@p3", companyCode, branchCode, dateStart, dateEnd).AsQueryable();

                    //var wb = new XLWorkbook();

                    //var ws = wb.Worksheets.Add("Rekapitulasi By Type");

                    //var currBranch = (branchCode != "" && branchCode != "%") ? listbranchs.Where(p => p.value == branchCode).FirstOrDefault() : listbranchs.ElementAt(count);
                    //var ws = wb.Worksheets.Add(reportOption == "1" ? "Summary Rekapilasi" : currBranch.value);
                    int recNo = 1;

                    var rngTable = ws.Range("A2:H2");

                    //rngTable.Style
                    //    .Font.SetBold()
                    //    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                    //    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;
                    rngTable.Style.Font.FontColor = XLColor.Black;
                    //rngTable.Style.Fill.BackgroundColor = XLColor.LightGray;

                    rngTable.Merge();
                    rngTable.Value = "REKAPITULASI By Type";
                    rngTable.Style.Font.FontSize = 20;
                    rngTable.Style.Font.Underline = XLFontUnderlineValues.Single;
                    //rngTable.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    ws.Columns("1").Width = 60;
                    ws.Columns("2").Width = 25;
                    ws.Columns("3").Width = 25;
                    ws.Columns("4").Width = 25;
                    ws.Columns("5").Width = 25;
                    ws.Columns("6").Width = 25;
                    ws.Columns("7").Width = 25;
                    ws.Columns("8").Width = 25;

                    //ws.Cell("B" + recNo).Value = "REKAPILASI";
                    //ws.Cell("B" + recNo).Style.Font.FontSize = 30;
                    //ws.Cell("B" + recNo).Style.Font.Underline = XLFontUnderlineValues.Single;
                    //ws.Cell("B" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    recNo += 2;

                    string BranchName = ctx.CoProfiles.Find(companyCode, currBranch.value).CompanyName;

                    ws.Cell("A" + recNo).Value = "Dealer : " + CompanyName;
                    ws.Cell("A" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    ws.Cell("G" + recNo).Value = "Outlet : " + BranchName;
                    ws.Cell("G" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("G" + recNo).Style.Font.Bold = true;

                    recNo++;

                    ws.Cell("A" + recNo).Value = "Status Konsumen : " + (statusKonsumen == "%" ? "ALL" : statusKonsumen == "A" ? "Individu" : "Fleet/Perusahaan");
                    ws.Cell("A" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("A" + recNo).Style.Font.Bold = true;

                    recNo++;

                    ws.Cell("A" + recNo).Value = "Date : " + dateStart.ToString("dd MMM yyyy") + " s/d " + dateEnd.ToString("dd MMM yyyy");
                    ws.Cell("A" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    recNo++;

                    ws.Cell("A" + recNo).Value = "Print Date : " + DateTime.UtcNow.ToString("dd MMM yyyy");
                    ws.Cell("A" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    recNo++;

                    foreach (var row in qry)
                    {
                        ws.Cell("A" + recNo).Value = row.QuestionAnswer;
                        ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("B" + recNo).Value = row.Model;
                        ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("C" + recNo).Value = row.Model2;
                        ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("D" + recNo).Value = row.Model3;
                        ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("E" + recNo).Value = row.Model4;
                        ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("F" + recNo).Value = row.Model5;
                        ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("G" + recNo).Value = row.Model6;
                        ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("H" + recNo).Value = row.Total;
                        ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        if (row.Style == "LGA")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightGray;
                        }
                        else if (row.Style == "LG")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        }
                        else if (row.Style == "BL")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                        }
                        else if (row.Style == "PI")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightPink;
                        }
                        else if (row.Style == "LB")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        }
                        else if (row.Style == "Y")
                        {
                            var Totalt = ws.Range("A" + recNo + ":H" + recNo);
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            Totalt.Style.Fill.BackgroundColor = XLColor.Yellow;
                            Totalt.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            Totalt.Style.Font.Bold = true;
                            Totalt.Style.Font.FontSize = 14;
                            //ws.Cell("B" + recNo).Value = row.Quantity;
                        }

                        ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        ws.Cell("A" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("B" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("C" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("D" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("E" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("F" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("G" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("H" + recNo).Style.Font.FontSize = 10;

                        if (recNo == 7)
                        {
                            ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("D" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("E" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("F" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("G" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("H" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                            ws.Cell("A" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("B" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("C" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("D" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("E" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("F" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("G" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("H" + recNo).Style.Font.FontSize = 11;

                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("B" + recNo).Style.Font.Bold = true;
                            ws.Cell("C" + recNo).Style.Font.Bold = true;
                            ws.Cell("D" + recNo).Style.Font.Bold = true;
                            ws.Cell("E" + recNo).Style.Font.Bold = true;
                            ws.Cell("F" + recNo).Style.Font.Bold = true;
                            ws.Cell("G" + recNo).Style.Font.Bold = true;
                            ws.Cell("H" + recNo).Style.Font.Bold = true;
                        }

                        recNo++;
                    }
                    count++;
                }
            }
            else
            {
                var ws = wb.Worksheets.Add("No Data Exists");
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult QuestionnaireRekapProdByType()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_RekapByType" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var statusKonsumen = Request.Params["StatusKonsumen"] == "" ? "%" : Request.Params["StatusKonsumen"];
            var companyCode = Request.Params["CompanyCode"] ?? "";
            //var branchCode = Request.Params["BranchCode"] ?? "";
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);
            //var reportOption = Request.Params["OutletOption"];

            #region old code by one dealer one outlet
            /*var qry = ctx.Database.SqlQuery<QaProdByTypeModel>("exec uspfn_SpGnQaRekapByType @companyCode=@p0, @branchCode=@p1, @dateStart=@p2, @dateEnd=@p3", companyCode, branchCode, dateStart, dateEnd).AsQueryable();

            var wb = new XLWorkbook();

            var ws = wb.Worksheets.Add("Rekapitulasi By Type");

            //var currBranch = (branchCode != "" && branchCode != "%") ? listbranchs.Where(p => p.value == branchCode).FirstOrDefault() : listbranchs.ElementAt(count);
            //var ws = wb.Worksheets.Add(reportOption == "1" ? "Summary Rekapilasi" : currBranch.value);
            int recNo = 1;

            var rngTable = ws.Range("A2:H2");

            //rngTable.Style
            //    .Font.SetBold()
            //    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
            //    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;
            //rngTable.Style.Fill.BackgroundColor = XLColor.LightGray;

            rngTable.Merge();
            rngTable.Value = "REKAPITULASI By Type";
            rngTable.Style.Font.FontSize = 20;
            rngTable.Style.Font.Underline = XLFontUnderlineValues.Single;
            //rngTable.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

            ws.Columns("1").Width = 60;
            ws.Columns("2").Width = 25;
            ws.Columns("3").Width = 25;
            ws.Columns("4").Width = 25;
            ws.Columns("5").Width = 25;
            ws.Columns("6").Width = 25;
            ws.Columns("7").Width = 25;
            ws.Columns("8").Width = 25;

            //ws.Cell("B" + recNo).Value = "REKAPILASI";
            //ws.Cell("B" + recNo).Style.Font.FontSize = 30;
            //ws.Cell("B" + recNo).Style.Font.Underline = XLFontUnderlineValues.Single;
            //ws.Cell("B" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thick;

            recNo += 2;


            string CompanyName = ctx.Organizations.Find(companyCode).CompanyName;
            string BranchName = ctx.CoProfiles.Find(companyCode, branchCode).CompanyName;

            ws.Cell("A" + recNo).Value = "Dealer : " + CompanyName;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;
            //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            ws.Cell("G" + recNo).Value = "Outlet : " + BranchName;
            ws.Cell("G" + recNo).Style.Font.FontSize = 12;
            ws.Cell("G" + recNo).Style.Font.Bold = true;

            recNo++;

            ws.Cell("A" + recNo).Value = "Date : " + dateStart.ToString("dd MMM yyyy") + " s/d " + dateEnd.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;
            //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            recNo++;

            ws.Cell("A" + recNo).Value = "Print Date : " + DateTime.UtcNow.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;
            //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.QuestionAnswer;
                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Value = row.Model;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("C" + recNo).Value = row.Model2;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("D" + recNo).Value = row.Model3;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("E" + recNo).Value = row.Model4;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("F" + recNo).Value = row.Model5;
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("G" + recNo).Value = row.Model6;
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cell("H" + recNo).Value = row.Total;
                ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                if (row.Style == "LGA")
                {
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightGray;
                }
                else if (row.Style == "LG")
                {
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightGreen;
                }
                else if (row.Style == "BL")
                {
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                }
                else if (row.Style == "PI")
                {
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightPink;
                }
                else if (row.Style == "LB")
                {
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightBlue;
                }
                else if (row.Style == "Y")
                {
                    var Totalt = ws.Range("A" + recNo + ":H" + recNo);
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    Totalt.Style.Fill.BackgroundColor = XLColor.Yellow;
                    Totalt.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    Totalt.Style.Font.Bold = true;
                    Totalt.Style.Font.FontSize = 14;
                    //ws.Cell("B" + recNo).Value = row.Quantity;
                }

                ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                ws.Cell("A" + recNo).Style.Font.FontSize = 10;
                ws.Cell("B" + recNo).Style.Font.FontSize = 10;
                ws.Cell("C" + recNo).Style.Font.FontSize = 10;
                ws.Cell("D" + recNo).Style.Font.FontSize = 10;
                ws.Cell("E" + recNo).Style.Font.FontSize = 10;
                ws.Cell("F" + recNo).Style.Font.FontSize = 10;
                ws.Cell("G" + recNo).Style.Font.FontSize = 10;
                ws.Cell("H" + recNo).Style.Font.FontSize = 10;

                if (recNo == 6)
                {
                    ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Cell("D" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Cell("E" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Cell("F" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Cell("G" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                    ws.Cell("H" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    ws.Cell("A" + recNo).Style.Font.FontSize = 11;
                    ws.Cell("B" + recNo).Style.Font.FontSize = 11;
                    ws.Cell("C" + recNo).Style.Font.FontSize = 11;
                    ws.Cell("D" + recNo).Style.Font.FontSize = 11;
                    ws.Cell("E" + recNo).Style.Font.FontSize = 11;
                    ws.Cell("F" + recNo).Style.Font.FontSize = 11;
                    ws.Cell("G" + recNo).Style.Font.FontSize = 11;
                    ws.Cell("H" + recNo).Style.Font.FontSize = 11;

                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    ws.Cell("B" + recNo).Style.Font.Bold = true;
                    ws.Cell("C" + recNo).Style.Font.Bold = true;
                    ws.Cell("D" + recNo).Style.Font.Bold = true;
                    ws.Cell("E" + recNo).Style.Font.Bold = true;
                    ws.Cell("F" + recNo).Style.Font.Bold = true;
                    ws.Cell("G" + recNo).Style.Font.Bold = true;
                    ws.Cell("H" + recNo).Style.Font.Bold = true;
                }

                recNo++;
            }*/
            #endregion



            List<IQueryable<QaProdByTypeModel>> listIqueryable = new List<IQueryable<QaProdByTypeModel>>();

            IQueryable<QaProdByTypeModel> z = null;

            var qryz = z;

            var listbranchs = ctx.Database.SqlQuery<qaBranchModel>("exec uspfn_SpGetBranchesByCompanyCode @companyCode=@p0", companyCode).AsQueryable().OrderBy(p => p.value);

            foreach (var row in listbranchs.OrderBy(p => p.value))
            {
                var bcode = row.value;
                qryz = ctx.Database.SqlQuery<QaProdByTypeModel>("exec uspfn_SpGnQaRekapByType @companyCode=@p0, @branchCode=@p1, @dateStart=@p2, @dateEnd=@p3, @statusKonsumen=@p4", companyCode, bcode, dateStart, dateEnd, statusKonsumen).AsQueryable();
                listIqueryable.Add(qryz);
            }

            var wb = new XLWorkbook();

            if (listIqueryable != null && listIqueryable.Count > 0)
            {
                int count = 0;
                foreach (var qry in listIqueryable)
                {
                    var currBranch = listbranchs.ElementAt(count);
                    var ws = wb.Worksheets.Add(currBranch.value);

                    //qry = ctx.Database.SqlQuery<QaProdByTypeModel>("exec uspfn_SpGnQaRekapByType @companyCode=@p0, @branchCode=@p1, @dateStart=@p2, @dateEnd=@p3", companyCode, branchCode, dateStart, dateEnd).AsQueryable();

                    //var wb = new XLWorkbook();

                    //var ws = wb.Worksheets.Add("Rekapitulasi By Type");

                    //var currBranch = (branchCode != "" && branchCode != "%") ? listbranchs.Where(p => p.value == branchCode).FirstOrDefault() : listbranchs.ElementAt(count);
                    //var ws = wb.Worksheets.Add(reportOption == "1" ? "Summary Rekapilasi" : currBranch.value);
                    int recNo = 1;

                    var rngTable = ws.Range("A2:H2");

                    //rngTable.Style
                    //    .Font.SetBold()
                    //    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                    //    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;
                    rngTable.Style.Font.FontColor = XLColor.Black;
                    //rngTable.Style.Fill.BackgroundColor = XLColor.LightGray;

                    rngTable.Merge();
                    rngTable.Value = "REKAPITULASI By Type";
                    rngTable.Style.Font.FontSize = 20;
                    rngTable.Style.Font.Underline = XLFontUnderlineValues.Single;
                    //rngTable.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                    ws.Columns("1").Width = 60;
                    ws.Columns("2").Width = 25;
                    ws.Columns("3").Width = 25;
                    ws.Columns("4").Width = 25;
                    ws.Columns("5").Width = 25;
                    ws.Columns("6").Width = 25;
                    ws.Columns("7").Width = 25;
                    ws.Columns("8").Width = 25;

                    //ws.Cell("B" + recNo).Value = "REKAPILASI";
                    //ws.Cell("B" + recNo).Style.Font.FontSize = 30;
                    //ws.Cell("B" + recNo).Style.Font.Underline = XLFontUnderlineValues.Single;
                    //ws.Cell("B" + recNo).Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    recNo += 2;

                    string BranchName = ctx.CoProfiles.Find(companyCode, currBranch.value).CompanyName;
                    string CompanyName = ctx.CoProfiles.Find(companyCode, listbranchs.ElementAt(0).value).CompanyName;

                    ws.Cell("A" + recNo).Value = "Dealer : " + CompanyName;
                    ws.Cell("A" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    ws.Cell("G" + recNo).Value = "Outlet : " + BranchName;
                    ws.Cell("G" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("G" + recNo).Style.Font.Bold = true;

                    recNo++;

                    ws.Cell("A" + recNo).Value = "Status Konsumen : " + (statusKonsumen == "%" ? "ALL" : statusKonsumen == "A" ? "Individu" : "Fleet/Perusahaan");
                    ws.Cell("A" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("A" + recNo).Style.Font.Bold = true;

                    recNo++;

                    ws.Cell("A" + recNo).Value = "Date : " + dateStart.ToString("dd MMM yyyy") + " s/d " + dateEnd.ToString("dd MMM yyyy");
                    ws.Cell("A" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    recNo++;

                    ws.Cell("A" + recNo).Value = "Print Date : " + DateTime.UtcNow.ToString("dd MMM yyyy");
                    ws.Cell("A" + recNo).Style.Font.FontSize = 12;
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    //ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;

                    recNo++;

                    foreach (var row in qry)
                    {
                        ws.Cell("A" + recNo).Value = row.QuestionAnswer;
                        ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("B" + recNo).Value = row.Model;
                        ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("C" + recNo).Value = row.Model2;
                        ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("D" + recNo).Value = row.Model3;
                        ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("E" + recNo).Value = row.Model4;
                        ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("F" + recNo).Value = row.Model5;
                        ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("G" + recNo).Value = row.Model6;
                        ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell("H" + recNo).Value = row.Total;
                        ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        if (row.Style == "LGA")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightGray;
                        }
                        else if (row.Style == "LG")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        }
                        else if (row.Style == "BL")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                        }
                        else if (row.Style == "PI")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightPink;
                        }
                        else if (row.Style == "LB")
                        {
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("A" + recNo).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        }
                        else if (row.Style == "Y")
                        {
                            var Totalt = ws.Range("A" + recNo + ":H" + recNo);
                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            Totalt.Style.Fill.BackgroundColor = XLColor.Yellow;
                            Totalt.Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            Totalt.Style.Font.Bold = true;
                            Totalt.Style.Font.FontSize = 14;
                            //ws.Cell("B" + recNo).Value = row.Quantity;
                        }

                        ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        ws.Cell("A" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("B" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("C" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("D" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("E" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("F" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("G" + recNo).Style.Font.FontSize = 10;
                        ws.Cell("H" + recNo).Style.Font.FontSize = 10;

                        if (recNo == 7)
                        {
                            ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("D" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("E" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("F" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("G" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
                            ws.Cell("H" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;

                            ws.Cell("A" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("B" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("C" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("D" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("E" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("F" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("G" + recNo).Style.Font.FontSize = 11;
                            ws.Cell("H" + recNo).Style.Font.FontSize = 11;

                            ws.Cell("A" + recNo).Style.Font.Bold = true;
                            ws.Cell("B" + recNo).Style.Font.Bold = true;
                            ws.Cell("C" + recNo).Style.Font.Bold = true;
                            ws.Cell("D" + recNo).Style.Font.Bold = true;
                            ws.Cell("E" + recNo).Style.Font.Bold = true;
                            ws.Cell("F" + recNo).Style.Font.Bold = true;
                            ws.Cell("G" + recNo).Style.Font.Bold = true;
                            ws.Cell("H" + recNo).Style.Font.Bold = true;
                        }

                        recNo++;
                    }
                    count++;
                }
            }
            else
            {
                var ws = wb.Worksheets.Add("No Data Exists");
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult generateQaRowData()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_Row_Data" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var statusKonsumen = Request.Params["StatusKonsumen"] == "" ? "%" : Request.Params["StatusKonsumen"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var qry = ctx.Database.SqlQuery<QaRowModel>("exec uspfn_SpGenerateQaRowData @dateStart=@p0, @dateEnd=@p1, @statusKonsumen=@p2", dateStart, dateEnd, statusKonsumen).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Qa_Row_Data");

            var rngTable = ws.Range("A1:AI1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.WhiteSmoke;

            ws.Columns("1").Width = 15;
            ws.Columns("2", "35").Width = 50;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Outlet";
            ws.Cell("D" + recNo).Value = "Input Date";
            ws.Cell("E" + recNo).Value = "Chassis Code";
            ws.Cell("F" + recNo).Value = "Chassis No";
            ws.Cell("G" + recNo).Value = "Responden Name";
            ws.Cell("H" + recNo).Value = "Responden Phone";
            ws.Cell("I" + recNo).Value = "Age";
            ws.Cell("J" + recNo).Value = "Gender";
            ws.Cell("K" + recNo).Value = "Payment";
            ws.Cell("L" + recNo).Value = "Sales Model";
            ws.Cell("M" + recNo).Value = "(1)Responden Status";
            ws.Cell("N" + recNo).Value = "(2)Replacement Merk";
            ws.Cell("O" + recNo).Value = "(2)Replacement Type";
            ws.Cell("P" + recNo).Value = "(2)Replacement Reason";
            ws.Cell("Q" + recNo).Value = "(3)Additional Suzuki";
            ws.Cell("R" + recNo).Value = "(3)Additional Daihatsu";
            ws.Cell("S" + recNo).Value = "(3)Additional Mitsubishi";
            ws.Cell("T" + recNo).Value = "(3)Additional Other";
            ws.Cell("U" + recNo).Value = "(3)Additional Total";
            ws.Cell("V" + recNo).Value = "(4)Load Capacity";
            ws.Cell("W" + recNo).Value = "(5)Annual Drive";
            ws.Cell("X" + recNo).Value = "(6)First Time";
            ws.Cell("Y" + recNo).Value = "(7)Occupation";
            ws.Cell("Z" + recNo).Value = "(7)Occupation Detail";
            ws.Cell("AA" + recNo).Value = "(8)Passenger Car";
            ws.Cell("AB" + recNo).Value = "(8)Passenger Car Unit";
            ws.Cell("AC" + recNo).Value = "(8)Passenger Car Toyota";
            ws.Cell("AD" + recNo).Value = "(8)Passenger Car Honda";
            ws.Cell("AE" + recNo).Value = "(8)Passenger Car Daihatsu";
            ws.Cell("AF" + recNo).Value = "(8)Passenger Car Suzuki";
            ws.Cell("AG" + recNo).Value = "(8)Passenger Car Others";
            ws.Cell("AH" + recNo).Value = "(8)Motor Cycle";
            ws.Cell("AI" + recNo).Value = "Status Konsumen";

            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("H" + recNo).Style.NumberFormat.Format = "@";

                //ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("B" + recNo).Value = row.CompanyName;
                ws.Cell("C" + recNo).Value = row.BranchName;
                ws.Cell("D" + recNo).Value = row.TransactionDate.ToString("dd/MM/yyyy");
                ws.Cell("E" + recNo).Value = row.ChassisCode;
                ws.Cell("F" + recNo).Value = row.ChassisNo;
                ws.Cell("G" + recNo).Value = row.RespondenName;
                ws.Cell("H" + recNo).Value = row.RespondenPhone;
                ws.Cell("I" + recNo).Value = row.RespondenAge;
                ws.Cell("J" + recNo).Value = row.RespondenGender;
                ws.Cell("K" + recNo).Value = row.IsCredit;
                ws.Cell("L" + recNo).Value = row.SalesModelReport;
                ws.Cell("M" + recNo).Value = row.RespondenStatusDescE;
                ws.Cell("N" + recNo).Value = row.IsReplacementMerkDescE;
                ws.Cell("O" + recNo).Value = row.IsReplacementTypeDescE;
                ws.Cell("P" + recNo).Value = row.IsReplacementReasonDescE;
                ws.Cell("Q" + recNo).Value = row.IsAdditionalSuzuki;
                ws.Cell("R" + recNo).Value = row.IsAdditionalDaihatsu;
                ws.Cell("S" + recNo).Value = row.IsAdditionalMitsubishi;
                ws.Cell("T" + recNo).Value = row.IsAdditionalOther;
                ws.Cell("U" + recNo).Value = row.IsAdditionalTotal;
                ws.Cell("V" + recNo).Value = row.LoadCapacityDescE;
                ws.Cell("W" + recNo).Value = row.AnnualDriveDescE;
                ws.Cell("X" + recNo).Value = row.FirstTimeDescE;
                ws.Cell("Y" + recNo).Value = row.OccupationDescE;
                ws.Cell("Z" + recNo).Value = row.OccupationDetailDescE;
                ws.Cell("AA" + recNo).Value = row.PassengerCarDescE;
                ws.Cell("AB" + recNo).Value = row.PassengerCarUnitDescE;
                ws.Cell("AC" + recNo).Value = row.IsPassengerCarToyota;
                ws.Cell("AD" + recNo).Value = row.IsPassengerCarHonda;
                ws.Cell("AE" + recNo).Value = row.IsPassengerCarDaihatsu;
                ws.Cell("AF" + recNo).Value = row.IsPassengerCarSuzuki;
                ws.Cell("AG" + recNo).Value = row.IsPassengerCarOthers;
                ws.Cell("AH" + recNo).Value = row.MotorCycleDescE;
                ws.Cell("AI" + recNo).Value = row.StatusKonsumenDescE;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult generateQaRowDataDealer()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_Row_Data" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var statusKonsumen = Request.Params["StatusKonsumen"] == "" ? "%" : Request.Params["StatusKonsumen"];
            var companyCode = CompanyCode ?? "";
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            List<IQueryable<QaRowModel>> listIqueryable = new List<IQueryable<QaRowModel>>();

            IQueryable<QaRowModel> z = null;

            var qryz = z;

            var listbranchs = ctx.Database.SqlQuery<qaBranchModel>("exec uspfn_SpGetBranchesByCompanyCode @companyCode=@p0", companyCode).AsQueryable().OrderBy(p => p.value);

            foreach (var row in listbranchs.OrderBy(p => p.value))
            {
                var bcode = row.value;
                qryz = ctx.Database.SqlQuery<QaRowModel>("exec uspfn_SpGenerateQaRowData @companyCode=@p0, @branchCode=@p1, @dateStart=@p2, @dateEnd=@p3, @statusKonsumen=@p4", companyCode, bcode, dateStart, dateEnd, statusKonsumen).AsQueryable();
                listIqueryable.Add(qryz);
            }

            var wb = new XLWorkbook();

            if (listIqueryable != null && listIqueryable.Count > 0)
            {
                int count = 0;
                foreach (var qry in listIqueryable)
                {
                    var currBranch = listbranchs.ElementAt(count);
                    var ws = wb.Worksheets.Add(currBranch.value);

                    //var qry = ctx.Database.SqlQuery<QaRowModel>("exec uspfn_SpGenerateQaRowData @dateStart=@p0, @dateEnd=@p1", dateStart, dateEnd).AsQueryable();

                    int recNo = 1;

                    //var wb = new XLWorkbook();
                    //var ws = wb.Worksheets.Add("Qa_Row_Data");

                    var rngTable = ws.Range("A1:AI1");

                    rngTable.Style
                        .Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;
                    rngTable.Style.Font.FontColor = XLColor.WhiteSmoke;

                    ws.Columns("1").Width = 15;
                    ws.Columns("2", "35").Width = 50;

                    //First Names                                              
                    ws.Cell("A" + recNo).Value = "Area";
                    ws.Cell("B" + recNo).Value = "Dealer";
                    ws.Cell("C" + recNo).Value = "Outlet";
                    ws.Cell("D" + recNo).Value = "Input Date";
                    ws.Cell("E" + recNo).Value = "Chassis Code";
                    ws.Cell("F" + recNo).Value = "Chassis No";
                    ws.Cell("G" + recNo).Value = "Responden Name";
                    ws.Cell("H" + recNo).Value = "Responden Phone";
                    ws.Cell("I" + recNo).Value = "Age";
                    ws.Cell("J" + recNo).Value = "Gender";
                    ws.Cell("K" + recNo).Value = "Payment";
                    ws.Cell("L" + recNo).Value = "Sales Model";
                    ws.Cell("M" + recNo).Value = "(1)Responden Status";
                    ws.Cell("N" + recNo).Value = "(2)Replacement Merk";
                    ws.Cell("O" + recNo).Value = "(2)Replacement Type";
                    ws.Cell("P" + recNo).Value = "(2)Replacement Reason";
                    ws.Cell("Q" + recNo).Value = "(3)Additional Suzuki";
                    ws.Cell("R" + recNo).Value = "(3)Additional Daihatsu";
                    ws.Cell("S" + recNo).Value = "(3)Additional Mitsubishi";
                    ws.Cell("T" + recNo).Value = "(3)Additional Other";
                    ws.Cell("U" + recNo).Value = "(3)Additional Total";
                    ws.Cell("V" + recNo).Value = "(4)Load Capacity";
                    ws.Cell("W" + recNo).Value = "(5)Annual Drive";
                    ws.Cell("X" + recNo).Value = "(6)First Time";
                    ws.Cell("Y" + recNo).Value = "(7)Occupation";
                    ws.Cell("Z" + recNo).Value = "(7)Occupation Detail";
                    ws.Cell("AA" + recNo).Value = "(8)Passenger Car";
                    ws.Cell("AB" + recNo).Value = "(8)Passenger Car Unit";
                    ws.Cell("AC" + recNo).Value = "(8)Passenger Car Toyota";
                    ws.Cell("AD" + recNo).Value = "(8)Passenger Car Honda";
                    ws.Cell("AE" + recNo).Value = "(8)Passenger Car Daihatsu";
                    ws.Cell("AF" + recNo).Value = "(8)Passenger Car Suzuki";
                    ws.Cell("AG" + recNo).Value = "(8)Passenger Car Others";
                    ws.Cell("AH" + recNo).Value = "(8)Motor Cycle";
                    ws.Cell("AI" + recNo).Value = "Status Konsumen";

                    recNo++;

                    foreach (var row in qry)
                    {
                        ws.Cell("H" + recNo).Style.NumberFormat.Format = "@";

                        //ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("A" + recNo).Value = row.Area;
                        ws.Cell("B" + recNo).Value = row.CompanyName;
                        ws.Cell("C" + recNo).Value = row.BranchName;
                        ws.Cell("D" + recNo).Value = row.TransactionDate.ToString("dd/MM/yyyy");
                        ws.Cell("E" + recNo).Value = row.ChassisCode;
                        ws.Cell("F" + recNo).Value = row.ChassisNo;
                        ws.Cell("G" + recNo).Value = row.RespondenName;
                        ws.Cell("H" + recNo).Value = row.RespondenPhone;
                        ws.Cell("I" + recNo).Value = row.RespondenAge;
                        ws.Cell("J" + recNo).Value = row.RespondenGender;
                        ws.Cell("K" + recNo).Value = row.IsCredit;
                        ws.Cell("L" + recNo).Value = row.SalesModelReport;
                        ws.Cell("M" + recNo).Value = row.RespondenStatusDescE;
                        ws.Cell("N" + recNo).Value = row.IsReplacementMerkDescE;
                        ws.Cell("O" + recNo).Value = row.IsReplacementTypeDescE;
                        ws.Cell("P" + recNo).Value = row.IsReplacementReasonDescE;
                        ws.Cell("Q" + recNo).Value = row.IsAdditionalSuzuki;
                        ws.Cell("R" + recNo).Value = row.IsAdditionalDaihatsu;
                        ws.Cell("S" + recNo).Value = row.IsAdditionalMitsubishi;
                        ws.Cell("T" + recNo).Value = row.IsAdditionalOther;
                        ws.Cell("U" + recNo).Value = row.IsAdditionalTotal;
                        ws.Cell("V" + recNo).Value = row.LoadCapacityDescE;
                        ws.Cell("W" + recNo).Value = row.AnnualDriveDescE;
                        ws.Cell("X" + recNo).Value = row.FirstTimeDescE;
                        ws.Cell("Y" + recNo).Value = row.OccupationDescE;
                        ws.Cell("Z" + recNo).Value = row.OccupationDetailDescE;
                        ws.Cell("AA" + recNo).Value = row.PassengerCarDescE;
                        ws.Cell("AB" + recNo).Value = row.PassengerCarUnitDescE;
                        ws.Cell("AC" + recNo).Value = row.IsPassengerCarToyota;
                        ws.Cell("AD" + recNo).Value = row.IsPassengerCarHonda;
                        ws.Cell("AE" + recNo).Value = row.IsPassengerCarDaihatsu;
                        ws.Cell("AF" + recNo).Value = row.IsPassengerCarSuzuki;
                        ws.Cell("AG" + recNo).Value = row.IsPassengerCarOthers;
                        ws.Cell("AH" + recNo).Value = row.MotorCycleDescE;
                        ws.Cell("AI" + recNo).Value = row.StatusKonsumenDescE;

                        recNo++;
                    }
                    count++;
                }
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult QuestionnaireRekapProdSummary()
        {
            DateTime now = DateTime.Now;
            string fileName = "Qa_RekapSummary" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var statusKonsumen = Request.Params["StatusKonsumen"] == "" ? "%" : Request.Params["StatusKonsumen"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var areas = ctx.GroupAreas.Select(x => new { AreaDealer = x.AreaDealer, GroupNo = x.GroupNo }).OrderBy(x => x.GroupNo).ToList();

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Rekapitulasi Summary");

            int recNo = 1;

            var rngTable = ws.Range("B1:E1");


            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;

            rngTable.Merge();
            rngTable.Value = "SUMMARY";
            rngTable.Style.Font.FontSize = 20;
            rngTable.Style.Font.Underline = XLFontUnderlineValues.Single;

            ws.Columns("1").Width = 50;
            ws.Columns("2", "55").Width = 17;

            recNo += 2;

            ws.Cell("A" + recNo).Value = "Status Konsumen : " + (statusKonsumen == "%" ? "ALL" : statusKonsumen == "A" ? "Individu" : "Fleet/Perusahaan");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            ws.Cell("A" + recNo).Value = "Periode : " + dateStart.ToString("dd MMM yyyy") + " s/d " + dateEnd.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            ws.Cell("A" + recNo).Value = "Print Date : " + DateTime.UtcNow.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            #region loop insert summary
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_SpGnQaRekapSummary";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("areacode", "");
            cmd.Parameters.AddWithValue("dateStart", dateStart);
            cmd.Parameters.AddWithValue("dateEnd", dateEnd);
            cmd.Parameters.AddWithValue("statusKonsumen", statusKonsumen);
            cmd.Parameters.AddWithValue("isDelete", true);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            #endregion

            #region loop per area get summary
            int counter = 0;
            foreach (var item in areas)
            {
                List<string> listCell = new List<string>();
                if (counter == 0)
                {
                    listCell.Add("A");
                    listCell.Add("B");
                    listCell.Add("C");
                    listCell.Add("D");
                    listCell.Add("E");
                    listCell.Add("F");
                    listCell.Add("G");
                }
                else if (counter == 1)
                {
                    listCell.Add("H");
                    listCell.Add("I");
                    listCell.Add("J");
                    listCell.Add("K");
                    listCell.Add("L");
                    listCell.Add("M");
                }
                else if (counter == 2)
                {
                    listCell.Add("N");
                    listCell.Add("O");
                    listCell.Add("P");
                    listCell.Add("Q");
                    listCell.Add("R");
                    listCell.Add("S");
                }
                else if (counter == 3)
                {
                    listCell.Add("T");
                    listCell.Add("U");
                    listCell.Add("V");
                    listCell.Add("W");
                    listCell.Add("X");
                    listCell.Add("Y");
                }
                else if (counter == 4)
                {
                    listCell.Add("Z");
                    listCell.Add("AA");
                    listCell.Add("AB");
                    listCell.Add("AC");
                    listCell.Add("AD");
                    listCell.Add("AE");
                }
                else if (counter == 5)
                {
                    listCell.Add("AF");
                    listCell.Add("AG");
                    listCell.Add("AH");
                    listCell.Add("AI");
                    listCell.Add("AJ");
                    listCell.Add("AK");
                }
                else if (counter == 6)
                {
                    listCell.Add("AL");
                    listCell.Add("AM");
                    listCell.Add("AN");
                    listCell.Add("AO");
                    listCell.Add("AP");
                    listCell.Add("AQ");
                }
                else if (counter == 7)
                {
                    listCell.Add("AR");
                    listCell.Add("AS");
                    listCell.Add("AT");
                    listCell.Add("AU");
                    listCell.Add("AV");
                    listCell.Add("AW");
                }
                else if (counter == 8)
                {
                    listCell.Add("AX");
                    listCell.Add("AY");
                    listCell.Add("AZ");
                    listCell.Add("BA");
                    listCell.Add("BB");
                    listCell.Add("BC");
                }

                string groupno = areas[counter].GroupNo;

                recNo = 5;

                var qry = ctx.Database.SqlQuery<QaProdByTypeModel>("exec uspfn_SpGetQaProdSummary @groupNo=@p0, @statusKonsumen=@p1", groupno, statusKonsumen).AsQueryable();

                foreach (var row in qry)
                {
                    if (counter == 0)
                    {
                        if (row.Num.ToString().ToUpper() == "AREA")
                        {
                            var rngTable2 = ws.Range(listCell[1] + recNo + ":" + listCell[6] + recNo);
                            rngTable2.Merge();

                            rngTable2.Value = row.Model;
                            rngTable2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            if (row.Style == "LB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightBlue;
                            }
                            else if (row.Style == "LG")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightGreen;
                            }
                            else if (row.Style == "LPI")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightPink;
                            }
                            else if (row.Style == "LSB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                            }
                            else if (row.Style == "LSPI")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSalmonPink;
                            }
                            else if (row.Style == "LSTB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
                            }
                            else if (row.Style == "OR")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.OrangeColorWheel;
                            }
                            else if (row.Style == "LPP")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightPastelPurple;
                            }
                            else if (row.Style == "YL")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightYellow;
                            }

                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                        }
                        else
                        {
                            ws.Cell(listCell[1] + recNo).Value = row.Model;
                            ws.Cell(listCell[1] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        if (row.Num.ToString().ToUpper() == "TYPE")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[6] + recNo);
                            rngTable2.Style.Fill.BackgroundColor = XLColor.LightSlateGray;
                            rngTable2.Style.Font.Bold = true;
                        }

                        if (row.Style.ToString().ToUpper() == "ALLGREY")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[6] + recNo);
                            rngTable2.Style.Fill.BackgroundColor = XLColor.LightGray;
                            rngTable2.Style.Font.Bold = true;
                        }

                        if (row.Style.ToString().ToUpper() == "ALLYELLOW")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[6] + recNo);
                            rngTable2.Style.Fill.BackgroundColor = XLColor.Yellow;
                            rngTable2.Style.Font.Bold = true;
                        }

                        ws.Cell(listCell[0] + recNo).Value = row.QuestionAnswer;
                        ws.Cell(listCell[0] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell(listCell[2] + recNo).Value = row.Model2;
                        ws.Cell(listCell[2] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(listCell[3] + recNo).Value = row.Model3;
                        ws.Cell(listCell[3] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(listCell[4] + recNo).Value = row.Model4;
                        ws.Cell(listCell[4] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(listCell[5] + recNo).Value = row.Model5;
                        ws.Cell(listCell[5] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(listCell[6] + recNo).Value = row.Model6;
                        ws.Cell(listCell[6] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        if (row.Style == "LGA")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                            ws.Cell(listCell[0] + recNo).Style.Fill.BackgroundColor = XLColor.LightGray;
                        }
                        else if (row.Style == "LG")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                            ws.Cell(listCell[0] + recNo).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        }
                        else if (row.Style == "BL")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                        }
                        else if (row.Style == "PI")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                            ws.Cell(listCell[0] + recNo).Style.Fill.BackgroundColor = XLColor.LightPink;
                        }
                        else if (row.Style == "LB")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                            ws.Cell(listCell[0] + recNo).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        }
                        else if (row.Style == "YA")
                        {
                            var ALlCell = ws.Range(listCell[0] + recNo + ":" + listCell[6] + recNo);
                            ALlCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                        }

                        ws.Cell(listCell[0] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[1] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[2] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[3] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[4] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[5] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[6] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        ws.Cell(listCell[0] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[1] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[2] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[3] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[4] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[5] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[6] + recNo).Style.Font.FontSize = 10;
                    }
                    else
                    {
                        if (row.Num.ToString().ToUpper() == "AREA")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[5] + recNo);
                            rngTable2.Merge();

                            if (row.Style == "LB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightBlue;
                            }
                            else if (row.Style == "LG")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightGreen;
                            }
                            else if (row.Style == "LPI")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightPink;
                            }
                            else if (row.Style == "LSB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                            }
                            else if (row.Style == "LSPI")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSalmonPink;
                            }
                            else if (row.Style == "LSTB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
                            }
                            else if (row.Style == "OR")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.OrangeColorWheel;
                            }
                            else if (row.Style == "LPP")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightPastelPurple;
                            }
                            else if (row.Style == "YL")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.Yellow;
                            }

                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                        }

                        if (row.Num.ToString().ToUpper() == "TYPE")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[5] + recNo);
                            rngTable2.Style.Fill.BackgroundColor = XLColor.LightSlateGray;
                            rngTable2.Style.Font.Bold = true;
                        }

                        if (row.Style.ToString().ToUpper() == "ALLGREY")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[5] + recNo);
                            rngTable2.Style.Fill.BackgroundColor = XLColor.LightGray;
                            rngTable2.Style.Font.Bold = true;
                        }

                        if (row.Style.ToString().ToUpper() == "ALLYELLOW")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[5] + recNo);
                            rngTable2.Style.Fill.BackgroundColor = XLColor.Yellow;
                            rngTable2.Style.Font.Bold = true;
                        }

                        ws.Cell(listCell[0] + recNo).Value = row.Model;
                        ws.Cell(listCell[0] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(listCell[1] + recNo).Value = row.Model2;
                        ws.Cell(listCell[1] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(listCell[2] + recNo).Value = row.Model3;
                        ws.Cell(listCell[2] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(listCell[3] + recNo).Value = row.Model4;
                        ws.Cell(listCell[3] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(listCell[4] + recNo).Value = row.Model5;
                        ws.Cell(listCell[4] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(listCell[5] + recNo).Value = row.Model6;
                        ws.Cell(listCell[5] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        if (row.Style == "YA")
                        {
                            var ALlCell = ws.Range(listCell[0] + recNo + ":" + listCell[5] + recNo);
                            ALlCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                        }

                        ws.Cell(listCell[0] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[1] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[2] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[3] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[4] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[5] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        ws.Cell(listCell[0] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[1] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[2] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[3] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[4] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[5] + recNo).Style.Font.FontSize = 10;
                    }

                    recNo++;

                }
                counter++;
            }
            #endregion

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult generateMonitoringPerCust()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_MonitoringPerCust" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var Area = (Request.Params["Area"] ?? "") == "" ? "%" : Request.Params["Area"];
            var CompanyCode = (Request.Params["CompanyCode"] ?? "") == "" ? "%" : Request.Params["CompanyCode"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var CompanyText = Request.Params["CompanyText"] ?? "";

            //var qry = ctx.Database.SqlQuery<QaMonitoringCust>("exec uspfn_SpGetMonitoringPerCust @groupno=@p0, @companyCode=@p1, @dateStart=@p2, @dateEnd=@p3", Area, CompanyCode, dateStart, dateEnd).AsQueryable();
            var qry = ctx.Database.SqlQuery<QaMonitoringCust>("exec uspfn_SpGetMonitoringPerCust_New @groupno=@p0, @company=@p1, @dateStart=@p2, @dateEnd=@p3", Area, CompanyCode, dateStart, dateEnd).AsQueryable();

            var areas = ctx.DealerGroupMappingViews.Select(x => new { AreaDealer = x.Area, GroupNo = x.GroupNo, CompanyCode = x.CompanyCode, CompanyName = x.CompanyName }).OrderBy(x => x.GroupNo).ToList();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Qa_Monitoring_Per_Customer");


            ws.Columns("1").Width = 40;
            ws.Columns("2").Width = 35;
            ws.Columns("3").Width = 15;
            ws.Columns("4").Width = 50;
            ws.Columns("5", "16").Width = 20;

            recNo++;

            ws.Cell("A" + recNo).Value = "Printed Date : " + DateTime.UtcNow.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo += 2;

            ws.Cell("A" + recNo).Value = "Monitoring Input Qnr Pick Up Per Customer";
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo += 2;

            ws.Cell("A" + recNo).Value = "Periode : " + dateStart.ToString("dd MMM yyyy") + " s/d " + dateEnd.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            ws.Cell("A" + recNo).Value = "Area : " + (Area != "%" ? areas.Where(p => p.GroupNo == Area).Select(x => new { Area = x.AreaDealer }).FirstOrDefault().Area.ToString() : "SELECT ALL");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            //ws.Cell("A" + recNo).Value = "Dealer : " + (CompanyCode != "%" ? areas.Where(p => p.CompanyCode == CompanyCode).Select(x => new { CompanyName = x.CompanyName }).FirstOrDefault().CompanyName.ToString() : "SELECT ALL");
            ws.Cell("A" + recNo).Value = "Dealer : " + CompanyText;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo += 2;


            var rngTable = ws.Range("A" + recNo + ":P" + recNo);

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.WhiteSmoke;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Outlet Code";
            ws.Cell("D" + recNo).Value = "Outlet";
            ws.Cell("E" + recNo).Value = "Chassis Code";
            ws.Cell("F" + recNo).Value = "Chassis No";
            ws.Cell("G" + recNo).Value = "Sales Model";
            ws.Cell("H" + recNo).Value = "Responden Name";
            ws.Cell("I" + recNo).Value = "Age";
            ws.Cell("J" + recNo).Value = "Gender";
            ws.Cell("K" + recNo).Value = "Phone";
            ws.Cell("L" + recNo).Value = "Payment";
            ws.Cell("M" + recNo).Value = "Faktur Penjualan Date";
            ws.Cell("N" + recNo).Value = "Permohonan Faktur Polisi Date";
            ws.Cell("O" + recNo).Value = "QnR Input Date";
            ws.Cell("P" + recNo).Value = "Status Konsumen";

            recNo++;

            foreach (var row in qry)
            {
                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("B" + recNo).Value = row.CompanyName;
                ws.Cell("C" + recNo).Value = row.BranchCode;
                ws.Cell("D" + recNo).Value = row.BranchName;
                ws.Cell("E" + recNo).Value = row.ChassisCode;
                ws.Cell("F" + recNo).Value = row.ChassisNo;
                ws.Cell("G" + recNo).Value = row.SalesModel;
                ws.Cell("H" + recNo).Value = row.CustomerName;
                ws.Cell("I" + recNo).Value = row.Age;
                ws.Cell("J" + recNo).Value = row.Gender;
                ws.Cell("K" + recNo).Value = row.ContactNo;
                ws.Cell("L" + recNo).Value = row.Payment;
                ws.Cell("M" + recNo).Value = !String.IsNullOrEmpty(row.FakturPajakDate) ? Convert.ToDateTime(row.FakturPajakDate).ToString("dd/MM/yyyy") : "";
                ws.Cell("N" + recNo).Value = !String.IsNullOrEmpty(row.FakturPolisiDate) ? Convert.ToDateTime(row.FakturPolisiDate).ToString("dd/MM/yyyy") : "";
                ws.Cell("O" + recNo).Value = !String.IsNullOrEmpty(row.TransactionDate) ? Convert.ToDateTime(row.TransactionDate).ToString("dd/MM/yyyy") : "";
                ws.Cell("P" + recNo).Value = row.StatusKonsumenDescI;


                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("N" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("O" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("P" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("M" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("N" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("O" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("P" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                ws.Cell("A" + recNo).Style.Font.FontSize = 10;
                ws.Cell("B" + recNo).Style.Font.FontSize = 10;
                ws.Cell("C" + recNo).Style.Font.FontSize = 10;
                ws.Cell("D" + recNo).Style.Font.FontSize = 10;
                ws.Cell("E" + recNo).Style.Font.FontSize = 10;
                ws.Cell("F" + recNo).Style.Font.FontSize = 10;
                ws.Cell("G" + recNo).Style.Font.FontSize = 10;
                ws.Cell("H" + recNo).Style.Font.FontSize = 10;
                ws.Cell("I" + recNo).Style.Font.FontSize = 10;
                ws.Cell("J" + recNo).Style.Font.FontSize = 10;
                ws.Cell("K" + recNo).Style.Font.FontSize = 10;
                ws.Cell("L" + recNo).Style.Font.FontSize = 10;
                ws.Cell("M" + recNo).Style.Font.FontSize = 10;
                ws.Cell("N" + recNo).Style.Font.FontSize = 10;
                ws.Cell("O" + recNo).Style.Font.FontSize = 10;
                ws.Cell("P" + recNo).Style.Font.FontSize = 10;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult generateMonitoringPerOutlet()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_MonitoringPerOutlet" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var Area = (Request.Params["Area"] ?? "") == "" ? "%" : Request.Params["Area"];
            var CompanyCode = (Request.Params["CompanyCode"] ?? "") == "" ? "%" : Request.Params["CompanyCode"];
            var IncludeZero = (Request.Params["IncludeZero"] ?? "") == "on" ? true : false;
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var CompanyText = Request.Params["CompanyText"] ?? "";

            //var qry = ctx.Database.SqlQuery<QaMonitoringOutlet>("exec uspfn_SpGetMonitoringPerOutlet @groupno=@p0, @companyCode=@p1, @dateStart=@p2, @dateEnd=@p3, @includeZero=@p4", Area, CompanyCode, dateStart, dateEnd, IncludeZero).AsQueryable();
            var qry = ctx.Database.SqlQuery<QaMonitoringOutlet>("exec uspfn_SpGetMonitoringPerOutlet_New @groupno=@p0, @Company=@p1, @dateStart=@p2, @dateEnd=@p3, @includeZero=@p4", Area, CompanyCode, dateStart, dateEnd, IncludeZero).AsQueryable();

            var areas = ctx.DealerGroupMappingViews.Select(x => new { AreaDealer = x.Area, GroupNo = x.GroupNo, CompanyCode = x.CompanyCode, CompanyName = x.CompanyName }).OrderBy(x => x.GroupNo).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Qa_Monitoring_Per_Outlet");

            ws.Columns("1").Width = 15;
            ws.Columns("2").Width = 30;
            ws.Columns("3").Width = 18;
            ws.Columns("4").Width = 50;
            ws.Columns("5", "7").Width = 22;
            ws.Columns("8", "12").Width = 20;

            ws.Cell("A" + recNo).Value = "Printed Date : " + DateTime.UtcNow.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo += 2;

            ws.Cell("A" + recNo).Value = "Monitoring Input Qnr Pick Up Per Outlet";
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo += 2;

            ws.Cell("A" + recNo).Value = "Periode : " + dateStart.ToString("dd MMM yyyy") + " s/d " + dateEnd.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            ws.Cell("A" + recNo).Value = "Area : " + (Area != "%" ? areas.Where(p => p.GroupNo == Area).Select(x => new { Area = x.AreaDealer }).FirstOrDefault().Area.ToString() : "SELECT ALL");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            //ws.Cell("A" + recNo).Value = "Dealer : " + (CompanyCode != "%" ? areas.Where(p => p.CompanyCode == CompanyCode).Select(x => new { CompanyName = x.CompanyName }).FirstOrDefault().CompanyName.ToString() : "SELECT ALL");
            ws.Cell("A" + recNo).Value = "Dealer : " + CompanyText;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo += 2;

            var rngTable = ws.Range("A" + recNo + ":L" + recNo);

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.WhiteSmoke;

            //First Names                                              
            ws.Cell("A" + recNo).Value = "Area";
            ws.Cell("B" + recNo).Value = "Dealer";
            ws.Cell("C" + recNo).Value = "Outlet Code";
            ws.Cell("D" + recNo).Value = "Outlet";
            ws.Cell("E" + recNo).Value = "PIC Outlet";
            ws.Cell("F" + recNo).Value = "No. HP";
            ws.Cell("G" + recNo).Value = "Email";
            ws.Cell("H" + recNo).Value = "Faktur Penjualan";
            ws.Cell("I" + recNo).Value = "Permohonan Faktur Polisi";
            ws.Cell("J" + recNo).Value = "QnR Input";
            ws.Cell("K" + recNo).Value = "Qty Konsumen Individu";
            ws.Cell("L" + recNo).Value = "Qty Konsumen Fleet";

            recNo++;

            foreach (var row in qry)
            {
                //ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                if (row.GroupNo == "99999")
                {
                    ws.Cell("A" + recNo).Style.Font.Bold = true;
                    ws.Cell("B" + recNo).Style.Font.Bold = true;
                    ws.Cell("C" + recNo).Style.Font.Bold = true;
                    ws.Cell("D" + recNo).Style.Font.Bold = true;
                    ws.Cell("E" + recNo).Style.Font.Bold = true;
                    ws.Cell("F" + recNo).Style.Font.Bold = true;
                    ws.Cell("G" + recNo).Style.Font.Bold = true;
                    ws.Cell("H" + recNo).Style.Font.Bold = true;
                    ws.Cell("I" + recNo).Style.Font.Bold = true;
                    ws.Cell("J" + recNo).Style.Font.Bold = true;
                    ws.Cell("K" + recNo).Style.Font.Bold = true;
                    ws.Cell("L" + recNo).Style.Font.Bold = true;
                }

                ws.Cell("A" + recNo).Value = row.Area;
                ws.Cell("B" + recNo).Value = row.CompanyName;
                ws.Cell("C" + recNo).Value = row.BranchCode;
                ws.Cell("D" + recNo).Value = row.BranchName;
                ws.Cell("E" + recNo).Value = row.PICOutlet;
                ws.Cell("F" + recNo).Value = row.NoHP;
                ws.Cell("G" + recNo).Value = row.Email;
                ws.Cell("H" + recNo).Value = row.FakturPenjualan;
                ws.Cell("I" + recNo).Value = row.PermohonanFakPol;
                ws.Cell("J" + recNo).Value = row.QnrInput;
                ws.Cell("K" + recNo).Value = row.StatusKonsumenIndividu;
                ws.Cell("L" + recNo).Value = row.StatusKonsumenFleet;

                ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("D" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("E" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("F" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("G" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("H" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("I" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("J" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("K" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell("L" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                ws.Cell("A" + recNo).Style.Font.FontSize = 10;
                ws.Cell("B" + recNo).Style.Font.FontSize = 10;
                ws.Cell("C" + recNo).Style.Font.FontSize = 10;
                ws.Cell("D" + recNo).Style.Font.FontSize = 10;
                ws.Cell("E" + recNo).Style.Font.FontSize = 10;
                ws.Cell("F" + recNo).Style.Font.FontSize = 10;
                ws.Cell("G" + recNo).Style.Font.FontSize = 10;
                ws.Cell("H" + recNo).Style.Font.FontSize = 10;
                ws.Cell("I" + recNo).Style.Font.FontSize = 10;
                ws.Cell("J" + recNo).Style.Font.FontSize = 10;
                ws.Cell("K" + recNo).Style.Font.FontSize = 10;
                ws.Cell("L" + recNo).Style.Font.FontSize = 10;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult generateMonitoringPerOutletv2()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_MonitoringPerOutlet" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var Area = (Request.Params["Area"] ?? "") == "" ? "%" : Request.Params["Area"];
            var CompanyCode = (Request.Params["CompanyCode"] ?? "") == "" ? "%" : Request.Params["CompanyCode"];
            var IncludeZero = true;
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var CompanyText = Request.Params["CompanyText"] ?? "All Dealer";

            List<IQueryable<QaMonitoringOutletv2>> listIqueryable = new List<IQueryable<QaMonitoringOutletv2>>();

            IQueryable<QaMonitoringOutletv2> z = null;

            var qry = z;

            DateTime date2 = new DateTime();
            DateTime dateStart2 = new DateTime();

            for (int i = 0; i < 2; i++)
            {
                var date = dateEnd;
                var datee = dateStart;

                if (i == 0)
                {
                    date = DateTime.ParseExact(new DateTime(dateEnd.Year, dateEnd.Month, 1).AddDays(-1).ToString("yyyy-MM-dd") + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);
                    date2 = date;
                }
                else
                {
                    dateStart2 = DateTime.ParseExact(new DateTime(dateEnd.Year, dateEnd.Month, 1).ToString("yyyy-MM-dd") + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
                    datee = dateStart2;
                }

                //qry = ctx.Database.SqlQuery<QaMonitoringOutletv2>("exec uspfn_SpGetMonitoringPerOutlet2 @groupno=@p0, @companyCode=@p1, @dateStart=@p2, @dateEnd=@p3, @includeZero=@p4", Area, CompanyCode, datee, date, IncludeZero).AsQueryable();
                qry = ctx.Database.SqlQuery<QaMonitoringOutletv2>("exec uspfn_SpGetMonitoringPerOutletV2_New @groupno=@p0, @Company=@p1, @dateStart=@p2, @dateEnd=@p3, @includeZero=@p4", Area, CompanyCode, datee, date, IncludeZero).AsQueryable();

                listIqueryable.Add(qry);
            }

            var areas = ctx.DealerGroupMappingViews.Select(x => new { AreaDealer = x.Area, GroupNo = x.GroupNo, CompanyCode = x.CompanyCode, CompanyName = x.CompanyName }).OrderBy(x => x.GroupNo).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Qa_Monitoring_Per_Outletv2");

            ws.Columns("1").Width = 48;
            ws.Columns("2").Width = 30;
            ws.Columns("3").Width = 18;
            ws.Columns("4").Width = 50;
            ws.Columns("5", "7").Width = 22;
            ws.Columns("8", "14").Width = 20;

            ws.Cell("A" + recNo).Value = "Printed Date : " + DateTime.UtcNow.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo += 2;

            ws.Cell("A" + recNo).Value = "Monitoring Input Qnr Pick Up Per Outlet v2";
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            //recNo += 2;

            //ws.Cell("A" + recNo).Value = "Periode : " + dateStart.ToString("dd MMM yyyy") + " s/d " + dateEnd.ToString("dd MMM yyyy");
            //ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            //ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            ws.Cell("A" + recNo).Value = "Area : " + (Area != "%" ? areas.Where(p => p.GroupNo == Area).Select(x => new { Area = x.AreaDealer }).FirstOrDefault().Area.ToString() : "SELECT ALL");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            //ws.Cell("A" + recNo).Value = "Dealer : " + (CompanyCode != "%" ? areas.Where(p => p.CompanyCode == CompanyCode).Select(x => new { CompanyName = x.CompanyName }).FirstOrDefault().CompanyName.ToString() : "SELECT ALL");
            ws.Cell("A" + recNo).Value = "Dealer : " + CompanyText;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo += 2;

            var rngTable = ws.Range("A" + recNo + ":N" + (recNo + 1));

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.WhiteSmoke;

            ws.Range("A" + recNo + ":A" + (recNo + 1)).Merge().Value = "Outlet";
            ws.Range("B" + recNo + ":B" + (recNo + 1)).Merge().Value = "PIC Outlet";
            ws.Range("C" + recNo + ":C" + (recNo + 1)).Merge().Value = "No. HP";
            ws.Range("D" + recNo + ":D" + (recNo + 1)).Merge().Value = "Email";
            ws.Range("E" + recNo + ":I" + recNo).Merge().Value = dateStart.ToString("dd MMM yyyy") + " - " + date2.ToString("dd MMM yyyy");
            ws.Range("E" + (recNo + 1) + ":E" + (recNo + 1)).Merge().Value = "Faktur Penjualan";
            ws.Range("F" + (recNo + 1) + ":F" + (recNo + 1)).Merge().Value = "Permohonan Faktur Polisi";
            ws.Range("G" + (recNo + 1) + ":G" + (recNo + 1)).Merge().Value = "QnR Input";
            ws.Range("H" + (recNo + 1) + ":H" + (recNo + 1)).Merge().Value = "% Faktur Penjualan";
            ws.Range("I" + (recNo + 1) + ":I" + (recNo + 1)).Merge().Value = "% Permohonan Faktur Polisi";
            ws.Range("J" + recNo + ":N" + recNo).Merge().Value = dateStart2.ToString("dd MMM yyyy") + " - " + dateEnd.ToString("dd MMM yyyy");
            ws.Range("J" + (recNo + 1) + ":J" + (recNo + 1)).Merge().Value = "Faktur Penjualan";
            ws.Range("K" + (recNo + 1) + ":K" + (recNo + 1)).Merge().Value = "Permohonan Faktur Polisi";
            ws.Range("L" + (recNo + 1) + ":L" + (recNo + 1)).Merge().Value = "QnR Input";
            ws.Range("M" + (recNo + 1) + ":M" + (recNo + 1)).Merge().Value = "% Faktur Penjualan";
            ws.Range("N" + (recNo + 1) + ":N" + (recNo + 1)).Merge().Value = "% Permohonan Faktur Polisi";

            ws.Range("A" + recNo + ":A" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("B" + recNo + ":B" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("C" + recNo + ":C" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("D" + recNo + ":D" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("E" + recNo + ":I" + recNo).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("E" + (recNo + 1) + ":E" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("F" + (recNo + 1) + ":F" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("G" + (recNo + 1) + ":G" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("H" + (recNo + 1) + ":H" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("I" + (recNo + 1) + ":I" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("J" + recNo + ":N" + recNo).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("J" + (recNo + 1) + ":J" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("K" + (recNo + 1) + ":K" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("L" + (recNo + 1) + ":L" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("M" + (recNo + 1) + ":M" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range("N" + (recNo + 1) + ":N" + (recNo + 1)).Merge().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            //recNo+=2;
            int counter = 0;
            foreach (var item in listIqueryable)
            {
                recNo = 9;
                foreach (var row in item)
                {
                    //ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    if (row.GroupNo == "99999")
                    {
                        ws.Cell("A" + recNo).Style.Font.Bold = true;
                        ws.Cell("B" + recNo).Style.Font.Bold = true;
                        ws.Cell("C" + recNo).Style.Font.Bold = true;
                        ws.Cell("D" + recNo).Style.Font.Bold = true;
                        ws.Cell("E" + recNo).Style.Font.Bold = true;
                        ws.Cell("F" + recNo).Style.Font.Bold = true;
                        ws.Cell("G" + recNo).Style.Font.Bold = true;
                        ws.Cell("H" + recNo).Style.Font.Bold = true;
                        ws.Cell("I" + recNo).Style.Font.Bold = true;
                        ws.Cell("J" + recNo).Style.Font.Bold = true;
                        ws.Cell("K" + recNo).Style.Font.Bold = true;
                        ws.Cell("L" + recNo).Style.Font.Bold = true;
                        ws.Cell("M" + recNo).Style.Font.Bold = true;
                        ws.Cell("N" + recNo).Style.Font.Bold = true;
                    }

                    if (counter == 0)
                    {
                        ws.Cell("A" + recNo).Value = row.BranchName;
                        ws.Cell("B" + recNo).Value = row.PICOutlet;
                        ws.Cell("C" + recNo).Value = row.NoHP;
                        ws.Cell("D" + recNo).Value = row.Email;
                        ws.Cell("E" + recNo).Value = row.FakturPenjualan;
                        ws.Cell("F" + recNo).Value = row.PermohonanFakPol;
                        ws.Cell("G" + recNo).Value = row.QnrInput;
                        ws.Cell("H" + recNo).Value = row.PersenFakturPenjualan;
                        ws.Cell("I" + recNo).Value = row.PersenPermohonanFakPol;
                    }
                    else
                    {
                        ws.Cell("J" + recNo).Value = row.FakturPenjualan;
                        ws.Cell("K" + recNo).Value = row.PermohonanFakPol;
                        ws.Cell("L" + recNo).Value = row.QnrInput;
                        ws.Cell("M" + recNo).Value = row.PersenFakturPenjualan;
                        ws.Cell("N" + recNo).Value = row.PersenPermohonanFakPol;

                    }

                    ws.Cell("A" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("B" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("C" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("D" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("E" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("F" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("G" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("H" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("I" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("J" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("K" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("L" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("M" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    ws.Cell("N" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("A" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    ws.Cell("A" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("B" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("C" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("D" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("E" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("F" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("G" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("H" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("I" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("J" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("K" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("L" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("M" + recNo).Style.Font.FontSize = 10;
                    ws.Cell("N" + recNo).Style.Font.FontSize = 10;

                    recNo++;
                }
                counter++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult QuestionnaireRekapProdSummaryOccupation()
        {
            #region step1
            DateTime now = DateTime.Now;
            string fileName = "Qa_RekapSummary_Occupation" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var statusKonsumen = Request.Params["StatusKonsumen"] == "" ? "%" : Request.Params["StatusKonsumen"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var areas = ctx.GroupAreas.Select(x => new { AreaDealer = x.AreaDealer, GroupNo = x.GroupNo }).OrderBy(x => x.GroupNo).ToList();

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Rekap Summary Occupation");

            int recNo = 1;

            var rngTable = ws.Range("B1:E1");

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.Black;

            rngTable.Merge();
            rngTable.Value = "SUMMARY";
            rngTable.Style.Font.FontSize = 20;
            rngTable.Style.Font.Underline = XLFontUnderlineValues.Single;

            ws.Columns("1").Width = 50;
            ws.Columns("2", "433").Width = 17;
            recNo += 2;

            ws.Cell("A" + recNo).Value = "Status Konsumen : " + (statusKonsumen == "%" ? "ALL" : statusKonsumen == "A" ? "Individu" : "Fleet/Perusahaan");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            ws.Cell("A" + recNo).Value = "Periode : " + dateStart.ToString("dd MMM yyyy") + " s/d " + dateEnd.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo++;

            ws.Cell("A" + recNo).Value = "Print Date : " + DateTime.UtcNow.ToString("dd MMM yyyy");
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            #region loop insert summary
            int idx2 = 0;
            foreach (var item in areas)
            {
                string groupno = item.GroupNo;

                bool isdeletetable = false;

                if (idx2 == 0)
                {
                    isdeletetable = true;
                }

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600;
                cmd.CommandText = "uspfn_SpGetSummaryOccpPerArea";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("areacode", groupno);
                cmd.Parameters.AddWithValue("dateStart", dateStart);
                cmd.Parameters.AddWithValue("dateEnd", dateEnd);
                cmd.Parameters.AddWithValue("statusKonsumen", statusKonsumen);
                cmd.Parameters.AddWithValue("@isDeleteTable", isdeletetable);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                idx2++;

            }

            SqlCommand cmd2 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd2.CommandTimeout = 3600;
            cmd2.CommandText = "uspfn_SpGetSummaryOccpMainAllArea";
            cmd2.CommandType = CommandType.StoredProcedure;
            cmd2.Parameters.Clear();

            cmd2.Parameters.AddWithValue("dateStart", dateStart);
            cmd2.Parameters.AddWithValue("dateEnd", dateEnd);
            cmd2.Parameters.AddWithValue("statusKonsumen", statusKonsumen);

            SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
            DataSet ds2 = new DataSet();
            da2.Fill(ds2);
            #endregion

            List<IQueryable<QaProdSummaryOccp>> listIqueryable = new List<IQueryable<QaProdSummaryOccp>>();

            IQueryable<QaProdSummaryOccp> z = null;

            var qryz = z;

            foreach (var item in areas)
            {
                string groupno = item.GroupNo;

                qryz = ctx.Database.SqlQuery<QaProdSummaryOccp>("exec uspfn_SpGetQaSummaryByOccupation @dateStart=@p0, @dateEnd=@p1, @groupNo=@p2, @statusKonsumen=@p3", dateStart, dateEnd, groupno, statusKonsumen).AsQueryable();

                listIqueryable.Add(qryz);
            }
            #endregion

            #region Loop Occp Sum 1
            int counter = 0;
            foreach (var qry in listIqueryable)
            {
                #region listcell
                List<string> listCell = new List<string>();
                if (counter == 0)
                {
                    listCell.Add("A");
                    listCell.Add("B");
                    listCell.Add("C");
                    listCell.Add("D");
                    listCell.Add("E");
                    listCell.Add("F");
                    listCell.Add("G");
                    listCell.Add("H");
                    listCell.Add("I");
                    listCell.Add("J");
                    listCell.Add("K");
                    listCell.Add("L");
                    listCell.Add("M");
                    listCell.Add("N");
                    listCell.Add("O");
                    listCell.Add("P");
                    listCell.Add("Q");
                    listCell.Add("R");
                    listCell.Add("S");
                    listCell.Add("T");
                    listCell.Add("U");
                    listCell.Add("V");
                    listCell.Add("W");
                    listCell.Add("X");
                    listCell.Add("Y");
                    listCell.Add("Z");
                    listCell.Add("AA");
                    listCell.Add("AB");
                    listCell.Add("AC");
                    listCell.Add("AD");
                    listCell.Add("AE");
                    listCell.Add("AF");
                    listCell.Add("AG");
                    listCell.Add("AH");
                    listCell.Add("AI");
                    listCell.Add("AJ");
                    listCell.Add("AK");
                    listCell.Add("AL");
                    listCell.Add("AM");
                    listCell.Add("AN");
                    listCell.Add("AO");
                    listCell.Add("AP");
                    listCell.Add("AQ");
                    listCell.Add("AR");
                    listCell.Add("AS");
                    listCell.Add("AT");
                    listCell.Add("AU");
                    listCell.Add("AV");
                    listCell.Add("AW");
                    listCell.Add("AX");
                    listCell.Add("AY");
                    listCell.Add("AZ");
                }
                else if (counter == 1)
                {
                    listCell.Add("BA");
                    listCell.Add("BB");
                    listCell.Add("BC");
                    listCell.Add("BD");
                    listCell.Add("BE");
                    listCell.Add("BF");
                    listCell.Add("BG");
                    listCell.Add("BH");
                    listCell.Add("BI");
                    listCell.Add("BJ");
                    listCell.Add("BK");
                    listCell.Add("BL");
                    listCell.Add("BM");
                    listCell.Add("BN");
                    listCell.Add("BO");
                    listCell.Add("BP");
                    listCell.Add("BQ");
                    listCell.Add("BR");
                    listCell.Add("BS");
                    listCell.Add("BT");
                    listCell.Add("BU");
                    listCell.Add("BV");
                    listCell.Add("BW");
                    listCell.Add("BX");
                    listCell.Add("BY");
                    listCell.Add("BZ");
                    listCell.Add("CA");
                    listCell.Add("CB");
                    listCell.Add("CC");
                    listCell.Add("CD");
                    listCell.Add("CE");
                    listCell.Add("CF");
                    listCell.Add("CG");
                    listCell.Add("CH");
                    listCell.Add("CI");
                    listCell.Add("CJ");
                    listCell.Add("CK");
                    listCell.Add("CL");
                    listCell.Add("CM");
                    listCell.Add("CN");
                    listCell.Add("CO");
                    listCell.Add("CP");
                    listCell.Add("CQ");
                    listCell.Add("CR");
                    listCell.Add("CS");
                    listCell.Add("CT");
                    listCell.Add("CU");
                    listCell.Add("CV");
                    listCell.Add("CW");
                    listCell.Add("CX");
                    listCell.Add("CY");
                }
                else if (counter == 2)
                {
                    listCell.Add("CZ");
                    listCell.Add("DA");
                    listCell.Add("DB");
                    listCell.Add("DC");
                    listCell.Add("DD");
                    listCell.Add("DE");
                    listCell.Add("DF");
                    listCell.Add("DG");
                    listCell.Add("DH");
                    listCell.Add("DI");
                    listCell.Add("DJ");
                    listCell.Add("DK");
                    listCell.Add("DL");
                    listCell.Add("DM");
                    listCell.Add("DN");
                    listCell.Add("DO");
                    listCell.Add("DP");
                    listCell.Add("DQ");
                    listCell.Add("DR");
                    listCell.Add("DS");
                    listCell.Add("DT");
                    listCell.Add("DU");
                    listCell.Add("DV");
                    listCell.Add("DW");
                    listCell.Add("DX");
                    listCell.Add("DY");
                    listCell.Add("DZ");
                    listCell.Add("EA");
                    listCell.Add("EB");
                    listCell.Add("EC");
                    listCell.Add("ED");
                    listCell.Add("EE");
                    listCell.Add("EF");
                    listCell.Add("EG");
                    listCell.Add("EH");
                    listCell.Add("EI");
                    listCell.Add("EJ");
                    listCell.Add("EK");
                    listCell.Add("EL");
                    listCell.Add("EM");
                    listCell.Add("EN");
                    listCell.Add("EO");
                    listCell.Add("EP");
                    listCell.Add("EQ");
                    listCell.Add("ER");
                    listCell.Add("ES");
                    listCell.Add("ET");
                    listCell.Add("EU");
                    listCell.Add("EV");
                    listCell.Add("EW");
                    listCell.Add("EX");

                }
                else if (counter == 3)
                {
                    listCell.Add("EY");
                    listCell.Add("EZ");
                    listCell.Add("FA");
                    listCell.Add("FB");
                    listCell.Add("FC");
                    listCell.Add("FD");
                    listCell.Add("FE");
                    listCell.Add("FF");
                    listCell.Add("FG");
                    listCell.Add("FH");
                    listCell.Add("FI");
                    listCell.Add("FJ");
                    listCell.Add("FK");
                    listCell.Add("FL");
                    listCell.Add("FM");
                    listCell.Add("FN");
                    listCell.Add("FO");
                    listCell.Add("FP");
                    listCell.Add("FQ");
                    listCell.Add("FR");
                    listCell.Add("FS");
                    listCell.Add("FT");
                    listCell.Add("FU");
                    listCell.Add("FV");
                    listCell.Add("FW");
                    listCell.Add("FX");
                    listCell.Add("FY");
                    listCell.Add("FZ");
                    listCell.Add("GA");
                    listCell.Add("GB");
                    listCell.Add("GC");
                    listCell.Add("GD");
                    listCell.Add("GE");
                    listCell.Add("GF");
                    listCell.Add("GG");
                    listCell.Add("GH");
                    listCell.Add("GI");
                    listCell.Add("GJ");
                    listCell.Add("GK");
                    listCell.Add("GL");
                    listCell.Add("GM");
                    listCell.Add("GN");
                    listCell.Add("GO");
                    listCell.Add("GP");
                    listCell.Add("GQ");
                    listCell.Add("GR");
                    listCell.Add("GS");
                    listCell.Add("GT");
                    listCell.Add("GU");
                    listCell.Add("GV");
                    listCell.Add("GW");

                }
                else if (counter == 4)
                {
                    listCell.Add("GX");
                    listCell.Add("GY");
                    listCell.Add("GZ");
                    listCell.Add("HA");
                    listCell.Add("HB");
                    listCell.Add("HC");
                    listCell.Add("HD");
                    listCell.Add("HE");
                    listCell.Add("HF");
                    listCell.Add("HG");
                    listCell.Add("HH");
                    listCell.Add("HI");
                    listCell.Add("HJ");
                    listCell.Add("HK");
                    listCell.Add("HL");
                    listCell.Add("HM");
                    listCell.Add("HN");
                    listCell.Add("HO");
                    listCell.Add("HP");
                    listCell.Add("HQ");
                    listCell.Add("HR");
                    listCell.Add("HS");
                    listCell.Add("HT");
                    listCell.Add("HU");
                    listCell.Add("HV");
                    listCell.Add("HW");
                    listCell.Add("HX");
                    listCell.Add("HY");
                    listCell.Add("HZ");
                    listCell.Add("IA");
                    listCell.Add("IB");
                    listCell.Add("IC");
                    listCell.Add("ID");
                    listCell.Add("IE");
                    listCell.Add("IF");
                    listCell.Add("IG");
                    listCell.Add("IH");
                    listCell.Add("II");
                    listCell.Add("IJ");
                    listCell.Add("IK");
                    listCell.Add("IL");
                    listCell.Add("IM");
                    listCell.Add("IN");
                    listCell.Add("IO");
                    listCell.Add("IP");
                    listCell.Add("IQ");
                    listCell.Add("IR");
                    listCell.Add("IS");
                    listCell.Add("IT");
                    listCell.Add("IU");
                    listCell.Add("IV");

                }
                else if (counter == 5)
                {
                    listCell.Add("IW");
                    listCell.Add("IX");
                    listCell.Add("IY");
                    listCell.Add("IZ");
                    listCell.Add("JA");
                    listCell.Add("JB");
                    listCell.Add("JC");
                    listCell.Add("JD");
                    listCell.Add("JE");
                    listCell.Add("JF");
                    listCell.Add("JG");
                    listCell.Add("JH");
                    listCell.Add("JI");
                    listCell.Add("JJ");
                    listCell.Add("JK");
                    listCell.Add("JL");
                    listCell.Add("JM");
                    listCell.Add("JN");
                    listCell.Add("JO");
                    listCell.Add("JP");
                    listCell.Add("JQ");
                    listCell.Add("JR");
                    listCell.Add("JS");
                    listCell.Add("JT");
                    listCell.Add("JU");
                    listCell.Add("JV");
                    listCell.Add("JW");
                    listCell.Add("JX");
                    listCell.Add("JY");
                    listCell.Add("JZ");
                    listCell.Add("KA");
                    listCell.Add("KB");
                    listCell.Add("KC");
                    listCell.Add("KD");
                    listCell.Add("KE");
                    listCell.Add("KF");
                    listCell.Add("KG");
                    listCell.Add("KH");
                    listCell.Add("KI");
                    listCell.Add("KJ");
                    listCell.Add("KK");
                    listCell.Add("KL");
                    listCell.Add("KM");
                    listCell.Add("KN");
                    listCell.Add("KO");
                    listCell.Add("KP");
                    listCell.Add("KQ");
                    listCell.Add("KR");
                    listCell.Add("KS");
                    listCell.Add("KT");
                    listCell.Add("KU");

                }
                else if (counter == 6)
                {
                    listCell.Add("KV");
                    listCell.Add("KW");
                    listCell.Add("KX");
                    listCell.Add("KY");
                    listCell.Add("KZ");
                    listCell.Add("LA");
                    listCell.Add("LB");
                    listCell.Add("LC");
                    listCell.Add("LD");
                    listCell.Add("LE");
                    listCell.Add("LF");
                    listCell.Add("LG");
                    listCell.Add("LH");
                    listCell.Add("LI");
                    listCell.Add("LJ");
                    listCell.Add("LK");
                    listCell.Add("LL");
                    listCell.Add("LM");
                    listCell.Add("LN");
                    listCell.Add("LO");
                    listCell.Add("LP");
                    listCell.Add("LQ");
                    listCell.Add("LR");
                    listCell.Add("LS");
                    listCell.Add("LT");
                    listCell.Add("LU");
                    listCell.Add("LV");
                    listCell.Add("LW");
                    listCell.Add("LX");
                    listCell.Add("LY");
                    listCell.Add("LZ");
                    listCell.Add("MA");
                    listCell.Add("MB");
                    listCell.Add("MC");
                    listCell.Add("MD");
                    listCell.Add("ME");
                    listCell.Add("MF");
                    listCell.Add("MG");
                    listCell.Add("MH");
                    listCell.Add("MI");
                    listCell.Add("MJ");
                    listCell.Add("MK");
                    listCell.Add("ML");
                    listCell.Add("MM");
                    listCell.Add("MN");
                    listCell.Add("MO");
                    listCell.Add("MP");
                    listCell.Add("MQ");
                    listCell.Add("MR");
                    listCell.Add("MS");
                    listCell.Add("MT");

                }
                else if (counter == 7)
                {
                    listCell.Add("MU");
                    listCell.Add("MV");
                    listCell.Add("MW");
                    listCell.Add("MX");
                    listCell.Add("MY");
                    listCell.Add("MZ");
                    listCell.Add("NA");
                    listCell.Add("NB");
                    listCell.Add("NC");
                    listCell.Add("ND");
                    listCell.Add("NE");
                    listCell.Add("NF");
                    listCell.Add("NG");
                    listCell.Add("NH");
                    listCell.Add("NI");
                    listCell.Add("NJ");
                    listCell.Add("NK");
                    listCell.Add("NL");
                    listCell.Add("NM");
                    listCell.Add("NN");
                    listCell.Add("NO");
                    listCell.Add("NP");
                    listCell.Add("NQ");
                    listCell.Add("NR");
                    listCell.Add("NS");
                    listCell.Add("NT");
                    listCell.Add("NU");
                    listCell.Add("NV");
                    listCell.Add("NW");
                    listCell.Add("NX");
                    listCell.Add("NY");
                    listCell.Add("NZ");
                    listCell.Add("OA");
                    listCell.Add("OB");
                    listCell.Add("OC");
                    listCell.Add("OD");
                    listCell.Add("OE");
                    listCell.Add("OF");
                    listCell.Add("OG");
                    listCell.Add("OH");
                    listCell.Add("OI");
                    listCell.Add("OJ");
                    listCell.Add("OK");
                    listCell.Add("OL");
                    listCell.Add("OM");
                    listCell.Add("ON");
                    listCell.Add("OO");
                    listCell.Add("OP");
                    listCell.Add("OQ");
                    listCell.Add("OR");
                    listCell.Add("OS");

                }
                else if (counter == 8)
                {
                    listCell.Add("OT");
                    listCell.Add("OU");
                    listCell.Add("OV");
                    listCell.Add("OW");
                    listCell.Add("OX");
                    listCell.Add("OY");
                    listCell.Add("OZ");
                    listCell.Add("PA");
                    listCell.Add("PB");
                    listCell.Add("PC");
                    listCell.Add("PD");
                    listCell.Add("PE");
                    listCell.Add("PF");
                    listCell.Add("PG");
                    listCell.Add("PH");
                    listCell.Add("PI");
                    listCell.Add("PJ");
                    listCell.Add("PK");
                    listCell.Add("PL");
                    listCell.Add("PM");
                    listCell.Add("PN");
                    listCell.Add("PO");
                    listCell.Add("PP");
                    listCell.Add("PQ");
                    listCell.Add("PR");
                    listCell.Add("PS");
                    listCell.Add("PT");
                    listCell.Add("PU");
                    listCell.Add("PV");
                    listCell.Add("PW");
                    listCell.Add("PX");
                    listCell.Add("PY");
                    listCell.Add("PZ");
                    listCell.Add("QA");
                    listCell.Add("QB");
                    listCell.Add("QC");
                    listCell.Add("QD");
                    listCell.Add("QE");
                    listCell.Add("QF");
                    listCell.Add("QG");
                    listCell.Add("QH");
                    listCell.Add("QI");
                    listCell.Add("QJ");
                    listCell.Add("QK");
                    listCell.Add("QL");
                    listCell.Add("QM");
                    listCell.Add("QN");
                    listCell.Add("QO");
                    listCell.Add("QP");
                    listCell.Add("QQ");
                    listCell.Add("QR");
                }
                #endregion

                recNo = 5;

                foreach (var row in qry)
                {
                    if (counter == 0)
                    {
                        if (row.Num.ToString().ToUpper() == "AREA")
                        {
                            var rngTable2 = ws.Range(listCell[1] + recNo + ":" + listCell[51] + recNo);
                            rngTable2.Merge();

                            rngTable2.Value = row.ModelA1;
                            rngTable2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            if (row.Style == "LB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightBlue;
                            }
                            else if (row.Style == "LG")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightGreen;
                            }
                            else if (row.Style == "LPI")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightPink;
                            }
                            else if (row.Style == "LSB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                            }
                            else if (row.Style == "LSPI")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSalmonPink;
                            }
                            else if (row.Style == "LSTB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
                            }
                            else if (row.Style == "OR")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.OrangeColorWheel;
                            }
                            else if (row.Style == "LPP")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightPastelPurple;
                            }

                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                        }
                        else
                        {
                            if (row.SectionCode != null)
                            {
                                ws.Cell(listCell[1] + recNo).Value = row.ModelA1;
                                ws.Cell(listCell[1] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            }
                        }

                        if (row.Num.ToString().ToUpper() == "OCCUPATION")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[51] + recNo);
                            rngTable2.Style.Fill.BackgroundColor = XLColor.LightGray;

                            if (row.ModelA1.ToString().ToUpper() == "AGRICULTURE")
                            {
                                var rngt = ws.Range(listCell[1] + recNo + ":" + listCell[4] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelB1.ToString().ToUpper() == "PLANTATION")
                            {
                                var rngt = ws.Range(listCell[5] + recNo + ":" + listCell[11] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelC1.ToString().ToUpper() == "FISHERY")
                            {
                                var rngt = ws.Range(listCell[12] + recNo + ":" + listCell[14] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelD1.ToString().ToUpper() == "ENTREPRENEURIAL")
                            {
                                var rngt = ws.Range(listCell[15] + recNo + ":" + listCell[24] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelE1.ToString().ToUpper() == "TRADER")
                            {
                                var rngt = ws.Range(listCell[25] + recNo + ":" + listCell[34] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelF1.ToString().ToUpper() == "CONTRACTOR")
                            {
                                var rngt = ws.Range(listCell[35] + recNo + ":" + listCell[37] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelG1.ToString().ToUpper() == "LIVESTOCK")
                            {
                                var rngt = ws.Range(listCell[38] + recNo + ":" + listCell[40] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelH1.ToString().ToUpper() == "SERVICE")
                            {
                                var rngt = ws.Range(listCell[41] + recNo + ":" + listCell[42] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelI1.ToString().ToUpper() == "SHOP OWNER")
                            {
                                var rngt = ws.Range(listCell[43] + recNo + ":" + listCell[47] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelJ1.ToString().ToUpper() == "MINING")
                            {
                                var rngt = ws.Range(listCell[48] + recNo + ":" + listCell[50] + recNo);
                                rngt.Merge();
                            }
                        }

                        if (row.SectionCode == null)
                        {
                            ws.Cell(listCell[0] + recNo).Value = row.QuestionAnswer;
                            ws.Cell(listCell[0] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        }
                        else
                        {
                            ws.Cell(listCell[0] + recNo).Value = row.QuestionAnswer;
                            ws.Cell(listCell[0] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Cell(listCell[2] + recNo).Value = row.ModelA2;
                            ws.Cell(listCell[2] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[3] + recNo).Value = row.ModelA3;
                            ws.Cell(listCell[3] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[4] + recNo).Value = row.ModelA4;
                            ws.Cell(listCell[4] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[5] + recNo).Value = row.ModelB1;
                            ws.Cell(listCell[5] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[6] + recNo).Value = row.ModelB2;
                            ws.Cell(listCell[6] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[7] + recNo).Value = row.ModelB3;
                            ws.Cell(listCell[7] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[8] + recNo).Value = row.ModelB4;
                            ws.Cell(listCell[8] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[9] + recNo).Value = row.ModelB5;
                            ws.Cell(listCell[9] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[10] + recNo).Value = row.ModelB6;
                            ws.Cell(listCell[10] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[11] + recNo).Value = row.ModelB7;
                            ws.Cell(listCell[11] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[12] + recNo).Value = row.ModelC1;
                            ws.Cell(listCell[12] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[13] + recNo).Value = row.ModelC2;
                            ws.Cell(listCell[13] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[14] + recNo).Value = row.ModelC3;
                            ws.Cell(listCell[14] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[15] + recNo).Value = row.ModelD1;
                            ws.Cell(listCell[15] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[16] + recNo).Value = row.ModelD2;
                            ws.Cell(listCell[16] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[17] + recNo).Value = row.ModelD3;
                            ws.Cell(listCell[17] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[18] + recNo).Value = row.ModelD4;
                            ws.Cell(listCell[18] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[19] + recNo).Value = row.ModelD5;
                            ws.Cell(listCell[19] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[20] + recNo).Value = row.ModelD6;
                            ws.Cell(listCell[20] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[21] + recNo).Value = row.ModelD7;
                            ws.Cell(listCell[21] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[22] + recNo).Value = row.ModelD8;
                            ws.Cell(listCell[22] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[23] + recNo).Value = row.ModelD9;
                            ws.Cell(listCell[23] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[24] + recNo).Value = row.ModelD10;
                            ws.Cell(listCell[24] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[25] + recNo).Value = row.ModelE1;
                            ws.Cell(listCell[25] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[26] + recNo).Value = row.ModelE2;
                            ws.Cell(listCell[26] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[27] + recNo).Value = row.ModelE3;
                            ws.Cell(listCell[27] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[28] + recNo).Value = row.ModelE4;
                            ws.Cell(listCell[28] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[29] + recNo).Value = row.ModelE5;
                            ws.Cell(listCell[29] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[30] + recNo).Value = row.ModelE6;
                            ws.Cell(listCell[30] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[31] + recNo).Value = row.ModelE7;
                            ws.Cell(listCell[31] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[32] + recNo).Value = row.ModelE8;
                            ws.Cell(listCell[32] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[33] + recNo).Value = row.ModelE9;
                            ws.Cell(listCell[33] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[34] + recNo).Value = row.ModelE10;
                            ws.Cell(listCell[34] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[35] + recNo).Value = row.ModelF1;
                            ws.Cell(listCell[35] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[36] + recNo).Value = row.ModelF2;
                            ws.Cell(listCell[36] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[37] + recNo).Value = row.ModelF3;
                            ws.Cell(listCell[37] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[38] + recNo).Value = row.ModelG1;
                            ws.Cell(listCell[38] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[39] + recNo).Value = row.ModelG2;
                            ws.Cell(listCell[39] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[40] + recNo).Value = row.ModelG3;
                            ws.Cell(listCell[40] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[41] + recNo).Value = row.ModelH1;
                            ws.Cell(listCell[41] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[42] + recNo).Value = row.ModelH2;
                            ws.Cell(listCell[42] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[43] + recNo).Value = row.ModelI1;
                            ws.Cell(listCell[43] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[44] + recNo).Value = row.ModelI2;
                            ws.Cell(listCell[44] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[45] + recNo).Value = row.ModelI3;
                            ws.Cell(listCell[45] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[46] + recNo).Value = row.ModelI4;
                            ws.Cell(listCell[46] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[47] + recNo).Value = row.ModelI5;
                            ws.Cell(listCell[47] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[48] + recNo).Value = row.ModelJ1;
                            ws.Cell(listCell[48] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[49] + recNo).Value = row.ModelJ2;
                            ws.Cell(listCell[49] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[50] + recNo).Value = row.ModelJ3;
                            ws.Cell(listCell[50] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[51] + recNo).Value = row.ModelK1;
                            ws.Cell(listCell[51] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        if (row.Style == "LGA")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                            ws.Cell(listCell[0] + recNo).Style.Fill.BackgroundColor = XLColor.LightGray;
                        }
                        else if (row.Style == "LG")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                            ws.Cell(listCell[0] + recNo).Style.Fill.BackgroundColor = XLColor.LightGreen;
                        }
                        else if (row.Style == "BL")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                        }
                        else if (row.Style == "PI")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                            ws.Cell(listCell[0] + recNo).Style.Fill.BackgroundColor = XLColor.LightPink;
                        }
                        else if (row.Style == "LB")
                        {
                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                            ws.Cell(listCell[0] + recNo).Style.Fill.BackgroundColor = XLColor.LightBlue;
                        }
                        else if (row.Style == "YA")
                        {
                            var ALlCell = ws.Range(listCell[0] + recNo + ":" + listCell[51] + recNo);
                            ALlCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                        }
                        else if (row.Style == "ALLGREY")
                        {
                            var ALlCell = ws.Range(listCell[0] + recNo + ":" + listCell[51] + recNo);
                            ALlCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                            ALlCell.Style.Font.Bold = true;
                        }
                        else if (row.Style == "ALLYELLOW")
                        {
                            var ALlCell = ws.Range(listCell[0] + recNo + ":" + listCell[50] + recNo);
                            ALlCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                            ALlCell.Style.Font.Bold = true;
                        }

                        ws.Cell(listCell[0] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[1] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[2] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[3] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[4] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[5] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[6] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[7] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[8] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[9] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[10] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[11] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[12] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[13] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[14] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[15] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[16] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[17] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[18] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[19] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[20] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[21] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[22] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[23] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[24] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[25] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[26] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[27] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[28] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[29] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[30] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[31] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[32] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[33] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[34] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[35] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[36] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[37] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[38] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[39] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[40] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[41] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[42] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[43] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[44] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[45] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[46] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[47] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[48] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[49] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[50] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[51] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        ws.Cell(listCell[0] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[1] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[2] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[3] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[4] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[5] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[6] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[7] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[8] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[9] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[10] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[11] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[12] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[13] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[14] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[15] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[16] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[17] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[18] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[19] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[20] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[21] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[22] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[23] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[24] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[25] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[26] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[27] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[28] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[29] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[30] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[31] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[32] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[33] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[34] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[35] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[36] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[37] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[38] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[39] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[40] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[41] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[42] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[43] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[44] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[45] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[46] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[47] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[48] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[49] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[50] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[51] + recNo).Style.Font.FontSize = 10;
                    }
                    else
                    {
                        if (row.Num.ToString().ToUpper() == "AREA")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[50] + recNo);
                            rngTable2.Merge();

                            rngTable2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                            if (row.Style == "LB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightBlue;
                            }
                            else if (row.Style == "LG")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightGreen;
                            }
                            else if (row.Style == "LPI")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightPink;
                            }
                            else if (row.Style == "LSB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSkyBlue;
                            }
                            else if (row.Style == "LSPI")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSalmonPink;
                            }
                            else if (row.Style == "LSTB")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightSteelBlue;
                            }
                            else if (row.Style == "OR")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.OrangeColorWheel;
                            }
                            else if (row.Style == "LPP")
                            {
                                rngTable2.Style.Font.Bold = true;
                                rngTable2.Style.Fill.BackgroundColor = XLColor.LightPastelPurple;
                            }

                            ws.Cell(listCell[0] + recNo).Style.Font.Bold = true;
                        }

                        if (row.Num.ToString().ToUpper() == "OCCUPATION")
                        {
                            var rngTable2 = ws.Range(listCell[0] + recNo + ":" + listCell[50] + recNo);
                            rngTable2.Style.Fill.BackgroundColor = XLColor.LightGray;

                            if (row.ModelA1.ToString().ToUpper() == "AGRICULTURE")
                            {
                                var rngt = ws.Range(listCell[0] + recNo + ":" + listCell[3] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelB1.ToString().ToUpper() == "PLANTATION")
                            {
                                var rngt = ws.Range(listCell[4] + recNo + ":" + listCell[10] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelC1.ToString().ToUpper() == "FISHERY")
                            {
                                var rngt = ws.Range(listCell[11] + recNo + ":" + listCell[13] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelD1.ToString().ToUpper() == "ENTREPRENEURIAL")
                            {
                                var rngt = ws.Range(listCell[14] + recNo + ":" + listCell[23] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelE1.ToString().ToUpper() == "TRADER")
                            {
                                var rngt = ws.Range(listCell[24] + recNo + ":" + listCell[33] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelF1.ToString().ToUpper() == "CONTRACTOR")
                            {
                                var rngt = ws.Range(listCell[34] + recNo + ":" + listCell[36] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelG1.ToString().ToUpper() == "LIVESTOCK")
                            {
                                var rngt = ws.Range(listCell[37] + recNo + ":" + listCell[39] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelH1.ToString().ToUpper() == "SERVICE")
                            {
                                var rngt = ws.Range(listCell[40] + recNo + ":" + listCell[41] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelI1.ToString().ToUpper() == "SHOP OWNER")
                            {
                                var rngt = ws.Range(listCell[42] + recNo + ":" + listCell[46] + recNo);
                                rngt.Merge();
                            }

                            if (row.ModelJ1.ToString().ToUpper() == "MINING")
                            {
                                var rngt = ws.Range(listCell[47] + recNo + ":" + listCell[49] + recNo);
                                rngt.Merge();
                            }
                        }

                        if (row.SectionCode != null)
                        {
                            ws.Cell(listCell[0] + recNo).Value = row.ModelA1;
                            ws.Cell(listCell[0] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[1] + recNo).Value = row.ModelA2;
                            ws.Cell(listCell[1] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[2] + recNo).Value = row.ModelA3;
                            ws.Cell(listCell[2] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[3] + recNo).Value = row.ModelA4;
                            ws.Cell(listCell[3] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[4] + recNo).Value = row.ModelB1;
                            ws.Cell(listCell[4] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[5] + recNo).Value = row.ModelB2;
                            ws.Cell(listCell[5] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[6] + recNo).Value = row.ModelB3;
                            ws.Cell(listCell[6] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[7] + recNo).Value = row.ModelB4;
                            ws.Cell(listCell[7] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[8] + recNo).Value = row.ModelB5;
                            ws.Cell(listCell[8] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[9] + recNo).Value = row.ModelB6;
                            ws.Cell(listCell[9] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[10] + recNo).Value = row.ModelB7;
                            ws.Cell(listCell[10] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[11] + recNo).Value = row.ModelC1;
                            ws.Cell(listCell[11] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[12] + recNo).Value = row.ModelC2;
                            ws.Cell(listCell[12] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[13] + recNo).Value = row.ModelC3;
                            ws.Cell(listCell[13] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[14] + recNo).Value = row.ModelD1;
                            ws.Cell(listCell[14] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[15] + recNo).Value = row.ModelD2;
                            ws.Cell(listCell[15] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[16] + recNo).Value = row.ModelD3;
                            ws.Cell(listCell[16] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[17] + recNo).Value = row.ModelD4;
                            ws.Cell(listCell[17] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[18] + recNo).Value = row.ModelD5;
                            ws.Cell(listCell[18] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[19] + recNo).Value = row.ModelD6;
                            ws.Cell(listCell[19] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[20] + recNo).Value = row.ModelD7;
                            ws.Cell(listCell[20] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[21] + recNo).Value = row.ModelD8;
                            ws.Cell(listCell[21] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[22] + recNo).Value = row.ModelD9;
                            ws.Cell(listCell[22] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[23] + recNo).Value = row.ModelD10;
                            ws.Cell(listCell[23] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[24] + recNo).Value = row.ModelE1;
                            ws.Cell(listCell[24] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[25] + recNo).Value = row.ModelE2;
                            ws.Cell(listCell[25] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[26] + recNo).Value = row.ModelE3;
                            ws.Cell(listCell[26] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[27] + recNo).Value = row.ModelE4;
                            ws.Cell(listCell[27] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[28] + recNo).Value = row.ModelE5;
                            ws.Cell(listCell[28] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[29] + recNo).Value = row.ModelE6;
                            ws.Cell(listCell[29] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[30] + recNo).Value = row.ModelE7;
                            ws.Cell(listCell[30] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[31] + recNo).Value = row.ModelE8;
                            ws.Cell(listCell[31] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[32] + recNo).Value = row.ModelE9;
                            ws.Cell(listCell[32] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[33] + recNo).Value = row.ModelE10;
                            ws.Cell(listCell[33] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[34] + recNo).Value = row.ModelF1;
                            ws.Cell(listCell[34] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[35] + recNo).Value = row.ModelF2;
                            ws.Cell(listCell[35] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[36] + recNo).Value = row.ModelF3;
                            ws.Cell(listCell[36] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[37] + recNo).Value = row.ModelG1;
                            ws.Cell(listCell[37] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[38] + recNo).Value = row.ModelG2;
                            ws.Cell(listCell[38] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[39] + recNo).Value = row.ModelG3;
                            ws.Cell(listCell[39] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[40] + recNo).Value = row.ModelH1;
                            ws.Cell(listCell[40] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[41] + recNo).Value = row.ModelH2;
                            ws.Cell(listCell[41] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[42] + recNo).Value = row.ModelI1;
                            ws.Cell(listCell[42] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[43] + recNo).Value = row.ModelI2;
                            ws.Cell(listCell[43] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[44] + recNo).Value = row.ModelI3;
                            ws.Cell(listCell[44] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[45] + recNo).Value = row.ModelI4;
                            ws.Cell(listCell[45] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[46] + recNo).Value = row.ModelI5;
                            ws.Cell(listCell[46] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[47] + recNo).Value = row.ModelJ1;
                            ws.Cell(listCell[47] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[48] + recNo).Value = row.ModelJ2;
                            ws.Cell(listCell[48] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[49] + recNo).Value = row.ModelJ3;
                            ws.Cell(listCell[49] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(listCell[50] + recNo).Value = row.ModelK1;
                            ws.Cell(listCell[50] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        }

                        if (row.Style == "YA")
                        {
                            var ALlCell = ws.Range(listCell[0] + recNo + ":" + listCell[50] + recNo);
                            ALlCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                        }
                        else if (row.Style == "ALLGREY")
                        {
                            var ALlCell = ws.Range(listCell[0] + recNo + ":" + listCell[50] + recNo);
                            ALlCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                            ALlCell.Style.Font.Bold = true;
                        }
                        else if (row.Style == "ALLYELLOW")
                        {
                            var ALlCell = ws.Range(listCell[0] + recNo + ":" + listCell[50] + recNo);
                            ALlCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                            ALlCell.Style.Font.Bold = true;
                        }

                        ws.Cell(listCell[0] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[1] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[2] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[3] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[4] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[5] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[6] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[7] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[8] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[9] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[10] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[11] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[12] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[13] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[14] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[15] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[16] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[17] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[18] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[19] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[20] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[21] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[22] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[23] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[24] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[25] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[26] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[27] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[28] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[29] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[30] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[31] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[32] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[33] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[34] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[35] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[36] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[37] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[38] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[39] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[40] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[41] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[42] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[43] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[44] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[45] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[46] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[47] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[48] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[49] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        ws.Cell(listCell[50] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                        ws.Cell(listCell[0] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[1] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[2] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[3] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[4] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[5] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[6] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[7] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[8] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[9] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[10] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[11] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[12] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[13] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[14] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[15] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[16] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[17] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[18] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[19] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[20] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[21] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[22] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[23] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[24] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[25] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[26] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[27] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[28] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[29] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[30] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[31] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[32] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[33] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[34] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[35] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[36] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[37] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[38] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[39] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[40] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[41] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[42] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[43] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[44] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[45] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[46] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[47] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[48] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[49] + recNo).Style.Font.FontSize = 10;
                        ws.Cell(listCell[50] + recNo).Style.Font.FontSize = 10;
                    }

                    recNo++;
                }
                counter++;
            }
            #endregion

            #region Loop Occp Sum 2
            var qryLast = ctx.Database.SqlQuery<QaSumOccpModel>("exec uspfn_SpGetQaSummaryByOccupationMain @dateStart=@p0, @dateEnd=@p1, @statusKonsumen=@p2", dateStart, dateEnd, statusKonsumen).AsQueryable();

            List<string> listCell2 = new List<string>();

            listCell2.Add("QT");
            listCell2.Add("QU");
            listCell2.Add("QV");
            listCell2.Add("QW");
            listCell2.Add("QX");
            listCell2.Add("QY");
            listCell2.Add("QZ");
            listCell2.Add("RA");
            listCell2.Add("RB");
            listCell2.Add("RC");
            listCell2.Add("RD");

            recNo = 6;
            foreach (var row in qryLast)
            {
                if (row.SectionCode != null)
                {
                    ws.Cell(listCell2[0] + recNo).Value = row.Model1;
                    ws.Cell(listCell2[0] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[1] + recNo).Value = row.Model2;
                    ws.Cell(listCell2[1] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[2] + recNo).Value = row.Model3;
                    ws.Cell(listCell2[2] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[3] + recNo).Value = row.Model4;
                    ws.Cell(listCell2[3] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[4] + recNo).Value = row.Model5;
                    ws.Cell(listCell2[4] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[5] + recNo).Value = row.Model6;
                    ws.Cell(listCell2[5] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[6] + recNo).Value = row.Model7;
                    ws.Cell(listCell2[6] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[7] + recNo).Value = row.Model8;
                    ws.Cell(listCell2[7] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[8] + recNo).Value = row.Model9;
                    ws.Cell(listCell2[8] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[9] + recNo).Value = row.Model10;
                    ws.Cell(listCell2[9] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(listCell2[10] + recNo).Value = row.Model11;
                    ws.Cell(listCell2[10] + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                if (row.Style == "YA")
                {
                    var ALlCell = ws.Range(listCell2[0] + recNo + ":" + listCell2[10] + recNo);
                    ALlCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                }
                else if (row.Style == "ALLGREY")
                {
                    var ALlCell = ws.Range(listCell2[0] + recNo + ":" + listCell2[10] + recNo);
                    ALlCell.Style.Fill.BackgroundColor = XLColor.LightGray;
                    ALlCell.Style.Font.Bold = true;
                }
                else if (row.Style == "ALLYELLOW")
                {
                    var ALlCell = ws.Range(listCell2[0] + recNo + ":" + listCell2[10] + recNo);
                    ALlCell.Style.Fill.BackgroundColor = XLColor.Yellow;
                    ALlCell.Style.Font.Bold = true;
                }

                ws.Cell(listCell2[0] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[1] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[2] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[3] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[4] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[5] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[6] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[7] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[8] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[9] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.Cell(listCell2[10] + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;


                ws.Cell(listCell2[0] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[1] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[2] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[3] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[4] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[5] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[6] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[7] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[8] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[9] + recNo).Style.Font.FontSize = 10;
                ws.Cell(listCell2[10] + recNo).Style.Font.FontSize = 10;

                recNo++;
            }
            #endregion

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public JsonResult ValidateInvoiceDate(DateTime? invoiceDate, string chassisNo)
        {
            bool result = true;
            string msgErr = "";
            var mstCompany = ctx.QaMstCompanys.Find(CurrentUser.DealerCode);
            if (mstCompany != null)
            {
                if (mstCompany.IsNonSDMS == true)
                    return Json(new { result = result, message = msgErr });
            }
            if (invoiceDate == null)
            {
                msgErr = string.Format("ChassisNo {0}, tidak bisa diisi, tgl faktur penjualan belum di-entry", chassisNo);
                return Json(new { result = false, message = msgErr });
            }

            var exception = ctx.QaMstExceptions.Find("PU", DateTime.Now.Year, DateTime.Now.Month);
            DateTime InvoiceDate = Convert.ToDateTime(invoiceDate);
            DateTime dtException = new DateTime();
            var protection = new QaMstProtection();
            DateTime dtCurrent = DateTime.Now.Date;

            if (exception != null)
            {
                dtException = new DateTime(exception.ServerYear, exception.ServerMonth, exception.MaxPrevMonthEx);
            }
            else
            {
                protection = ctx.QaMstProtections.Find("PU");
                dtException = new DateTime(DateTime.Now.Year, DateTime.Now.Month, protection.MaxPrevMonthDefault).Date;
            }

            if (DateTime.Now.Date <= dtException.Date)
            {
                if (InvoiceDate.Date < new DateTime(dtCurrent.Year, dtCurrent.Month, 1).AddMonths(-1))
                {
                    result = false;
                    msgErr = string.Format("ChassisNo {0}, tidak bisa diisi, tgl faktur penjualan {1}, melewati tgl yg ditentukan PT SIS", chassisNo, invoiceDate.Value.ToString("dd-MMM-yyyy"));
                }
            }
            else if (DateTime.Now.Date > dtException.Date)
            {
                if (InvoiceDate.Date < new DateTime(dtCurrent.Year, dtCurrent.Month, 1))
                {
                    result = false;
                    msgErr = string.Format("ChassisNo {0}, tidak bisa diisi, tgl faktur penjualan {1}, melewati tgl yg ditentukan PT SIS", chassisNo, invoiceDate);
                }
            }

            return Json(new { result = result, message = msgErr });
        }

        public JsonResult ReloadMstException()
        {
            var Event = Request["Event"];
            var PeriodYear = Request["PeriodYear"];
            var PeriodMonth = Request["PeriodMonth"];

            int Year = Convert.ToInt32(PeriodYear != "" ? PeriodYear : "0");
            int Month = Convert.ToInt32(PeriodMonth != "" ? PeriodMonth : "0");
            try
            {

                string qry = String.Format("SELECT * FROM qaMstException" +
                   " WHERE Event = @p0 AND ServerYear = @p1 AND ServerMonth = @p2");

                var data = ctx.Database.SqlQuery<QaMstException>(qry, Event, Year, Month);

                return Json(new { message = "Success", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = "Error", data = ex.InnerException });
            }
        }
    }
}
