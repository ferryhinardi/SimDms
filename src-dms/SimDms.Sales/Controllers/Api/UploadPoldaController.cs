using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.Common;
using SimDms.Common.DcsWs;
using System.Data;
using SimDms.Common.Models;
using SimDms.Sales.BLL;
using SimDms.Sales.Models;
using System.Data.SqlClient;
using System.Text;
using OfficeOpenXml;
using System.IO;
using System.Web.Script.Serialization;

namespace SimDms.Sales.Controllers.Api
{
    public class UploadPoldaController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new { CompanyCode = CompanyCode, CompanyName = CompanyName });
        }

        public JsonResult CheckDuplicateData(string Data)
        {
            var message = true;
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<string> listNoFaktur = ser.Deserialize<List<string>>(Data);

            foreach (var item in listNoFaktur)
            {
                if (ctx.omFakturPoldas.Count(x => x.NoFaktur == item) > 0)
                {
                    return Json(message);
                }
            }
            message = false;
            return Json(message);
        }

        public JsonResult Save(string Data)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();
            List<omFakturPolda> listPolda = ser.Deserialize<List<omFakturPolda>>(Data);


            foreach (var item in listPolda)
            {
                SqlParameter p0 = new SqlParameter("@NoFaktur",item.NoFaktur ?? "");
                SqlParameter p1 = new SqlParameter("@TglFaktur", item.TglFaktur ?? Convert.ToDateTime("01-01-1900"));
                SqlParameter p2 = new SqlParameter("@Merk", item.Merk ?? "");
                SqlParameter p3 = new SqlParameter("@Tipe", item.Tipe ?? "");
                SqlParameter p4 = new SqlParameter("@ThnPembuatan", item.ThnPembuatan);
                SqlParameter p5 = new SqlParameter("@ThnPerakitan", item.ThnPerakitan);
                SqlParameter p6 = new SqlParameter("@Silinder",item.Silinder ?? "");
                SqlParameter p7 = new SqlParameter("@Warna",item.Warna ?? "");
                SqlParameter p8 = new SqlParameter("@NoRangka",item.NoRangka ?? "");
                SqlParameter p9 = new SqlParameter("@NoMesin",item.NoMesin ?? "");
                SqlParameter p10 = new SqlParameter("@BahanBakar", item.BahanBakar ?? "");
                SqlParameter p11 = new SqlParameter("@Pemilik", item.Pemilik ?? "");
                SqlParameter p12 = new SqlParameter("@Pemilik2", item.Pemilik2 ?? "");
                SqlParameter p13 = new SqlParameter("@Alamat", item.Alamat ?? "");
                SqlParameter p14 = new SqlParameter("@Alamat2", item.Alamat2 ?? "");
                SqlParameter p15 = new SqlParameter("@Alamat3", item.Alamat3 ?? "");
                SqlParameter p16 = new SqlParameter("@DealerCode", item.DealerCode ?? "");
                SqlParameter p17 = new SqlParameter("@DealerName", item.DealerName ?? "");
                SqlParameter p18 = new SqlParameter("@JenisKendaraan", item.JenisKendaraan ?? "");
                SqlParameter p19 = new SqlParameter("@NoFormA", item.NoFormA ?? "");
                SqlParameter p20 = new SqlParameter("@TglFormA", item.TglFormA ?? Convert.ToDateTime("01-01-1900"));
                SqlParameter p21 = new SqlParameter("@NoKTP", item.NoKTP ?? "");
                SqlParameter p22 = new SqlParameter("@NoTelp", item.NoTelp ?? "");
                SqlParameter p23 = new SqlParameter("@NoHP", item.NoHP ?? "");
                SqlParameter p24 = new SqlParameter("@NoPIB", item.NoPIB ?? "");
                SqlParameter p25 = new SqlParameter("@NoSUT", item.NoSUT ?? "");
                SqlParameter p26 = new SqlParameter("@NoTPT", item.NoTPT ?? "");
                SqlParameter p27 = new SqlParameter("@NoSRUT", item.NoSRUT ?? "");
                SqlParameter p28 = new SqlParameter("@IsActive", 1);
                SqlParameter p29 = new SqlParameter("@CreatedBy", CurrentUser.UserId);
                SqlParameter p30 = new SqlParameter("@CreatedDate", DateTime.Now);
                SqlParameter p31 = new SqlParameter("@LastUpdateBy", CurrentUser.UserId);
                SqlParameter p32 = new SqlParameter("@LastUpdateDate", DateTime.Now);

                try
                {
                    //SqlTransaction tran = new SqlTransaction();
                    //tran.Connection.BeginTransaction();
                    Object[] oPeams = new Object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31, p32 };
                    ctx.Database.ExecuteSqlCommand("EXEC uspfn_omInsertFakturPolda @NoFaktur,@TglFaktur,@Merk,@Tipe,@ThnPembuatan,@ThnPerakitan,@Silinder,@Warna,@NoRangka,@NoMesin,@BahanBakar,@Pemilik,@Pemilik2,@Alamat,@Alamat2,@Alamat3,@DealerCode,@DealerName,@JenisKendaraan,@NoFormA,@TglFormA,@NoKTP,@NoTelp,@NoHP,@NoPIB,@NoSUT,@NoTPT,@NoSRUT,@IsActive,@CreatedBy,@CreatedDate,@LastUpdateBy,@LastUpdateDate", oPeams);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Violation of PRIMARY KEY"))
                    {
                        p0 = new SqlParameter("@NoFaktur", item.NoFaktur ?? "");
                        p1 = new SqlParameter("@TglFaktur", item.TglFaktur ?? Convert.ToDateTime("01-01-1900"));
                        p2 = new SqlParameter("@Merk", item.Merk ?? "");
                        p3 = new SqlParameter("@Tipe", item.Tipe ?? "");
                        p4 = new SqlParameter("@ThnPembuatan", item.ThnPembuatan);
                        p5 = new SqlParameter("@ThnPerakitan", item.ThnPerakitan);
                        p6 = new SqlParameter("@Silinder", item.Silinder ?? "");
                        p7 = new SqlParameter("@Warna", item.Warna ?? "");
                        p8 = new SqlParameter("@NoRangka", item.NoRangka ?? "");
                        p9 = new SqlParameter("@NoMesin", item.NoMesin ?? "");
                        p10 = new SqlParameter("@BahanBakar", item.BahanBakar ?? "");
                        p11 = new SqlParameter("@Pemilik", item.Pemilik ?? "");
                        p12 = new SqlParameter("@Pemilik2", item.Pemilik2 ?? "");
                        p13 = new SqlParameter("@Alamat", item.Alamat ?? "");
                        p14 = new SqlParameter("@Alamat2", item.Alamat2 ?? "");
                        p15 = new SqlParameter("@Alamat3", item.Alamat3 ?? "");
                        p16 = new SqlParameter("@DealerCode", item.DealerCode ?? "");
                        p17 = new SqlParameter("@DealerName", item.DealerName ?? "");
                        p18 = new SqlParameter("@JenisKendaraan", item.JenisKendaraan ?? "");
                        p19 = new SqlParameter("@NoFormA", item.NoFormA ?? "");
                        p20 = new SqlParameter("@TglFormA", item.TglFormA ?? Convert.ToDateTime("01-01-1900"));
                        p21 = new SqlParameter("@NoKTP", item.NoKTP ?? "");
                        p22 = new SqlParameter("@NoTelp", item.NoTelp ?? "");
                        p23 = new SqlParameter("@NoHP", item.NoHP ?? "");
                        p24 = new SqlParameter("@NoPIB", item.NoPIB ?? "");
                        p25 = new SqlParameter("@NoSUT", item.NoSUT ?? "");
                        p26 = new SqlParameter("@NoTPT", item.NoTPT ?? "");
                        p27 = new SqlParameter("@NoSRUT", item.NoSRUT ?? "");
                        p28 = new SqlParameter("@IsActive", 1);
                        p29 = new SqlParameter("@CreatedBy", CurrentUser.UserId);
                        p30 = new SqlParameter("@CreatedDate", DateTime.Now);
                        p31 = new SqlParameter("@LastUpdateBy", CurrentUser.UserId);
                        p32 = new SqlParameter("@LastUpdateDate", DateTime.Now);
                        Object[] oPeams = new Object[] { p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31, p32 };
                        ctx.Database.ExecuteSqlCommand("EXEC uspfn_omUpdateFakturPolda @NoFaktur,@TglFaktur,@Merk,@Tipe,@ThnPembuatan,@ThnPerakitan,@Silinder,@Warna,@NoRangka,@NoMesin,@BahanBakar,@Pemilik,@Pemilik2,@Alamat,@Alamat2,@Alamat3,@DealerCode,@DealerName,@JenisKendaraan,@NoFormA,@TglFormA,@NoKTP,@NoTelp,@NoHP,@NoPIB,@NoSUT,@NoTPT,@NoSRUT,@IsActive,@CreatedBy,@CreatedDate,@LastUpdateBy,@LastUpdateDate", oPeams);
                    }
                    else
                        return Json(new { success = false });
                }

            }

            return Json(new { success = true });
        }

        public JsonResult Upload(HttpPostedFileBase file)
        {
            var message = string.Empty;

            try
            {
                //var fileType = file.FileName.ToLower().EndsWith("xlsx") ? "xlsx" :
                //    file.FileName.ToLower().EndsWith("xls") ? "xls" : string.Empty;

                //if (fileType == "xlsx")
                //{ }
                //else throw new Exception(
                //    "File format is not supported. Please use only XLSX Excel File");

                byte[] rawData = new byte[file.ContentLength];
                file.InputStream.Read(rawData, 0, file.ContentLength);
                var result = OpenXLSX(file.InputStream);
                return Json(new { message = file.FileName, data = result });
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(null);
        }

        private DataTable OpenXLSX(Stream stream)
        {
            var message = String.Empty;
            var user = "anonym";
            if (User.Identity.IsAuthenticated) user = User.Identity.Name;
            DataTable dt = new DataTable("table1");
            
            dt.Columns.Add("NoFaktur");
            dt.Columns.Add("TglFaktur");
            dt.Columns.Add("Merk");
            dt.Columns.Add("Tipe");
            dt.Columns.Add("ThnPembuatan");
            dt.Columns.Add("ThnPerakitan");
            dt.Columns.Add("Silinder");
            dt.Columns.Add("Warna");
            dt.Columns.Add("NoRangka");
            dt.Columns.Add("NoMesin");
            dt.Columns.Add("BahanBakar");
            dt.Columns.Add("Pemilik");
            dt.Columns.Add("Pemilik2");
            dt.Columns.Add("Alamat");
            dt.Columns.Add("Alamat2");
            dt.Columns.Add("Alamat3");
            dt.Columns.Add("DealerCode");
            dt.Columns.Add("DealerName");
            dt.Columns.Add("JenisKendaraan");
            dt.Columns.Add("NoFormA");
            dt.Columns.Add("TglFormA");
            dt.Columns.Add("NoKTP");
            dt.Columns.Add("NoTelp");
            dt.Columns.Add("NoHP");
            dt.Columns.Add("NoPIB");
            dt.Columns.Add("NoSUT");
            dt.Columns.Add("NoTPT");
            dt.Columns.Add("NoSRUT");

            dt.Columns[4].DataType = typeof(Int32);
            dt.Columns[5].DataType = typeof(Int32);

            try
            {
                var workSheet = new ExcelPackage(stream).Workbook.Worksheets.FirstOrDefault();
                if (workSheet == null) throw new Exception(
                    "Tidak ada Worksheet yang terbaca. Harap cek kembali.");

                var cells = workSheet.Cells;

                for (int i = 2; i < workSheet.Dimension.End.Row + 1; i++)
                {
                    dt.Rows.Add(
                        cells[i, 1].Value, 
                        cells[i, 2].Value, 
                        cells[i, 3].Value,
                        cells[i, 4].Value,
                        cells[i, 5].Value,
                        cells[i, 6].Value,
                        cells[i, 7].Value,
                        cells[i, 8].Value,
                        cells[i, 9].Value,
                        cells[i, 10].Value, 
                        cells[i, 11].Value,
                        cells[i, 12].Value,
                        cells[i, 13].Value,
                        cells[i, 14].Value,
                        cells[i, 15].Value,
                        cells[i, 16].Value,
                        cells[i, 17].Value,
                        cells[i, 18].Value, 
                        cells[i, 19].Value,
                        cells[i, 20].Value,
                        cells[i, 21].Value,
                        cells[i, 22].Value,
                        cells[i, 23].Value,
                        cells[i, 24].Value,
                        cells[i, 25].Value,
                        cells[i, 26].Value,
                        cells[i, 27].Value
                        );
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return dt;
        }

    }

}