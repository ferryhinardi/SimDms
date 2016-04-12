using SimDms.Common.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using SimDms.Common.DcsWs;
using System;
using System.Text;
using System.Collections.Generic;
using System.Globalization;

namespace SimDms.Common
{
    public class DcsHelper
    {
        protected CommonContext ctx = new CommonContext();
        private readonly DcsWsSoapClient ws = new DcsWsSoapClient();
        private string UserId;

        public DcsHelper(string _UserId)
        {
            UserId = _UserId;
        }

        public DcsHelper(CommonContext _ctx, string _UserId)
        {
            ctx = _ctx;
        }

        private MyUserInfo CurrentUserInfo()
        {
            string s = "";
            var f = ctx.UserProfitCenters.Find(UserId);
            if (f != null) s = f.ProfitCenter;
            var u = ctx.Users.Find(UserId);
            var g = ctx.MstCoProfiles.Find(u.CompanyCode, u.BranchCode);
            u.CoProfile = ctx.MstCoProfiles.Find(u.CompanyCode, u.BranchCode);
            var uv = ctx.UserViews.Find(UserId);

            var info = new MyUserInfo
            {
                UserId = u.UserId,
                FullName = u.FullName,
                CompanyCode = u.CompanyCode,
                CompanyGovName = g.CompanyGovName,
                BranchCode = u.BranchCode,
                CompanyName = g.CompanyName,
                TypeOfGoods = u.TypeOfGoods,
                ProductType = g.ProductType,
                ProfitCenter = s,
                TypeOfGoodsName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TypeOfGoods, u.TypeOfGoods),
                ProductTypeName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProductType, g.ProductType),
                ProfitCenterName = GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.ProfitCenter, s),
                IsActive = u.IsActive,
                RequiredChange = u.RequiredChange,
                SimDmsVersion = GetType().Assembly.GetName().Version.ToString(),
                ShowHideTypePart = (uv.RoleId.Equals("Admin", StringComparison.InvariantCultureIgnoreCase) || f.ProfitCenter == "300") ? GetLookupValueName(u.CompanyCode, GnMstLookUpHdr.TypeOfGoods, u.TypeOfGoods) : ""
            };

            return info;
        }

        private string GetLookupValueName(string CompanyCode, string VarGroup, string varCode)
        {
            string s = "";
            var x = ctx.MstLookUpDtls.Find(CompanyCode, VarGroup, varCode);
            if (x != null) s = x.LookUpValueName;
            return s;
        }

        public string WSUrl
        {
            get 
            {
                var paramValue = ctx.Parameters.Find(WsParamID.DCS_URL).ParamValue;
               return paramValue;
            }
        }

        public bool IsValid()
        {
            bool result = false;
            try
            {
                result = ws.IsValid();
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public IEnumerable<GnDcsUploadFile> RetrieveUploadDataV2(string dataId, DateTime dateFrom, DateTime dateTo, bool online, bool allStatus)
        {
            IEnumerable<GnDcsUploadFile> records;
            var uid = CurrentUserInfo();
            try
            {
                if (online)
                {
                    var recDcs = ctx.GnDcsUploadFiles.Where(p => p.DataID == dataId);
                    long id = (recDcs.Count() > 0) ? Convert.ToInt64(recDcs.Max(p => p.ID)) : 0;

                    string custCode = ctx.Database.SqlQuery<string>("exec uspfn_gnGetDcsDealerCode @CompanyCode=@p0, @BranchCode=@p1, @CodeID=@p2",
                        uid.CompanyCode, uid.BranchCode, dataId).FirstOrDefault();

                    DcsWsSoapClient ws = new DcsWsSoapClient();
                    var data = ws.RetrieveUploadDataV2(custCode, dataId, id);

                    if (data.Count > 0 && data[0].StartsWith("FAIL"))
                    {
                        throw new Exception(data[0].Substring(5));
                    }

                    foreach (string var in data)
                    {
                        string[] lines = var.Split('\n');
                        string[] headers = lines[0].Split('|');

                        StringBuilder sbContent = new StringBuilder();
                        for (int i = 1; i < lines.Length; i++)
                        {
                            if (lines[i].Length > 10)
                            {
                                //content += ((i == 1) ? "" : "\n") + lines[i];
                                sbContent.Append(((i == 1) ? "" : "\n") + lines[i]);
                            }
                        }

                        var record = new GnDcsUploadFile();
                        record.ID = Convert.ToDecimal(headers[0]);
                        record.DataID = headers[1];
                        record.CustomerCode = headers[2];
                        record.ProductType = headers[3];
                        record.Contents = sbContent.ToString();
                        record.Status = headers[4];
                        //record.CreatedDate = (headers[5] != null) ? Convert.ToDateTime(headers[5]) : new DateTime(1900, 1, 1);
                        record.CreatedDate = (headers[5] != null) ? DateTime.ParseExact(headers[5], "yyyyMMdd HH:mm:ss", CultureInfo.InvariantCulture) : new DateTime(1900, 1, 1);
                        record.Header = lines[1];

                        ctx.GnDcsUploadFiles.Add(record);
                        ctx.SaveChanges();
                    }
                }
                
                var productType = uid.ProductType == "2W" ? "2" : "4";
                var productType1 = string.Empty;
                if (uid.ProfitCenter == "300")
                    productType1 = uid.ProductType == "2W" ? "2" : "4";
                else
                    productType1 = uid.ProductType == "2W" ? "A" : "B";

                if (allStatus)
                {

                    records = ctx.Database.SqlQuery<GnDcsUploadFile>(@"
                            select ID, DataID, CustomerCode, ProductType, Contents
                                 , CASE WHEN Status = 'A' THEN 'BARU' ELSE CASE WHEN Status = 'X' THEN 'GAGAL' ELSE CASE WHEN Status = 'P' THEN 'BERHASIL' END END END Status
                                 , CreatedDate, UpdatedDate, Header
                              from gnDcsUploadFile
                             where 1 = 1
                               and DataID = @p0
                               and (convert(varchar, CreatedDate, 112) between convert(varchar, @p1, 112) and convert(varchar, @p2, 112)) 
                               and ProductType in (@p3, @p4)",
                               dataId, dateFrom, dateTo, productType, productType1);
                }
                else
                {
                    records = ctx.Database.SqlQuery<GnDcsUploadFile>(@"
                            select ID, DataID, CustomerCode, ProductType, Contents
                                 , CASE WHEN Status = 'A' THEN 'BARU' ELSE CASE WHEN Status = 'X' THEN 'GAGAL' ELSE CASE WHEN Status = 'P' THEN 'BERHASIL' END END END Status
                                 , CreatedDate, UpdatedDate, Header
                              from gnDcsUploadFile
                             where 1 = 1
                               and DataID=@p0
                               and (convert(varchar, CreatedDate, 112) between convert(varchar,@p1, 112) and convert(varchar, @p2, 112)) 
                               and ProductType in (@p3, @p4)
                               and Status = 'A'",
                               dataId, dateFrom, dateTo, productType, productType1);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            uid = null;

            return records.OrderByDescending(p => p.CreatedDate);
        }

        public bool Validate(string[] lines, UploadTypeDCS uploadType)
        {
            switch (uploadType)
            {
                // REGION : SPAREPART                       
                case UploadTypeDCS.PINVS:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PINVS"));
                case UploadTypeDCS.PPRCD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PPRCD"));
                case UploadTypeDCS.PMODP:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PMODP"));
                case UploadTypeDCS.PMDLM:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("PMDLM"));
                case UploadTypeDCS.MSMDL:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("MSMDL"));

                //region : service
                case UploadTypeDCS.WCAMP:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WCAMP"));
                case UploadTypeDCS.WFRAT:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WFRAT"));
                case UploadTypeDCS.WJUDG:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WJUDG"));
                case UploadTypeDCS.WPDFS:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WPDFS"));
                case UploadTypeDCS.WSECT:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WSECT"));
                case UploadTypeDCS.WTROB:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WTROB"));
                case UploadTypeDCS.WWRNT:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("WWRNT"));

                // REGION : SALES                       
                case UploadTypeDCS.SPORD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SPORD"));
                case UploadTypeDCS.SDORD:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SDORD"));
                case UploadTypeDCS.SPRIC:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SPRIC"));
                case UploadTypeDCS.SSJAL:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SSJAL"));
                case UploadTypeDCS.SHPOK:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SHPOK"));
                case UploadTypeDCS.SACCS:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SACCS"));
                case UploadTypeDCS.SFPO1:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SFPO1"));
                case UploadTypeDCS.SFPO2:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SFPO2"));
                case UploadTypeDCS.SFPLB:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SFPLB"));
                case UploadTypeDCS.SUADE:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SUADE"));
                case UploadTypeDCS.SFPDA:
                    return (lines.Length > 0 && lines[0].Substring(1, 5).Equals("SFPDA"));

                default:
                    return false;
            }
        }

        #region *** support sfplb ***
        public string GetParaValueDCS(string companyCode, string codeId, string lookValue)
        {
            string x = "";
            LookUpDtl ox = ctx.MstLookUpDtls.Find(companyCode, codeId, lookValue);
            x = ox.ParaValue;
            return x;
        }
        #endregion

        #region *** update status gnDcsUploadFile & ws **
        public int UpdateUploadDataStatusCtx(long id, bool success)
        {
            int result = 0;
            string status = (success) ? "P" : "X";

            //update status gnDcsUploadFile
            var sql = string.Format("update gnDcsUploadFile set Status = '{0}', UpdatedDate = getdate() where ID ='{1}'", status, id);
            result = ctx.Database.ExecuteSqlCommand(sql);

            //update status ws untuk test local di command
            //ws.UpdateUploadDataStatus(id, status);

            return result;
        }

        #endregion

    }
}
            