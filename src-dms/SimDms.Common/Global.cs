using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TracerX;

namespace SimDms.Common
{
    public abstract class BaseTable
    {
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        public void SetCreatedBy(string userid, DateTime dt)
        {
            CreatedDate = dt;
            CreatedBy = userid;
        }

        public void SetModifiedBy(string userid, DateTime dt)
        {
            LastUpdateBy = userid;
            LastUpdateDate = dt;
        }
    }

    [Table("SysMessage")]
    public partial class SysMessage 
    {
        [Key]
        public string MessageCode { get; set; }
        public string MessageCaption { get; set; }
    }

    public class MyUserInfo
    {
         public string UserId { get; set; }
         public string FullName { get; set; }
         public string CompanyCode { get; set; }
         public string CompanyGovName { get; set; }
         public string BranchCode { get; set; }
         public string CompanyName { get; set; }
         public string TypeOfGoods { get; set; }
         public string ProductType { get; set; }
         public string ProfitCenter { get; set; }         
         public string TypeOfGoodsName { get; set; }
         public string ProductTypeName { get; set; }
         public string ProfitCenterName { get; set; }
         public bool IsActive { get; set; }
         public bool RequiredChange { get; set; }
         public string SimDmsVersion { get; set; }
         public string ShowHideTypePart { get; set; }
    }

    public enum UploadTypeDCS
    {
        PINVS, PPRCD, PMODP, PMDLM, MSMDL, SHIST, WFRAT, WCAMP, WJUDG, WPDFS, WSECT, WWRNT, WTROB, SPORD, SDORD, SPRIC, SSJAL, SHPOK, SACCS, SFPO1, SFPO2, SFPLB, SFPLA, SFPLR, SUADE, WSMRD, WSMRR, FAPIO, SFPDA
    }

    public partial class SysMessages
    {
        /// <summary>
        /// Message : Delete Success
        /// </summary>
        public const string MSG_1000 = "1000";

        /// <summary>
        /// Message : Delete Fail
        /// </summary>
        public const string MSG_1001 = "1001";

        /// <summary>
        /// Message : Cannot delete data because still have detail
        /// </summary>
        public const string MSG_1002 = "1002";

        /// <summary>
        /// Message : Cannot delete important data
        /// </summary>
        public const string MSG_1003 = "1003";

        /// <summary>
        /// Message : Data duplicate
        /// </summary>
        public const string MSG_1004 = "1004";

        /// <summary>
        /// Message : Delete Fail because Constraint!, 
        /// </summary>
        public const string MSG_1005 = "1005";

        /// <summary>
        /// Message : Save Success
        /// </summary>
        public const string MSG_1100 = "1100";

        /// <summary>
        /// Message : Save Fail
        /// </summary>
        public const string MSG_1101 = "1101";

        /// <summary>
        /// Message : Populated Berhasil
        /// </summary>
        public const string MSG_1102 = "1102";

        /// <summary>
        /// Message : Populated Tidak Berhasil
        /// </summary>
        public const string MSG_1103 = "1103";

        /// <summary>
        /// Message : Update Data Berhasil
        /// </summary>
        public const string MSG_1104 = "1104";

        /// <summary>
        /// Message : Update Data Gagal
        /// </summary>
        public const string MSG_1105 = "1105";

        /// <summary>
        /// Message : Upload File is Succeed
        /// </summary>
        public const string MSG_1106 = "1106";

        /// <summary>
        /// Message : Upload File is Fail
        /// </summary>
        public const string MSG_1107 = "1107";

        /// <summary>
        /// Message : Invalid User
        /// </summary>
        public const string MSG_1200 = "1200";

        /// <summary>
        /// Message : Invalid Password
        /// </summary>
        public const string MSG_1201 = "1201";

        /// <summary>
        /// Message : Password Not Match
        /// </summary>
        public const string MSG_1202 = "1202";

        /// <summary>
        /// Message : Password Change
        /// </summary>
        public const string MSG_1203 = "1203";

        /// <summary>
        /// Message : Access Denied
        /// </summary>
        public const string MSG_1204 = "1204";

        /// <summary>
        /// Delete data gagal, karena masih ada outstanding data
        /// </summary>
        public const string MSG_1205 = "1205";

        /// <summary>
        /// Message : Are you Sure???
        /// </summary>
        public const string MSG_2000 = "2000";

        /// <summary>
        /// Message : Delete Data
        /// </summary>
        public const string MSG_2001 = "2001";

        /// <summary>
        /// Message : Close Application
        /// </summary>
        public const string MSG_2002 = "2002";

