using SimDms.Sales.BLL;
using SimDms.Sales.Models;
//using SimDms.Sales.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Sales.Models.Result;
using System.Data.SqlClient;
using System.Data;
using SimDms.Common.TransOutService.cs;

namespace SimDms.Sales.Controllers.Api
{
    public class TransferInMultiController : BaseController
    {
        public JsonResult DetailLoad(string TransferInNo)
        {
            var query = string.Format(@"
                 SELECT 
                Convert(varchar,a.TransferInSeq) as TransferInSeq,a.SalesModelCode,
                Convert(varchar,a.SalesModelYear) as SalesModelYear,b.SalesModelDesc,
                a.ChassisCode,Convert(varchar,a.ChassisNo) as ChassisNo,a.EngineCode,
                Convert(varchar,a.EngineNo) as EngineNo,a.ColourCode,c.RefferenceDesc1 as ColourName,
                a.Remark
            FROM OmTrInventTransferInDetailMultiBranch a
            LEFT JOIN omMstModel b ON a.CompanyCode = b.CompanyCode
                AND a.SalesModelCode = b.SalesModelCode
            LEFT JOIN omMstRefference c ON a.CompanyCode = c.CompanyCode
                AND a.ColourCode = c.RefferenceCode
            WHERE c.RefferenceType = 'COLO'
                AND a.CompanyCode = '{0}'
                AND a.BranchCode = '{1}'
                AND a.TransferInNo = '{2}'
            ORDER BY a.TransferInSeq, a.SalesModelCode,a.SalesModelYear,a.ChassisNo ASC
                       ", CompanyCode, BranchCode, TransferInNo);
            return Json(ctx.Database.SqlQuery<TransferOutDetailView>(query).AsQueryable());
        }

        public JsonResult Default()
        {
            var me = ctx.CoProfiles.Where(m => m.BranchCode == CurrentUser.BranchCode).FirstOrDefault();
            var my = ctx.LookUpDtls.Where(m => m.ParaValue == CurrentUser.BranchCode && m.CodeID == "MPWH").FirstOrDefault();
            var data = new InquiryTrTransferOutView
            {
                CompanyCode = me.CompanyCode,
                BranchCodeFrom = CurrentUser.BranchCode,
                BranchNameFrom = me.CompanyName,
                WareHouseCodeFrom = my.LookUpValue,
                WareHouseNameFrom = my.LookUpValueName
            };
            return Json(new { success = true, data = data });
        }

        public JsonResult Select4WH(string branchCode)
        {
            var query = String.Format(@" 
            SELECT LookUpValue , LookUpValueName , SeqNo
                FROM gnMstLookUpDtl
                WHERE CompanyCode = '{0}'
                    AND CodeID = 'MPWH'
                    AND ParaValue= '{1}'
                ORDER BY SeqNo", CompanyCode, branchCode);
            return Json(new { data = ctx.Database.SqlQuery<WHview>(query).AsQueryable().FirstOrDefault() });
        }

        public JsonResult Approve(OmTrInventTransferInMultiBranch model, string CompanyCodeFrom)
        {
            string msg = "";
            #region ** old **
            //var me = ctx.OmTrInventTransferInMultiBranch.Find(CompanyCode, BranchCode, model.TransferInNo);
            //var data = new OmTrInventTransferInMultiBranch();
            //if (me != null)
            //{
            //    var meBPU = ctx.omTrInventTransferInDetailMultiBranch.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.TransferInNo == model.TransferInNo).FirstOrDefault();


            //    if (meBPU != null && me.Status == "0")
            //    {
            //        var meDSM = ctx.omTrInventTransferInDetailMultiBranch
            //            .Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.TransferInNo == model.TransferInNo)
            //            .FirstOrDefault();
            //        if (meDSM != null)
            //        {
            //            me.Status = "1";
            //            ctx.SaveChanges();
            //            data = ctx.OmTrInventTransferInMultiBranch.Find(CompanyCode, BranchCode, model.TransferInNo);
            //            return Json(new { success = true, data = data });
            //        }
            //        else
            //        {
            //            return Json(new { success = false, message = meBPU.TransferInNo + " : do not have table detail model in HPP!" });
            //        }
            //    }
            //    else if (meBPU != null && me.Status == "1")
            //    {
            //        me.Status = "2";
            //        ctx.SaveChanges();
            //        data = ctx.OmTrInventTransferInMultiBranch.Find(CompanyCode, BranchCode, model.TransferInNo);


            //        return Json(new { success = true, data = data });
            //    }
            //    else
            //    {
            //        return Json(new { success = false, message = "You must fill table detail!" });
            //    }

            //}
            //else
            //{
            //    return Json(new { success = false, data = data });
            //}

            #endregion

            using(var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted)){
                try{
                    object [] parameters = {CompanyCode, BranchCode, model.TransferInNo, CurrentUser.UserId};
                    ctx.Database.ExecuteSqlCommand("uspfn_OmApproveTransferInMultiBranch @p0, @p1, @p2, @p3", parameters);
                
                    TransOutService srv = new TransOutService();
                    //srv.Url = "http://localhost:50436/TransOutService.svc?wsdl";
                    srv.Url = Helpers.GetURL(CompanyCode, CompanyCodeFrom);

                    var datas = ctx.omTrInventTransferInDetailMultiBranch.Where(x => x.CompanyCode == CompanyCode && 
                        x.BranchCode == BranchCode && x.TransferInNo == model.TransferInNo).ToList();

                    if (datas.Count() > 0)
                    {
                        foreach (omTrInventTransferInDetailMultiBranch row in datas)
	                    {
                            srv.UpdateVehicle(CompanyCodeFrom, row.ChassisCode
                                ,Convert.ToDecimal(row.ChassisNo), true, model.TransferInNo, CurrentUser.UserId);

                            SalesVehicle vehicleFrom = srv.GetInfoVehicle(CompanyCodeFrom, row.ChassisCode
                                , Convert.ToInt32(row.ChassisNo), true);

                            if (vehicleFrom != null)
                            {
                                OmMstVehicle dtVehicle = ctx.OmMstVehicles.Find(CompanyCode,
                                    row.ChassisCode, Convert.ToDecimal(row.ChassisNo));

                                dtVehicle.SuzukiDONo = (vehicleFrom.SuzukiDONo != null) ? vehicleFrom.SuzukiDONo.ToString() : "";
                                dtVehicle.SuzukiDODate = vehicleFrom.SuzukiDODate;
                                dtVehicle.SuzukiSJNo = (vehicleFrom.SuzukiSJNo != null) ? vehicleFrom.SuzukiSJNo.ToString() : "";
                                dtVehicle.SuzukiSJDate = vehicleFrom.SuzukiSJDate;
                            
                                ctx.SaveChanges();
                            }
                            else
                            {
                                trans.Rollback();
                                return Json(new { success = false, message = "Update Data Master Kendaraan Gagal !" });
                            }
	                    }
                    
                        trans.Commit();
                        var me = ctx.OmTrInventTransferInMultiBranch.Find(CompanyCode, BranchCode, model.TransferInNo);
                        return Json(new { success = true, transferInNo = me.TransferInNo, data = me });
                    }
                    else{
                        trans.Rollback();
                        return Json(new { success = false, message = "Update Data Master Kendaraan Gagal !" });
                    }
                
                    #region ** remark **
                //if (result)
                //{
                //    DataTable datas = Select4DetailTransferIn(model.TransferInNo);

                //    if (datas.Rows.Count > 0)
                //    {                        
                //        foreach (DataRow row in datas.Rows)
                //        {
                //            if (result)
                //            {
                //                var dtMstVehicle = ctx.OmMstVehicles.Where(p => p.CompanyCode == CompanyCode && p.ChassisCode == row.ChassisCode && p.ChassisNo == Convert.ToDecimal(row.ChassisNo)).FirstOrDefault();

                //                dtMstVehicle.TransferInMultiBranchNo = model.TransferInNo;
                //                dtMstVehicle.LastUpdateBy = CurrentUser.UserId;
                //                dtMstVehicle.LastUpdateDate = DateTime.Now;

                //                ctx.SaveChanges();

                //                var vehicleFrom = from a in ctx.OmMstVehicles
                //                                  where a.CompanyCode == CompanyCode
                //                                  && a.ChassisCode == row.ChassisCode
                //                                  && a.ChassisNo == Convert.ToInt32(row.ChassisNo)

                //                                  select new OmMstVehicle
                //                                  {
                //                                      ChassisCode = a.ChassisCode,
                //                                      ChassisNo = a.ChassisNo,
                //                                      SuzukiDONo = a.SuzukiDONo,
                //                                      SuzukiDODate = a.SuzukiDODate,
                //                                      SuzukiSJNo = a.SuzukiSJNo,
                //                                      SuzukiSJDate = a.SuzukiSJDate
                //                                  };

                //                if (vehicleFrom != null)
                //                {
                //                    OmMstVehicle dtVehicle = ctx.OmMstVehicles.Find(CompanyCode, row.ChassisCode, Convert.ToInt32(row.ChassisNo));
                //                    dtVehicle.SuzukiDONo = (vehicleFrom.FirstOrDefault().SuzukiDONo != null) ? vehicleFrom.FirstOrDefault().SuzukiDONo : "";
                //                    dtVehicle.SuzukiDODate = vehicleFrom.FirstOrDefault().SuzukiDODate;
                //                    dtVehicle.SuzukiSJNo = (vehicleFrom.FirstOrDefault().SuzukiSJNo != null) ? vehicleFrom.FirstOrDefault().SuzukiSJNo : "";
                //                    dtVehicle.SuzukiSJDate = vehicleFrom.FirstOrDefault().SuzukiSJDate;

                //                    result = ctx.SaveChanges() >= 0;
                //                }
                //                else
                //                {
                //                    msg = "Update Data Master Kendaraan Gagal !";
                //                    result = false;
                //                }
                //            }                            
                //        }
                //    }
                //}
                #endregion
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return Json(new { success = false, message = ex.Message + " : " + msg });
                }
            }
        }

        [HttpPost]
        public JsonResult Save(OmTrInventTransferInMultiBranch model)
        {
            string userID = CurrentUser.UserId;
            string companyCode = CompanyCode;
            DateTime currentTime = DateTime.Now;

            var me = ctx.OmTrInventTransferInMultiBranch.Find(companyCode, BranchCode, model.TransferInNo);

            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    if (model.BranchCodeFrom == model.BranchCodeTo && model.WareHouseCodeFrom == model.WareHouseCodeTo)
                    {
                        throw new Exception("Kode Cabang/Gudang asal dan tujuan tidak boleh sama!");
                    }

                    if (me == null)
                    {
                        me = new OmTrInventTransferInMultiBranch();
                        me.CreatedDate = currentTime;
                        me.LastUpdateDate = currentTime;
                        me.CreatedBy = userID;
                        var TransferInNo = GetNewDocumentNo("VTI", model.TransferInDate.Value);
                        me.TransferInNo = TransferInNo;
                        me.Status = "0";
                        ctx.OmTrInventTransferInMultiBranch.Add(me);
                    }
                    else
                    {
                        me.LastUpdateDate = currentTime;
                        me.LastUpdateBy = userID;
                    }
                    me.CompanyCode = CompanyCode;
                    me.BranchCode = BranchCode;
                    //me.TransferInNo = model.TransferInNo;
                    me.TransferInDate = model.TransferInDate;
                    me.TransferOutNo = model.TransferOutNo;
                    me.ReferenceNo = model.ReferenceNo;
                    me.ReferenceDate = model.ReferenceDate;
                    me.CompanyCodeFrom = model.CompanyCodeFrom ?? "";
                    me.BranchCodeFrom = model.BranchCodeFrom ?? "";
                    me.WareHouseCodeFrom = model.WareHouseCodeFrom ?? "";
                    me.CompanyCodeTo = model.CompanyCodeTo ?? "";
                    me.BranchCodeTo = model.BranchCodeTo ?? "";
                    me.WareHouseCodeTo = model.WareHouseCodeTo ?? "";
                    me.ReturnDate = model.ReturnDate;
                    me.Remark = model.Remark;
                    //me.Status = model.Status;
                    ctx.SaveChanges();
                    trans.Commit();

                    return Json(new { success = true, data = me });
                }
                catch (Exception Ex)
                {
                    trans.Rollback();

                    return Json(new { success = false, message = Ex.Message });
                }
            }
        }

