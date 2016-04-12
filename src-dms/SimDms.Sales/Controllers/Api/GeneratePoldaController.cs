using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.IO;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using SimDms.Sales.Models;

namespace SimDms.Sales.Controllers.Api
{
    public class GeneratePoldaController : BaseController
    {
        public JsonResult Default()
        {
            return Json(new { CompanyCode = CompanyCode, CompanyName = CompanyName });
        }

        public FileStreamResult Download(string FakturDate)
        {
            using (var package = new ExcelPackage())
            {
                var stream = new MemoryStream();
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                DateTime? TglFaktur = (DateTime)ctx.omFakturPoldas.FirstOrDefault().TglFaktur;//Convert.ToDateTime(FakturDate);

                #region HEADERS
                workSheet.Cells["A1"].Value = "NoFaktur	";
                workSheet.Cells["B1"].Value = "TglFaktur ";
                workSheet.Cells["C1"].Value = "Merk";
                workSheet.Cells["D1"].Value = "Tipe";
                workSheet.Cells["E1"].Value = "ThnPembuatan";
                workSheet.Cells["F1"].Value = "ThnPerakitan";
                workSheet.Cells["G1"].Value = "Silinder";
                workSheet.Cells["H1"].Value = "Warna";
                workSheet.Cells["I1"].Value = "NoRangka";
                workSheet.Cells["J1"].Value = "NoMesin";
                workSheet.Cells["K1"].Value = "BahanBakar";
                workSheet.Cells["L1"].Value = "Pemilik";
                workSheet.Cells["M1"].Value = "Pemilik2";
                workSheet.Cells["N1"].Value = "Alamat";
                workSheet.Cells["O1"].Value = "Alamat2";
                workSheet.Cells["P1"].Value = "Alamat3";
                workSheet.Cells["Q1"].Value = "DealerCode";
                workSheet.Cells["R1"].Value = "DealerName";
                workSheet.Cells["S1"].Value = "JenisKendaraan";
                workSheet.Cells["T1"].Value = "NoFormA";
                workSheet.Cells["U1"].Value = "TglFormA";
                workSheet.Cells["V1"].Value = "NoKTP";
                workSheet.Cells["W1"].Value = "NoTelp";
                workSheet.Cells["X1"].Value = "NoHP";
                workSheet.Cells["Y1"].Value = "NoPIB";
                workSheet.Cells["Z1"].Value = "NoSUT";
                workSheet.Cells["AA1"].Value = "NoTPT";
                workSheet.Cells["AB1"].Value = "NoSRUT";
                #endregion

                #region ROWS
                int rownum = 2;
                foreach (var item in ctx.omFakturPoldas.Where(x=>x.DealerCode == CompanyCode && x.TglFaktur == (DateTime)TglFaktur))
                {
                    workSheet = FillExcel(workSheet, rownum.ToString(), item);
                    rownum++;
                }
                #endregion

                package.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "CDPB [Tanggal].xlsx");
            }
        }

        public ExcelWorksheet FillExcel(ExcelWorksheet ws, string rownum, omFakturPolda model)
        {
            ws.Cells["A" + rownum].Value = model.NoFaktur;
            ws.Cells["B" + rownum].Value = model.TglFaktur.Value.ToString("dd/MM/yyyy");
            ws.Cells["C" + rownum].Value = model.Merk;
            ws.Cells["D" + rownum].Value = model.Tipe;
            ws.Cells["E" + rownum].Value = model.ThnPembuatan;
            ws.Cells["F" + rownum].Value = model.ThnPerakitan;
            ws.Cells["G" + rownum].Value = model.Silinder;
            ws.Cells["H" + rownum].Value = model.Warna;
            ws.Cells["I" + rownum].Value = model.NoRangka;
            ws.Cells["J" + rownum].Value = model.NoMesin;
            ws.Cells["K" + rownum].Value = model.BahanBakar;
            ws.Cells["L" + rownum].Value = model.Pemilik;
            ws.Cells["M" + rownum].Value = model.Pemilik2;
            ws.Cells["N" + rownum].Value = model.Alamat;
            ws.Cells["O" + rownum].Value = model.Alamat2;
            ws.Cells["P" + rownum].Value = model.Alamat3;
            ws.Cells["Q" + rownum].Value = model.DealerCode;
            ws.Cells["R" + rownum].Value = model.DealerName;
            ws.Cells["S" + rownum].Value = model.JenisKendaraan;
            ws.Cells["T" + rownum].Value = model.NoFormA;
            ws.Cells["U" + rownum].Value = (model.TglFormA.HasValue) ? model.TglFormA.Value.ToString("dd/MM/yyyy") : "";
            ws.Cells["V" + rownum].Value = model.NoKTP;
            ws.Cells["W" + rownum].Value = model.NoTelp;
            ws.Cells["X" + rownum].Value = model.NoHP;
            ws.Cells["Y" + rownum].Value = model.NoPIB;
            ws.Cells["Z" + rownum].Value = model.NoSUT;
            ws.Cells["AA" + rownum].Value = model.NoTPT;
            ws.Cells["AB" + rownum].Value = model.NoSRUT;

            return ws;
        }
    }
}