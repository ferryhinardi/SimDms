using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SimDms.Sparepart.Models;
using System.Data;

namespace SimDms.Sparepart.Controllers.Api
{
    public class LampiranDokumenServiceController : BaseController
    {
        private const string STATUS_1 = "OPEN SUPPLY SLIP";
        private const string STATUS_2 = "OUTSTANDING SUPPLY SLIP";
        private const string STATUS_3 = "CLOSING SUPPLY SLIP";
        private const string STATUS_4 = "NO PICKING AVAILABLE";
        private const string STATUS_5 = "NO LAMPIRAN AVAILABLE";

        public JsonResult Default()
        {
            var curDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            return Json(new { currentDate = curDate });
        }

        public JsonResult JobOrderNo(SelectSPKNoEnhance model)
        {
            string query = string.Format("exec uspfn_spSelectSPKNoEnhance '{0}','{1}','{2}'", CompanyCode, BranchCode, ProductType);
            var record = ctx.Database.SqlQuery<SelectSPKNoEnhance>(query).Where(p => p.JobOrderNo == model.JobOrderNo).FirstOrDefault();

            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        [HttpPost]
        public JsonResult CheckPickingData(SelectSPKNoEnhance model, string Process="")
        {
            //check picking data
            LampiranDocMode LmpDocService = new LampiranDocMode();
            LmpDocService.Controls = new ControlLmp();
            //Get SO Detail
            var SSList = GetPreviewSupplySlipData(model.JobOrderNo);
            LmpDocService.Controls.IsDisableBtnPicking = (SSList.Count > 0 ? false : true);
            LmpDocService.JOSrvEnhance = SSList;
            LmpDocService.OrderDate = DateTime.Now;
            LmpDocService.Controls.IsDisableJobOrderDate = true;
            var PickingList = GetSelectPickingSlipData(model.JobOrderNo);
            if (PickingList.Count > 0)
            {
                LmpDocService.Controls.IsDisableBtnLmp = false;
                //get to mstLookup
                var transType = string.IsNullOrWhiteSpace(PickingList[0].TransTypeDesc) ? "" : PickingList[0].TransTypeDesc;
                var rLkp = ctx.LookUpDtls.Where(m => m.CodeID == "TTSR" && m.LookUpValueName == transType).FirstOrDefault();
                var lookVal = rLkp == null ? "" : rLkp.LookUpValue;
                LmpDocService.TransType = lookVal;
                LmpDocService.OrderDate = PickingList[0].DocDate;

                LmpDocService.Controls.IsDisableTransType = LmpDocService.Controls.IsDisableOrderDate = LmpDocService.Controls.IsDisableBtnPicking =
                    LmpDocService.Controls.IsDisableBtnUsageDocNo = LmpDocService.Controls.IsDisableTxtUsageNo = LmpDocService.Controls.IsDisableTransType = true;
                try
                {
                    if (decimal.Parse(PickingList[0].Status) < 2)
                    {
                        LmpDocService.Controls.IsDisableBtnPickedBy = false;
                    }
                }
                catch (Exception ex)
                { }
                LmpDocService.Controls.PickStatus = STATUS_2;
                LmpDocService.Controls.Status = 2;
            }
            else
            {
                LmpDocService.Controls.IsDisableBtnPicking = false;
                LmpDocService.Controls.IsDisableTransType = false;
                LmpDocService.Controls.IsDisableBtnLmp = true;
                LmpDocService.Controls.IsDisableBtnPickedBy = true;
                LmpDocService.Controls.PickStatus = STATUS_1;
                LmpDocService.Controls.Status = 1;
                LmpDocService.TransType = InitTransType(model.JobOrderNo);
                if (Process.ToLower() == "pick")
                {
                    LmpDocService.Controls.PickStatus = STATUS_4;
                    LmpDocService.Controls.Status = 4;
                    LmpDocService.Controls.IsDisableBtnPicking = true;
                    LmpDocService.Controls.IsDisableBtnLmp = true;
                }
            }
            return Json(LmpDocService);
        }

        public JsonResult EmployeePickedUp(string PickedBy)
        {
            var record = ctx.Employees.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.PersonnelStatus == "1" && m.TitleCode == "7" && m.EmployeeID == PickedBy).FirstOrDefault();
            if (record != null)
                return Json(new { success = true, data = record });
            else
                return Json(new { success = false });
        }