        /// <summary>
        /// Message : Backup Data Confirmation
        /// </summary>
        public const string MSG_2003 = "2003";

        /// <summary>
        /// Message : Posting Data
        /// </summary>
        public const string MSG_2004 = "2004";

        /// <summary>
        /// Message : Closing Data
        /// </summary>
        public const string MSG_2005 = "2005";


        /// <summary>
        /// Message : Print Data
        /// </summary>
        public const string MSG_3000 = "3000";

        /// <summary>
        /// Message : Already Stock Taking
        /// </summary>
        public const string MSG_4000 = "4000";

        /// <summary>
        /// Message : Not In Stock Taking
        /// </summary>
        public const string MSG_4001 = "4001";

        /// <summary>
        /// Message : Adjustment CheckBox in Stock Taking To Master
        /// </summary>
        public const string MSG_4002 = "4002";

        /// <summary>
        /// Message : Stock Taking CheckBox in Stock Taking To Master
        /// </summary>
        public const string MSG_4003 = "4003";

        /// <summary>
        /// Message : Stock Taking To Master is sucessfull
        /// </summary>
        public const string MSG_4004 = "4004";

        /// <summary>
        /// Message : Stock Taking To Master is failed
        /// </summary>
        public const string MSG_4005 = "4005";

        /// <summary>
        /// Message : {Control} Cannot be blank
        /// </summary>
        public const string MSG_5000 = "5000";

        /// <summary>
        /// Message : Quantity not enought
        /// </summary>
        public const string MSG_5001 = "5001";

        /// <summary>
        /// Message : Ordering parameter (Lead time, Order Cycle, Safety Stock) harus di setup terlebih dahulu
        /// </summary>
        public const string MSG_5002 = "5002";

        /// <summary>
        /// Message : Lead time, Order Cycle, Safety Stock harus di isi  (Proses Penyimpanan Gagal)
        /// </summary>
        public const string MSG_5003 = "5003";

        /// <summary>
        /// Message : Product Type harus di isi di Master Perusahaan (Proses Penyimpanan Gagal)
        /// </summary>
        public const string MSG_5004 = "5004";

        /// <summary>
        /// Message : Tidak bisa di edit karena transaksi sudah diposting 
        /// </summary>
        public const string MSG_5005 = "5005";

        /// <summary>
        /// Message : {0} tidak sesuai dengan {1}
        /// </summary>
        public const string MSG_5006 = "5006";

        /// <summary>
        /// Message : Profit Centre not valid
        /// </summary>
        public const string MSG_5007 = "5007";

        /// <summary>
        /// Message : Quantity Invalid
        /// </summary>
        public const string MSG_5008 = "5008";

        /// <summary>
        /// Message : Quantity Cannot less or equal than 0
        /// </summary>
        public const string MSG_5009 = "5009";

        /// <summary>
        /// Message : Quantity Received cannot greater than maximum quantity On Order
        /// </summary>
        public const string MSG_5010 = "5010";

        /// <summary>
        /// Message : Item alredy received
        /// </summary>
        public const string MSG_5011 = "5011";

        /// <summary>
        /// Message : Quantity subtitusi tidak boleh lebih besar dari quantity asal
        /// </summary>
        public const string MSG_5012 = "5012";

        /// <summary>
        /// Message : Quantity Received cannot greater than quantity Intransit 
        /// </summary>
        public const string MSG_5013 = "5013";

        /// <summary>
        /// Message : Quantity Received cannot greater than quantity Claim
        /// </summary>
        public const string MSG_5014 = "5014";

        /// <summary>
        /// Message : Data not found
        /// </summary>
        public const string MSG_5015 = "5015";

        /// <summary>
        /// Message : Quantity Picked cannot greater than quantity Pick
        /// </summary>
        public const string MSG_5016 = "5016";

        /// <summary>
        /// Message : Quantity Demand greater than Quantity Supply, Continue BO?
        /// </summary>
        public const string MSG_5017 = "5017";

        /// <summary>
        /// Message : Quantity Return cannot greater than maximum quantity Return
        /// </summary>
        public const string MSG_5018 = "5018";

        /// <summary>
        /// Message : Transaction can't be process because not enough information
        /// </summary>
        public const string MSG_5019 = "5019";

        /// <summary>
        /// Message : Transaction can't be process because no detail data found
        /// </summary>
        public const string MSG_5020 = "5020";

        /// <summary>
        /// Message : Transaction can't be process because no availablity less than 0
        /// </summary>
        public const string MSG_5021 = "5021";

