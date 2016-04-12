using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SimDms.Service.Models;
using System.Collections;
using System.Transactions;
using System.Threading;
using System.Globalization;

namespace SimDms.Service.BLL
{
    public class WarrantyClaimBLL : BaseBLL
    {
        #region "Initiate"
        /// <summary>
        /// 
        /// </summary>
        private static WarrantyClaimBLL _WarrantyClaimBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_username"></param>
        /// <returns></returns>
        public static WarrantyClaimBLL Instance(string _username)
        {
            CultureInfo ci = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = ci;

            //if (_WarrantyClaimBLL == null)
            //{
                _WarrantyClaimBLL = new WarrantyClaimBLL();
            //}
            if (string.IsNullOrEmpty(username))
            {
                username = _username;
            }
            return _WarrantyClaimBLL;
        }
        #endregion

        #region Constant
        
        /// Document Prefix 
        public const string CLM = "CLM"; // Supplier Claim (Spareparts)
        public const string CLA = "CLA"; // Warranty Claim (Service)
        public const string CLW = "CLW"; // Warranty Claim SPK (Service)

        public enum WarrantyReportSource
        {
            GET = 0, //Get Warranty Claim Report
            INPUT = 1, //Input Warranty Claim Report
            UPLOAD = 2 //Upload Warranty Claim Report
        };

        #endregion
        
        #region -- Generate Warranty Claim --
        public WarrantyClaimHdrFile GetWClaimHdrFile(string dataID, string batchNo)
        {
            SqlCommand sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            sqlCmd.CommandText = "uspfn_SvGetFlatFileClaimHdr";
            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
            sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
            sqlCmd.Parameters.AddWithValue("@ProductType", ProductType);
            sqlCmd.Parameters.AddWithValue("@BatchNo", batchNo);
            sqlCmd.Parameters.AddWithValue("@DataID", dataID);
            DataTable dt = new DataTable();
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            sqlDA.Fill(dt);
            
            WarrantyClaimHdrFile file = new WarrantyClaimHdrFile();
            foreach (DataRow row in dt.Rows)
            {
                if (row != null)
                {
                    file.DealerCode = row["DealerCode"].ToString();
                    file.RcvDealerCode = row["RcvDealerCode"].ToString();
                    file.DealerName = row["DealerName"].ToString();
                    file.TotalItem = Convert.ToInt32(row["TotalItem"]);
                    file.ReceiptNo = row["ReceiptNo"].ToString();
                    file.ReceiptDate = Convert.ToDateTime(row["ReceiptDate"]);
                    file.ProductType = row["ProductType"].ToString();
                    file.LotNo = Convert.ToInt32(row["LotNo"]);
                    file.OtherCompensation = Convert.ToDecimal(row["OtherCompensation"]);
                    file.FlagData = row["ClaimFlag"].ToString();
                }
            }

            dt = null;
            sqlDA = null;
            sqlCmd = null;

            return file;
        }
        #endregion
        