        public JsonResult GenerateSupplySlipPickingEnhance(LampiranDocMode model, string Process = "")
        {
            object returnObj = null;
            
            //check picking data
            LampiranDocMode LmpDocService = new LampiranDocMode();
            LmpDocService.Controls = new ControlLmp();
            //Get SO Detail
            var SSList = GetPreviewSupplySlipData(model.JobOrderNo);
            LmpDocService.Controls.IsDisableBtnPicking = (SSList.Count > 0 ? false : true);
            LmpDocService.JOSrvEnhance = SSList;
            LmpDocService.OrderDate = DateTime.Now;
            LmpDocService.Controls.IsDisableJobOrderDate = true;

            try
            {
                string errorMsg = ValidationSSPicking(model.CustomerCode,model.OrderDate.Date);
                if (!string.IsNullOrWhiteSpace(errorMsg))
                {
                    throw new Exception(errorMsg);
                }
                    
                InsertSupplySlipAndPickingList(model.TransType, model.OrderDate, model.JobOrderNo, model.CustomerCode);
                
                var PickingList = GetSelectPickingSlipData(model.JobOrderNo);
                if (PickingList.Count > 0)
                {
                    LmpDocService.Controls.IsDisableBtnLmp = false;
                    //get to mstLookup
                    var transType = string.IsNullOrWhiteSpace(PickingList[0].TransTypeDesc) ? "" : PickingList[0].TransTypeDesc;
                    var rLkp = ctx.LookUpDtls.Where(m => m.CodeID == "TTSR" && m.LookUpValueName == transType).FirstOrDefault();
                    var lookVal = rLkp == null ? "" : rLkp.LookUpValue;
                    LmpDocService.TransType = lookVal;
                    LmpDocService.OrderDate = PickingList[0].DocDate;

                    LmpDocService.Controls.IsDisableTransType = LmpDocService.Controls.IsDisableOrderDate = LmpDocService.Controls.IsDisableBtnPicking =
                        LmpDocService.Controls.IsDisableBtnUsageDocNo = LmpDocService.Controls.IsDisableTxtUsageNo = LmpDocService.Controls.IsDisableTransType = true;
                    try
                    {
                        if (decimal.Parse(PickingList[0].Status) < 2)
                        {
                            LmpDocService.Controls.IsDisableBtnPickedBy = false;
                        }
                    }
                    catch (Exception ex)
                    { }
                    LmpDocService.Controls.PickStatus = STATUS_2;
                    LmpDocService.Controls.Status = 2;
                }
                else
                {
                    LmpDocService.Controls.IsDisableBtnPicking = false;
                    LmpDocService.Controls.IsDisableTransType = false;
                    LmpDocService.Controls.IsDisableBtnLmp = true;
                    LmpDocService.Controls.IsDisableBtnPickedBy = true;
                    LmpDocService.Controls.PickStatus = STATUS_1;
                    LmpDocService.Controls.Status = 1;
                    LmpDocService.TransType = InitTransType(model.JobOrderNo);
                    if (Process.ToLower() == "pick")
                    {
                        LmpDocService.Controls.PickStatus = STATUS_4;
                        LmpDocService.Controls.Status = 4;
                        LmpDocService.Controls.IsDisableBtnPicking = true;
                        LmpDocService.Controls.IsDisableBtnLmp = true;
                    }
                }

                returnObj = new { success = true, message = "", LmpDocService };
                return Json(returnObj);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error pada function GenerateSupplySlipPickingEnhance, Message=" + ex.Message.ToString() });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="JobOrderNo"></param>
        /// <param name="CustomerCode"></param>
        /// <param name="PickedBy"></param>
        /// <returns></returns>
        public JsonResult ProcessLampiran(string JobOrderNo, string CustomerCode, string PickedBy, string[] PickList, DateTime OrderDate)
        {
            using (ctx)
            {
                using (var trx = ctx.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        var errorMsg = ValidationSSPicking(CustomerCode,OrderDate);
                        if (!string.IsNullOrEmpty(errorMsg))
                        {
                            throw new Exception(errorMsg);
                        }
                        InsertLampiranNonPenjualan(PickedBy, JobOrderNo, CustomerCode);
                        var ListLmp = GetGeneratedLampiranData(PickList);
                        ControlLmp Controls = new ControlLmp();
                        Controls.PickStatus = STATUS_3;
                        Controls.Status = 3;
                        Controls.IsDisableTransType = Controls.IsDisableOrderDate = Controls.IsDisableBtnPicking = Controls.IsDisableBtnLmp = Controls.IsDisableBtnPickedBy =
                            Controls.IsDisableBtnUsageDocNo = Controls.IsDisableTxtUsageNo = Controls.IsDisableTransType = true;
                        if (ListLmp.Count < 0)
                        {
                            Controls.PickStatus = STATUS_5;
                            Controls.Status = 5;
                        }

                        //Commit Transaction
                        trx.Commit();
                        
                        return Json(new { success = true, message = "", Controls = Controls });
                    }
                    catch (Exception ex)
                    {
                        //Rollback Transaction
                        trx.Rollback();
                        
                        return Json(new { success = false, message = "Error pada function ProcessLampiran, Message=" + ex.Message.ToString() });
                    }
                }
            }
        }

        [HttpPost]
        public JsonResult SavePickingList(string PickingSlipNo, string PartNo, string PartNoOri, string DocNo, decimal QtyPicked, decimal QtySupply)
        {
            //object returnVal = null;
            try
            {
                string errorMsg = IsValidStatusPicking(PickingSlipNo);
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    throw new Exception(errorMsg);
                }

                if (QtyPicked > QtySupply)
                {
                    throw new Exception(ctx.SysMsgs.Find("5016").MessageCaption);
                }

                if (QtyPicked < 0)
                {
                    throw new Exception("Nilai tidak boleh < 0 !!!");
                }

                var oPickDtl = ctx.SpTrnSPickingDtls.Find(CompanyCode, BranchCode, PickingSlipNo, "00", PartNo, PartNoOri, DocNo);
                if (oPickDtl != null)
                {
                    oPickDtl.QtyPicked = QtyPicked;
                    oPickDtl.LastUpdateBy = CurrentUser.UserId;
                    oPickDtl.LastUpdateDate = DateTime.Now;
                }
                ctx.SaveChanges();
                return Json(new { success=true,message=""});
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error pada function SavePickingList, Message=" + ex.Message.ToString() });
            }
            //return Json(returnVal);
        }