        /// <summary>
        /// Message : Transaction can't be process because found different retail price on part
        /// </summary>
        public const string MSG_5022 = "5022";

        /// <summary>
        /// Message : You have unpayed order from transaction before, please finish it first
        /// </summary>
        public const string MSG_5023 = "5023";

        /// <summary>
        /// Message : Your credit limit is less to begin new transaction
        /// </summary>
        public const string MSG_5024 = "5024";

        /// <summary>
        /// Message : Facility Under Construction
        /// </summary>
        public const string MSG_9000 = "9000";

        /// <summary>
        /// Message : Total quantity harus lebih besar atau sama dengan Damage Quantity
        /// </summary>
        public const string MSG_5025 = "5025";

        /// <summary>
        /// Message : Berhasil print data
        /// </summary>
        public const string MSG_5026 = "5026";

        /// <summary>
        /// Message : Gagal print data
        /// </summary>
        public const string MSG_5027 = "5027";

        /// <summary>
        /// Message : Data tidak ditemukan
        /// </summary>
        public const string MSG_5028 = "5028";

        /// <summary>
        /// Message : Data tidak dapat disimpan karena tidak memiliki Cost Price, 
        /// Retail Price atau Retail Price Include Tax
        /// </summary>        
        public const string MSG_5029 = "5029";

        /// <summary>
        /// Message : Proses tidak dapat dilakukan karena tidak mempunyai nilai DPP dan Pajak
        /// </summary>        
        public const string MSG_5030 = "5030";

        /// <summary>
        /// Message: {0} tidak boleh lebih besar dari {1}. {2}
        /// </summary>
        public const string MSG_5031 = "5031";

        /// <summary>
        /// Message: {0} tidak boleh kurang atau sama dengan nol
        /// </summary>
        public const string MSG_5032 = "5032";

        /// <summary>
        /// Message: {0} tidak boleh kosong
        /// </summary>
        public const string MSG_5033 = "5033";

        /// <summary>
        /// Message: {0} tidak ditemukan
        /// </summary>
        public const string MSG_5034 = "5034";

        /// <summary>
        /// Message: Data tidak dapat disimpan karena masih ada qty persediaan
        /// </summary>
        public const string MSG_5035 = "5035";

        /// <summary>
        /// Message: Tidak ada SUGGOR yang diproses
        /// </summary>
        public const string MSG_5036 = "5036";

        /// <summary>
        /// Message: Data tidak dapat disimpan karena data detail mempunyai Stock Available kurang atau sama dengan nol
        /// </summary>
        public const string MSG_5037 = "5037";

        /// <summary>
        /// Proses {0} berhasil. {1}
        /// </summary>
        public const string MSG_5038 = "5038";

        /// <summary>
        /// Proses {0} gagal. {1}
        /// </summary>
        public const string MSG_5039 = "5039";

        /// <summary>
        /// Gudang tujuan dan gudang asal tidak boleh sama
        /// </summary>
        public const string MSG_5040 = "5040";

        /// <summary>
        /// Data tidak dapat disimpan karena available item tidak mencukupi
        /// </summary>
        public const string MSG_5041 = "5041";

        /// <summary>
        /// Posting tidak dapat dilakukan karena available part tidak mencukupi
        /// </summary>
        public const string MSG_5042 = "5042";

        /// <summary>
        /// {0} tidak valid.
        /// </summary>
        public const string MSG_5043 = "5043";

        /// <summary>
        /// {0}, Tidak boleh Nol atau Karakter.
        /// </summary>
        public const string MSG_5044 = "5044";

        /// <summary>
        /// Doc No {0} sudah di {1}
        /// </summary>
        public const string MSG_5045 = "5045";

        /// <summary>
        /// Dokumen {0} belum diinput di tabel gnMstDocument
        /// </summary>
        public const string MSG_5046 = "5046";

        /// <summary>
        /// Dokumen tidak dapat dicetak karena tidak memiliki data detail
        /// </summary>
        public const string MSG_5047 = "5047";

        /// <summary>
        /// Document Warranty Claim is Duplicate
        /// </summary>
        public const string MSG_5048 = "5048";

        /// <summary>
        /// Receive Warranty Claim File has been uploaded
        /// </summary>
        public const string MSG_5049 = "5049";

        /// <summary>
        /// No. {0} Tidak Boleh Melebihi No. {1}
        /// </summary>
        public const string MSG_6001 = "6001";
    }

    public partial class GnMstLookUpHdr
    {
        public const string AreaCode = "AREA";
        public const string AreaCodeCustomer = "AREACD";
        public const string AdjustmentCode = "ADJS";
        public const string AccountType = "ACTP";

