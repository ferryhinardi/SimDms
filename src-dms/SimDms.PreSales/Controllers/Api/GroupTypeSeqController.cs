using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common.Models;
using SimDms.PreSales.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class GroupTypeSeqController : BaseController
    {
        private string InsertNewData(GroupItem data)
        {
            var message = "";
            try
            {
                var record = new GroupType
                {
                    CompanyCode = CompanyCode,
                    GroupCode = data.GroupCode,
                    TypeCode = data.TypeCode,
                    CreatedBy = CurrentUser.UserId,
                    CreatedDate = DateTime.Now,
                    UsageFlag = "I",
                    GroupCodeSeq = data.GroupCodeSeq.Value,
                    TypeCodeSeq = data.TypeCodeSeq.Value,
                    LastUpdateBy = CurrentUser.UserId,
                    LastUpdateDate = DateTime.Now
                };
                ctx.GroupTypes.Add(record);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        private string UpdateOrderGroupCodeSeq(decimal seqOld, decimal seqNew, string groupCode)
        {
            var message = "";
            try
            {
                var query = @"UPDATE pmGroupTypeSeq SET GroupCodeSeq = 0 WHERE CompanyCode = @p0 AND GroupCode = @p1";
                ctx.Database.ExecuteSqlCommand(query, CompanyCode, groupCode);

                if (seqOld > seqNew)
                {
                    query = @"UPDATE pmGroupTypeSeq SET GroupCodeSeq = GroupCodeSeq + 1 
                                    WHERE CompanyCode = @p0 AND GroupCodeSeq >= @p1 AND GroupCodeSeq < @p2";
                }
                else
                {
                    query = @"UPDATE pmGroupTypeSeq SET GroupCodeSeq = GroupCodeSeq - 1 
                                    WHERE CompanyCode = @p0 AND GroupCodeSeq <= @p1 AND GroupCodeSeq > @p2";
                }
                ctx.Database.ExecuteSqlCommand(query, CompanyCode, seqNew, seqOld);

                query = @"UPDATE pmGroupTypeSeq SET GroupCodeSeq = @p1, LastUpdateDate = @p2
                                    , LastUpdateBy = @p3 WHERE CompanyCode = @p0 AND GroupCode = @p4";
                ctx.Database.ExecuteSqlCommand(query, CompanyCode, seqNew, DateTime.Now, CurrentUser.UserId, groupCode);

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return message;
        }

        private string UpdateOrderTypeCodeSeq(decimal seqOld, decimal seqNew, string groupCode, string typeCode)
        {
            var message = "";
            try
            {
                var query = @"UPDATE pmGroupTypeSeq SET TypeCodeSeq = 0 
                                WHERE CompanyCode = @p0 AND GroupCode = @p1 AND TypeCode = @p2";
                ctx.Database.ExecuteSqlCommand(query, CompanyCode, groupCode, typeCode);

                if (seqOld > seqNew)
                {
                    query = @"UPDATE pmGroupTypeSeq SET TypeCodeSeq=TypeCodeSeq+1 
                                        WHERE CompanyCode = @p0 AND TypeCodeSeq >= @p1 AND 
                                        TypeCodeSeq < @p2 AND GroupCode = @p3";
                    ctx.Database.ExecuteSqlCommand(query, CompanyCode, seqNew, seqOld, groupCode);
                }
                else
                {
                    query = @"UPDATE pmGroupTypeSeq SET TypeCodeSeq = TypeCodeSeq - 1 
                                        WHERE CompanyCode = @p0 AND TypeCodeSeq <= @p1 AND 
                                        TypeCodeSeq > @p2 AND GroupCode = @p3";
                    ctx.Database.ExecuteSqlCommand(query, CompanyCode, seqNew, seqOld, groupCode);
                }
                query = @"UPDATE pmGroupTypeSeq SET TypeCodeSeq=@p1, LastUpdateBy=@p2,
                                        LastUpdateDate=@p3 WHERE CompanyCode = @p0 AND 
                                        GroupCode=@p4 AND TypeCode=@p5";
                ctx.Database.ExecuteSqlCommand(query, CompanyCode, seqNew, CurrentUser.UserId, 
                    DateTime.Now, groupCode, typeCode);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }

        public JsonResult GetGroupTypeSeq(string groupCode)
        {
            var query = @"
SELECT
     a.GroupCode, isnull(convert(varchar(5),b.GroupCodeSeq),'') GroupCodeSeq, a.TypeCode, isnull(convert(varchar(5),TypeCodeSeq),'') TypeCodeSeq
FROM
    OmMstModel a
INNER JOIN PmGroupTypeSeq b
    ON b.CompanyCode=a.CompanyCode AND b.GroupCode = a.GroupCode AND b.TypeCode = a.TypeCode 
WHERE
    a.CompanyCode = @p0
    AND a.GroupCode <>'' 
    AND ((CASE WHEN @p1 = '' THEN a.GroupCode END)= a.GroupCode 
            OR (CASE WHEN @p1 <> '' THEN a.GroupCode END)=@p1)
GROUP BY 
    a.GroupCode, b.GroupCodeSeq, a.TypeCode, TypeCodeSeq
ORDER BY 
    GroupCodeSeq ASC, TypeCodeSeq ASC
";
            var result = ctx.Database.SqlQuery<GroupItemSeq>(query, CompanyCode, groupCode).AsQueryable();

            return Json(result);
        }

        public JsonResult Save(GroupItem data)
        {
            var message = "";
            
            try
            {
                var record = ctx.GroupTypes.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                    x.GroupCode == data.GroupCode && x.TypeCode == data.TypeCode);
                if (record == null)
                {
                    var checkTypeSeq = ctx.GroupTypes.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                        x.GroupCodeSeq == data.GroupCodeSeq && x.TypeCodeSeq == data.TypeCodeSeq &&
                        x.GroupCode == data.GroupCode);
                    if (checkTypeSeq != null) throw new Exception("Urutan variant sudah terdaftar!");
                    var checkGroupVariant = ctx.GroupTypes.FirstOrDefault(x => x.CompanyCode == CompanyCode &&
                        x.GroupCodeSeq == data.GroupCodeSeq && x.GroupCode != data.GroupCode);
                    if (checkGroupVariant == null)
                    {
                        message = InsertNewData(data);
                        if (message != "") throw new Exception("Proses Update Gagal !" + message);
                        return Json(new { message = message });
                    }
                    else
                    {
                        decimal urut = 0;
                        List<GroupType> ListRturDtl = ctx.GroupTypes.Where(a => a.CompanyCode == CompanyCode && a.GroupCodeSeq >= data.GroupCodeSeq).OrderBy(a=> a.GroupCodeSeq).ToList();
                        foreach (var CodeSeq in ListRturDtl)
                        {
                            if (urut == 0) { urut = CodeSeq.GroupCodeSeq; } else { urut = urut + Convert.ToDecimal(1); }
                            var NextSeq = ctx.GroupTypes.Where(a => a.CompanyCode == CompanyCode && a.GroupCodeSeq == urut).FirstOrDefault();
                            if (NextSeq != null)
                            {
                                var query = @"UPDATE PmGroupTypeSeq SET GroupCodeSeq = @p3
                                WHERE CompanyCode = @p0 AND GroupCode = @p1 AND GroupCodeSeq = @p2";
                                ctx.Database.ExecuteSqlCommand(query, CompanyCode, CodeSeq.GroupCode, CodeSeq.GroupCodeSeq, CodeSeq.GroupCodeSeq + 1);
                            }
                            else
                            {
                                break;
                            }
                        };
                        message = InsertNewData(data);
                        if (message != "") throw new Exception("Proses Update Gagal !" + message);
                        return Json(new { message = message });
                    }
                }
                else
                {
                    if(record.GroupCodeSeq != data.GroupCodeSeq.Value &&
                        record.TypeCodeSeq != data.TypeCodeSeq.Value)
                        throw new Exception("Hanya diperbolehkan mengubah salah satu urutan saja !");
                    var seqOld = decimal.Zero;
                    var seqNew = decimal.Zero;
                    if (record.GroupCodeSeq != data.GroupCodeSeq.Value)
                    {
                        seqOld = record.GroupCodeSeq;
                        seqNew = data.GroupCodeSeq.Value;

                        // Update Order GroupCodeSeq 
                        message = UpdateOrderGroupCodeSeq(seqOld, seqNew, data.GroupCode);
                        if (message != "") throw new Exception("Proses Update Gagal !" + message);
                        return Json(new { message = message });
                    }

                    // Yang Berubah TypeCodeSeq
                    if (record.TypeCodeSeq != data.TypeCodeSeq.Value)
                    {
                        seqOld = record.TypeCodeSeq;
                        seqNew = data.TypeCodeSeq.Value;

                        // Update Order TypeCodeSeq
                        message = UpdateOrderTypeCodeSeq(seqOld, seqNew, data.GroupCode, data.TypeCode);
                        if (message != "") throw new Exception("Proses Update Gagal !" + message);
                        return Json(new { message = message });
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message });
        }
    }
}