        private string IsValidStatusPicking(string PickingSlipNo)
        {
            var returnmsg = "";
            var rTrnPckHdr = ctx.SpTrnSPickingHdrs.Find(CompanyCode, BranchCode, PickingSlipNo);
            if (rTrnPckHdr != null)
            {
                if (int.Parse(rTrnPckHdr.Status) > 1)
                {
                    returnmsg = "Nomor dokumen ini sudah ter-posting !!";
                }
            }
            return returnmsg;
        }

        private void InsertSupplySlipAndPickingList(string transType, DateTime dateOrder, string JobOrderNo, string CustCode)
        {
            using (var trans = ctx.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {

                    var sql = string.Format("exec uspfn_GenerateSSPickingslipNew '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'",
                    CompanyCode, BranchCode, JobOrderNo, ProductType, CustCode, transType, CurrentUser.UserId, dateOrder);

                    ctx.Database.ExecuteSqlCommand(sql);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
           }
        }

        private void InsertLampiranNonPenjualan(string PickedBy, string JobOrderNo, string CustCode)
        {
            try
            {
                var sql = string.Format("exec uspfn_GenerateBPSLampiranNew '{0}','{1}','{2}','{3}','{4}','{5}','{6}'",
                    CompanyCode, BranchCode, JobOrderNo, ProductType, CustCode, CurrentUser.UserId, PickedBy);

                ctx.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function InsertLampiranNonPenjualan, Message=" + ex.Message.ToString());
            }
        }

        public string ValidationSSPicking(string CustomerCode,DateTime orderDate)
        {
            string errorMsg;
            //errorMsg = DateTransValidation(DateTime.Now);
            errorMsg = DateTransValidation(orderDate);

            if (!string.IsNullOrEmpty(errorMsg))
            {
                return errorMsg;
            }

            ProfitCenter recProf = null;
            if (string.IsNullOrEmpty(errorMsg))
            {
                recProf = ctx.ProfitCenters.Find(CompanyCode, BranchCode, CustomerCode, ProfitCenter);
                if (recProf == null)
                {
                    return errorMsg = string.Format("Pelanggan ini belum terdaftar di profit center {0}.", ProfitCenter);
                }
            }

            if (!string.IsNullOrEmpty(errorMsg))
            {
                if (string.IsNullOrEmpty(recProf.TaxCode))
                {
                    return errorMsg = string.Format("Kode pajak pelanggan ini belum terisi di profit center {0}.", ProfitCenter);
                }
            }

            errorMsg = isOverdueOrderExist(CustomerCode, recProf);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                return errorMsg;
            }

            if (!CurrentUser.CoProfile.IsLinkToService.Value)
            {
                errorMsg = "Link to Service tidak diketemukan";
            }
            return errorMsg;
        }

