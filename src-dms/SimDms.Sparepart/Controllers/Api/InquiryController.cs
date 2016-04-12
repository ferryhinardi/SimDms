using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using SimDms.Common;
using TracerX;
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;
using System.Text;

namespace SimDms.Sparepart.Controllers.Api
{
    public class InquiryController : BaseController
    {

        public JsonResult Customers(bool AllowPeriod, DateTime StartDate, DateTime EndDate, string Branch)
        {

            try
            {

                string dtFirstDate, dtLastDate;
                string flag1 = AllowPeriod ? "1" : "0";

                dtFirstDate = StartDate.ToString("yyyy-MM-dd");
                dtLastDate = EndDate.ToString("yyyy-MM-dd");


                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "usprpt_QueryCustomerDealer2 '" + flag1 + "','" + dtFirstDate + "','" + dtLastDate + "','','','" + Branch + "'";
                cmd.CommandTimeout = 3600;

                MyLogger.Log.Info("Inquiry Customers: EXEC " + cmd.CommandText);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);
                var records = dt.Tables[0];
                var rows = records.Rows.Count;

                return Json(new { success = true, data = records, total = rows }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        private IList<MasterModelBrowse> ListModel(string PartNo)
        {
            return ctx.Database.SqlQuery<MasterModelBrowse>("uspfn_spModelGridLookup '" + CompanyCode + "','" + GnMstLookUpHdr.ModelVehicle + "','" + PartNo + "'").ToList();
        }

        private spMstItemPrice _SplGetPrice(string PartNo)
        {
            return ctx.spMstItemPrices.Find(CompanyCode, BranchCode, PartNo);
        }

        public JsonResult DetailOfPartInquiry(string PartNo, int HQ = 0)
        {
            try
            {
                string msg = "Item not found";
                var record = ctx.spMstItems.Find(CompanyCode, BranchCode, PartNo);
                var info = ctx.MasterItemInfos.Find(CompanyCode, PartNo);

                if (record != null && info != null && record.ProductType == ProductType && record.TypeOfGoods == TypeOfGoods)
                {
                    if (HQ == 1)
                    {
                        string HO = ctx.Database.SqlQuery<String>(string.Format("select branchcode from gnMstOrganizationDtl where companycode='{0}' and isbranch=0", CompanyCode)).FirstOrDefault();

                        var lokasi = ctx.Database.SqlQuery<PartInquiry_Location>("uspfn_sp_partinquiry_locationhq '" + CompanyCode + "','" + PartNo + "'").ToList();
                        var subsitusi = ctx.Database.SqlQuery<PartInquiry_Subsitusi>("uspfn_sp_partinquiry_subsitusi '" + CompanyCode + "','" + BranchCode + "','" + PartNo + "','" + TypeOfGoods + "'").ToList();
                        var demandandsales = ctx.Database.SqlQuery<PartInquiry_DemandAndSales>("uspfn_sp_partinquiry_demandandsaleshq '" + CompanyCode + "','" + PartNo + "'").ToList();
                        var onorder = ctx.Database.SqlQuery<PartInquiry_OnOrder>("uspfn_sp_partinquiry_onorder '" + CompanyCode + "','" + HO + "','" + PartNo + "'").ToList();

                        return Json(new
                        {
                            success = true,
                            data = record,
                            info = info,
                            lokasi = lokasi,
                            model = ListModel(PartNo),
                            subsitusi = subsitusi,
                            demandandsales = demandandsales,
                            price = _SplGetPrice(PartNo),
                            SupplierName = ctx.Database.SqlQuery<String>(string.Format("select SupplierName from GnMstSupplier where suppliercode='{0}' and companycode='{1}'", info.SupplierCode, CompanyCode)).FirstOrDefault(),
                            onorder = onorder
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (IsMD == false)
                        {
                            var lokasi = ctxMD.Database.SqlQuery<PartInquiry_Location>("uspfn_sp_partinquiry_location '" + CompanyMD + "','" + BranchMD + "','" + PartNo + "'").ToList();
                            var subsitusi = ctxMD.Database.SqlQuery<PartInquiry_Subsitusi>("uspfn_sp_partinquiry_subsitusi '" + CompanyMD + "','" + BranchMD + "','" + PartNo + "','" + TypeOfGoods + "'").ToList();
                            var demandandsales = ctxMD.Database.SqlQuery<PartInquiry_DemandAndSales>("uspfn_sp_partinquiry_demandandsales '" + CompanyMD + "','" + BranchMD + "','" + PartNo + "'").ToList();
                            var onorder = ctxMD.Database.SqlQuery<PartInquiry_OnOrder>("uspfn_sp_partinquiry_onorder '" + CompanyMD + "','" + BranchMD + "','" + PartNo + "'").ToList();

                            return Json(new
                            {
                                success = true,
                                data = record,
                                info = info,
                                lokasi = lokasi,
                                model = ListModel(PartNo),
                                subsitusi = subsitusi,
                                demandandsales = demandandsales,
                                price = _SplGetPrice(PartNo),
                                SupplierName = ctx.Database.SqlQuery<String>(string.Format("select SupplierName from GnMstSupplier where suppliercode='{0}' and companycode='{1}'", info.SupplierCode, CompanyCode)).FirstOrDefault(),
                                onorder = onorder
                            }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var lokasi = ctx.Database.SqlQuery<PartInquiry_Location>("uspfn_sp_partinquiry_location '" + CompanyCode + "','" + BranchCode + "','" + PartNo + "'").ToList();
                            var subsitusi = ctx.Database.SqlQuery<PartInquiry_Subsitusi>("uspfn_sp_partinquiry_subsitusi '" + CompanyCode + "','" + BranchCode + "','" + PartNo + "','" + TypeOfGoods + "'").ToList();
                            var demandandsales = ctx.Database.SqlQuery<PartInquiry_DemandAndSales>("uspfn_sp_partinquiry_demandandsales '" + CompanyCode + "','" + BranchCode + "','" + PartNo + "'").ToList();
                            var onorder = ctx.Database.SqlQuery<PartInquiry_OnOrder>("uspfn_sp_partinquiry_onorder '" + CompanyCode + "','" + BranchCode + "','" + PartNo + "'").ToList();


                            return Json(new
                            {
                                success = true,
                                data = record,
                                info = info,
                                lokasi = lokasi,
                                model = ListModel(PartNo),
                                subsitusi = subsitusi,
                                demandandsales = demandandsales,
                                price = _SplGetPrice(PartNo),
                                SupplierName = ctx.Database.SqlQuery<String>(string.Format("select SupplierName from GnMstSupplier where suppliercode='{0}' and companycode='{1}'", info.SupplierCode, CompanyCode)).FirstOrDefault(),
                                onorder = onorder
                            }, JsonRequestBehavior.AllowGet);
                        }
                        
                    }
                }

                return Json(new { success = false, message = msg, mode = 0 }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        public JsonResult InquiryPenerimaanBarang(string Flag, DateTime StartDate, DateTime EndDate, string parameter)
        {

            try
            {
                string orderBy = string.Empty;
                string param = string.Empty;
                string docDate = string.Empty;
                string dtFirstDate, dtLastDate;
                string flag1 = "";

                dtFirstDate = StartDate.ToString("yyyyMMdd");
                dtLastDate = EndDate.ToString("yyyyMMdd");

                switch (Flag)
                {
                    case "opsi1":
                        orderBy = "SupplierName";
                        param = string.IsNullOrEmpty(parameter) ? string.Empty : string.Format("AND {0} LIKE '{1}'",
                            orderBy, string.Format("%{0}%", parameter));
                        docDate = string.Format("AND CONVERT(VARCHAR, a.BinningDate, 112) BETWEEN '{0}' AND '{1}'",
                            dtFirstDate, dtLastDate);
                        flag1 = "0";
                        break;
                    case "opsi2":
                        orderBy = "a.BinningDate";
                        param = string.IsNullOrEmpty(parameter) ? string.Empty : string.Format("AND a.BinningNo LIKE '{0}'", parameter);
                        docDate = string.Format("AND CONVERT(VARCHAR, {0}, 112) BETWEEN '{1}' AND '{2}'",
                            orderBy, dtFirstDate, dtLastDate);
                        flag1 = "1";
                        break;
                    case "opsi3":
                        orderBy = "b.WRSDate";
                        param = string.IsNullOrEmpty(parameter) ? string.Empty : string.Format("AND b.WRSNo LIKE '{0}'", parameter);
                        docDate = string.Format("AND CONVERT(VARCHAR, {0}, 112) BETWEEN '{1}' AND '{2}'",
                            orderBy, dtFirstDate, dtLastDate);
                        flag1 = "2";
                        break;
                    case "opsi4":
                        orderBy = "c.HPPDate";
                        param = string.IsNullOrEmpty(parameter) ? string.Empty : string.Format("AND c.HPPNo LIKE '{0}'", parameter);
                        docDate = string.Format("AND CONVERT(VARCHAR, {0}, 112) BETWEEN '{1}' AND '{2}'",
                            orderBy, dtFirstDate, dtLastDate);
                        flag1 = "3";
                        break;
                    default:
                        break;
                }

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "uspfn_GetInqReceivingHdr '" + CompanyCode + "','" + BranchCode + "','" + TypeOfGoods + "','" + flag1 + "','" + dtFirstDate + "','" + dtLastDate + "','" + param + "'";

                MyLogger.Log.Info("Inquiry Penerimaan Barang: EXEC " + cmd.CommandText);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                return Json(new { success = true, data = dt.Tables[0], total = dt.Tables[1], sql = cmd.CommandText }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }


        public JsonResult InquiryPenerimaanBarangDetail(string BinningNo)
        {

            try
            {

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "uspfn_sp_inquiry_penerimaan_barang_detail '" + CompanyCode + "','" + BranchCode + "','" + BinningNo + "'";

                MyLogger.Log.Info("Inquiry Penerimaan Barang Detail: EXEC " + cmd.CommandText);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                return Json(new { success = true, data = dt.Tables[0], sql = cmd.CommandText }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult InquirySalesOrder(string Flag, DateTime StartDate, DateTime EndDate, string parameter)
        {

            try
            {
                string orderBy = string.Empty;
                string param = string.Empty;
                string docDate = string.Empty;
                string dtFirstDate, dtLastDate;
                string flag1 = "";

                dtFirstDate = StartDate.ToString("yyyyMMdd");
                dtLastDate = EndDate.ToString("yyyyMMdd");

                switch (Flag)
                {
                    case "PELANGGAN":
                        orderBy = "Cust.CustomerName";
                        param = string.IsNullOrEmpty(parameter) ? string.Empty : string.Format("AND {0} LIKE '{1}'",
                            orderBy, string.Format("%{0}%", parameter));
                        docDate = string.Format("AND CONVERT(VARCHAR, a.PickingSlipDate, 112) BETWEEN '{0}' AND '{1}'",
                            dtFirstDate, dtLastDate);
                        break;
                    case "PICKINGLIST":
                        orderBy = "a.PickingSlipDate";
                        param = string.IsNullOrEmpty(parameter) ? string.Empty : string.Format("AND a.PickingSlipNo LIKE '{0}'", parameter);
                        docDate = string.Format("AND CONVERT(VARCHAR, {0}, 112) BETWEEN '{1}' AND '{2}'",
                            orderBy, dtFirstDate, dtLastDate);
                        break;
                    case "FAKTURPENJUALAN":
                        orderBy = "c.FPJDate";
                        param = string.IsNullOrEmpty(parameter) ? string.Empty : string.Format("AND c.FPJNo LIKE '{0}'", parameter);
                        docDate = string.Format("AND CONVERT(VARCHAR, {0}, 112) BETWEEN '{1}' AND '{2}'",
                            orderBy, dtFirstDate, dtLastDate);
                        break;
                    case "FAKTURPAJAK":
                        orderBy = "c.FPJGovNo";
                        param = string.IsNullOrEmpty(parameter) ? string.Empty : string.Format("AND c.FPJGovNo LIKE '{0}'", parameter);
                        docDate = string.Format("AND CONVERT(VARCHAR, c.FPJSignature, 112) BETWEEN '{1}' AND '{2}'",
                            orderBy, dtFirstDate, dtLastDate);
                        break;
                    default:
                        break;
                }

                string query = string.Format("EXEC uspfn_sp_inquiry_sales_order '{0}', '{1}', '{2}', '{3}', '{4}'", CompanyCode, BranchCode, orderBy, docDate.Replace("'", "''"), param.Replace("'", "''"));

                MyLogger.Log.Info("Inquiry sales order:  " + query);

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = query;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                return Json(new { success = true, data = dt.Tables[0], total = dt.Tables[1], sql = cmd.CommandText }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult InquirySalesOrderDetail(string pickingSlipNo)
        {

            try
            {

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = string.Format("EXEC [uspfn_sp_inquiry_sales_order_detail] '{0}', '{1}', '{2}'", CompanyCode, BranchCode, pickingSlipNo);

                MyLogger.Log.Info("Inquiry sales order detail:  " + cmd.CommandText);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                return Json(new { success = true, data = dt.Tables[0], sql = cmd.CommandText }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult InquirySupplySlip(string Flag, DateTime StartDate, DateTime EndDate, string parameter)
        {

            try
            {
                string orderBy = string.Empty;
                string param = string.Empty;
                string param1 = string.Empty;
                string docDate = string.Empty;
                string dtFirstDate, dtLastDate;
                string flag1 = "";

                dtFirstDate = StartDate.ToString("yyyyMMdd");
                dtLastDate = EndDate.ToString("yyyyMMdd");

                string paramText = "";
                string paramText1 = "";
                string filter = "";

                switch (Flag)
                {
                    case "PELANGGAN":
                        param = "";
                        paramText = string.Format(")v WHERE v.CustomerName LIKE '%{0}%' AND CONVERT(VARCHAR, v.SSDate, 112) BETWEEN '{1}' AND '{2}' ORDER BY Nomor", parameter, dtFirstDate, dtLastDate);
                        filter = "v.CustomerName";
                        paramText1 = string.Format(")v WHERE v.CustomerName LIKE '%{0}%' AND CONVERT(VARCHAR, v.SSDate, 112) BETWEEN '{1}'AND '{2}' ", parameter, dtFirstDate, dtLastDate);
                        break;
                    case "SUPPLYSLIP":
                        param = string.Format(" AND a.DocNo LIKE '%{0}%' AND CONVERT(VARCHAR, a.DocDate, 112) BETWEEN '{1}' AND '{2}') v ORDER BY Nomor", parameter, dtFirstDate, dtLastDate);
                        filter = "v.SSNo";
                        param1 = string.Format(" AND a.DocNo LIKE '%{0}%' AND CONVERT(VARCHAR, a.DocDate, 112) BETWEEN '{1}'AND '{2}')v ", parameter, dtFirstDate, dtLastDate);
                        break;
                    case "SPK":
                        param = string.Format(" AND a.UsageDocNo LIKE '%{0}%' AND CONVERT(VARCHAR, a.UsageDocDate, 112) BETWEEN '{1}' AND '{2}') v ORDER BY Nomor", parameter, dtFirstDate, dtLastDate);
                        filter = "v.SPKNo";
                        param1 = string.Format(" AND a.UsageDocNo LIKE '%{0}%' AND CONVERT(VARCHAR, a.UsageDocDate, 112) BETWEEN '{1}' AND '{2}')v ", parameter, dtFirstDate, dtLastDate);
                        break;
                    case "POLICEREGNO":
                        param = string.Format(" AND c.PoliceRegNo LIKE '%{0}%' AND CONVERT(VARCHAR, c.JobOrderDate, 112) BETWEEN '{1}' AND '{2}') v ORDER BY Nomor", parameter, dtFirstDate, dtLastDate);
                        filter = "v.PoliceNo";
                        param1 = string.Format(" AND c.PoliceRegNo LIKE '%{0}%' AND CONVERT(VARCHAR, c.JobOrderDate, 112) BETWEEN '{1}' AND '{2}')v ", parameter, dtFirstDate, dtLastDate);
                        break;
                    case "LAMPIRAN":
                        param = string.Format(" AND b.LmpNo LIKE '%{0}%' AND CONVERT(VARCHAR, b.LmpDate, 112) BETWEEN '{1}' AND '{2}') v ORDER BY Nomor", parameter, dtFirstDate, dtLastDate);
                        filter = "v.LmpNo";
                        param1 = string.Format(" AND b.LmpNo LIKE '%{0}%' AND CONVERT(VARCHAR, b.LmpDate, 112) BETWEEN '{1}'AND '{2}')v ", parameter, dtFirstDate, dtLastDate);
                        break;
                    default:
                        break;
                }

                string query = string.Format("EXEC [uspfn_sp_inquiry_supply_slip] '{0}', '{1}', '{2}', '{3}', '{4}'", CompanyCode, BranchCode, filter, param.Replace("'", "''"), paramText.Replace("'", "''"));
                string query2 = query + string.Format("  EXEC [uspfn_sp_inquiry_supply_slip_sum] '{0}', '{1}', '{2}', '{3}'", CompanyCode, BranchCode, param1.Replace("'", "''"), paramText1.Replace("'", "''"));

                MyLogger.Log.Info("Inquiry supply slip:  " + query2);

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = query2;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                return Json(new { success = true, data = dt.Tables[0], total = dt.Tables[1], sql = cmd.CommandText }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult InquirySupplySlipDetail(string JO, string SS)
        {

            try
            {

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = string.Format("EXEC [uspfn_sp_inquiry_supply_slip_detail] '{0}', '{1}', '{2}', '{3}' exec [dbo].[uspfn_sp_inquiry_supply_slip_detail_sum]   '{0}', '{1}', '{3}' ", CompanyCode, BranchCode, JO, SS);

                MyLogger.Log.Info("Inquiry Supply Slip detail:  " + cmd.CommandText);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                return Json(new { success = true, data = dt.Tables[0], total = dt.Tables[1], sql = cmd.CommandText }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

        }

        public JsonResult InquirySparepartAnalisys(InqSAparam param)
        {

            try
            {

                MyLogger.Log.Info("Param Item:  " + param.ItemTypeS);

                var a = param.ItemTypeS.Replace("[", "").Replace("]", "");

                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = string.Format("EXEC [usprpt_SpRpSum024_rev01] '{0}', '{1}', '{2}', '{3}', '{4}' ", param.Area, param.Dealer, param.Outlet, param.Periode, a.Replace(@"""", "''"));

                MyLogger.Log.Info("InquirySparepartAnalisys:  " + cmd.CommandText);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);

                return Json(new { success = true, data = dt.Tables[0], metadata = dt.Tables[1], sql = cmd.CommandText }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult InquiryPartSales(string CompanyCode, string BranchCode, string InvoiceDateFrom, string InvoiceDateTo, string CustomerCode, string PartSales, string SalesType)
        {
            if (BranchCode == "")
            {
                BranchCode = "%";
            }
            if (CustomerCode == "")
            {
                CustomerCode = "%";
            }
            if (PartSales == "")
            {
                PartSales = "%";
            }
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "EXEC usprpt_SpRpSum025 '" + CompanyCode + "','" + BranchCode + "','" + InvoiceDateFrom + "','" + InvoiceDateTo + "','" + CustomerCode + "','" + PartSales + "','" + SalesType + "'";
                MyLogger.Log.Info("InquiryPartSales : " + cmd.CommandText);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);


                return Json(new { success = true, data = dt.Tables[0], grid = dt.Tables[1] }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult inquirypartsalesgenexcell(string CompanyCode, string BranchCode, string InvoiceDateFrom, string InvoiceDateTo, string CustomerCode, string PartSales, string SalesType, string SpID, string From, string To)
        {
            string fileName = "";
            fileName = SpID + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");
            var customerCd = "";
            var partSales = "";
            var typeP = "";

            if (CustomerCode == "")
            {
                customerCd = "All";
            }
            else
            {
                customerCd = CustomerCode;
            }
            if (PartSales == "")
            {
                partSales = "All";
            }
            else
            {
                partSales = PartSales;
            }
            if (SalesType == "'0'")
            {
                typeP = "SP Penjualan";
            }

            if (BranchCode == "")
            {
                BranchCode = "%";
            }
            if (CustomerCode == "")
            {
                CustomerCode = "%";
            }
            if (PartSales == "")
            {
                PartSales = "%";
            }


            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandTimeout = 1800;
            cmd1.CommandText = "uspfn_GetAreaDealerOutlet";

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.Clear();
            cmd1.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd1.Parameters.AddWithValue("@BranchCode", BranchCode);
            SqlDataAdapter ga = new SqlDataAdapter(cmd1);
            DataSet gt = new DataSet();
            ga.Fill(gt);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = SpID;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@InvoiceDateFrom", InvoiceDateFrom);
            cmd.Parameters.AddWithValue("@InvoiceDateTo", InvoiceDateTo);
            cmd.Parameters.AddWithValue("@CustomerCode", CustomerCode);
            cmd.Parameters.AddWithValue("@PartSales", PartSales);
            cmd.Parameters.AddWithValue("@SalesType", SalesType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            else
            {
                int lastRow = 9;
                int no = 0;

                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Inquiry Sparepart");
                var hdrTable = ws.Range("A1:X9");
                hdrTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                var rngTable = ws.Range("A9:X9");
                ws.Range("A9", "X9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A9", "X9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A9", "X9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A9", "X9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                rngTable.Style
                    .Font.SetBold()
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                    .Alignment.SetWrapText();

                rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                rngTable.Style.Font.Bold = true;

                ws.Columns("1").Width = 10;
                ws.Columns("2").Width = 30;
                ws.Columns("3").Width = 15;
                ws.Columns("4").Width = 30;
                ws.Columns("5").Width = 15;
                ws.Columns("6").Width = 20;
                ws.Columns("7").Width = 15;
                ws.Columns("8").Width = 20;
                ws.Columns("9").Width = 15;
                ws.Columns("10").Width = 20;
                ws.Columns("11").Width = 15;
                ws.Columns("12").Width = 20;
                ws.Columns("13").Width = 15;
                ws.Columns("14").Width = 50;
                ws.Columns("15").Width = 15;
                ws.Columns("16").Width = 15;
                ws.Columns("17").Width = 15;
                ws.Columns("18").Width = 15;
                ws.Columns("19").Width = 15;
                ws.Columns("20").Width = 15;
                ws.Columns("21").Width = 15;
                ws.Columns("22").Width = 15;
                ws.Columns("23").Width = 20;
                ws.Columns("24").Width = 20;
                ws.Columns("25").Width = 20;


                //First Names   
                ws.Cell("A1").Value = "Inquiry Part Sales";
                ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                ws.Cell("A2").Value = "Dealer";
                ws.Cell("A3").Value = "Cabang";
                ws.Cell("A4").Value = "Periode";
                ws.Cell("A5").Value = "Customer";
                ws.Cell("A6").Value = "Part Sales";
                ws.Cell("A7").Value = "Tipe Penjualan";

                foreach (var row1 in gt.Tables[0].Rows)
                {
                    ws.Cell("B2").Value = " : " + CompanyCode + " - " + ((System.Data.DataRow)(row1)).ItemArray[0];
                    ws.Cell("B3").Value = " : " + BranchCode + " - " + ((System.Data.DataRow)(row1)).ItemArray[1];
                }
                ws.Cell("B4").Value = " : " + From + " s/d " + To;
                ws.Cell("B5").Value = " : " + customerCd;
                ws.Cell("B6").Value = " : " + partSales;
                ws.Cell("B7").Value = " : " + typeP;

                ws.Range("A9:B9").Merge();
                ws.Range("A9:B9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("A9:B9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("A9:B9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("A9:B9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("C9:D9").Merge();
                ws.Range("C9:D9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("C9:D9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("C9:D9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("C9:D9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("E9:E9").Merge();
                ws.Range("E9:E9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("E9:E9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("E9:E9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("E9:E9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("G9:H9").Merge();
                ws.Range("G9:H9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("G9:H9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("G9:H9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("G9:H9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("I9:J9").Merge();
                ws.Range("I9:J9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("I9:J9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("I9:J9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("I9:J9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("K9:L9").Merge();
                ws.Range("K9:L9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("K9:L9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("K9:L9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("K9:L9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("M9:N9").Merge();
                ws.Range("M9:N9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("M9:N9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("M9:N9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("M9:N9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("O9:O10").Merge();
                ws.Range("O9:O10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("O9:O10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("O9:O10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("O9:O10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("P9:P10").Merge();
                ws.Range("P9:P10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("P9:P10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("P9:P10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("P9:P10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("Q9:Q10").Merge();
                ws.Range("Q9:Q10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("Q9:Q10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("Q9:Q10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("Q9:Q10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("R9:R10").Merge();
                ws.Range("R9:R10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("R9:R10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("R9:R10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("R9:R10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("S9:S10").Merge();
                ws.Range("S9:S10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("S9:S10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("S9:S10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("S9:S10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("T9:T10").Merge();
                ws.Range("T9:T10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("T9:T10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("T9:T10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("T9:T10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("U9:U10").Merge();
                ws.Range("U9:U10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("U9:U10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("U9:U10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("U9:U10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("V9:V10").Merge();
                ws.Range("V9:V10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("V9:V10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("V9:V10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("V9:V10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("W9:W10").Merge();
                ws.Range("W9:W10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("W9:W10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("W9:W10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("W9:W10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Range("X9:X10").Merge();
                ws.Range("X9:X10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Range("X9:X10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Range("X9:X10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Range("X9:X10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("A9").Value = "Part Sales ";
                ws.Cell("C9").Value = "Pelanggan ";
                ws.Cell("E9").Value = "Sales Order ";
                ws.Cell("G9").Value = "Picking List ";
                ws.Cell("I9").Value = "Invoice ";
                ws.Cell("K9").Value = "Faktur Pajak ";
                ws.Cell("M9").Value = "Part ";

                ws.Cell("A10").Value = "Kode ";
                ws.Cell("A10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("A10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("A10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("A10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("B10").Value = "Nama ";
                ws.Cell("B10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("B10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("B10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("B10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("C10").Value = "Kode ";
                ws.Cell("C10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("C10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("C10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("C10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("D10").Value = "Nama ";
                ws.Cell("D10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("D10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("D10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("D10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("E10").Value = "No.  ";
                ws.Cell("E10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("E10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("E10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("E10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("F10").Value = "Tanggal ";
                ws.Cell("F10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("F10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("F10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("F10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("F10").Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell("G10").Value = "No. ";
                ws.Cell("G10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("G10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("G10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("G10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("H10").Value = "Tanggal ";
                ws.Cell("H10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("H10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("H10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("H10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("H10").Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell("I10").Value = "No. ";
                ws.Cell("I10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("I10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("I10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("I10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("J10").Value = "Tanggal ";
                ws.Cell("J10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("J10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("J10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("J10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("J10").Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell("K10").Value = "No. ";
                ws.Cell("K10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("K10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("K10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("K10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("L10").Value = "Tanggal ";
                ws.Cell("L10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("L10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("L10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("L10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;
                ws.Cell("L10").Style.DateFormat.Format = "dd-MMM-yyyy";

                ws.Cell("M10").Value = "No ";
                ws.Cell("M10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("M10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("M10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("M10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("N10").Value = "Nama ";
                ws.Cell("N10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                ws.Cell("N10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                ws.Cell("N10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                ws.Cell("N10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                ws.Cell("O9").Value = "Qty Order ";
                ws.Cell("P9").Value = "Qty Bill ";
                ws.Cell("Q9").Value = "@Price Unit ";
                ws.Cell("R9").Value = "Total Price ";
                ws.Cell("S9").Value = "Disc% ";
                ws.Cell("T9").Value = "Disc Amount ";
                ws.Cell("U9").Value = "DPP ";
                ws.Cell("V9").Value = "PPN ";
                ws.Cell("W9").Value = "Total Sales ";
                ws.Cell("X9").Value = "Harga Pokok ";

                lastRow = lastRow + 1;
                lastRow++;

                foreach (var row in ds.Tables[1].Rows)
                {
                    ws.Cell("A" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("A" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[1];
                    ws.Cell("A" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("B" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("B" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[2];
                    ws.Cell("B" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("C" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("C" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[3];
                    ws.Cell("C" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("D" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("D" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[4];
                    ws.Cell("D" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("E" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("E" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[5];
                    ws.Cell("E" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("F" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("F" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[6];
                    ws.Cell("F" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("G" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("G" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[7];
                    ws.Cell("G" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("H" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("H" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[8];
                    ws.Cell("H" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("I" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("I" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[9];
                    ws.Cell("I" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("J" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("J" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[10];
                    ws.Cell("J" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("K" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("K" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[11];
                    ws.Cell("K" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("L" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("L" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[12];
                    ws.Cell("L" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("M" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("M" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[13];
                    ws.Cell("M" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("N" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("N" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[14];
                    ws.Cell("N" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("O" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("O" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[15];
                    ws.Cell("O" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("P" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("P" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[16];
                    ws.Cell("P" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("Q" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("Q" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[17];
                    ws.Cell("Q" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("R" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("R" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[18];
                    ws.Cell("R" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("S" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("S" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[19];
                    ws.Cell("S" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("T" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("T" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[20];
                    ws.Cell("T" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("U" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("U" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[21];
                    ws.Cell("U" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("V" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("V" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[22];
                    ws.Cell("V" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("W" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("W" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("W" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("W" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("W" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[23];
                    ws.Cell("W" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    ws.Cell("X" + lastRow).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    ws.Cell("X" + lastRow).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    ws.Cell("X" + lastRow).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    ws.Cell("X" + lastRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    ws.Cell("X" + lastRow).Value = ((System.Data.DataRow)(row)).ItemArray[24];
                    ws.Cell("X" + lastRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                    lastRow++;
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
        }

        public JsonResult GetTypePart()
        {
            try
            {


                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = string.Format("SELECT  1 IsSelected, LookUpValue, LookUpValueName FROM GnMstLookUpDtl WHERE CompanyCode='" + CompanyCode + "' AND CodeID = 'TPGO'");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return Json(new { success = true, data = dt });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetTypePartPenjualan()
        {
            try
            {


                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = string.Format("SELECT  1 IsSelected, gnMstLookupDtl.LookUpValue, gnMstLookupDtl.LookUpValueName "+
                                                " FROM gnMstLookupDtl "+
                                                " INNER JOIN gnMstLookupDtl istf "+
                                                " ON istf.CompanyCode = gnmstlookupdtl.CompanyCode  AND istf.CodeID = 'ISTF'"+
	                                            " AND istf.ParaValue = '1' AND istf.SeqNo = gnMstLookupDtl.SeqNo "+
                                                " WHERE gnMstLookupDtl.codeid = 'SLTP'"+
                                                " AND gnMstLookupDtl.ParaValue != '1'");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return Json(new { success = true, data = dt });

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult InquerySparepart(string CompanyCode, string BranchCode, string Area, string Year, string S1)
        {
            if (CompanyCode == "")
            {
                CompanyCode = "";
            }
            if (BranchCode == "")
            {
                BranchCode = "";
            }
            if (Area == "")
            {
                Area = "";
            }

            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "EXEC usprpt_SpRpSum024 '" + Area + "','" + CompanyCode + "','" + BranchCode + "','" + Year + "','" + S1 + "'";
                MyLogger.Log.Info("InquirySparepart : " + cmd.CommandText);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);


                return Json(new { success = true, data = dt.Tables[0], grid = dt.Tables[1] }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public ActionResult inquirysparepartgenexcell(string Area, string CompanyCode, string BranchCode, string Year, string S1, string SpID)
        {
            string fileName = "";
            fileName = SpID + "_";
            fileName = fileName + DateTime.Now.ToString("yyyyMMdd-hhmmss");

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "EXEC usprpt_SpRpSum024 '" + Area + "','" + CompanyCode + "','" + BranchCode + "','" + Year + "','" + S1 + "'";
            MyLogger.Log.Info("InquirySparepart : " + cmd.CommandText);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            SqlCommand cmd1 = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd1.CommandText = "exec usprpt_SpCheckType '" + S1 + "'";
            MyLogger.Log.Info("InquirySparepart : " + cmd1.CommandText);

            SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
            DataSet ds1 = new DataSet();
            da1.Fill(ds1);

            StringBuilder sb = new StringBuilder();
            string Tipe = "";

            foreach (DataRow rw in ds1.Tables[0].Rows)
            {
                sb.Append(rw["LookUpValueName"].ToString() + " + ");
            }
            if (ds1.Tables[0].Rows.Count == 0)
            {
                Tipe = "";
            }
            else
            {
                Tipe = sb.ToString().Remove(sb.Length - 2, 2);
            }

            DataTable header = ds.Tables[0];
            DataTable data = ds.Tables[1];

            int cnt = header.Rows.Count;

            if (header.Rows.Count == 0)
            {
                return Json("Tidak ada data yang ditampilkan", "text/html", JsonRequestBehavior.AllowGet);
            }
            #region --- format excel ---
            else
            {
                var wb = new XLWorkbook();
                for (int i = 0; i < header.Rows.Count; i++)
                {
                    var sheetName = header.Rows[i][2].ToString();
                    var ws = wb.Worksheets.Add(sheetName);

                    var hdrTable = ws.Range("A1:X9");
                    hdrTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                    var rngTable = ws.Range("A9:Z9");
                    ws.Range("A9", "Z9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("A9", "Z9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("A9", "Z9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("A9", "Z9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    rngTable.Style
                        .Font.SetBold()
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                        .Alignment.SetVertical(XLAlignmentVerticalValues.Center)
                        .Alignment.SetWrapText();

                    rngTable.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    rngTable.Style.Font.Bold = true;

                    ws.Columns("1").Width = 30;
                    ws.Columns("2").Width = 30;
                    ws.Columns("3").Width = 15;
                    ws.Columns("4").Width = 30;
                    ws.Columns("5").Width = 15;
                    ws.Columns("6").Width = 20;
                    ws.Columns("7").Width = 15;
                    ws.Columns("8").Width = 20;
                    ws.Columns("9").Width = 15;
                    ws.Columns("10").Width = 20;
                    ws.Columns("11").Width = 15;
                    ws.Columns("12").Width = 20;
                    ws.Columns("13").Width = 15;
                    ws.Columns("14").Width = 50;
                    ws.Columns("15").Width = 15;
                    ws.Columns("16").Width = 15;
                    ws.Columns("17").Width = 15;
                    ws.Columns("18").Width = 15;
                    ws.Columns("19").Width = 15;
                    ws.Columns("20").Width = 15;
                    ws.Columns("21").Width = 15;
                    ws.Columns("22").Width = 15;
                    ws.Columns("23").Width = 20;
                    ws.Columns("24").Width = 20;
                    ws.Columns("25").Width = 20;
                    ws.Columns("26").Width = 20;

                    //First Names   
                    ws.Cell("A1").Value = "Sparepart Analysis Report";
                    ws.Cell("A1").Style.Font.SetBold().Font.SetFontSize(14);
                    ws.Cell("A2").Value = "Kode Dealer";
                    ws.Cell("A3").Value = "Nama Dealer";
                    ws.Cell("A4").Value = "Kode Outlet";
                    ws.Cell("A5").Value = "Nama Outlet";
                    ws.Cell("A6").Value = "Tahun";
                    ws.Cell("A7").Value = "Tipe Part";

                    ws.Cell("B2").Value = " : " + header.Rows[i][0].ToString();
                    ws.Cell("B3").Value = " : " + header.Rows[i][1].ToString();
                    ws.Cell("B4").Value = " : " + header.Rows[i][2].ToString();
                    ws.Cell("B5").Value = " : " + header.Rows[i][3].ToString();

                    ws.Cell("B6").Value = " : " + Year;
                    ws.Cell("B7").Value = " : " + Tipe;

                    ws.Cell("A8").Value = "";
                    ws.Cell("A9").Value = "Bulan";

                    ws.Cell("A11").Value = "Januari";
                    ws.Cell("A11").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A11").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A11").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A11").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A12").Value = "Februari";
                    ws.Cell("A12").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A12").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A12").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A12").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A13").Value = "Maret";
                    ws.Cell("A13").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A13").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A13").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A13").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A14").Value = "April";
                    ws.Cell("A14").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A14").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A14").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A14").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A15").Value = "Mei";
                    ws.Cell("A15").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A15").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A15").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A15").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A16").Value = "Juni";
                    ws.Cell("A16").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A16").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A16").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A16").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A17").Value = "Juli";
                    ws.Cell("A17").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A17").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A17").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A17").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A18").Value = "Agustus";
                    ws.Cell("A18").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A18").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A18").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A18").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A19").Value = "September";
                    ws.Cell("A19").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A19").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A19").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A19").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A20").Value = "Oktober";
                    ws.Cell("A20").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A20").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A20").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A20").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A21").Value = "November";
                    ws.Cell("A21").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A21").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A21").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A21").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("A22").Value = "Desember";
                    ws.Cell("A22").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A22").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A22").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("A22").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("A9:A10").Merge();
                    ws.Range("A9:A10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("A9:A10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("A9:A10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("A9:A10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("B9:B10").Merge();
                    ws.Range("B9:B10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("B9:B10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("B9:B10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("B9:B10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("C9:C10").Merge();
                    ws.Range("C9:C10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("C9:C10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("C9:C10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("C9:C10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("D9:D10").Merge();
                    ws.Range("D9:D10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("D9:D10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("D9:D10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("D9:D10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("E9:E10").Merge();
                    ws.Range("E9:E10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("E9:E10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("E9:E10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("E9:E10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("F9:F10").Merge();
                    ws.Range("F9:F10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("F9:F10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("F9:F10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("F9:F10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("G9:G10").Merge();
                    ws.Range("G9:G10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("G9:G10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("G9:G10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("G9:G10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("H9:H10").Merge();
                    ws.Range("H9:H10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("H9:H10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("H9:H10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("H9:H10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("I9:I10").Merge();
                    ws.Range("I9:I10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("I9:I10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("I9:I10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("I9:I10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("J9:J10").Merge();
                    ws.Range("J9:J10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("J9:J10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("J9:J10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("J9:J10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("K9:K10").Merge();
                    ws.Range("K9:K10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("K9:K10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("K9:K10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("K9:K10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("L9:L10").Merge();
                    ws.Range("L9:L10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("L9:L10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("L9:L10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("L9:L10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("M9:M10").Merge();
                    ws.Range("M9:M10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("M9:M10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("M9:M10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("M9:M10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("N9:N10").Merge();
                    ws.Range("N9:N10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("N9:N10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("N9:N10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("N9:N10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("O9:Q9").Merge();
                    ws.Range("O9:Q9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("O9:Q9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("O9:Q9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("O9:Q9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("R9:T9").Merge();
                    ws.Range("R9:T9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("R9:T9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("R9:T9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("R9:T9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("U9:W9").Merge();
                    ws.Range("U9:W9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("U9:W9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("U9:W9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("U9:W9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("X9:Y9").Merge();
                    ws.Range("X9:Y9").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("X9:Y9").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("X9:Y9").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("X9:Y9").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Range("Z9:Z10").Merge();
                    ws.Range("Z9:Z10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Range("Z9:Z10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Range("Z9:Z10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Range("Z9:Z10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("O10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("O10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("O10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("O10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("P10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("P10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("P10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("P10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("Q10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("Q10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("Q10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("Q10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("R10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("R10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("R10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("R10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("S10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("S10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("S10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("S10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("T10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("T10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("T10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("T10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("U10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("U10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("U10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("U10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("V10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("V10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("V10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("V10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("W10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("W10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("W10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("W10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("X10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("X10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("X10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("X10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("Y10").Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                    ws.Cell("Y10").Style.Border.RightBorder = XLBorderStyleValues.Thick;
                    ws.Cell("Y10").Style.Border.TopBorder = XLBorderStyleValues.Thick;
                    ws.Cell("Y10").Style.Border.BottomBorder = XLBorderStyleValues.Thick;

                    ws.Cell("B9").Value = "Jumlah Jaringan ";
                    ws.Cell("C9").Value = "Penjualan Kotor ";
                    ws.Cell("D9").Value = "Penjualan Bersih";
                    ws.Cell("E9").Value = "Penjualan ke 3S + 2S";
                    ws.Cell("F9").Value = "Penjualan ke Parts Shop ";
                    ws.Cell("G9").Value = "Penjualan ke Lain - lain";
                    ws.Cell("H9").Value = "Harga Pokok ";
                    ws.Cell("I9").Value = "Penerimaan Pembelian ";
                    ws.Cell("J9").Value = "Nilai Stock ";
                    ws.Cell("K9").Value = "ITO ";
                    ws.Cell("L9").Value = "ITO(AVG) ";
                    ws.Cell("M9").Value = "Ratio ";
                    ws.Cell("N9").Value = "Ratio Suzuki ";
                    ws.Cell("O9").Value = "Demand ";
                    ws.Cell("R9").Value = "Supply ";
                    ws.Cell("U9").Value = "Service Ratio ";
                    ws.Cell("X9").Value = "Data Stock ";
                    ws.Cell("Z9").Value = "Slow Moving(%) ";

                    ws.Cell("O10").Value = "Line ";
                    ws.Cell("P10").Value = "Qty ";
                    ws.Cell("Q10").Value = "Nilai ";
                    ws.Cell("R10").Value = "Line ";
                    ws.Cell("S10").Value = "Qty ";
                    ws.Cell("T10").Value = "Nilai ";
                    ws.Cell("U10").Value = "Line(%) ";
                    ws.Cell("V10").Value = "Qty(%) ";
                    ws.Cell("W10").Value = "Nilai(%) ";
                    ws.Cell("X10").Value = "Moving Code 4 ";
                    ws.Cell("Y10").Value = "Moving Code 5 ";

                    int rowData = 0;
                    int rowStart = 11;
                    do
                    {
                        ws.Cell("B" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("B" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("B" + (rowStart + rowData)).Value = "-";

                        ws.Cell("C" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("C" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("C" + (rowStart + rowData)).Value = "-";

                        ws.Cell("D" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("D" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("D" + (rowStart + rowData)).Value = "-";

                        ws.Cell("E" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("E" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("E" + (rowStart + rowData)).Value = "-";

                        ws.Cell("F" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("F" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("F" + (rowStart + rowData)).Value = "-";

                        ws.Cell("G" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("G" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("G" + (rowStart + rowData)).Value = "-";

                        ws.Cell("H" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("H" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("H" + (rowStart + rowData)).Value = "-";

                        ws.Cell("I" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("I" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("I" + (rowStart + rowData)).Value = "-";

                        ws.Cell("J" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("J" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("J" + (rowStart + rowData)).Value = "-";

                        ws.Cell("K" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("K" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("K" + (rowStart + rowData)).Value = "-";

                        ws.Cell("L" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("L" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("L" + (rowStart + rowData)).Value = "-";

                        ws.Cell("M" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("M" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("M" + (rowStart + rowData)).Value = "-";

                        ws.Cell("N" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("N" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("N" + (rowStart + rowData)).Value = "-";

                        ws.Cell("O" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("O" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("O" + (rowStart + rowData)).Value = "-";

                        ws.Cell("P" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("P" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("P" + (rowStart + rowData)).Value = "-";

                        ws.Cell("Q" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Q" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("Q" + (rowStart + rowData)).Value = "-";

                        ws.Cell("R" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("R" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("R" + (rowStart + rowData)).Value = "-";

                        ws.Cell("S" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("S" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("S" + (rowStart + rowData)).Value = "-";

                        ws.Cell("T" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("T" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("T" + (rowStart + rowData)).Value = "-";

                        ws.Cell("U" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("U" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("U" + (rowStart + rowData)).Value = "-";

                        ws.Cell("V" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("V" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("V" + (rowStart + rowData)).Value = "-";

                        ws.Cell("W" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("W" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("W" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("W" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("W" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("W" + (rowStart + rowData)).Value = "-";

                        ws.Cell("X" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("X" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("X" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("X" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("X" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("X" + (rowStart + rowData)).Value = "-";

                        ws.Cell("Y" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Y" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Y" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Y" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Y" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("Y" + (rowStart + rowData)).Value = "-";

                        ws.Cell("Z" + (rowStart + rowData)).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Z" + (rowStart + rowData)).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Z" + (rowStart + rowData)).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Z" + (rowStart + rowData)).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        ws.Cell("Z" + (rowStart + rowData)).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        ws.Cell("Z" + (rowStart + rowData)).Value = "-";

                        rowData++;
                    } while (rowData < 12);


                    var check = "";
                    for (int j = 0; j < data.Rows.Count; j++)
                    {
                        int rowLine = 0;
                        var dtl = data.Rows[j][4].ToString();
                        if (sheetName == dtl)
                        {
                            check = data.Rows[j][7].ToString();
                            if (check == "1")
                            {
                                rowLine = rowLine + 11;
                            }
                            else if (check == "2")
                            {
                                rowLine = rowLine + 12;
                            }
                            else if (check == "3")
                            {
                                rowLine = rowLine + 13;
                            }
                            else if (check == "4")
                            {
                                rowLine = rowLine + 14;
                            }
                            else if (check == "5")
                            {
                                rowLine = rowLine + 15;
                            }
                            else if (check == "6")
                            {
                                rowLine = rowLine + 16;
                            }
                            else if (check == "7")
                            {
                                rowLine = rowLine + 17;
                            }
                            else if (check == "8")
                            {
                                rowLine = rowLine + 18;
                            }
                            else if (check == "9")
                            {
                                rowLine = rowLine + 19;

                            }
                            else if (check == "10")
                            {
                                rowLine = rowLine + 20;
                            }
                            else if (check == "11")
                            {
                                rowLine = rowLine + 21;
                            }
                            else if (check == "12")
                            {
                                rowLine = rowLine + 22;
                            }
                            ws.Cell("B" + rowLine).Value = data.Rows[j][8].ToString();
                            ws.Cell("C" + rowLine).Value = data.Rows[j][9].ToString();
                            ws.Cell("D" + rowLine).Value = data.Rows[j][10].ToString();
                            ws.Cell("E" + rowLine).Value = data.Rows[j][11].ToString();
                            ws.Cell("F" + rowLine).Value = data.Rows[j][12].ToString();
                            ws.Cell("G" + rowLine).Value = data.Rows[j][13].ToString();
                            ws.Cell("H" + rowLine).Value = data.Rows[j][14].ToString();
                            ws.Cell("I" + rowLine).Value = data.Rows[j][15].ToString();
                            ws.Cell("J" + rowLine).Value = data.Rows[j][16].ToString();
                            ws.Cell("K" + rowLine).Value = data.Rows[j][17].ToString();
                            ws.Cell("L" + rowLine).Value = data.Rows[j][18].ToString();
                            ws.Cell("M" + rowLine).Value = data.Rows[j][19].ToString();
                            ws.Cell("N" + rowLine).Value = data.Rows[j][20].ToString();
                            ws.Cell("O" + rowLine).Value = data.Rows[j][21].ToString();
                            ws.Cell("P" + rowLine).Value = data.Rows[j][22].ToString();
                            ws.Cell("Q" + rowLine).Value = data.Rows[j][23].ToString();
                            ws.Cell("R" + rowLine).Value = data.Rows[j][24].ToString();
                            ws.Cell("S" + rowLine).Value = data.Rows[j][25].ToString();
                            ws.Cell("T" + rowLine).Value = data.Rows[j][26].ToString();
                            ws.Cell("U" + rowLine).Value = data.Rows[j][27].ToString();
                            ws.Cell("V" + rowLine).Value = data.Rows[j][28].ToString();
                            ws.Cell("W" + rowLine).Value = data.Rows[j][29].ToString();
                            ws.Cell("X" + rowLine).Value = data.Rows[j][30].ToString();
                            ws.Cell("Y" + rowLine).Value = data.Rows[j][31].ToString();
                            ws.Cell("Z" + rowLine).Value = data.Rows[j][32].ToString();
                        }
                    }
                }

                wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName + ".xlsx"));
                return Redirect(Url.Content("~/ReportTemp/" + fileName + ".xlsx"));
            }
            #endregion
        }
    }
}