        #region -- Upload Warranty Claim --
        public bool Save(WarrantyClaimHdrFile header, List<WarrantyClaimDtlFile> details, out string generateNo, DateTime generateDate, string FPJNo)
        {
            generateNo = "";
            // Check Available Record
            Claim record = GetRecordClaim(header);

            int result = -1;
            try
            {
                using( var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (record == null)
                    {
                        result = p_Insert(header, details, out generateNo, generateDate, FPJNo);
                    }
                    else
                    {
                        record.GenerateDate = generateDate;
                        result = p_Update(record, header, details);
                        generateNo = record.GenerateNo;
                    }

                    if (result > 0) result += p_RecalculateParent(generateNo);
                    tranScope.Complete();
                    
                    return (result > 0);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool Save(WarrantyClaimHdrFile header, List<WarrantyClaimDtlFile> details, out string generateNo, DateTime generateDate, bool bSPK)
        {
            generateNo = "";
            // Check Available Record
            Claim oClaim = (bSPK) ? null : GetRecordClaim(header);
            SvTrnClaimSPK oSvTrnClaimSPK = (bSPK) ? GetRecordSvTrnClaimSPK(header) : null;

            int result = -1;
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if ((bSPK) ? oSvTrnClaimSPK == null : oClaim == null)
                    {
                        result = p_Insert(header, details, out generateNo, generateDate, bSPK);
                    }
                    else
                    {
                        if (bSPK) oSvTrnClaimSPK.GenerateDate = generateDate;
                        else oClaim.GenerateDate = generateDate;
                        result = p_Update(oSvTrnClaimSPK, oClaim, header, details, bSPK);
                        generateNo = (bSPK) ? oSvTrnClaimSPK.GenerateNo : oClaim.GenerateNo;
                    }

                    if (result > 0) result += p_RecalculateParent(generateNo, bSPK);
                    tranScope.Complete();
                    
                    return (result > 0);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        public SvTrnClaimSPK GetRecordSvTrnClaimSPK(WarrantyClaimHdrFile header)
        {
            var oSvTrnClaimSPK = ctx.SvTrnClaimSPKs.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.ProductType == ProductType && p.SenderDealerCode == header.DealerCode && p.RefferenceNo == header.ReceiptNo 
                && p.SourceData == "2").FirstOrDefault();


            return oSvTrnClaimSPK;
        }

        public SvTrnClaimSPK GetRecordSvTrnClaimSPK(string generateNo, string srcdata)
        {
            var oSvTrnClaimSPK = ctx.SvTrnClaimSPKs.Where(p => p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.ProductType == ProductType && p.GenerateNo == generateNo && p.SourceData == srcdata).FirstOrDefault();

            return oSvTrnClaimSPK;
        }
        
        public Claim GetRecordClaim(WarrantyClaimHdrFile header)
        {
            var oClaim = ctx.Claims.Where(
                p => p.CompanyCode == CompanyCode &&
                p.BranchCode == BranchCode &&
                p.ProductType == ProductType &&
                p.SenderDealerCode == header.DealerCode &&
                p.RefferenceNo == header.ReceiptNo && 
                p.SourceData == "2"
            ).FirstOrDefault();

            return oClaim;
        }

        public Claim GetRecordClaim(string generateNo, string srcdata)
        {
            var oClaim = ctx.Claims.Where(
                p => p.CompanyCode == CompanyCode &&
                p.BranchCode == BranchCode &&
                p.ProductType == ProductType &&
                p.GenerateNo == generateNo &&
                p.SourceData == srcdata
            ).FirstOrDefault();

            return oClaim;
        }
  
        public DataSet GetDataClaim(string generateNo)
        {
            string sql = string.Format(@"
            declare @CompanyCode varchar(15)
            declare @BranchCode varchar(15)
            declare @ProductType varchar(15) 
            declare @GenerateNo varchar(15)

            set @CompanyCode = '{0}'
            set @BranchCode = '{1}'
            set @ProductType = '{2}'
            set @GenerateNo = '{3}'

            select (row_number() over (order by a.GenerateSeq)) RecNo
            ,a.GenerateSeq
            ,a.CategoryCode
            ,a.IssueNo
            ,a.IssueDate
            ,a.InvoiceNo
            ,a.ServiceBookNo
            ,a.ChassisCode
            ,a.ChassisNo
            ,a.EngineCode
            ,a.EngineNo
            ,a.RegisteredDate
            ,a.RepairedDate
            ,a.Odometer
            ,a.ComplainCode
            ,a.DefectCode
            ,a.SubletHour
            ,a.SubletAmt
            ,a.OperationNo
            ,left(a.OperationNo,6) BasicCode
            --,left(right(rtrim(a.OperationNo),len(a.OperationNo) - 6),1) VarCom
            ,SUBSTRING(a.OperationNo, 7, len(a.OperationNo)) VarCom
            ,a.OperationHour
            ,a.ClaimAmt
            ,a.TroubleDescription
            ,a.ProblemExplanation
            ,a.BasicModel
            from svTrnClaimApplication a
            where 1 = 1
             and a.CompanyCode = @CompanyCode
             and a.BranchCode = @BranchCode
             and a.ProductType = @ProductType
             and a.GenerateNo = @GenerateNo

            select (row_number() over (order by a.BasicModel)) RecNo
            ,a.BasicModel
            ,sum(a.ClaimAmt) TotalClaimAmt
            from svTrnClaimApplication a
            where 1 = 1
             and a.CompanyCode = @CompanyCode
             and a.BranchCode = @BranchCode
             and a.ProductType = @ProductType
             and a.GenerateNo = @GenerateNo
            group by a.BasicModel

            select 
             a.GenerateSeq
            ,a.PartSeq
            ,a.IsCausal
            ,a.PartNo
            ,a.Quantity
            ,b.PartName
            from svTrnClaimPart a
            left join spMstItemInfo b on b.PartNo = a.PartNo and b.CompanyCode = a.COmpanyCode
            where 1 = 1
             and a.CompanyCode = @CompanyCode
             and a.BranchCode = @BranchCode
             and a.ProductType = @ProductType
             and a.GenerateNo = @GenerateNo

            ", CompanyCode, BranchCode, ProductType, generateNo);

            using(var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                sqlCmd.CommandText = sql.Trim();
                sqlCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
                sqlDA.Fill(ds);

                return ds;
            }
        }

        public DataSet GetDataClaim(string generateNo, bool bSPK)
        {
            string tblClaimAPP = (bSPK) ? "SvTrnClaimSPKApp" : "svTrnClaimApplication";
            string tblClaimPart = (bSPK) ? "SvTrnClaimSPKPart" : "svTrnClaimPart";

            string sql = string.Format(@"
            declare @CompanyCode varchar(15)
            declare @BranchCode varchar(15)
            declare @ProductType varchar(15) 
            declare @GenerateNo varchar(15)

            set @CompanyCode = '{0}'
            set @BranchCode = '{1}'
            set @ProductType = '{2}'
            set @GenerateNo = '{3}'

            select (row_number() over (order by a.GenerateSeq)) RecNo
            ,a.GenerateSeq
            ,a.CategoryCode
            ,a.IssueNo
            ,a.IssueDate
            ,a.InvoiceNo
            ,a.ServiceBookNo
            ,a.ChassisCode
            ,a.ChassisNo
            ,a.EngineCode
            ,a.EngineNo
            ,a.RegisteredDate
            ,a.RepairedDate
            ,a.Odometer
            ,a.ComplainCode
            ,a.DefectCode
            ,a.SubletHour
            ,a.SubletAmt
            ,a.OperationNo
            ,left(a.OperationNo,6) BasicCode
            --,left(right(rtrim(a.OperationNo),len(a.OperationNo) - 6),1) VarCom
            ,SUBSTRING(a.OperationNo, 7, len(a.OperationNo)) VarCom
            ,a.OperationHour
            ,a.ClaimAmt
            ,a.TroubleDescription
            ,a.ProblemExplanation
            ,a.BasicModel
            from {4} a
            where 1 = 1
                and a.CompanyCode = @CompanyCode
                and a.BranchCode = @BranchCode
                and a.ProductType = @ProductType
                and a.GenerateNo = @GenerateNo

            select (row_number() over (order by a.BasicModel)) RecNo
            ,a.BasicModel
            ,sum(a.ClaimAmt) TotalClaimAmt
            from {4} a
            where 1 = 1
                and a.CompanyCode = @CompanyCode
                and a.BranchCode = @BranchCode
                and a.ProductType = @ProductType
                and a.GenerateNo = @GenerateNo
            group by a.BasicModel

            select 
                a.GenerateSeq
            ,a.PartSeq
            ,a.IsCausal
            ,a.PartNo
            ,a.Quantity
            ,b.PartName
            from {5} a
            left join spMstItemInfo b on b.PartNo = a.PartNo and b.CompanyCode = a.COmpanyCode
            where 1 = 1
                and a.CompanyCode = @CompanyCode
                and a.BranchCode = @BranchCode
                and a.ProductType = @ProductType
                and a.GenerateNo = @GenerateNo

            ", CompanyCode, BranchCode, ProductType, generateNo, tblClaimAPP, tblClaimPart);

            using (var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                sqlCmd.CommandText = sql.Trim();
                sqlCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
                sqlDA.Fill(ds);

                return ds;
            }
        }

        #region -- Private Methods --

        private int p_Insert(WarrantyClaimHdrFile header, List<WarrantyClaimDtlFile> details, out string generateNo, DateTime generateDate, string FPJNo)
        {
            int result = p_Insert(header, out generateNo, generateDate, FPJNo);
            if (result > 0)
            {
                result += p_Save(generateNo, details);
                if (result > 1) result += p_RecalculateParent(generateNo);
            }

            return result;
        }

        private int p_Insert(WarrantyClaimHdrFile header, List<WarrantyClaimDtlFile> details, out string generateNo, DateTime generateDate, bool bSPK)
        {
            int result = p_Insert(header, out generateNo, generateDate, bSPK);
            if (result > 0)
            {
                result += p_Save(generateNo, details, bSPK);
                if (result > 1) result += p_RecalculateParent(generateNo, bSPK);
            }

            return result;
        }

        private int p_Insert(WarrantyClaimHdrFile header, out string generateNo, DateTime generateDate, bool bSPK)
        {
            if (bSPK)
            {
                //SvTrnClaimSPKDao oSvTrnClaimSPKDao = new SvTrnClaimSPKDao(ctx);
                SvTrnClaimSPK oSvTrnClaimSPK = new SvTrnClaimSPK();
                return p_Insert(header, out generateNo, generateDate, oSvTrnClaimSPK);
            }
            else
            {
                //ClaimDao oClaimDao = new ClaimDao(ctx);
                Claim oClaim = new Claim();
                return p_Insert(header, out generateNo, generateDate, oClaim);
            }
        }

        private int p_Insert(WarrantyClaimHdrFile header, out string generateNo, DateTime? generateDate, SvTrnClaimSPK record)
        {
            record.CompanyCode = CompanyCode;
            record.BranchCode = BranchCode;
            record.ProductType = ProductType;
            record.GenerateNo = p_GetNewDocumentNo(CLW, generateDate);
            record.GenerateDate = generateDate;
            record.SourceData = "2";
            record.SenderDealerCode = header.DealerCode;
            record.ReceiveDealerCode = header.RcvDealerCode;
            record.SenderDealerName = header.DealerName;
            record.RefferenceNo = header.ReceiptNo;
            record.RefferenceDate = header.ReceiptDate;
            record.TotalNoOfItem = header.TotalItem;
            record.OtherCompensationAmt = header.OtherCompensation;
            record.LotNo = Convert.ToDecimal(header.LotNo);
            record.PostingFlag = "1";
            record.CreatedBy = CurrentUser.UserId;
            record.CreatedDate = DateTime.Now;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;

            generateNo = record.GenerateNo;
            generateDate = record.GenerateDate;

            ctx.SvTrnClaimSPKs.Add(record);
            return ctx.SaveChanges();
        }

        private int p_Insert( WarrantyClaimHdrFile header, out string generateNo, DateTime? generateDate, Claim record)
        {
            record.CompanyCode = CompanyCode;
            record.BranchCode = BranchCode;
            record.ProductType = ProductType;
            record.GenerateNo = p_GetNewDocumentNo(CLA, generateDate);
            record.GenerateDate = generateDate;
            record.SourceData = "2";
            record.SenderDealerCode = header.DealerCode;
            record.ReceiveDealerCode = header.RcvDealerCode;
            record.SenderDealerName = header.DealerName;
            record.RefferenceNo = header.ReceiptNo;
            record.RefferenceDate = header.ReceiptDate;
            record.TotalNoOfItem = header.TotalItem;
            record.OtherCompensationAmt = header.OtherCompensation;
            record.LotNo = Convert.ToDecimal(header.LotNo);
            record.PostingFlag = "1";
            record.CreatedBy = CurrentUser.UserId;
            record.CreatedDate = DateTime.Now;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;

            generateNo = record.GenerateNo;
            generateDate = record.GenerateDate;
            
            ctx.Claims.Add(record);
            return ctx.SaveChanges();
        }

        private int p_Insert(WarrantyClaimHdrFile header, out string generateNo, DateTime? generateDate, string FPJNo)
        {
            //ClaimDao oClaimDao = new ClaimDao(ctx);
            Claim record = new Claim();

            record.CompanyCode = CompanyCode;
            record.BranchCode = BranchCode;
            record.ProductType = ProductType;
            record.GenerateNo = p_GetNewDocumentNo(CLA, generateDate);
            record.GenerateDate = generateDate;
            record.SourceData = "2";
            record.SenderDealerCode = header.DealerCode;
            record.ReceiveDealerCode = header.RcvDealerCode;
            record.SenderDealerName = header.DealerName;
            record.RefferenceNo = header.ReceiptNo;
            record.RefferenceDate = header.ReceiptDate;
            record.TotalNoOfItem = header.TotalItem;
            record.OtherCompensationAmt = header.OtherCompensation;
            record.LotNo = Convert.ToDecimal(header.LotNo);
            record.PostingFlag = "1";
            record.CreatedBy = CurrentUser.UserId;
            record.CreatedDate = DateTime.Now;
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;
            record.FPJNo = FPJNo;
            
            generateNo = record.GenerateNo;
            generateDate = record.GenerateDate;


            ctx.Claims.Add(record);
            return ctx.SaveChanges();
        }

        private int p_Update(Claim record, WarrantyClaimHdrFile header, List<WarrantyClaimDtlFile> details)
        {
            int result = p_Update(record, header);
            if (result > 0)
            {
                result += p_Save(record.GenerateNo, details);
                if (result > 1) result += p_RecalculateParent(record.GenerateNo);
            }

            return result;
        }

        private int p_Update(SvTrnClaimSPK oSvTrnClaimSPK, Claim oClaim, WarrantyClaimHdrFile header, List<WarrantyClaimDtlFile> details, bool bSPK)
        {
            int result = (bSPK) ? p_Update(oSvTrnClaimSPK, header) : p_Update(oClaim, header);
            if (result > 0)
            {
                result += p_Save((bSPK) ? oSvTrnClaimSPK.GenerateNo : oClaim.GenerateNo, details, bSPK);
                if (result > 1) result += p_RecalculateParent((bSPK) ? oSvTrnClaimSPK.GenerateNo : oClaim.GenerateNo, bSPK);
            }

            return result;
        }

        private int p_Update(Claim record, WarrantyClaimHdrFile header)
        {
            //ClaimDao oClaimDao = new ClaimDao(ctx);
            record.SenderDealerCode = header.DealerCode;
            record.ReceiveDealerCode = header.RcvDealerCode;
            record.SenderDealerName = header.DealerName;
            record.RefferenceNo = header.ReceiptNo;
            record.RefferenceDate = header.ReceiptDate;
            record.TotalNoOfItem = header.TotalItem;
            record.OtherCompensationAmt = header.OtherCompensation;
            record.LotNo = Convert.ToDecimal(header.LotNo);
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;

            return ctx.SaveChanges();
        }

        private int p_Update(SvTrnClaimSPK record, WarrantyClaimHdrFile header)
        {
            //SvTrnClaimSPKDao oClaimDao = new SvTrnClaimSPKDao(ctx);
            record.SenderDealerCode = header.DealerCode;
            record.ReceiveDealerCode = header.RcvDealerCode;
            record.SenderDealerName = header.DealerName;
            record.RefferenceNo = header.ReceiptNo;
            record.RefferenceDate = header.ReceiptDate;
            record.TotalNoOfItem = header.TotalItem;
            record.OtherCompensationAmt = header.OtherCompensation;
            record.LotNo = Convert.ToDecimal(header.LotNo);
            record.LastupdateBy = CurrentUser.UserId;
            record.LastupdateDate = DateTime.Now;

            return ctx.SaveChanges();
        }

        private int p_RecalculateParent(string generateNo)
        {
            int result = -1;
            string sql = "";

            sql = string.Format(@"
            select GenerateNo,GenerateSeq,OperationHour,SubletHour,RepairedDate from svTrnClaimApplication
            where 1 = 1
             and CompanyCode='{0}'
             and BranchCode='{1}'
             and ProductType='{2}'
             and GenerateNo='{3}'
            ", CompanyCode, BranchCode, ProductType, generateNo);
            SqlCommand sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            sqlCmd.CommandText = sql;
            sqlCmd.CommandType = CommandType.Text;
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            sqlDA.Fill(dt);

            sql = "";
            foreach (DataRow row in dt.Rows)
            {
                string sqlupd = @"
            update svTrnClaimApplication set
             OperationAmt = {5}
            ,SubletAmt = {6}
            ,PartAmt = (select sum(TotalPrice) from svTrnClaimPart where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}' and GenerateSeq='{4}')
            ,ClaimAmt = (OperationAmt + SubletAmt + PartAmt)
            where 1 = 1
             and CompanyCode='{0}'
             and BranchCode='{1}'
             and ProductType='{2}'
             and GenerateNo='{3}'
             and GenerateSeq='{4}'
            ";
                decimal laborRate = p_GetLaborRate(Convert.ToDateTime(row["RepairedDate"]));
                decimal oprAmt = Convert.ToDecimal(row["OperationHour"]) * laborRate;
                decimal subAmt = Convert.ToDecimal(row["SubletHour"]) * laborRate;

                sql += string.Format(sqlupd.Trim() + "\n"
                       , CompanyCode
                       , BranchCode
                       , ProductType
                       , generateNo
                       , row["GenerateSeq"]
                       , oprAmt
                       , subAmt
                       );
            }

            sql += string.Format(@"
            update svTrnClaim set 
             FPJDate = null
            ,TotalOperationHour = (select sum(OperationHour) from svTrnClaimApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalSubletHour = (select sum(SubletHour) from svTrnClaimApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalOperationAmt = (select sum(OperationAmt) from svTrnClaimApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalSubletAmt = (select sum(SubletAmt) from svTrnClaimApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalPartAmt = (select sum(PartAmt) from svTrnClaimApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalClaimAmt = (select sum(ClaimAmt) from svTrnClaimApplication where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            where 1 = 1
             and CompanyCode='{0}'
             and BranchCode='{1}'
             and ProductType='{2}'
             and GenerateNo='{3}'
            ", CompanyCode, BranchCode, ProductType, generateNo);
                    
            ctx.Database.ExecuteSqlCommand(sql.Trim());
            result = ctx.SaveChanges();

            dt = null;
            sqlDA = null;
            sqlCmd = null;

            return result;
        }

        private int p_RecalculateParent(string generateNo, bool bSPK)
        {
            int result = -1;
            string sql = "";
            string tblClaimApp = (bSPK) ? "SvTrnClaimSPKApp" : "svTrnClaimApplication";
            string tblClaimPart = (bSPK) ? "SvTrnClaimSPKPart" : "svTrnClaimPart";

            sql = string.Format(@"
                select GenerateNo,GenerateSeq,OperationHour,SubletHour,RepairedDate from {4}
                where 1 = 1
                 and CompanyCode='{0}'
                 and BranchCode='{1}'
                 and ProductType='{2}'
                 and GenerateNo='{3}'
                ", CompanyCode, BranchCode, ProductType, generateNo, tblClaimApp);
            
            SqlCommand sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            sqlCmd.CommandText = sql;
            sqlCmd.CommandType = CommandType.Text;
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            sqlDA.Fill(dt);

            sql = "";
            foreach (DataRow row in dt.Rows)
            {
                string sqlupd = @"
                update {7} set
                 OperationAmt = {5}
                ,SubletAmt = {6}
                ,PartAmt = (select sum(TotalPrice) from {8} where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}' and GenerateSeq='{4}')
                ,ClaimAmt = (OperationAmt + SubletAmt + PartAmt)
                where 1 = 1
                 and CompanyCode='{0}'
                 and BranchCode='{1}'
                 and ProductType='{2}'
                 and GenerateNo='{3}'
                 and GenerateSeq='{4}'
                ";
                decimal laborRate = p_GetLaborRate(Convert.ToDateTime(row["RepairedDate"]));
                decimal oprAmt = Convert.ToDecimal(row["OperationHour"]) * laborRate;
                decimal subAmt = Convert.ToDecimal(row["SubletHour"]) * laborRate;

                sql += string.Format(sqlupd.Trim() + "\n"
                       , CompanyCode
                       , BranchCode
                       , ProductType
                       , generateNo
                       , row["GenerateSeq"]
                       , oprAmt
                       , subAmt
                       , tblClaimApp
                       , tblClaimPart
                       );
            }

            string tblClaim = (bSPK) ? "SvTrnClaimSPK" : "svTrnClaim";

            sql += string.Format(@"
            update {4} set 
                FPJDate = null
            ,TotalOperationHour = (select sum(OperationHour) from {5} where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalSubletHour = (select sum(SubletHour) from {5} where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalOperationAmt = (select sum(OperationAmt) from {5} where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalSubletAmt = (select sum(SubletAmt) from {5} where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalPartAmt = (select sum(PartAmt) from {5} where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            ,TotalClaimAmt = (select sum(ClaimAmt) from {5} where CompanyCode='{0}' and BranchCode='{1}' and ProductType='{2}' and GenerateNo='{3}')
            where 1 = 1
                and CompanyCode='{0}'
                and BranchCode='{1}'
                and ProductType='{2}'
                and GenerateNo='{3}'
            ", CompanyCode, BranchCode, ProductType, generateNo, tblClaim, tblClaimApp);

            //sqlCmd.CommandText = sql.Trim();
            //result = sqlCmd.ExecuteNonQuery();

            ctx.Database.ExecuteSqlCommand(sql.Trim());
            result = ctx.SaveChanges();

            dt = null;
            sqlDA = null;
            sqlCmd = null;

            return result;
        }

        private int p_Save(string generateNo, List<WarrantyClaimDtlFile> details)
        {
            DateTime DateTime = DateTime.Now;
            string sql = string.Format(@"
            select GenerateSeq from svTrnClaimApplication
            where CompanyCode='{0}'
             and BranchCode='{1}'
             and ProductType='{2}'
             and GenerateNo='{3}'
            ", CompanyCode, BranchCode, ProductType, generateNo).Trim();

            SqlCommand sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            sqlCmd.CommandText = sql;
            sqlCmd.CommandType = CommandType.Text;
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            sqlDA.Fill(dt);

            Hashtable hash = new Hashtable();
            foreach (DataRow row in dt.Rows) hash[Convert.ToInt32(row["GenerateSeq"])] = true;

            sql = "";
            string sqltplins = @"
                INSERT INTO svTrnClaimApplication 
                (
                 CompanyCode,BranchCode,ProductType,GenerateNo,GenerateSeq
                ,InvoiceNo,IssueNo,IssueDate,ClaimStatus,ServiceBookNo
                ,BasicModel,TechnicalModel,ChassisCode,ChassisNo,EngineCode
                ,EngineNo,RegisteredDate,RepairedDate,Odometer,IsCBU
                ,ComplainCode,DefectCode,CategoryCode,OperationNo,OperationHour
                ,SubletHour,OperationAmt,SubletAmt,PartAmt,ClaimAmt
                ,TroubleDescription,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
                )
                VALUES
                (
                 '{0}','{1}','{2}','{3}','{4}',
                 '{5}','{6}','{7}','{8}','{9}',
                 '{10}','{11}','{12}','{13}','{14}',
                 '{15}','{16}','{17}','{18}','{19}',
                 '{20}','{21}','{22}','{23}','{24}',
                 '{25}','{26}','{27}','{28}','{29}',
                 '{30}','{31}','{32}','{33}','{34}'
                )
                ";

            string sqltplupd = @"
                UPDATE svTrnClaimApplication SET
                 InvoiceNo='{5}'
                ,IssueNo='{6}'
                ,IssueDate='{7}'
                ,ClaimStatus='{8}'
                ,ServiceBookNo='{9}'
                ,BasicModel='{10}'
                ,TechnicalModel='{11}'
                ,ChassisCode='{12}'
                ,ChassisNo='{13}'
                ,EngineCode='{14}'
                ,EngineNo='{15}'
                ,RegisteredDate='{16}'
                ,RepairedDate='{17}'
                ,Odometer='{18}'
                ,IsCBU='{19}'
                ,ComplainCode='{20}'
                ,DefectCode='{21}'
                ,CategoryCode='{22}'
                ,OperationNo='{23}'
                ,OperationHour='{24}'
                ,SubletHour='{25}'
                ,TroubleDescription='{26}'
                ,LastUpdateBy='{27}'
                ,LastUpdateDate='{28}'
                WHERE 1 = 1
                 AND CompanyCode='{0}'
                 AND BranchCode='{1}'
                 AND ProductType='{2}'
                 AND GenerateNo='{3}'
                 AND GenerateSeq={4}
                ";

            DataTable dtPrice = p_SelectPrice(details);
            for (int i = 0; i < details.Count; i++)
            {
                if (hash[i + 1] == null)
                {
                    sql += string.Format(sqltplins.Trim() + "\n"
                        , CompanyCode, BranchCode, ProductType, generateNo, i + 1
                        , "", details[i].IssueNo, details[i].IssueDate, 0, details[i].ServiceBookNo
                        , details[i].BasicModel, details[i].TechnicalModel, details[i].ChassisCode, details[i].ChassisNo, details[i].EngineCode
                        , details[i].EngineNo, details[i].RegisteredDate, details[i].RepairedDate, details[i].Odometer, details[i].KdOemCode.Equals("X") ? 0 : 1
                        , details[i].TroubleCode.Substring(0, 2), details[i].TroubleCode.Substring(2, 2), details[i].ClaimCategoryCode, details[i].OperationNumber, Convert.ToDecimal(details[i].ActualLaborTime) / 10
                        , Convert.ToDecimal(details[i].SubletWorkTime) / 10, 0, 0, 0, 0
                        , details[i].RepairDescription, CurrentUser.UserId, DateTime, CurrentUser.UserId, DateTime
                        );
                }
                else
                {
                    sql += string.Format(sqltplupd.Trim() + "\n"
                        , CompanyCode, BranchCode, ProductType, generateNo, i + 1
                        , "", details[i].IssueNo, details[i].IssueDate, 0, details[i].ServiceBookNo
                        , details[i].BasicModel, details[i].TechnicalModel, details[i].ChassisCode, details[i].ChassisNo, details[i].EngineCode
                        , details[i].EngineNo, details[i].RegisteredDate, details[i].RepairedDate, details[i].Odometer, details[i].KdOemCode.Equals("X") ? 0 : 1
                        , details[i].TroubleCode.Substring(0, 2), details[i].TroubleCode.Substring(2, 2), details[i].ClaimCategoryCode, details[i].OperationNumber, Convert.ToDecimal(details[i].ActualLaborTime) / 10
                        , Convert.ToDecimal(details[i].SubletWorkTime) / 10, details[i].RepairDescription, CurrentUser.UserId, DateTime
                        );
                }

                string sqldelpart = @"
                    delete svTrnClaimPart
                    where 1 = 1
                     and CompanyCode='{0}'
                     and BranchCode='{1}'
                     and ProductType='{2}'
                     and GenerateNo='{3}'
                     and GenerateSeq={4}
                    ";
                
                sql += string.Format(sqldelpart.Trim() + "\n"
                    , CompanyCode, BranchCode, ProductType, generateNo, i + 1
                    );

                int j = 1;
                foreach (WarrantyClaimPartFile partfile in details[i].ListPartFiles)
                {
                    foreach (WarrantyClaimPartFile.WarrantyClaimPart claimpart in partfile.WarrantyClaimParts)
                    {
                        string sqlpart = @"
                            INSERT INTO svTrnClaimPart 
                            (
                             CompanyCode,BranchCode,ProductType,GenerateNo,GenerateSeq
                            ,PartSeq,IsCausal,PartNo,ProcessedPartNo,Quantity
                            ,UnitPrice,TotalPrice,PaymentQuantity,PaymentTotalPrice,IsActive
                            ,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
                            )
                            VALUES
                            (
                             '{0}','{1}','{2}','{3}','{4}',
                             '{5}','{6}','{7}','{8}','{9}',
                             '{10}','{11}','{12}','{13}','{14}',
                             '{15}','{16}','{17}','{18}'
                            )
                            ";
                        decimal unitPrice = p_GetPrice(details[i], claimpart.PartNo, dtPrice);
                        decimal totalPrice = unitPrice * claimpart.Qty;

                        sql += string.Format(sqlpart.Trim() + "\n"
                        , CompanyCode, BranchCode, ProductType, generateNo, i + 1
                        , j++, claimpart.CausalPartCode.Equals("X") ? 1 : 0, claimpart.PartNo, claimpart.PartNo, claimpart.Qty
                        , unitPrice, totalPrice, 0, 0, 1
                        , CurrentUser.UserId, DateTime, CurrentUser.UserId, DateTime
                        );
                    }
                }
            }

            //sqlCmd.CommandText = sql.Trim();
            //int result = sqlCmd.ExecuteNonQuery();

            ctx.Database.ExecuteSqlCommand(sql.Trim());
            int result = ctx.SaveChanges();

            dt = null;
            sqlDA = null;
            sqlCmd = null;
            
            return result;
        }

        private int p_Save(string generateNo, List<WarrantyClaimDtlFile> details, bool bSPK)
        {
            DateTime DateTime = DateTime.Now;
            string table = (bSPK) ? "SvTrnClaimSPKApp" : "svTrnClaimApplication";

            string sql = string.Format(@"
                select GenerateSeq from {4}
                where CompanyCode='{0}'
                 and BranchCode='{1}'
                 and ProductType='{2}'
                 and GenerateNo='{3}'
                ", CompanyCode, BranchCode, ProductType, generateNo, table).Trim();

            SqlCommand sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            sqlCmd.CommandText = sql;
            sqlCmd.CommandType = CommandType.Text;
            SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
            DataTable dt = new DataTable();
            sqlDA.Fill(dt);

            Hashtable hash = new Hashtable();
            foreach (DataRow row in dt.Rows) hash[Convert.ToInt32(row["GenerateSeq"])] = true;

            sql = "";
            string sqltplins = @"INSERT INTO {35}
                (
                 CompanyCode,BranchCode,ProductType,GenerateNo,GenerateSeq
                ,InvoiceNo,IssueNo,IssueDate,ClaimStatus,ServiceBookNo
                ,BasicModel,TechnicalModel,ChassisCode,ChassisNo,EngineCode
                ,EngineNo,RegisteredDate,RepairedDate,Odometer,IsCBU
                ,ComplainCode,DefectCode,CategoryCode,OperationNo,OperationHour
                ,SubletHour,OperationAmt,SubletAmt,PartAmt,ClaimAmt
                ,TroubleDescription,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
                )
                VALUES
                (
                 '{0}','{1}','{2}','{3}','{4}',
                 '{5}','{6}','{7}','{8}','{9}',
                 '{10}','{11}','{12}','{13}','{14}',
                 '{15}','{16}','{17}','{18}','{19}',
                 '{20}','{21}','{22}','{23}','{24}',
                 '{25}','{26}','{27}','{28}','{29}',
                 '{30}','{31}','{32}','{33}','{34}'
                )
                ";
            
            string sqltplupd = @"
                UPDATE {29} SET
                 InvoiceNo='{5}'
                ,IssueNo='{6}'
                ,IssueDate='{7}'
                ,ClaimStatus='{8}'
                ,ServiceBookNo='{9}'
                ,BasicModel='{10}'
                ,TechnicalModel='{11}'
                ,ChassisCode='{12}'
                ,ChassisNo='{13}'
                ,EngineCode='{14}'
                ,EngineNo='{15}'
                ,RegisteredDate='{16}'
                ,RepairedDate='{17}'
                ,Odometer='{18}'
                ,IsCBU='{19}'
                ,ComplainCode='{20}'
                ,DefectCode='{21}'
                ,CategoryCode='{22}'
                ,OperationNo='{23}'
                ,OperationHour='{24}'
                ,SubletHour='{25}'
                ,TroubleDescription='{26}'
                ,LastUpdateBy='{27}'
                ,LastUpdateDate='{28}'
                WHERE 1 = 1
                 AND CompanyCode='{0}'
                 AND BranchCode='{1}'
                 AND ProductType='{2}'
                 AND GenerateNo='{3}'
                 AND GenerateSeq={4}
                ";

            DataTable dtPrice = p_SelectPrice(details);
            for (int i = 0; i < details.Count; i++)
            {
                if (hash[i + 1] == null)
                {
                    sql += string.Format(sqltplins.Trim() + "\n"
                        , CompanyCode, BranchCode, ProductType, generateNo, i + 1
                        , "", details[i].IssueNo, details[i].IssueDate, 0, details[i].ServiceBookNo
                        , details[i].BasicModel, details[i].TechnicalModel, details[i].ChassisCode, details[i].ChassisNo, details[i].EngineCode
                        , details[i].EngineNo, details[i].RegisteredDate, details[i].RepairedDate, details[i].Odometer, details[i].KdOemCode.Equals("X") ? 0 : 1
                        , details[i].TroubleCode.Substring(0, 2), details[i].TroubleCode.Substring(2, 2), details[i].ClaimCategoryCode, details[i].OperationNumber, Convert.ToDecimal(details[i].ActualLaborTime) / 10
                        , Convert.ToDecimal(details[i].SubletWorkTime) / 10, 0, 0, 0, 0
                        , details[i].RepairDescription, CurrentUser.UserId, DateTime, CurrentUser.UserId, DateTime
                        , table
                        );
                }
                else
                {
                    sql += string.Format(sqltplupd.Trim() + "\n"
                        , CompanyCode, BranchCode, ProductType, generateNo, i + 1
                        , "", details[i].IssueNo, details[i].IssueDate, 0, details[i].ServiceBookNo
                        , details[i].BasicModel, details[i].TechnicalModel, details[i].ChassisCode, details[i].ChassisNo, details[i].EngineCode
                        , details[i].EngineNo, details[i].RegisteredDate, details[i].RepairedDate, details[i].Odometer, details[i].KdOemCode.Equals("X") ? 0 : 1
                        , details[i].TroubleCode.Substring(0, 2), details[i].TroubleCode.Substring(2, 2), details[i].ClaimCategoryCode, details[i].OperationNumber, Convert.ToDecimal(details[i].ActualLaborTime) / 10
                        , Convert.ToDecimal(details[i].SubletWorkTime) / 10, details[i].RepairDescription, CurrentUser.UserId, DateTime
                        , table
                        );
                }

                string tblPart = (bSPK) ? "SvTrnClaimSPKPart" : "svTrnClaimPart";
                string sqldelpart = @"
                    delete {5}
                    where 1 = 1
                     and CompanyCode='{0}'
                     and BranchCode='{1}'
                     and ProductType='{2}'
                     and GenerateNo='{3}'
                     and GenerateSeq={4}
                    ";
                sql += string.Format(sqldelpart.Trim() + "\n"
                    , CompanyCode, BranchCode, ProductType, generateNo, i + 1
                    , tblPart
                    );

                int j = 1;
                foreach (WarrantyClaimPartFile partfile in details[i].ListPartFiles)
                {
                    foreach (WarrantyClaimPartFile.WarrantyClaimPart claimpart in partfile.WarrantyClaimParts)
                    {
                        string sqlpart = @"
                            INSERT INTO {19} 
                            (
                             CompanyCode,BranchCode,ProductType,GenerateNo,GenerateSeq
                            ,PartSeq,IsCausal,PartNo,ProcessedPartNo,Quantity
                            ,UnitPrice,TotalPrice,PaymentQuantity,PaymentTotalPrice,IsActive
                            ,CreatedBy,CreatedDate,LastUpdateBy,LastUpdateDate
                            )
                            VALUES
                            (
                             '{0}','{1}','{2}','{3}','{4}',
                             '{5}','{6}','{7}','{8}','{9}',
                             '{10}','{11}','{12}','{13}','{14}',
                             '{15}','{16}','{17}','{18}'
                            )
                            ";
                        decimal unitPrice = p_GetPrice(details[i], claimpart.PartNo, dtPrice);
                        decimal totalPrice = unitPrice * claimpart.Qty;

                        sql += string.Format(sqlpart.Trim() + "\n"
                        , CompanyCode, BranchCode, ProductType, generateNo, i + 1
                        , j++, claimpart.CausalPartCode.Equals("X") ? 1 : 0, claimpart.PartNo, claimpart.PartNo, claimpart.Qty
                        , unitPrice, totalPrice, 0, 0, 1
                        , CurrentUser.UserId, DateTime, CurrentUser.UserId, DateTime
                        , tblPart
                        );
                    }
                }
            }

            //sqlCmd.CommandText = sql.Trim();
            //int result = sqlCmd.ExecuteNonQuery();

            ctx.Database.ExecuteSqlCommand(sql.Trim());
            int result = ctx.SaveChanges();

            dt = null;
            sqlDA = null;
            sqlCmd = null;

            return result;
        }

        private DataTable p_SelectPrice(List<WarrantyClaimDtlFile> details)
        {
            string sql = @"
                select 
                 a.PartNo
                ,a.RetailPrice
                ,a.LastRetailPriceUpdate
                ,b.RetailPrice HRetailPrice
                ,b.UpdateDate HUpdateDate
                ,b.LastRetailPriceUpdate HLastRetailPriceUpdate
                from spMstItemPrice a
                left join spHstItemPrice b
                  on b.CompanyCode = a.CompanyCode
                 and b.BranchCode = a.BranchCode
                 and b.PartNo = a.PartNo
                where 1 = 1
                 and a.CompanyCode='{0}'
                 and a.BranchCode='{1}'
                 and a.PartNo in({2})
                order by b.UpdateDate desc, b.LastRetailPriceUpdate desc
                ";
            
            string parts = ""; Hashtable hash = new Hashtable();
            foreach (WarrantyClaimDtlFile detail in details)
            {
                foreach (WarrantyClaimPartFile partfile in detail.ListPartFiles)
                {
                    foreach (WarrantyClaimPartFile.WarrantyClaimPart claimpart in partfile.WarrantyClaimParts)
                    {
                        if (hash[claimpart.PartNo] == null)
                        {
                            hash[claimpart.PartNo] = true;
                            parts += string.Format(",'{0}'", claimpart.PartNo);
                        }
                    }
                }
            }
            parts = (parts.Length > 1) ? parts.Substring(1) : "";
            sql = string.Format(sql.Trim(), CompanyCode, BranchCode, parts);

            using (SqlCommand sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                sqlCmd.CommandText = sql.Trim();
                sqlCmd.CommandType = CommandType.Text;
                SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
                DataTable dt = new DataTable();
                sqlDA.Fill(dt);

                return dt;
            }
        }

        private decimal p_GetPrice(WarrantyClaimDtlFile detail, string partno, DataTable dt)
        {
            DataRow[] rows;

            rows = dt.Select(string.Format("PartNo='{0}'", partno));
            if (rows != null && rows.Length > 0)
            {
                DateTime lastRtlPriceUpd = Convert.ToDateTime(rows[0]["LastRetailPriceUpdate"]).Date;
                DateTime repairedDate = detail.RepairedDate.Date;
                if (DateTime.Compare(repairedDate, lastRtlPriceUpd) >= 0)
                {
                    return Convert.ToDecimal(rows[0]["RetailPrice"]);
                }
                else
                {
                    foreach (DataRow row in rows)
                    {
                        DateTime histUpdDate = Convert.ToDateTime(row["HUpdateDate"]).Date;
                        DateTime histLastPriceUpdDate = Convert.ToDateTime(row["HLastRetailPriceUpdate"]).Date;
                        if (DateTime.Compare(repairedDate, histUpdDate) >= 0 && DateTime.Compare(repairedDate, histLastPriceUpdDate) >= 0)
                        {
                            return Convert.ToDecimal(row["HRetailPrice"]);
                        }
                    }
                }
            }

            return 0;
        }

        private decimal p_GetLaborRate(DateTime repairedDate)
        {
            var oSvMstLaborRate = ctx.SvMstTarifJasas.Where(p => p.IsActive == true && p.LaborCode == "SUZUKI"
                && p.CompanyCode == CompanyCode && p.BranchCode == BranchCode
                && p.ProductType == ProductType && p.EffectiveDate <= repairedDate
                ).OrderByDescending(p => p.EffectiveDate).FirstOrDefault();

            return (oSvMstLaborRate != null) ? Convert.ToDecimal(oSvMstLaborRate.LaborPrice) : 0;
        }
        
        private string p_GetNewDocumentNo(string doctype, DateTime? transdate)
        {
            string newDocument = ctx.Database.SqlQuery<string>("exec uspfn_GnDocumentGetNew @p0, @p1, @p2, @p3, @p4", 
                CompanyCode, BranchCode, doctype, CurrentUser.UserId, transdate).FirstOrDefault();

            return newDocument;
        }
        #endregion
        #endregion

        #region -- Receive Warranty Claim --

        public bool Receive(WClaimRcvHdrFile header, List<WClaimRcvDtlFile> details, out string msg)
        {
            //List<string> list = ListGenerateNo(header, out msg);
            List<string> list = p_ListGenerateNo(details, out msg);
            if (list.Count > 0)
            {
                string sql = "";
                foreach (string generateNo in list)
                {
                    decimal frt, csh, clc, csc, cpc, ctc;
                    sql += p_GetSqlUpdateDtl(generateNo, header, details, out frt, out csh, out clc, out csc, out cpc, out ctc);
                    sql += p_GetSqlUpdateHdr(generateNo, header, frt, csh, clc, csc, cpc, ctc);
                }

                int i = -1;
                try
                {
                    using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        ctx.Database.ExecuteSqlCommand(sql.Trim());
                        i = ctx.SaveChanges();
                        tranScope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    i = -1;
                    throw new Exception(ex.Message);
                }
                
                return i > 0;
            }

            return false;
        }

        public bool ReceiveSPK(WClaimRcvHdrFile header, List<WClaimRcvDtlFile> details, out string msg)
        {
            //List<string> list = ListGenerateNo(header, out msg);
            List<string> list = p_ListGenerateNoSPK(details, out msg);
            if (list.Count > 0)
            {
                string sql = "";
                foreach (string generateNo in list)
                {
                    decimal frt, csh, clc, csc, cpc, ctc;
                    sql += p_GetSqlUpdateDtlSPK(generateNo, header, details, out frt, out csh, out clc, out csc, out cpc, out ctc);
                    sql += p_GetSqlUpdateHdrSPK(generateNo, header, frt, csh, clc, csc, cpc, ctc);
                }

                int i = -1;
                try
                {
                    using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        ctx.Database.ExecuteSqlCommand(sql.Trim());
                        i = ctx.SaveChanges();
                        tranScope.Complete();
                    }
                }
                catch (Exception ex)
                {
                    i = -1;
                    throw new Exception(ex.Message);
                }

                return i > 0;
            }

            return false;
        }

        public WClaimRcvHdrFile GetWRcvClaimHdrFile(string dataID, string reimbursementNo, string receiveDealerCode, string senderDealerName, DateTime receiveDate)
        {
            WClaimRcvHdrFile file = new WClaimRcvHdrFile();
            using (var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                sqlCmd.CommandText = "uspfn_SvGetWRcvClaimHdrFile";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                sqlCmd.Parameters.AddWithValue("@ProductType", ProductType);
                sqlCmd.Parameters.AddWithValue("@DataID", dataID);
                sqlCmd.Parameters.AddWithValue("@ReimbursementNo", reimbursementNo);
                sqlCmd.Parameters.AddWithValue("@ReceiveDealerCode", receiveDealerCode);
                sqlCmd.Parameters.AddWithValue("@ReceiveDate", receiveDate);
                sqlCmd.Parameters.AddWithValue("@SenderDealerName", senderDealerName);
                var reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    file.DealerCode = reader["DealerCode"].ToString();
                    file.RcvDealerCode = reader["ReceivedDealerCode"].ToString();
                    file.DealerName = reader["DealerName"].ToString();
                    file.TotalItem = Convert.ToInt32(reader["TotalItems"]);
                    file.ProductType = reader["ProductType"].ToString();
                    file.ReimbursementNo = reader["ReimbursementNo"].ToString();
                    file.ReimbursementDate = (reader["ReimbursementDate"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(reader["ReimbursementDate"]); ;
                }
            }

            return file;
        }

        public List<WClaimRcvDtlFile> GetWRcvClaimDtlFile(string reimbursement, string receivedDealerCode, DateTime receiveDate)
        {
            List<WClaimRcvDtlFile> files = new List<WClaimRcvDtlFile>();
            using (var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                sqlCmd.CommandText = "uspfn_SvGetWRcvClaimDtlFile";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("@CompanyCode", CompanyCode);
                sqlCmd.Parameters.AddWithValue("@BranchCode", BranchCode);
                sqlCmd.Parameters.AddWithValue("@ProductType", ProductType);
                sqlCmd.Parameters.AddWithValue("@Reimbursement", reimbursement);
                sqlCmd.Parameters.AddWithValue("@ReceivedDate", receiveDate);
                sqlCmd.Parameters.AddWithValue("@ReceivedDealerCode", receivedDealerCode);
                var reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    WClaimRcvDtlFile file = new WClaimRcvDtlFile();
                    file.IssueNo = Convert.ToString(reader["IssueNo"]);
                    file.ClaimDivisionCode = Convert.ToString(reader["DivisionCode"]);
                    file.JudgementCode = Convert.ToString(reader["JudgementCode"]);
                    file.LotNo = Convert.ToInt32(reader["LotNo"]);
                    file.ServiceBookNo = Convert.ToString(reader["ServiceBookNo"]);
                    file.ChassisCode = Convert.ToString(reader["ChassisCode"]);
                    file.ChassisNo = Convert.ToString(reader["ChassisNo"]);
                    file.EngineCode = Convert.ToString(reader["EngineCode"]);
                    file.EngineNo = Convert.ToString(reader["EngineNo"]);
                    file.PertClaimTotalCost = (reader["TotalClaimAmt"] is DBNull) ? 0 : Convert.ToInt32(reader["TotalClaimAmt"]);
                    file.PertClaimPartsCost = (reader["PartCost"] is DBNull) ? 0 : Convert.ToInt32(reader["PartCost"]);
                    file.PertClaimShippingChargesCost = 0;
                    file.PertClaimLaborCost = (reader["PaymentOprAmt"] is DBNull) ? 0 : Convert.ToInt32(reader["PaymentOprAmt"]);
                    file.PertClaimSubletCost = (reader["PaymentSubletAmt"] is DBNull) ? 0 : Convert.ToInt32(reader["PaymentSubletAmt"]);
                    file.PertClaimSubletHours = Convert.ToInt32(reader["SubletWorkTime"]);
                    file.OperationNumber = Convert.ToString(reader["OperationNo"]);
                    file.FlatRateTime = Convert.ToString(reader["ActualLaborTime"]);
                    file.TechnicalModel = Convert.ToString(reader["TechnicalModel"]);
                    files.Add(file);
                    p_SetWRcvClaimPartFiles(file, reimbursement, receivedDealerCode, Convert.ToInt32(reader["GenerateSeq"]), receiveDate);
                }
            }

            return files;
        }
        
        #region -- Private Methods --

        private List<string> p_ListGenerateNo(List<WClaimRcvDtlFile> details, out string msg)
        {
            string issueNo = string.Empty;
            foreach (WClaimRcvDtlFile detail in details)
            {
                issueNo += ",'" + detail.IssueNo + "'";
            }
            if (issueNo.Length > 3)
            {
                issueNo = issueNo.Substring(1);
            }
            string sql = string.Format(@"
                -- Untuk check issue no di file dengan di database
                select GenerateNo, IssueNo from SvTrnClaimApplication 
                where 
                CompanyCode = '{0}' and
                BranchCode = '{1}' and
                ProductType = '{2}' and
                IssueNo in ({3}) 
                order by GenerateNo;

                -- Untuk mengambil GenerateNo/ Claim no untuk di update sesuai dengan issue no
                select DISTINCT GenerateNo from SvTrnClaimApplication 
                where 
                CompanyCode = '{0}' and
                BranchCode = '{1}' and
                ProductType = '{2}' and
                IssueNo in ({3}) 
                order by GenerateNo;

                ", CompanyCode, BranchCode, ProductType, issueNo);

            List<string> list_all = new List<string>();
            using (var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                sqlCmd.CommandText = sql.Trim();
                sqlCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
                sqlDA.Fill(ds);
            
                msg = string.Empty;
                if (ds.Tables[0].Rows.Count == details.Count)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                        list_all.Add(row["GenerateNo"].ToString());
                }
                else
                {
                    msg = "Ada Issue No yang tidak cocok antara di File dengan di Database!";
                }
            }
            
            return list_all;
        }

        private List<string> p_ListGenerateNoSPK(List<WClaimRcvDtlFile> details, out string msg)
        {
            string issueNo = string.Empty;
            foreach (WClaimRcvDtlFile detail in details)
            {
                issueNo += ",'" + detail.IssueNo + "'";
            }
            if (issueNo.Length > 3)
            {
                issueNo = issueNo.Substring(1);
            }
            string sql = string.Format(@"
                -- Untuk check issue no di file dengan di database
                select GenerateNo, IssueNo from svTrnClaimSPKApp
                where 
                CompanyCode = '{0}' and
                BranchCode = '{1}' and
                ProductType = '{2}' and
                IssueNo in ({3}) 
                order by GenerateNo;

                -- Untuk mengambil GenerateNo/ Claim no untuk di update sesuai dengan issue no
                select DISTINCT GenerateNo from svTrnClaimSPKApp
                where 
                CompanyCode = '{0}' and
                BranchCode = '{1}' and
                ProductType = '{2}' and
                IssueNo in ({3}) 
                order by GenerateNo;

                ", CompanyCode, BranchCode, ProductType, issueNo);

            List<string> list_all = new List<string>();
            using (var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                sqlCmd.CommandText = sql.Trim();
                sqlCmd.CommandType = CommandType.Text;
                DataSet ds = new DataSet();
                SqlDataAdapter sqlDA = new SqlDataAdapter(sqlCmd);
                sqlDA.Fill(ds);

                msg = string.Empty;
                if (ds.Tables[0].Rows.Count == details.Count)
                {
                    foreach (DataRow row in ds.Tables[1].Rows)
                        list_all.Add(row["GenerateNo"].ToString());
                }
                else
                {
                    msg = "Ada Issue No yang tidak cocok antara di File dengan di Database!";
                }

            }
            
            return list_all;
        }

        private List<string> p_ListGenerateNo(WClaimRcvHdrFile header, out string msg)
        {
            string sql = string.Format(@"
                select b.GenerateNo, b.PostingFlag from svTrnClaimBatch a
                inner join svTrnClaim b on b.CompanyCode=a.CompanyCode
                 and b.BranchCode=a.BranchCode
                 and b.ProductType=a.ProductType
                 and b.BatchNo=a.BatchNo
                where a.CompanyCode='{0}'
                 and a.BranchCode='{1}'
                 and a.ProductType='{2}'
                 and a.ReceiptNo='{3}'
                order by b.GenerateNo desc
                ", CompanyCode, BranchCode, ProductType, header.ReimbursementNo);

            List<string> list_selected = new List<string>();
            using (var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                List<string> list_all = new List<string>();

                sqlCmd.CommandText = sql.Trim();
                sqlCmd.CommandType = CommandType.Text;
                var reader = sqlCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string flag = (reader["PostingFlag"] is DBNull) ? "" : reader["PostingFlag"].ToString();
                        string genno = reader["GenerateNo"].ToString();
                        if (flag != "4") list_selected.Add(genno);
                        list_all.Add(genno);
                    }

                    msg = "";
                    if (list_all.Count > list_selected.Count && list_selected.Count > 0) msg = "Sebagian Data Sudah Pernah Di Receive!";
                    if (list_all.Count > 0 && list_selected.Count == 0) msg = "Semua Data Sudah Pernah Di Receive!";
                    if (list_all.Count == 0 && list_selected.Count == 0) msg = "Data Tidak Di Temukan !";

                }
                else
                {
                    msg = "";
                    reader.Close();
                }
            }
            
            return list_selected;
        }

        private string p_GetSqlUpdateDtl(string generateNo, WClaimRcvHdrFile header, List<WClaimRcvDtlFile> details, out decimal frt, out decimal csh, out decimal clc, out decimal csc, out decimal cpc, out decimal ctc)
        {
            frt = 0; csh = 0; clc = 0; csc = 0; cpc = 0; ctc = 0;

            string sql = ""; 
            foreach (WClaimRcvDtlFile detail in details)
            {
                ClaimApplication oClaimDetail = null;
                if (!detail.IssueNo.Equals(string.Empty) || detail.IssueNo != null)
                    oClaimDetail = (detail.IssueNo.Split('-').Length > 1) ?
                        p_GetClaimDetail(detail.IssueNo, detail.IssueNo.Split('-')[1]) :
                        p_GetClaimDetail(detail.IssueNo);

                if (oClaimDetail.GenerateNo.Equals(generateNo))
                {
                    if (oClaimDetail != null)
                    {
                        foreach (WClaimRcvPartFile part in detail.ListPartFiles)
                        {
                            sql += string.Format(" update svTrnClaimPart set");
                            sql += string.Format(" ProcessedPartNo='{0}'", part.ProcessedPartNumber);
                            sql += string.Format(",PaymentQuantity='{0}'", part.Quantity);
                            sql += string.Format(",PaymentTotalPrice='{0}'", part.SparePartTotalCost);
                            sql += string.Format(",LastUpdateBy='{0}'", CurrentUser.UserId);
                            sql += string.Format(",LastUpdateDate=getdate()");
                            sql += string.Format(" where CompanyCode='{0}'", CompanyCode);
                            sql += string.Format(" and BranchCode='{0}'", BranchCode);
                            sql += string.Format(" and ProductType='{0}'", ProductType);
                            sql += string.Format(" and GenerateNo='{0}'", oClaimDetail.GenerateNo);
                            sql += string.Format(" and GenerateSeq='{0}'", oClaimDetail.GenerateSeq);
                            sql += string.Format(" and PartNo like '{0}%'", part.OriginalPartNumber);
                            sql += "\n";
                        }
                    }

                    sql += string.Format(@"
                        insert into svTrnClaimJudgement
                        (
                         CompanyCode,BranchCode,ProductType,GenerateNo,GenerateSeq
                        ,ReceivedDate,SuzukiRefferenceNo,SuzukiRefferenceDate,DivisionCode,JudgementCode
                        ,PaymentOprHour,PaymentSubletHour,PaymentOprAmt,PaymentSubletAmt,CreatedBy
                        ,CreatedDate,LastupdateBy,LastupdateDate
                        )
                        values
                        (
                         '{0}','{1}','{2}','{3}','{4}',
                         getdate(),'{5}','{6}','{7}','{8}',
                         '{9}','{10}','{11}','{12}','{13}',
                         getdate(),'{14}',getdate()
                        );
                        ", CompanyCode, BranchCode, ProductType, oClaimDetail.GenerateNo, oClaimDetail.GenerateSeq
                             , header.ReimbursementNo, header.ReimbursementDate.ToString("yyyy-MM-dd"), detail.ClaimDivisionCode, detail.JudgementCode
                             , Convert.ToDecimal(detail.FlatRateTime) / 10, Convert.ToDecimal(detail.PertClaimSubletHours / 10), detail.PertClaimLaborCost, detail.PertClaimSubletCost, CurrentUser.UserId
                             , CurrentUser.UserId
                             );

                    sql += string.Format(@"
                        update SvTrnClaimApplication set PaymentNo = '{0}', PaymentDate = '{1}', lastupdateBy = '{2}', lastupdatedate = getDate()
                        where CompanyCode = '{3}' and BranchCode = '{4}' and ProductType = '{5}' and generateNo = '{6}' and generateSeq = '{7}';
                        ", header.ReimbursementNo, header.ReimbursementDate.ToString("yyyy-MM-dd"), CurrentUser.UserId, CurrentUser.CompanyCode
                            , BranchCode, ProductType, oClaimDetail.GenerateNo, oClaimDetail.GenerateSeq);

                    frt += Convert.ToDecimal(detail.FlatRateTime) / 10;
                    csh += detail.PertClaimSubletHours;
                    clc += detail.PertClaimLaborCost;
                    csc += detail.PertClaimSubletCost;
                    cpc += detail.PertClaimPartsCost;
                    ctc += detail.PertClaimTotalCost;
                }
            }

            return sql;
        }

        private string p_GetSqlUpdateDtlSPK(string generateNo, WClaimRcvHdrFile header, List<WClaimRcvDtlFile> details, out decimal frt, out decimal csh, out decimal clc, out decimal csc, out decimal cpc, out decimal ctc)
        {
            frt = 0; csh = 0; clc = 0; csc = 0; cpc = 0; ctc = 0;

            string sql = "";
            foreach (WClaimRcvDtlFile detail in details)
            {
                SvTrnClaimSPKApp oClaimDetail = null;
                if (!detail.IssueNo.Equals(string.Empty) || detail.IssueNo != null)
                    oClaimDetail = (detail.IssueNo.Split('-').Length > 1) ?
                        p_GetClaimDetailSPK(detail.IssueNo, detail.IssueNo.Split('-')[1]) :
                        p_GetClaimDetailSPK(detail.IssueNo);

                if (oClaimDetail.GenerateNo.Equals(generateNo))
                {
                    if (oClaimDetail != null)
                    {
                        foreach (WClaimRcvPartFile part in detail.ListPartFiles)
                        {
                            sql += string.Format(" update svTrnClaimSPKPart set");
                            sql += string.Format(" ProcessedPartNo='{0}'", part.ProcessedPartNumber);
                            sql += string.Format(",PaymentQuantity='{0}'", part.Quantity);
                            sql += string.Format(",PaymentTotalPrice='{0}'", part.SparePartTotalCost);
                            sql += string.Format(",LastUpdateBy='{0}'", CurrentUser.UserId);
                            sql += string.Format(",LastUpdateDate=getdate()");
                            sql += string.Format(" where CompanyCode='{0}'", CompanyCode);
                            sql += string.Format(" and BranchCode='{0}'", BranchCode);
                            sql += string.Format(" and ProductType='{0}'", ProductType);
                            sql += string.Format(" and GenerateNo='{0}'", oClaimDetail.GenerateNo);
                            sql += string.Format(" and GenerateSeq='{0}'", oClaimDetail.GenerateSeq);
                            sql += string.Format(" and PartNo like '{0}%'", part.OriginalPartNumber);
                            sql += "\n";
                        }
                    }

                    sql += string.Format(@"
                        insert into svTrnClaimJudgement
                        (
                         CompanyCode,BranchCode,ProductType,GenerateNo,GenerateSeq
                        ,ReceivedDate,SuzukiRefferenceNo,SuzukiRefferenceDate,DivisionCode,JudgementCode
                        ,PaymentOprHour,PaymentSubletHour,PaymentOprAmt,PaymentSubletAmt,CreatedBy
                        ,CreatedDate,LastupdateBy,LastupdateDate
                        )
                        values
                        (
                         '{0}','{1}','{2}','{3}','{4}',
                         getdate(),'{5}','{6}','{7}','{8}',
                         '{9}','{10}','{11}','{12}','{13}',
                         getdate(),'{14}',getdate()
                        );
                        ", CompanyCode, BranchCode, ProductType, oClaimDetail.GenerateNo, oClaimDetail.GenerateSeq
                             , header.ReimbursementNo, header.ReimbursementDate.ToString("yyyy-MM-dd"), detail.ClaimDivisionCode, detail.JudgementCode
                             , Convert.ToDecimal(detail.FlatRateTime) / 10, Convert.ToDecimal(detail.PertClaimSubletHours / 10), detail.PertClaimLaborCost, detail.PertClaimSubletCost, CurrentUser.UserId
                             , CurrentUser.UserId
                             );
                    
                    sql += string.Format(@"
                        update SvTrnClaimSPKApp set PaymentNo = '{0}', PaymentDate = '{1}', lastupdateBy = '{2}', lastupdatedate = getDate()
                        where CompanyCode = '{3}' and BranchCode = '{4}' and ProductType = '{5}' and generateNo = '{6}' and generateSeq = '{7}';
                        ", header.ReimbursementNo, header.ReimbursementDate.ToString("yyyy-MM-dd"), CurrentUser.UserId, CompanyCode
                         , BranchCode, ProductType, oClaimDetail.GenerateNo, oClaimDetail.GenerateSeq);

                    frt += Convert.ToDecimal(detail.FlatRateTime) / 10;
                    csh += detail.PertClaimSubletHours;
                    clc += detail.PertClaimLaborCost;
                    csc += detail.PertClaimSubletCost;
                    cpc += detail.PertClaimPartsCost;
                    ctc += detail.PertClaimTotalCost;
                }
            }

            return sql;
        }

        private string p_GetSqlUpdateHdr(string generateNo, WClaimRcvHdrFile header, decimal frt, decimal csh, decimal clc, decimal csc, decimal cpc, decimal ctc)
        {
            string sql = "";

            // nilai payment merupakan penambahan nilai claim sebelumnya ditambah dengan kekurangannya.
            sql += string.Format(" update svTrnClaim set");
            sql += string.Format(" TotalOperationPayHour = TotalOperationPayHour + {0}", frt);
            sql += string.Format(",TotalSubletPayHour = TotalSubletPayHour + {0}", csh);
            sql += string.Format(",TotalOperationPaymentAmt = TotalOperationPaymentAmt  + {0}", clc);
            sql += string.Format(",TotalSubletPaymentAmt = TotalSubletPaymentAmt + {0}", csc);
            sql += string.Format(",TotalPartPaymentAmt = TotalPartPaymentAmt + {0}", cpc);
            sql += string.Format(",TotalClaimPaymentAmt = TotalClaimPaymentAmt + {0}", ctc);
            sql += ",PostingFlag = 4, LastupdateDate = getDate()";// setelah diterima daris suzuki, posting flag Claim = 4.
            sql += string.Format(" ,LastupdateBy = '{0}'", CurrentUser.UserId);
            sql += string.Format(" where CompanyCode = '{0}'", CompanyCode);
            sql += string.Format(" and BranchCode = '{0}'", BranchCode);
            sql += string.Format(" and ProductType = '{0}'", ProductType);
            sql += string.Format(" and GenerateNo = '{0}'", generateNo);
            sql += "\n";

            return sql;
        }

        private string p_GetSqlUpdateHdrSPK(string generateNo, WClaimRcvHdrFile header, decimal frt, decimal csh, decimal clc, decimal csc, decimal cpc, decimal ctc)
        {
            string sql = "";

            // nilai payment merupakan penambahan nilai claim sebelumnya ditambah dengan kekurangannya.
            sql += string.Format(" update svTrnClaimSPK set");
            sql += string.Format(" TotalOperationPayHour = TotalOperationPayHour + {0}", frt);
            sql += string.Format(",TotalSubletPayHour = TotalSubletPayHour + {0}", csh);
            sql += string.Format(",TotalOperationPaymentAmt = TotalOperationPaymentAmt  + {0}", clc);
            sql += string.Format(",TotalSubletPaymentAmt = TotalSubletPaymentAmt + {0}", csc);
            sql += string.Format(",TotalPartPaymentAmt = TotalPartPaymentAmt + {0}", cpc);
            sql += string.Format(",TotalClaimPaymentAmt = TotalClaimPaymentAmt + {0}", ctc);
            sql += ",PostingFlag = 4, LastupdateDate = getDate()";// setelah diterima daris suzuki, posting flag Claim = 4.
            sql += string.Format(" ,LastupdateBy = '{0}'", CurrentUser.UserId);
            sql += string.Format(" where CompanyCode = '{0}'", CompanyCode);
            sql += string.Format(" and BranchCode = '{0}'", BranchCode);
            sql += string.Format(" and ProductType = '{0}'",ProductType);
            sql += string.Format(" and GenerateNo = '{0}'", generateNo);
            sql += "\n";

            return sql;
        }

        private ClaimApplication p_GetClaimDetail(string issueNo)
        {
            return p_GetClaimDetail(issueNo, null);
        }

        private SvTrnClaimSPKApp p_GetClaimDetailSPK(string issueNo)
        {
            return p_GetClaimDetailSPK(issueNo, null);
        }

        private ClaimApplication p_GetClaimDetail(string issueNo, string custAbbrNo)
        {
            string sql = @"
                select a.* from svTrnClaimApplication a
                left join svTrnClaim b on b.GenerateNo = a.GenerateNo
                 and b.CompanyCode = a.CompanyCode
                 and b.BranchCode = a.BranchCode
                 and b.ProductType = a.ProductType
                left join gnMstCustomer c on c.CustomerCode = b.SenderDealerCode
                 and c.CompanyCode = a.CompanyCode
                where 1 = 1
                 and a.CompanyCode = '{0}'
                 and a.BranchCode = '{1}'
                 and a.ProductType = '{2}'
                 and a.IssueNo = '{3}'
                ";
            // digunakan untuk issueno yang datang dari subdealer (issueno dari subdealer ditambah ABBR Name)
            if (custAbbrNo != null)
            {
                sql += "and c.CustomerAbbrName = '{4}'";
                sql = string.Format(sql, CompanyCode, BranchCode, ProductType, issueNo, custAbbrNo);
            }
            // digunakan untuk issue no yang dari SPK main dealer (tidak ada penambahan ABBR name)
            else
            {
                sql = string.Format(sql, CompanyCode, BranchCode, ProductType, issueNo, custAbbrNo);
            }
            
            return ctx.Database.SqlQuery<ClaimApplication>(sql.Trim()).FirstOrDefault();
        }

        private SvTrnClaimSPKApp p_GetClaimDetailSPK(string issueNo, string custAbbrNo)
        {
            string sql = @"
                select a.* from svTrnClaimSPKApp a
                left join svTrnClaimSPK b on b.GenerateNo = a.GenerateNo
                 and b.CompanyCode = a.CompanyCode
                 and b.BranchCode = a.BranchCode
                 and b.ProductType = a.ProductType
                left join gnMstCustomer c on c.CustomerCode = b.SenderDealerCode
                 and c.CompanyCode = a.CompanyCode
                where 1 = 1
                 and a.CompanyCode = '{0}'
                 and a.BranchCode = '{1}'
                 and a.ProductType = '{2}'
                 and a.IssueNo = '{3}'
                ";
            
            // digunakan untuk issueno yang datang dari subdealer (issueno dari subdealer ditambah ABBR Name)
            if (custAbbrNo != null && custAbbrNo != "")
            {
                sql += "and c.CustomerAbbrName = '{4}'";
                sql = string.Format(sql, CompanyCode, BranchCode, ProductType, issueNo, custAbbrNo);
            }
            // digunakan untuk issue no yang dari SPK main dealer (tidak ada penambahan ABBR name)
            else
            {
                sql = string.Format(sql, CompanyCode, BranchCode, ProductType, issueNo, custAbbrNo);
            }

            return ctx.Database.SqlQuery<SvTrnClaimSPKApp>(sql.Trim()).FirstOrDefault();
        }

        private ClaimApplication p_GetClaimApp(string p)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public WClaimRcvHdrFile p_GetWRcvClaimHdrFile(string dataID, string reimbursementNo, string receiveDealerCode, string senderDealerName, DateTime receiveDate)
        {
            string sql = string.Format(@"
                declare @CompanyCode varchar(15)
                declare @BranchCode varchar(15)
                declare @ProductType varchar(15) 
                declare @DataID varchar(15) 
                declare @ReimbursementNo varchar(15) 
                declare @ReceiveDealerCode varchar(15)
                declare @ReceiveDate varchar(15)
                declare @SenderDealerName varchar(50) 

                set @CompanyCode = '{0}'
                set @BranchCode = '{1}'
                set @ProductType = '{2}'
                set @DataID = '{3}'
                set @ReimbursementNo = '{4}'
                set @ReceiveDealerCode = '{5}'
                set @ReceiveDate = '{6}'
                set @SenderDealerName = '{7}'

                select
	                RecordID, DataID, DealerCode, ReceivedDealerCode, ReceivedDealerName
	                , DealerName, TotalItems = (		
		                select 
			                count(app.GenerateSeq)	
		                from 
			                SvTrnClaim cla 
			                inner join SvTrnClaimJudgement jud on cla.CompanyCode = jud.CompanyCode 
				                and cla.BranchCode = jud.BranchCode and cla.ProductType = jud.ProductType
				                and cla.GenerateNo = jud.GenerateNo
			                inner join SvtrnClaimApplication app on cla.CompanyCode = app.CompanyCode 
				                and cla.BranchCode = app.BranchCode and cla.ProductType = app.ProductType
				                and cla.GenerateNo = app.GenerateNo and app.GenerateSeq = jud.GenerateSeq
		                where
			                cla.CompanyCode = @CompanyCode
			                and cla.BranchCode = @BranchCode
			                and cla.ProductType = @ProductType
			                and jud.SuzukiRefferenceNo = @ReimbursementNo
			                and cla.SenderDealerCode = @ReceiveDealerCode
			                and Convert(varchar, jud.ReceivedDate , 110) = @ReceiveDate	
	                ), ProductType = '4W' , ReimbursementNo
	                , ReimbursementDate , BlankFiller = ''
                from (
	                select TOP 1
		                RecordID = 'H'
		                , DataID = @DataID
                        , DealerCode = a.CompanyCode
		                , a.SenderDealerCode ReceivedDealerCode
		                , a.SenderDealerName ReceivedDealerName
		                , DealerName = @SenderDealerName
		                , ProductType = a.ProductType
		                , b.SuzukiRefferenceNo ReimbursementNo
		                , SuzukiRefferenceDate ReimbursementDate
		                , BlankFiller = ''
	                from 
		                SvTrnClaim a
		                inner join SvTrnClaimJudgement b on a.CompanyCode = b.CompanyCode 
			                and a.BranchCode = b.BranchCode and a.ProductType = b.ProductType
			                and a.GenerateNo = b.GenerateNo
	                where
		                a.CompanyCode = @CompanyCode
		                and a.BranchCode = @BranchCode
		                and a.ProductType = @ProductType
		                and b.SuzukiRefferenceNo = @ReimbursementNo
		                and a.SenderDealerCode = @ReceiveDealerCode
		                and Convert(varchar, b.ReceivedDate, 110) = @ReceiveDate	
                ) as Header
                ", CompanyCode, BranchCode, ProductType, dataID, reimbursementNo,
                 receiveDealerCode, receiveDate.ToString("MM-dd-yyyy"), senderDealerName);

            WClaimRcvHdrFile file = new WClaimRcvHdrFile();
            using( var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand){
                sqlCmd.CommandText = sql.Trim();
                sqlCmd.CommandType = CommandType.Text;
                var reader = sqlCmd.ExecuteReader();
                if (reader.Read())
                {
                    file.DealerCode = reader["DealerCode"].ToString();
                    file.RcvDealerCode = reader["ReceivedDealerCode"].ToString();
                    file.DealerName = reader["DealerName"].ToString();
                    file.TotalItem = Convert.ToInt32(reader["TotalItems"]);
                    file.ProductType = reader["ProductType"].ToString();
                    file.ReimbursementNo = reader["ReimbursementNo"].ToString();
                    file.ReimbursementDate = (reader["ReimbursementDate"] is DBNull) ? DateTime.MinValue : Convert.ToDateTime(reader["ReimbursementDate"]); ;
                }
            }
            
            return file;
        }

        private void p_SetWRcvClaimPartFiles(WClaimRcvDtlFile detail, string reimbursement, string receivedDealerCode, int generateSeq, DateTime receivedDate)
        {
            string sql = string.Format(@"
                declare @CompanyCode varchar(15)
                declare @BranchCode varchar(15)
                declare @ProductType varchar(15) 
                declare @Reimbursement varchar(15)
                declare @ReceivedDate varchar(15)
                declare @ReceivedDealerCode varchar(15)
                declare @GenerateSeq int

                set @CompanyCode = '{0}'
                set @BranchCode = '{1}'
                set @ProductType = '{2}'
                set @Reimbursement = '{3}'
                set @ReceivedDate = '{4}'
                set @ReceivedDealerCode = '{5}'
                set @GenerateSeq = '{6}'


                Select 
	                a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo, a.GenerateSeq
	                ,b.PartSeq, b.PartNo, b.ProcessedPartNo, b.Quantity, b.PaymentQuantity, b.PaymentTotalPrice
	                ,d.PartName PartOriginalName, e.PartName PartProcesedName
                from 
                SvTrnClaimJudgement a
	                inner join SvTrnClaimPart b on a.CompanyCode = b.CompanyCode 
		                and a.BranchCode = b.BranchCode and a.ProductType = b.ProductType
		                and a.GenerateNo = b.GenerateNo and a.GenerateSeq = b.GenerateSeq
	                inner join SvTrnClaim c on a.CompanyCode = c.CompanyCode 
		                and a.BranchCode = b.BranchCode and a.ProductType = c.ProductType
		                and a.GenerateNo = c.GenerateNo
	                left join SpMstItemInfo d on a.CompanyCode = d.CompanyCode and b.PartNo = d.PartNo
	                left join SpMstItemInfo e on a.CompanyCode = e.CompanyCode and b.ProcessedPartNo = e.PartNo
                where 
	                a.CompanyCode = @CompanyCode
	                and a.BranchCode = @BranchCode
	                and a.ProductType = @ProductType
	                and a.SuzukiRefferenceNo = @Reimbursement
	                and Convert(varchar,a.ReceivedDate,110) = @ReceivedDate
	                and c.SenderDealerCode = @ReceivedDealerCode
	                and a.GenerateSeq = @GenerateSeq
                ", CompanyCode, BranchCode, ProductType, reimbursement, receivedDate.ToString("MM-dd-yyyy"), receivedDealerCode, generateSeq);

            using (var sqlCmd = ctx.Database.Connection.CreateCommand() as SqlCommand)
            {
                sqlCmd.CommandText = sql.Trim();
                sqlCmd.CommandType = CommandType.Text;
                var reader = sqlCmd.ExecuteReader();
                while (reader.Read())
                {
                    WClaimRcvPartFile partfile = new WClaimRcvPartFile();
                    partfile.OriginalPartNumber = Convert.ToString(reader["PartNo"]);
                    partfile.OriginalPartName = Convert.ToString(reader["PartOriginalName"]);
                    partfile.ProcessedPartNumber = Convert.ToString(reader["ProcessedPartNo"]);
                    partfile.ProcessedPartName = Convert.ToString(reader["PartProcesedName"]);
                    partfile.Quantity = Convert.ToInt32(reader["Quantity"]);
                    partfile.SparePartUnitPrice = Convert.ToInt32(reader["PaymentQuantity"]);
                    partfile.SparePartTotalCost = Convert.ToInt32(reader["PaymentTotalPrice"]);
                    detail.AddPartFile(partfile);
                }
            }
        }

        #endregion

        #endregion

        #region -- Select Methods --
        public IQueryable<Claim> GetReceiveDealer(string reimbursement, DateTime receivedDate)
        {
            var claim = from a in ctx.Claims
                        join b in ctx.SvTrnClaimJudgements
                        on new { a.CompanyCode, a.BranchCode, a.ProductType, a.GenerateNo }
                        equals new { b.CompanyCode, b.BranchCode, b.ProductType, b.GenerateNo }
                        where a.CompanyCode == CompanyCode
                        && a.BranchCode == BranchCode
                        && a.ProductType == ProductType
                        && b.SuzukiRefferenceNo == reimbursement
                        && a.SenderDealerCode != CompanyCode
                        && b.ReceivedDate.Date == receivedDate.Date
                        select  a;

            return claim;
        }

        #endregion
    }

    #region -- Warranty Claim File --

    public class WarrantyClaimHdrFile
    {
        #region -- Initialize --

        private string line = "";

        public WarrantyClaimHdrFile()
        {
            string text = "HWCLAM";
            line = "#" + text.PadRight(155, ' ');
        }

        public WarrantyClaimHdrFile(string text)
        {
            line = "#" + text.PadRight(155, ' ');
        }

        #endregion

        #region -- Public Properties --

        public string RecordID { get { return GetString(1, 1); } }

        public string DataID { get { return GetString(2, 5); } }

        public string DealerCode
        {
            get { return GetString(7, 10); }
            set { SetValue(7, 10, value); }
        }

        public string RcvDealerCode
        {
            get { return GetString(17, 10); }
            set { SetValue(17, 10, value); }
        }

        public string DealerName
        {
            get { return GetString(27, 50); }
            set { SetValue(27, 50, value); }
        }

        public int TotalItem
        {
            get { return GetInt32(77, 6); }
            set { SetValue(77, 6, value); }
        }

        public string ReceiptNo
        {
            get { return GetString(83, 15); }
            set { SetValue(83, 15, value); }
        }

        public DateTime ReceiptDate
        {
            get { return GetDate(98, 8); }
            set { SetValue(98, 8, value); }
        }

        public string ProductType
        {
            get { return GetString(106, 1); }
            set
            {
                string val = value;
                if (val == "2W") val = "A";
                if (val == "4W") val = "B";
                if (val == "OB") val = "C";
                SetValue(106, 1, val);
            }
        }

        public int LotNo
        {
            get { return GetInt32(107, 3); }
            set { SetValue(107, 3, value); }
        }

        public decimal OtherCompensation
        {
            get { return GetDecimal(110, 12); }
            set { SetValue(110, 12, value); }
        }

        public string FlagData
        {
            get { return GetString(122, 2); }
            set { SetValue(122, 2, value); }
        }

        public string BlankFilter { get { return GetString(124, 32); } }

        public string Text { get { return line.Substring(1); } }

        #endregion

        #region -- Private Methods --

        private string GetString(int start, int length)
        {
            return line.Substring(start, length).Trim();
        }

        private int GetInt32(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return Int32.MinValue;
            }
        }

        private decimal GetDecimal(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return Decimal.Zero;
            }
        }

        private DateTime GetDate(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                int year = Convert.ToInt32(value.Substring(0, 4));
                int month = Convert.ToInt32(value.Substring(4, 2));
                int day = Convert.ToInt32(value.Substring(6, 2));
                return new DateTime(year, month, day);
            }
            catch
            {
                return new DateTime(1900, 1, 1);
            }
        }

        private void SetValue(int start, int length, string value)
        {
            string a = line.Substring(0, start);
            string b = value.PadRight(length, ' ').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, int value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString().PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, decimal value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString("#").PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, DateTime value)
        {
            if (length == 8)
            {
                string a = line.Substring(0, start);
                string b = value.ToString("yyyMMdd");
                string c = line.Substring(start + length);
                if (Convert.ToInt32(b) <= 19000101) b = "00000000";
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        private void SetValue(int start, int length, bool value)
        {
            if (length == 1)
            {
                string a = line.Substring(0, start);
                string b = value ? "Y" : "N";
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        #endregion
    }

    public class WarrantyClaimDtlFile
    {
        #region -- Initialize --

        private string line = "";
        private List<WarrantyClaimPartFile> partfiles = new List<WarrantyClaimPartFile>();
        private string remark = "";

        public WarrantyClaimDtlFile()
        {
            string text = "1";
            line = "#" + text.PadRight(155, ' ');
        }

        public WarrantyClaimDtlFile(string text)
        {
            line = "#" + text.PadRight(155, ' ');
        }

        #endregion

        #region -- Public Properties --

        public string RecordID
        {
            get { return GetString(1, 1); }
            set { SetValue(1, 1, value); }
        }

        public string IssueNo
        {
            get { return GetString(2, 15); }
            set { SetValue(2, 15, value); }
        }

        public string ServiceBookNo
        {
            get { return GetString(17, 10); }
            set { SetValue(17, 10, value); }
        }

        public string ChassisCode
        {
            get { return GetString(27, 15); }
            set { SetValue(27, 15, value); }

        }

        public string ChassisNo
        {
            get { return GetString(42, 10); }
            set { SetValue(42, 10, value); }
        }

        public string EngineCode
        {
            get { return GetString(52, 15); }
            set { SetValue(52, 15, value); }
        }

        public string EngineNo
        {
            get { return GetString(67, 10); }
            set { SetValue(67, 10, value); }
        }

        public string BasicModel
        {
            get { return GetString(77, 15); }
            set { SetValue(77, 15, value); }
        }

        public string TechnicalModel
        {
            get { return GetString(92, 4); }
            set { SetValue(92, 4, value); }
        }

        public DateTime IssueDate
        {
            get { return GetDate(96, 8); }
            set { SetValue(96, 8, value); }
        }

        public DateTime RegisteredDate
        {
            get { return GetDate(104, 8); }
            set { SetValue(104, 8, value); }
        }

        public DateTime RepairedDate
        {
            get { return GetDate(112, 8); }
            set { SetValue(112, 8, value); }
        }

        public decimal Odometer
        {
            get { return GetDecimal(120, 8); }
            set { SetValue(120, 8, Convert.ToInt32(value)); }
        }

        public string TroubleCode
        {
            get { return line.Substring(128, 4); }
            set { SetValue(128, 4, value); }
        }

        public string KdOemCode
        {
            get { return GetString(132, 1); }
            set { SetValue(132, 1, value); }
        }

        public string ClaimCategoryCode
        {
            get { return GetString(133, 1); }
            set { SetValue(133, 1, value); }
        }

        public string OperationNumber
        {
            get { return line.Substring(134, 11); }
            set { SetValue(134, 11, value); }
        }

        public string ActualLaborTime
        {
            get { return GetString(145, 3); }
            set { SetValue(145, 3, value); }
        }

        public string SubletWorkTime
        {
            get { return GetString(148, 3); }
            set { SetValue(148, 3, value); }
        }

        public string BlankFiller { get { return GetString(151, 5); } }

        public string RepairDescription { get { return remark; } set { remark = value; } }

        public List<WarrantyClaimPartFile> ListPartFiles { get { return partfiles; } }

        public void AddPartFile(WarrantyClaimPartFile partfile) { partfiles.Add(partfile); }

        public string Text { get { return line.Substring(1); } }

        #endregion

        #region -- Private Methods --

        private string GetString(int start, int length)
        {
            return line.Replace("\n", " ").Replace("\t", " ").Substring(start, length).Trim();
        }

        private int GetInt32(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return Int32.MinValue;
            }
        }

        private decimal GetDecimal(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return Decimal.Zero;
            }
        }

        private DateTime GetDate(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                int year = Convert.ToInt32(value.Substring(0, 4));
                int month = Convert.ToInt32(value.Substring(4, 2));
                int day = Convert.ToInt32(value.Substring(6, 2));
                return new DateTime(year, month, day);
            }
            catch
            {
                return new DateTime(1900, 1, 1);
            }
        }

        private void SetValue(int start, int length, string value)
        {
            string a = line.Substring(0, start);
            string b = value.PadRight(length, ' ').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, int value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString().PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, DateTime value)
        {
            if (length == 8)
            {
                string a = line.Substring(0, start);
                string b = value.ToString("yyyMMdd");
                string c = line.Substring(start + length);
                if (b == "19000101") b = "00000000";
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        private void SetValue(int start, int length, bool value)
        {
            if (length == 1)
            {
                string a = line.Substring(0, start);
                string b = value ? "Y" : "N";
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        #endregion
    }

    public class WarrantyClaimPartFile
    {
        #region -- Initialize --

        private string line = "";

        public WarrantyClaimPartFile()
        {
            line = "#2";
        }

        public WarrantyClaimPartFile(string text)
        {
            line = "#" + text.PadRight(155, ' ');
        }

        public void AddPart(WarrantyClaimPart part)
        {
            line += part.CausalPartCode.PadRight(1, ' ').Substring(0, 1);
            line += part.PartNo.PadRight(15, ' ').Substring(0, 15);
            line += part.Qty.ToString("#").PadLeft(2, '0').Substring(0, 2);
        }

        public string Text { get { return line.Substring(1).PadRight(155, ' '); } }

        #endregion

        #region -- Public Properties --

        public string RecordID { get { return GetString(1, 1); } }

        public List<WarrantyClaimPart> WarrantyClaimParts
        {
            get
            {
                List<WarrantyClaimPart> parts = new List<WarrantyClaimPart>();
                for (int i = 0; i < 7; i++)
                {
                    if (GetString(2 + (18 * i), 18) != "")
                    {
                        WarrantyClaimPart part = new WarrantyClaimPart();
                        part.CausalPartCode = GetString(2 + (18 * i), 1);
                        part.PartNo = GetString(3 + (18 * i), 15);
                        part.Qty = GetInt32(18 + (18 * i), 2);
                        parts.Add(part);
                    }
                }
                return parts;
            }
        }

        public struct WarrantyClaimPart
        {
            public string CausalPartCode;
            public string PartNo;
            public int Qty;
        }

        #endregion

        #region -- Private Methods --

        private string GetString(int start, int length)
        {
            return line.Substring(start, length).Trim();
        }

        private int GetInt32(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return Int32.MinValue;
            }
        }

        private decimal GetDecimal(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return Decimal.Zero;
            }
        }

        private DateTime GetDate(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                int year = Convert.ToInt32(value.Substring(0, 4));
                int month = Convert.ToInt32(value.Substring(4, 2));
                int day = Convert.ToInt32(value.Substring(6, 2));
                return new DateTime(year, month, day);
            }
            catch
            {
                return new DateTime(1900, 1, 1);
            }
        }

        private void SetValue(int start, int length, string value)
        {
            string a = line.Substring(0, start);
            string b = value.PadRight(length, ' ').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, int value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString().PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, DateTime value)
        {
            if (length == 8)
            {
                string a = line.Substring(0, start);
                string b = value.ToString("yyyMMdd");
                string c = line.Substring(start + length);
                if (Convert.ToInt32(b) <= 19000101) b = "00000000";
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        private void SetValue(int start, int length, bool value)
        {
            if (length == 1)
            {
                string a = line.Substring(0, start);
                string b = value ? "Y" : "N";
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        #endregion
    }

    #endregion

    #region -- WClaim Receive File --

    public class WClaimRcvHdrFile
    {
        #region -- Initialize --

        private string line = "";

        public WClaimRcvHdrFile()
        {
            string text = "HWCMRB";
            line = "#" + text.PadRight(156, ' ');
        }

        public WClaimRcvHdrFile(string text)
        {
            line = "#" + text.PadRight(156, ' ');
        }

        #endregion

        #region -- Public Properties --

        public string RecordID { get { return GetString(1, 1); } }

        public string DataID { get { return GetString(2, 5); } }

        public string DealerCode
        {
            get { return GetString(7, 10); }
            set { SetValue(7, 10, value); }
        }

        public string RcvDealerCode
        {
            get { return GetString(17, 10); }
            set { SetValue(17, 10, value); }
        }

        public string DealerName
        {
            get { return GetString(27, 50); }
            set { SetValue(27, 50, value); }
        }

        public int TotalItem
        {
            get { return GetInt32(77, 6); }
            set { SetValue(77, 6, value); }
        }

        public string ProductType
        {
            get { return GetString(83, 1); }
            set
            {
                string val = value;
                if (val == "2W") val = "A";
                if (val == "4W") val = "B";
                if (val == "OB") val = "C";
                SetValue(83, 1, val);
                //SetValue(83, 1, value); 
            }
        }

        public string ReimbursementNo
        {
            get { return GetString(84, 15); }
            set { SetValue(84, 15, value); }
        }

        public DateTime ReimbursementDate
        {
            get { return GetDate(99, 8); }
            set { SetValue(99, 8, value); }
        }

        public string BlankFilter { get { return GetString(107, 49); } }

        public string Text { get { return line.Substring(1); } }

        #endregion

        #region -- Private Methods --

        private string GetString(int start, int length)
        {
            return line.Substring(start, length).Trim();
        }

        private int GetInt32(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return Int32.MinValue;
            }
        }

        private decimal GetDecimal(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return Decimal.Zero;
            }
        }

        private DateTime GetDate(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                int year = Convert.ToInt32(value.Substring(0, 4));
                int month = Convert.ToInt32(value.Substring(4, 2));
                int day = Convert.ToInt32(value.Substring(6, 2));
                return new DateTime(year, month, day);
            }
            catch
            {
                return new DateTime(1900, 1, 1);
            }
        }

        private void SetValue(int start, int length, string value)
        {
            string a = line.Substring(0, start);
            string b = value.PadRight(length, ' ').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, int value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString().PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, decimal value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString("#").PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, DateTime value)
        {
            if (length == 8)
            {
                string a = line.Substring(0, start);
                string b = value.ToString("yyyMMdd");
                string c = line.Substring(start + length);
                if (Convert.ToInt32(b) <= 19000101) b = "00000000";
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        private void SetValue(int start, int length, bool value)
        {
            if (length == 1)
            {
                string a = line.Substring(0, start);
                string b = value ? "Y" : "N";
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        #endregion
    }

    public class WClaimRcvDtlFile
    {
        #region -- Initialize --

        private string line = "";
        private List<WClaimRcvPartFile> partfiles = new List<WClaimRcvPartFile>();

        public WClaimRcvDtlFile()
        {
            string text = "1";
            line = "#" + text.PadRight(156, ' ');
        }

        public WClaimRcvDtlFile(string text)
        {
            line = "#" + text.PadRight(156, ' ');
        }

        #endregion

        #region -- Public Properties --

        public string RecordID { get { return GetString(1, 1); } }

        public string IssueNo
        {
            get { return GetString(2, 15); }
            set { SetValue(2, 15, value); }
        }

        public string ClaimDivisionCode
        {
            get { return GetString(17, 1); }
            set { SetValue(17, 1, value); }
        }

        public string JudgementCode
        {
            get { return GetString(18, 4); }
            set { SetValue(18, 4, value); }
        }

        public int LotNo
        {
            get { return GetInt32(22, 3); }
            set { SetValue(22, 3, value); }
        }

        public string ServiceBookNo
        {
            get { return GetString(25, 10); }
            set { SetValue(25, 10, value); }
        }

        public string ChassisCode
        {
            get { return GetString(35, 15); }
            set { SetValue(35, 15, value); }
        }

        public string ChassisNo
        {
            get { return GetString(50, 10); }
            set { SetValue(50, 10, value); }
        }

        public string EngineCode
        {
            get { return GetString(60, 15); }
            set { SetValue(60, 15, value); }
        }

        public string EngineNo
        {
            get { return GetString(75, 10); }
            set { SetValue(75, 10, value); }
        }

        public int PertClaimTotalCost
        {
            get { return GetInt32(85, 9); }
            set { SetValue(85, 9, value); }
        }

        public int PertClaimPartsCost
        {
            get { return GetInt32(94, 9); }
            set { SetValue(94, 9, value); }
        }

        public int PertClaimShippingChargesCost
        {
            get { return GetInt32(103, 9); }
            set { SetValue(103, 9, value); }
        }

        public int PertClaimLaborCost
        {
            get { return GetInt32(112, 9); }
            set { SetValue(112, 9, value); }
        }

        public int PertClaimSubletCost
        {
            get { return GetInt32(121, 9); }
            set { SetValue(121, 9, value); }
        }

        public int PertClaimSubletHours
        {
            get { return GetInt32(130, 3); }
            set { SetValue(130, 3, value); }
        }

        public string OperationNumber
        {
            get { return GetString(133, 11); }
            set { SetValue(133, 11, value); }
        }

        public string FlatRateTime
        {
            get { return GetString(144, 3); }
            set { SetValue(144, 3, value); }
        }

        public string TechnicalModel
        {
            get { return GetString(147, 4); }
            set { SetValue(147, 4, value); }
        }

        public string BlankFiller { get { return GetString(151, 5); } }

        public string Text { get { return line.Substring(1); } }

        public List<WClaimRcvPartFile> ListPartFiles { get { return partfiles; } }

        public void AddPartFile(WClaimRcvPartFile partfile)
        {
            partfiles.Add(partfile);
        }

        #endregion

        #region -- Private Methods --

        private string GetString(int start, int length)
        {
            return line.Substring(start, length).Trim();
        }

        private int GetInt32(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return Int32.MinValue;
            }
        }

        private decimal GetDecimal(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return Decimal.Zero;
            }
        }

        private DateTime GetDate(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                int year = Convert.ToInt32(value.Substring(0, 4));
                int month = Convert.ToInt32(value.Substring(4, 2));
                int day = Convert.ToInt32(value.Substring(6, 2));
                return new DateTime(year, month, day);
            }
            catch
            {
                return new DateTime(1900, 1, 1);
            }
        }

        private void SetValue(int start, int length, string value)
        {
            string a = line.Substring(0, start);
            string b = value.PadRight(length, ' ').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, int value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString().PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, decimal value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString("#").PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, DateTime value)
        {
            if (length == 8)
            {
                string a = line.Substring(0, start);
                string b = value.ToString("yyyMMdd");
                string c = line.Substring(start + length);
                if (Convert.ToInt32(b) <= 19000101) b = "00000000";
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        private void SetValue(int start, int length, bool value)
        {
            if (length == 1)
            {
                string a = line.Substring(0, start);
                string b = value ? "Y" : "N";
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        #endregion
    }

    public class WClaimRcvPartFile
    {
        #region -- Initialize --

        private string line = "";

        public WClaimRcvPartFile()
        {
            string text = "2";
            line = "#" + text.PadRight(156, ' ');
        }

        public WClaimRcvPartFile(string text)
        {
            line = "#" + text.PadRight(156, ' ');
        }

        #endregion

        #region -- Public Properties --

        public string RecordID { get { return GetString(1, 1); } }

        public string OriginalPartNumber
        {
            get { return GetString(2, 15); }
            set { SetValue(2, 15, value); }
        }

        public string OriginalPartName
        {
            get { return GetString(17, 50); }
            set { SetValue(17, 50, value); }
        }

        public string ProcessedPartNumber
        {
            get { return GetString(67, 15); }
            set { SetValue(67, 15, value); }
        }

        public string ProcessedPartName
        {
            get { return GetString(82, 50); }
            set { SetValue(82, 50, value); }
        }

        public int Quantity
        {
            get { return GetInt32(132, 2); }
            set { SetValue(132, 2, value); }
        }

        public int SparePartUnitPrice
        {
            get { return GetInt32(134, 10); }
            set { SetValue(134, 10, value); }
        }

        public int SparePartTotalCost
        {
            get { return GetInt32(144, 12); }
            set { SetValue(144, 12, value); }
        }

        public string Text { get { return line.Substring(1); } }

        #endregion

        #region -- Private Methods --

        private string GetString(int start, int length)
        {
            return line.Substring(start, length).Trim();
        }

        private int GetInt32(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return Int32.MinValue;
            }
        }

        private decimal GetDecimal(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return Decimal.Zero;
            }
        }

        private DateTime GetDate(int start, int length)
        {
            string value = line.Substring(start, length);
            try
            {
                int year = Convert.ToInt32(value.Substring(0, 4));
                int month = Convert.ToInt32(value.Substring(4, 2));
                int day = Convert.ToInt32(value.Substring(6, 2));
                return new DateTime(year, month, day);
            }
            catch
            {
                return new DateTime(1900, 1, 1);
            }
        }

        private void SetValue(int start, int length, string value)
        {
            string a = line.Substring(0, start);
            string b = value.PadRight(length, ' ').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, int value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString().PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, decimal value)
        {
            string a = line.Substring(0, start);
            string b = value.ToString("#").PadLeft(length, '0').Substring(0, length);
            string c = line.Substring(start + length);
            line = string.Format("{0}{1}{2}", a, b, c);
        }

        private void SetValue(int start, int length, DateTime value)
        {
            if (length == 8)
            {
                string a = line.Substring(0, start);
                string b = value.ToString("yyyMMdd");
                string c = line.Substring(start + length);
                if (Convert.ToInt32(b) <= 19000101) b = "00000000";
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        private void SetValue(int start, int length, bool value)
        {
            if (length == 1)
            {
                string a = line.Substring(0, start);
                string b = value ? "Y" : "N";
                string c = line.Substring(start + length);
                line = string.Format("{0}{1}{2}", a, b, c);
            }
        }

        #endregion
    }

    #endregion
}