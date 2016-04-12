using SimDms.DataWarehouse.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class ModelMappingController : BaseController
    {
        string companyCode = "0000000";

        public JsonResult LoadTree()
        {
            try
            {
                var query = "exec uspfn_mmmLoadMappings @p0";
                var rawdata = ctx.Database.SqlQuery<ModelMappingTreeItem>(query, companyCode).ToList();
                if (rawdata.Count() == 0) return Json(new { result = rawdata });
                var maxLvl = rawdata.Max(x => x.lvl);

                var structured = new List<ModelMappingTreeItem>();

                for (int i = 0; i <= maxLvl; i++)
                {
                    var records = rawdata.Where(x => x.lvl == i).ToList();
                    records.ForEach(x => { x.data = new List<ModelMappingTreeItem>(); });
                    if (i == 0) structured.AddRange(records);
                    else
                    {
                        foreach (var record in records)
                        {
                            var head = structured.Flatten(x => x.data).FirstOrDefault(x =>
                                i == 1 ? x.Item == record.GroupCode && x.GroupCodeSeq == record.GroupCodeSeq :
                                i == 2 ? x.Item == record.TypeCode && x.TypeCodeSeq == record.TypeCodeSeq && x.GroupCodeSeq == record.GroupCodeSeq :
                                x.Item == record.TransmissionType && x.TransmissionSeq == record.TransmissionSeq && x.TypeCodeSeq == record.TypeCodeSeq && x.GroupCodeSeq == record.GroupCodeSeq);
                            if (head != null) head.data.Add(record);
                        }
                    }
                }
                return Json(new { message = "", result = structured });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
            
        }

        public JsonResult LoadSmc()
        {
            var query = @"
SELECT a.SalesModelCode
FROM omMstModel a
WHERE a.CompanyCode = @p0 AND a.Status = '1' AND a.SalesModelCode NOT IN (
        SELECT SalesModelCode
        FROM msModelMappingDtl b
        WHERE b.CompanyCode = @p0
    )
ORDER BY a.SalesModelCode
";
            var smcs = ctx.Database.SqlQuery<ModelMappingSmcItem>(query, companyCode).ToList();

            return Json(new { result = smcs });
        }

        public JsonResult LoadGroupCodes()
        {
            try
            {
                var groupCodes = ctx.MsModelMappingHdrs.Where(x => x.CompanyCode == companyCode)
                    .GroupBy(x => x.GroupCode)
                    .Select(x => new
                    {
                        value = x.Key,
                        label = x.Key
                    });
                return Json(new { message = "", items = groupCodes });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult LoadTypeCodes(string groupCode)
        {
            try
            {
                var typeCodes = ctx.MsModelMappingHdrs.Where(x => x.CompanyCode == companyCode && x.GroupCode == groupCode)
                    .GroupBy(x => x.TypeCode)
                    .Select(x => new 
                    {
                        value = x.Key,
                        label = x.Key
                    });
                return Json(new { message = "", items = typeCodes });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult ReorderGroupCodes()
        {
            try
            {
                var query = "exec uspfn_mmmReorderGroupCode @CompanyCode";
                var parameters = new SqlParameter[] { new SqlParameter("@CompanyCode", companyCode) };
                var result = ctx.Database.ExecuteSqlCommand(query, parameters);

                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult ReorderTypeCodes(string groupCode)
        {
            try
            {
                var query = "exec uspfn_mmmReorderTypeCode @CompanyCode, @GroupCode";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@CompanyCode", companyCode),
                    new SqlParameter("@GroupCode", groupCode)
                };
                var result = ctx.Database.ExecuteSqlCommand(query, parameters);

                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult AssignSmc(string groupCode, string typeCode, string trans, string salesModelCode)
        {
            try
            {
                var record = ctx.MsModelMappingDtls.FirstOrDefault(x => x.CompanyCode == companyCode &&
                    x.GroupCode == groupCode && x.TypeCode == typeCode && x.TransmissionType == trans &&
                    x.SalesModelCode == salesModelCode);
                if (record != null) throw new Exception("This Sales Model Code is already registered. Please click Reload.");
                record = new MsModelMappingDtl
                {
                    CompanyCode = companyCode,
                    GroupCode = groupCode,
                    TypeCode = typeCode,
                    TransmissionType = trans,
                    SalesModelCode = salesModelCode,
                    CreatedBy = CurrentUser.Username,
                    CreatedDate = DateTime.Now,
                    LastUpdateBy = CurrentUser.Username,
                    LastUpdateDate = DateTime.Now
                };
                ctx.MsModelMappingDtls.Add(record);
                ctx.SaveChanges();
                var members = ctx.MsModelMappingDtls.Where(x => x.CompanyCode == companyCode &&
                    x.GroupCode == groupCode && x.TypeCode == typeCode && x.TransmissionType == trans);
                var memberCount = members.Count();

                return Json(new { message = "", count = memberCount });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult WithdrawSmc(string groupCode, string typeCode, string trans, string salesModelCode)
        {
            try
            {
                var record = ctx.MsModelMappingDtls.FirstOrDefault(x => x.CompanyCode == companyCode &&
                    x.GroupCode == groupCode && x.TypeCode == typeCode && x.TransmissionType == trans && 
                    x.SalesModelCode == salesModelCode);
                if (record == null) throw new Exception("This Sales Model Code is already been withdrawn. Please click Reload.");
                ctx.MsModelMappingDtls.Remove(record);
                ctx.SaveChanges();

                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult SwapSequences(string groupCode, int oldSeq, int newSeq, int lvl)
        {
            try
            {
                var query = "exec uspfn_mmmSwapSequences @CompanyCode, @GroupCode, @oldSeq, @newSeq, @lvl";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@CompanyCode", companyCode),
                    new SqlParameter("@GroupCode", groupCode),
                    new SqlParameter("@oldSeq", oldSeq),
                    new SqlParameter("@newSeq", newSeq),
                    new SqlParameter("@lvl", lvl)
                };
                var result = ctx.Database.ExecuteSqlCommand(query, parameters);
                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult DeleteItem(string groupCode, string typeCode, string trans)
        {
            try
            {
                var hdr = ctx.MsModelMappingHdrs.FirstOrDefault(x => x.CompanyCode == companyCode &&
                    x.GroupCode == groupCode && x.TypeCode == typeCode && x.TransmissionType == trans);
                var dtls = ctx.MsModelMappingDtls.Where(x => x.CompanyCode == companyCode &&
                    x.GroupCode == groupCode && x.TypeCode == typeCode && x.TransmissionType == trans);
                if (dtls.Count() > 0)
                {
                    foreach (var dtl in dtls.ToList())
                    {
                        ctx.MsModelMappingDtls.Remove(dtl);
                        ctx.SaveChanges();
                    }
                }

                var query = @"INSERT INTO hstModelMappingHdr
SELECT @CompanyCode, @GroupCode, @TypeCode, @TransmissionType, @LastUpdateDate, @LastUpdateBy, @CreatedDate, @CreatedBy, @DeletedDate, @DeletedBy
";
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@CompanyCode", hdr.CompanyCode),
                    new SqlParameter("@GroupCode", hdr.GroupCode),
                    new SqlParameter("@TypeCode", hdr.TypeCode),
                    new SqlParameter("@TransmissionType", hdr.TransmissionType),
                    new SqlParameter("@LastUpdateDate", hdr.LastUpdateDate),
                    new SqlParameter("@LastUpdateBy", hdr.LastUpdateBy),
                    new SqlParameter("@CreatedDate", hdr.CreatedDate),
                    new SqlParameter("@CreatedBy", hdr.CreatedBy),
                    new SqlParameter("@DeletedDate", DateTime.Now),
                    new SqlParameter("@DeletedBy", CurrentUser.Username)
                };
                ctx.Database.ExecuteSqlCommand(query, parameters);
                
                ctx.MsModelMappingHdrs.Remove(hdr);
                ctx.SaveChanges();

                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult CheckNewTrans(string groupCode, string typeCode, string trans, int groupSeq, int typeSeq)
        {
            var msg = "";
            var errorType = 0;
            try
            {
                var prefix = "Type: " + groupCode +
                    "\nVariant: " + typeCode +
                    "\nTransmission: " + trans;
                //GroupCode same > TypeCode same > Trans same > Exception
                var hdr = ctx.MsModelMappingHdrs.Where(x => x.CompanyCode == companyCode &&
                    x.GroupCode == groupCode && x.TypeCode == typeCode &&
                    x.TransmissionType == trans);
                if (hdr.Count() > 0)
                {
                    msg += prefix + "\nData above is already exist in database.";
                    errorType = 1;
                }

                //GroupCode same	> TypeCode same	> Trans differ	> GroupSeq same		> TypeSeq differ	> Exception
                hdr = ctx.MsModelMappingHdrs.Where(x => x.CompanyCode == companyCode &&
                    x.GroupCode == groupCode && x.TypeCode == typeCode &&
                    x.TransmissionType != trans && x.GroupCodeSeq == groupSeq &&
                    x.TypeCodeSeq != typeSeq);
                if (hdr.Count() > 0)
                {
                    msg += prefix + "\nVariant sequence must be same as the existing sequence";
                    errorType = 1;
                }

                //GroupCode same	> TypeCode same	> Trans differ	> GroupSeq differ	> Warning > Insert
                hdr = ctx.MsModelMappingHdrs.Where(x => x.CompanyCode == companyCode &&
                    x.GroupCode == groupCode && x.TypeCode == typeCode &&
                    x.TransmissionType != trans && x.GroupCodeSeq != groupSeq);
                if (hdr.Count() > 0)
                {
                    msg += prefix + "\nData with Type and Variant above is already defined at Type Sequence no. " +
                        hdr.FirstOrDefault().GroupCodeSeq +
                        "\nProceed with Type Sequence no. " + groupSeq + "?";
                    errorType = 2;
                }          

                if (msg != "") throw new Exception(msg);
                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message, errorType = errorType });
            }
        }

        public JsonResult NewItem(string groupCode, string typeCode, string trans, int groupSeq, int typeSeq)
        {
            try
            {
                var query = "exec uspfn_mmmNewModel @GroupCode, @TypeCode, @Trans, @GroupSeq, @TypeSeq, @User";
                var parameters = new SqlParameter[] 
                {
                    new SqlParameter("@GroupCode", groupCode),
                    new SqlParameter("@TypeCode", typeCode),
                    new SqlParameter("@Trans", trans),
                    new SqlParameter("@GroupSeq", groupSeq),
                    new SqlParameter("@TypeSeq", typeSeq),
                    new SqlParameter("@User", CurrentUser.Username)
                };
                var result = ctx.Database.ExecuteSqlCommand(query, parameters);
                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult SaveChecklist(List<ModelMappingTreeItem> list)
        {
            try
            {
                foreach (var item in list)
                {
                    var record = ctx.MsModelMappingHdrs.FirstOrDefault(x => x.CompanyCode == companyCode &&
                        x.GroupCode == item.GroupCode && x.TypeCode == item.TypeCode &&
                        x.TransmissionType == item.TransmissionType);
                    if (record == null) throw new Exception(
                        "Save checklist failed because the list of data has been altered. Please click reload");
                    record.IsSelected = true;
                    record.LastUpdateBy = CurrentUser.Username;
                    record.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                }

                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public class ModelMappingTreeItem
        {
            public string id { get; set; }
            public int? lvl { get; set; }
            public string Item { get; set; }
            public string GroupCode { get; set; }
            public string TypeCode { get; set; }
            public string TransmissionType { get; set; }
            public string SalesModelCode { get; set; }
            public bool? IsSelected { get; set; }
            public string GroupCodeSeq { get; set; }
            public string TypeCodeSeq { get; set; }
            public string TransmissionSeq { get; set; }
            public string SalesModelSeq { get; set; }
            public List<ModelMappingTreeItem> data { get; set; }
        }

        private class ModelMappingSmcItem
        {
            public string SalesModelCode { get; set; }
        }
    }


    public static class Linq
    {
        public static IEnumerable<T> Flatten<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> f)
        {
            return e.SelectMany(c => f(c).Flatten(f)).Concat(e);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}