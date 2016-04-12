using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Sales.Models;
using SimDms.Common;
using GeLang;
using SimDms.Common.Models;
using SimDms.Sales.BLL;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace SimDms.Sales.Controllers.Api
{
    public class GridController : BaseController
    {
        public JsonResult ItsList()
        {
            var data = ctx.Database.SqlQuery<ITS>("exec uspfn_SelectITSNo @CompanyCode=@p0, @BranchCode=@p1", CompanyCode, BranchCode).AsQueryable();
            string inquiryNo = Request["filterInquiryNo"] ?? "";
            string vehicleType = Request["filterTipeKendaraan"] ?? "";
            string salesman = Request["filterSalesman"] ?? "";
            string namaProspek = Request["filterNamaProspek"] ?? "";

            if (!string.IsNullOrWhiteSpace(inquiryNo))
            {
                data = data.Where(x => x.InquiryNo.Contains(inquiryNo));
            }
            if (!string.IsNullOrWhiteSpace(vehicleType))
            {
                data = data.Where(x => x.TipeKendaraan.Contains(vehicleType));
            }
            if (!string.IsNullOrWhiteSpace(salesman))
            {
                data = data.Where(x => x.EmployeeName != null && x.EmployeeName.Contains(salesman));
            }
            if (!string.IsNullOrWhiteSpace(namaProspek))
            {
                data = data.Where(x => x.NamaProspek.Contains(namaProspek));
            }

            return Json(data.KGrid());
        }

        public JsonResult SoCustomers()
        {
            var data = ctx.Database.SqlQuery<SOCustomer>("exec uspfn_SelectCustomerSO @CompanyCode=@p0, @BranchCode=@p1, @UserID=@p2", CompanyCode, BranchCode, CurrentUser.UserId).AsQueryable();

            string customerCode = Request["filterCustomerCode"] ?? "";
            string customerName = Request["filterCustomerName"] ?? "";

            if (!string.IsNullOrWhiteSpace(customerCode))
            {
                data = data.Where(x => x.CustomerCode.Contains(customerCode));
            }

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                data = data.Where(x => x.CustomerName.Contains(customerName));
            }

            return Json(data.KGrid());
        }

        public JsonResult SalesmanList()
        {
            string employeeID = Request["filterEmployeeID"] ?? "";
            string employeeName = Request["filterEmployeeName"] ?? "";

            var data = ctx.HrEmployeeViews.Where(x => x.Department == "SALES" && x.Position == "S");

            if (!string.IsNullOrWhiteSpace(employeeID))
            {
                data = data.Where(x => x.EmployeeID.Contains(employeeID));
            }

            if (!string.IsNullOrWhiteSpace(employeeName))
            {
                data = data.Where(x => x.EmployeeName.Contains(employeeName));
            }

            return Json(data.KGrid());
        }

        public JsonResult TopCList()
        {
            var data = ctx.LookUpDtls.Where(x => x.CodeID == "TOPC");
            string topCode = Request["filterTOPCode"] ?? "";
            string topName = Request["filterTOPName"] ?? "";

            if (!string.IsNullOrWhiteSpace(topCode))
            {
                data = data.Where(x => x.LookUpValue.Contains(topCode));
            }

            if (!string.IsNullOrWhiteSpace(topName))
            {
                data = data.Where(x => x.LookUpValueName.Contains(topName));
            }
            return Json(data.KGrid());
        }

        public JsonResult WarehouseListRetur()
        {
            // update
            var data = ctx.LookUpDtls.Where(x => x.CompanyCode == CompanyCode && x.CodeID == "MPWH" && x.ParaValue == BranchCode);

            string warehouseCode = Request["filterWarehouseCode"] ?? "";
            string warehouseName = Request["filterWarehouseName"] ?? "";

            if (!string.IsNullOrWhiteSpace(warehouseCode))
            {
                data = data.Where(x => x.LookUpValue == warehouseCode);
            }

            if (!string.IsNullOrWhiteSpace(warehouseName))
            {
                data = data.Where(x => x.LookUpValueName == warehouseName);
            }

            return Json(data.KGrid());
        }

        public JsonResult WarehouseList()
        {
            var data = ctxMD.LookUpDtls.Where(x => x.CompanyCode == CompanyMD && x.CodeID == "MPWH" && x.ParaValue == BranchMD);

            if (!cekOtomatis())
            {
                data = ctx.LookUpDtls.Where(x => x.CompanyCode == CompanyCode && x.CodeID == "MPWH" && x.ParaValue == BranchCode);
            }

            string warehouseCode = Request["filterWarehouseCode"] ?? "";
            string warehouseName = Request["filterWarehouseName"] ?? "";

            if (!string.IsNullOrWhiteSpace(warehouseCode))
            {
                data = data.Where(x => x.LookUpValue == warehouseCode);
            }

            if (!string.IsNullOrWhiteSpace(warehouseName))
            {
                data = data.Where(x => x.LookUpValueName == warehouseName);
            }

            return Json(data.KGrid());
        }

        public JsonResult SalesModelList()
        {
            string salesModelCode = Request["filterSalesModelCode"] ?? "";
            string salesModelDesc = Request["filterSalesModelDesc"] ?? "";
            string inquiryNumber = Request["InquiryNumber"] ?? "";
            var data = ctx.Database.SqlQuery<SalesModel>("exec uspfn_SelectSalesModel @CompanyCode=@p0, @BranchCode=@p1, @InquiryNumber=@p2", CompanyCode, BranchCode, inquiryNumber).AsQueryable();

            if (!string.IsNullOrWhiteSpace(salesModelCode))
            {
                data = data.Where(x => x.SalesModelCode.Contains(salesModelCode));
            }

            if (!string.IsNullOrWhiteSpace(salesModelDesc))
            {
                data = data.Where(x => x.SalesModelDesc.Contains(salesModelDesc));
            }

            return Json(data.KGrid());
        }

        public JsonResult LeasingList()
        {
            string leasingCode = Request["filterLeasingCode"] ?? "";
            string leasingName = Request["filterLeasingName"] ?? "";
            var data = ctx.Database.SqlQuery<Leasing>("exec uspfn_SelectLeasing @CompanyCode=@p0, @BranchCode=@p1, @UserID=@p2", CompanyCode, BranchCode, CurrentUser.UserId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(leasingCode))
            {
                data = data.Where(x => x.LeasingCode.Contains(leasingCode));
            }

            if (!string.IsNullOrEmpty(leasingName))
            {
                data = data.Where(x => x.LeasingName.Contains(leasingName));
            }

            return Json(data.KGrid());
        }

        public JsonResult EmployeeList()
        {
            string employeeID = Request["filterEmployeeID"] ?? "";
            string employeeName = Request["filterEmployeeName"] ?? "";
            string position = Request["filterEmployeePosition"] ?? "";
            bool isSalesman = Convert.ToBoolean(Request["isSalesman"] ?? "false");

            var data = ctx.HrEmployeeViews.AsQueryable();

            if (isSalesman)
            {
                data = data.Where(x => x.Department == "SALES" && x.Position == "S");
            }

            if (!string.IsNullOrWhiteSpace(employeeID))
            {
                data = data.Where(x => x.EmployeeID.Contains(employeeID));
            }

            if (!string.IsNullOrWhiteSpace(employeeName))
            {
                data = data.Where(x => x.EmployeeName.Contains(employeeName));
            }

            if (!string.IsNullOrWhiteSpace(position))
            {
                data = data.Where(x => x.PositionName.Contains(position));
            }

            return Json(data.KGrid());
        }

        public JsonResult SalesModelYearList()
        {
            string salesModelCode = Request["SalesModelCode"] ?? "";
            string groupPriceCode = Request["GroupPriceCode"] ?? "";

            var data = ctx.Database.SqlQuery<SalesModelYearModel>("exec uspfn_Select4SalesModelYear @CompanyCode=@p0, @SalesModelCode=@p1, @GroupPriceCode=@p2", CompanyCode, salesModelCode, groupPriceCode).AsQueryable();

            string filterSalesModelYearRaw = Request["filterSalesModelYear"];
            if (!string.IsNullOrWhiteSpace(filterSalesModelYearRaw))
            {
                try
                {
                    decimal? filterSalesModelYear = Convert.ToDecimal(filterSalesModelYearRaw);
                    data = data.Where(x => x.SalesModelYear == filterSalesModelYear);
                }
                catch (Exception) { }
            }

            string filterChassisCode = Request["filterChassisCode"] ?? "";
            if (!string.IsNullOrWhiteSpace(filterChassisCode))
            {
                data = data.Where(x => x.ChassisCode.Contains(filterChassisCode));
            }

            return Json(data.KGrid());
        }

        public JsonResult SalesOrderList()
        {
            var data = ctx.Database.SqlQuery<SalesOrderForm>("exec uspfn_ItsLkuSOList @CompanyCode=@p0, @BranchCode=@p1", CompanyCode, BranchCode).AsQueryable();
            string soNumber = Request["filterSONumber"] ?? "";

            if (!string.IsNullOrWhiteSpace(soNumber))
            {
                data = data.Where(x => x.SONumber.Contains(soNumber));
            }
            return Json(data.KGrid());
        }

        public JsonResult SavedSalesModelList(SalesOrderForm model)
        {
            string soNumber = Request["SONumber"] ?? "";
            var data = ctx.Database.SqlQuery<SalesModelList>("exec uspfn_OmTrSalesModelList @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", CompanyCode, BranchCode, model.SONumber);

            return Json(data);
        }

        public JsonResult ColourModelList(SalesOrderForm model)
        {
            string salesModelCode = Request["SalesModelCode"] ?? "";
            string soNumber = Request["InquiryNumber"] ?? "";

            var data = ctx.Database.SqlQuery<ColourModel>("exec uspfn_SelectColourModel @CompanyCode=@p0, @BranchCode=@p1, @SalesModelCode=@p2, @InquiryNumber=@p3", CompanyCode, BranchCode, salesModelCode, soNumber).AsQueryable();

            string colourCode = Request["filterColourCode"] ?? "";
            string colourName = Request["filterColourDesc"] ?? "";

            if (!string.IsNullOrWhiteSpace(colourCode))
            {
                data = data.Where(x => x.ColourCode.Contains(colourCode));
            }

            if (!string.IsNullOrWhiteSpace(colourName))
            {
                data = data.Where(x => x.ColourDesc.Contains(colourName));
            }

            return Json(data.KGrid());
        }

        public JsonResult SupplierBNNList(SalesModelForm model)
        {
            var data = ctx.Database.SqlQuery<SupplierBNNModel>("exec uspfn_SelectBNN @CompanyCode=@p0, @BranchCode=@p1, @SalesModelCode=@p2, @SalesModelYear=@p3, @UserID=@p4", CompanyCode, BranchCode, model.SalesModelCode, model.SalesModelYear, CurrentUser.UserId).AsQueryable();

            string filterSupplierCode = Request["filterSupplierCode"] ?? "";
            string filterSupplierName = Request["filterSupplierName"] ?? "";

            if (!string.IsNullOrWhiteSpace(filterSupplierCode))
            {
                data = data.Where(x => x.SupplierCode.Contains(filterSupplierCode));
            }

            if (!string.IsNullOrWhiteSpace(filterSupplierName))
            {
                data = data.Where(x => x.SupplierName.Contains(filterSupplierName));
            }

            return Json(data.KGrid());
        }

        public JsonResult AccOthersList(SalesOrderForm model)
        {
            var data = ctx.Database.SqlQuery<AccOtherModel>("exec uspfn_SelectAccOthers @CompanyCode=@p0, @Reff=@p1", CompanyCode, "OTHS").AsQueryable();

            string reffCode = Request["filterRefferenceCode"] ?? "";
            string reffDesc = Request["filterRefferenceDesc"] ?? "";

            if (string.IsNullOrWhiteSpace(reffCode) == false)
            {
                data = data.Where(x => x.RefferenceCode.Contains(reffCode));
            }

            if (string.IsNullOrWhiteSpace(reffDesc) == false)
            {
                data = data.Where(x => x.RefferenceDesc.Contains(reffDesc));
            }

            return Json(data.KGrid());
        }

        public JsonResult PartList(SalesOrderForm model)
        {
            var data = ctx.Database.SqlQuery<SelectPartsModel>("exec uspfn_SelectParts @CompanyCode=@p0, @Reff=@p1", CompanyCode).AsQueryable();

            string partNo = Request["filterPartNo"] ?? "";
            string partName = Request["filterPartName"] ?? "";

            if (string.IsNullOrWhiteSpace(partNo) == false)
            {
                data = data.Where(x => x.PartNo.Contains(partNo));
            }

            if (string.IsNullOrWhiteSpace(partName) == false)
            {
                data = data.Where(x => x.PartName.Contains(partName));
            }

            return Json(data.KGrid());
        }

        public JsonResult SavedColourModelList()
        {
            string soNumber = Request["SONumber"] ?? "";
            string salesModelCode = Request["SalesModelCode"] ?? "";
            decimal salesModelYear = 0;
            try
            {
                salesModelYear = Convert.ToDecimal(Request["SalesModelYear"] ?? "0");
            }
            catch (Exception) { }

            var data = ctx.Database.SqlQuery<ColourModelList>("exec uspfn_SOLkuColourModelList @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2, @SalesModelCode=@p3, @SalesModelYear=@p4", CompanyCode, BranchCode, soNumber, salesModelCode, salesModelYear);
            return Json(data);
        }

        public JsonResult SavedOmTrSalesSOModelOthersList()
        {
            string soNumber = Request["SONumber"] ?? "";
            string salesModelCode = Request["SalesModelCode"] ?? "";
            decimal salesModelYear = 0;
            try
            {
                salesModelYear = Convert.ToDecimal(Request["SalesModelYear"] ?? "0");
            }
            catch (Exception) { }

            var data = ctx.Database.SqlQuery<OmTrSalesSOModelOtherList>("exec uspfn_OmTrSalesSOModelOthersList @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2, @SalesModelCode=@p3, @SalesModelYear=@p4", CompanyCode, BranchCode, soNumber, salesModelCode, salesModelYear);
            return Json(data);
        }

        public JsonResult ChassisList()
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string salesModelCode = Request["SalesModelCode"] ?? "";
            decimal salesModelYear = Convert.ToDecimal(Request["SalesModelYear"] ?? "0");
            string chassisCode = Request["ChassisCode"] ?? "";
            string colourCode = Request["ColourCode"] ?? "";
            string warehouseCode = Request["WarehouseCode"] ?? "";

            var data = ctx.Database.SqlQuery<ChassisListModel>("exec uspfn_SelectChassis @CompanyCode=@p0, @BranchCode=@p1, @SalesModelCode=@p2, @SalesModelYear=@p3, @ChassisCode=@p4, @ColourCode=@p5, @WarehouseCode=@p6", companyCode, branchCode, salesModelCode, salesModelYear, chassisCode, colourCode, warehouseCode).AsQueryable();

            return Json(data.KGrid());
        }

        public JsonResult CityList()
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string salesModelCode = Request["SalesModelCode"] ?? "";
            decimal salesModelYear = Convert.ToDecimal(Request["SalesModelYear"] ?? "0");
            string suppplierCode = Request["SupplierCode"] ?? "";

            var data = ctx.Database.SqlQuery<CityListModel>("exec uspfn_SelectCity @CompanyCode=@p0, @BranchCode=@p1, @SalesModelCode=@p2, @SalesModelYear=@p3, @SupplierCode=@p4", companyCode, branchCode, salesModelCode, salesModelYear, suppplierCode).AsQueryable();

            string filterCityCode = Request["filterCityCode"] ?? "";
            string filterCityName = Request["filterCityName"] ?? "";

            if (!string.IsNullOrWhiteSpace(filterCityCode))
            {
                data = data.Where(x => x.CityCode.Contains(filterCityCode));
            }

            if (!string.IsNullOrWhiteSpace(filterCityName))
            {
                data = data.Where(x => x.CityName.Contains(filterCityName));
            }

            return Json(data.KGrid());
        }

        public JsonResult OthersList()
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;
            string soNumber = Request["SONumber"] ?? "";
            string salesModelCode = Request["SalesModelCode"] ?? "";
            decimal salesModelYear = 0;
            try
            {
                salesModelYear = Convert.ToDecimal(Request["SalesModelYear"] ?? "0");
            }
            catch (Exception) { }
            string colourCode = Request["ColourCode"] ?? "";

            var data = ctx.omTrSalesSOVins.Where(x =>
                       x.CompanyCode == companyCode
                    && x.BranchCode == branchCode
                    && x.SONo == soNumber
                    && x.SalesModelCode == salesModelCode
                    && x.SalesModelYear == salesModelYear
                    && x.ColourCode == colourCode
                );

            return Json(data);
        }

        public JsonResult getPartAccessories(string InvoiceNo)
        {
            var qry = String.Format(@"SELECT a.PartNo, b.PartName, a.SupplySlipNo, a.Quantity, a.DPP, a.PPn, a.Total FROM omtrsalesinvoiceaccsseq a 
                                      LEFT JOIN spMstItemInfo b ON b.PartNo = a.PartNo AND b.CompanyCode = a.CompanyCode 
                                      WHERE a.CompanyCode='{0}' AND a.BranchCode='{1}' AND  a.InvoiceNo='{2}'", CompanyCode, BranchCode, InvoiceNo);
            var dt = ctx.Database.SqlQuery<omSlsInvSlctFrTblInvAccSeq>(qry).AsQueryable();
            return Json(new { success = true, data = dt});
        }

        public JsonResult SparePartList()
        {
            string companyCode = CompanyCode;
            string branchCode = BranchCode;

            var data = ctx.Database.SqlQuery<SparePartList>("exec uspfn_SelectParts @CompanyCode=@p0, @BranchCode=@p1", companyCode, branchCode).AsQueryable();

            string filterPartNo = Request["filterPartNo"] ?? "";
            string filterPartName = Request["filterPartName"] ?? "";

            if (!string.IsNullOrWhiteSpace(filterPartNo))
            {
                data = data.Where(x => x.PartNo.Contains(filterPartNo));
            }

            if (!string.IsNullOrWhiteSpace(filterPartName))
            {
                data = data.Where(x => x.PartName.Contains(filterPartName));
            }

            return Json(data.KGrid());
        }

        public JsonResult SavedOmTrSalesSOAccSeq()
        {
            string soNumber = Request["SONumber"] ?? "";

            var data = ctx.Database.SqlQuery<OmTrSalesSOAccsSeqList>("exec uspfn_omTrSalesSOAccSeqList @CompanyCode=@p0, @BranchCode=@p1, @SONumber=@p2", CompanyCode, BranchCode, soNumber);
            return Json(data);
        }



        #region Sales Order
        public JsonResult SlsSOBrowse()
        {
            
//            var query = string.Format(@"g
//            select EmployeeID,EmployeeName,TitleCode,case when TitleName is null then '' else TitleName end as TitleName
//            from
//            (
//               SELECT 
//                    a.EmployeeID, a.EmployeeName, a.TitleCode
//                    , (SELECT LookUpValueName FROM gnMstLookupDtl WHERE CompanyCode = a.CompanyCode and CodeID = 'TITL' AND LookUpValue = a.TitleCode) AS TitleName
//                FROM GnMstEmployee a
//                WHERE a.CompanyCode = '{0}' 
//                    AND a.BranchCode  = '{1}'
//				)c", CompanyCode, BranchCode);


            var rslt = ctx.Database.SqlQuery<TrSalesSOView>(string.Format("exec uspfn_omSoLkp {0},{1}", CompanyCode, BranchCode)).AsQueryable();
            //Helpers.ReplaceNullable(rslt);
            
            
            return Json(rslt.toKG());
        }

        public JsonResult SlsSOPOBrowse()
        {
            var rslt = ctx.Database.SqlQuery<TrSalesSOView>(string.Format("exec uspfn_omSoPOLkp {0},{1}", CompanyCode, BranchCode)).AsQueryable();
            return Json(rslt.toKG());
        }

        public JsonResult CustBrowse(int cols, string spId)
        {
            var field = "";
            var value = "";
            string companyCode = CompanyCode;
            string branchCode = BranchCode;

            string dynamicFilter = "";

            for (int i = 0; i < cols; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (field=="CustomerCode")
                {
                    field = "a." + field;
                }

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? ",' AND " + field + " LIKE ''%" + value + "%''" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE ''%" + value + "%''" : "";
                }
            }

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            var query = string.Format(@"exec {0} '{1}', '{2}', '{3}' {4}", spId, CompanyCode, BranchCode,"100", dynamicFilter);
            var queryable = ctx.Database.SqlQuery<MstCustomerView>(query).AsQueryable();
            return Json(queryable.toKG());
        }


        #endregion
        #region OM Sales DO
        public JsonResult SlsDoBrowse()
        {
            ctx.Database.CommandTimeout = 60;
            var rslt = ctx.Database.SqlQuery<omSlsDOBrowse>(string.Format("exec uspfn_omSlsDoBrowse {0},{1}", CompanyCode, BranchCode)).AsQueryable();
            return Json(rslt.toKG());
        }

        public JsonResult SlsDoLkpso()
        {           
            var rslt = ctx.Database.SqlQuery<omSlsDOLkpSO>(string.Format("exec uspfn_omSlsDoLkpSO {0},{1},{2}", CompanyCode, BranchCode,"WARE")).AsQueryable();
            return Json (rslt.toKG());
        }

        public JsonResult SlsDoLkpShipto()
        {
            var rslt = ctx.Database.SqlQuery<GnMstCustomer>(string.Format("exec uspfn_omSlsDoLkpShipto {0},{1},{2}", CompanyCode, BranchCode, getprofitcenter())).AsQueryable().toKG();                      
            return Json(new {total=rslt.total,data=rslt.data.Select(x => new {CustomerCode=x.CustomerCode,CustomerName=x.CustomerName })});
        }

        public JsonResult SlsDoLkpShiptoV2(int cols)
        {
            var field = "";
            var value = "";
            string companyCode = CompanyCode;
            string branchCode = BranchCode;

            string dynamicFilter = "";

            for (int i = 0; i < cols; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (field == "CustomerCode")
                {
                    field = "a." + field;
                }

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? ",' AND " + field + " LIKE ''%" + value + "%''" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE ''%" + value + "%''" : "";
                }
            }

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            var query = string.Format(@"exec uspfn_omSlsDoLkpShiptoV2 '{0}', '{1}', '{2}' {3}", CompanyCode, BranchCode, getprofitcenter(), dynamicFilter);
            var queryable = ctx.Database.SqlQuery<GnMstCustomer>(query).AsQueryable();
            return Json(queryable.toKG());
        }

        public JsonResult SlsDoLkpExpedition()
        {
            var rslt = ctx.Database.SqlQuery<omSlsDOLkpExpdtion>(string.Format("exec uspfn_omSlsDoLkpExpdtn {0},{1},{2},{3}", CompanyCode, BranchCode,"1", getprofitcenter())).AsQueryable().toKG();
            
            return Json(new { total = rslt.total, 
                              data = rslt.data.Select(x => new { SupplierCode = x.SupplierCode, SupplierName = x.Suppliername }) });
        }

        public JsonResult SlsDoLkpSlsMdlCd()
        {
            string sono = Request["SONo"]??"";

            var rslt = ctx.Database.SqlQuery<OmMstModel>(string.Format("exec uspfn_omSlsDoLkpMdlCode {0},{1},'{2}'", CompanyCode, BranchCode, sono)).AsQueryable().toKG();

            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x => new { SalesModelCode = x.SalesModelCode, SalesModelDesc = x.SalesModelDesc })
            });

        }

        public JsonResult SlsDoLkpSlsMdlYear()
        {
            string sono = Request["SONo"]??"";
            string slsmdlcd = Request["SalesModelCode"] ?? "";

            var rslt = ctx.Database.SqlQuery<MstModelYear>(string.Format("exec uspfn_omSlsDoLkpMdlYear {0},{1},'{2}','{3}'", CompanyCode, BranchCode, sono, slsmdlcd)).AsQueryable().toKG();

            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x => new { SalesModelYear = x.SalesModelYear, SalesModelDesc = x.SalesModelDesc, ChassisCode = x.ChassisCode })
            });
        }

        public JsonResult SlsDoLkpChasisNo()
        {            
            string sono = Request["SONo"]??"";
            string slsmdlcd = Request["SalesModelCode"] ?? "";
            string slsmdlyr = Request["SalesModelYear"] ?? "";
            string chassicode = Request["ChassisCode"] ?? "";
            string reftype = "COLO";
            string wrhcd = Request["WareHouseCode"] ?? "";

            var rslst = ctx.Database.SqlQuery<OmInquiryChassisDO>(string.Format("exec uspfn_OmInquiryChassisDO {0},{1},'{2}','{3}',{4},'{5}','{6}','{7}'", 
                CompanyCode, BranchCode, sono, slsmdlcd,slsmdlyr,chassicode,reftype,wrhcd)).AsQueryable().toKG();

            
            return Json(rslst);
        }

        public JsonResult SlsDoDtl(string DONo)
        {
                var rslst = ctx.Database.SqlQuery<omSlsDoDtl>(string.Format("exec uspfn_omSlsDoDtl {0},{1},'{2}'",CompanyCode,BranchCode,DONo));
                return Json(rslst);           
        }
       
        #endregion

        #region Om Sales BPK
        public JsonResult SlsBPKBrowse()
        {
            var rslt = ctx.Database.SqlQuery<omSlsBPKBrowse>(string.Format("exec uspfn_omSlsBPKBrowse {0},{1}", CompanyCode, BranchCode)).AsQueryable();
            return Json(rslt.toKG());
        }

        public JsonResult SlsBpkDtl(string BPKNo)
        {
            var rslt = ctx.Database.SqlQuery<omSlsBPKDtl>(string.Format("exec uspfn_omSlsBPKBrwDtl {0},{1},'COLO','{2}'", CompanyCode, BranchCode, BPKNo));
            return Json(rslt);
        }

        public JsonResult SlsBpkLkpDO()
        {
            var rslt = ctx.Database.SqlQuery<omSlsBPkLkpDO>(string.Format("exec uspfn_omSlsBPKLkpDO {0},{1},'{2}'", CompanyCode, BranchCode, ProfitCenter)).AsQueryable();
            return Json(rslt.toKG());
        }

        public JsonResult SlsBPKLkpSlsMdlCd()
        {
            string dono = Request["DONo"] ?? "";

            var rslt = ctx.Database.SqlQuery<OmMstModel>(string.Format("exec uspfn_omSlsBPKLkpMdlCode {0},{1},'{2}'", CompanyCode, BranchCode, dono)).AsQueryable().toKG();

            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x => new { SalesModelCode = x.SalesModelCode, SalesModelDesc = x.SalesModelDesc })
            });

        }

        public JsonResult SlsBPKLkpSlsMdlYear()
        {
            string dono = Request["DONo"] ?? "";
            string slsmdlcd = Request["SalesModelCode"] ?? "";

            var rslt = ctx.Database.SqlQuery<MstModelYear>(string.Format("exec uspfn_omSlsBPKLkpMdlYear {0},{1},'{2}','{3}'", CompanyCode, BranchCode, dono, slsmdlcd)).AsQueryable().toKG();

            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x => new { SalesModelYear = x.SalesModelYear, SalesModelDesc = x.SalesModelDesc, ChassisCode = x.ChassisCode })
            });
        }

        public JsonResult SlsBPKLkpChasisNo()
        {
            string dono = Request["DONo"] ?? "";
            string bpkno = Request["BPKNo"] ?? "";
            
            string chassicode = Request["ChassisCode"] ?? "";
            string reftype = "COLO";
            string wrhcd = Request["WareHouseCode"] ?? "";

            var rslst = ctx.Database.SqlQuery<omSLsBPKLkpChasisNo>(string.Format("exec uspfn_omSlsBPKLkpChassisNo {0},{1},'{2}','{3}',{4}",
                CompanyCode, BranchCode, dono, bpkno, chassicode)).AsQueryable().toKG();

            return Json(new
            {
                total = rslst.total,
                data = rslst.data
            });
            
        }

        #endregion

        #region Om Sales Perlengkapan Out
        public JsonResult SlsPerlengkapanOutBrowse()
        {
            var rslt = ctx.omTrSalesPerlengkapanOuts
                       .Where(x => x.CompanyCode == CompanyCode &&
                                x.BranchCode == BranchCode)
                       .OrderByDescending(x => x.PerlengkapanNo)
                       .toKG();

            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x =>
                    new
                    {
                        PerlengkapanNo = x.PerlengkapanNo,
                        PerlengkapanDate = x.PerlengkapanDate,
                        CustomerCode=x.CustomerCode,
                        ReferenceNo = x.ReferenceNo,
                        SourceDoc=x.SourceDoc,
                        Remark=x.Remark,
                        PerlengkapanType=x.PerlengkapanType,
                        PerlengkapanTypeDsc=
                                x.PerlengkapanType=="1"?"BPK": 
                                x.PerlengkapanType=="2"?"TRansfeer":
                                x.PerlengkapanType=="3"?"Return":"",
                        Status=x.Status,
                        StatusDsc=getStringStatus(x.Status)
                    })
            });       
        }

        public JsonResult SlsPerlengkapanOutDetailSalesCode(omTrSalesPerlengkapanOut hdr)
        {
            var plkpmodel = ctx.OmTrSalesPerlengkapanOutModels
                           .Where(x => x.CompanyCode == CompanyCode &&
                                     x.BranchCode == BranchCode &&
                                     x.PerlengkapanNo == hdr.PerlengkapanNo)
                           .OrderBy(x => x.SalesModelCode)
                           .Select(x => new
                           {
                               SalesModelCode = x.SalesModelCode,
                               Quantity = x.Quantity,
                               Remark = x.Remark
                           });

            var cust= ctx.GnMstCustomer.Find(CompanyCode,hdr.CustomerCode);

            return Json(new {data=plkpmodel,CustomerName=(cust==null?"":cust.CustomerName)});
        }

        public JsonResult SlsPerlengkapanOutDetailPerlengkapan(OmTrSalesPerlengkapanOutModel mdl)
        {
            string squery = string.Format("exec uspfn_omSlsPrlgkpnOutDtl {0},{1},'{2}','{3}'", CompanyCode, BranchCode, mdl.PerlengkapanNo, mdl.SalesModelCode);
            var dtlplkp=ctx.Database.SqlQuery<omSalesPerlengkapanOutDtl>(squery).AsQueryable();
            return Json(dtlplkp);
        }

        public JsonResult SlsPerlengkapanOutBrwSrcDoc(string PerlengkapanType)
        {
            if (PerlengkapanType == "1")
            {
                return Json(ctx.Database.SqlQuery<omSalesPerlengkapanOutBrwDocBPK>(string.Format("exec uspfn_omSlsPrlgkpnOutBrwDocBPK {0},{1},{2}", CompanyCode, BranchCode, getprofitcenter())).AsQueryable().toKG());
            }
            else if (PerlengkapanType == "2")
            {
                return Json(ctx.Database.SqlQuery<omSalesPerlengkapanOutBrwDocTransfer>(string.Format("exec uspfn_omSlsPrlgkpnOutBrwDocTransfer {0},{1}", CompanyCode, BranchCode )).AsQueryable().toKG());
            }
            else
            {
                var rslt = ctx.omTrPurchaseReturn
                            .Where(x => x.CompanyCode == CompanyCode &&
                                       x.BranchCode == BranchCode &&
                                       x.Status == "2")
                            .toKG();

                return Json(new
                {
                    total = rslt.total,
                    data = rslt.data.Select(x => new { ReturnNo = x.ReturnNo })
                });    
            }
        }

        public JsonResult SlsPerlengkapanOutLkpSlsMdlCd(omTrSalesPerlengkapanOut hdr)
        {

            var rslt = OmTrSalesBLL.Instance(CurrentUser.UserId).Select4LookupModel(hdr).toKG();
            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x => new { SalesModelCode = x.SalesModelCode, SalesModelDesc = x.SalesModelDesc })
            });    

        }

        public JsonResult SlsPerlengkapanOutLkpSlsMdlDtl(string SalesModelCode)
        {
            var rslt = ctx.Database.SqlQuery<omSlsPrlgkpnOutLkpMdlDtl>(string.Format("exec uspfn_omSlsPrlgkpnOutLkpMdlDtl {0},{1},'{2}'", CompanyCode, BranchCode, SalesModelCode)).AsQueryable().toKG();
            return Json(rslt);
        }
        #endregion


        #region Om Sales Invoice
        public JsonResult SlsInvLkpBrowse()
        {
            return Json(ctx.Database.SqlQuery<omSlsInvBrowse>(string.Format("exec uspfn_omSlsInvBrowse {0},{1}",
                            CompanyCode, BranchCode))
                            .AsQueryable().toKG());

        }

        public JsonResult SlsInvDtlBPk(OmTrSalesInvoice hdr)
        {
            return Json(ctx.omTrSalesInvoiceBPK
                            .Where(x => x.CompanyCode == CompanyCode &&
                                        x.BranchCode == BranchCode &&
                                        x.InvoiceNo == hdr.InvoiceNo));
        }


                

        public JsonResult SlsInvDtlSlsModel(omTrSalesInvoiceModel hdr)
        {
            return Json(ctx.omTrSalesInvoiceModel
                            .Where(x => x.CompanyCode == CompanyCode &&
                                        x.BranchCode == BranchCode &&
                                        x.InvoiceNo == hdr.InvoiceNo));
        }


        
        public JsonResult SlsInvDtl(OmTrSalesInvoice hdr)
        {
            return Json(ctx.omTrSalesInvoiceBPK
                            .Where(x => x.CompanyCode == CompanyCode &&
                                        x.BranchCode == BranchCode &&
                                        x.InvoiceNo == hdr.InvoiceNo));
        }

        public JsonResult SlsInvLkpSO()
        {
            return Json(ctx.Database.SqlQuery<omSlsInvLkpSO>(string.Format("exec uspfn_omSlsInvLkpSO {0},{1}",
                          CompanyCode, BranchCode))
                          .AsQueryable().toKG());
        }


        public JsonResult SlsInvLkpBillTo()
        {
            return Json(ctx.Database.SqlQuery<omSlsInvLkpBillTo>(string.Format("exec uspfn_omSlsInvLkpBillTo {0},{1},{2}",
                      CompanyCode, BranchCode,getprofitcenter()))
                      .AsQueryable().toKG());                                    
        }

        public JsonResult SlsInvLkpBPK(OmTrSalesInvoice hdr)
        {
            return Json(ctx.Database.SqlQuery<omSlsInvLkpBPK>(string.Format("exec uspfn_omSlsInvLkpBPK {0},{1},'{2}','{3}'",
                      CompanyCode, BranchCode, hdr.SONo,hdr.InvoiceDate))
                      .AsQueryable().toKG()); 
        }


        public JsonResult SlsInvLkpSlsMdlCd(string BPKNo)
        {
            var rslt = ctx.Database.SqlQuery<OmMstModel>(string.Format("exec uspfn_omSlsInvLkpSlsMdlCd {0},{1},'{2}'",
                      CompanyCode, BranchCode, BPKNo))
                      .AsQueryable().toKG();
            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x => new { SalesModelCode = x.SalesModelCode, SalesModelDesc = x.SalesModelDesc })
            });
        }


        public JsonResult SlsInvLkpSlsMdlYear(OmTrSalesBPKModel mdl )
        {
            var rslt = ctx.Database.SqlQuery<MstModelYear>(string.Format("exec uspfn_omSlsInvLkpSlsMdlYear {0},{1},'{2}','{3}'",
                     CompanyCode, BranchCode, mdl.BPKNo,mdl.SalesModelCode))
                     .AsQueryable().toKG();
            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x => new { SalesModelYear = x.SalesModelYear, SalesModelDesc = x.SalesModelDesc })
            });
        }


        #endregion


        #region Om Sales Return
        public JsonResult SlsRtrnLkpBrowse()
        {
            return Json(ctx.Database.SqlQuery<omSlsRtrnBrowse>(string.Format("exec uspfn_omSlsReturnBrowse {0},{1}",
                            CompanyCode, BranchCode))
                            .AsQueryable().toKG());

        }


        public JsonResult SlsRtrnLkpInv()
        {
            return Json(ctx.Database.SqlQuery<omSlsInvcLkp>(string.Format("exec uspfn_omSlsReturnLkpInvoice {0},{1}",
                         CompanyCode, BranchCode))
                         .AsQueryable().toKG());
        }

        public JsonResult SlsRtrnLkpSlsMdlCd(string InvoiceNo)
        {
            var rslt = ctx.Database.SqlQuery<OmMstModel>(string.Format("exec uspfn_omSlsReturnLkpSlsMdlCd {0},{1},'{2}'",
                      CompanyCode, BranchCode, InvoiceNo))
                      .AsQueryable().toKG();
            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x => new { SalesModelCode = x.SalesModelCode, SalesModelDesc = x.SalesModelDesc })
            });
        }

        public JsonResult SlsRtrnLkpSlsMdlYear()
        {
            string InvoiceNo = Request["InvoiceNo"] ?? "";
            string slsmdlcd = Request["SalesModelCode"] ?? "";           

            return Json(ctx.Database.SqlQuery<omSlsModelYrLkp>
                (string.Format("exec uspfn_omSlsReturnLkpMdlYear {0},{1},'{2}','{3}'", 
                CompanyCode, BranchCode, InvoiceNo, slsmdlcd))
                .AsQueryable()
                .toKG());
        }

        public JsonResult SlsReturGridModel(string ReturnNo)
        {
            var qry = String.Format(@"SELECT a.SalesModelCode, c.SalesModelDesc, a.SalesModelYear, a.AfterDiscDPP, a.DiscExcludePPn, a.AfterDiscPPn, a.AfterDiscPPnBM, a.OthersDPP, a.OthersPPn, 
                                    a.BPKNo, b.ChassisCode, b.ChassisNo, b.Remark FROM omTrSalesReturnDetailModel a 
                                    LEFT JOIN omTrSalesReturnVIN b ON (b.CompanyCode = a.CompanyCode AND b.BranchCode = a.BranchCode AND b.ReturnNo = a.ReturnNo
                                    AND b.BPKNo = a.BPKNo) LEFT JOIN omMstModel c ON (c.CompanyCode = a.CompanyCode AND c.SalesModelCode = a.SalesModelCode) 
                                    WHERE a.CompanyCode='{0}' AND a.BranchCode='{1}' AND a.ReturnNo='{2}'", CompanyCode, BranchCode, ReturnNo);
            var dt = ctx.Database.SqlQuery<omSlsReturGridModel>(qry).AsQueryable();
            if (dt != null)
            {
                return Json(new { success = true, data = dt });
            }
            return Json(new { success = false, data = "" });
        }


        public JsonResult SlsRtrnLkpChassisNo(omTrSalesInvoiceVin mdl)
        {
            var rslt = ctx.omTrSalesInvoiceVin
                     .Where(x => x.CompanyCode == CompanyCode &&
                                x.BranchCode == BranchCode &&
                                x.InvoiceNo == mdl.InvoiceNo &&
                                x.SalesModelCode == mdl.SalesModelCode &&
                                x.SalesModelYear == mdl.SalesModelYear &&
                                x.ChassisCode == mdl.ChassisCode &&
                                x.IsReturn == false)
                               .AsQueryable().toKG();
            return Json(new
            {
                total = rslt.total,
                data = rslt.data.Select(x => new { ChassisCode = x.ChassisCode, ChassisNo = x.ChassisNo, BPKNo = x.BPKNo })
            });
        }

       
        #endregion

        string getprofitcenter()
        {

            var IsAdmin = ctx.Database.SqlQuery<bool>(string.Format("select top 1 b.IsAdmin from sysusergroup a left join SysGroup b on b.GroupId = a.GroupId where 1 = 1 and a.UserId = '{0}' and b.GroupId = a.GroupId", CurrentUser.UserId)).SingleOrDefault();
            string profitCenter = "100";
            if (!IsAdmin)
            {
                profitCenter = ctx.SysUserProfitCenters.Where(x => x.UserId == CurrentUser.UserId).FirstOrDefault().ProfitCenter;
            }

            return profitCenter;
        }

        #region Delivery Information

        public class LookUp
        {
            public string BPKNo { get; set; }
            public DateTime BPKDate { get; set; }
        }

        public JsonResult BPKNoLookUp()
        {
            var rslt = ctx.Database.SqlQuery<LookUp>(string.Format("SELECT BPKNo, BPKDate from omTrSalesBPK where CompanyCode = '{0}' AND BranchCode = '{1}' order by BPKNo ASC ", CompanyCode, BranchCode)).AsQueryable();
            return Json(rslt.toKG());

        }

        public JsonResult CustomerCode(string CustomerCode)
        {
            var rslt = ctx.GnMstCustomer.Where(a => a.CompanyCode == CompanyCode && a.CustomerCode == CustomerCode).FirstOrDefault().CustomerName;
            return Json(new { success = rslt != null, data = rslt });

        }

        public JsonResult CustomerName(string CustomerName)
        {
            var rslt = ctx.GnMstCustomer.Where(a => a.CompanyCode == CompanyCode && a.CustomerName == CustomerName).FirstOrDefault().CustomerCode;
            return Json(new { success = rslt != null, data = rslt });

        }

        public JsonResult LoadTableBPK(string FromBPKNo, string ToBPKNo, DateTime FromBPKDate, DateTime ToBPKDate,string CustomerCode)
        {

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 1800;
            cmd.CommandText = "sp_InquiryInformasiKendaraan";

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@FromBPKNo", FromBPKNo);
            cmd.Parameters.AddWithValue("@ToBPKNo", ToBPKNo);
            cmd.Parameters.AddWithValue("@FromBPKDate", FromBPKDate);
            cmd.Parameters.AddWithValue("@ToBPKDate", ToBPKDate);
            cmd.Parameters.AddWithValue("@CustomerCode", CustomerCode);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable("datTable1");
            da.Fill(dt);

            return Json(new { success = dt.Rows.Count > 0 , data = dt }, JsonRequestBehavior.AllowGet);
        }

        public class ListDataBPK
        {
            public string CompanyCode { get; set; }
            public string BranchCode { get; set; }
            public string BPKNo { get; set; }
            public string BPKDate { get; set; }
            public string SONo { get; set; }
            public string CustomerCode { get; set; }
            public string DeliveryDate { get; set; }
            
        }

        public JsonResult Save(string Data)
        {
            var msg = "";
            var BPKNo = "";
            var items = Data;
            string date;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<ListDataBPK> BPK = ser.Deserialize<List<ListDataBPK>>(Data);


            foreach (var item in BPK)
            {
                if (item.DeliveryDate != null) {
                    date = (item.DeliveryDate).Replace("-","/");
                }

                var BPKDate = ctx.OmTrSalesBPKs.Find(CompanyCode, BranchCode, item.BPKNo).BPKDate;

                if (item.DeliveryDate != " ")
                {
                    DateTime startDate = DateTime.Parse(Convert.ToString(DateTime.Now));
                    DateTime expiryDate = startDate.AddDays(-3);

                    if (Convert.ToDateTime(item.DeliveryDate).Date < Convert.ToDateTime(BPKDate).Date)
                    {
                        BPKNo = item.BPKNo + "," + BPKNo;
                        msg = BPKNo + ", Tanggal delivery tidak boleh lebih kecil dari tanggal BPK, silakan cek kembali !";
                    }
                    else if (Convert.ToDateTime(item.DeliveryDate).Date < expiryDate)
                    {
                        BPKNo = item.BPKNo + "," + BPKNo;
                        msg = BPKNo + ", Tanggal delivery tidak boleh kurang dari 3 hari dari hari ini, silakan cek kembali !";
                    }
                    else if (Convert.ToDateTime(item.DeliveryDate).Date > Convert.ToDateTime(DateTime.Now).Date)
                    {
                        BPKNo = item.BPKNo + "," + BPKNo;
                        msg = BPKNo + ", Tanggal delivery tidak boleh lebih besar dari hari ini, silakan cek kembali !";
                    }
                    else
                    {
                        SqlParameter p0 = new SqlParameter("@CompanyCode", CompanyCode);
                        SqlParameter p1 = new SqlParameter("@BranchCode", BranchCode);
                        SqlParameter p2 = new SqlParameter("@BPKNo", item.BPKNo);
                        //SqlParameter p3 = new SqlParameter("@DeliveryDate", date);
                        SqlParameter p3 = new SqlParameter("@DeliveryDate", item.DeliveryDate);
                        SqlParameter p4 = new SqlParameter("@UserId", CurrentUser.UserId);

                        Object[] oPeams = new Object[] { p0, p1, p2, p3, p4 };

                        ctx.Database.ExecuteSqlCommand("EXEC uspfn_omTrSalesBPK @CompanyCode,@BranchCode,@BPKNo,@DeliveryDate,@UserId", oPeams);
                    }
                }
            }

            if (msg == "")
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, msg = msg });
            }
        }

        #endregion
    }
}


