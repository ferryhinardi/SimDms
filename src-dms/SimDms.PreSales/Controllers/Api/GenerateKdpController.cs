using SimDms.PreSales.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SimDms.PreSales.Controllers.Api
{
    public class GenerateKdpController : BaseController
    {
        public JsonResult GenerateKdp(DateTime dateFrom, DateTime dateTo)
        {
            var message = "";
            var sessionName = "";
            var rowCount = 0;
            
            try
            {
                var query1 = @"
select * from PmKDP where Convert(varchar, InquiryDate, 112) between @p0 and @p1
";
                var query2 = @"
select * from PmStatusHistory where InquiryNumber in (select InquiryNumber from PmKDP where Convert(varchar, InquiryDate, 112) between @p0 and @p1)
";
                var query3 = @"
select * from PmActivities where InquiryNumber in (select InquiryNumber from PmKDP where Convert(varchar, InquiryDate, 112) between @p0 and @p1)
";

                var tableKdp = ctx.Database.SqlQuery<PmKdpItem>(query1, dateFrom, dateTo).ToList();
                var tableHist = ctx.Database.SqlQuery<PmStatusHistItem>(query2, dateFrom, dateTo).ToList();
                var tableActivity = ctx.Database.SqlQuery<PmActivitiesItem>(query3, dateFrom, dateTo).ToList();
                
                if (tableKdp.Count() == 0) throw new Exception("Data tidak ditemukan");

                var lines = new List<string>();
                var lineHeader = "H".PadRight(1, ' ');
                lineHeader += "HITSO".PadRight(5, ' ');
                lineHeader += CompanyCode.PadRight(15, ' ');
                lineHeader += BranchCode.PadRight(15, ' ');
                lineHeader += string.Empty.PadRight(1164, ' ');
                lines.Add(lineHeader);

                var lineDtl = "";
                foreach (var row in tableKdp)
                {
                    lineDtl = "1";
                    lineDtl += row.InquiryNumber.ToString().PadRight(9, ' ');
                    lineDtl += row.CompanyCode.PadRight(15, ' ');
                    lineDtl += row.BranchCode.PadRight(15, ' ');
                    lineDtl += row.EmployeeID.PadRight(15, ' ');
                    lineDtl += row.SpvEmployeeID.PadRight(15, ' ');
                    lineDtl += (!row.InquiryDate.ToString().Equals(string.Empty) ? row.InquiryDate.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                    lineDtl += row.OutletID.PadRight(15, ' ');
                    lineDtl += row.StatusProspek.PadRight(2, ' ');
                    lineDtl += row.PerolehanData.PadRight(15, ' ');
                    lineDtl += row.NamaProspek.PadRight(30, ' ');
                    lineDtl += row.AlamatProspek.Replace(Environment.NewLine, " ").PadRight(200, ' ');
                    lineDtl += row.TelpRumah.ToString().PadRight(15, ' ');
                    lineDtl += row.CityID.PadRight(15, ' ');
                    lineDtl += row.NamaPerusahaan.PadRight(50, ' ');
                    lineDtl += row.AlamatPerusahaan.Replace(Environment.NewLine, " ").PadRight(200, ' ');
                    lineDtl += row.Jabatan.PadRight(30, ' ');
                    lineDtl += row.Handphone.PadRight(14, ' ');
                    lineDtl += row.Faximile.PadRight(15, ' ');
                    lineDtl += row.Email.PadRight(50, ' ');
                    lineDtl += row.TipeKendaraan.PadRight(20, ' ');
                    lineDtl += row.Variant.PadRight(50, ' ');
                    lineDtl += row.Transmisi.PadRight(2, ' ');
                    lineDtl += row.ColourCode.PadRight(3, ' ');
                    lineDtl += row.CaraPembayaran.PadRight(2, ' ');
                    lineDtl += row.TestDrive.PadRight(2, ' ');
                    lineDtl += row.QuantityInquiry.ToString().PadRight(4, ' ');
                    lineDtl += row.LastProgress.PadRight(15, ' ');
                    lineDtl += (row.LastUpdateStatus != null ? row.LastUpdateStatus.Value.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                    lineDtl += (row.SPKDate != null ? row.SPKDate.Value.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                    lineDtl += (row.LostCaseDate != null ? row.LostCaseDate.Value.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                    lineDtl += row.LostCaseCategory.PadRight(1, ' ');
                    lineDtl += row.LostCaseReasonID.PadRight(2, ' ');
                    lineDtl += row.LostCaseOtherReason.Replace(Environment.NewLine, " ").PadRight(100, ' ');
                    lineDtl += row.LostCaseVoiceOfCustomer.Replace(Environment.NewLine, " ").PadRight(200, ' ');
                    lineDtl += (row.CreationDate != null ? row.CreationDate.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                    lineDtl += row.CreatedBy.PadRight(15, ' ');
                    lineDtl += row.LastUpdateBy.PadRight(15, ' ');
                    lineDtl += (row.LastUpdateDate != null ? row.LastUpdateDate.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                    lines.Add(lineDtl);

                    var historyRows = tableHist.Where(x => x.InquiryNumber == row.InquiryNumber).ToList();
                    if (historyRows.Count() > 0)
                    {
                        foreach (var histRow in historyRows)
                        {
                            lineDtl = "2".PadRight(1, ' ');
                            lineDtl += histRow.InquiryNumber.ToString().PadRight(9, ' ');
                            lineDtl += histRow.CompanyCode.PadRight(15, ' ');
                            lineDtl += histRow.BranchCode.PadRight(15, ' ');
                            lineDtl += histRow.SequenceNo.ToString().PadRight(9, ' ');
                            lineDtl += histRow.LastProgress.PadRight(15, ' ');
                            lineDtl += (histRow.UpdateDate != null  ? histRow.UpdateDate.Value.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                            lineDtl += histRow.UpdateUser.PadRight(15, ' ');
                            lineDtl += " ".PadRight(1113, ' ');
                            lines.Add(lineDtl);
                        }
                    }

                    var actRows = tableActivity.Where(x => x.InquiryNumber == row.InquiryNumber).ToList();
                    if (actRows.Count() > 0)
                    {
                        foreach (var actRow in actRows)
                        {
                            lineDtl = "3".PadRight(1, ' ');
                            lineDtl += actRow.CompanyCode.PadRight(15, ' ');
                            lineDtl += actRow.BranchCode.PadRight(15, ' ');
                            lineDtl += actRow.InquiryNumber.ToString().PadRight(9, ' ');
                            lineDtl += actRow.ActivityID.ToString().PadRight(9, ' ');
                            lineDtl += (actRow.ActivityDate != null ? actRow.ActivityDate.Value.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                            lineDtl += actRow.ActivityType.PadRight(10, ' ');
                            lineDtl += actRow.ActivityDetail.Replace(Environment.NewLine, " ").PadRight(200, ' ');
                            lineDtl += (actRow.NextFollowUpDate != null  ? actRow.NextFollowUpDate.Value.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                            lineDtl += (actRow.CreationDate != null  ? actRow.CreationDate.Value.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                            lineDtl += actRow.CreatedBy.PadRight(15, ' ');
                            lineDtl += actRow.LastUpdateBy.PadRight(15, ' ');
                            lineDtl += (actRow.LastUpdateDate != null ? actRow.LastUpdateDate.Value.ToString("yyyyMMdd").PadRight(8, ' ') : "19000101".PadRight(8, ' '));
                            lineDtl += " ".PadRight(879, ' ');
                            lines.Add(lineDtl);
                        }
                    }
                }

                var sb = new StringBuilder();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (i == lines.Count - 1) sb.Append(lines[i]);
                    else sb.AppendLine(lines[i]);
                }

                var text = sb.ToString();
                var content = new byte[text.Length * sizeof(char)];
                Buffer.BlockCopy(text.ToCharArray(), 0, content, 0, content.Length);
                sessionName = "downloadGenKdp";
                TempData.Add(sessionName, content);
                rowCount = tableKdp.Count();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { message = message, sessionName = sessionName, rowCount = rowCount });
        }

        public FileContentResult DownloadFile(string sessionName)
        {
            var content = TempData[sessionName] as byte[];
            TempData.Clear();
            var stream = new MemoryStream(content);
            var contentType = "application/text";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "attachment;filename=KDP.txt");
            Response.Buffer = true;
            stream.WriteTo(Response.OutputStream);
            Response.End();
            return File(content, contentType, "");
        }
    }
}