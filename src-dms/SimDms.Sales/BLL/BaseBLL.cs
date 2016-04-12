using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimDms.Sales.Models;
using SimDms.Common.Models;
//using System.Web.Security;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using System.Data.SqlClient;

namespace SimDms.Sales.BLL
{
    public class BaseBLL
    {
        public DataContext ctx = new DataContext(MyHelpers.GetConnString("DataContext"));
        public MDContext ctxMD = new MDContext(MyHelpers.GetConnString("MDContext")); 

        public static string username = "";

        protected SysUser CurrentUser
        {
            get
            {
                return ctx.SysUsers.Find(username);
            }
        }

        protected SysUser CurrentUserByUname()
        {
            return ctx.SysUsers.Find(username);
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
                string s = "000";
                var x = ctx.SysUserProfitCenters.Find(CurrentUser.UserId);
                if (x != null) s = x.ProfitCenter;
                return s;
            }
        }

        protected string GetMessage(string MessageCode)
        {
            var record = ctx.SysMsgs.Find(MessageCode);
            if (record != null)
            {
                return record.MessageCaption;
            }
            else
            {
                return string.Empty;
            }
        }

        protected string GetNewDocumentNo(string doctype, DateTime transdate)
        {
            string docNo = string.Empty;
            var records = ctx.Database.SqlQuery<String>("exec uspfn_GnDocumentGetNew @CompanyCode=@p0, @BranchCode=@p1, @DocumentType=@p2, @UserID=@p3, @TransDate=@p4",
                CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate);
            
            if (records != null)
            {
                docNo = records.FirstOrDefault();
                records = null;
            }

            return docNo;
        }