        public const string BankCode = "BANK";
        public const string BloodCode = "BLOD"; // A, B, O, AB

        public const string CityCode = "CITY";
        public const string CityCodeCustomer = "CITYCD";
        public const string CustomerType = "CSTP";
        public const string CustomerGrade = "CSGR";
        public const string CustomerCategory = "CSCT";
        public const string CostCenter = "CSTR";
        public const string Condition = "COND";
        public const string CabKdPjk = "CBPJ";

        public const string DebetInfo = "DBT_INFO";
        public const string DealerArea = "DFSC";

        public const string EstimateTime = "ESTM";
        public const string EmployeeStatusCode = "PERS";

        public const string FormalEducation = "FEDU";
        public const string FPGabungan = "FPGB"; // Parameter Faktur Pajak Gabungan Service 1=Central , 0= Tidak

        public const string GenderCode = "GNDR";  // L (Laki-laki), P (Perempuan)

        public const string IndexVehicle = "IDX_VHC";
        public const string InterchangeCode = "INCD";
        public const string InformalTrainingCode = "IEDU";
        public const string InsentifPart = "ISTF";

        public const string LabourRateAmount = "LBCD";

        public const string ModelVehicle = "MODL";
        public const string MaritalCode = "MRTL"; // KWN, TDKKWIN, DUDA, JANDA

        public const string OrderType = "ORTP";

        public const string PartCategory = "PRCT";
        public const string PaymentBy = "PYBY";
        public const string ProductType = "PRDT";
        public const string ProfitCenter = "PFCN";
        public const string ProspectActivity = "PACT";
        public const string ProspectLostCategory = "PLCC";
        public const string ProspectLeasing = "LSNG";
        public const string ProspectDP = "DWPM";
        public const string ProspectTenor = "TENOR";
        public const string ProspectStatus = "PSTS";
        public const string LostCaseReason = "ITLR";
        public const string DataSourceReason = "PSRC";
        public const string ProvinceCode = "PROV";
        public const string ProvinceCodeCustomer = "PROVCD";
        public const string PersonnelStatus = "PERS";

        public const string ReasonAdjustment = "RSAD";
        public const string ReasonWarehouseTransfer = "RSWT";
        public const string ReligionCode = "RLGN"; // 
        public const string RefferenceType = "RFTP"; // 

        public const string StatusPart = "STPR";
        public const string SparepartCentralized = "SPTR";
        public const string SupplierGrade = "SPGR";
        public const string SalesType = "SLTP";
        public const string StatusPrintingSpare = "FLPG";
        public const string StatusPORDS = "PORDS";
        public const string StatusFakturGabunganSpare = "FFPG";
        public const string StatusCheckNomorFaktur = "STHP";

        public const string SubConStatus = "PORRSTAT"; // Status Pekerjaan Luar Service
        public const string PurchaseOrderApproval = "POAP";

        public const string FPOLRevision = "REVI";

        /// <summary>
        /// Code ID "STATUS"
        /// </summary>
        public const string StatusCode = "STAT"; // data status
        public const string SatuanWaktu = "STWK";
        public const string ShoesSize = "SHOE";
        public const string SuzukiTrainingCode = "SZKT";

        public const string TermOfPayment = "TOPC";
        public const string TermOfPaymentSZK = "TOP-SZK";
        public const string TransmitionType = "TRTY";
        public const string TransType4PickingListNonPenjualan = "TTNP";
        public const string TransType4PickingListPenjualan = "TTPJ";
        public const string TransType4SalesUnit = "TTSL";
        public const string TransType4Service = "TTSR";
        public const string TransType4WRS = "TTWR";
        public const string TypeAdjusment = "ADJS";
        public const string TypeOfGoods = "TPGO";
        public const string TypeCode = "TPCD";
        public const string TypeDocument = "TPDK";
        public const string TitleCode = "TITL"; // KADEP, KACAB
        public const string TransKdPjk = "TRPJ";

        public const string UnitOfMeasurement = "UOMC";
        public const string UniformSize = "UNIF";

        public const string WarehouseCode = "WRCD";
        public const string WarehouseTransfer = "WHTR"; // warehose transfer
        public const string GroupAR = "GPAR"; // Group / Kelompok AR(General)
        public const string FlagPaymentBy = "FLAG_PYBY";