        private string InitTransType(string JobOrderNo)
        {
            string TransType = "";
            var SvcList = ctx.SvTrnServices.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.ProductType == ProductType && m.JobOrderNo == JobOrderNo).ToList();
            long svNo = (long)SvcList[0].ServiceNo;
            try
            {
                var oSvc = ctx.SvTrnServices.Find(CompanyCode, BranchCode, ProductType, svNo);
                if (oSvc != null)
                {
                    if (oSvc.JobType.StartsWith("F"))
                    {
                        TransType = "22";
                    }
                    else if (oSvc.JobType.StartsWith("C"))
                    {
                        TransType = "23";
                    }
                    else
                    {
                        TransType = "20";
                    }
                }
            }
            catch (Exception)
            {
                TransType = "21";
            }
            return TransType;
        }

        public JsonResult GetPreviewSupplySlip(SelectSPKNoEnhance model)
        {
            var JOSrvEnhance = GetPreviewSupplySlipData(model.JobOrderNo);
            return Json(JOSrvEnhance);
        }

        public JsonResult GetSelectPickingSlip(string JobOrderNo)
        {
            var SelectPS = GetSelectPickingSlipData(JobOrderNo);
            return Json(SelectPS);
        }

        public JsonResult GetSelectPickingSlipAfterLmp(string JobOrderNo)
        {
            var SelectPS = GetSelectPickingSlipDataAfterLmp(JobOrderNo);
            return Json(SelectPS);
        }

        public JsonResult GetGeneratedLampiran(string[] PickList)
        {
            var SelectLmp = GetGeneratedLampiranData(PickList);
            return Json(SelectLmp);
        }

