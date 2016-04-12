using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Transactions;
using SimDms.Common;
using SimDms.Sparepart.Models;


namespace SimDms.Sparepart.BLL
{
    /// <summary>
    /// Stock Taking BLL. Processing Inventory Form/Tag. Process BLank, Get Label Info etc.
    /// </summary>
    public class StockTackingBLL : BaseBLL
    {
        /// <summary>
        /// private instance of StockTackingBLL
        /// </summary>
        #region "Initiate"
        private static StockTackingBLL _StockTackingBLL;

        /// <summary>
        /// Get Instance
        /// </summary>
        /// <param name="_username">Current User</param>
        /// <returns>StockTackingBLL Object</returns>
        public static StockTackingBLL Instance(string _username)
        {
            if (_StockTackingBLL == null)
            {
                _StockTackingBLL = new StockTackingBLL();
            }
            if (string.IsNullOrEmpty(username))
            {
                username = _username;
            }
            return _StockTackingBLL;
        }
        #endregion


        /// <summary>
        /// Process Stock Taking Form/Tag
        /// </summary>
        /// <param name="GenDocNo">Function for Generate Document Number</param>
        /// <param name="whcode">Wareouse Code</param>
        /// <param name="codetype">Type Code Tag/Form</param>
        /// <param name="condition">Condition Code</param>
        /// <param name="locationCode">Location Code</param>
        /// <param name="typeOfGoods">Type Of Goods (ex. SGP, SGO ext)</param>
        /// <param name="blank">blank Form/Tag State</param>
        /// <param name="all">All Part State</param>
        /// <returns>
        /// Process Success or Failed
        /// </returns>
        public bool ProsesInvTag(Func<string, DateTime, string> GenDocNo, string whcode, string codetype, string condition, string locationCode, string typeOfGoods, bool blank, bool all)
        {
            var hdr = new SpTrnStockTakingHdr()
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                WarehouseCode = whcode,
                TypeCode = codetype,
                Condition = condition,
                Status = "1",
                CreatedBy = username,
                CreatedDate = ctx.CurrentTime,
                LastUpdatedBy = username,
                LastUpdatedDate = ctx.CurrentTime,
                STHdrNo = GetNewDocumentNo(GenDocNo,"STH", ctx.CurrentTime.ToString("yy"))// GenDocNo("STH", ctx.CurrentTime)
            };


            if (!all)
                hdr.TypeOfGoods = typeOfGoods;

