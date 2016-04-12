using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GeLang;
using SimDms.Sparepart.Models;
using TracerX;
using SimDms.Sparepart.BLL;
using System.Text;
using SimDms.Common;
using System.Transactions;

namespace SimDms.Sparepart.Controllers.Api
{
    public class StockOpnameController : BaseController
    {
        /// <summary>
        /// Print Stock Taking
        /// </summary>
        /// <param name="condition"></param>
        /// <returns>Json Result</returns>
        public JsonResult PrintStockTaking(string condition) 
        {
            bool result = false, isAlreadyUpdated = false;
            bool blank = (condition == "3") ? true : false;
            string stFirst = "", stLast = "";
            string msgError = "";

            var dtStockTakingHdr = ctx.SpTrnStockTakingHdrs.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && (new string[] {"0", "1"}).Contains(p.Status));

            var row = ctx.SpTrnStockTakingTemps.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.STHdrNo == dtStockTakingHdr.FirstOrDefault().STHdrNo && p.PartNo != "").OrderBy(p => p.STNo);
            if (row != null)
                stFirst = row.FirstOrDefault().STNo;

            row = ctx.SpTrnStockTakingTemps.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.STHdrNo == dtStockTakingHdr.FirstOrDefault().STHdrNo && p.PartNo != "").OrderByDescending(p => p.STNo);
            if (row != null)
                stLast = row.FirstOrDefault().STNo;

            // check header with status 0
            if (dtStockTakingHdr != null || dtStockTakingHdr.Count() > 0)
            {
                // check header with status 1
                dtStockTakingHdr = ctx.SpTrnStockTakingHdrs.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.Status == "1");

                // is header status already updated ?
                isAlreadyUpdated = dtStockTakingHdr != null && dtStockTakingHdr.Count() > 0;
            }

            // if header status isn't updated
            if (isAlreadyUpdated)
            {
                result = true;
            }
            else
            {
                // Added By : Aryo
                // BUGS ID : 366
                dtStockTakingHdr = ctx.SpTrnStockTakingHdrs.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.Status == "0");
                if (dtStockTakingHdr.Count() > 0)
                {
                    var recordHdr = ctx.SpTrnStockTakingHdrs.Find(CompanyCode, BranchCode, dtStockTakingHdr.FirstOrDefault().STHdrNo);
                    result = recordHdr == null ? false : ProsesInvPrint(recordHdr, blank, ref msgError);
                }
            }

            if (result)
            {
                if (blank)
                    return Json(new { success = false, message = "Kondisi Stock Taking Blank" });  
                else
                {
                    //if (dtStockTakingHdr.Count() == 1)
                    //{
                        string type = dtStockTakingHdr.FirstOrDefault().TypeCode;
                        string paramSTHdrNo = dtStockTakingHdr.FirstOrDefault().STHdrNo;

                        if (type == "1")
                            return Json(new { success = true, reportID = "SpRpTrn020", param1 = paramSTHdrNo, param2 = stFirst, param3 = stLast }); 
                        else
                            return Json(new { success = true, reportID = "SpRpTrn021", param1 = paramSTHdrNo, param2 = stFirst, param3 = stLast }); 
                    //}
                }
            }
            else
                return Json(new { success = false, message = msgError });
        }

        private bool ProsesInvPrint(SpTrnStockTakingHdr recordHdr, bool blank, ref string msgError)
        {
            bool result = false;
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                   new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (!blank)
                        result = UpdateInventoryTagForm(recordHdr);
                    else
                        result = UpdateInventoryTagFormBlank(recordHdr);

                    tranScope.Complete();
                }
            }
            catch (Exception ex)
            {
                msgError = ex.Message;
                return result;
            }

            return result;
        }

        /// <summary>
        /// Update Inventory Tag Form
        /// Set SpTrnStockTakingHdr, status 1 
        /// Set spTrnStockTakingTemp, status 1 for all STHDrNo in SpTrnStockTakingHdr
        /// </summary>
        /// <param name="recordHdr"></param>
        /// <returns>boolean</returns>
        private bool UpdateInventoryTagForm(SpTrnStockTakingHdr recordHdr)
        {
            bool result = false;
            var spTrnStockTakingHdrBLL = SpTrnStockTakingHdrBLL.Instance(CurrentUser.UserId);
            var spTrnStockTakingTempBLL = SpTrnStockTakingTempBLL.Instance(CurrentUser.UserId);

            var dtDetail = ctx.SpTrnStockTakingTemps.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.STHdrNo == recordHdr.STHdrNo);

            recordHdr.Status = "1";
            recordHdr.LastUpdatedBy = CurrentUser.UserId;
            recordHdr.LastUpdatedDate = DateTime.Now;

            foreach (SpTrnStockTakingTemp row in dtDetail)
            {
                SpTrnStockTakingTemp recordDtl = spTrnStockTakingTempBLL.GetRecord(recordHdr.STHdrNo, row.STNo, row.SeqNo.Value);
                if (recordDtl != null)
                {
                    recordDtl.Status = "1";
                    recordDtl.LastUpdatedBy = CurrentUser.UserId;
                    recordDtl.LastUpdatedDate = DateTime.Now;

                    spTrnStockTakingTempBLL.Update();
                }
                else
                {
                    return false;
                }
            }
            result = spTrnStockTakingHdrBLL.Update() > 0;

            return result;
        }

        /// <summary>
        /// Update Inventory Tag Form Blank
        /// Set spTrnStockTakingHdr, status 1 
        /// </summary>
        /// <param name="recordHdr"></param>
        /// <returns>boolean</returns>
        private bool UpdateInventoryTagFormBlank(SpTrnStockTakingHdr recordHdr)
        {
            bool result = false;
            var spTrnStockTakingHdrBLL = SpTrnStockTakingHdrBLL.Instance(CurrentUser.UserId);

            recordHdr.Status = "1";
            recordHdr.LastUpdatedBy = CurrentUser.UserId;
            recordHdr.LastUpdatedDate = DateTime.Now;

            result = spTrnStockTakingHdrBLL.Update() > 0;

            return result;
        }

        public JsonResult SaveEntry(SpTrnStockTakingDtl mdl)
        {
            if (mdl.SeqNo == null || mdl.SeqNo < 1)
                return Json(new { success = false, message="SeqNo Tidak Boleh Kosong" });
            

            if (mdl.STDmgQty>mdl.STQty)
                return Json(new { success = false, message = "Qty. rusak tidak boleh melebihi qty. total" });
            
            mdl.LocationCode=mdl.LocationCode==null?"":mdl.LocationCode;

            var tmp = ctx.SpTrnStockTakingTemps
                    .Where(x => x.BranchCode == BranchCode
                        && x.CompanyCode == CompanyCode
                        && x.STHdrNo == mdl.STHdrNo
                        && x.STNo == mdl.STNo
                        && x.SeqNo == mdl.SeqNo)
                        .SingleOrDefault();

            if (tmp == null)
                return Json(new { success = false, message = "Invalid Data" });
            
            var IsExistInLocation = ctx.SpTrnStockTakingDtls
                                      .Where(x => x.BranchCode == BranchCode
                                                && x.CompanyCode == CompanyCode
                                                && x.STHdrNo == mdl.STHdrNo
                                                && x.PartNo == mdl.PartNo
                                                && x.SeqNo != mdl.SeqNo
                                                && x.LocationCode == mdl.LocationCode)
                                      .ToList();
            if (IsExistInLocation.Count > 0)
                return Json(new { success = false, message = "Part No = " + IsExistInLocation[0].PartNo + "\n" + "Lokasi = " + IsExistInLocation[0].LocationCode + "\n" + "ter-registrasi pada STNo = " + IsExistInLocation[0].STNo });
            
            

            var dtl = ctx.SpTrnStockTakingDtls
                    .Where(x => x.BranchCode == BranchCode
                            && x.CompanyCode == CompanyCode
                            && x.STHdrNo == mdl.STHdrNo
                            && x.STNo == mdl.STNo
                            && x.SeqNo == mdl.SeqNo)
                            .SingleOrDefault();

            var hdr = ctx.SpTrnStockTakingHdrs.Find(CompanyCode, BranchCode, mdl.STHdrNo);
            if(hdr==null)
                return Json(new { success = false, message = "Invalid Data" });
            
            
            
            var curdate = ctx.CurrentTime;
            if (dtl == null)
            {
                dtl = new SpTrnStockTakingDtl();
                ctx.SpTrnStockTakingDtls.Add(dtl);
                dtl.CreatedBy = CurrentUser.UserId;
                dtl.CreatedDate = curdate;
                dtl.EntryDate = curdate;
                
            }
            else
            {
                //insert log
                if (dtl.PartNo != mdl.PartNo
                    || dtl.LocationCode != mdl.LocationCode
                    || dtl.STQty != mdl.STQty
                    || dtl.STDmgQty != mdl.STDmgQty)
                {
                    SpTrnStockTakingLog recSTLog = new SpTrnStockTakingLog();
                    recSTLog.CompanyCode = CompanyCode;
                    recSTLog.BranchCode = BranchCode;
                    recSTLog.STHdrNo = tmp.STHdrNo;
                    recSTLog.STNo = tmp.STNo;
                    recSTLog.SeqNo = tmp.SeqNo;
                    recSTLog.CreatedDate = curdate;
                    recSTLog.CreatedBy = CurrentUser.UserId;
                    recSTLog.PartNo = dtl.PartNo;
                    recSTLog.LocationCode = dtl.LocationCode;
                    recSTLog.isMainLocation =dtl.isMainLocation;
                    recSTLog.MovingCode =dtl.MovingCode;
                    recSTLog.OnHandQty = dtl.OnHandQty;
                    recSTLog.STQty = dtl.STQty;
                    recSTLog.STDmgQty = dtl.STDmgQty;
                    recSTLog.EntryDate = dtl.EntryDate;
                    recSTLog.PrintSeq = 0;
                    recSTLog.Status =((int)logStatus.Open).ToString();
                    ctx.SpTrnStockTakingLogs.Add(recSTLog);
                }

                
                    
            }


     
            dtl.LastUpdatedBy = CurrentUser.UserId;
            dtl.LastUpdatedDate =curdate;
            dtl.isMainLocation = SpMstItemsBLL.Instance(CurrentUser.UserId).isMainLocation(mdl);

            if (dtl.isMainLocation)
            {
                var recLoc = ctx.SpMstItemLocs.Find(CompanyCode, BranchCode, mdl.PartNo, hdr.WarehouseCode);
                dtl.OnHandQty = recLoc == null ? recLoc.OnHand : 0;
            }
            else
                dtl.OnHandQty=0;

            dtl.BranchCode = tmp.BranchCode;
            dtl.CompanyCode = tmp.CompanyCode;
            dtl.LocationCode = mdl.LocationCode;
            dtl.MovingCode = (string.IsNullOrEmpty(tmp.MovingCode)) ? ctx.spMstItems.Find(CompanyCode, BranchCode, mdl.PartNo).MovingCode : tmp.MovingCode; 
            dtl.OnHandQty = mdl.OnHandQty;
            dtl.PartNo = mdl.PartNo;
            dtl.PrintSeq = 0;
            dtl.SeqNo = tmp.SeqNo;
            dtl.Status = "0";
            dtl.STDmgQty = mdl.STDmgQty;
            dtl.STHdrNo = tmp.STHdrNo;
            dtl.STNo = mdl.STNo;
            dtl.STQty = mdl.STQty;

            //update temp
            tmp.Status = ((int)logStatus.Closed).ToString();
            tmp.LastUpdatedBy = CurrentUser.UserId;
            tmp.LastUpdatedDate =curdate;



            try
            {
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                return Json(new { success = false , message=ex.Message});
            }

            return Json(new { success = true,data=dtl });
        }

        public JsonResult ProsesInvTag(string warehousecode, string locationcode, string typecode, string condition, string typeofgoods)
        {
            bool blank = (condition == "3" ? true : false);
            bool result = false;
            string lblinfo = "";
            if (locationcode == null)
                locationcode = "";
            if (typeofgoods == null)
                typeofgoods = "";

            try
            {
                result = StockTackingBLL.Instance(CurrentUser.UserId).ProsesInvTag(GetNewDocumentNo, warehousecode, typecode, condition, locationcode, typeofgoods, blank, (typeofgoods == "" ? true : false));
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            
            var hdr = getlabelinfo(out lblinfo);
            
            return Json(new { success = result, message = lblinfo, data = hdr });
        }

        public JsonResult ValidationProsesInvTag(string warehousecode, string locationcode, string typecode, string condition, string typeofgoods)
        {
            var usr = CurrentUser;
            if (locationcode == null)
                locationcode = "";
            if (typeofgoods == null)
                typeofgoods = "";

            string msg=  DateTransValidation(DateTime.Now.Date);
            if ( msg!= "")
            {
                return Json(new { success = false, message = msg });
            }
            var com = CommonBLL.Instance(usr.UserId);

            var lstpartno = SpMstItemsBLL.Instance(usr.UserId).GetPartNo4ValidationStockTaking(warehousecode);
            if (lstpartno.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\nPastikan jumlah InTransit, Alokasi Sparepart, Alokasi Unit, \nAlokasi Bengkel, Reserved Sparepart, Reserved Unit dan \nReserved Bengkel bernilai 0 untuk no. part di bawah ini :");
                lstpartno.ForEach(x => sb.AppendFormat("\n- {0}", x));
                return Json(new { success = false, message = sb.ToString() });
            }
            else
            {
                var dtItemLoc = SpMstItemsBLL.Instance(usr.UserId).GetPartNo4ValidationStockTaking(warehousecode, locationcode);
                if (dtItemLoc.Count <= 0)
                {
                    return Json(new { success = false, message = string.Format("Data part dalam lokasi \"{0}\" tidak ditemukan", locationcode) });
                }
                else
                {
                    return Json(new { success = true, message = "" });  
                }                
            }            
        }

        public JsonResult ProsesInvBlankTag(string sqty)
        {
            int qty = 0;            
            var hdr = new SpTrnStockTakingHdr();
            if (int.TryParse(sqty,out qty))
            {
                if (qty > 0)
                {
                    var bret = false;
                    try
                    {
                       bret=StockTackingBLL.Instance(CurrentUser.UserId).ProsesBlank(GetNewDocumentNo, (decimal)qty,out hdr);
                    }
                    catch (Exception ex)
                    {
                        if(ex.Message.Contains("Violation of PRIMARY KEY"))
                            bret = false;
                        else 
                        return Json(new { success = false, message = ex.Message });
                    }
                    if (bret)
                    {
                        string sfirst, slast = "";
                        string slbl = StockTackingBLL.Instance(CurrentUser.UserId).GetLabelInfo(hdr.STHdrNo, out sfirst, out slast);
                        return Json(new { success = true,
                                          data = new { LabelInfo = slbl, SThdrNo = hdr.STHdrNo,TypeCode=hdr.TypeCode,FirstSTKNO = sfirst, LastSTKNO = slast, BranchCode = BranchCode, CompanyCode = CompanyCode },
                                          message = ctx.SysMsgs.Find(SysMessages.MSG_1100).MessageCaption });
                    }
                    else
                        return Json(new { success = false, message = ctx.SysMsgs.Find(SysMessages.MSG_1101).MessageCaption });
                }
                else
                {
                   
                    return Json(new { success = false, message = "Jumlah form/tag tidak boleh bernilai 0" });
                }

            }
            else
            {
                return Json(new {success=false,message="Invalid Parameter" });
            }

        }
        
        public JsonResult CheckButtonValidation()
        {
        
            string lblinfo="";
            var hdr = getlabelinfo(out lblinfo);
            if (hdr != null)
            {
                return Json(new { success = true, message = lblinfo, data = hdr }); 
            }
            return Json(new { success = true, message = "" });                    
        }

        public JsonResult BatalInvForm(SpTrnStockTakingHdr mdl)
        {
            string msg = DateTransValidation(DateTime.Now.Date);
            if (msg != "")
            {
                return Json(new { success = false, message = msg });
            }


            try
            {
                var uid = CurrentUser.UserId;
                var curdt = ctx.CurrentTime;
                var ifound = 0;
                ctx.SpTrnStockTakingTemps
                    .Where(x => x.CompanyCode == CompanyCode
                        && x.BranchCode == BranchCode
                        && x.STHdrNo == mdl.STHdrNo
                        && x.PartNo == ""
                        && x.Status == "1")
                    .ToList()
                    .ForEach(x =>
                    {
                        ifound++;
                        x.Status = "3";
                        x.LastUpdatedBy = uid;
                        x.LastUpdatedDate = curdt;
                    });

                if (ifound > 0)
                {
                    ctx.SaveChanges();
                    return Json(new { success = true, message = "Blank Inventory Form/Tag has been canceled" });
                }
                else
                {
                    return Json(new { success = false, message = "Record Blank Inventory Form/Tag Not Found!" });
                }                

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
           
                


        }
        public JsonResult CheckBatalInventory()
        {
            string sinfo = "";
            string[] rstatus = new string[] { "0", "1" };

            var hdr = getlabelinfo(out sinfo);
            if (hdr != null)
            {
                var tmp = ctx.SpTrnStockTakingTemps
                         .Where(x => x.CompanyCode == CompanyCode
                             && x.BranchCode == BranchCode
                             && x.STHdrNo == hdr.STHdrNo
                             && x.PartNo == ""
                             && rstatus.Contains(x.Status))
                             .ToList();
                if (tmp.Count == 0)
                {
                    return Json(new { success = false, message = "Record Blank Form/Tag Not Found" });
                }
                return Json(new { success = true, data = hdr });
            }
            return Json(new { success = false, message = "Stock Taking Not Found" });
        }

        public JsonResult SparepartValidation(string menuType)
        {
                   string[] rstatus = new string[] { "0", "1" };                    
                   var hdr= ctx.SpTrnStockTakingHdrs
                           .Where(x => x.BranchCode == BranchCode &&
                                     x.CompanyCode == CompanyCode &&
                                      rstatus.Contains(x.Status))
                            .FirstOrDefault();
            

                   bool isStockTacking = hdr != null;
                
                   if(ProfitCenter!="300")
                   {
                       return Json(new { success = false, message = "Kode Profitcenter user tidak benar, silahkan diseting terlebih dahulu" });                    
                   }

                   if (menuType == "ST")
                   {
                       if (!isStockTacking)
                       {
                           var msg = ctx.SysMsgs.Find(SysMessages.MSG_4001).MessageCaption;
                           return Json(new { success = false, message = msg });
                       }
                       else
                       {
                           var sformtag = ctx.LookUpDtls
                                    .Where(x => x.CompanyCode == CompanyCode
                                        && x.CodeID == "TPCD"
                                        && x.LookUpValue == hdr.TypeCode)
                                    .SingleOrDefault().ParaValue;


                           var blankrow = ctx.SpTrnStockTakingTemps
                                       .Where(x => x.BranchCode == BranchCode
                                              && x.CompanyCode == CompanyCode
                                              && x.STHdrNo == hdr.STHdrNo
                                              && x.Status != "3"
                                              && x.PartNo == "")
                                        .OrderBy(x => x.STNo)
                                        .ToList();

                           if (blankrow.Count > 0)
                           {
                               var slblinfo = "";
                               if (blankrow[0].STNo == blankrow.Last().STNo)
                               {
                                   slblinfo = "Nomor Stock Taking : " + blankrow[0].STNo;
                               }
                               else
                               {
                                   slblinfo = "Nomor Stock Taking : " + blankrow[0].STNo + " s/d " + blankrow.Last().STNo;
                                   
                               }
                               return Json(new { success = true, message = slblinfo, formtag = sformtag ,data=hdr});
                           }
                           else
                           {
                               return Json(new { success = true, message = "Tidak ada record blank TAG/FORM", formtag = sformtag ,data=hdr});
                           }
                       }
                   }

                   return Json(new { success = "true", message = "" });
        }

        public JsonResult StockTackingTemp(SpTrnStockTakingTemp mdl)
        {

            var tmp = ctx.SpTrnStockTakingTemps                    
                     .Where(x => x.BranchCode == BranchCode
                         && x.CompanyCode == CompanyCode
                         && x.STHdrNo==mdl.STHdrNo
                         )
                         .AsQueryable();
            
            if(!string.IsNullOrEmpty( mdl.STNo))
            {
                tmp = tmp.Where(x => x.STNo == mdl.STNo);
            }
            if(!string.IsNullOrEmpty( mdl.PartNo))
            {
                tmp = tmp.Where(x => x.PartNo == mdl.PartNo);
            }
            
            if (mdl.SeqNo != null)
            {
                tmp = tmp.Where(x => x.SeqNo == mdl.SeqNo);                
                if (!string.IsNullOrEmpty(mdl.PartNo))
                {
                     var part = ctx.MasterItemInfos.Find(CompanyCode, mdl.PartNo);
                     return Json(new { success = true, data = tmp.FirstOrDefault(), PartInfo = part });
                }
                return Json(new { success = true, data = tmp.FirstOrDefault()});
            }
            return Json(new { success = true, data = tmp });
        }

        public JsonResult StockTackingDtl(SpTrnStockTakingDtl mdl)
        {

            var tmp = ctx.SpTrnStockTakingDtls.Join(ctx.MasterItemInfos,
                        a=>new {a.PartNo},
                        b=>new {b.PartNo},                        
                        (a, b) => new { SpTrnStockTakingDtls = a, MasterItemInfos = b })
                        .Where(x=>x.SpTrnStockTakingDtls.BranchCode==BranchCode
                               && x.SpTrnStockTakingDtls.CompanyCode==CompanyCode);



            if (!string.IsNullOrEmpty(mdl.STHdrNo))
            {
                tmp = tmp.Where(x => x.SpTrnStockTakingDtls.STHdrNo == mdl.STHdrNo);
            }


            if (!string.IsNullOrEmpty(mdl.STNo))
            {
                tmp = tmp.Where(x => x.SpTrnStockTakingDtls.STNo == mdl.STNo);
            }

            if (!string.IsNullOrEmpty(mdl.PartNo))
            {
                tmp = tmp.Where(x => x.SpTrnStockTakingDtls.PartNo == mdl.PartNo);
            }

            if (mdl.SeqNo != null)
            {
                tmp = tmp.Where(x => x.SpTrnStockTakingDtls.SeqNo == mdl.SeqNo);
               
            }

            


            //.ToList();

            //var tl = ret.ToList();
            //return Json(new { success = true, data = ret });
            //return Json(ret.ToList().AsQueryable().toKG());
            return Json(

                tmp.Select(x => new SpTrnStockTakingDtlGrid
                {
                    CompanyCode = x.SpTrnStockTakingDtls.CompanyCode,
                    BranchCode = x.SpTrnStockTakingDtls.BranchCode,
                    STHdrNo = x.SpTrnStockTakingDtls.STHdrNo,
                    STNo = x.SpTrnStockTakingDtls.STNo,
                    SeqNo = x.SpTrnStockTakingDtls.SeqNo,
                    PartNo = x.SpTrnStockTakingDtls.PartNo,
                    LocationCode = x.SpTrnStockTakingDtls.LocationCode,
                    isMainLocation = x.SpTrnStockTakingDtls.isMainLocation,
                    MovingCode = x.SpTrnStockTakingDtls.MovingCode,
                    OnHandQty = x.SpTrnStockTakingDtls.OnHandQty,
                    STQty = x.SpTrnStockTakingDtls.STQty,
                    STDmgQty = x.SpTrnStockTakingDtls.STDmgQty,
                    PrintSeq = x.SpTrnStockTakingDtls.PrintSeq,
                    Status = x.SpTrnStockTakingDtls.CompanyCode,
                    PartName = x.MasterItemInfos.PartName
                })
                    .OrderBy(x => x.SeqNo)
                
                );
            
        }

        public JsonResult CheckPartPrice(string PartNo)
        {
            
            var prc = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, PartNo.Trim());
            if (prc == null)
            {
                return Json(new { success = false, message = "Part No yang dipilih belum memiliki harga." + "\n" + "Silahkan input harga part tersebut terlebih dahulu" });
            }
            return Json(new {success=true });
        }

  
        private SpTrnStockTakingHdr getlabelinfo(out string lblinfo )
        {
            lblinfo = "";
            string[] rstatus = new string[] { "0", "1" };
            var hdr = ctx.SpTrnStockTakingHdrs
                   .Where(x => x.BranchCode == BranchCode && x.CompanyCode == CompanyCode && rstatus.Contains(x.Status))
                   .ToList();

            if (hdr.Count > 0)
            {
                #region setlabelInfo
                string stno = hdr[0].STHdrNo;
                string sf, sl = "";
                lblinfo = StockTackingBLL.Instance(CurrentUser.UserId).GetLabelInfo(stno,out sf,out sl);
                #endregion
                return hdr[0]; 
            }
            return null;
        }


        




    }
}