        public List<SelectListPickingSlip> GetSelectPickingSlipData(string JobOrderNo)
        {
            var sql = string.Format("exec uspfn_spSelectPickingSlip '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, ProductType, JobOrderNo);
            var SelectPS = ctx.Database.SqlQuery<SelectListPickingSlip>(sql).ToList();
            return SelectPS;
        }

        public List<SelectListPickingSlip> GetSelectPickingSlipDataLmp(string JobOrderNo)
        {
            var sql = string.Format("exec uspfn_spSelectPickingSlipLmp '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, ProductType, JobOrderNo);
            var SelectPS = ctx.Database.SqlQuery<SelectListPickingSlip>(sql).ToList();
            return SelectPS;
        }

        public List<SelectListPickingSlip> GetSelectPickingSlipDataAfterLmp(string JobOrderNo)
        {
            var sql = string.Format("exec uspfn_spSelectPickingSlipAfterLmp '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, ProductType, JobOrderNo);
            var SelectPS = ctx.Database.SqlQuery<SelectListPickingSlip>(sql).ToList();
            return SelectPS;
        }

        private List<JobOrderNo4SOServiceEnhanche> GetPreviewSupplySlipData(string JobOrderNo)
        {
            var sql = string.Format("exec uspfn_SelectJobOrderNo4SOServiceEnhanche '{0}','{1}','{2}','{3}'", CompanyCode, BranchCode, ProductType, JobOrderNo);
            var JOSrvEnhance = ctx.Database.SqlQuery<JobOrderNo4SOServiceEnhanche>(sql).ToList();
            return JOSrvEnhance;
        }

        private List<SpLmpDtlView> GetGeneratedLampiranData(string[] PickingList)
        {
            //var listPLS = GetSelectPickingSlipData(JobOrderNo);
            string PickingListStr="";
            int idx = 0;
            string[] a = {"",""};
            List<SpLmpDtlView> lmpDtl = new List<SpLmpDtlView>();
            if (PickingList != null)
            {
                foreach (var PickNo in PickingList)
                {
                    if (idx == 0)
                        PickingListStr = "'" + PickNo + "'";
                    else
                        PickingListStr += ",'" + PickNo + "'";
                    idx++;
                }

                var sql = string.Format(@"SELECT     
                row_number () OVER (ORDER BY spTrnSLmpDtl.CreatedDate ASC) AS NoUrut,
                spTrnSLmpDtl.LmpNo,
                spTrnSLmpDtl.PartNo,
                spTrnSLmpDtl.PartNoOriginal,
                spTrnSLmpDtl.DocNo, 
                CONVERT(VARCHAR, spTrnSLmpDtl.DocDate, 106) AS DocDate, 
                spTrnSLmpDtl.ReferenceNo, 
                spTrnSLmpDtl.QtyBill
            FROM spTrnSLmpDtl
            WHERE
            spTrnSLmpDtl.LmpNo IN (
                SELECT LmpNo FROM spTrnSLmpHdr WHERE CompanyCode = '{0}' AND BranchCode = '{1}' AND 
                    PickingSlipNo IN ({2})
            ) AND spTrnSLmpDtl.CompanyCode = '{0}' AND
                spTrnSLmpDtl.BranchCode = '{1}' ", CompanyCode, BranchCode, PickingListStr);
                lmpDtl = ctx.Database.SqlQuery<SpLmpDtlView>(sql).ToList();
            }
            return lmpDtl;            
        }

        public JsonResult prePrint(string jobOrderNo, string statSS)
        {
            List<prePrint> data = new List<prePrint>();
            object[] parameters = {};
            var query = "";
            if (statSS == "3")
            {
                var listPicking = GetSelectPickingSlipDataLmp(jobOrderNo).Distinct();
                
                var slipNo = listPicking.Select(p => p.PickingSlipNo).ToArray();
                var recTrnSLmpHdrs = ctx.SpTrnSLmpHdrs.Where(p => slipNo.Contains(p.PickingSlipNo));

                data = recTrnSLmpHdrs.Select(p => new prePrint
                {
                    LmpNo = p.LmpNo, PickingSlipNo= p.PickingSlipNo, TypeOfGoods = p.TypeOfGoods, TransType = p.TransType,
                    SalesType = p.TransType.Substring(0,1),
                    DocNo = ctx.SpTrnSPickingDtls.FirstOrDefault(
                        x => x.CompanyCode == p.CompanyCode && x.BranchCode == BranchCode && x.PickingSlipNo == p.PickingSlipNo
                        ).DocNo
                }).ToList(); 
                
                
                //ctx.Database.SqlQuery<prePrint>(query, par).ToList();

                foreach (var d in data)
                {
                    var recordLM = ctx.SpTrnSLmpHdrs.Find(CompanyCode, BranchCode, d.LmpNo);
                    if (recordLM != null)
                    {
                        if (recordLM.Status == "0")
                            recordLM.Status = "1";

                        recordLM.PrintSeq = recordLM.PrintSeq + 1;
                        recordLM.LastUpdateBy = CurrentUser.UserId;
                        recordLM.LastUpdateDate = DateTime.Now;

                        ctx.SaveChanges();
                    }
                }

            }
            else
            {
                query = string.Format(@"uspfn_spPickingSlipForPrint '{0}','{1}','{2}','{3}'
                ", CompanyCode, BranchCode, ProductType, jobOrderNo);

                data = ctx.Database.SqlQuery<prePrint>(query).ToList();
            }

            return Json(data);
        }

        public JsonResult reportParam(string transType)
        {
            var data = ctx.LookUpDtls.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.CodeID == "TTSR" && a.LookUpValue == transType).LookUpValueName;
            return Json(data);
        }
    }
}