            if (hdr.STHdrNo.EndsWith("X"))
            {
                var msg = string.Format(ctx.SysMsgs.Find(SysMessages.MSG_5046).MessageCaption, "Stock Taking (STH)");
                throw new Exception(msg);
            }
            else
            {
                
                    ctx.SpTrnStockTakingHdrs.Add(hdr);
                    if (!blank)
                    {

                        var qrbl = ctx.Database.SqlQuery<SpSOSelectforInsert>("sp_SpSOSelectforInsert '" + CompanyCode + "','" + BranchCode + "','" + locationCode + "'")
                                   .Where(x => x.WarehouseCode == whcode)
                                    .AsQueryable();

                        if (!all)
                        {
                            qrbl = qrbl.Where(x => x.TypeOfGoods == typeOfGoods).AsQueryable();
                        }

                        if (condition == "1")
                            qrbl = qrbl.Where(x => x.OnHand > 0);
                        else if (condition == "2")
                            qrbl = qrbl.Where(x => x.OnHand > 0 && locationCode != "");

                        var dtpart = qrbl.ToList();


                        var dtTempRec = new List<SpTempRec>();
                        var lstitmloc = ctx.SpMstItemLocs
                                          .Where(y => y.CompanyCode == CompanyCode &&
                                              y.BranchCode == BranchCode &&
                                              y.WarehouseCode == whcode).ToList();

                        var listitmpart = ctx.spMstItems
                                                    .Where(y =>
                                                       y.CompanyCode == CompanyCode &&
                                                       y.BranchCode == BranchCode)
                                                       .ToList();

                        if (dtpart.Count > 0)
                        {
                            var icnt = 0;
                            dtpart.ForEach(x =>
                            {
                                icnt++;
                                //Debug.WriteLine(icnt.ToString()+ " "+ x.PartNo);                               

                                ArrayList loccode = new ArrayList();
                                var itmloc = lstitmloc.Where(y => y.PartNo == x.PartNo).SingleOrDefault();
                                int iLoopLocation = CheckLocation(itmloc, condition, ref loccode);

                                for (int j = 0; j < iLoopLocation; j++)
                                {


                                    var dttemp = new SpTempRec() { PartNo = itmloc.PartNo, LocationCode = loccode[j].ToString() };
                                    if (j == 0)
                                    {
                                        dttemp.isMainLocation = true;
                                        dttemp.OnHand = itmloc.OnHand;
                                    }
                                    else
                                    {
                                        dttemp.isMainLocation = false;
                                        dttemp.OnHand = 0;
                                    }

                                    var recordItem = listitmpart.Where(y => y.PartNo == x.PartNo).SingleOrDefault();


                                    dttemp.MovingCode = recordItem.MovingCode;
                                    dttemp.Flag = 1;
                                    dtTempRec.Add(dttemp);

                                }



                            });
                        }


                        var param = ctx.SysParameters.Where(y => y.ParamId == "FORMQTY").SingleOrDefault();
                        var dtltemp = new SpTrnStockTakingTemp();

                        var rowFinal = dtTempRec
                                     .Where(x => x.Flag == 1)
                                     .OrderBy(x => x.LocationCode)
                                     .ThenBy(x => x.PartNo)
                                     .ToList();
                        int iSeq = 0;
                        string STKNo = "";
                        bool bNew = true;

                        string sbranch = BranchCode;
                        string scompany = CompanyCode;
                        string periodbag = ctx.GnMstCoProfileSpares.Find(scompany, sbranch).PeriodBeg.ToString("yy");
                        string curruser = CurrentUser.UserId;

                        int iprm = Convert.ToInt16(param.ParamValue);
                        ctx.Configuration.AutoDetectChangesEnabled = false;
                        var lst = new List<SpTrnStockTakingTemp>();
                        rowFinal.ForEach(x =>
                        {
                            dtltemp = new SpTrnStockTakingTemp();
                            DateTime curdt = ctx.CurrentTime;
                            if (codetype == "0")
                            {
                                if (iSeq + 1 > iprm)
                                {
                                    iSeq = 0;
                                    iSeq++;
                                    dtltemp.SeqNo = iSeq;
                                    STKNo = GetNewDocumentNo(GenDocNo, "STK", periodbag);                                   
                                }
                                else
                                {
                                    iSeq = iSeq + 1;
                                    dtltemp.SeqNo = iSeq;
                                }

                                if (bNew)
                                {
                                    STKNo = GetNewDocumentNo(GenDocNo, "STK", periodbag);
                                    if (STKNo.EndsWith("X"))
                                    {
                                        var msg = string.Format(ctx.SysMsgs.Find(SysMessages.MSG_5046).MessageCaption, "Stock Taking (STK)");
                                        throw new Exception(msg);
                                    }
                                    bNew = false;
                                }
                                else
                                    dtltemp.STNo = STKNo;
                            }
                            else if (codetype == "1")
                            {
                                STKNo = GetNewDocumentNo(GenDocNo, "STK", periodbag);
                                dtltemp.SeqNo = 1;
                            }

                            dtltemp.STNo = STKNo;
                            dtltemp.BranchCode = sbranch;
                            dtltemp.CompanyCode = scompany;
                            dtltemp.STHdrNo = hdr.STHdrNo;
                            dtltemp.PartNo = x.PartNo.ToString() ?? "";
                            dtltemp.LocationCode = x.LocationCode;
                            dtltemp.isMainLocation = x.isMainLocation;
                            dtltemp.OnHandQty = x.OnHand;
                            dtltemp.MovingCode = x.MovingCode;
                            dtltemp.EntryDate = curdt;
                            dtltemp.Status = "0";
                            dtltemp.CreatedBy = curruser;
                            dtltemp.CreatedDate = curdt;
                            dtltemp.LastUpdatedBy = curruser;
                            dtltemp.LastUpdatedDate = curdt;
                            lst.Add(dtltemp);
                            ctx.SpTrnStockTakingTemps.Add(dtltemp);

                            //Debug.WriteLine(iSeq);
                        });

                    }

                
                    ctx.SaveChanges();

            }

