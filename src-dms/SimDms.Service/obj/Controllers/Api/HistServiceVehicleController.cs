using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Service.Models;

namespace SimDms.Service.Controllers.Api
{
    public class HistServiceVehicleController : BaseController
    {
        //
        // GET: /HistServiceVehicle/
        private bool isAllBranch = false;

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                ProductType = ProductType,
                LastServiceDate = DateTime.Now
            });
        }

        public JsonResult InqVehicleHistory(HistVehicle model)
        {
            #region hide
            //DataTable dt = new DataTable();
            //SqlConnection con = (SqlConnection)ctx.Database.Connection;
            //SqlCommand cmd = con.CreateCommand();
            //SqlDataAdapter da = new SqlDataAdapter(cmd);

            //cmd.CommandText = "uspfn_SvInqVehicleHistory";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@ProductType", ProductType);
            //cmd.Parameters.AddWithValue("@PoliceRegNo", model.PoliceRegNo);
            //cmd.Parameters.AddWithValue("@ServiceBookNo", model.ServiceBookNo);
            //cmd.Parameters.AddWithValue("@CustomerCode", model.CustomerCode);
            //cmd.Parameters.AddWithValue("@ChassisCode", model.ChassisCode);
            //cmd.Parameters.AddWithValue("@ChassisNo", model.ChassisNo);
            //cmd.Parameters.AddWithValue("@EngineCode", model.EngineCode);
            //cmd.Parameters.AddWithValue("@EngineNo", model.EngineNo);
            //cmd.Parameters.AddWithValue("@BasicModel", model.BasicModel);
            //cmd.Parameters.AddWithValue("@IsCheckDate", model.IsCheckDate);
            //cmd.Parameters.AddWithValue("@ServiceDate", model.LastServiceDate);
            //cmd.Parameters.AddWithValue("@IsAllBranch", model.IsAllBranch);

            //da.Fill(dt);

            //var data = (from a in dt.AsEnumerable().AsQueryable()
            //            select new HistVehicle
            //            {
            //                PoliceRegNo = a[2].ToString(),
            //                BasicModel = a[3].ToString(),
            //                TransmissionType = a[4].ToString(),
            //                Chassis = a[5].ToString(),
            //                ChassisCode = a[6].ToString(),
            //                ChassisNo = Convert.ToDecimal(a[7]),
            //                Engine = a[8].ToString(),
            //                ServiceBookNo = a[9].ToString(),
            //                ColourCode = a[10].ToString(),
            //                CustomerCode = a[11].ToString(),
            //                CustomerName = a[12].ToString(),
            //                Customer = a[13].ToString(),
            //                FakturPolisiDate = a[14].ToString() == "" ? DateTime.MinValue : Convert.ToDateTime(a[14]),
            //                LastServiceDate = a[15].ToString() == "" ? DateTime.MinValue : Convert.ToDateTime(a[15]),
            //                LastServiceOdometer = Convert.ToDecimal(a[16]),
            //                DealerCode = a[17].ToString(),
            //                DealerName = a[18].ToString(),
            //                Dealer = a[19].ToString(),
            //                Remarks = a[20].ToString(),
            //            }).Take(2900); 
            #endregion

            //var query = "exec uspfn_SvInqVehicleHistory {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}";
            //object[] parameters = { CompanyCode, BranchCode, ProductType
            //                        , string.IsNullOrEmpty(model.PoliceRegNo) ? "" : model.PoliceRegNo 
            //                        , string.IsNullOrEmpty(model.ServiceBookNo) ? "" : model.ServiceBookNo
            //                        , string.IsNullOrEmpty(model.CustomerCode) ? "" : model.CustomerCode
            //                        , string.IsNullOrEmpty(model.ChassisCode) ? "" : model.ChassisCode
            //                        , string.IsNullOrEmpty(model.ChassisNo.ToString()) ? "" : model.ChassisNo.ToString()
            //                        , string.IsNullOrEmpty(model.EngineCode) ? "" : model.EngineCode
            //                        , string.IsNullOrEmpty(model.EngineNo.ToString()) ? "" : model.EngineNo.ToString()
            //                        , string.IsNullOrEmpty(model.BasicModel) ? "" : model.BasicModel
            //                        , model.IsCheckDate == true ? 1 : 0, model.LastServiceDate, model.IsAllBranch == true ? 1 : 0};

            isAllBranch = model.IsAllBranch;

            var query = "exec uspfn_SvInqVehicleHistory @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13";
            object[] parameters = {CompanyCode,BranchCode,ProductType
                                    , string.IsNullOrEmpty(model.PoliceRegNo) ? "" : model.PoliceRegNo 
                                    , string.IsNullOrEmpty(model.ServiceBookNo) ? "" : model.ServiceBookNo
                                    , string.IsNullOrEmpty(model.CustomerCode) ? "" : model.CustomerCode
                                    , string.IsNullOrEmpty(model.ChassisCode) ? "" : model.ChassisCode
                                    , string.IsNullOrEmpty(model.ChassisNo.ToString()) ? "" : model.ChassisNo.ToString()
                                    , string.IsNullOrEmpty(model.EngineCode) ? "" : model.EngineCode
                                    , string.IsNullOrEmpty(model.EngineNo.ToString()) ? "" : model.EngineNo.ToString()
                                    , string.IsNullOrEmpty(model.BasicModel) ? "" : model.BasicModel
                                    , model.IsCheckDate == true ? 1 : 0, model.LastServiceDate, model.IsAllBranch == true ? 1 : 0};

            var data = ctx.Database.SqlQuery<HistVehicle>(query, parameters);

            return Json(data.Take(2900));
        }

        public JsonResult GetVehicleInfo(object BranchCode, object PoliceRegNo, object ChassisCode, object ChassisNo, object BasicMode, object JobOrderDate, object CustomerCode)
        {
            var query = "exec uspfn_GetVehicleInfo @p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9";
            object[] parameters = {CompanyCode, BranchCode, ProductType, PoliceRegNo, ChassisCode
                                   , ChassisNo, BasicMode, JobOrderDate, CustomerCode, isAllBranch == true ? 1 : 0};
        
            var data = ctx.Database.SqlQuery<HistVehicleDetails>(query,parameters);

            return Json(data);
        }

    }
}