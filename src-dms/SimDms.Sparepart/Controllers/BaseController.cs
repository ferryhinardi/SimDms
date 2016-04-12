using GeLang;
using SimDms.Common;
using SimDms.Common.Models;
using SimDms.Sparepart.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TracerX;
using System.Text;
using ClosedXML.Excel;
using System.Data.SqlClient;

namespace SimDms.Sparepart.Controllers
{
    public class BaseController : Controller
    {

        protected DataContext ctx;
        protected MDContext ctxMD;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            ctx = new DataContext(MyHelpers.GetConnString("DataContext"));
            ctxMD = new MDContext(MyHelpers.GetConnString("MDContext"));

            if (User != null && User.Identity.IsAuthenticated)
            {
                ctx.CurrentUser = User.Identity.Name;
            }
            else
            {
                ctx.CurrentUser = "Guest";
            }
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonNetResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior
            };
        }

        protected string HtmlRender(string jsname)
        {
            return string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/sp/"), jsname);
        }

        protected string HtmlRender(string id, string jsname)
        {
            var jshtml = "";
            if (!string.IsNullOrWhiteSpace(jsname))
            {
                jshtml = string.Format(@"<script src=""{0}{1}"" type=""text/javascript""><script>", Url.Content("~/assets/js/app/sp/"), jsname);
            }
            return string.Format(@"<div id=""{0}"" ></div>", id) + jshtml;
        }

        protected List<Dictionary<string, object>> GetJson(DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row = null;

            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName.Trim(), dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }

        protected SysUser CurrentUser
        {
            get
            {
                var user = ctx.SysUsers.Find(User.Identity.Name);
                user.CoProfile = ctx.CoProfiles.Find(user.CompanyCode, user.BranchCode);
                return user;
            }
            set { }
        }

        protected SysUser CurrentUserByUname(string username)
        {
            return ctx.SysUsers.Find(User.Identity.Name);
        }

        protected string CompanyCode
        {
            get
            {
                return CurrentUser.CompanyCode;
            }
        }

        protected string CompanyName
        {
            get
            {
                return ctx.OrganizationHdrs.Find(CurrentUser.CompanyCode).CompanyName;
            }
        }

        protected string BranchCode
        {
            get
            {
                return CurrentUser.BranchCode;
            }
        }

        protected string BranchName
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).CompanyName;
            }
        }

        protected string ProductType
        {
            get
            {
                return ctx.CoProfiles.Find(CompanyCode, BranchCode).ProductType;
            }
        }

        protected string ProfitCenter
        {
            get
            {
                var itm = ctx.SysUserViews.Where(x => x.UserId == CurrentUser.UserId)
                                .FirstOrDefault();

                bool IsAdmin = false;
                if (itm != null)
                {
                    IsAdmin = itm.RoleId == "ADMIN";
                }   
                    
                string profitCenter = "300";
                if (!IsAdmin)
                {
                    string s = "000";
                    var x = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
                    if (x != null) s = x.ProfitCenter;
                    return s;
                }
                else
                {
                    return profitCenter;
                }
            }
        }

        protected string ProfitCenterName
        {
            get
            {
                string name = "";
                name = ctx.LookUpDtls.Find(CompanyCode, "PFCN", ProfitCenter).LookUpValueName;
                return name;
            }
        }

        protected string TypeOfGoods
        {
            get
            {
                return CurrentUser.TypeOfGoods;
            }
        }

        protected string defaultSpParam(string sp, string id = null, bool branch = true)
        {
            string s = " '" + CompanyCode + "'";

            if (branch)
            {
                s += ",'" + BranchCode + "'";
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                s += ",'" + id + "'";
            }
            else
            {
                string p = Request["sSearch"].ToString();

                if (!string.IsNullOrWhiteSpace(p))
                {
                    s += ",'" + p + "'";
                }
            }

            return sp + s;
        }

        public DataResult<T> eXecSp<T>(string spName, string id = null, bool branch = true) where T : class
        {
            string s = spName + " '" + CompanyCode + "'";

            if (branch)
            {
                s += ",'" + BranchCode + "'";
            }

            if (!string.IsNullOrWhiteSpace(id))
            {
                s += "," + id;
            }
            else
            {

                if (Request["sSearch"] != null)
                {
                    string p = Request["sSearch"].ToString();

                    if (!string.IsNullOrWhiteSpace(p))
                    {
                        s += ",'" + p + "'";
                    }
                }
            }

            var data = ctx.Database.SqlQuery<T>(s).AsQueryable();
            return DataTableSp<T>.Parse(data, Request);
        }

        public DataResult<T> eXecSQL<T>(string SQL) where T : class
        {
            var data = ctx.Database.SqlQuery<T>(SQL).AsQueryable();
            return DataTableSp<T>.Parse(data, Request);
        }

        public DataResult<T> eXecScalar<T>(string SQL) where T : class
        {

            var data = ctx.Database.SqlQuery<T>(SQL).AsQueryable();
            return DataTableSp<T>.Parse(data, Request);
        }

        protected string GetNewDocumentNo(string doctype, DateTime transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
            return result.First();
        }

        protected string GetNewDocumentNoHpp(string doctype, string transdate)
        {
            var sql = "uspfn_GnDocumentGetNew {0}, {1}, {2}, {3}, {4}";
            var result = ctx.Database.SqlQuery<string>(sql, CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
            return result.First();
        }

        protected string DealerCode()
        {
            var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
            if (result != null)
            {
                if (result.CompanyMD == CompanyCode && result.BranchMD == BranchCode) { return "MD"; }
                else { return "SD"; }
            }
            else return "MD";
        }

        protected string CompanyMD
        {
            get
            {
                var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if(result != null){
                    return result.CompanyMD;
                }
                else return CompanyCode;
            }
        }

        protected string BranchMD
        {
            get
            {
                var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (result != null)
                {
                    return result.BranchMD;
                }
                else return BranchCode;
            }
        }

        protected string WarehouseMD
        {
            get
            {
                var result = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (result != null)
                {
                    return result.WarehouseMD;
                }
                else return "00";
            }
        }


        protected string DateTransValidation(DateTime date)
        {
            var user = CurrentUser.UserId;
            var currDate = DateTime.Now.Date;
            var errMsg1 = string.Format(ctx.SysMsgs.Find("5006").MessageCaption, "Tanggal Transaksi", "Periode Transaksi");//string.Format("{0} tidak sesuai dengan {1}", "Tanggal transaksi", "periode transaksi");
            var errMsg2 = string.Format(ctx.SysMsgs.Find("5006").MessageCaption, "Tanggal Transaksi", "Tanggal Server");//string.Format("{0} tidak sesuai dengan {1}", "Tanggal Transaksi", "Tanggal Server");
            var errMsg3 = "Periode sedang di locked";//string.Format("Periode sedang di locked");
            var errMsg4 = "Tanggal Transaksi lebih kecil dari tanggal [TransDate]";//string.Format("Tanggal Transaksi lebih kecil dari tanggal [TransDate]");
            var msg = "";
            try
            {
                 string pcentre = ProfitCenter;
                var oProfCenter = ctx.SysUserProfitCenters.Find(user);
                //300 For Sparepart
                //if (oProfCenter.ProfitCenter.Equals("300"))
                if (pcentre.Equals("300"))
                {
                    var oSpare = ctx.GnMstCoProfileSpares.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode);
                    if (oSpare != null)
                    {
                        if (oSpare.TransDate.Equals(DBNull.Value) || oSpare.TransDate < new DateTime(1900, 1, 2)) oSpare.TransDate = DateTime.Now;
                        if (date.Date >= oSpare.PeriodBeg.Date && date.Date <= oSpare.PeriodEnd.Date)
                        {
                            if (date.Date <= currDate)
                            {
                                if (date >= oSpare.TransDate.Date)
                                {
                                    if (oSpare.isLocked == true)
                                    {
                                        msg = errMsg3;
                                    }
                                }
                                else
                                {
                                    errMsg4 = errMsg4.Replace("[TransDate]", oSpare.TransDate.ToString("dd-MMM-yyyy"));
                                    msg = errMsg4;
                                }
                            }
                            else
                            {
                                msg = errMsg2;
                            }
                        }
                        else
                        {
                            msg = errMsg1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return msg;
        }

        protected void UpdateStock(string partno, string whcode, decimal onhand, decimal alloaction, decimal backorder, decimal reserved, string salesType)
        {
            try
            {
                bool md = DealerCode() == "MD";
                //spMstItem oItem = ctxMD.spMstItems.Find(CompanyMD, BranchMD, partno);
                var Item = @"select * from " + GetDbMD() + @"..spMstItems where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + partno + "'";
                spMstItem oItem = ctx.Database.SqlQuery<spMstItem>(Item).FirstOrDefault();                       
                if (oItem != null)
                {
                    //SpMstItemLoc oItemLoc = ctxMD.SpMstItemLocs.Find(CompanyMD, BranchMD, partno, whcode);
                    var ItemLoc = @"select * from " + GetDbMD() + @"..SpMstItemLoc where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + partno + "' and WarehouseCode ='" + whcode + "'";
                    SpMstItemLoc oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(ItemLoc).FirstOrDefault();
                    if (oItemLoc != null)
                    {
                        //if (Math.Abs(onhand) > 0)
                        //{
                        oItemLoc.OnHand += onhand;
                        oItem.OnHand += onhand;

                        if (oItemLoc.OnHand < 0)
                        {
                            throw new Exception(string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.OnHand));
                        }

                        if (oItem.OnHand < 0)
                        {
                            throw new Exception(string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.OnHand));
                        }
                        //}

                        //if (Math.Abs(alloaction) > 0)
                        //{
                        if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
                        {
                            oItemLoc.AllocationSP += alloaction;
                            oItem.AllocationSP += alloaction;

                            if (oItemLoc.AllocationSP < 0)
                            {
                                throw new Exception(string.Format("AllocationSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSP));
                            }
                            if (oItem.AllocationSP < 0)
                            {
                                throw new Exception(string.Format("AllocationSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSP));
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "2")
                        {
                            oItemLoc.AllocationSR += alloaction;
                            oItem.AllocationSR += alloaction;

                            if (oItemLoc.AllocationSR < 0)
                            {
                                throw new Exception(string.Format("AllocationSR untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSR));
                            }

                            if (oItem.AllocationSR < 0)
                            {
                                throw new Exception(string.Format("AllocationSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItem.AllocationSR));
                            }
                        }

                        if (!string.IsNullOrEmpty(salesType) && salesType == "3")
                        {
                            oItemLoc.AllocationSL += alloaction;
                            oItem.AllocationSL += alloaction;

                            if (oItemLoc.AllocationSL < 0)
                            {
                                throw new Exception(string.Format("AllocationSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSR));
                            }
                            if (oItem.AllocationSL < 0)
                            {
                                throw new Exception(string.Format("AllocationSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.AllocationSR));
                            }
                        }
                        //}

                        if (Math.Abs(backorder) > 0)
                        {
                            if (!string.IsNullOrEmpty(salesType) && (salesType == "0" || salesType == "1"))
                            {
                                oItemLoc.BackOrderSP += backorder;
                                oItem.BackOrderSP += backorder;

                                if (oItemLoc.BackOrderSP < 0)
                                {
                                    throw new Exception(string.Format("BackOrderSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSP));
                                }
                                if (oItem.BackOrderSP < 0)
                                {
                                    throw new Exception(string.Format("BackOrderSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSP));
                                }
                            }

                            if (!string.IsNullOrEmpty(salesType) && (salesType == "2"))
                            {
                                oItemLoc.BackOrderSR += backorder;
                                oItem.BackOrderSR += backorder;

                                if (oItemLoc.BackOrderSR < 0)
                                {
                                    throw new Exception(string.Format("BackOrderSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSP));
                                }

                                if (oItem.BackOrderSP < 0)
                                {
                                    throw new Exception(string.Format("BackOrderSR untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSR));
                                }
                            }

                            if (!string.IsNullOrEmpty(salesType) && (salesType == "3"))
                            {
                                oItemLoc.BackOrderSL += backorder;
                                oItem.BackOrderSL += backorder;

                                if (oItemLoc.BackOrderSL < 0)
                                {
                                    throw new Exception(string.Format("BackOrderSL untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSL));
                                }

                                if (oItem.BackOrderSL < 0)
                                {
                                    throw new Exception(string.Format("BackOrderSL untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.BackOrderSL));
                                }
                            }

                            if (Math.Abs(reserved) > 0)
                            {
                                oItemLoc.ReservedSP += reserved;
                                oItem.ReservedSP += reserved;

                                if (oItemLoc.ReservedSP < 0)
                                {
                                    throw new Exception(string.Format("ReservedSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP));
                                }
                                if (oItem.ReservedSP < 0)
                                {
                                    throw new Exception(string.Format("ReservedSP untuk Part {0}, Item = {1}, transaksi tidak bisa lanjut", oItem.PartNo, oItem.ReservedSP));
                                }
                            }

                        }
                        //oItemLoc.LastUpdateDate = DateTime.Now;
                        //oItemLoc.LastUpdateBy = CurrentUser.UserId;
                        //oItem.LastUpdateDate = DateTime.Now;
                        //oItem.LastUpdateBy = CurrentUser.UserId;
                        //ctxMD.SaveChanges();

                        var sqlUpdateItemLocMD = string.Format(@"UPDATE {0}..SpMstItemLoc SET 
                        OnHand ={1}
                        ,AllocationSP ={2}
                        ,AllocationSR ={3}
                        ,AllocationSL ={4}
                        ,BackOrderSP ={5}
                        ,BackOrderSR ={6}
                        ,BackOrderSL ={7}
                        ,ReservedSP ={8}
                        ,LastUpdateDate ='{9}'
                        ,LastUpdateBy ='{10}' 
                    WHERE CompanyCode='{11}' AND BranchCode ='{12}' AND PartNo ='{13}' AND WarehouseCode ='{14}'",
                        GetDbMD()
                        , oItemLoc.OnHand
                        , oItemLoc.AllocationSP
                        , oItemLoc.AllocationSR
                        , oItemLoc.AllocationSL
                        , oItemLoc.BackOrderSP
                        , oItemLoc.BackOrderSR
                        , oItemLoc.BackOrderSL
                        , oItemLoc.ReservedSP
                        , DateTime.Now
                        , CurrentUser.UserId
                        , CompanyMD, BranchMD, partno, whcode
                    );
                        ctx.Database.ExecuteSqlCommand(sqlUpdateItemLocMD);
                        ctx.SaveChanges();

                        var sqlUpdateItemMD = string.Format(@"UPDATE {0}..spMstItems SET 
                        OnHand ={1}
                        ,AllocationSP ={2}
                        ,AllocationSR ={3}
                        ,AllocationSL ={4}
                        ,BackOrderSP ={5}
                        ,BackOrderSR ={6}
                        ,BackOrderSL ={7}
                        ,ReservedSP ={8}
                        ,LastUpdateDate ='{9}'
                        ,LastUpdateBy ='{10}' 
                    WHERE CompanyCode='{11}' AND BranchCode ='{12}' AND PartNo ='{13}'",
                            GetDbMD()
                            , oItem.OnHand
                            , oItem.AllocationSP
                            , oItem.AllocationSR
                            , oItem.AllocationSL
                            , oItem.BackOrderSP
                            , oItem.BackOrderSR
                            , oItem.BackOrderSL
                            , oItem.ReservedSP
                            , DateTime.Now
                            , CurrentUser.UserId
                            , CompanyMD, BranchMD, partno
                        );
                        ctx.Database.ExecuteSqlCommand(sqlUpdateItemMD);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada Fungsi UpdateStock, Message=" + ex.Message.ToString());
            }
        }

        protected void UpdateStockWarehouse(string partno, string whcode, decimal onhand, decimal allocation, decimal backorder, decimal reserved)
        {
            try
            {
                var sqlItemLocFrom = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                            GetDbMD(), CompanyMD, BranchMD, partno, whcode);
                SpMstItemLoc oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLocFrom).FirstOrDefault();

                if (oItemLoc != null)
                {
                    //TODO : Tambahkan check result untuk yang hasilnya negatif
                    if (Math.Abs(onhand) > 0)
                    {
                        oItemLoc.OnHand += onhand;

                        // Tambahkan check result untuk yang ItemLoc negatif
                        if (oItemLoc.OnHand < 0)
                        {
                            throw new Exception(string.Format("OnHand untuk Part = {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.OnHand));
                        }
                    }

                    if (Math.Abs(allocation) > 0)
                    {
                        oItemLoc.AllocationSP += allocation;

                        // Tambahkan check result untuk yang ItemLoc negatif
                        if (oItemLoc.AllocationSP < 0)
                        {
                             throw new Exception(string.Format("AllocationSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.AllocationSP));
                        }
                    }

                    if (Math.Abs(backorder) > 0)
                    {
                        oItemLoc.BackOrderSP += backorder;

                        // Tambahkan check result untuk yang ItemLoc negatif
                        if (oItemLoc.BackOrderSP < 0)
                        {
                            throw new Exception(string.Format("BackOrderSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.BackOrderSP));
                        }
                    }

                    if (Math.Abs(reserved) > 0)
                    {
                        oItemLoc.ReservedSP += reserved;

                        // Tambahkan check result untuk yang ItemLoc negatif
                        if (oItemLoc.ReservedSP < 0)
                        {
                            throw new Exception(string.Format("ReservedSP untuk Part {0}, ItemLoc = {1}, transaksi tidak bisa lanjut", oItemLoc.PartNo, oItemLoc.ReservedSP));
                        }
                    }
                    oItemLoc.LastUpdateDate = DateTime.Now;
                    oItemLoc.LastUpdateBy = CurrentUser.UserId;

                    var sqlUpdateItemLocMD = string.Format(@"UPDATE {0}..SpMstItemLoc SET 
                        OnHand ={1}
                        ,AllocationSP ={2}
                        ,BackOrderSP ={3}
                        ,ReservedSP ={4}
                        ,LastUpdateDate ='{5}'
                        ,LastUpdateBy ='{6}' 
                    WHERE CompanyCode='{7}' AND BranchCode ='{8}' AND PartNo ='{9}' AND WarehouseCode ='{10}'",
                        GetDbMD()
                        , oItemLoc.OnHand
                        , oItemLoc.AllocationSP
                        , oItemLoc.BackOrderSP
                        , oItemLoc.ReservedSP
                        , DateTime.Now
                        , CurrentUser.UserId
                        , CompanyMD, BranchMD, partno, whcode
                    );
                    ctx.Database.ExecuteSqlCommand(sqlUpdateItemLocMD);
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada Fungsi UpdateStockWarehouse, Message=" + ex.Message.ToString());
            }
        }

        protected void MovementLog(string docno, DateTime docdate, string partno, string whcode, string signcode, string subsigncode, decimal qty)
        {
            try
            {
                bool md = DealerCode() == "MD";

                var oIMovement = new SpTrnIMovement();
                var oSDMovement = new SvSDMovement();

                //IF "SD" get data from MD and Save MovementLog to SD
                //spMstItem oItems = ctxMD.spMstItems.Find(CompanyMD, BranchMD, partno);
                var Item = @"select * from " + GetDbMD() + @"..spMstItems where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + partno + "'";
                spMstItem oItems = ctx.Database.SqlQuery<spMstItem>(Item).FirstOrDefault();

                spMstItemPrice oItemPrice = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, partno);
                //var ItemPrice = @"select * from " + GetDbMD() + @"..spMstItemPrice where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + partno + "'";
                //spMstItemPrice oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(ItemPrice).FirstOrDefault();

                //SpMstItemLoc oItemLoc = ctxMD.SpMstItemLocs.Find(CompanyMD, BranchMD, partno, whcode);
                var ItemLoc = @"select * from " + GetDbMD() + @"..SpMstItemLoc where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and PartNo ='" + partno + "' and WarehouseCode ='" + whcode + "'";
                SpMstItemLoc oItemLoc = ctx.Database.SqlQuery<SpMstItemLoc>(ItemLoc).FirstOrDefault();

                if (oItemLoc != null && oItemPrice != null && oItems != null)
                {
                    oIMovement.CompanyCode = CompanyCode;
                    oIMovement.BranchCode = BranchCode;
                    oIMovement.DocNo = docno;
                    oIMovement.DocDate = docdate;
                    oIMovement.WarehouseCode = oItemLoc.WarehouseCode;
                    oIMovement.LocationCode = oItemLoc.LocationCode;
                    oIMovement.PartNo = oItemLoc.PartNo;
                    oIMovement.SignCode = signcode;
                    oIMovement.SubSignCode = subsigncode;
                    oIMovement.Qty = qty;
                    oIMovement.Price = oItemPrice.RetailPrice;
                    oIMovement.CostPrice = oItemPrice.CostPrice;
                    oIMovement.ABCClass = oItems.ABCClass;
                    oIMovement.MovingCode = oItems.MovingCode;
                    oIMovement.ProductType = oItems.ProductType;
                    oIMovement.PartCategory = oItems.PartCategory;
                    oIMovement.CreatedBy = CurrentUser.UserId;
                    oIMovement.CreatedDate = DateTime.Now;

                    ctx.SpTrnIMovements.Add(oIMovement);
                    ctx.SaveChanges();

                    if (!md)
                    {
                        int partSeq = 0;
                        //var listSDMovement = ctxMD.SvSDMovements.Where(m => m.CompanyCode == CompanyMD && m.BranchCode == BranchMD && m.DocNo == docno).ToList();
                        var list = @"select * from " + GetDbMD() + @"..svSDMovement where CompanyCode='" + CompanyMD + "' AND BranchCode='" + BranchMD + "' and DocNo ='" + docno + "'";
                        var listSDMovement = ctx.Database.SqlQuery<SvSDMovement>(list).ToList();
                        if (listSDMovement.Count == 0)
                        {
                            partSeq += 1;
                        }
                        else
                        {
                            partSeq = listSDMovement.Max(m => m.PartSeq) + 1;
                        }
                        var sql = @"INSERT INTO " + GetDbMD() + @"..svSDMovement(
                                CompanyCode
                                ,BranchCode
                                ,DocNo
                                ,DocDate
                                ,PartNo
                                ,PartSeq
                                ,WarehouseCode 
                                ,Qty
                                ,QtyOrder 
                                ,RetailPriceInclTaxMD
                                ,RetailPriceMD
                                ,CostPriceMD
                                ,QtyFlag
                                ,DiscPct
                                ,CostPrice
                                ,RetailPrice
                                ,TypeOfGoods
                                ,CompanyMD
                                ,BranchMD
                                ,WarehouseMD
                                ,Status
                                ,ProcessStatus 
                                ,ProductType
                                ,ProfitCenterCode
                                ,ProcessDate
                                ,CreatedBy
                                ,CreatedDate 
                                ,LastUpdateBy
                                ,LastUpdateDate
                            )
                            VALUES(
                                '" + CompanyCode + "'"
                                + ",'" + BranchCode + "'"
                                + ",'" +  docno + "'"
                                + ",'" +  docdate + "'"
                                + ",'" + oItemLoc.PartNo + "'"
                                + ",'" +  partSeq + "'"
                                + ",'" + oItemLoc.WarehouseCode + "'"
                                + "," + qty + ""
                                + "," + 0 + ""
                                + "," + 0 + ""
                                + "," + 0 + ""
                                + "," + 0 + ""
                                + "," + 0 + ""
                                + "," + 0 + ""
                                + "," + oItemPrice.CostPrice.Value + ""
                                + "," + oItemPrice.RetailPrice.Value + ""
                                + ",'" + TypeOfGoods + "'"
                                + ",'" + CompanyMD + "'"
                                + ",'" + BranchMD + "'"
                                + ",'" + WarehouseMD + "'"
                                + ",'" + "0" + "'"
                                + ",'" + "0" + "'"
                                + ",'" + ProductType + "'"
                                + ",'" + "300" + "'"
                                + ",'" + DateTime.Now + "'"
                                + ",'" + CurrentUser.UserId + "'"
                                + ",'" + DateTime.Now + "'"
                                + ",'" + CurrentUser.UserId + "'"
                                + ",'" + DateTime.Now + "')";

                        ctx.Database.ExecuteSqlCommand(sql);
                        ctx.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada fungsi BaseController.MovementLog, Message=" + ex.Message.ToString());
            }
        }

        protected bool MovementLog(string docno, DateTime docdate, string partno, string whcodeTo, string whcodeFrom, bool caseAdj, string opAdjust, decimal qty)
        {
            string signCode = "OUT";
            string subSignCode = "";
            bool result = false;
            
            var sqlItem = string.Format("SELECT * FROM {0}..spMstItems WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                GetDbMD(), CompanyMD, BranchMD, partno);
            spMstItem oItems = ctx.Database.SqlQuery<spMstItem>(sqlItem).FirstOrDefault();

            var sqlItemPrice = string.Format("SELECT * FROM {0}..spMstItemPrice WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}'",
                        GetDbMD(), CompanyMD, BranchMD, partno);
            spMstItemPrice oItemPrice = ctx.Database.SqlQuery<spMstItemPrice>(sqlItemPrice).FirstOrDefault();

            var sqlItemLocFrom = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                GetDbMD(), CompanyMD, BranchMD, partno, whcodeFrom);
            SpMstItemLoc oItemLocFrom = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLocFrom).FirstOrDefault();
            
            SpMstItemLoc oItemLocTo = new SpMstItemLoc();

            //TO DO : Nilai dari subsigncode dan signcode perlu dimasukkan dalam commonBLL
            if (caseAdj)
            {
                subSignCode = GnMstDocumentConstant.ADJ;
                if (opAdjust == "+")
                    signCode = "IN";
            }
            else
                subSignCode = GnMstDocumentConstant.WTR;

            // TO DO : Insert to Item Movement record
            if (whcodeTo != "")
            {
                SpTrnIMovement oIMovement = new SpTrnIMovement();

                var sqlItemLocTo = string.Format("SELECT * FROM {0}..SpMstItemLoc WHERE CompanyCode='{1}' AND BranchCode ='{2}' AND PartNo ='{3}' AND WarehouseCode ='{4}'",
                    GetDbMD(), CompanyMD, BranchMD, partno, whcodeTo);

                oItemLocTo = ctx.Database.SqlQuery<SpMstItemLoc>(sqlItemLocTo).FirstOrDefault(); 
                if (oItemLocTo != null)
                {
                    oIMovement.CompanyCode = CompanyCode;
                    oIMovement.BranchCode = BranchCode;
                    oIMovement.DocNo = docno;
                    oIMovement.DocDate = docdate;
                    oIMovement.CreatedDate = DateTime.Now;

                    oIMovement.WarehouseCode = oItemLocTo.WarehouseCode;
                    oIMovement.LocationCode = oItemLocTo.LocationCode;
                    oIMovement.PartNo = oItemLocTo.PartNo;
                    oIMovement.SignCode = "IN";
                    oIMovement.SubSignCode = subSignCode;
                    oIMovement.Qty = qty;
                    oIMovement.Price = oItemPrice.RetailPrice;
                    oIMovement.CostPrice = oItemPrice.CostPrice;
                    oIMovement.ABCClass = oItems.ABCClass;
                    oIMovement.MovingCode = oItems.MovingCode;
                    oIMovement.ProductType = oItems.ProductType;
                    oIMovement.PartCategory = oItems.PartCategory;
                    oIMovement.CreatedBy = CurrentUser.UserId;

                    ctx.SpTrnIMovements.Add(oIMovement);
                    result = ctx.SaveChanges() > 0;
                }
                else
                    result = false;
            }

            if (oItemLocFrom != null && oItemPrice != null && oItems != null)
            {
                SpTrnIMovement oIMovement = new SpTrnIMovement();

                oIMovement.CompanyCode = CompanyCode;
                oIMovement.BranchCode = BranchCode;
                oIMovement.DocNo = docno;
                oIMovement.DocDate = docdate;
                //oIMovement.CreatedDate = DmsTime.Now.AddMinutes(1);
                oIMovement.CreatedDate = DateTime.Now;

                oIMovement.WarehouseCode = oItemLocFrom.WarehouseCode;
                oIMovement.LocationCode = oItemLocFrom.LocationCode;
                oIMovement.PartNo = oItemLocFrom.PartNo;
                oIMovement.SignCode = signCode;
                oIMovement.SubSignCode = subSignCode;
                oIMovement.Qty = qty;
                oIMovement.Price = oItemPrice.RetailPrice;
                oIMovement.CostPrice = oItemPrice.CostPrice;
                oIMovement.ABCClass = oItems.ABCClass;
                oIMovement.MovingCode = oItems.MovingCode;
                oIMovement.ProductType = oItems.ProductType;
                oIMovement.PartCategory = oItems.PartCategory;
                oIMovement.CreatedBy = CurrentUser.UserId;

                ctx.SpTrnIMovements.Add(oIMovement);
                result = ctx.SaveChanges() > 0;
            }
            return result;
        }

        protected string isOverdueOrderExist(string customerCode, ProfitCenter recCustomer)
        {
            string returnMsg = "";
            var recLookUpDtl = ctx.LookUpDtls.Find(CompanyCode, "TOPC", recCustomer.TOPCode);
            var islinktofinance = ctx.CoProfiles.Find(CompanyCode, BranchCode).IsLinkToFinance;
            if (recLookUpDtl != null)
            {
                if (recLookUpDtl.ParaValue != "0")
                {
                    if (islinktofinance.Value && !recCustomer.isOverDueAllowed.Value)
                    {
                        if (isOverdueOrder(customerCode))
                        {
                            returnMsg = "Anda memiliki transaksi yang telah jatuh tempo, silahkan selesaikan terlebih dahulu pembayaran untuk transaksi sebelumnya";
                        }
                    }
                }
            }
            else
            {
                returnMsg = "Kode TOP belum disetting untuk pelanggan ini";
            }
            return returnMsg;
        }

        private bool isOverdueOrder(string CustomerCode)
        {
            var data = ctx.ArInterfaces.Where(x => x.ProfitCenterCode == ProfitCenter)
                .Join(ctx.SpTrnSORDHdrs.Where(y => y.CompanyCode == CompanyCode && y.BranchCode == BranchCode && y.CustomerCode == CustomerCode),
                a => new { a.CustomerCode, a.CompanyCode, a.BranchCode },
                d => new { d.CustomerCode, d.CompanyCode, d.BranchCode },
                (a, b) => new { a, b }).Count();

            return data > 0 ? true : false;
        }

        /// <summary>
        /// Create Journal SP Return
        /// </summary>
        /// <param name="docno"></param>
        /// <param name="docdate"></param>
        /// <param name="applyTo"></param>
        /// <param name="returnAmt"></param>
        /// <param name="pajakAmt"></param>
        /// <param name="diskonAmt"></param>
        /// <param name="cogsAmt"></param>
        /// <param name="typeOfGoods"></param>
        /// <param name="customerCode"></param>
        public void JournalSpReturn(string docno, DateTime docdate, string applyTo, decimal returnAmt, decimal pajakAmt, decimal diskonAmt, decimal cogsAmt, string typeOfGoods, string customerCode)
        {
            try
            {

                var userProfCtr = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
                var custProf = ctx.MstCustomerProfitCenters.Find(CompanyCode, BranchCode, customerCode, ProfitCenter);
                string custClass = string.Empty;
                if (custProf != null) custClass = custProf.CustomerClass;
                GnMstCustomerClass oCust = ctx.GnMstCustomerClasses.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode && p.CustomerClass == custClass && 
                    p.ProfitCenterCode == ProfitCenter && p.TypeOfGoods == typeOfGoods).FirstOrDefault();
                if (oCust != null)
                {
                    spMstAccount OAccount = ctx.spMstAccounts.Find(CompanyCode, BranchCode, CurrentUser.TypeOfGoods);
                    GLInterface oJurnal = new GLInterface();
                    int seqNo = 1;
                    oJurnal.DocNo = docno;
                    oJurnal.DocDate = docdate;
                    oJurnal.JournalCode = "SPAREPART";
                    oJurnal.TypeJournal = "RETURN";
                    oJurnal.ApplyTo = applyTo;
                    oJurnal.StatusFlag = "0";
                    oJurnal.CreateDate = DateTime.Now;

                    // Insert Return Journal
                    if (returnAmt > 0)
                    {
                        oJurnal.SeqNo = seqNo++;
                        oJurnal.AccountNo = OAccount.ReturnAccNo;
                        oJurnal.AmountDb = returnAmt;
                        oJurnal.AmountCr = decimal.Zero;
                        oJurnal.TypeTrans = "RETURN";
                        GenerateGLInterface(oJurnal, custProf);
                    }

                    // Insert Pajak Journal
                    if (pajakAmt > 0)
                    {
                        oJurnal = new GLInterface();
                        oJurnal.DocNo = docno;
                        oJurnal.DocDate = docdate;
                        oJurnal.JournalCode = "SPAREPART";
                        oJurnal.TypeJournal = "RETURN";
                        oJurnal.ApplyTo = applyTo;
                        oJurnal.StatusFlag = "0";
                        oJurnal.CreateDate = DateTime.Now;

                        oJurnal.SeqNo = seqNo++;
                        oJurnal.AccountNo = oCust.TaxOutAccNo;
                        oJurnal.AmountDb = pajakAmt;
                        oJurnal.AmountCr = decimal.Zero;
                        oJurnal.TypeTrans = "TAXOUT";
                        GenerateGLInterface(oJurnal, custProf);
                    }

                    // Insert Hutang Return Journal
                    if ((returnAmt + pajakAmt - diskonAmt) > 0)
                    {
                        oJurnal = new GLInterface();
                        oJurnal.DocNo = docno;
                        oJurnal.DocDate = docdate;
                        oJurnal.JournalCode = "SPAREPART";
                        oJurnal.TypeJournal = "RETURN";
                        oJurnal.ApplyTo = applyTo;
                        oJurnal.StatusFlag = "0";
                        oJurnal.CreateDate = DateTime.Now;

                        oJurnal.SeqNo = seqNo++;
                        oJurnal.AccountNo = OAccount.ReturnPybAccNo;
                        oJurnal.AmountDb = decimal.Zero;
                        oJurnal.AmountCr = returnAmt + pajakAmt - diskonAmt;
                        oJurnal.TypeTrans = "HRETURN";
                        GenerateGLInterface(oJurnal, custProf);
                    }

                    // Insert Diskon Journal
                    if (diskonAmt > 0)
                    {
                        oJurnal = new GLInterface();
                        oJurnal.DocNo = docno;
                        oJurnal.DocDate = docdate;
                        oJurnal.JournalCode = "SPAREPART";
                        oJurnal.TypeJournal = "RETURN";
                        oJurnal.ApplyTo = applyTo;
                        oJurnal.StatusFlag = "0";
                        oJurnal.CreateDate = DateTime.Now;

                        oJurnal.SeqNo = seqNo++;
                        oJurnal.AccountNo = OAccount.DiscAccNo;
                        oJurnal.AmountDb = decimal.Zero;
                        oJurnal.AmountCr = diskonAmt;
                        oJurnal.TypeTrans = "DISC1";
                        GenerateGLInterface(oJurnal, custProf);
                    }

                    if (cogsAmt > 0)
                    {
                        // Insert Inventory Journal
                        oJurnal = new GLInterface();
                        oJurnal.DocNo = docno;
                        oJurnal.DocDate = docdate;
                        oJurnal.JournalCode = "SPAREPART";
                        oJurnal.TypeJournal = "RETURN";
                        oJurnal.ApplyTo = applyTo;
                        oJurnal.StatusFlag = "0";
                        oJurnal.CreateDate = DateTime.Now;

                        oJurnal.SeqNo = seqNo++;
                        oJurnal.AccountNo = OAccount.InventoryAccNo;
                        oJurnal.AmountDb = cogsAmt;
                        oJurnal.AmountCr = decimal.Zero;
                        oJurnal.TypeTrans = "INVENTORY";
                        GenerateGLInterface(oJurnal, custProf);

                        // Insert COGS Journal
                        oJurnal = new GLInterface();
                        oJurnal.DocNo = docno;
                        oJurnal.DocDate = docdate;
                        oJurnal.JournalCode = "SPAREPART";
                        oJurnal.TypeJournal = "RETURN";
                        oJurnal.ApplyTo = applyTo;
                        oJurnal.StatusFlag = "0";
                        oJurnal.CreateDate = DateTime.Now;

                        oJurnal.SeqNo = seqNo++;
                        oJurnal.AccountNo = OAccount.COGSAccNo;
                        oJurnal.AmountDb = decimal.Zero;
                        oJurnal.AmountCr = cogsAmt;
                        oJurnal.TypeTrans = "COGS";
                        GenerateGLInterface(oJurnal, custProf);
                    }
                }
                else
                {
                    throw new Exception("Customer Class untuk Pelanggan " + customerCode + "belum di-setting. Mohon di-cek di data Master Customer");
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function JournalSpReturn, Message=" + ex.Message.ToString());
            }
        }

        /// <summary>
        /// Generate GL Interface
        /// </summary>
        /// <param name="oJournalParam">GL Interface Object</param>
        /// <param name="userProfitCenter">Customer Profit Center</param>
        public void GenerateGLInterface(GLInterface oJournal, MstCustomerProfitCenter userProfitCenter)
        {
            //GLInterface oJournal = new GLInterface();
            oJournal.CompanyCode = CompanyCode;
            oJournal.BranchCode = BranchCode;
            oJournal.ProfitCenterCode = userProfitCenter.ProfitCenterCode;
            oJournal.AccDate = oJournal.CreateDate;
            oJournal.BatchNo = string.Empty;
            oJournal.BatchDate = DateTime.Parse("1900/01/01");
            oJournal.StatusFlag = "0";
            oJournal.CreateBy = CurrentUser.UserId;
            oJournal.LastUpdateBy = CurrentUser.UserId;
            oJournal.CreateDate = DateTime.Now;
            oJournal.LastUpdateDate = DateTime.Now;
            ctx.GLInterfaces.Add(oJournal);
            Helpers.ReplaceNullable(oJournal);
            ctx.SaveChanges();
        }


        public void GenerateAR(string custcode, string docno, DateTime docdate,
            decimal nettAmt, decimal receiveAmt, string typeTrans, string fpjGovNo, DateTime fpjSignature,
            string typeOfGoods)
        {
            try
            {
                var oCustProfitCenter = ctx.MstCustomerProfitCenters.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode, custcode, ProfitCenter);
                var oCustomerClass = ctx.GnMstCustomerClasses.Where(e => e.CompanyCode == oCustProfitCenter.CompanyCode
                    && e.BranchCode == oCustProfitCenter.BranchCode
                    && e.CustomerClass == oCustProfitCenter.CustomerClass
                    && e.ProfitCenterCode == oCustProfitCenter.ProfitCenterCode
                    && e.TypeOfGoods == typeOfGoods).FirstOrDefault();
                if (oCustomerClass == null)
                { throw new Exception("Customer Class tidak di temukan"); };
                var oLookupDtl = ctx.LookUpDtls.Find(CurrentUser.CompanyCode, "TOPC", oCustProfitCenter.TOPCode);
                if (oLookupDtl != null)
                {
                    ArInterface oArInterface = new ArInterface();
                    oArInterface.CompanyCode = CurrentUser.CompanyCode;
                    oArInterface.BranchCode = CurrentUser.BranchCode;
                    oArInterface.DocNo = docno;
                    oArInterface.DocDate = docdate;
                    oArInterface.ProfitCenterCode = ProfitCenter;
                    oArInterface.NettAmt = nettAmt;
                    oArInterface.ReceiveAmt = receiveAmt;
                    oArInterface.CustomerCode = custcode;
                    oArInterface.TOPCode = oCustProfitCenter.TOPCode;
                    oArInterface.SalesCode = oCustProfitCenter.SalesCode;
                    oArInterface.DueDate = docdate.AddDays(Convert.ToDouble(oLookupDtl.ParaValue));
                    oArInterface.StatusFlag = "0";
                    oArInterface.TypeTrans = typeTrans;
                    oArInterface.CreateBy = CurrentUser.UserId;
                    oArInterface.CreateDate = DateTime.Now;
                    oArInterface.AccountNo = oCustomerClass.ReceivableAccNo;
                    oArInterface.FakturPajakNo = fpjGovNo;
                    oArInterface.FakturPajakDate = fpjSignature;
                    ctx.ArInterfaces.Add(oArInterface);
                    Helpers.ReplaceNullable(oArInterface);
                    ctx.SaveChanges();
                }
                else
                {
                    throw new Exception(string.Format("Lookup details value untuk TOPC {0} tidak ditemukan", oCustProfitCenter.TOPCode));
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function GenerateAR, Message=" + ex.Message.ToString());
            }
        }

        /// <summary>
        /// Update Item Price
        /// </summary>
        /// <param name="ReturnNo"></param>
        /// <param name="returType"></param>
        public void UpdateItemPriceAvgCost(string ReturnNo, string returType, List<RturItem> ListRturItem)
        {
            //List<SpTrnSRTurSSDtl> ListRturDtl = ctx.SpTrnSRTurSSDtls.Where(m => m.CompanyCode == CompanyCode && m.BranchCode == BranchCode && m.ReturnNo == ReturnNo).ToList();
            foreach (var RturItem in ListRturItem)
            {
                decimal avgCost = 0;
                //Get Cost Price from SpMstItem
                string sqlCP = string.Format("exec uspfn_spGetSelectCostPrice '{0}','{1}','{2}','{3}','{4}'", CompanyCode, BranchCode, RturItem.PartNo, TypeOfGoods, CurrentUser.CoProfile.ProductType);
                var selectCost = ctx.Database.SqlQuery<SelectCostPrice>(sqlCP).FirstOrDefault();

                if (returType.ToUpper() == "RJUAL")
                {
                    avgCost = AverageCost(selectCost.OnHand, selectCost.CostPrice, RturItem.QtyReturn, RturItem.CostPrice, 0);
                }
                else
                {
                    avgCost = AverageCost(selectCost.OnHand, selectCost.CostPrice, RturItem.QtyReturn, RturItem.CostPrice, 0);
                }

                if (avgCost != RturItem.CostPrice)
                {
                    //update item avg cost
                    //Get spMstItem
                    var oItem = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, selectCost.PartNo);
                    if (oItem != null)
                    {
                        oItem.LastUpdateBy = CurrentUser.UserId;
                        oItem.LastUpdateDate = DateTime.Now;
                        oItem.CostPrice = avgCost;
                        oItem.OldCostPrice = oItem.CostPrice;
                        ctx.SaveChanges();

                        //GetSpHstItemPrice
                        var ItemPriceLatest = ctx.Database.SqlQuery<GetLatestRecord>(string.Format("exec uspfn_spGetLatestRecordByPartNo '{0}','{1}','{2}'", CompanyCode, BranchCode, RturItem.PartNo)).FirstOrDefault();
                        spHstItemPrice oItemPrice = new spHstItemPrice();
                        oItemPrice.RetailPrice = (ItemPriceLatest == null ? 0 : ItemPriceLatest.RetailPrice);
                        oItemPrice.RetailPriceInclTax = (ItemPriceLatest == null ? 0 : ItemPriceLatest.RetailPriceInclTax); ;
                        oItemPrice.PurchasePrice = (ItemPriceLatest == null ? 0 : ItemPriceLatest.PurchasePrice); ;
                        oItemPrice.OldRetailPrice = (ItemPriceLatest == null ? 0 : ItemPriceLatest.OldRetailPrice); ;
                        oItemPrice.OldPurchasePrice = (ItemPriceLatest == null ? 0 : ItemPriceLatest.OldPurchasePrice); ;
                        oItemPrice.Discount = (ItemPriceLatest == null ? 0 : ItemPriceLatest.Discount); ;
                        oItemPrice.OldDiscount = (ItemPriceLatest == null ? 0 : ItemPriceLatest.OldDiscount);
                        oItemPrice.CompanyCode = CompanyCode;
                        oItemPrice.BranchCode = BranchCode;
                        oItemPrice.PartNo = RturItem.PartNo;
                        oItemPrice.UpdateDate = DateTime.Now;
                        oItemPrice.CostPrice = avgCost;
                        oItemPrice.OldCostPirce = RturItem.CostPrice;
                        oItemPrice.CreatedBy = CurrentUser.UserId;
                        oItemPrice.CreatedDate = DateTime.Now;
                        ctx.spHstItemPrices.Add(oItemPrice);
                        ctx.SaveChanges();
                    }
                }
            }

            //return Json("");
        }

        /// <summary>
        /// calculate average cost
        /// </summary>
        /// <param name="invStock"></param>
        /// <param name="costPrice"></param>
        /// <param name="receivedQty"></param>
        /// <param name="purchasePrice"></param>
        /// <param name="discPct"></param>
        /// <returns></returns>
        public decimal AverageCost(decimal invStock, decimal costPrice, decimal receivedQty, decimal purchasePrice, decimal discPct)
        {
            decimal returnVal = 0;
            try
            {
                returnVal = Convert.ToDecimal(((invStock * costPrice) + ((receivedQty * purchasePrice) * (1 - (discPct / 100)))) / (invStock + receivedQty));
            }
            catch (Exception ex)
            {
                throw new Exception("Error pada function AverageCost, Message=" + ex.ToString());
            }

            return returnVal;
        }

        public void UpdateLastItemDate(string PartNO, string UpdateType)
        {
            try
            {
                var MstItem = ctx.spMstItems.Find(CompanyCode, BranchCode, PartNO);
                if (MstItem != null)
                {
                    switch (UpdateType.ToUpper())
                    {
                        case "LLD":
                            MstItem.LastDemandDate = DateTime.Now;
                            break;
                        case "LPD":
                            MstItem.LastPurchaseDate = DateTime.Now;
                            break;
                        case "LSD":
                            MstItem.LastSalesDate = DateTime.Now;
                            break;
                    }
                    MstItem.LastUpdateDate = DateTime.Now;
                    MstItem.LastUpdateBy = CurrentUser.UserId;
                }
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Update Last Purchase Date Persediaan Gagal");
            }
        }

        public string SetStatusLabel(string status)
        {
            var lblStatus = ctx.LookUpDtls.Find(CompanyCode, "STAT", status).LookUpValueName;

            return lblStatus;
        }

        protected bool IsBranch
        {
            get
            {
                return ctx.OrganizationDtls.Find(CompanyCode, BranchCode).IsBranch;
            }
        }

        protected bool IsMD
        {
            get
            {
                return DealerCode() == "MD" ? true : false;
            }
        }

        /// <summary>
        /// Insert Into Table spMstItems, spMstItemLoc, spMstItemPrice
        /// </summary>
        /// <param name="PartNo"></param>
        /// <returns>Int</returns>
        protected int InsertItemsLocPriceFromMD(string PartNo){
            string sql = string.Format(@"EXEC uspfn_spMstItemsInsertFromMD '{0}', '{1}', '{2}','{3}'", 
                CompanyCode, BranchCode, PartNo, CurrentUser.UserId);

            return ctx.Database.ExecuteSqlCommand(sql);
        }

        protected string GetDbMD()
        {
            string sql = string.Format(@"SELECT dbo.GetDbMD('{0}', '{1}')",
                        CompanyCode, BranchCode);

            return ctx.Database.SqlQuery<string>(sql).FirstOrDefault();
        }

        #region Excel
        protected string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        protected JsonResult GenerateReportXls(DataTable dt, string sheetName, string fileName, List<List<dynamic>> header = null, List<dynamic> format = null)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(sheetName);
            var seqno = 1;
            fileName = fileName + "_" + DateTime.Now.ToString("yyyy_MMdd_HHmm") + ".xlsx";

            // add header information
            if (header != null)
            {
                seqno++;
                foreach (List<dynamic> row in header)
                {
                    //foreach (var col in row)
                    for (int i = 0; i < row.Count; i++)
                    {
                        var caption = row[i];
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = caption;
                    }
                    seqno++;
                }
                seqno++;
            }

            if (format != null)
            {
                // set caption
                for (int i = 0; i < format.Count; i++)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = format[i].text;
                }
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(format.Count)))
                    .Style.Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                seqno++;

                // set cell value
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < format.Count; i++)
                    {
                        var caption = format[i].text;
                        if (row[format[i].name].GetType().Name == "String")
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[format[i].name];
                        }
                        else
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[format[i].name];
                        }
                    }
                    seqno++;
                }

                // set width columns
                for (int i = 0; i < format.Count; i++)
                {
                    ws.Columns((i + 1).ToString()).Width = format[i].width;
                }
            }
            else
            {
                // set caption
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = dt.Columns[i].Caption;
                }
                ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)))
                    .Style.Font.SetBold()
                    .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                seqno++;

                // set cell value
                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var caption = dt.Columns[i].Caption;
                        if (row[caption].GetType().Name == "String")
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[caption];
                        }
                        else
                        {
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                        }
                    }
                    seqno++;
                }

                // set width columns
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws.Columns((i + 1).ToString()).Width = dt.Columns[i].Caption.Length + 5;
                }
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName));

            return Json(new
            {
                rows = dt.Rows.Count,
                cols = dt.Columns.Count,
                range = string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)),
                fileUrl = Url.Content("~/ReportTemp/" + fileName),
                header = header,
                format = format
            });
        }

        protected JsonResult GenerateReportXls(DataSet ds, List<string> sheets, string fileName, List<List<dynamic>> header = null, List<List<dynamic>> formats = null)
        {
            var wb = new XLWorkbook();
            var count = 0;
            fileName = fileName + "_" + DateTime.Now.ToString("yyyy_MMdd_HHmm") + ".xlsx";
            for (int idx = 0; idx < sheets.Count; idx++)
            {
                var sheetName = sheets[idx];
                var ws = wb.Worksheets.Add(sheetName);
                var dt = ds.Tables[idx];
                var seqno = 1;
                List<dynamic> format = (formats != null && formats.Count > idx) ? formats[idx] : null;
                count += dt.Rows.Count;

                // add header information
                if (header != null)
                {
                    seqno++;
                    foreach (List<dynamic> row in header)
                    {
                        //foreach (var col in row)
                        for (int i = 0; i < row.Count; i++)
                        {
                            var caption = row[i];
                            ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = caption;
                        }
                        seqno++;
                    }
                    seqno++;
                }

                if (format != null)
                {
                    // set caption
                    for (int i = 0; i < format.Count; i++)
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = format[i].text;
                    }
                    ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(format.Count)))
                        .Style.Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    seqno++;

                    // set cell value
                    foreach (DataRow row in dt.Rows)
                    {
                        for (int i = 0; i < format.Count; i++)
                        {
                            var caption = format[i].text;
                            if (row[format[i].name].GetType().Name == "String")
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[format[i].name];
                            }
                            else
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[format[i].name];
                            }
                        }
                        seqno++;
                    }

                    // set width columns
                    for (int i = 0; i < format.Count; i++)
                    {
                        ws.Columns((i + 1).ToString()).Width = format[i].width;
                    }
                }
                else
                {
                    // set caption
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = dt.Columns[i].Caption;
                    }
                    ws.Range(string.Format("A{0}:{1}{0}", seqno, GetExcelColumnName(dt.Columns.Count)))
                        .Style.Font.SetBold()
                        .Fill.SetBackgroundColor(XLColor.CornflowerBlue)
                        .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                    seqno++;

                    // set cell value
                    foreach (DataRow row in dt.Rows)
                    {
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            var caption = dt.Columns[i].Caption;
                            if (row[caption].GetType().Name == "String")
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = "'" + row[caption];
                            }
                            else
                            {
                                ws.Cell(string.Format("{0}{1}", GetExcelColumnName(i + 1), seqno)).Value = row[caption];
                            }
                        }
                        seqno++;
                    }

                    // set width columns
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        ws.Columns((i + 1).ToString()).Width = dt.Columns[i].Caption.Length + 5;
                    }
                }
            }

            wb.SaveAs(Server.MapPath("~/ReportTemp/" + fileName));

            return Json(new
            {
                fileUrl = Url.Content("~/ReportTemp/" + fileName),
                header = header,
                rows = count
            });
        }
        #endregion
    
        protected decimal GetCostPrice(string PartNo){
            var parCompanyCode = new SqlParameter("CompanyCode", SqlDbType.VarChar);
            parCompanyCode.Value = CompanyCode;
            var parBranchCode = new SqlParameter("BranchCode", SqlDbType.VarChar);
            parBranchCode.Value = BranchCode;
            var parPartNo = new SqlParameter("PartNo", SqlDbType.VarChar);
            parPartNo.Value = PartNo;
            var CostPrice = new SqlParameter("CostPrice", SqlDbType.Decimal) 
            { 
                Direction = System.Data.ParameterDirection.Output 
            };

            object[] parameters = {parCompanyCode, parBranchCode, parPartNo, CostPrice};
            ctx.Database.ExecuteSqlCommand("exec uspfn_GetCostPrice @CompanyCode, @BranchCode, @PartNo, @CostPrice OUTPUT", parameters);

            decimal? val = CostPrice.Value as decimal?;
            var decCostPrice = val ?? 0;

            return decCostPrice;
        }

        protected decimal GetRetailPrice(string PartNo, decimal RetailPriceMD)
        {
            if (IsMD)
            {
                return RetailPriceMD;
            }
            else
            {
                var rec = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, PartNo);
                if (rec != null)
                {
                    return rec.RetailPrice.Value;
                }
                else
                {
                    InsertItemsLocPriceFromMD(PartNo);
                    rec = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, PartNo);

                    return rec.RetailPrice.Value;
                }
            }
        }

        protected decimal GetRetailPriceInclTax(string PartNo, decimal RetailPriceInclTaxMD)
        {
            if (IsMD)
            {
                return RetailPriceInclTaxMD;
            }
            else
            {
                var rec = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, PartNo);
                if (rec != null)
                {
                    return rec.RetailPriceInclTax.Value;
                }
                else
                {
                    InsertItemsLocPriceFromMD(PartNo);
                    rec = ctx.spMstItemPrices.Find(CompanyCode, BranchCode, PartNo);

                    return rec.RetailPriceInclTax.Value;
                }

            }
        }

        protected string GetDynFilters(int cols)
        {
            var field = "";
            var value = "";
            string dynamicFilter = "";

            for (int i = 0; i < cols; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? ",' AND " + field + " LIKE ''%" + value + "%''" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE ''%" + value + "%''" : "";
                }
            }

            return dynamicFilter;
        }
    }
}