            return true;
        }
        

        /// <summary>
        /// Proses Stock Taking with blank condition. 
        /// </summary>
        /// <param name="GenDocNo">Function for Generate Document Number</param>
        /// <param name="qty">Qty for Inventory Tag/form</param>
        /// <param name="hdr">Stock Taking Header item</param>
        /// <returns>Process Success or Failed</returns>
        public bool ProsesBlank(Func<string, DateTime, string> GenDocNo, decimal qty,out SpTrnStockTakingHdr hdr)
        {
            string branchcode = BranchCode;
            string companycode = CompanyCode;
            string[] rstatus = new string[] { "0", "1" };            

            hdr = ctx.SpTrnStockTakingHdrs
                      .Where(x => x.BranchCode == branchcode
                                && x.CompanyCode == companycode
                                && rstatus.Contains(x.Status))
                      .FirstOrDefault();
            if (hdr == null)
                return false;
            

            var param = Convert.ToInt16(ctx.SysParameters.Where(y => y.ParamId == "FORMQTY").SingleOrDefault().ParamValue);
            string periodbag = ctx.GnMstCoProfileSpares.Find(companycode, branchcode).PeriodBeg.ToString("yy");
            int quantityTag = 0;
            string STKNo = "";
            string userid = CurrentUser.UserId;            
            bool bNew = true;
            int iSeq = 0;            


            if (hdr.TypeCode == "1")
                quantityTag = Convert.ToInt32(qty);
            else if (hdr.TypeCode == "0")
                quantityTag = param * Convert.ToInt32(qty);
            DateTime curdt = ctx.CurrentTime;
            var lsttemp = new List<SpTrnStockTakingTemp>();
                for (int x = 0; x < quantityTag; x++)
                {
                    var recordTemp = new SpTrnStockTakingTemp();
                    //Form
                    if (hdr.TypeCode == "0")
                    {
                        if (iSeq + 1 > param)
                        {
                            // Reset sequence.
                            iSeq = 0;

                            iSeq = iSeq + 1;
                            recordTemp.SeqNo = iSeq;
                            STKNo = GetNewDocumentNo(GenDocNo, "STK", periodbag);
                            recordTemp.STNo = STKNo;

                        }
                        else
                        {
                            iSeq = iSeq + 1;
                            recordTemp.SeqNo = iSeq;
                        }

                        if (bNew)
                        {
                            STKNo = GetNewDocumentNo(GenDocNo, "STK", periodbag);
                            recordTemp.STNo = STKNo;
                            bNew = false;
                        }
                        else
                            recordTemp.STNo = STKNo;
                    }
                    //Tag
                    else
                    {
                        STKNo = GetNewDocumentNo(GenDocNo, "STK", periodbag);
                        recordTemp.STNo = STKNo;
                        recordTemp.SeqNo = 1;
                    }
                    recordTemp.CompanyCode = companycode;
                    recordTemp.BranchCode = branchcode;
                    recordTemp.STHdrNo = hdr.STHdrNo;
                    recordTemp.EntryDate = curdt;
                    recordTemp.Status = "1";
                    recordTemp.isMainLocation = false;
                    recordTemp.PartNo = "";
                    recordTemp.LocationCode = "";
                    recordTemp.MovingCode = "";
                    recordTemp.OnHandQty = 0;

                    recordTemp.CreatedBy = userid;
                    recordTemp.CreatedDate = curdt;
                    recordTemp.LastUpdatedBy = userid;
                    recordTemp.LastUpdatedDate = curdt;
                    ctx.SpTrnStockTakingTemps.Add(recordTemp);
                    lsttemp.Add(recordTemp);
                }
                ctx.SaveChanges();                
            
            return true;
        }

