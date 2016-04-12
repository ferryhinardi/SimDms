using SimDms.Common;
using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Reflection;

namespace SimDms.Service.Controllers.Api
{
    public class SpkController : BaseController
    {
        public JsonResult Default()
        {
            var user = CurrentUser; //ctx.SysUsers.FirstOrDefault();

            var data = (from p in ctx.Employees
                       where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && (new string[] { "3", "7" }).Contains(p.TitleCode) && p.PersonnelStatus == "1" &&
                            p.EmployeeID == user.UserId
                       select new
                       {
                           ForemanID = p.EmployeeID,
                           ForemanName = p.EmployeeName
                       }).FirstOrDefault();

            var admin = ctx.SysRoleUsers.Where(p => p.UserId == user.UserId && p.RoleId.ToString().ToUpper().Contains("ADMIN")).FirstOrDefault();
            var isFSCLock = false; 
            var lookUpdtl =  ctx.LookUpDtls.Find(CompanyCode, "SPK_FLAG", "LOCK_FLATE_RATE");
            if(lookUpdtl != null){
                isFSCLock = lookUpdtl.ParaValue == "1" ? true : false;
            }
            
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
                //ServiceType = 2,
                JobOrderDate = DateTime.Now,
                StartService = DateTime.Now,
                FinishService = DateTime.Now,
                //ForemanID = data != null ? data.ForemanID : "",
                //ForemanName = data != null ? data.ForemanName : "",      
                Admin = admin != null ? true : false,
                IsFSCLock = isFSCLock
            });
        }

        public JsonResult LookUpServiceEstimation()
        {
            var ShowAll = Request["ShowAll"] ?? "1";
            var EstimationNo = Request["EstimationNo"] ?? "";
            var PoliceRegNo = Request["PoliceRegNo"] ?? "";
            var CustomerName = Request["Customer"] ?? "";
            var ServiceBookNo = Request["ServiceBookNo"] ?? "";
            var records = ctx.Database.SqlQuery<LookUpTrnServiceEstimation>("exec uspfn_SvTrnServiceSelectEstimationData @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7",
                CompanyCode, BranchCode, ProductType, ShowAll == "0" ? true : false, EstimationNo, PoliceRegNo, CustomerName, ServiceBookNo).AsQueryable();

            return Json(records.toKG());
        }

        public JsonResult LookUpServiceBooking()
        {
            var ShowAll = Request["ShowAll"] ?? "1";
            var JobOrderNo = Request["BookingNo"] ?? "";
            var PoliceRegNo = Request["PoliceRegNo"] ?? "";
            var CustomerName = Request["Customer"] ?? "";
            var ServiceBookNo = Request["ServiceBookNo"] ?? "";
            var records = ctx.Database.SqlQuery<LookUpTrnServiceEstimation>("exec uspfn_SvTrnServiceSelectBookingData @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7",
                CompanyCode, BranchCode, ProductType, ShowAll == "0" ? true : false, JobOrderNo, PoliceRegNo, CustomerName, ServiceBookNo).AsQueryable();

            return Json(records.toKG());
        }

        public JsonResult LookUpServiceJobOrder()
        {
            var ShowAll = Request["ShowAll"] ?? "1";
            var JobOrderNo = Request["JobOrderNo"] ?? "";
            var PoliceRegNo = Request["PoliceRegNo"] ?? "";
            var CustomerName = Request["CustomerName"] ?? "";
            var ServiceBookNo = Request["ServiceBookNo"] ?? "";
            var uid = CurrentUser;
            //var records = ctx.Database.SqlQuery<LookUpTrnServiceEstimation>("exec uspfn_SvTrnServiceSelectBookingData @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7",
            //   CompanyCode, BranchCode, ProductType, ShowAll == "0" ? true : false, JobOrderNo, PoliceRegNo, CustomerName, ServiceBookNo).AsQueryable();
            if (JobOrderNo == "undefined") JobOrderNo = "";
            var records = ctx.JobOrderViews
                .Where(p => p.CompanyCode == uid.CompanyCode
                && p.BranchCode == uid.BranchCode && p.ServiceType == "2"
                && new string[] { "0", "1", "2", "3", "4", "5" }
                .Contains(p.ServiceStatus));
                //.OrderByDescending(p => p.ServiceNo);

            if (ShowAll == "0")
            {
                records = ctx.JobOrderViews
                .Where(p => p.CompanyCode == uid.CompanyCode
                && p.BranchCode == uid.BranchCode && p.ServiceType == "2");
                //.OrderByDescending(p => p.ServiceNo);
            }

            if ((!string.IsNullOrEmpty(JobOrderNo)))
            {
                records = records.Where(p => p.JobOrderNo.Contains(JobOrderNo));
            }

            if ((!string.IsNullOrEmpty(PoliceRegNo)))
            {
                records = records.Where(p => p.PoliceRegNo.Contains(PoliceRegNo));
            }

            if ((!string.IsNullOrEmpty(CustomerName)))
            {
                records = records.Where(p => p.Customer.Contains(CustomerName));
            }

            if ((!string.IsNullOrEmpty(ServiceBookNo)))
            {
                records = records.Where(p => p.ServiceBookNo.Contains(ServiceBookNo));
            }
            records.OrderByDescending(p => p.ServiceNo);
            return Json(records.toKG());
        }

        public JsonResult LookUpCustomerVehicles()
        {
            var dicParams = new Dictionary<string, string>();
            foreach(PropertyInfo prop in Request.Params){
                dicParams.Add(prop.Name, prop.GetValue(prop, null).ToString());
            }
            var x = dicParams;
            
            //string BatchNo = Request["VinNo"].To;
            //string ReceiptNo = Request["PoliceRegNo"];
            //string FPJNo = Request["CustomerName"];
            //string FPJGovNo = Request["BasicModel"];
            //string FPJGovNo = Request["TransmissionType"];

            var uid = CurrentUser;
            var records = ctx.CustomerVehicleViews.Where(p => p.CompanyCode == uid.CompanyCode &&
                p.BranchCode == uid.BranchCode).AsQueryable();
            
            return Json(records.toKG());
        }

        public JsonResult TaskPartList()
        {
            var qry = from p in ctx.JobOrderViews
                      select new
                      {
                          ItemType = p.BasicModel,
                          BillTo = p.BranchCode,
                          TaskPart = p.JobOrderNo,
                          TaskPartQty = p.JobOrderDate,
                          QtyAvail = p.JobType,
                          Discount = p.MechanicName,
                          NetPrice = p.ServiceStatusDesc,
                          p.JobOrderNo
                      };
            return Json(qry.Take(5));
        }

        public JsonResult Get(JobOrder model)
        {
            if (model.JobOrderNo != null)
            {
                var jobOrderTemp = ctx.JobOrderViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                      && x.BranchCode == BranchCode
                      && x.ProductType == ProductType
                      && (model.ServiceType == "0" ? x.EstimationNo : (model.ServiceType == "1" ? x.BookingNo : x.JobOrderNo)) == model.JobOrderNo);

                if (jobOrderTemp != null){
                    model.ServiceNo = jobOrderTemp.ServiceNo;
                    model.JobOrderNo = jobOrderTemp.JobOrderNo;
                    model.EstimationNo = jobOrderTemp.EstimationNo;
                    model.BookingNo = jobOrderTemp.BookingNo;
                }
                    
                else return Json(new { success = false });
            }

            var jobOrder = ctx.JobOrderViews.FirstOrDefault(x => x.CompanyCode == CompanyCode
                      && x.BranchCode == BranchCode
                      && x.ProductType == ProductType
                      && x.ServiceNo == model.ServiceNo);

            if (jobOrder == null) return Json(new { success = false, message = "data not found" });

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = "uspfn_SvTrnInvoiceDraft";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@JobOrderNo", (jobOrder.ServiceType == "0" ? jobOrder.EstimationNo :
                jobOrder.ServiceType == "1" ? jobOrder.BookingNo : jobOrder.JobOrderNo));
            SqlDataAdapter daHdr = new SqlDataAdapter(cmd);
            DataTable dtHdr = new DataTable();
            daHdr.Fill(dtHdr);
            dtHdr.Rows[0]["TotalSrvAmt"] = (jobOrder.TotalSrvAmount == null) ? 0 : jobOrder.TotalSrvAmount.Value;

            cmd.CommandText = "uspfn_SvTrnServiceSelectDtl";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", dtHdr.Rows[0]["ProductType"]);
            cmd.Parameters.AddWithValue("@ServiceNo", dtHdr.Rows[0]["ServiceNo"]);
            SqlDataAdapter daDtl = new SqlDataAdapter(cmd);
            DataTable dtDtl = new DataTable();
            daDtl.Fill(dtDtl);

            var amountDesc = "";
            if (dtHdr.Rows[0]["JobType"].ToString().StartsWith("FSC") || dtHdr.Rows[0]["JobType"].ToString().StartsWith("PDI") || dtHdr.Rows[0]["JobType"].ToString().StartsWith("CLAIM")
                || dtHdr.Rows[0]["JobType"].ToString().StartsWith("REWORK"))
            {
                cmd.CommandText = "uspfn_SvTrnServiceBill";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("@ProductType", dtHdr.Rows[0]["ProductType"]);
                cmd.Parameters.AddWithValue("@ServiceNo", dtHdr.Rows[0]["ServiceNo"]);
                cmd.Parameters.AddWithValue("@BillType", "C");
                SqlDataAdapter daSrv = new SqlDataAdapter(cmd);
                DataTable dtSrv = new DataTable();
                daSrv.Fill(dtSrv);

                dtHdr.Rows[0]["TotalSrvAmt"] = dtSrv.Rows[0]["CustTotalSrvAmt"];
                amountDesc = dtSrv.Rows[0]["AmtDesc"].ToString();


                if (dtHdr.Rows[0]["JobType"].ToString().StartsWith("CLAIM")) dtHdr.Rows[0]["TotalSrvAmt"] = 0;
            }          
            
            var header = GetJson(dtHdr)[0];
            header.Add("amountDesc", amountDesc);

            var detail = GetJson(dtDtl);
            var countDetail = GetJson(dtDtl).Count;

            var spkAdmin = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "SPKADMIN");

            var lockBill = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "LOCK_BILL");

            var lockInsu = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "LOCK_INSU");

            var lockQtyNK = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "LOCK_QTYNK");

            var lockPrice = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "LOCK_PRICE");


            return Json(new { success = true, data = header, list = detail, detailList = countDetail, spkFlagLockBill = lockBill, spkFlagLockInsu = lockInsu, 
                spkFlagLockQtyNK = lockQtyNK, spkAdmin = spkAdmin, lockPrice = lockPrice });
        }

        public JsonResult EditDetail(svMstJob model)
        {
            var job = ctx.svMstJobs.Where(p => p.CompanyCode == CompanyCode && p.BasicModel == model.BasicModel && p.JobType == model.JobType).FirstOrDefault();

            var spkAdmin = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "SPKADMIN");

            var lockBill = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "LOCK_BILL");

            var lockInsu = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "LOCK_INSU");

            var lockPrice = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "LOCK_PRICE");

            var lockQtyNK = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "SPK_FLAG" && x.LookUpValue == "LOCK_QTYNK");

            var paketSrv = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "PAKET SRV" && x.LookUpValue == model.JobType);

            return Json(new { success = true, job = job, spkAdmin = spkAdmin, lockBill = lockBill, lockInsu = lockInsu, lockPrice = lockPrice, lockQtyNK = lockQtyNK, paketSrv = paketSrv });
        }

        public JsonResult GetPaketSrv(string jobType)
        {
            var paketSrv = ctx.LookUpDtls.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.CodeID == "PAKET SRV" && x.LookUpValue == jobType);

            return Json(new { success = true, paketSrv = paketSrv });

        }

        public JsonResult ClaimList(JobOrder model)
        {         
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                        
            cmd.CommandText = "uspfn_SvTrnSrvAddList";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@JobOrderNo", model.JobOrderNo);
            DataTable dtClm = new DataTable();
            SqlDataAdapter daClm = new SqlDataAdapter(cmd);
            daClm.Fill(dtClm);
            var claim = GetJson(dtClm);

            return Json(new { success = true, claimList = claim });
        }

        public JsonResult EditDiscPart(TrnService model)
        {
            var record = ctx.Services.Find(CompanyCode, BranchCode, ProductType, model.ServiceNo);         

            if (record != null || model.ServiceNo > 1)
            {
                if (record.LaborDiscPct != model.LaborDiscPct || record.PartDiscPct != model.PartDiscPct || record.MaterialDiscPct != model.MaterialDiscPct)
                    return Json(new { data = "Apakah perubahan discount ini akan diupdate ke detail?" });
                else return Json(new { data = "" });
            }
            else
            {
                return Json(new { data = "" });
            }
        }

        public JsonResult CalculateTotal()
        {
            decimal decOprHourDemandQty = Convert.ToDecimal(Request["OprHourDemandQty"]);
            decimal decPrice = Convert.ToDecimal(Request["Price"]);
            decimal decDiscPct = Convert.ToDecimal(Request["DiscPct"]);

            decimal decPriceNet = decOprHourDemandQty * decPrice * (100 - decDiscPct) / 100;
            return Json(new { success = true, PriceNet = decPriceNet });
        }

        public JsonResult CalculateTotalNew(double OprHourDemandQty, double Price, double DiscPct)
        {
            double v3 = OprHourDemandQty * Price;
            string decPriceNet = Math.Floor((v3 * (100.0 - DiscPct) * 0.01)).ToString("n0");
            return Json(new { success = true, PriceNet = decPriceNet });
        }

        public JsonResult Save(TrnService model, bool bOverrideDisc, decimal odometer)
        {
            Helpers.ReplaceNullable(model);
            var JobOrderNo = model.JobOrderNo == "" ? null : model.JobOrderNo;
            var record = ctx.Services.Where(p => (p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType) && 
                                            (p.JobOrderNo == JobOrderNo||p.EstimationNo==JobOrderNo || p.BookingNo==JobOrderNo)).FirstOrDefault();
            var userId = CurrentUser.UserId;
            var currDate = model.JobOrderDate.Value; 
            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            using (TransactionScope trans = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30)))
            {
                if (record == null && model.ServiceNo < 1)
                //if (record == null)
                {
                    long serviceNo = 1;
                    var oServiceNo = ctx.Services.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).FirstOrDefault();
                    if (oServiceNo != null)
                        serviceNo = ctx.Services.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode).Select(p => p.ServiceNo).Max() + 1;

                    var docNo = ctx.Database.SqlQuery<string>("exec uspfn_GnDocumentGetNew @p0, @p1, @p2, @p3, @p4",
                        CompanyCode, BranchCode,
                        model.ServiceType == "0" ? "EST" : model.ServiceType == "1" ? "BOK" : "SPK",
                        userId, currDate).FirstOrDefault();

                    string temp = "0";
                    string tempChassisNo = (model.ChassisNo.Value.ToString().Length > 6) ? model.ChassisNo.Value.ToString().Substring(model.ChassisNo.Value.ToString().Length - 6, 6) : model.ChassisNo.Value.ToString().Substring(0, model.ChassisNo.Value.ToString().Length);
                    if (tempChassisNo.Length < 6)
                    {
                        for (int i = 0; i < 6 - tempChassisNo.Length; i++)
                        {
                            temp = temp + "0";
                        }
                        tempChassisNo = temp + tempChassisNo;
                    }
                    string vin = model.ChassisCode.Substring(0, 11 - (11 - model.ChassisCode.Length)).Trim() + tempChassisNo;
                    model.VIN = vin;

                    record = new TrnService
                    {
                        CompanyCode = CompanyCode,
                        BranchCode = BranchCode,
                        ProductType = ProductType,
                        ServiceNo = serviceNo,
                        ServiceType = model.ServiceType,
                        CustomerCode = model.CustomerCode,
                        CustomerCodeBill = model.CustomerCodeBill,
                        BasicModel = model.BasicModel,
                        TransmissionType = model.TransmissionType,
                        VIN = model.VIN,
                        JobType = model.JobType,
                        InsurancePayFlag = model.InsurancePayFlag,
                        ServiceStatus = "0",
                        PrintSeq = 0,
                        PPNPct = 0,
                        PPHPct = 0,
                        LaborDiscAmt = 0,
                        LaborGrossAmt = 0,
                        LaborDppAmt = 0,
                        PartsDiscAmt = 0,
                        PartsGrossAmt = 0,
                        PartsDppAmt = 0,
                        MaterialDiscAmt = 0,
                        MaterialGrossAmt = 0,
                        MaterialDppAmt = 0,
                        IsLocked = false,
                        CreatedBy = userId,
                        CreatedDate = currDate
                    };
                    if (model.ServiceType == "0")
                    {
                        record.EstimationNo = docNo;
                        record.EstimationDate = currDate;
                    }
                    else if (model.ServiceType == "1")
                    {
                        record.BookingNo = docNo;
                        record.BookingDate = currDate;
                    }
                    else if (model.ServiceType == "2")
                    {
                        record.JobOrderNo = docNo;
                        record.JobOrderDate = currDate;
                    }
                    ctx.Services.Add(record);
                }
                //else
                //{
                    if (bOverrideDisc)
                    {
                        ctx.Database.ExecuteSqlCommand("exec uspfn_SvTrnServiceOverrideDiscount @p0,@p1,@p2", CompanyCode, BranchCode, record.ServiceNo);
                        //cmd.CommandText = "uspfn_SvTrnServiceOverrideDiscount";
                        //cmd.CommandType = CommandType.StoredProcedure;
                        //cmd.Parameters.Clear();
                        //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                        //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                        //cmd.Parameters.AddWithValue("@ServiceNo", record.ServiceNo);

                        ////cmd.Connection.Open();
                        //cmd.ExecuteNonQuery();
                    }
                //}

                record.PoliceRegNo = model.PoliceRegNo;
                record.BasicModel = model.BasicModel;
                record.ChassisCode = model.ChassisCode;
                record.ChassisNo = model.ChassisNo;
                record.VIN = model.ChassisCode + model.ChassisNo.ToString();
                record.EngineCode = model.EngineCode;
                record.EngineNo = model.EngineNo;
                record.ColorCode = model.ColorCode;
                record.TransmissionType = model.TransmissionType;
                record.Odometer = odometer;

                record.InsurancePayFlag = model.InsurancePayFlag;
                record.InsuranceOwnRisk = model.InsuranceOwnRisk;
                record.InsuranceJobOrderNo = model.InsuranceJobOrderNo;
                record.InsuranceNo = model.InsuranceNo;

                record.CustomerCode = model.CustomerCode;
                record.CustomerCodeBill = model.CustomerCodeBill;

                record.LaborDiscPct = Convert.ToDecimal(model.LaborDiscPct);
                record.PartDiscPct = model.PartDiscPct;
                record.MaterialDiscPct = model.MaterialDiscPct;

                record.ServiceRequestDesc = model.ServiceRequestDesc;
                record.JobType = model.JobType;
                record.ConfirmChangingPart = model.ConfirmChangingPart;
                record.ForemanID = model.ForemanID;
                record.MechanicID = model.MechanicID;
                record.EstimateFinishDate = model.EstimateFinishDate;
                record.IsSparepartClaim = model.IsSparepartClaim;
                record.ServiceBookNo = model.ServiceBookNo ?? "-";

                record.LastUpdateBy = userId;
                record.LastUpdateDate = currDate;
                try
                {
                    Helpers.ReplaceNullable(record);
                    ctx.SaveChanges();

                    if (bOverrideDisc)
                    {
                        ctx.Database.ExecuteSqlCommand("exec uspfn_SvTrnServiceOverrideDiscount @p0,@p1,@p2", CompanyCode, BranchCode, record.ServiceNo);
                    }

                    ctx.Database.ExecuteSqlCommand("exec uspfn_SvTrnServiceInsertDefaultTaskNew @p0,@p1,@p2,@p3,@p4,@p5", CompanyCode, BranchCode, ProductType, record.ServiceNo, record.ServiceBookNo, userId);
                                       
                    //ctx.Database.ExecuteSqlCommand("exec uspfn_SvInsertDefaultTaskMovement @p0,@p1,@p2,@p3,@p4", CompanyCode, BranchCode, ProductType, record.ServiceNo, userId);
                        
                    //cmd.CommandText = "uspfn_SvTrnServiceInsertDefaultTask";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Clear();
                    //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                    //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                    //cmd.Parameters.AddWithValue("@ProductType", ProductType);
                    //cmd.Parameters.AddWithValue("@ServiceNo", record.ServiceNo);
                    //cmd.Parameters.AddWithValue("@ServiceBookNo", record.ServiceBookNo);
                    ////cmd.Connection.Open();
                    //cmd.ExecuteNonQuery();

                    ctx.Database.ExecuteSqlCommand("exec uspfn_SvTrnServiceReCalculate @p0,@p1,@p2,@p3", CompanyCode, BranchCode, ProductType, record.ServiceNo);
                    //cmd.CommandText = "uspfn_SvTrnServiceReCalculate";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Clear();
                    //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                    //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                    //cmd.Parameters.AddWithValue("@ProductType", ProductType);
                    //cmd.Parameters.AddWithValue("@ServiceNo", record.ServiceNo);
                    ////cmd.Connection.Open();
                    //cmd.ExecuteNonQuery();

                    ctx.Database.ExecuteSqlCommand("exec uspfn_GnChangeCustStatus @p0,@p1,@p2,@p3", CompanyCode, model.CustomerCode, "SPK", CurrentUser.UserId);
                    
                    //cmd.CommandText = "uspfn_GnChangeCustStatus";
                    //cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.Clear();
                    //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                    //cmd.Parameters.AddWithValue("@CustomerCode", model.CustomerCode);
                    //cmd.Parameters.AddWithValue("@FuncCode", "SPK");
                    //cmd.Parameters.AddWithValue("@UserId", CurrentUser.UserId);

                    ////cmd.Connection.Open();
                    //cmd.ExecuteNonQuery();
                    //cmd.Connection.Close();
                    trans.Complete();
                    return Json(new { success = true, data = record });
                }
                catch (Exception ex)
                {
                    trans.Dispose();
                    return Json(new { success = false, data = record, message = ex.Message });
                }
            };
        }

        public JsonResult SaveDetail()
        {
            //Insert Into spMstItems, spMstItemLoc, spMstItemPrice 
            string PartNo = Request["TaskPart"] != null ? Request["TaskPart"].ToString() : ""; 
            InsertItemsLocPriceFromMD(PartNo);
            
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvTrnServiceSaveDtlWeb";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@ServiceNo", Request["ServiceNo"]);
            cmd.Parameters.AddWithValue("@BillType", Request["BillType"]);
            cmd.Parameters.AddWithValue("@ItemType", Request["ItemType"]);
            cmd.Parameters.AddWithValue("@TaskPart", Request["TaskPart"]);
            cmd.Parameters.AddWithValue("@HourQty", Convert.ToDecimal(Request["HourQty"]));
            cmd.Parameters.AddWithValue("@PartSeq", Convert.ToDecimal(Request["PartSeq"]));
            cmd.Parameters.AddWithValue("@UserPrice", true);
            cmd.Parameters.AddWithValue("@TaskPrice", Convert.ToDecimal(Request["TaskPrice"]));
            cmd.Parameters.AddWithValue("@DiscPct", Convert.ToDecimal(Request["DiscPct"]));
            cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserId);
            
            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        public JsonResult DeleteDetail()
        {
            //validate delete
            string typeofgoods = Request["TaskPartType"];
            string taskpartno = Request["TaskPartNo"];
            string partseq = Request["PartSeq"];
            string serviceno = Request["ServiceNo"];
            string joborderno = Request["JobOrderNo"];
            

            string sql = string.Format("uspfn_SvTrnServiceSelectDtl '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, ProductType, serviceno);
            var tskdtl = ctx.Database.SqlQuery<TrxSrvTaskDtl>(sql).ToList();

            if (typeofgoods == "L")
            {                
                var srvtsk = ctx.ServiceTasks.Find(BranchCode, BranchCode, ProductType, long.Parse(serviceno), taskpartno);

                if (srvtsk != null)
                {
                    if (srvtsk.IsSubCon??false)
                    {
                        if (srvtsk.PONo != "")
                        {
                            return Json(new { success = false, message = string.Format("Pekerjaan {0} tidak dapat dihapus karena telah diproses pesanan atau penerimaan pekerjaan luar", taskpartno) });
                        }
                    }
                }

                
                if (tskdtl.Where(x => x.TypeOfGoods == "L").Count() <= 1 && tskdtl.Where(x => x.SupplySlipNo != "").Count() > 0)
                {
                    return Json(new { success = false, message = string.Format("Pekerjaan {0} tidak dapat dihapus karena detail Part telah mempunyai No Supply Slip", taskpartno) });
                }

                var lg= long.Parse(serviceno);
                var mknk = ctx.Mechanics.Where(x => x.CompanyCode == CompanyCode &&
                                                   x.BranchCode == BranchCode &&
                                                   x.ProductType == ProductType &&
                                                   x.ServiceNo == lg &&
                                                   x.OperationNo == taskpartno)
                                                   .FirstOrDefault();


                                                   //Find(CompanyCode, BranchCode, ProductType, serviceno, taskpartno);
                if (mknk != null)
                {

                    return Json(new { success = false, message = string.Format("Pekerjaan {0} sudah dialokasikan. Check Input Alokasi Mekanik", taskpartno) });
                }

                        
            }
            else
            {
                //var row = tskdtl.Where(x => x.SeqNo.ToString() == partseq && x.TaskPartNo == taskpartno).SingleOrDefault();

                //if (!string.IsNullOrEmpty(row.SupplySlipNo))
                //{
                //    return Json(new { success = false, message = string.Format("Pekerjaan {0} tidak dapat dihapus karena detail Part telah mempunyai No Supply Slip", taskpartno) });
                //}
            }




            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvTrnServiceDeleteDtl";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@ServiceNo", Request["ServiceNo"]);
            cmd.Parameters.AddWithValue("@TaskPartType", Request["TaskPartType"]);
            cmd.Parameters.AddWithValue("@TaskPartNo", Request["TaskPartNo"]);
            cmd.Parameters.AddWithValue("@PartSeq", Request["PartSeq"]);

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                cmd.Connection.Dispose();

                //var recSDMovement = ctxMD.SvSDMovements.Find(CompanyCode, BranchCode, joborderno, taskpartno, Convert.ToInt16(partseq));
                //if (recSDMovement != null)
                //{
                //    ctxMD.SvSDMovements.Remove(recSDMovement);
                //    ctxMD.SaveChanges();
                //}

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult DeleteInvoice()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvTrnSrvAddDelete";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            cmd.Parameters.AddWithValue("@JobOrderNo", Request["JobOrderNo"]);
            cmd.Parameters.AddWithValue("@InvoiceNo", Request["InvoiceNo"]);           

            try
            {
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                cmd.Connection.Dispose();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult CreateSPK(long serviceNo)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand("exec uspfn_SvTrnJobOrderCreate @p0, @p1, @p2, @p3, @p4",
                    CompanyCode, BranchCode, ProductType, serviceNo, CurrentUser.UserId);

                ctx.Database.ExecuteSqlCommand("exec uspfn_SvInsertDefaultTaskMovement @p0,@p1,@p2,@p3,@p4", CompanyCode, BranchCode, ProductType, serviceNo, CurrentUser.UserId);

                var JobOrderNo = ctx.Services.Find(CompanyCode, BranchCode, ProductType, serviceNo).JobOrderNo;

                return Json(new { success = true , joborderno = JobOrderNo, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult CancelSPK(long serviceNo)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec uspfn_SvTrnJobOrderCancel @p0, @p1, @p2, @p3, @p4",
                    CompanyCode, BranchCode, ProductType, serviceNo, CurrentUser.UserId);
                
                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult CloseSPK(long serviceNo)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec uspfn_SvTrnJobOrderClose @p0, @p1, @p2, @p3, @p4",
                    CompanyCode, BranchCode, ProductType, serviceNo, CurrentUser.UserId);

                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult OpenSPK(long serviceNo)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec uspfn_SvTrnJobOrderOpen @p0, @p1, @p2, @p3, @p4",
                    CompanyCode, BranchCode, ProductType, serviceNo, CurrentUser.UserId);

                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult ValidateOdometer(decimal Odometer, TrnService model)
        {            
            var record = ctx.Services.Find(CompanyCode, BranchCode, ProductType, model.ServiceNo);

            string message = string.Empty;
            if (record == null)
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "uspfn_SvTrnServiceOutstandingKM";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("@ProductType", ProductType);
                cmd.Parameters.AddWithValue("@PoliceRegNo", model.PoliceRegNo);
                cmd.Parameters.AddWithValue("@KMVehicle", Odometer);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    message = GetJson(dt)[0]["MessageInfo"].ToString();
                }
            }

            if (message != string.Empty)
                return Json(new { success = false, data = message });
            else
                return Json(new { success = true });
        }

        public JsonResult ValidateInsertSPK(TrnService model)
        {
            var record = ctx.Services.Find(CompanyCode, BranchCode, ProductType, model.ServiceNo);
            bool confirm = true;
            string message = string.Empty;
            if (record == null)
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = "uspfn_SvTrnServiceOutstandingWeb";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@OutType", "FSC");
                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                cmd.Parameters.AddWithValue("@ProductType", ProductType);
                cmd.Parameters.AddWithValue("@PoliceRegNo", model.PoliceRegNo);
                cmd.Parameters.AddWithValue("@JobType", model.JobType);
                cmd.Parameters.AddWithValue("@ChassisCode", model.ChassisCode);
                cmd.Parameters.AddWithValue("@ChassisNo", model.ChassisNo);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
               
                if (dt.Rows.Count > 0)
                {
                    message = GetJson(dt)[0]["MessageInfo"].ToString();
                    confirm = false;
                }
                else
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@OutType", "OUT");
                    cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                    cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                    cmd.Parameters.AddWithValue("@ProductType", ProductType);
                    cmd.Parameters.AddWithValue("@PoliceRegNo", model.PoliceRegNo);
                    cmd.Parameters.AddWithValue("@JobType", "");
                    cmd.Parameters.AddWithValue("@ChassisCode", model.ChassisCode);
                    cmd.Parameters.AddWithValue("@ChassisNo", model.ChassisNo);

                    da = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        message = GetJson(dt)[0]["MessageInfo"].ToString();
                    }
                    else
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@OutType", "BOK");
                        cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                        cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                        cmd.Parameters.AddWithValue("@ProductType", ProductType);
                        cmd.Parameters.AddWithValue("@PoliceRegNo", model.PoliceRegNo);
                        cmd.Parameters.AddWithValue("@JobType", "");
                        cmd.Parameters.AddWithValue("@ChassisCode", model.ChassisCode);
                        cmd.Parameters.AddWithValue("@ChassisNo", model.ChassisNo);

                        da = new SqlDataAdapter(cmd);
                        dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            message = GetJson(dt)[0]["MessageInfo"].ToString();
                        }
                        else
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@OutType", "EST");
                            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                            cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                            cmd.Parameters.AddWithValue("@ProductType", ProductType);
                            cmd.Parameters.AddWithValue("@PoliceRegNo", model.PoliceRegNo);
                            cmd.Parameters.AddWithValue("@JobType", "");
                            cmd.Parameters.AddWithValue("@ChassisCode", model.ChassisCode);
                            cmd.Parameters.AddWithValue("@ChassisNo", model.ChassisNo);

                            da = new SqlDataAdapter(cmd);
                            dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                message = GetJson(dt)[0]["MessageInfo"].ToString();
                            }
                            else
                            {
                                cmd.CommandText = "uspfn_SvTrnServiceValidation";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                                cmd.Parameters.AddWithValue("@JobOrderNo", "");
                                cmd.Parameters.AddWithValue("@ChassisCode", model.ChassisCode);
                                cmd.Parameters.AddWithValue("@ChassisNo", model.ChassisNo);
                                cmd.Parameters.AddWithValue("@ProductType", ProductType);
                                cmd.Parameters.AddWithValue("@BasicModel", model.BasicModel == null ? "" : model.BasicModel);
                                cmd.Parameters.AddWithValue("@JobType", model.JobType);
                                cmd.Parameters.AddWithValue("@OperationNo", "");

                                da = new SqlDataAdapter(cmd);
                                dt = new DataTable();
                                da.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    message = GetJson(dt)[0]["MessageInfo"].ToString();
                                }
                            }
                        }
                    }
                }
            }

            if (message != string.Empty)
                return Json(new { success = false, data = message, confirm = confirm });
            else  
                return Json(new { success = true });
        }

        public JsonResult ServiceValidation(TrnService model)
        {
            string message = string.Empty;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvTrnServiceValidation";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@JobOrderNo", model.JobOrderNo ?? "");
            cmd.Parameters.AddWithValue("@ChassisCode", model.ChassisCode);
            cmd.Parameters.AddWithValue("@ChassisNo", model.ChassisNo);
            cmd.Parameters.AddWithValue("@ProductType", ProductType);
            cmd.Parameters.AddWithValue("@BasicModel", model.BasicModel);
            cmd.Parameters.AddWithValue("@JobType", model.JobType);
            cmd.Parameters.AddWithValue("@OperationNo", "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                message = GetJson(dt)[0]["MessageInfo"].ToString();
            }
            if (message != string.Empty)
                return Json(new { success = false, message = message });
            else
                return Json(new { success = true });
        }

        public JsonResult ListDiscountService(TrnService model)
        {
            object[] parameters = { CompanyCode, BranchCode, model.CustomerCodeBill, ((model.ChassisCode == null) ? "" : model.ChassisCode), ((model.ChassisNo == null) ? 0 : model.ChassisNo.Value), DateTime.Now, ((model.JobType == null) ? "" : model.JobType) };
            var query = "exec uspfn_SvListDicount @p0,@p1,@p2,@p3,@p4,@p5,@p6";

            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandText = "uspfn_SvListDicount";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            //cmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            //cmd.Parameters.AddWithValue("@CustomerCode", model.CustomerCodeBill);
            //cmd.Parameters.AddWithValue("@ChassisCode", (model.ChassisCode == null) ? "" : model.ChassisCode);
            //cmd.Parameters.AddWithValue("@ChassisNo", (model.ChassisNo == null) ? 0 : model.ChassisNo.Value);
            //cmd.Parameters.AddWithValue("@TransPeriod", DateTime.Now);
            //cmd.Parameters.AddWithValue("@JobType", (model.JobType == null) ? "" : model.JobType);

            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);

            var listDisc = ctx.Database.SqlQuery<DiscSPK>(query, parameters);
            var dataDisc = new DiscSPK();

            foreach (var disc in listDisc)
            {
                if (disc.SeqNo == 2)
                {
                    dataDisc = disc;
                }
                else if (disc.SeqNo == 3)
                {
                    dataDisc = disc;
                }
                else
                {
                    dataDisc = disc;
                }
            }

            //var dataDisc = GetJson(dt)[0];
            //var listDisc = GetJson(dt);
            return Json(new { success = true, data = dataDisc, count = listDisc.Count(), list = listDisc });
        }

        public JsonResult UpdateNewPrice(long serviceNo, string operationNo)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec uspfn_SvTrnReUpdTaskPrice @p0, @p1, @p2, @p3, @p4",
                    CompanyCode, BranchCode, ProductType, serviceNo, operationNo);

                return Json(new { success = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, Message = ex.Message });
            }
        }

        public JsonResult SaveInvoice(string jobOrderNo, string invoiceNo)
        {
            try
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec uspfn_SvTrnSrvAddSave @p0, @p1, @p2, @p3, @p4",
                    CompanyCode, BranchCode, jobOrderNo, invoiceNo, CurrentUser.UserId);

                return Json(new { Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { Message = ex.Message });
            }
        }

        public JsonResult ValidatePackage(TrnService model, string taskPartNo, string itemType)
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_SvCheckPackage";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            cmd.Parameters.AddWithValue("@ChassisCode", model.ChassisCode);
            cmd.Parameters.AddWithValue("@ChassisNo", model.ChassisNo);
            cmd.Parameters.AddWithValue("@JobType", model.JobType);
            cmd.Parameters.AddWithValue("@TransDate", model.JobOrderDate);
            cmd.Parameters.AddWithValue("@TaskPartNo", taskPartNo);
            cmd.Parameters.AddWithValue("@ItemType", itemType);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(new { success = true, data = dt.Rows.Count > 0 ? GetJson(dt)[0] : null });
        }

        public JsonResult IsEnableCloseSPK(TrnService model)
        {           
            var oRecord = ctx.Services.Find(CompanyCode, BranchCode, ProductType, model.ServiceNo);
            string query = string.Empty;

            // Syarat 1 : harus ada spk yang akan di close
            if (oRecord == null) return Json(new { enabled = false });
            // Syarat 2 : service type = "2"
            if (oRecord.ServiceType != "2") return Json(new { enabled = false });
            // Syarat 3 : spk dengan status 0, 1, 2, 3, 4
            if (string.Compare(oRecord.ServiceStatus, "5") > 0) return Json(new { enabled = false });          
            
            // check task sub con / no sub con
            var dtSubCon = (from p in ctx.ServiceTasks
                           where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType && p.ServiceNo == model.ServiceNo && p.IsSubCon == true
                           select new 
                           {
                               CompanyCode = p.CompanyCode,
                               BranchCode = p.BranchCode,
                               ProductType = p.ProductType,
                               ServiceNo = p.ServiceNo,
                               OperationNo = p.OperationNo
                           }).ToList();
            var dtTask = (from p in ctx.ServiceTasks
                          where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType && p.ServiceNo == model.ServiceNo && p.IsSubCon == false
                          select new
                          {
                              CompanyCode = p.CompanyCode,
                              BranchCode = p.BranchCode,
                              ProductType = p.ProductType,
                              ServiceNo = p.ServiceNo,
                              OperationNo = p.OperationNo
                          }).ToList();

            bool isSubCon = dtSubCon.Count() > 0;

            //var subCon = GetJson((DataTable)dtSubCon);
            // Syarat 4 : jika sub con
            if (dtSubCon.Count() > 0)
            {
                // SPK sudah diproses untuk Pesanan dan Penerimaan Pekerjaan Luar
                foreach (var row in dtSubCon)
                {
                    var oSvTrnSrvTask = ctx.ServiceTasks.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType && p.ServiceNo == row.ServiceNo && p.OperationNo == row.OperationNo).FirstOrDefault();
                    // Syarat 4.1 : sub con task harus terdaftar di SvTrnSrvTask
                    if (oSvTrnSrvTask == null) return Json(new { enabled = false });

                    var oSvTrnPoSubCon = ctx.SubCons.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType && p.PONo == oSvTrnSrvTask.PONo).FirstOrDefault();
                    // Syarat 4.2 : sub con task harus sudah di proses di penerimaan sub con
                    if (oSvTrnPoSubCon == null || oSvTrnPoSubCon.POStatus != "5") return Json(new { enabled = false });
                }
            }

            // Syarat 5 : jika buka sub con
            if (dtTask.Count() > 0)
            {
                query = string.Format(@"select distinct srvMechanic.OperationNo 
                from svTrnSrvMechanic srvMechanic with (nolock, nowait)
                inner join svTrnSrvTask srvTask  with (nolock, nowait) on
	                srvTask.CompanyCode = srvMechanic.CompanyCode and
	                srvTask.BranchCode = srvMechanic.BranchCode and
	                srvTask.ServiceNo = srvMechanic.ServiceNo and
	                srvTask.OperationNo = srvMechanic.OperationNo and
                    srvTask.IsSubCon = 0
                where                     
                    srvMechanic.CompanyCode = '{0}' and
                    srvMechanic.BranchCode = '{1}' and
                    srvMechanic.ProductType = '{2}' and
                    srvMechanic.ServiceNo = {3}", CompanyCode, BranchCode, ProductType, model.ServiceNo);

                var dtMechanic = ctx.Database.SqlQuery<string>(query).ToList();
                // syarat 5.1 : semua task harus ada mekanik nya
                if (dtMechanic.Count != dtTask.Count()) return Json(new { enabled = false });

                // syarat 5.2 : semua task mekanik statusnya harus closed
                query = string.Format(@"select srvMechanic.OperationNo 
                from svTrnSrvMechanic srvMechanic with (nolock, nowait)
                inner join svTrnSrvTask srvTask  with (nolock, nowait) on
	                srvTask.CompanyCode = srvMechanic.CompanyCode and
	                srvTask.BranchCode = srvMechanic.BranchCode and
	                srvTask.ServiceNo = srvMechanic.ServiceNo and
	                srvTask.OperationNo = srvMechanic.OperationNo
                where                     
                    srvMechanic.CompanyCode = '{0}' and
                    srvMechanic.BranchCode = '{1}' and
                    srvMechanic.ProductType = '{2}' and
                    srvMechanic.ServiceNo = {3} and
                    srvMechanic.MechanicStatus not in ('2')", CompanyCode, BranchCode, ProductType, model.ServiceNo);
                var dtStatusMechanic = ctx.Database.SqlQuery<string>(query).ToList();
                if (dtStatusMechanic.Count > 0) return Json(new { enabled = false });
            }

            // Syarat 6 : harus ada minimal 1 task nnya
            if (dtSubCon.Count() + dtTask.Count() < 1) return Json(new { enabled = false });

            // Syarat 7 : Jika ada part, harus sampai supply slip
            bool isValidSS = false;
            var dtSvTrnSrvItem = (from p in ctx.ServiceItems
                                 where p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType && p.ServiceNo == model.ServiceNo
                                 select new 
                                 {
                                     CompanyCode = p.CompanyCode, 
                                     BranchCode = p.BranchCode,
                                     SupplySlipNo = p.SupplySlipNo
                                 }).ToList();
            if (dtSvTrnSrvItem.Count() > 0)
            {
                foreach (var row in dtSvTrnSrvItem)
                {
                    query = string.Format(@"select LmpNo 
                    from spTrnSLmpHdr a
                    left join spTrnSPickingHdr b on 
	                    b.CompanyCode = a.CompanyCode
	                    and b.BranchCode = a.BranchCode
                        and b.PickingSlipNo = a.PickingSlipNo
                    left join spTrnSOSupply c on             
	                    c.CompanyCode = a.CompanyCode 
	                    and c.BranchCode = a.BranchCode 
                        and c.PickingSlipNo = a.PickingSlipNo
                    where 
                        a.CompanyCode = '{0}' and
                        a.BranchCode = '{1}' and 
                        c.DocNo = '{2}'
                    UNION
                    select '1' LmpNo
                    from sptrnSoRdHdr
                    where (select count(*) from
                        spTrnSOSUpply
                        where 
                        CompanyCode = '{0}' and
                        BranchCode = '{1}' and
                        DocNo = '{2}') = 0 and
                    CompanyCode = '{0}' and
                    BranchCode = '{1}' and 
                    DocNo = '{2}' and 
                    Status >= 2", CompanyCode, BranchCode, row.SupplySlipNo);
                    isValidSS = ctx.Database.SqlQuery<string>(query).ToList().Count > 0;
                    if (!isValidSS) return Json(new { enabled = false });
                }
            }

            var isHaveInvoice = ctx.Invoices.Where(a => a.CompanyCode == model.CompanyCode && a.BranchCode == model.BranchCode && a.JobOrderNo == model.JobOrderNo);

            if (isHaveInvoice.Count() > 0)
            { return Json(new { enabled = false }); }


            return Json(new { enabled = true });
        }

        public void UpdateSPKPrintSeq(string JobOrderNo, string JobOrderNoEnd)
        {
            var datt = ctx.Services.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.ProductType == ProductType ).AsQueryable();

            List<TrnService> dat = new List<TrnService>(); 
            if(JobOrderNo==JobOrderNoEnd)
            {
                dat = datt.Where(x => x.JobOrderNo == JobOrderNo).ToList();
            }
            else
            {
                dat = datt.Where(p => (string.Compare(p.JobOrderNo, JobOrderNo) >= 0 && string.Compare(p.JobOrderNo, JobOrderNoEnd) <= 0)).ToList();
            }

            if (dat.Count > 0)
            {
                foreach (var i in dat)
                {
                    i.PrintSeq += 1;
                    ctx.SaveChanges();
                }
            }
        }

        public JsonResult GetVeh(TrnService model)
        {
            var record = ctx.CustomerVehicleViews.Where(p => p.CompanyCode == CompanyCode && p.PoliceRegNo == model.PoliceRegNo).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }
       
        public JsonResult GetJobType(TrnService model)
        {
            var record = ctx.JobTypeViews.Where(p => p.CompanyCode == CompanyCode && p.BasicModel == model.BasicModel && p.JobType == model.JobType).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult GetServiceAdvisor(TrnService model)
        {
            var record = ctx.SaViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.EmployeeID == model.ForemanID).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult GetForeman(TrnService model)
        {
            var record = ctx.FmViews.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.EmployeeID == model.MechanicID).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult GetCustomerBill(TrnService model)
        {
            var record = ctx.CustomerViews.Where(p => p.CompanyCode == CompanyCode && p.CustomerCode == model.CustomerCodeBill).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult CheckCampaign (string ChassisCode, int ChassisNo, DateTime TrsDate)
        {
            string dsc = String.Empty;
            string strSQL = String.Empty;
            string JobNo = String.Empty;
            string job = String.Empty;
            string descr = String.Empty;

            ctx.SvMstCampaigns.Where(x => x.CompanyCode == CompanyCode && x.ChassisCode == ChassisCode && (ChassisNo >= x.ChassisStartNo && ChassisNo <= x.ChassisEndNo) && x.CloseDate > TrsDate && x.IsActive == true)
            .ToList().ForEach(y => {
                strSQL = "SELECT a.JobOrderNo FROM svTrnservice a INNER JOIN svTrnSrvTask b ON (a.ServiceNo = b.ServiceNo AND b.OperationNo='" + y.OperationNo +  
                         "') WHERE a.JobType='CLAIM' AND a.ChassisCode='" + ChassisCode + "' AND a.ChassisNo='" + ChassisNo + "'";

                job = ctx.Database.SqlQuery<string>(strSQL).FirstOrDefault() == null ? "" : ctx.Database.SqlQuery<string>(strSQL).FirstOrDefault();
                descr = y.Description == null ? "" : y.Description;

                if (JobNo == "") 
                {
                    JobNo = job;
                }
                else
                {
                    JobNo = JobNo + ", " + job;
                }
                if (dsc == "")
                {
                    dsc = descr;
                }
                else
                {
                    dsc = dsc + ", " + descr;
                }
            });
            
            //if(dsc != "" && JobNo != ""){
            //    return Json(new { success = true, message = "Kendaraan termasuk dalam kategori Campaign ( " + dsc + " ) dan telah dilakukan proses penggantian pada No SPK: " + JobNo + "" });
            //}
            //else if(dsc != "")
            //{
            //    return Json(new { success = true, message = "Kendaraan termasuk dalam kategori Campaign ( " + dsc + " ), segera lakukan proses pengecekan" });
            //}
            if (dsc != "" && JobNo == "")
            {
                return Json(new { success = true, message = "Kendaraan termasuk dalam kategori Campaign ( " + dsc + " ), segera lakukan proses pengecekan" });
            }
            return Json(new { success = false, message = "" });
        }

        // IsFSCLock
        public JsonResult ValidateFSC(string basicModel, string jobType, string taskPartNo)
        {
            var check = (ctx.svMstTasks.Where(a => a.BasicModel == basicModel && a.JobType == jobType)
                        .Select(a => new { a.CompanyCode, a.ProductType, a.BasicModel, a.JobType, a.OperationNo, Item = a.OperationNo })
                        .Union(ctx.svMstTaskParts.Where(a => a.BasicModel == basicModel && a.JobType == jobType)
                        .Select(d => new { d.CompanyCode, d.ProductType, d.BasicModel, d.JobType, d.OperationNo, Item = d.PartNo }))).Where(c=> c.Item == taskPartNo);
                                
            return Json(new { IsFSCItem = check.Count() > 0 });
        }
    }
}