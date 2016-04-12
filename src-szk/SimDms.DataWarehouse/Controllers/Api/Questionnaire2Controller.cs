using ClosedXML.Excel;
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
    public class Questionnaire2Controller : BaseController
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
                var statuskonsumen = Request["StatusKonsumen"];
                var ColourCode = Request["ColourCode"];
                var ColourDesc = Request["ColourDesc"];
                var namakonsumen = Request["NamaKonsumen1"];

                var IndBirthDate1 = Request["IndBirthDate1"];
                var UmurKonsumen = Request["IndUmurKonsumen1"];
                var JenisKelamin = Request["IndJenisKelamin1"];
                var IndOccupation1 = Request["IndOccupation1"];
                var OccupationOther1 = Request["OccupationOther1"];

                var Kota = Request["Kota1"];
                var CityCode = Request["CityCode"];
                var CashOrCredit = Request["CashOrCredit1"] == "1" ? true : false;
                var CreditInstalment = Request["CreditInstalment1"];

                var FleetPembelianAtasNama = Request["FleetPembelianAtasNama"];
                var FleetJenisUsaha = Request["FleetJenisUsaha"];
                var FleetPurpose = Request["FleetPurpose"];
                var PurposeOther = Request["PurposeOther"];
                var FleetPeriod = Request["FleetPeriod"];
                var FleetRenovation = Request["FleetRenovation"];

                var ProductSource1 = Request["ProductSource1"];
                var ProductSourceDetail1 = Request["ProductSourceDetail1"];
                var TestDrive1 = Request["TestDrive1"];
                var RespondenStatus1 = Request["RespondenStatus1"];
                var FirstTime1 = Request["FirstTime1"];
                var TotalCar1 = Request["TotalCar1"];
                var ReplacementReason1 = Request["ReplacementReason1"];
                var ReplacementReasonOther1 = Request["ReplacementReasonOther1"];
                var Comparison1 = Request["Comparison1"];
                var ComparisonOther1 = Request["ComparisonOther1"];
                var AspectBrand = Request["PriorityAB"];
                var AspectEngine = Request["PriorityAE"];
                var AspectExterior = Request["PriorityAEX"];
                var AspectInterior = Request["PriorityAI"];
                var AspectPrice = Request["PriorityAP"];
                var AspectAfterSales = Request["PriorityAAS"];
                var AspectOutlet = Request["PriorityAO"];
                var AspectResalePrice = Request["PriorityARP"];
                var AspectOther = Request["PriorityOT"];
                var AspectOtherDetail = Request["AspectOtherInput"];

                var ReplacementMerk1 = Request["ReplacementMerk1"];
                var ReplacementMerkOther1 = Request["ReplacementMerkOther1"];
                var ReplacementType1 = Request["ReplacementType1"];
                var ReplacementYear1 = Request["ReplacementYear1"];

                var EmployeeName = Request["EmployeeName"];


                var addtCars = Request["addtCars"];

                JArray jsonObjArr = JArray.Parse(addtCars);

                DataTable _dt = new DataTable();
                _dt.Columns.Add("IsAdditionalMerkCode", typeof(string));
                _dt.Columns.Add("IsAdditionalMerkDescI", typeof(string));
                _dt.Columns.Add("IsAdditionalMerkDescE", typeof(string));
                _dt.Columns.Add("IsAdditionalMerkOthers", typeof(string));
                _dt.Columns.Add("IsAdditionalType", typeof(string));
                _dt.Columns.Add("IsAdditionalYear", typeof(string));

                for (int i = 0; i < jsonObjArr.Count; i++)
                {
                    JObject jsonObj = (JObject)jsonObjArr[i];
                    var merkCode = jsonObj["merk"].ToString().Substring(0, 1);
                    var merk = jsonObj["merk"].ToString().Substring(3, jsonObj["merk"].ToString().Length - 3);
                    var merkOthers = "";
                    var tipe = jsonObj["tipe"];
                    var year = jsonObj["year"];

                    var merkdesce = ctx.qa2MstRefferenceCharModel.Where(p => p.RefferenceType == "Merk" && p.RefferenceCode == merkCode).Select(x => x.RefferenceDescE).FirstOrDefault();

                    if (merkCode == "K")
                    {
                        //others
                        merk = "Others";
                        merkOthers = jsonObj["merk"].ToString().Substring(3, jsonObj["merk"].ToString().Length - 3);
                    }
                    _dt.Rows.Add(merkCode, merk, merkdesce, merkOthers, tipe, year);
                }

                if (RespondenStatus1 == "A")
                {
                    ReplacementMerk1 = "";
                    ReplacementMerkOther1 = "";
                    ReplacementType1 = "";
                    ReplacementYear1 = "";
                    TotalCar1 = "";

                    _dt = new DataTable();
                    _dt.Columns.Add("IsAdditionalMerkCode", typeof(string));
                    _dt.Columns.Add("IsAdditionalMerkDescI", typeof(string));
                    _dt.Columns.Add("IsAdditionalMerkDescE", typeof(string));
                    _dt.Columns.Add("IsAdditionalMerkOthers", typeof(string));
                    _dt.Columns.Add("IsAdditionalType", typeof(string));
                    _dt.Columns.Add("IsAdditionalYear", typeof(string));
                }
                else if (RespondenStatus1 == "B")
                {
                    _dt = new DataTable();
                    _dt.Columns.Add("IsAdditionalMerkCode", typeof(string));
                    _dt.Columns.Add("IsAdditionalMerkDescI", typeof(string));
                    _dt.Columns.Add("IsAdditionalMerkDescE", typeof(string));
                    _dt.Columns.Add("IsAdditionalMerkOthers", typeof(string));
                    _dt.Columns.Add("IsAdditionalType", typeof(string));
                    _dt.Columns.Add("IsAdditionalYear", typeof(string));

                    FirstTime1 = "";
                    TotalCar1 = "";
                }
                else
                {
                    FirstTime1 = "";
                    ReplacementMerk1 = "";
                    ReplacementMerkOther1 = "";
                    ReplacementType1 = "";
                    ReplacementYear1 = "";
                }

                if (statuskonsumen == "A")
                {
                    FleetPembelianAtasNama = "";
                    FleetJenisUsaha = "";
                    FleetPurpose = "";
                    PurposeOther = "";
                    FleetPeriod = "";
                    FleetRenovation = "";
                }
                else if (statuskonsumen == "B")
                {
                    IndBirthDate1 = "";
                    UmurKonsumen = "";
                    JenisKelamin = "";
                    IndOccupation1 = "";
                    OccupationOther1 = "";
                }

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spInsertTrQuestionnaire2";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("companycode", CompanyCode ?? "");
                cmd.Parameters.AddWithValue("branchcode", BranchCode ?? "");
                cmd.Parameters.AddWithValue("salesmodelcode", salesmodelcode ?? "");
                cmd.Parameters.AddWithValue("chassiscode", chassiscode ?? "");
                cmd.Parameters.AddWithValue("chassisno", chassisno ?? "");
                cmd.Parameters.AddWithValue("rname", namakonsumen ?? "");
                cmd.Parameters.AddWithValue("rage", string.IsNullOrEmpty(UmurKonsumen) ? DBNull.Value : (object)UmurKonsumen);
                cmd.Parameters.AddWithValue("rgender", JenisKelamin);
                cmd.Parameters.AddWithValue("iscredit", CashOrCredit);
                cmd.Parameters.AddWithValue("installment", string.IsNullOrEmpty(CreditInstalment) ? DBNull.Value : (object)CreditInstalment);
                cmd.Parameters.AddWithValue("pembeliancode", FleetPembelianAtasNama ?? "");
                cmd.Parameters.AddWithValue("jenisusaha", FleetJenisUsaha ?? "");
                cmd.Parameters.AddWithValue("rstatuscode", RespondenStatus1 ?? "");
                cmd.Parameters.AddWithValue("isremerkcode", ReplacementMerk1 ?? "");
                cmd.Parameters.AddWithValue("isremerkothers", ReplacementMerkOther1 ?? "");
                cmd.Parameters.AddWithValue("isreplacementtype", ReplacementType1 ?? "");
                cmd.Parameters.AddWithValue("isreplacementyear", string.IsNullOrEmpty(ReplacementYear1) ? DBNull.Value : (object)ReplacementYear1);
                cmd.Parameters.AddWithValue("isrereasoncode", ReplacementReason1 ?? "");
                cmd.Parameters.AddWithValue("isrereasonothers", ReplacementReasonOther1 ?? "");
                cmd.Parameters.AddWithValue("isadditionaltotal", string.IsNullOrEmpty(TotalCar1) ? DBNull.Value : (object)TotalCar1);

                cmd.Parameters.AddWithValue("firsttimecode", FirstTime1 ?? "");

                cmd.Parameters.AddWithValue("occpcode", IndOccupation1 ?? "");
                cmd.Parameters.AddWithValue("occpother", OccupationOther1 ?? "");

                cmd.Parameters.AddWithValue("statuskonsumencode", statuskonsumen ?? "");
                cmd.Parameters.AddWithValue("productsourcecode", ProductSource1 ?? "");
                cmd.Parameters.AddWithValue("productsourcedetail", ProductSourceDetail1 ?? "");

                cmd.Parameters.AddWithValue("testdrivecode", TestDrive1 ?? "");
                cmd.Parameters.AddWithValue("comparisoncode", Comparison1 ?? "");
                cmd.Parameters.AddWithValue("comparisonother", ComparisonOther1 ?? "");

                cmd.Parameters.AddWithValue("aspectbrand", string.IsNullOrEmpty(AspectBrand) ? DBNull.Value : (object)AspectBrand);
                cmd.Parameters.AddWithValue("aspectengine", string.IsNullOrEmpty(AspectEngine) ? DBNull.Value : (object)AspectEngine);
                cmd.Parameters.AddWithValue("aspectexterior", string.IsNullOrEmpty(AspectExterior) ? DBNull.Value : (object)AspectExterior);
                cmd.Parameters.AddWithValue("aspectinterior", string.IsNullOrEmpty(AspectInterior) ? DBNull.Value : (object)AspectInterior);
                cmd.Parameters.AddWithValue("aspectprice", string.IsNullOrEmpty(AspectPrice) ? DBNull.Value : (object)AspectPrice);
                cmd.Parameters.AddWithValue("aspectaftersales", string.IsNullOrEmpty(AspectAfterSales) ? DBNull.Value : (object)AspectAfterSales);
                cmd.Parameters.AddWithValue("aspectoutlet", string.IsNullOrEmpty(AspectOutlet) ? DBNull.Value : (object)AspectOutlet);
                cmd.Parameters.AddWithValue("aspectresaleprice", string.IsNullOrEmpty(AspectResalePrice) ? DBNull.Value : (object)AspectResalePrice);
                cmd.Parameters.AddWithValue("aspectothers", string.IsNullOrEmpty(AspectOther) ? DBNull.Value : (object)AspectOther);

                cmd.Parameters.AddWithValue("aspectothersdetail", AspectOtherDetail ?? "");

                cmd.Parameters.AddWithValue("purposecode", FleetPurpose ?? "");
                cmd.Parameters.AddWithValue("purposeother", PurposeOther ?? "");

                cmd.Parameters.AddWithValue("periodcode", FleetPeriod ?? "");
                cmd.Parameters.AddWithValue("renovationcode", FleetRenovation ?? "");
                cmd.Parameters.AddWithValue("username", CurrentUser.Username ?? "");
                cmd.Parameters.AddWithValue("colourcode", ColourCode ?? "");
                cmd.Parameters.AddWithValue("colourdesc", ColourDesc ?? "");
                cmd.Parameters.AddWithValue("birthdate", IndBirthDate1 ?? "");

                cmd.Parameters.AddWithValue("citycode", CityCode ?? "");
                cmd.Parameters.AddWithValue("citydesc", Kota ?? "");

                cmd.Parameters.AddWithValue("ttQa2Sub", _dt);

                cmd.Parameters.AddWithValue("employeename", EmployeeName);


                //cmd.Parameters.AddWithValue("isaddsuzuki", string.IsNullOrEmpty(AsVehicleAdditionalSuzuki) ? DBNull.Value : (object)AsVehicleAdditionalSuzuki);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);

                isupdated = ds.Tables[0].Rows[0]["EndResult"].ToString() == "2" ? true : false;

                bool isValidData = ds.Tables[0].Rows[0]["EndResult"].ToString() == "0" ? false : true;

                isUpdateFailed = ds.Tables[0].Rows[0]["EndResult"].ToString() == "3" ? true : false;

                result.isValid = isValidData & !isUpdateFailed;

                result.message = ds.Tables[0].Rows[0]["EndResult"].ToString() == "0" ? "Data tidak berhasil karena tidak valid" : ds.Tables[0].Rows[0]["EndResult"].ToString() == "1" ? "Data has been saved" : ds.Tables[0].Rows[0]["EndResult"].ToString() == "2" ? "Data has been updated" : ds.Tables[0].Rows[0]["EndResult"].ToString() == "3" ? "Data tidak dapat diubah karena sudah diinput lewat dari sehari" : "";

                result.status = true;
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
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spGetQuestionnaireDetail2";
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
                cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spRemoveQuestionnaire2";
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

        public JsonResult getTrQaSub()
        {
            var chassiscode = Request["ChassisCode"];
            var chassisno = Request["ChassisNo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_getQaTrSub";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("companycode", CompanyCode ?? "");
            cmd.Parameters.AddWithValue("branchcode", BranchCode ?? "");
            cmd.Parameters.AddWithValue("chassiscode", chassiscode ?? "");
            cmd.Parameters.AddWithValue("chassisno", chassisno ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        string IndexToColumn(int index)
        {
            string[] columns = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N","O", "P", "Q", "R", "S", "T", "U", "V","W","X","Y","Z",
                               "AA", "AB", "AC", "AD", "AE", "AF", "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN","AO", "AP", "AQ", "AR", "AS", "AT", "AU", "AV","AW","AX","AY","AZ",
                               "BA", "BB", "BC", "BD", "BE", "BF", "BG", "BH", "BI", "BJ", "BK", "BL", "BM", "BN","BO", "BP", "BQ", "BR", "BS", "BT", "BU", "BV","BW","BX","BY","BZ",
                               "CA", "CB", "CC", "CD", "CE", "CF", "CG", "CH", "CI", "CJ", "CK", "CL", "CM", "CN","CO", "CP", "CQ", "CR", "CS", "CT", "CU", "CV","CW","CX","CY","CZ"};

            if (index <= 0)
            {
                throw new IndexOutOfRangeException("Index must be a possitive number");
            }

            return columns[index - 1];
        }

        public ActionResult generateQaRowData()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire2_Row_Data" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var TypeEvent = Request.Params["Event"] == "A" ? "Ertiga" : Request.Params["Event"] == "B" ? "WagonR" : "CBU";
            var statusKonsumen = Request.Params["StatusKonsumen"]; // == "" ? "%" : Request.Params["StatusKonsumen"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var qry = ctx.Database.SqlQuery<Qa2RowModel>("exec uspfn_SpGenerateQa2RowData @dateStart=@p0, @dateEnd=@p1, @statusKonsumen=@p2, @event=@p3", dateStart, dateEnd, statusKonsumen, TypeEvent).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Qa2_Row_Data");

            var rngTable = ws.Range("A1:CE1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.WhiteSmoke;

            ws.Columns("1").Width = 15;
            ws.Columns("2", "83").Width = 40;

            if (statusKonsumen == "A")
            {
                ws.Cell("A" + recNo).Value = "Area";
                ws.Cell("B" + recNo).Value = "Dealer";
                ws.Cell("C" + recNo).Value = "Outlet";
                ws.Cell("D" + recNo).Value = "Input Date";
                ws.Cell("E" + recNo).Value = "Status Konsumen";
                ws.Cell("F" + recNo).Value = "Chassis Code";
                ws.Cell("G" + recNo).Value = "Chassis No";
                ws.Cell("H" + recNo).Value = "Colour Desc";
                ws.Cell("I" + recNo).Value = "Sales Model";
                ws.Cell("J" + recNo).Value = "Employee Name";
                ws.Cell("K" + recNo).Value = "Responden Name";
                ws.Cell("L" + recNo).Value = "Tanggal Lahir";
                ws.Cell("M" + recNo).Value = "Age";
                ws.Cell("N" + recNo).Value = "Gender";
                ws.Cell("O" + recNo).Value = "Kota Dimisili";
                ws.Cell("P" + recNo).Value = "Payment";
                ws.Cell("Q" + recNo).Value = "Installment";
                ws.Cell("R" + recNo).Value = "(1)Occupation";
                ws.Cell("S" + recNo).Value = "(1)Occupation Others";
                ws.Cell("T" + recNo).Value = "(2)Product Source";
                ws.Cell("U" + recNo).Value = "(2)Product Source Detail";
                ws.Cell("V" + recNo).Value = "(3)Test Drive";
                ws.Cell("W" + recNo).Value = "(4)Responden Status";
                ws.Cell("X" + recNo).Value = "(4)First Time";
                ws.Cell("Y" + recNo).Value = "(4)Replacement Merk";
                ws.Cell("Z" + recNo).Value = "(4)Replacement Merk Other";
                ws.Cell("AA" + recNo).Value = "(4)Replacement Type";
                ws.Cell("AB" + recNo).Value = "(4)Replacement Year";

                ws.Cell("AC" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AD" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AE" + recNo).Value = "(4)Additional Type";
                ws.Cell("AF" + recNo).Value = "(4)Additional Year";
                ws.Cell("AG" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AH" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AI" + recNo).Value = "(4)Additional Type";
                ws.Cell("AJ" + recNo).Value = "(4)Additional Year";
                ws.Cell("AK" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AL" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AM" + recNo).Value = "(4)Additional Type";
                ws.Cell("AN" + recNo).Value = "(4)Additional Year";
                ws.Cell("AO" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AP" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AQ" + recNo).Value = "(4)Additional Type";
                ws.Cell("AR" + recNo).Value = "(4)Additional Year";
                ws.Cell("AS" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AT" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AU" + recNo).Value = "(4)Additional Type";
                ws.Cell("AV" + recNo).Value = "(4)Additional Year";
                ws.Cell("AW" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AX" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AY" + recNo).Value = "(4)Additional Type";
                ws.Cell("AZ" + recNo).Value = "(4)Additional Year";
                ws.Cell("BA" + recNo).Value = "(4)Additional Merk";
                ws.Cell("BB" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("BC" + recNo).Value = "(4)Additional Type";
                ws.Cell("BD" + recNo).Value = "(4)Additional Year";
                ws.Cell("BE" + recNo).Value = "(4)Additional Merk";
                ws.Cell("BF" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("BG" + recNo).Value = "(4)Additional Type";
                ws.Cell("BH" + recNo).Value = "(4)Additional Year";
                ws.Cell("BI" + recNo).Value = "(4)Additional Merk";
                ws.Cell("BJ" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("BK" + recNo).Value = "(4)Additional Type";
                ws.Cell("BL" + recNo).Value = "(4)Additional Year";
                ws.Cell("BM" + recNo).Value = "(4)Additional Merk";
                ws.Cell("BN" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("BO" + recNo).Value = "(4)Additional Type";
                ws.Cell("BP" + recNo).Value = "(4)Additional Year";
                ws.Cell("BQ" + recNo).Value = "(4)Additional Total";
                ws.Cell("BR" + recNo).Value = "(5)Replacement Reason";
                ws.Cell("BS" + recNo).Value = "(5)Replacement Reason Other";
                ws.Cell("BT" + recNo).Value = "(7)Comparison";
                ws.Cell("BU" + recNo).Value = "(7)Comparison Other";
                ws.Cell("BV" + recNo).Value = "(8)Aspect Brand";
                ws.Cell("BW" + recNo).Value = "(8)Aspect Engine";
                ws.Cell("BX" + recNo).Value = "(8)Aspect Exterior";
                ws.Cell("BY" + recNo).Value = "(8)Aspect Interior";
                ws.Cell("BZ" + recNo).Value = "(8)Aspect Price";
                ws.Cell("CA" + recNo).Value = "(8)Aspect Aftersales";
                ws.Cell("CB" + recNo).Value = "(8)Aspect Outlet";
                ws.Cell("CC" + recNo).Value = "(8)Aspect ResalePrice";
                ws.Cell("CD" + recNo).Value = "(8)Aspect Other";
                ws.Cell("CE" + recNo).Value = "(8)Aspect Other Detail";
            }
            else
            {
                ws.Cell("A" + recNo).Value = "Area";
                ws.Cell("B" + recNo).Value = "Dealer";
                ws.Cell("C" + recNo).Value = "Outlet";
                ws.Cell("D" + recNo).Value = "Input Date";
                ws.Cell("E" + recNo).Value = "Status Konsumen";
                ws.Cell("F" + recNo).Value = "Chassis Code";
                ws.Cell("G" + recNo).Value = "Chassis No";
                ws.Cell("H" + recNo).Value = "Colour Desc";
                ws.Cell("I" + recNo).Value = "Sales Model";
                ws.Cell("J" + recNo).Value = "Employee Name";
                ws.Cell("K" + recNo).Value = "Pembelian Atas Nama";
                ws.Cell("L" + recNo).Value = "Responden Name";
                ws.Cell("M" + recNo).Value = "Jenis Usaha";
                ws.Cell("N" + recNo).Value = "Kota Dimisili";
                ws.Cell("O" + recNo).Value = "Payment";
                ws.Cell("P" + recNo).Value = "Installment";
                ws.Cell("Q" + recNo).Value = "(1)Purpose";
                ws.Cell("R" + recNo).Value = "(1)Purpose Other";
                ws.Cell("S" + recNo).Value = "(2)Period";
                ws.Cell("T" + recNo).Value = "(3)Renovation";
                ws.Cell("U" + recNo).Value = "(4)Product Source";
                ws.Cell("V" + recNo).Value = "(4)Product Source Detail";
                ws.Cell("W" + recNo).Value = "(5)Test Drive";
                ws.Cell("X" + recNo).Value = "(6)Responden Status";
                ws.Cell("Y" + recNo).Value = "(6)Replacement Merk";
                ws.Cell("Z" + recNo).Value = "(6)Replacement Merk Other";
                ws.Cell("AA" + recNo).Value = "(6)Replacement Type";
                ws.Cell("AB" + recNo).Value = "(6)Replacement Year";

                ws.Cell("AC" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AD" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AE" + recNo).Value = "(6)Additional Type";
                ws.Cell("AF" + recNo).Value = "(6)Additional Year";
                ws.Cell("AG" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AH" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AI" + recNo).Value = "(6)Additional Type";
                ws.Cell("AJ" + recNo).Value = "(6)Additional Year";
                ws.Cell("AK" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AL" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AM" + recNo).Value = "(6)Additional Type";
                ws.Cell("AN" + recNo).Value = "(6)Additional Year";
                ws.Cell("AO" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AP" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AQ" + recNo).Value = "(6)Additional Type";
                ws.Cell("AR" + recNo).Value = "(6)Additional Year";
                ws.Cell("AS" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AT" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AU" + recNo).Value = "(6)Additional Type";
                ws.Cell("AV" + recNo).Value = "(6)Additional Year";
                ws.Cell("AW" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AX" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AY" + recNo).Value = "(6)Additional Type";
                ws.Cell("AZ" + recNo).Value = "(6)Additional Year";
                ws.Cell("BA" + recNo).Value = "(6)Additional Merk";
                ws.Cell("BB" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("BC" + recNo).Value = "(6)Additional Type";
                ws.Cell("BD" + recNo).Value = "(6)Additional Year";
                ws.Cell("BE" + recNo).Value = "(6)Additional Merk";
                ws.Cell("BF" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("BG" + recNo).Value = "(6)Additional Type";
                ws.Cell("BH" + recNo).Value = "(6)Additional Year";
                ws.Cell("BI" + recNo).Value = "(6)Additional Merk";
                ws.Cell("BJ" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("BK" + recNo).Value = "(6)Additional Type";
                ws.Cell("BL" + recNo).Value = "(6)Additional Year";
                ws.Cell("BM" + recNo).Value = "(6)Additional Merk";
                ws.Cell("BN" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("BO" + recNo).Value = "(6)Additional Type";
                ws.Cell("BP" + recNo).Value = "(6)Additional Year";
                ws.Cell("BQ" + recNo).Value = "(6)Additional Total";
                ws.Cell("BR" + recNo).Value = "(7)Replacement Reason";
                ws.Cell("BS" + recNo).Value = "(7)Replacement Reason Other";
                ws.Cell("BT" + recNo).Value = "(8)Comparison";
                ws.Cell("BU" + recNo).Value = "(8)Comparison Other";
                ws.Cell("BV" + recNo).Value = "(9)Aspect Brand";
                ws.Cell("BW" + recNo).Value = "(9)Aspect Engine";
                ws.Cell("BX" + recNo).Value = "(9)Aspect Exterior";
                ws.Cell("BY" + recNo).Value = "(9)Aspect Interior";
                ws.Cell("BZ" + recNo).Value = "(9)Aspect Price";
                ws.Cell("CA" + recNo).Value = "(9)Aspect Aftersales";
                ws.Cell("CB" + recNo).Value = "(9)Aspect Outlet";
                ws.Cell("CC" + recNo).Value = "(9)Aspect ResalePrice";
                ws.Cell("CD" + recNo).Value = "(9)Aspect Other";
                ws.Cell("CE" + recNo).Value = "(9)Aspect Other Detail";
            }

            recNo++;

            DataContext ctx2 = new DataContext();

            foreach (var row in qry)
            {
                if (statusKonsumen == "A")
                {
                    ws.Cell("A" + recNo).Value = row.Area;
                    ws.Cell("B" + recNo).Value = row.CompanyName;
                    ws.Cell("C" + recNo).Value = row.BranchName;
                    ws.Cell("D" + recNo).Value = row.TransactionDate.ToString("dd/MM/yyyy");
                    ws.Cell("E" + recNo).Value = row.StatusKonsumenDescE;
                    ws.Cell("F" + recNo).Value = row.ChassisCode;
                    ws.Cell("G" + recNo).Value = row.ChassisNo;
                    ws.Cell("H" + recNo).Value = row.RefferenceDesc1;
                    ws.Cell("I" + recNo).Value = row.SalesModelReport;
                    ws.Cell("J" + recNo).Value = row.EmployeeName;
                    ws.Cell("K" + recNo).Value = row.RespondenName;
                    ws.Cell("L" + recNo).Value = row.BirthDate.ToString("dd/MM/yyyy");
                    ws.Cell("M" + recNo).Value = row.RespondenAge;
                    ws.Cell("N" + recNo).Value = row.RespondenGender;
                    ws.Cell("O" + recNo).Value = row.LookUpValueName;
                    ws.Cell("P" + recNo).Value = row.IsCredit;
                    ws.Cell("Q" + recNo).Value = row.Installment;
                    ws.Cell("R" + recNo).Value = row.OccupationDescE;
                    ws.Cell("S" + recNo).Value = row.OccupationOthers;
                    ws.Cell("T" + recNo).Value = row.ProductSourceDescE;
                    ws.Cell("U" + recNo).Value = row.ProductSourceDetail;
                    ws.Cell("V" + recNo).Value = row.TestDrivedescE;
                    ws.Cell("W" + recNo).Value = row.RespondenStatusDescE;
                    ws.Cell("X" + recNo).Value = row.FirstTimeDescE;
                    ws.Cell("Y" + recNo).Value = row.IsReplacementMerkDescE;
                    ws.Cell("Z" + recNo).Value = row.IsReplacementMerkOthers;
                    ws.Cell("AA" + recNo).Value = row.IsReplacementType;
                    ws.Cell("AB" + recNo).Value = row.IsReplacementYear;
                }
                else
                {
                    ws.Cell("A" + recNo).Value = row.Area;
                    ws.Cell("B" + recNo).Value = row.CompanyName;
                    ws.Cell("C" + recNo).Value = row.BranchName;
                    ws.Cell("D" + recNo).Value = row.TransactionDate.ToString("dd/MM/yyyy");
                    ws.Cell("E" + recNo).Value = row.StatusKonsumenDescE;
                    ws.Cell("F" + recNo).Value = row.ChassisCode;
                    ws.Cell("G" + recNo).Value = row.ChassisNo;
                    ws.Cell("H" + recNo).Value = row.RefferenceDesc1;
                    ws.Cell("I" + recNo).Value = row.SalesModelReport;
                    ws.Cell("J" + recNo).Value = row.EmployeeName;
                    ws.Cell("K" + recNo).Value = row.PembelianAtasNamaDescE;
                    ws.Cell("L" + recNo).Value = row.RespondenName;
                    ws.Cell("M" + recNo).Value = row.JenisUsaha;
                    ws.Cell("N" + recNo).Value = row.LookUpValueName;
                    ws.Cell("O" + recNo).Value = row.IsCredit;
                    ws.Cell("P" + recNo).Value = row.Installment;
                    ws.Cell("Q" + recNo).Value = row.PurposeDescE;
                    ws.Cell("R" + recNo).Value = row.PurposeOthers;
                    ws.Cell("S" + recNo).Value = row.PeriodDescE;
                    ws.Cell("T" + recNo).Value = row.RenovationDescE;
                    ws.Cell("U" + recNo).Value = row.ProductSourceDescE;
                    ws.Cell("V" + recNo).Value = row.ProductSourceDetail;
                    ws.Cell("W" + recNo).Value = row.TestDrivedescE;
                    ws.Cell("X" + recNo).Value = row.RespondenStatusDescE;
                    ws.Cell("Y" + recNo).Value = row.IsReplacementMerkDescE;
                    ws.Cell("Z" + recNo).Value = row.IsReplacementMerkOthers;
                    ws.Cell("AA" + recNo).Value = row.IsReplacementType;
                    ws.Cell("AB" + recNo).Value = row.IsReplacementYear;
                }

                var qrysub = ctx2.Database.SqlQuery<Qa2RowDataSub>("exec uspfn_getQaTrSub @companycode=@p0, @branchcode=@p1, @chassiscode=@p2, @chassisno=@p3", row.CompanyCode, row.BranchCode, row.ChassisCode, row.ChassisNo).AsQueryable();
                int recSub = 29;
                foreach (var item in qrysub)
                {
                    ws.Cell(IndexToColumn(recSub) + recNo).Value = item.IsAdditionalMerkDescE;
                    recSub++;
                    ws.Cell(IndexToColumn(recSub) + recNo).Value = item.IsAdditionalMerkOthers;
                    recSub++;
                    ws.Cell(IndexToColumn(recSub) + recNo).Value = item.IsAdditionalType;
                    recSub++;
                    ws.Cell(IndexToColumn(recSub) + recNo).Value = item.IsAdditionalYear;
                    recSub++;
                }
                ws.Cell("BQ" + recNo).Value = row.IsAdditionalTotal;
                ws.Cell("BR" + recNo).Value = row.IsReplacementReasonDescE;
                ws.Cell("BS" + recNo).Value = row.IsReplacementReasonOthers;
                ws.Cell("BT" + recNo).Value = row.ComparisonDescE;
                ws.Cell("BU" + recNo).Value = row.ComparisonOthers;
                ws.Cell("BV" + recNo).Value = row.AspectBrand;
                ws.Cell("BW" + recNo).Value = row.AspectEngine;
                ws.Cell("BX" + recNo).Value = row.AspectExterior;
                ws.Cell("BY" + recNo).Value = row.AspectInterior;
                ws.Cell("BZ" + recNo).Value = row.AspectPrice;
                ws.Cell("CA" + recNo).Value = row.AspectAfterSales;
                ws.Cell("CB" + recNo).Value = row.AspectOutlet;
                ws.Cell("CC" + recNo).Value = row.AspectResalePrice;
                ws.Cell("CD" + recNo).Value = row.AspectOthers;
                ws.Cell("CE" + recNo).Value = row.AspectOthersDetail;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult generateQaRowDataDealer()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire2_Row_Data" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var TypeEvent = Request.Params["Event"] == "A" ? "Ertiga" : Request.Params["Event"] == "B" ? "WagonR" : "CBU";
            var statusKonsumen = Request.Params["StatusKonsumen"]; // == "" ? "%" : Request.Params["StatusKonsumen"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var qry = ctx.Database.SqlQuery<Qa2RowModel>("exec uspfn_SpGenerateQa2RowData @companyCode=@p0, @branchCode=@p1, @dateStart=@p2, @dateEnd=@p3, @statusKonsumen=@p4, @event=@p5", CompanyCode, BranchCode, dateStart, dateEnd, statusKonsumen, TypeEvent).AsQueryable();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Qa2_Row_Data");

            var rngTable = ws.Range("A1:CE1");

            rngTable.Style
                .Font.SetBold()
                .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngTable.Style.Font.Bold = true;
            rngTable.Style.Font.FontColor = XLColor.WhiteSmoke;

            ws.Columns("1").Width = 15;
            ws.Columns("2", "83").Width = 40;

            if (statusKonsumen == "A")
            {
                ws.Cell("A" + recNo).Value = "Area";
                ws.Cell("B" + recNo).Value = "Dealer";
                ws.Cell("C" + recNo).Value = "Outlet";
                ws.Cell("D" + recNo).Value = "Input Date";
                ws.Cell("E" + recNo).Value = "Status Konsumen";
                ws.Cell("F" + recNo).Value = "Chassis Code";
                ws.Cell("G" + recNo).Value = "Chassis No";
                ws.Cell("H" + recNo).Value = "Colour Desc";
                ws.Cell("I" + recNo).Value = "Sales Model";
                ws.Cell("J" + recNo).Value = "Employee Name";
                ws.Cell("K" + recNo).Value = "Responden Name";
                ws.Cell("L" + recNo).Value = "Tanggal Lahir";
                ws.Cell("M" + recNo).Value = "Age";
                ws.Cell("N" + recNo).Value = "Gender";
                ws.Cell("O" + recNo).Value = "Kota Dimisili";
                ws.Cell("P" + recNo).Value = "Payment";
                ws.Cell("Q" + recNo).Value = "Installment";
                ws.Cell("R" + recNo).Value = "(1)Occupation";
                ws.Cell("S" + recNo).Value = "(1)Occupation Others";
                ws.Cell("T" + recNo).Value = "(2)Product Source";
                ws.Cell("U" + recNo).Value = "(2)Product Source Detail";
                ws.Cell("V" + recNo).Value = "(3)Test Drive";
                ws.Cell("W" + recNo).Value = "(4)Responden Status";
                ws.Cell("X" + recNo).Value = "(4)First Time";
                ws.Cell("Y" + recNo).Value = "(4)Replacement Merk";
                ws.Cell("Z" + recNo).Value = "(4)Replacement Merk Other";
                ws.Cell("AA" + recNo).Value = "(4)Replacement Type";
                ws.Cell("AB" + recNo).Value = "(4)Replacement Year";

                ws.Cell("AC" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AD" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AE" + recNo).Value = "(4)Additional Type";
                ws.Cell("AF" + recNo).Value = "(4)Additional Year";
                ws.Cell("AG" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AH" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AI" + recNo).Value = "(4)Additional Type";
                ws.Cell("AJ" + recNo).Value = "(4)Additional Year";
                ws.Cell("AK" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AL" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AM" + recNo).Value = "(4)Additional Type";
                ws.Cell("AN" + recNo).Value = "(4)Additional Year";
                ws.Cell("AO" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AP" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AQ" + recNo).Value = "(4)Additional Type";
                ws.Cell("AR" + recNo).Value = "(4)Additional Year";
                ws.Cell("AS" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AT" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AU" + recNo).Value = "(4)Additional Type";
                ws.Cell("AV" + recNo).Value = "(4)Additional Year";
                ws.Cell("AW" + recNo).Value = "(4)Additional Merk";
                ws.Cell("AX" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("AY" + recNo).Value = "(4)Additional Type";
                ws.Cell("AZ" + recNo).Value = "(4)Additional Year";
                ws.Cell("BA" + recNo).Value = "(4)Additional Merk";
                ws.Cell("BB" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("BC" + recNo).Value = "(4)Additional Type";
                ws.Cell("BD" + recNo).Value = "(4)Additional Year";
                ws.Cell("BE" + recNo).Value = "(4)Additional Merk";
                ws.Cell("BF" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("BG" + recNo).Value = "(4)Additional Type";
                ws.Cell("BH" + recNo).Value = "(4)Additional Year";
                ws.Cell("BI" + recNo).Value = "(4)Additional Merk";
                ws.Cell("BJ" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("BK" + recNo).Value = "(4)Additional Type";
                ws.Cell("BL" + recNo).Value = "(4)Additional Year";
                ws.Cell("BM" + recNo).Value = "(4)Additional Merk";
                ws.Cell("BN" + recNo).Value = "(4)Additional Merk Other";
                ws.Cell("BO" + recNo).Value = "(4)Additional Type";
                ws.Cell("BP" + recNo).Value = "(4)Additional Year";
                ws.Cell("BQ" + recNo).Value = "(4)Additional Total";
                ws.Cell("BR" + recNo).Value = "(5)Replacement Reason";
                ws.Cell("BS" + recNo).Value = "(5)Replacement Reason Other";
                ws.Cell("BT" + recNo).Value = "(7)Comparison";
                ws.Cell("BU" + recNo).Value = "(7)Comparison Other";
                ws.Cell("BV" + recNo).Value = "(8)Aspect Brand";
                ws.Cell("BW" + recNo).Value = "(8)Aspect Engine";
                ws.Cell("BX" + recNo).Value = "(8)Aspect Exterior";
                ws.Cell("BY" + recNo).Value = "(8)Aspect Interior";
                ws.Cell("BZ" + recNo).Value = "(8)Aspect Price";
                ws.Cell("CA" + recNo).Value = "(8)Aspect Aftersales";
                ws.Cell("CB" + recNo).Value = "(8)Aspect Outlet";
                ws.Cell("CC" + recNo).Value = "(8)Aspect ResalePrice";
                ws.Cell("CD" + recNo).Value = "(8)Aspect Other";
                ws.Cell("CE" + recNo).Value = "(8)Aspect Other Detail";
            }
            else
            {
                ws.Cell("A" + recNo).Value = "Area";
                ws.Cell("B" + recNo).Value = "Dealer";
                ws.Cell("C" + recNo).Value = "Outlet";
                ws.Cell("D" + recNo).Value = "Input Date";
                ws.Cell("E" + recNo).Value = "Status Konsumen";
                ws.Cell("F" + recNo).Value = "Chassis Code";
                ws.Cell("G" + recNo).Value = "Chassis No";
                ws.Cell("H" + recNo).Value = "Colour Desc";
                ws.Cell("I" + recNo).Value = "Sales Model";
                ws.Cell("J" + recNo).Value = "Employee Name";
                ws.Cell("K" + recNo).Value = "Pembelian Atas Nama";
                ws.Cell("L" + recNo).Value = "Responden Name";
                ws.Cell("M" + recNo).Value = "Jenis Usaha";
                ws.Cell("N" + recNo).Value = "Kota Dimisili";
                ws.Cell("O" + recNo).Value = "Payment";
                ws.Cell("P" + recNo).Value = "Installment";
                ws.Cell("Q" + recNo).Value = "(1)Purpose";
                ws.Cell("R" + recNo).Value = "(1)Purpose Other";
                ws.Cell("S" + recNo).Value = "(2)Period";
                ws.Cell("T" + recNo).Value = "(3)Renovation";
                ws.Cell("U" + recNo).Value = "(4)Product Source";
                ws.Cell("V" + recNo).Value = "(4)Product Source Detail";
                ws.Cell("W" + recNo).Value = "(5)Test Drive";
                ws.Cell("X" + recNo).Value = "(6)Responden Status";
                ws.Cell("Y" + recNo).Value = "(6)Replacement Merk";
                ws.Cell("Z" + recNo).Value = "(6)Replacement Merk Other";
                ws.Cell("AA" + recNo).Value = "(6)Replacement Type";
                ws.Cell("AB" + recNo).Value = "(6)Replacement Year";

                ws.Cell("AC" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AD" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AE" + recNo).Value = "(6)Additional Type";
                ws.Cell("AF" + recNo).Value = "(6)Additional Year";
                ws.Cell("AG" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AH" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AI" + recNo).Value = "(6)Additional Type";
                ws.Cell("AJ" + recNo).Value = "(6)Additional Year";
                ws.Cell("AK" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AL" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AM" + recNo).Value = "(6)Additional Type";
                ws.Cell("AN" + recNo).Value = "(6)Additional Year";
                ws.Cell("AO" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AP" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AQ" + recNo).Value = "(6)Additional Type";
                ws.Cell("AR" + recNo).Value = "(6)Additional Year";
                ws.Cell("AS" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AT" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AU" + recNo).Value = "(6)Additional Type";
                ws.Cell("AV" + recNo).Value = "(6)Additional Year";
                ws.Cell("AW" + recNo).Value = "(6)Additional Merk";
                ws.Cell("AX" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("AY" + recNo).Value = "(6)Additional Type";
                ws.Cell("AZ" + recNo).Value = "(6)Additional Year";
                ws.Cell("BA" + recNo).Value = "(6)Additional Merk";
                ws.Cell("BB" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("BC" + recNo).Value = "(6)Additional Type";
                ws.Cell("BD" + recNo).Value = "(6)Additional Year";
                ws.Cell("BE" + recNo).Value = "(6)Additional Merk";
                ws.Cell("BF" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("BG" + recNo).Value = "(6)Additional Type";
                ws.Cell("BH" + recNo).Value = "(6)Additional Year";
                ws.Cell("BI" + recNo).Value = "(6)Additional Merk";
                ws.Cell("BJ" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("BK" + recNo).Value = "(6)Additional Type";
                ws.Cell("BL" + recNo).Value = "(6)Additional Year";
                ws.Cell("BM" + recNo).Value = "(6)Additional Merk";
                ws.Cell("BN" + recNo).Value = "(6)Additional Merk Other";
                ws.Cell("BO" + recNo).Value = "(6)Additional Type";
                ws.Cell("BP" + recNo).Value = "(6)Additional Year";
                ws.Cell("BQ" + recNo).Value = "(6)Additional Total";
                ws.Cell("BR" + recNo).Value = "(7)Replacement Reason";
                ws.Cell("BS" + recNo).Value = "(7)Replacement Reason Other";
                ws.Cell("BT" + recNo).Value = "(8)Comparison";
                ws.Cell("BU" + recNo).Value = "(8)Comparison Other";
                ws.Cell("BV" + recNo).Value = "(9)Aspect Brand";
                ws.Cell("BW" + recNo).Value = "(9)Aspect Engine";
                ws.Cell("BX" + recNo).Value = "(9)Aspect Exterior";
                ws.Cell("BY" + recNo).Value = "(9)Aspect Interior";
                ws.Cell("BZ" + recNo).Value = "(9)Aspect Price";
                ws.Cell("CA" + recNo).Value = "(9)Aspect Aftersales";
                ws.Cell("CB" + recNo).Value = "(9)Aspect Outlet";
                ws.Cell("CC" + recNo).Value = "(9)Aspect ResalePrice";
                ws.Cell("CD" + recNo).Value = "(9)Aspect Other";
                ws.Cell("CE" + recNo).Value = "(9)Aspect Other Detail";
            }

            recNo++;

            DataContext ctx2 = new DataContext();

            foreach (var row in qry)
            {
                if (statusKonsumen == "A")
                {
                    ws.Cell("A" + recNo).Value = row.Area;
                    ws.Cell("B" + recNo).Value = row.CompanyName;
                    ws.Cell("C" + recNo).Value = row.BranchName;
                    ws.Cell("D" + recNo).Value = row.TransactionDate.ToString("dd/MM/yyyy");
                    ws.Cell("E" + recNo).Value = row.StatusKonsumenDescE;
                    ws.Cell("F" + recNo).Value = row.ChassisCode;
                    ws.Cell("G" + recNo).Value = row.ChassisNo;
                    ws.Cell("H" + recNo).Value = row.RefferenceDesc1;
                    ws.Cell("I" + recNo).Value = row.SalesModelReport;
                    ws.Cell("J" + recNo).Value = row.EmployeeName;
                    ws.Cell("K" + recNo).Value = row.RespondenName;
                    ws.Cell("L" + recNo).Value = row.BirthDate.ToString("dd/MM/yyyy");
                    ws.Cell("M" + recNo).Value = row.RespondenAge;
                    ws.Cell("N" + recNo).Value = row.RespondenGender;
                    ws.Cell("O" + recNo).Value = row.LookUpValueName;
                    ws.Cell("P" + recNo).Value = row.IsCredit;
                    ws.Cell("Q" + recNo).Value = row.Installment;
                    ws.Cell("R" + recNo).Value = row.OccupationDescE;
                    ws.Cell("S" + recNo).Value = row.OccupationOthers;
                    ws.Cell("T" + recNo).Value = row.ProductSourceDescE;
                    ws.Cell("U" + recNo).Value = row.ProductSourceDetail;
                    ws.Cell("V" + recNo).Value = row.TestDrivedescE;
                    ws.Cell("W" + recNo).Value = row.RespondenStatusDescE;
                    ws.Cell("X" + recNo).Value = row.FirstTimeDescE;
                    ws.Cell("Y" + recNo).Value = row.IsReplacementMerkDescE;
                    ws.Cell("Z" + recNo).Value = row.IsReplacementMerkOthers;
                    ws.Cell("AA" + recNo).Value = row.IsReplacementType;
                    ws.Cell("AB" + recNo).Value = row.IsReplacementYear;
                }
                else
                {
                    ws.Cell("A" + recNo).Value = row.Area;
                    ws.Cell("B" + recNo).Value = row.CompanyName;
                    ws.Cell("C" + recNo).Value = row.BranchName;
                    ws.Cell("D" + recNo).Value = row.TransactionDate.ToString("dd/MM/yyyy");
                    ws.Cell("E" + recNo).Value = row.StatusKonsumenDescE;
                    ws.Cell("F" + recNo).Value = row.ChassisCode;
                    ws.Cell("G" + recNo).Value = row.ChassisNo;
                    ws.Cell("H" + recNo).Value = row.RefferenceDesc1;
                    ws.Cell("I" + recNo).Value = row.SalesModelReport;
                    ws.Cell("J" + recNo).Value = row.EmployeeName;
                    ws.Cell("K" + recNo).Value = row.PembelianAtasNamaDescE;
                    ws.Cell("L" + recNo).Value = row.RespondenName;
                    ws.Cell("M" + recNo).Value = row.JenisUsaha;
                    ws.Cell("N" + recNo).Value = row.LookUpValueName;
                    ws.Cell("O" + recNo).Value = row.IsCredit;
                    ws.Cell("P" + recNo).Value = row.Installment;
                    ws.Cell("Q" + recNo).Value = row.PurposeDescE;
                    ws.Cell("R" + recNo).Value = row.PurposeOthers;
                    ws.Cell("S" + recNo).Value = row.PeriodDescE;
                    ws.Cell("T" + recNo).Value = row.RenovationDescE;
                    ws.Cell("U" + recNo).Value = row.ProductSourceDescE;
                    ws.Cell("V" + recNo).Value = row.ProductSourceDetail;
                    ws.Cell("W" + recNo).Value = row.TestDrivedescE;
                    ws.Cell("X" + recNo).Value = row.RespondenStatusDescE;
                    ws.Cell("Y" + recNo).Value = row.IsReplacementMerkDescE;
                    ws.Cell("Z" + recNo).Value = row.IsReplacementMerkOthers;
                    ws.Cell("AA" + recNo).Value = row.IsReplacementType;
                    ws.Cell("AB" + recNo).Value = row.IsReplacementYear;
                }

                var qrysub = ctx2.Database.SqlQuery<Qa2RowDataSub>("exec uspfn_getQaTrSub @companycode=@p0, @branchcode=@p1, @chassiscode=@p2, @chassisno=@p3", row.CompanyCode, row.BranchCode, row.ChassisCode, row.ChassisNo).AsQueryable();
                int recSub = 29;
                foreach (var item in qrysub)
                {
                    ws.Cell(IndexToColumn(recSub) + recNo).Value = item.IsAdditionalMerkDescE;
                    recSub++;
                    ws.Cell(IndexToColumn(recSub) + recNo).Value = item.IsAdditionalMerkOthers;
                    recSub++;
                    ws.Cell(IndexToColumn(recSub) + recNo).Value = item.IsAdditionalType;
                    recSub++;
                    ws.Cell(IndexToColumn(recSub) + recNo).Value = item.IsAdditionalYear;
                    recSub++;
                }
                ws.Cell("BQ" + recNo).Value = row.IsAdditionalTotal;
                ws.Cell("BR" + recNo).Value = row.IsReplacementReasonDescE;
                ws.Cell("BS" + recNo).Value = row.IsReplacementReasonOthers;
                ws.Cell("BT" + recNo).Value = row.ComparisonDescE;
                ws.Cell("BU" + recNo).Value = row.ComparisonOthers;
                ws.Cell("BV" + recNo).Value = row.AspectBrand;
                ws.Cell("BW" + recNo).Value = row.AspectEngine;
                ws.Cell("BX" + recNo).Value = row.AspectExterior;
                ws.Cell("BY" + recNo).Value = row.AspectInterior;
                ws.Cell("BZ" + recNo).Value = row.AspectPrice;
                ws.Cell("CA" + recNo).Value = row.AspectAfterSales;
                ws.Cell("CB" + recNo).Value = row.AspectOutlet;
                ws.Cell("CC" + recNo).Value = row.AspectResalePrice;
                ws.Cell("CD" + recNo).Value = row.AspectOthers;
                ws.Cell("CE" + recNo).Value = row.AspectOthersDetail;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult QuestionnaireRekapProdSummary()
        {
            DateTime now = DateTime.Now;
            string fileName = "Qa_RekapSummary" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var TypeEvent = Request.Params["Event"] == "A" ? "Ertiga" : Request.Params["Event"] == "B" ? "WagonR" : "CBU";
            var statusKonsumen = Request.Params["StatusKonsumen"] == "" ? "%" : Request.Params["StatusKonsumen"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var areas = ctx.GroupAreas.Select(x => new { AreaDealer = x.AreaDealer, GroupNo = x.GroupNo }).OrderBy(x => x.GroupNo).ToList();

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Rekapitulasi Summary");

            int recNo = 1;

            ws.Cell("A" + recNo).Value = "REKAP KUESIONER ANGKET " + TypeEvent.ToUpper();
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            ws.Columns("1").Width = 50;
            ws.Columns("2", "69").Width = 30;
            ws.Columns("71", "81").Width = 25;

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

            recNo += 2;

            ws.Cell("A" + recNo).Value = "A. DATA KONSUMEN";
            ws.Cell("A" + recNo).Style.Font.FontSize = 11;

            int counter = 1;
            int index = 2;
            int firstRow = 0;
            int onceGrand = 0;
            int grandTotal = 71;
            foreach (var item in areas)
            {
                recNo = 8;

                var dealerList = from a in ctx.DealerGroupMapping
                                 where a.GroupNo == item.GroupNo
                                 && !(from b in ctx.Qa2MstCompanys select b.CompanyCode).Contains(a.DealerCode)
                                 orderby a.GroupNo
                                 select new
                                 {
                                     DealerCode = a.DealerCode
                                 };

                var areaCount = areas.Count();
                var dealerCount = dealerList.Count();
                string from = firstRow == 0 ? IndexToColumn(index) + recNo : IndexToColumn(index) + recNo;
                string to = firstRow == 0 ? IndexToColumn(dealerCount + 1) + recNo : IndexToColumn(dealerCount + index - 1) + recNo;

                var rngArea = ws.Range(from + ":" + to);

                rngArea.Merge();
                rngArea.Value = item.AreaDealer;
                rngArea.Style.Font.FontSize = 12;
                rngArea.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngArea.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                DataContext ctx2 = new DataContext();

                int lastcount = 0;
                foreach (var dealer in dealerList)
                {
                    recNo = 9;
                    var result = ctx2.Database.SqlQuery<Qa2SummaryModel>("exec uspfn_SpGnQa2RekapSummary @dealercode=@p0, @dateStart=@p1, @dateEnd=@p2, @statusKonsumen=@p3, @event=@p4", dealer.DealerCode, dateStart, dateEnd, statusKonsumen, TypeEvent).AsQueryable();
                    foreach (var row in result)
                    {
                        if (firstRow == 0)
                        {
                            string kolom = IndexToColumn(counter);
                            ws.Cell(kolom + recNo).Value = row.Question;
                            ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            kolom = IndexToColumn(counter + 1);
                            ws.Cell(kolom + recNo).Value = row.Value;
                            ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }
                        else
                        {
                            string kolom = IndexToColumn(counter);
                            ws.Cell(kolom + recNo).Value = row.Value;
                            ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        if (row.Style == "YL")
                        {
                            var rngColor = ws.Range("A" + recNo + ":CB" + recNo);
                            rngColor.Style.Fill.BackgroundColor = XLColor.Yellow;
                        }

                        recNo++;
                    }

                    if (firstRow == 0) counter++;

                    counter++;

                    if (lastcount == dealerCount - 1)//print total
                    {
                        recNo = 8;

                        string kolom = IndexToColumn(counter);
                        var rngTotal = ws.Range(kolom + recNo + ":" + kolom + (recNo + 1));

                        rngTotal.Merge();
                        rngTotal.Value = "Total";
                        rngTotal.Style.Font.FontSize = 12;
                        rngTotal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        rngTotal.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        rngTotal.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        var grandCounter = grandTotal + areaCount;
                        if (onceGrand == 0)
                        {
                            kolom = IndexToColumn(grandTotal);
                            var rngGrandTotal = ws.Range(kolom + (recNo - 1) + ":" + IndexToColumn(grandTotal - 1 + areaCount) + (recNo - 1));

                            rngGrandTotal.Merge();
                            rngGrandTotal.Value = "Total";
                            rngGrandTotal.Style.Font.FontSize = 12;
                            rngGrandTotal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            rngGrandTotal.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            var rngGrandTotal2 = ws.Range(IndexToColumn(grandCounter) + "7:" + IndexToColumn(grandCounter) + (recNo + 1));

                            rngGrandTotal2.Merge();
                            rngGrandTotal2.Value = "Grand Total";
                            rngGrandTotal2.Style.Font.FontSize = 12;
                            rngGrandTotal2.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            rngGrandTotal2.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                            rngGrandTotal2.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        }

                        recNo = 10;

                        int totalcount = 0;
                        foreach (var row in result)
                        {
                            if (totalcount == 0)
                            {

                                //print AreaName For Total most right
                                var rngRightArea = ws.Range(IndexToColumn(grandTotal) + "8:" + IndexToColumn(grandTotal) + "9");

                                rngRightArea.Merge();
                                rngRightArea.Value = item.AreaDealer;
                                rngRightArea.Style.Font.FontSize = 12;
                                rngRightArea.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                rngRightArea.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                rngRightArea.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                                totalcount++;
                                continue;
                            }

                            kolom = IndexToColumn(counter);
                            ws.Cell(kolom + recNo).Value = row.Total;
                            ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            kolom = IndexToColumn(counter);
                            ws.Cell(kolom + recNo).Value = row.Total;
                            ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            kolom = IndexToColumn(grandTotal);
                            ws.Cell(kolom + recNo).Value = row.Total;
                            ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                            if (onceGrand == 0)
                            {
                                kolom = IndexToColumn(grandCounter);

                                ws.Cell(kolom + recNo).Value = row.GrandTotal;
                                ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            }
                            totalcount++;
                            recNo++;
                        }
                        onceGrand = 1;
                        counter++;
                        grandTotal++;
                    }
                    lastcount++;
                    firstRow++;
                }

                index += dealerCount + 1;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult QuestionnaireRekapProdSummaryDealer()
        {
            DateTime now = DateTime.Now;
            string fileName = "Qa_RekapSummary" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var TypeEvent = Request.Params["Event"] == "A" ? "Ertiga" : Request.Params["Event"] == "B" ? "WagonR" : "CBU";
            var statusKonsumen = Request.Params["StatusKonsumen"] == "" ? "%" : Request.Params["StatusKonsumen"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var companyCode = CompanyCode;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Rekapitulasi Summary");

            int recNo = 1;

            ws.Cell("A" + recNo).Value = "REKAP KUESIONER ANGKET " + TypeEvent.ToUpper();
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            ws.Columns("1").Width = 50;
            ws.Columns("2", "69").Width = 30;
            ws.Columns("71", "81").Width = 25;

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

            recNo += 2;

            ws.Cell("A" + recNo).Value = "A. DATA KONSUMEN";
            ws.Cell("A" + recNo).Style.Font.FontSize = 11;

            int counter = 1;
            int index = 2;
            int firstRow = 0;

            recNo = 8;

            var outletlist = ctx.CoProfiles.Where(p => p.CompanyCode == companyCode).OrderBy(p => p.BranchCode).Select(p => p.BranchCode).ToList();

            var outletcount = outletlist.Count();

            string from = IndexToColumn(index) + recNo;
            string to = IndexToColumn(outletcount + 1) + recNo;

            var rngArea = ws.Range(from + ":" + to);

            rngArea.Merge();
            rngArea.Value = CompanyName;
            rngArea.Style.Font.FontSize = 12;
            rngArea.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            rngArea.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            DataContext ctx2 = new DataContext();

            int lastcount = 0;
            foreach (var outlet in outletlist)
            {
                recNo = 9;
                var result = ctx2.Database.SqlQuery<Qa2SummaryModel>("exec uspfn_SpGnQa2RekapSummaryDealer @dealercode=@p0, @dateStart=@p1, @dateEnd=@p2, @statusKonsumen=@p3, @event=@p4, @branchcode=@p5", CompanyCode, dateStart, dateEnd, statusKonsumen, TypeEvent, outlet).AsQueryable();
                foreach (var row in result)
                {
                    if (firstRow == 0)
                    {
                        string kolom = IndexToColumn(counter);
                        ws.Cell(kolom + recNo).Value = row.Question;
                        ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        kolom = IndexToColumn(counter + 1);
                        ws.Cell(kolom + recNo).Value = row.Value;
                        ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }
                    else
                    {
                        string kolom = IndexToColumn(counter);
                        ws.Cell(kolom + recNo).Value = row.Value;
                        ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }

                    if (row.Style == "YL")
                    {
                        var rngColor = ws.Range("A" + recNo + ":" + IndexToColumn(outletcount + 2) + recNo);
                        rngColor.Style.Fill.BackgroundColor = XLColor.Yellow;
                    }

                    recNo++;
                }

                if (firstRow == 0) counter++;

                counter++;

                if (lastcount == outletcount - 1)//print total
                {
                    recNo = 8;

                    string kolom = IndexToColumn(counter);
                    var rngTotal = ws.Range(kolom + recNo + ":" + kolom + (recNo + 1));

                    rngTotal.Merge();
                    rngTotal.Value = "Total";
                    rngTotal.Style.Font.FontSize = 12;
                    rngTotal.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTotal.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    rngTotal.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    recNo = 10;

                    int totalcount = 0;
                    foreach (var row in result)
                    {
                        if (totalcount == 0)
                        {
                            totalcount++;
                            continue;
                        }

                        kolom = IndexToColumn(counter);
                        ws.Cell(kolom + recNo).Value = row.Total;
                        ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        kolom = IndexToColumn(counter);
                        ws.Cell(kolom + recNo).Value = row.Total;
                        ws.Cell(kolom + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                        ws.Cell(kolom + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                        totalcount++;
                        recNo++;
                    }
                    counter++;
                }
                lastcount++;
                firstRow++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult generateMonitoringPerCust()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_MonitoringPerCust" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var TypeEvent = Request.Params["Event"] == "A" ? "Ertiga" : Request.Params["Event"] == "B" ? "WagonR" : "CBU";
            var Area = (Request.Params["Area"] ?? "") == "" ? "%" : Request.Params["Area"];
            var CompanyCode = (Request.Params["CompanyCode"] ?? "") == "" ? "%" : Request.Params["CompanyCode"];
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var CompanyText = Request.Params["CompanyText"] ?? " All Dealer ";

            //var qry = ctx.Database.SqlQuery<QaMonitoringCust>("exec uspfn_SpGetMonitoringPerCust2 @groupno=@p0, @companyCode=@p1, @dateStart=@p2, @dateEnd=@p3, @event=@p4", Area, CompanyCode, dateStart, dateEnd, TypeEvent).AsQueryable();
            var qry = ctx.Database.SqlQuery<QaMonitoringCust>("exec uspfn_SpGetMonitoringPerCust2_New @groupno=@p0, @Company=@p1, @dateStart=@p2, @dateEnd=@p3, @event=@p4", Area, CompanyCode, dateStart, dateEnd, TypeEvent).AsQueryable();

            var areas = ctx.DealerGroupMappingViews.Select(x => new { AreaDealer = x.Area, GroupNo = x.GroupNo, CompanyCode = x.CompanyCode, CompanyName = x.CompanyName }).OrderBy(x => x.GroupNo).ToList();

            int recNo = 1;

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Qa_Monitoring_Per_Customer");


            ws.Columns("1").Width = 40;
            ws.Columns("2").Width = 35;
            ws.Columns("3").Width = 15;
            ws.Columns("4").Width = 50;
            ws.Columns("5", "15").Width = 20;

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

            recNo++;

            ws.Cell("A" + recNo).Value = "Event : " + TypeEvent;
            ws.Cell("A" + recNo).Style.Font.FontSize = 12;
            ws.Cell("A" + recNo).Style.Font.Bold = true;

            recNo += 2;


            var rngTable = ws.Range("A" + recNo + ":O" + recNo);

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
            //ws.Cell("K" + recNo).Value = "Phone";
            ws.Cell("K" + recNo).Value = "Payment";
            ws.Cell("L" + recNo).Value = "Faktur Penjualan Date";
            ws.Cell("M" + recNo).Value = "Permohonan Faktur Polisi Date";
            ws.Cell("N" + recNo).Value = "QnR Input Date";
            ws.Cell("O" + recNo).Value = "Status Konsumen";

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
                //ws.Cell("K" + recNo).Value = row.ContactNo;
                ws.Cell("K" + recNo).Value = row.Payment;
                ws.Cell("L" + recNo).Value = !String.IsNullOrEmpty(row.FakturPajakDate) ? Convert.ToDateTime(row.FakturPajakDate).ToString("dd/MM/yyyy") : "";
                ws.Cell("M" + recNo).Value = !String.IsNullOrEmpty(row.FakturPolisiDate) ? Convert.ToDateTime(row.FakturPolisiDate).ToString("dd/MM/yyyy") : "";
                ws.Cell("N" + recNo).Value = !String.IsNullOrEmpty(row.TransactionDate) ? Convert.ToDateTime(row.TransactionDate).ToString("dd/MM/yyyy") : "";
                ws.Cell("O" + recNo).Value = row.StatusKonsumenDescI;


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
                //ws.Cell("P" + recNo).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

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
                //ws.Cell("P" + recNo).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

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
                //ws.Cell("P" + recNo).Style.Font.FontSize = 10;

                recNo++;
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
            return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
        }

        public ActionResult generateMonitoringPerOutlet()
        {
            DateTime now = DateTime.Now;
            string fileName = "Questionnaire_MonitoringPerOutlet" + now.Year + "-" + now.ToString("MM") + "-" + now.ToString("dd") + "_" + now.ToString("hh") + "-" + now.ToString("mm") + "-" + now.ToString("ss");

            var TypeEvent = Request.Params["Event"] == "A" ? "Ertiga" : Request.Params["Event"] == "B" ? "WagonR" : "CBU";
            var Area = (Request.Params["Area"] ?? "") == "" ? "%" : Request.Params["Area"];
            var CompanyCode = (Request.Params["CompanyCode"] ?? "") == "" ? "%" : Request.Params["CompanyCode"];
            var IncludeZero = (Request.Params["IncludeZero"] ?? "") == "on" ? true : false;
            var dateStart = DateTime.ParseExact(Request.Params["StartDate"] + " 00:00:00", "yyyy-MM-dd HH:mm:ss", null);
            var dateEnd = DateTime.ParseExact(Request.Params["EndDate"] + " 23:59:59", "yyyy-MM-dd HH:mm:ss", null);

            var CompanyText = Request.Params["CompanyText"] ?? "All Dealer ";

            //var qry = ctx.Database.SqlQuery<QaMonitoringOutlet>("exec uspfn_SpGetMonitoringPerOutlet2 @groupno=@p0, @companyCode=@p1, @dateStart=@p2, @dateEnd=@p3, @includeZero=@p4, @event=@p5", Area, CompanyCode, dateStart, dateEnd, IncludeZero, TypeEvent).AsQueryable();
            var qry = ctx.Database.SqlQuery<QaMonitoringOutlet>("exec uspfn_SpGetMonitoringPerOutlet2_New @groupno=@p0, @Company=@p1, @dateStart=@p2, @dateEnd=@p3, @includeZero=@p4, @event=@p5", Area, CompanyCode, dateStart, dateEnd, IncludeZero, TypeEvent).AsQueryable();

            var areas = ctx.DealerGroupMappingViews.Select(x => new { AreaDealer = x.Area, GroupNo = x.GroupNo, CompanyCode = x.CompanyCode, CompanyName = x.CompanyName }).OrderBy(x => x.GroupNo).ToList();

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

            recNo++;

            ws.Cell("A" + recNo).Value = "Event : " + TypeEvent;
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
    }
}