        public JsonResult Delete(OmTrInventTransferInMultiBranch model, string CompanyCodeFrom, string BranchCodeFrom)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.OmTrInventTransferInMultiBranch.Find(companyCode, BranchCode, model.TransferInNo);
                    if (me != null && (me.Status == "0" || me.Status == "1"))
                    {
                        me.Status = "3";
                        ctx.SaveChanges();
                    }

                    var meDtl = ctx.omTrInventTransferInDetailMultiBranch.Where(m => m.CompanyCode == companyCode && m.BranchCode == BranchCode && m.TransferInNo == model.TransferInNo).ToArray();
                    if (me != null)
                    {
                        // From Web Services
                        TransOutService srv = new TransOutService();
                        //srv.Url = "http://localhost:50436/TransOutService.svc?wsdl"; 
                        srv.Url = Helpers.GetURL(me.CompanyCode, CompanyCodeFrom);

                        srv.UpdateTransferOutDetail2(CompanyCodeFrom, BranchCodeFrom, me.TransferOutNo, CurrentUser.UserId);

                        // Delete table OmTrInventTransferInDetail
                        var x = meDtl.Length;
                        for (var i = 0; i < x; i++)
                        {
                            ctx.omTrInventTransferInDetailMultiBranch.Remove(meDtl[i]);
                            ctx.SaveChanges();
                        }
                        ctx.SaveChanges();
                        trans.Commit();
                        
                        returnObj = new { success = true, message = "Data Transfer In berhasil di delete." };
                    }
                    else
                    {
                        trans.Rollback();

                        returnObj = new { success = false, message = "Error ketika mendelete  Transfer In, Message=Data tidak ditemukan" };
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    
                    returnObj = new { success = false, message = "Error ketika mendelete  Transfer In, Message=" + ex.ToString() };
                }
            }
            
            return Json(returnObj);
        }

        [HttpPost]
        public JsonResult Save2(omTrInventTransferInDetailMultiBranch model, string ColourName, string CompanyCodeFrom, string BranchCodeFrom)
        {
            ResultModel result = InitializeResult();
            string userID = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;
            decimal seq = 0;
            try
            {
                var recordHdr = ctx.OmTrInventTransferInMultiBranch.Find(CompanyCode, BranchCode, model.TransferInNo);
                if (recordHdr != null)
                {
                    if (recordHdr.Status != "0" && recordHdr.Status != "1")
                    {

                        return Json(new { success = true, data = recordHdr });
                    }
                }

                // Cek SalesModelCode
                OmMstModel mdl = ctx.OmMstModels.Find(CompanyCode, model.SalesModelCode);
                if (mdl == null)
                {
                    throw new Exception("Model \'" + model.SalesModelCode + "\' belum terdaftar di data master. Lengkapi data Master (Model, Year, Colour, Account) !");
                }

                // Cek warna
                MstRefference wrn = ctx.MstRefferences.Find(CompanyCode, "COLO", model.ColourCode);
                if (wrn == null)
                {
                    throw new Exception("Kode Warna \'" + model.ColourCode + "\' (" + ColourName + ") belum terdaftar di Master Refference !");
                }

                // Cek SalesModelYear
                MstModelYear mdlWrn = ctx.MstModelYear.Find(CompanyCode, model.SalesModelCode, Convert.ToDecimal(model.SalesModelYear));
                if (mdlWrn == null)
                {
                    throw new Exception("Tahun model belum terdaftar di data master !");
                }
                // Cek SalesModelColour
                OmMstModelColour mdlClr = ctx.OmMstModelColours.Find(CompanyCode, model.SalesModelCode, model.ColourCode);
                if (mdlClr == null)
                {
                    throw new Exception("Warna model belum terdaftar di data master !");
                }

                var check = ctx.omTrInventTransferInDetailMultiBranch.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.ChassisNo == model.ChassisNo && m.EngineNo == model.EngineNo).FirstOrDefault();

                if (check != null)
                {
                    seq = Convert.ToDecimal(check.TransferInSeq);
                }

                var me = ctx.omTrInventTransferInDetailMultiBranch.Find(CompanyCode, BranchCode, model.TransferInNo, seq);
                if (me == null)
                {
                    var dt1 = CheckTransferInDetailWithTrfOut(recordHdr.CompanyCode, recordHdr.BranchCode,
                        recordHdr.TransferOutNo, model.ChassisCode, model.ChassisNo.Value, false);
                    if (dt1.Count() > 0)
                        throw new Exception("Data ini sudah ada di BranchCode= " + dt1.FirstOrDefault().BranchCode + " TrasferInNo= " + dt1.FirstOrDefault().TransferInNo);

                    var dt = CheckTransferInDetail(model.ChassisCode, model.ChassisNo.Value);
                    if (dt.Count() > 0)
                        throw new Exception("Data ini sudah ada di BranchCode= " + dt.FirstOrDefault().BranchCode + " TrasferInNo= " + dt.FirstOrDefault().TransferInNo);
                }

                using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // Insert table OmTrInventTransferInDetail
                        if (me == null)
                        {
                            me = new omTrInventTransferInDetailMultiBranch();
                            decimal seqNo = Select4MaxSeq(CompanyCode, BranchCode, model.TransferInNo);
                            me.CreatedDate = currentTime;
                            me.LastUpdateDate = currentTime;
                            me.CreatedBy = userID;
                            me.TransferInSeq = seqNo + 1;
                            me.StatusTransferOut = "0";
                            ctx.omTrInventTransferInDetailMultiBranch.Add(me);
                        }
                        else
                        {
                            me.LastUpdateDate = currentTime;
                            me.LastUpdateBy = userID;
                        }

                        me.CompanyCode = CompanyCode;
                        me.BranchCode = BranchCode;
                        me.TransferInNo = model.TransferInNo;
                        //me.TransferInSeq = model.TransferInSeq;
                        me.SalesModelCode = model.SalesModelCode;
                        me.SalesModelYear = model.SalesModelYear;
                        me.ChassisCode = model.ChassisCode;
                        me.ChassisNo = model.ChassisNo;
                        me.EngineCode = model.EngineCode;
                        me.EngineNo = model.EngineNo;
                        me.ColourCode = model.ColourCode;
                        me.Remark = model.Remark;

                        TransOutService srv = new TransOutService();
                        //srv.Url = "http://localhost:50436/TransOutService.svc?wsdl";
                        srv.Url = Helpers.GetURL(CompanyCode, CompanyCodeFrom);

                        SalesVehicle[] datas = srv.GetMasterVehicle(CompanyCodeFrom, BranchCodeFrom, model.ChassisCode,
                                                    Convert.ToDecimal(model.ChassisNo), true);
                        if (datas.Length > 0)
                        {
                            me.COGSUnit = datas[0].COGSUnit;
                            me.COGSOthers = datas[0].COGSOther;
                            me.COGSKaroseri = datas[0].COGSKaroseri;
                        }
                        ctx.SaveChanges();

                        srv.UpdateTransferOutDetail(CompanyCodeFrom, BranchCodeFrom, recordHdr.TransferOutNo, model.SalesModelCode.ToString(),
                                        model.SalesModelYear.Value, true, model.ChassisCode.ToString(),
                                        model.ChassisNo.Value, true, model.LastUpdateBy, false, true);
                        // change status header
                        if (!recordHdr.Status.Equals("0"))
                        {
                            recordHdr.Status = "0";
                            recordHdr.LastUpdateDate = DateTime.Now;
                            recordHdr.LastUpdateBy = CurrentUser.UserId;

                            ctx.SaveChanges();
                        }

                        trans.Commit();

                        var my = ctx.OmMstModels.Where(p => p.SalesModelCode == model.SalesModelCode).FirstOrDefault();
                        var mi = ctx.MstRefferences.Where(p => p.RefferenceCode == model.ColourCode).FirstOrDefault();
                        result.status = true;
                        result.message = "Data Detail Transfer in Multi Branch berhasil disimpan.";
                        result.data = new
                        {
                            SalesModelCode = me.SalesModelCode,
                            SalesModelYear = me.SalesModelYear,
                            SalesModelDesc = my.SalesModelDesc,
                            ChassisCode = me.ChassisCode,
                            ChassisNo = me.ChassisNo,
                            EngineCode = me.EngineCode,
                            EngineNo = me.EngineNo,
                            ColourCode = me.ColourCode,
                            ColourName = mi.RefferenceDesc1,
                            Remark = me.Remark
                        };
                        return Json(result);
                    }
                    catch (Exception Ex)
                    {
                        trans.Rollback();

                        return Json(new { success = false, message = Ex.Message });
                    }
                }
            }
            catch (Exception Ex)
            {
                return Json(new { success = false, message = Ex.Message });
            }
        }

        public JsonResult Delete2(omTrInventTransferInDetailMultiBranch model, string CompanyCodeFrom, string BranchCodeFrom, string TransferOutNo)
        {
            object returnObj = null;
            string companyCode = CompanyCode;
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    var me = ctx.omTrInventTransferInDetailMultiBranch.Find(CompanyCode, BranchCode, model.TransferInNo, model.TransferInSeq);
                    if (me != null)
                    {
                        // Delete table OmTrInventTransferInDetail
                        ctx.omTrInventTransferInDetailMultiBranch.Remove(me);
                        ctx.SaveChanges();

                        // Update table omTrInventTransferOutDetail
                        TransOutService srv = new TransOutService();
                        //srv.Url = "http://localhost:50436/TransOutService.svc?wsdl"; 
                        srv.Url = Helpers.GetURL(CompanyCode, CompanyCodeFrom);

                        srv.UpdateTransferOutDetail(CompanyCodeFrom, BranchCodeFrom, TransferOutNo, 
                                me.SalesModelCode.ToString(),me.SalesModelYear.Value, true,
                                me.ChassisCode.ToString(), me.ChassisNo.Value, true, me.LastUpdateBy, true, true);

                         //change Status omMstVehicle
                        // change status header                    
                        OmTrInventTransferInMultiBranch header = ctx.OmTrInventTransferInMultiBranch.Find(me.CompanyCode,
                                                            me.BranchCode, me.TransferInNo);
                        if (!header.Status.Equals("0"))
                        {
                            header.Status = "0";
                            header.LastUpdateDate = DateTime.Now;
                            header.LastUpdateBy = CurrentUser.UserId;
                            ctx.SaveChanges();
                        }

                        trans.Commit();

                        returnObj = new { success = true, message = "Data Transfer in Detail berhasil di delete." };
                    }
                    else
                    {
                        trans.Rollback();

                        returnObj = new { success = false, message = "Error ketika mendelete Transfer in Detail, Message=Data tidak ditemukan" };
                    }
                }
                catch (Exception ex)
                {
                    trans.Rollback();

                    returnObj = new { success = false, message = "Error ketika mendelete Transfer in Detail, Message=" + ex.ToString() };
                }
            }

            return Json(returnObj);
        }

        public JsonResult updateHdr(OmTrInventTransferInMultiBranch model)
        {
            var recordHdr = ctx.OmTrInventTransferInMultiBranch.Find(CompanyCode, BranchCode, model.TransferInNo);
            if (recordHdr != null)
            {
                var recordDtl = ctx.omTrInventTransferInDetailMultiBranch.Where(x => x.CompanyCode == recordHdr.CompanyCode && x.BranchCode == recordHdr.BranchCode && x.TransferInNo == recordHdr.TransferInNo);
                if (recordDtl.Count() > 0)
                {
                    if (recordHdr.Status == "0")
                    {
                        recordHdr.Status = "1";
                        recordHdr.LastUpdateBy = CurrentUser.UserId;
                        recordHdr.LastUpdateDate = DateTime.Now;
                        ctx.SaveChanges();
                    }
                        
                    return Json(new { success = true, data = recordHdr });
                }
                else
                {
                    return Json(new { success = true, message = "Dokumen tidak dapat dicetak karena tidak memiliki data detail." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Dokumen tidak dapat dicetak karena data tidak ditemukan." });
            }
        }
        protected decimal Select4MaxSeq(string CompanyCode, string BranchCode, string pNo)
        {
            var query = String.Format(@"
            SELECT isnull(max(TransferInSeq),0) FROM omTrInventTransferInDetailMultiBranch
            WHERE CompanyCode = '{0}'
            AND BranchCode = '{1}'
            AND TransferInNo = '{2}'", CompanyCode, BranchCode, pNo);
            var queryable = ctx.Database.SqlQuery<decimal>(query).FirstOrDefault();
            return (queryable);
        }
        private DataTable Select4DetailTransferIn(string transferInNo)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = string.Format("select *  from omTrInventTransferInDetailMultiBranch " +
                                            " where CompanyCode=@CompanyCode " +
                                            " and BranchCode=@BranchCode " +
                                            " and TransferInNo=@TransferInNo", CompanyCode, BranchCode, transferInNo);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dt);

            return dt;
        }

        private IEnumerable<dynamic> CheckTransferInDetailWithTrfOut(string companyCode, string branchCode, string transferOutNo
            , string chassisCode, decimal chassisNo, bool withStatus)
        {
            if (withStatus)
            {
                var status = "2,5".Split(',');
                var records = (from a in ctx.omTrInventTransferInDetailMultiBranch
                    join b in ctx.OmTrInventTransferInMultiBranch on new { a.CompanyCode, a.BranchCode, a.TransferInNo }
                    equals new { b.CompanyCode, b.BranchCode, b.TransferInNo }
                    where b.CompanyCode == companyCode
                    && b.BranchCode == branchCode
                    && b.TransferOutNo == transferOutNo
                    && a.ChassisCode == chassisCode
                    && a.ChassisNo == chassisNo
                    && status.Contains(b.Status)
                    select new
                    {
                        a.BranchCode,
                        a.TransferInNo,
                        a.ChassisCode,
                        a.ChassisNo.Value
                    });

                return records;
            }
            else
            {
                var records = (from a in ctx.omTrInventTransferInDetailMultiBranch
                    join b in ctx.OmTrInventTransferInMultiBranch on new { a.CompanyCode, a.BranchCode, a.TransferInNo }
                    equals new { b.CompanyCode, b.BranchCode, b.TransferInNo }
                    where b.CompanyCode == companyCode
                    && b.BranchCode == branchCode
                    && b.TransferOutNo == transferOutNo
                    && a.ChassisCode == chassisCode
                    && a.ChassisNo == chassisNo
                    select new
                    {
                        a.BranchCode,
                        a.TransferInNo,
                        a.ChassisCode,
                        a.ChassisNo.Value
                    });

                return records;
            }
        }
        private IEnumerable<dynamic> CheckTransferInDetail(string chassisCode, decimal chassisNo)
        {
            var records = (from a in ctx.omTrInventTransferInDetailMultiBranch
                           join b in ctx.OmTrInventTransferInMultiBranch on new { a.CompanyCode, a.BranchCode, a.TransferInNo }
                           equals new { b.CompanyCode, b.BranchCode, b.TransferInNo }
                           where a.ChassisCode == chassisCode
                           && a.ChassisNo == chassisNo
                           select new
                           {
                               a.BranchCode,
                               a.TransferInNo,
                               a.ChassisCode,
                               a.ChassisNo.Value
                           });

            return records;

        }
    }
}