        protected string GetNewDocumentNo(string doctype, DateTime transdate, string fintype)
        {
            GnMstDocument mDoc = ctx.GnMstDocuments.Where(p => p.CompanyCode == CompanyCode 
                && p.BranchCode == BranchCode && p.DocumentType == doctype).FirstOrDefault();

            if (mDoc == null)
            {
                return string.Format("{0}/{1}/{2}", doctype, DateTime.Now.ToString("yy"), "XXXXXX");
            }

            string profitCenterCode = mDoc.ProfitCenterCode;
            string docYear = "XX";

            switch (profitCenterCode)
            {
                case "000":
                    CoProfileFinance oCoProfileFin = ctx.CoProfileFinances.Find(CompanyCode, BranchCode);
                    if (fintype.ToUpper() == "AP")
                    {
                        if (Convert.ToDateTime(oCoProfileFin.PeriodBeg).ToString("yyy").Trim().Equals(mDoc.DocumentYear.ToString().Trim()))
                            docYear = mDoc.DocumentYear.ToString().Substring(2, 2);

                    }
                    if (fintype.ToUpper() == "AR" || fintype.ToUpper() == "ARPPN")
                    {
                        if (Convert.ToDateTime(oCoProfileFin.PeriodBegAR).ToString("yyy").Trim().Equals(mDoc.DocumentYear.ToString().Trim()))
                            docYear = mDoc.DocumentYear.ToString().Substring(2, 2);

                    }
                    if (fintype.ToUpper() == "GL")
                    {
                        if (Convert.ToDateTime(oCoProfileFin.PeriodBegGL).ToString("yyy").Trim().Equals(mDoc.DocumentYear.ToString().Trim()))
                            docYear = mDoc.DocumentYear.ToString().Substring(2, 2);

                    }
                    oCoProfileFin = null;
                    break;
                case "100":
                    GnMstCoProfileSales oCoProfileSls = ctx.GnMstCoProfileSaleses.Find(CompanyCode, BranchCode);
                    if (Convert.ToDateTime(oCoProfileSls.PeriodBeg).ToString("yyy").Trim().Equals(mDoc.DocumentYear.ToString().Trim()))
                        docYear = mDoc.DocumentYear.ToString().Substring(2, 2);
                    break;
                case "200":
                    GnMstCoProfileService oCoProfileSv = ctx.GnMstCoProfileServices.Find(CompanyCode, BranchCode);
                    if (oCoProfileSv.PeriodBeg.ToString("yyy").Trim().Equals(mDoc.DocumentYear.ToString().Trim()))
                        docYear = mDoc.DocumentYear.ToString().Substring(2, 2);
                    break;
                case "300":
                    GnMstCoProfileSpare oCoProfileSp = ctx.GnMstCoProfileSpares.Find(CompanyCode, BranchCode);
                    if (oCoProfileSp.PeriodBeg.ToString("yyy").Trim().Equals(mDoc.DocumentYear.ToString().Trim()))
                        docYear = mDoc.DocumentYear.ToString().Substring(2, 2);
                    break;
                default:
                    break;
            }

            p_UpdateTransDate(profitCenterCode, transdate, fintype);

            if (mDoc != null && docYear != "XX")
            {
                mDoc.DocumentSequence = mDoc.DocumentSequence + 1;
                mDoc.LastUpdateBy = CurrentUser.UserId;
                mDoc.LastUpdateDate = DateTime.Now;
                ctx.SaveChanges();
                return string.Format("{0}/{1}/{2}", mDoc.DocumentPrefix, mDoc.DocumentYear.ToString().Substring(2, 2), mDoc.DocumentSequence.ToString().PadLeft(6, '0'));
            }
            else
            {
                return string.Format("{0}/{1}/{2}", doctype, DateTime.Now.ToString("yy"), "XXXXXX");
            }
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
                var rd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (rd == null)
                {
                    return CompanyCode;
                }
                return rd.CompanyMD;
            }
        }

        protected string BranchMD
        {
            get
            {
                var rd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (rd == null)
                {
                    return BranchCode;
                }
                return rd.BranchMD;
            }
        }

        protected string UnitBranchMD
        {
            get
            {
                var rd = ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode);
                if (rd == null)
                {
                    return BranchCode;
                }
                return rd.UnitBranchMD;
            }
        }

        protected string WarehouseMD
        {
            get
            {
                return ctx.CompanyMappings.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.BranchCode == BranchCode).WarehouseMD;
            }
        }

        protected bool cekOtomatis()
        {
            bool otom = true;
            string rcd = ctx.Database.SqlQuery<string>("SELECT ParaValue from gnMstLookUpDtl WHERE CodeID='OTOM' AND LookUpValue='UNIT' AND CompanyCode='" + CompanyCode + "'").FirstOrDefault();
            if (rcd != null && rcd == "0")
            {
                otom = false;
            }

            return otom;
        }

        #region -- Private Method --
        private void p_UpdateTransDate(string profitCenterCode, DateTime transdate, string fintype)
        {
            string sql = "update [TableName] set TransDate=@TransDate, LastUpdateBy=@LastUpdateBy, LastUpdateDate=@LastUpdateDate where CompanyCode = @CompanyCode AND BranchCode = @BranchCode";
            switch (profitCenterCode)
            {
                case "000":
                    switch (fintype)
                    {
                        case "AP1":  // saat ini belum digunakan
                            sql = "update gnMstCoProfileFinance set TransDateAP=@TransDate, LastUpdateBy=@LastUpdateBy, LastUpdateDate=@LastUpdateDate where CompanyCode = @CompanyCode AND BranchCode = @BranchCode";
                            break;
                        case "ARPPN":
                            sql = "update gnMstCoProfileFinance set TransDateAR=@TransDate, LastUpdateBy=@LastUpdateBy, LastUpdateDate=@LastUpdateDate where CompanyCode = @CompanyCode AND BranchCode = @BranchCode";
                            break;
                        case "GL1": // saat ini belum digunakan
                            sql = "update gnMstCoProfileFinance set TransDateGL=@TransDate, LastUpdateBy=@LastUpdateBy, LastUpdateDate=@LastUpdateDate where CompanyCode = @CompanyCode AND BranchCode = @BranchCode";
                            break;
                        default:
                            break;
                    }
                    break;
                case "100":
                    if (fintype != "OM")
                        sql = sql.Replace("[TableName]", "gnMstCoProfileSales");
                    break;
                case "200":
                    sql = sql.Replace("[TableName]", "gnMstCoProfileService");
                    break;
                case "300":
                    sql = sql.Replace("[TableName]", "gnMstCoProfileSpare");
                    break;
                default:
                    break;
            }

            if (!sql.Contains("[TableName]"))
            {
                try
                {
                    SqlParameter p0 = new SqlParameter("@CompanyCode", CompanyCode);
                    SqlParameter p1 = new SqlParameter("@BranchCode", BranchCode);
                    SqlParameter p2 = new SqlParameter("@TransDate", transdate);
                    SqlParameter p3 = new SqlParameter("@LastUpdateBy", CurrentUser.UserId);
                    SqlParameter p4 = new SqlParameter("@LastUpdateDate", DateTime.Now);
                    Object[] oPeams = new Object[] { p0, p1, p2, p3, p4 };

                    ctx.Database.ExecuteSqlCommand(sql, oPeams);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        #endregion
    }
}