        /// <summary>
        /// Check item location on each location and return total location thats item is found
        /// </summary>
        /// <param name="record">Item Location </param>
        /// <param name="condition">Condition Code of Stock Taking </param>
        /// <param name="locationCode">Location Code </param>
        /// <returns>total location found</returns>
        private int CheckLocation(SpMstItemLoc record, string condition, ref ArrayList locationCode)
        {

            int iloop = 0;
            if ((condition == "0" || condition == "1") && record.LocationCode == "")
            {
                iloop++;
                locationCode.Add(record.LocationCode);
            }

            if (!string.IsNullOrEmpty(record.LocationCode))
            {
                iloop++;
                locationCode.Add(record.LocationCode);
            }

            if (!string.IsNullOrEmpty(record.LocationSub1))
            {
                iloop++;
                locationCode.Add(record.LocationSub1);
            }

            if (!string.IsNullOrEmpty(record.LocationSub2))
            {
                iloop++;
                locationCode.Add(record.LocationSub2);
            }
            if (!string.IsNullOrEmpty(record.LocationSub3))
            {
                iloop++;
                locationCode.Add(record.LocationSub3);
            }
            if (!string.IsNullOrEmpty(record.LocationSub4))
            {
                iloop++;
                locationCode.Add(record.LocationSub4);
            }
            if (!string.IsNullOrEmpty(record.LocationSub5))
            {
                iloop++;
                locationCode.Add(record.LocationSub5);
            }
            if (!string.IsNullOrEmpty(record.LocationSub6))
            {
                iloop++;
                locationCode.Add(record.LocationSub6);
            }

            return iloop;
        }

        /// <summary>
        /// Generate Text Info 
        /// </summary>
        /// <param name="STHdrNo">Current STHDRNo</param>
        /// <param name="stFirst">First STKNO</param>
        /// <param name="stLast">Last STLNO</param>
        /// <returns>
        /// Information for label info
        /// </returns>
        public string GetLabelInfo(string STHdrNo, out string stFirst, out string stLast)
        {

            stFirst = "";
            stLast = "";
            string lblinfo = "";

            var temp = ctx.SpTrnStockTakingTemps
                       .Where(x => x.BranchCode == BranchCode 
                                 && x.CompanyCode == CompanyCode
                                 && x.STHdrNo == STHdrNo 
                                 //&& x.PartNo != ""
                                 ).AsQueryable();

            var tempfirst = temp.OrderBy(x => x.STNo).FirstOrDefault();
            if (tempfirst != null)
            {
                stFirst = tempfirst.STNo;
            }
            else
            {
                lblinfo = "Kondisi Stock Taking Blank";
                return lblinfo;
            }

            var templst = temp.OrderByDescending(x => x.STNo).FirstOrDefault();
            if (templst != null)
            {
                stLast = templst.STNo;
            }

            if (stFirst == stLast)
                lblinfo = "Nomor Stock Taking : " + stFirst;
            else
                lblinfo = "Nomor Stock Taking : " + stFirst + " s/d " + stLast;

            return lblinfo;
        }
        
        /// <summary>
        /// Generate Document Number
        /// </summary>
        /// <param name="callback">Function for Generate Document Number</param>
        /// <param name="doctype">Type of Document</param>
        /// <param name="periodbag">Transaction Period</param>
        /// <returns>
        /// New Document Number.
        /// </returns>
        private string GetNewDocumentNo(Func<string, DateTime, string> callback, string doctype,string periodbag)
        {
            var curdt = ctx.CurrentTime;
            string docno = callback(doctype,curdt);

            if (periodbag != docno.Split('/')[1])
            {
                docno = doctype+"/" + curdt.ToString("yy") + "/XXXXXX";
            }
            return docno;
        }

        /// <summary>
        /// Get Current Stock Taking STHdr No
        /// </summary>
        /// <returns>Current Stock Taking STHdr No</returns>
        public string GetNoStockTakingReset()
        {
            string[] rstatus = new string[] { "0", "1" };
            var hdr = ctx.SpTrnStockTakingHdrs
                    .Where(x => x.CompanyCode == CompanyCode
                        && x.BranchCode == BranchCode
                        && rstatus.Contains(x.Status)
                        ).SingleOrDefault();           
                return hdr==null?"":hdr.STHdrNo;
        }
    }
}