        //FINANCE
        public const string FormatReport = "RPTF"; // Format Laporan (Finance)
        public const string ColumnReport = "RPTC"; // Kolom Laporan (Finance)
        public const string PeriodReport = "RPTP"; // Periode Laporan (Finance)
        public const string BTTCategory = "CBTT"; // BTT Kategori (Finance)
        public const string PaymentType = "PBKK"; // Tipe Pembayaran (Finance)
        public const string BKKType = "TBKK"; // Tipe BKK (Finance)
        public const string PostedStatus = "POST"; // status posting (Finance)
        public const string VoidStatus = "VOID"; // status Void (Finance)
        public const string PostVoidInvType = "IPOS"; // Tipe Posting/Void Invoice(Finance)
        public const string PostVoidPayType = "PPOS"; // Tipe Posting/Void Payment (Finance)
        public const string BKT_KwitansiType = "TBKT"; // Tipe Kwitansi (Finance)
        public const string BKKTypeNoOthers = "BKKN"; // Tipe BKK tanpa Others (Finance)
        public const string BKT_BankType = "BBKT"; // Tipe Bank Kas (Finance)
        public const string PermohonanUangCentralize = "FNRECDCS"; // Permohonan Uang (Invoice Others Suzuki) Terpusat

        //TAX
        public const string TaxCodeIN = "TXCI"; // Kode Pajak(Tax)
        public const string TaxTransactionCodeIN = "TRCI"; // Kode Transaksi Pajak(Tax)
        public const string TaxStatusCodeIN = "TSCI"; // Kode Status Pajak(Tax)
        public const string TaxCodeOUT = "TXCO"; // Kode Pajak(Tax)
        public const string TaxTransactionCodeOUT = "TRCO"; // Kode Transaksi Pajak(Tax)
        public const string TaxStatusCodeOUT = "TSCO"; // Kode Status Pajak(Tax)
        public const string TaxDocumentCode = "TDCD"; // Kode Dokumen Pajak(Tax)
        public const string TaxDocumentType = "TDTP"; // Tipe Dokumen Pajak (Faktur atau Retur)
        public const string TaxType = "TXTP"; // Tipe Transaksi Pajak (Faktur Penjualan, Retur Penjualan, Faktur Pembelian, Retur Pembelian, Grand Total)
        public const string TaxOnline = "TXOL"; // Tipe Transaksi Pajak (Faktur Penjualan, Retur Penjualan, Faktur Pembelian, Retur Pembelian, Grand Total)

        //ORDER MANAGEMENT
        public const string FakturCategory = "FPCT"; // Kategori OM
        public const string Installment = "INST"; //Installment
        public const string MappingWarehouse = "MPWH"; // Mapping BranchCode Warehouse 
        public const string PurchaseLocking = "PURL"; // Locking For Purchase Screen
        public const string IsLinkToITS = "ITSFL"; // Flag for Link to ITS
        public const string FlagSTK = "SDSTK"; // Flag for Sales Stock Info Setting 
        public const string FlagFRQ = "SFREQ"; // Flag for Sales Faktur Polisi Setting
        public const string FlagFRV = "SFREV"; // Flag for Sales Revisi Faktur Polisi Setting
        public const string ReturBranch = "RETUR_BRANCH"; // Flag for Info Branch on Report Retur

        //PRE SALES
        public const string FlagSIS = "SITSD";
        public const string SalesGrade = "ITSG";
        public const string ITPosition = "PGRD";
        public const string Outlet = "OLET";
        public const string ProspectInqStatus = "PMSP";
        public const string ProspectPayBy = "PMBY";
        public const string ProspectOption = "PMOP";
    }

    public partial class WsParamID
    {
        public const string DCS_URL = "DCS_URL";
        public const string TAX_URL = "TAX_URL";
    }

    public partial class DcsWsAction
    {
        public const string GetData = "GetData";
        public const string IsValid = "IsValid";
        public const string RetrieveData ="RetrieveData";
        public const string RetrieveDcsDownloadFile ="RetrieveDcsDownloadFile";
        public const string RetrieveDownloadByDataID = "RetrieveDownloadByDataID";
        public const string RetrieveDownloadByLastID ="RetrieveDownloadByLastID";
        public const string RetrieveDownloadByRangeDataID = "RetrieveDownloadByRangeDataID";
        public const string RetrieveDownloadData = "RetrieveDownloadData";
        public const string RetrieveUploadData = "RetrieveUploadData";
        public const string RetrieveUploadDataV2= "RetrieveUploadDataV2";
        public const string SendToDcs = "SendToDcs";
        public const string UpdateDownloadDataStatus = "UpdateDownloadDataStatus";
        public const string UpdateUploadDataStatus = "UpdateUploadDataStatus";
        
    }

