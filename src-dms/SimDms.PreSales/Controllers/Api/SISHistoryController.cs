using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Transactions;
using SimDms.PreSales.Models;
using SimDms.Common;
using System.IO;
using System.Text;

namespace SimDms.PreSales.Controllers.Api
{
    public class SISHistoryController : BaseController 
    {
        public JsonResult SISHistoryLoad(string DateFrom, string DateTo, string Branch, bool isGM = false)  
        {
            var sessionName = "";
            var rowCount = 0;
            var branchcode = Branch;
            if (isGM){
                branchcode = "";
            }
            var msg = string.Empty;
            string periodStart = Request["DateFrom"];
            string periodEnd = Request["DateTo"];
            string user = CurrentUser.UserId;
            //pgbGenerate.Visible = true;
            int result = 0;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            try
            {
                using (var tranScope = new TransactionScope(TransactionScopeOption.RequiresNew,
                  new TransactionOptions()))
                {
                    var query =string.Format( @"
                                -- updating KDP Log
                                update PmSISHistory set 
                                    Resend          = 1, 
                                    SendSeq         = SendSeq + 1, 
                                    LastUpdateBy    = '{2}', 
                                    LastUpdateDate  = getDate()
                                where 
                                    CompanyCode     = '{0}' 
                                    and BranchCode  = '{1}'
                                    and InquiryNumber in (
	                                                select InquiryNumber
	                                                from PMKDP 
	                                                where 
		                                                convert(varchar, LastUpdateDate, 112) between '{3}' and '{4}'
		                                                and CompanyCode = '{0}' 
                                                        and BranchCode  = '{1}'
                                    );

                                -- Inserting new KDP Log 
                                insert into PmSISHistory (
                                            CompanyCode, BranchCode, 
                                            InquiryNumber, Resend, 
                                            SendSeq, CreatedBy, 
                                            CreatedDate, LastUpdateBy, LastUpdateDate
                                                           )
                                select 
                                    CompanyCode, BranchCode, 
                                    InquiryNumber, 0, 1, '{2}', 
                                    getDate(), '{2}', getDate()
                                from PMKDP 
                                where 
                                    (convert(varchar, LastUpdateDate, 112) between '{3}' and '{4}' )
                                    and CompanyCode = '{0}' 
                                    and BranchCode  = '{1}'
                                    and InquiryNumber in (
	                                        select inq.kdpInq 
                                            from (
                                                select 
                                                    kdp.InquiryNumber kdpInq
                                                    , his.InquiryNumber hisInq
                                                from PmKDP kdp
                                                    left join PmSISHistory his on 
                                                        kdp.CompanyCode     = his.CompanyCode and 
                                                        kdp.BranchCode      = his.BranchCode and 
                                                        kdp.InquiryNumber   = his.InquiryNumber
                                                where 
                                                    kdp.CompanyCode = '{0}' and 
                                                    kdp.BranchCode  = '{1}'
                                                    and convert(varchar, kdp.LastUpdateDate, 112) between '{3}' and '{4}'
	                                        ) inq where inq.hisInq is null
                                    );
                                ", CompanyCode, BranchCode, user, DateFrom, DateTo);
                    //cmd.CommandType = CommandType.Text;
                    //cmd.Parameters.Clear();
                    result = ctx.Database.ExecuteSqlCommand(query);
                    int xx = ctx.Database.ExecuteSqlCommand("select top 1 Companycode from gnMstOrganizationDtl");
                    if (result >= 0) tranScope.Complete();
                } 
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Terjadi Kesalahan Silahkan hubungi SDMS support",
                    error_log = ex.Message
                });
            }
            var PMKDP = ctx.Database.SqlQuery<PmKdforInq>("Select top 100 InquiryNumber from PMKDP").AsQueryable();
            //int result = GenerateSIS(user, periodStart, periodEnd);
            if (result == 0)
            {
                msg = "Data Tidak Ditemukan.";
                return Json(new
                {
                    success = false,
                    message = msg
                });
            }

            if (result > 0)
            {
//                var queryPMKDP = string.Format(@"
//                                select (ROW_NUMBER() OVER(ORDER BY kdp.InquiryNumber)) IncNo 
//                                , kdp.OutletID
//	                            , case sis.Resend when 1 then 'Ya' else 'Tidak' end Resend_1
//	                            , case sis.Resend when 1 then 'Y' else 'N' end Resend_2
//	                            , kdp.InquiryNumber, kdp.InquiryDate
//                                , kdp.TipeKendaraan, kdp.Variant
//	                            , kdp.Transmisi, kdp.ColourCode
//                                , kdp.PerolehanData, kdp.EmployeeID
//	                            , kdp.SpvEmployeeID, SH.SalesHeader, kdp.LastProgress
//                                , case when convert(varchar, kdp.SPKDate, 112) not between '{0}' and '{1}' then Convert(datetime, '19000101') else kdp.SPKDate end SPKDate
//	                            , kdp.CompanyCode, kdp.BranchCode
//	                            , kdp.StatusProspek
//	                            , kdp.NamaProspek, kdp.AlamatProspek, kdp.TelpRumah
//	                            , kdp.CityID, kdp.NamaPerusahaan, kdp.AlamatPerusahaan
//	                            , kdp.Jabatan, kdp.HandPhone, kdp.Faximile, kdp.Email	
//	                            , kdp.CaraPembayaran, kdp.TestDrive, kdp.LostCaseDate
//	                            , kdp.LostCaseCategory, kdp.LostCaseReasonID
//                                , kdp.LostCaseOtherReason, kdp.LostCaseVoiceOfCustomer
//	                            , sis.InquiryNumber	
//                                , sis.SendSeq
//                            from PmKDP kdp
//	                            left join (
//		                            select 
//			                            teamMembers.CompanyCode, teamMembers.BranchCode
//                                        , salesMan.SalesHeader, teamMembers.EmployeeID
//		                            from 
//			                            pmMstTeamMembers teamMembers, (
//				                            select 
//                                                salesManTemp.CompanyCode, salesManTemp.BranchCode
//                                                , salesCO.salesHeader, salesManTemp.EmployeeID
//                                                , salesManTemp.TeamID
//				                            from 
//					                            pmMstTeamMembers salesManTemp, (
//						                            select 
//                                                        salesCOTemp.CompanyCode, salesCOTemp.BranchCode
//                                                        , salesHead.EmployeeID salesHeader, salesCOTemp.EmployeeID
//                                                        , salesCOTemp.TeamID
//						                            from  
//							                            pmMstTeamMembers salesCOTemp, (
//							                                select 
//								                                CompanyCode, BranchCode
//                                                                , EmployeeID, TeamID 
//							                                from 
//                                                                pmMstTeamMembers 
//							                                where 
//								                                CompanyCode = '{2}' and 
//                                                                BranchCode  = '{3}'
//								                                and EmployeeID in (
//									                                select 
//                                                                        EmployeeID 
//                                                                    from 
//                                                                        PmPosition 
//									                                where 
//                                                                        CompanyCode     = '{2}' and 
//                                                                        BranchCode      = '{3}'
//									                                    and PositionID  = 30--@PositionID
//								                                ) and IsSupervisor  = 1
//							                            ) salesHead
//						                            where 
//                                                        salesCOTemp.IsSupervisor    != 1 and 
//                                                        salesCOTemp.CompanyCode     = salesHead.CompanyCode and 
//                                                        salesCOTemp.BranchCode      = salesHead.BranchCode and 
//                                                        salesCOTemp.TeamID          = salesHead.TeamID
//					                            ) salesCO
//				                            where 
//                                                salesCO.CompanyCode         = salesManTemp.CompanyCode and 
//                                                salesCO.BranchCode          = salesManTemp.BranchCode and 
//                                                salesCO.EmployeeID          = salesManTemp.EmployeeID and 
//                                                salesManTemp.IsSupervisor   = 1
//			                            )salesMan
//		                            where 
//                                        salesMan.CompanyCode        = teamMembers.CompanyCode and 
//                                        salesMan.BranchCode         = teamMembers.BranchCode and 
//                                        salesMan.TeamID             = teamMembers.TeamID and 
//                                        teamMembers.IsSupervisor    != 1
//	                            ) SH on 
//                                    kdp.CompanyCode = SH.CompanyCode and 
//                                    kdp.BranchCode  = SH.BranchCode and 
//                                    kdp.EmployeeID  = SH.EmployeeID
//	                            , pmSISHistory sis 
//                            where 
//                                kdp.CompanyCode     = sis.CompanyCode and 
//                                kdp.BranchCode      = sis.BranchCode and 
//                                kdp.InquiryNumber   = sis.InquiryNumber and 
//                                kdp.CompanyCode     = '{2}' and 
//                                kdp.BranchCode      = '{3}' and 
//                                convert(varchar, kdp.LastUpdateDate, 112) between '{0}' and '{1}'
//                                ", DateFrom, DateTo, CompanyCode, BranchCode);
                var queryPMKDP = string.Format(@"
                                select (ROW_NUMBER() OVER(ORDER BY kdp.InquiryNumber)) IncNo 
                                , kdp.OutletID
	                            , case sis.Resend when 1 then 'Ya' else 'Tidak' end Resend_1
	                            , case sis.Resend when 1 then 'Y' else 'N' end Resend_2
	                            , kdp.InquiryNumber, kdp.InquiryDate
                                , kdp.TipeKendaraan, kdp.Variant
	                            , kdp.Transmisi, kdp.ColourCode
                                , kdp.PerolehanData, kdp.EmployeeID
	                            , kdp.SpvEmployeeID, (select EmployeeName from HrEmployee where EmployeeID = kdp.SpvEmployeeID)SalesHeader
								, kdp.LastProgress
                                , case when convert(varchar, kdp.SPKDate, 112) not between '{0}' and '{1}' then Convert(datetime, '19000101') else kdp.SPKDate end SPKDate
	                            , kdp.CompanyCode, kdp.BranchCode
	                            , kdp.StatusProspek
	                            , kdp.NamaProspek, kdp.AlamatProspek, kdp.TelpRumah
	                            , kdp.CityID, kdp.NamaPerusahaan, kdp.AlamatPerusahaan
	                            , kdp.Jabatan, kdp.HandPhone, kdp.Faximile, kdp.Email	
	                            , kdp.CaraPembayaran, kdp.TestDrive, kdp.LostCaseDate
	                            , kdp.LostCaseCategory, kdp.LostCaseReasonID
                                , kdp.LostCaseOtherReason, kdp.LostCaseVoiceOfCustomer
	                            , sis.InquiryNumber	
                                , sis.SendSeq
                            from PmKDP kdp, pmSISHistory sis
							where  kdp.CompanyCode     = sis.CompanyCode and 
                                --kdp.BranchCode      = sis.BranchCode and 
                                kdp.InquiryNumber   = sis.InquiryNumber and 
                                kdp.CompanyCode     = '{2}' and 
                                kdp.BranchCode      = (case when {3} = '' then kdp.BranchCode else '{3}' end ) and 
                                convert(varchar, kdp.LastUpdateDate, 112) between '{0}' and '{1}'"
                    , DateFrom, DateTo, CompanyCode, Branch);
                PMKDP = ctx.Database.SqlQuery<PmKdforInq>(queryPMKDP).AsQueryable();
                string lineDtl1 = string.Empty;
                string lineDtl2 = string.Empty;
                string lineHeader = "H";
                lineHeader += "SITSD".PadRight(5, ' ');;

                string companyName_1 = string.Empty;
                string customerCode = string.Empty;

                 var recDtl = ctx.LookUpDtls.Find(CompanyCode, "SITSD", "1");
                 if (recDtl != null && recDtl.ParaValue == "1")
                 {
                     string cust = ctx.GnMstCoProfileSales.Find(CompanyCode, BranchCode).LockingBy.ToString();
                     if (string.IsNullOrEmpty(cust) || cust == CompanyCode)
                         customerCode = Branch;
                     else
                         customerCode = cust;

                     companyName_1 = ctx.OrganizationDtls.Find(CompanyCode, customerCode).BranchName.ToString();
                 }
                 else
                 {
                     customerCode = CompanyCode;
                     companyName_1 = ctx.OrganizationDtls.Where(p=> p.BranchCode == customerCode).First().BranchName.ToString();
                 }
                 
                 var sb = new StringBuilder();
                 lineHeader += customerCode.PadRight(7, ' ');
                 lineHeader += "1000000".PadRight(7, ' ');
                 lineHeader += companyName_1.PadRight(50, ' ');
                 lineHeader += PMKDP.Count().ToString().PadRight(4, ' ');
                 lineHeader += " ".PadRight(130, ' ');
                 sb.AppendLine(lineHeader);
                 var lengPMKDP = PMKDP.Count().ToString();
                 foreach (var row in PMKDP)
                 {
                     Helpers.ReplaceNullable(row);
                     lineDtl1 = "1";
                     lineDtl1 += row.OutletID.ToString().PadRight(15, ' ');
                     lineDtl1 += row.Resend_2.ToString().PadRight(1, ' ');
                     lineDtl1 += row.InquiryNumber.ToString().PadRight(9, ' ');
                     lineDtl1 += Convert.ToDateTime(row.InquiryDate).ToString("yyyyMMdd").PadRight(8, ' ');
                     lineDtl1 += (!row.TipeKendaraan.ToString().Equals(string.Empty)) ? row.TipeKendaraan.ToString().PadRight(20, ' ') : "NA".PadRight(20, ' ');
                     lineDtl1 += (!row.Variant.ToString().Equals(string.Empty)) ? row.Variant.ToString().PadRight(50, ' ') : "NA".PadRight(50, ' ');
                     lineDtl1 += (!row.Transmisi.ToString().Equals(string.Empty)) ? row.Transmisi.ToString().PadRight(2, ' ') : "NA".PadRight(2, ' ');
                     lineDtl1 += (!row.ColourCode.ToString().Equals(string.Empty)) ? row.ColourCode.ToString().PadRight(3, ' ') : "NA".PadRight(3, ' ');
                     lineDtl1 += row.PerolehanData.ToString().PadRight(15, ' ');
                     lineDtl1 += row.EmployeeID.ToString().PadRight(15, ' ');
                     lineDtl1 += row.SpvEmployeeID.ToString().PadRight(15, ' ');
                     lineDtl1 += row.SalesHeader.ToString().PadRight(15, ' ');
                     lineDtl1 += row.LastProgress.ToString().PadRight(15, ' ');
                     lineDtl1 += Convert.ToDateTime(row.SPKDate).ToString("yyyyMMdd").PadRight(8, ' ');
                     lineDtl1 += " ".PadRight(12, ' ');
                     //lines.Add(lineDtl1);
                     sb.AppendLine(lineDtl1);

                     lineDtl2 = "2";
                     lineDtl2 += row.LostCaseCategory.ToString().PadRight(1, ' ');
                     lineDtl2 += row.LostCaseReasonID.ToString().PadRight(2, ' ');
                     lineDtl2 += row.LostCaseVoiceOfCustomer.ToString().PadRight(200, ' ');
                     //lines.Add(lineDtl2);
                     if (row.IncNo.ToString() != lengPMKDP)
                         sb.AppendLine(lineDtl2);
                     else
                         sb.Append(lineDtl2);
                 }

                 var text = sb.ToString();
                 var content = new byte[text.Length * sizeof(char)];
                 Buffer.BlockCopy(text.ToCharArray(), 0, content, 0, content.Length);
                 sessionName = "downloadHSITSD";
                 TempData.Add(sessionName, content);
                 rowCount = PMKDP.Count();
            }

            return Json(new
            {
                success = true,
                message = msg,
                data = PMKDP,
                sessionName = sessionName,
                rowCount = rowCount
            });
        }

        public FileContentResult DownloadFile(string sessionName)
        {
            var content = TempData[sessionName] as byte[];
            TempData.Clear();
            var stream = new MemoryStream(content);
            var contentType = "application/text";
            Response.Clear();
            Response.ContentType = contentType;
            Response.AddHeader("content-disposition", "attachment;filename=HSITSD.txt");
            Response.Buffer = true;
            stream.WriteTo(Response.OutputStream);
            Response.End();
            return File(content, contentType, "");
        }
    }
}