    public class ElmahCommandInterceptor : IDbCommandInterceptor
    {
        //private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void NonQueryExecuting(
            DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            LogIfNonAsync(command, interceptionContext);
        }

        public void NonQueryExecuted(
            DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            LogIfError(command, interceptionContext);
        }

        public void ReaderExecuting(
            DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            LogIfNonAsync(command, interceptionContext);
        }

        public void ReaderExecuted(
            DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            LogIfError(command, interceptionContext);
        }

        public void ScalarExecuting(
            DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            LogIfNonAsync(command, interceptionContext);
        }

        public void ScalarExecuted(
            DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            LogIfError(command, interceptionContext);
        }

        private void LogIfNonAsync<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (!interceptionContext.IsAsync)
            {
                //Logger.Warn("Non-async command used: {0}", command.CommandText);
            }
        }

        private void LogIfError<TResult>(
            DbCommand command, DbCommandInterceptionContext<TResult> interceptionContext)
        {
            if (interceptionContext.Exception != null)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(interceptionContext.Exception);
            }
        }
    }

    public class MyHelpers
    {
        public static DataTable GetTable(DbContext ctx, string sSQL)
        {
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = sSQL;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
           }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static DataSet GetDataSet(DbContext ctx, string sSQL)
        {
            try
            {
                SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
                cmd.CommandText = sSQL;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                da.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static string  GetConnString(string cfgName)
        {
            string MyAppPath = HttpContext.Current.Request.ApplicationPath.ToString();   
         
            if (MyAppPath.Length > 1)
            {
                var IsMultipleApp = System.Configuration.ConfigurationManager.AppSettings["MultipleApp"] ?? "0";
                if (Convert.ToBoolean(IsMultipleApp))
                {
                    cfgName += MyAppPath.Replace(@"/", "_");
                }                
            }

            string cnStr =  System.Configuration.ConfigurationManager.ConnectionStrings[cfgName].ToString();

            if (cfgName == "MDContext" &&  string.IsNullOrEmpty(cnStr))
            {
                cnStr = System.Configuration.ConfigurationManager.ConnectionStrings["DataContext"].ToString();
            }

            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                cnStr += "-" + HttpContext.Current.User.Identity.Name;
            }

            ////MyLogger.Info("Conn String: " + cnStr);
            return cnStr;
        }
   


    }

    public class JsonNetResult : JsonResult
    {
        public JsonNetResult()
        {
            Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Error
            };
        }

        public JsonSerializerSettings Settings { get; private set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("JSON GET is not allowed");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if (this.ContentEncoding != null)
                response.ContentEncoding = this.ContentEncoding;
            if (this.Data == null)
                return;

            var scriptSerializer = JsonSerializer.Create(this.Settings);

            using (var sw = new StringWriter())
            {
                try
                {
                        scriptSerializer.Serialize(sw, this.Data);
                } catch(Exception ex)
                {

                }
                
                response.Write(sw.ToString());
            }
        }
    }

    public class MyComboData
    {
        public string value { get; set; }
        public string text { get; set; }
        public string parent { get; set; }
    }

    public class GnMstDocumentConstant
    {
        /// A
        public const string ADJ = "ADJ"; // Adjustment Inventory (Spareparts)
        public const string APJ = "APJ"; //Journal No AP (Finance AP)
        public const string ARJ = "ARJ"; //Journal No (Finance AR)
        public const string ARR = "ARR"; //Return 3S (Finance AR)
        /// B
        public const string BAT = "BAT"; // PDI FSC Batch (Service)
        public const string BNL = "BNL"; // Binning List (Spareparts)
        public const string BPS = "BPS"; // Non Penjualan Bon Pengeluaran Spareparts (Spareparts)
        public const string BPF = "BPF"; // Non Penjualan Faktur (Spareparts)
        public const string BOC = "BOC"; // BO Cancel (Spareparts)
        public const string BOK = "BOK"; // No Booking (Service)
        public const string BPU = "BPU"; // Bukti Penerimaan Unit (Order Management/Unit)
        public const string BTU = "BTU"; //BTT Others - PU (Finance AP)
        public const string BBS = "BBS"; //BTT Others - BS (Finance AP)
        public const string BPJ = "BPJ"; //BTT Others - PJBS (Finance AP) 
        public const string BIV = "BIV"; //BTT Others - Invoice (Finance AP)
        public const string BPR = "BPR"; //BTT Others - Return (Finance AP)
        public const string BKK = "BKK"; //Bank Keluar (Finance AP)
        public const string BNP = "BNP"; //Batch No  (Finance AP)
        public const string BNR = "BNR"; //Batch No (Finance AR)
        public const string BKT = "BKT"; //Bank Terima (Finance AR) 
        public const string BNG = "BNG"; //Batch No Posting (Finance GL)
        public const string BPG = "BPG"; //Batch No UnPosting(Finance GL)
        public const string BPK = "BPK"; //Bukti Penerimaan Kredit
        public const string BFP = "BFP"; // Batch No Export Faktur Polisi (Order Management/Unit)
        public const string BRFP = "BRFP"; // Batch No Export Faktur Polisi (Order Management/Unit)
        public const string BVC = "BVC"; // Batch No Export Stock (Order Management/Unit)
        /// C 
        public const string CLM = "CLM"; // Supplier Claim (Spareparts)
        public const string CLA = "CLA"; // Warranty Claim (Service)
        public const string CLW = "CLW"; // Warranty Claim SPK (Service)
        public const string CKK = "CKK"; //Kas Keluar (Finance AP)
        public const string CRN = "CRN"; //Credit Note (Finance AP)
        public const string CBK = "CBK"; //Cek Keluar (Finance AP)
        public const string CKT = "CKT"; //Kas Terima (Finance AR)
        public const string CCT = "CCT"; //Cek Terima (Finance AR)
        public const string CCS = "CCS"; //Cek Setor (Finance AR)
        public const string CKC = "CKC"; //Cek Cair (Finance AR)
        public const string CCO = "CCO"; //Change Color (Sales Inventory)
        public const string CLR = "CLR"; // Receiving Claim No (Spareparts)
        ///D
        public const string DOR = "DOR"; // DO Release (Order Management/Unit)
        public const string DBN = "DBN"; //Debet Note (Finance AP)
        public const string DOS = "DOS"; // DO Sales(Order Management/Unit)
        public const string DNU = "DNU"; // Debet Note (Order Management/Unit)
        public const string DLRSTK = "DLRSTK"; // Dealer Stock (Order Managemen)
        /// E
        public const string EVT = "EVT"; // No Event (Service)
        public const string EST = "EST"; // No Estimasi (Service)
        /// F
        public const string FPJ = "FPJ"; // Faktur Pajak Standard (Spareparts)
        public const string FPS = "FPS"; // Faktur Pajak Standard (Service)
        public const string SDH = "SDH"; // Faktur Pajak Sederhana (Spareparts)
        public const string FSC = "FSC"; // Get PDI FSC (Service)
        public const string FPF = "FPF"; //Faktur Pajak Sederhana (Finance AR)
        public const string FPO = "FPO"; //Faktur Pajak Sederhana (Order Management)
        public const string FPH = "FPH"; //Faktur Pajak Gabungan (Holding) (Service)
        public const string FPOL = "FPOL"; //Faktur Polisi(Order Management)

        /// G
        public const string GBK = "GBK"; //Giro Bank Keluar (Finance AP)
        public const string GCK = "GCK"; //Giro Cek Keluar (Finance AP)
        public const string GCT = "GCT"; //Giro Terima (Finance AR)
        public const string GCS = "GCS"; //Giro Setor (Finance AR)
        public const string GCC = "GCC"; //Giro Cair (Finance AR)
        ///H
        public const string HPP = "HPP"; // Entry HPP (Spareparts)
        public const string HPU = "HPU"; // Harga Pokok Penjualan (Order Management/Unit)
        /// I
        public const string INV = "INV"; // Invoice Penjualan (Spareparts)
        public const string INC = "INC"; // "Penjualan" Invoice for customer (Service)
        public const string INA = "INA"; // "Penjualan" Invoice for Insurance (Service)
        public const string INI = "INI"; // "Penjualan" Invoice for Internal (Service)
        public const string INF = "INF"; // "Penjualan" Invoice for PDI/ FSC (Service)
        public const string INW = "INW"; // "Penjualan" Invoice for Warranty Claim (Service)
        public const string INP = "INP"; // "Penjualan" Invoice for Package (Paket Service)
        public const string ISU = "ISU"; // "Get Warranty Claim from Faktur Penjualan" Issue for Warranty Claim (Service)
        public const string IVP = "IVP"; //Invoice - With PPN (Finance AR)
        public const string IVN = "IVN"; //Invoice - Non PPN (Finance AR)
        public const string IVU = "IVU"; //Invoice (Order Management/Unit)
        ///J
        public const string JNH = "JNH"; //Jurnal Harian (Finance GL)
        public const string JNM = "JNM"; //Jurnal Memorial (Finance GL)
        public const string JNA = "JNA"; //Jurnal Audit (Finance GL)
        public const string JTS = "JTS"; //Jurnal Transfer Stock
        public const string JTI = "JTI"; //Jurnal Transfer Internal
        /// K
        public const string KTK = "KTK"; // Kontrak (Service)
        public const string KRI = "KRI"; // Karoseri (Order Management/Unit)
        public const string KRT = "KRT"; // Karoseri Terima (Order Management/Unit)
        public const string KLB = "KLB"; // Kode Klub (Service)
        public const string KWT = "KWT"; // Penerimaan Kwitansi (Finance AR) 
        /// L
        public const string LMP = "LMP"; // Lampiran Dokumen Non Penjualan (Spareparts)
        public const string LOT = "LOT"; // Lot No (Service)
        /// P
        public const string POS = "POS"; // Part Order Sheet (Spareparts)
        public const string POT = "POT"; // Input Pekerjaan Luar (Service)
        public const string PLS = "PLS"; // Picking Slip (Spareparts)
        public const string PUR = "PUR"; // Purchase Order (Order Management/Unit)
        public const string PIN = "PIN"; // Perlengkapan In (Order Management/Unit)
        public const string PAD = "PAD"; // Perlengkapan Adjustment (Order Management/Unit)
        public const string PJK = "PJK"; // Faktur Pajak Standard(Finance AR)
        public const string PJU = "PJU"; // Faktur Pajak Standard(Order Management/Unit)
        public const string PLK = "PLK"; // Perlengkapan Out (Order Management/Unit)
        /// R
        public const string RTR = "RTR"; // Return Penjualan (Spareparts)
        public const string RSV = "RSV"; // Reserved part inventory (Spareparts)
        public const string RTP = "RTP"; // Purchase Return (Order Management/Unit)
        public const string RDN = "RDN"; // Debet Note (Finance AR)
        public const string RCN = "RCN"; // Credit Note (Finance AR)
        public const string REC = "REC"; // Reconciled (Finance CM)
        public const string RTS = "RTS"; // Sales Return (Order Management/Unit)
        public const string RRO = "RRO"; // RRO Dokumen(Service)
        public const string RFP = "RFP"; // Req. Faktur Polisi (Order Management/Unit)
        public const string RTN = "RTN"; // Retur Service

        /// S
        public const string SGR = "SGR"; // Suggor (Spareparts)
        public const string SOC = "SOC"; // Sales Spareparts (Spareparts)
        public const string SSS = "SSS"; // Supply Slip Service (Spareparts)
        public const string SSU = "SSU"; // Supply Slip Unit (Spareparts)
        public const string STH = "STH"; // Stock Taking Header (Spareparts)
        public const string STK = "STK"; // Stock Taking Temporary (Spareparts)
        public const string SPK = "SPK"; // Surat Perintah Kerja (Service)
        public const string SHN = "SHN"; // Faktur Pajak Sederhana (Service)
        public const string SJR = "SJR"; // SJ Release (Order Management/Unit)
        public const string SOR = "SOR"; // Sales Order (Order Management/Unit)
        public const string SPF = "SPF"; // SPK (Order Management/Unit)
        public const string SPU = "SPU"; // SPU (Draft Sales Order/Unit)
        public const string STC = "STC"; // Stock Taking (Order Management/Unit)
        public const string SON = "SON"; // Catatan Penjualan (Order Management/Unit)
        public const string STR = "STR"; // Retur Supply Slip (Spareparts)
        public const string SJAL = "SJAL"; // Surat Jalan Dealer Stock(Order Management)

        /// T
        public const string TTB = "TTB"; // Tanda Terima BPKB (Order Management/Unit)
        public const string TTS = "TTS"; // Tanda Terima STNK (Order Management/Unit)
        ///V
        public const string VTO = "VTO"; // Inventory Transfer Out (Order Management/Unit)
        public const string VTI = "VTI"; // Inventory Transfer In (Order Management/Unit)
        /// W
        public const string WRL = "WRL"; // WRS tipe Pembelian (Spareparts)
        public const string WRN = "WRN"; // WRS tipe Non Pembelian (Spareparts)
        public const string WTR = "WTR"; // Warehouse Transfer (Spareparts)
    }

    public class OmMstRefferenceConstant
    {
        public const string CLCD = "COLO"; // Reff.Code Colour
        public const string GRPR = "GRPR"; // Reff. Group Price Code
        public const string WARE = "WARE"; // Reff. Warehouse
        public const string OTHS = "OTHS"; // Reff. Aksesoris Lain - lain
        public const string RSNF = "RSNF"; // Reason Code Non Faktur

    }
}
