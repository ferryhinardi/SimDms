using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SimDms.PreSales.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class UploadFileController : BaseController
    {
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file, string uploadType)
        {
            var message = "";
            IQueryable<UploadFile> result = null;
            try
            {
                var text = "";
                if (file != null)
                {
                    var rawData = new byte[file.ContentLength];
                    file.InputStream.Read(rawData, 0, file.ContentLength);
                    text = Encoding.UTF8.GetString(rawData);
                    if (!text.StartsWith("HHITSO")) throw new Exception("File tidak valid");

                    var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    var hitso = new HITSO(lines);
                    message = UploadKdp(hitso, ref result);



                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Json(new { message = message, fileName = file.FileName, data = result });
        }

        private string UploadKdp(HITSO hitso, ref IQueryable<UploadFile> result)
        {
            var message = "";
            try
            {
                #region ~~ Queries ~~
                var queryPrepare = @"
select * into #PmKDPTemp from PmKDP where 1 = 2
select * into #PmActivitiesTemp from PmActivities where 1 = 2
select * into #PmStatusHistoryTemp from PmStatusHistory where 1 = 2";

                var queryInsKDP = @"
insert into #PmKDPTemp (InquiryNumber, CompanyCode, BranchCode, EmployeeID, SpvemployeeID
, InquiryDate, OutletID, StatusProspek, PerolehanData, NamaProspek, AlamatProspek, TelpRumah
, CityID, NamaPerusahaan, AlamatPerusahaan, Jabatan, HandPhone, Faximile, Email, TipeKendaraan
, Variant, Transmisi, ColourCode, CaraPembayaran, TestDrive, QuantityInquiry, LastProgress
, LastUpdateStatus, SPKDate, LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason
, LostCaseVoiceOfCustomer, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate) 
values('{0}','{1}','{2}','{3}','{4}',convert(datetime,'{5}'),'{6}','{7}','{8}','{9}','{10}','{11}'
,'{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}'
,'{25}','{26}',convert(datetime,'{27}'),convert(datetime,'{28}'),convert(datetime,'{29}')
,'{30}','{31}','{32}','{33}',convert(datetime,'{34}'),'{35}','{36}',convert(datetime,'{37}'))";

                var queryInsHst = @"
insert into #PmStatusHistoryTemp (InquiryNumber,CompanyCode,BranchCode,SequenceNo,LastProgress
,UpdateDate,UpdateUser)
values ('{0}','{1}','{2}','{3}','{4}',convert(datetime,'{5}'),'{6}')";

                var queryInsAct = @"
insert into #PmActivitiesTemp (CompanyCode,BranchCode,InquiryNumber,ActivityID,ActivityDate
, ActivityType,ActivityDetail,NextFollowUpDate,CreationDate,CreatedBy,LastUpdateBy,LastUpdateDate)
values ('{0}','{1}','{2}','{3}',convert(datetime,'{4}'),'{5}'
,'{6}',convert(datetime,'{7}'),convert(datetime,'{8}'),'{9}','{10}',convert(datetime,'{11}'))"; 
                #endregion

                ctx.Database.ExecuteSqlCommand(queryPrepare);

                var CONST_EXEC = 50;
                var incExec = 0;
                var recentExec = 1;  
                var query = "";
                for (int i = 0; i < hitso.DetailPmKDP.Count; i++)
                {
                    query += string.Format(queryInsKDP, hitso.DetailPmKDP[i].InquiryNumber
                        , hitso.DetailPmKDP[i].CompanyCode, hitso.DetailPmKDP[i].BranchCode
                        , hitso.DetailPmKDP[i].EmployeeID, hitso.DetailPmKDP[i].SpvemployeeID
                        , hitso.DetailPmKDP[i].InquiryDate.ToString("yyyyMMdd")
                        , hitso.DetailPmKDP[i].OutletID, hitso.DetailPmKDP[i].StatusProspek
                        , hitso.DetailPmKDP[i].PerolehanData, hitso.DetailPmKDP[i].NamaProspek
                        , hitso.DetailPmKDP[i].AlamatProspek, hitso.DetailPmKDP[i].TelpRumah
                        , hitso.DetailPmKDP[i].CityID, hitso.DetailPmKDP[i].NamaPerusahaan
                        , hitso.DetailPmKDP[i].AlamatPerusahaan, hitso.DetailPmKDP[i].Jabatan
                        , hitso.DetailPmKDP[i].HandPhone, hitso.DetailPmKDP[i].Faximile
                        , hitso.DetailPmKDP[i].Email, hitso.DetailPmKDP[i].TipeKendaraan
                        , hitso.DetailPmKDP[i].Variant, hitso.DetailPmKDP[i].Transmisi
                        , hitso.DetailPmKDP[i].ColourCode, hitso.DetailPmKDP[i].CaraPembayaran
                        , hitso.DetailPmKDP[i].TestDrive, hitso.DetailPmKDP[i].QuantityInquiry.ToString()
                        , hitso.DetailPmKDP[i].LastProgress
                        , hitso.DetailPmKDP[i].LastUpdateStatus.ToString("yyyyMMdd")
                        , hitso.DetailPmKDP[i].SPKDate.ToString("yyyyMMdd")
                        , hitso.DetailPmKDP[i].LostCaseDate.ToString("yyyyMMdd")
                        , hitso.DetailPmKDP[i].LostCaseCategory, hitso.DetailPmKDP[i].LostCaseReasonID
                        , hitso.DetailPmKDP[i].LostCaseOtherReason, hitso.DetailPmKDP[i].LostCaseVoiceOfCustomer
                        , hitso.DetailPmKDP[i].CreationDate.ToString("yyyyMMdd")
                        , hitso.DetailPmKDP[i].CreatedBy, hitso.DetailPmKDP[i].LastUpdateBy
                        , hitso.DetailPmKDP[i].LastUpdateDate.ToString("yyyyMMdd"));

                    for (int j = 0; j < hitso.DetailPmKDP[i].DetailStatusHistories.Count; j++)
                    {
                        query += string.Format(queryInsHst
                            , hitso.DetailPmKDP[i].DetailStatusHistories[j].InquiryNumber
                            , hitso.DetailPmKDP[i].DetailStatusHistories[j].CompanyCode
                            , hitso.DetailPmKDP[i].DetailStatusHistories[j].BranchCode
                            , hitso.DetailPmKDP[i].DetailStatusHistories[j].SequenceNo
                            , hitso.DetailPmKDP[i].DetailStatusHistories[j].LastProgress
                            , hitso.DetailPmKDP[i].DetailStatusHistories[j].UpdateDate.ToString("yyyyMMdd")
                            , hitso.DetailPmKDP[i].DetailStatusHistories[j].UpdateUser);
                    }

                    for (int k = 0; k < hitso.DetailPmKDP[i].DetailActivities.Count; k++)
                    {
                        query += string.Format(queryInsAct
                            , hitso.DetailPmKDP[i].DetailActivities[k].CompanyCode
                            , hitso.DetailPmKDP[i].DetailActivities[k].BranchCode
                            , hitso.DetailPmKDP[i].DetailActivities[k].InquiryNumber
                            , hitso.DetailPmKDP[i].DetailActivities[k].ActivityID
                            , hitso.DetailPmKDP[i].DetailActivities[k].ActivityDate.ToString("yyyyMMdd")
                            , hitso.DetailPmKDP[i].DetailActivities[k].ActivityType
                            , hitso.DetailPmKDP[i].DetailActivities[k].ActivityDetail
                            , hitso.DetailPmKDP[i].DetailActivities[k].NextFollowUpDate.ToString("yyyyMMdd")
                            , hitso.DetailPmKDP[i].DetailActivities[k].CreationDate.ToString("yyyyMMdd")
                            , hitso.DetailPmKDP[i].DetailActivities[k].CreatedBy
                            , hitso.DetailPmKDP[i].DetailActivities[k].LastUpdateBy
                            , hitso.DetailPmKDP[i].DetailActivities[k].LastUpdateDate.ToString("yyyyMMdd"));
                    }

                    if (incExec == CONST_EXEC)
                    {
                        ctx.Database.ExecuteSqlCommand(query);
                        query = "";
                        recentExec++;
                        incExec = 0;
                    }
                    else incExec++;
                }
                
                if (!query.Equals(string.Empty))
                {
                    ctx.Database.ExecuteSqlCommand(query);
                }

                #region ~~ Query GenerateKDP ~~
                var queryGenerateKDP = @"
-- UPDATE KDP
-- create preparation table to update
select * into #KDPHistUpdateTemp  from (select 
	dptHist.NewInquiryNumber, 'Repeated' [Status], kdpTemp.* 
from 
	#PmKDPTemp kdpTemp, PmDPTHistory dptHist
where
	kdpTemp.CompanyCode = dptHist.CompanyCode
	and kdpTemp.BranchCode = dptHist.BranchCode
	and kdpTemp.OutletID = dptHist.OutletID
	and kdpTemp.InquiryNumber = dptHist.InquiryNumber
)#KDPHistUpdateTemp

delete from PmActivities 
where InquiryNumber IN (
	select NewInquiryNumber from #KDPHistUpdateTemp
)

delete from PmStatusHistory
where inquiryNumber in (
	select NewInquiryNumber from #KDPHistUpdateTemp
)

delete from PmKDP 
where InquiryNumber in (
	select NewInquiryNumber from #KDPHistUpdateTemp
)

insert into PmKDP (InquiryNumber, CompanyCode, BranchCode
, EmployeeID, SpvemployeeID, InquiryDate, OutletID, StatusProspek
, PerolehanData, NamaProspek, AlamatProspek, TelpRumah, CityID
, NamaPerusahaan, AlamatPerusahaan, Jabatan, HandPhone, Faximile
, Email, TipeKendaraan, Variant, Transmisi, ColourCode, CaraPembayaran
, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus, SPKDate
, LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason
, LostCaseVoiceOfCustomer, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate)
select 	
	kdpHistUpdTemp.NewInquiryNumber, kdpHistUpdTemp.CompanyCode, kdpHistUpdTemp.BranchCode, kdpHistUpdTemp.EmployeeID
	, kdpHistUpdTemp.SpvemployeeID, kdpHistUpdTemp.InquiryDate, kdpHistUpdTemp.OutletID, kdpHistUpdTemp.StatusProspek
	, kdpHistUpdTemp.PerolehanData, kdpHistUpdTemp.NamaProspek, kdpHistUpdTemp.AlamatProspek, kdpHistUpdTemp.TelpRumah
	, kdpHistUpdTemp.CityID, kdpHistUpdTemp.NamaPerusahaan, kdpHistUpdTemp.AlamatPerusahaan, kdpHistUpdTemp.Jabatan
	, kdpHistUpdTemp.HandPhone, kdpHistUpdTemp.Faximile, kdpHistUpdTemp.Email, kdpHistUpdTemp.TipeKendaraan
	, kdpHistUpdTemp.Variant, kdpHistUpdTemp.Transmisi, kdpHistUpdTemp.ColourCode, kdpHistUpdTemp.CaraPembayaran
	, kdpHistUpdTemp.TestDrive, kdpHistUpdTemp.QuantityInquiry, kdpHistUpdTemp.LastProgress, kdpHistUpdTemp.LastUpdateStatus
	, kdpHistUpdTemp.SPKDate, kdpHistUpdTemp.LostCaseDate, kdpHistUpdTemp.LostCaseCategory, kdpHistUpdTemp.LostCaseReasonID
	, kdpHistUpdTemp.LostCaseOtherReason, kdpHistUpdTemp.LostCaseVoiceOfCustomer, kdpHistUpdTemp.CreationDate
	, kdpHistUpdTemp.CreatedBy, kdpHistUpdTemp.LastUpdateBy, kdpHistUpdTemp.LastUpdateDate
from 
	#KDPHistUpdateTemp kdpHistUpdTemp

insert into PmActivities (CompanyCode, BranchCode, InquiryNumber
, ActivityID, ActivityDate, ActivityType, ActivityDetail, NextFollowUpDate
, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate)
select 
	actTemp.CompanyCode, actTemp.BranchCode, kdpHistUpdTemp.NewInquiryNumber, actTemp.ActivityID, actTemp.ActivityDate
	, actTemp.ActivityType, actTemp.ActivityDetail, actTemp.NextFollowUpDate, actTemp.CreationDate
	, actTemp.CreatedBy, actTemp.LastUpdateBy, actTemp.LastUpdateDate
from 
    #PmActivitiesTemp actTemp,  #KDPHistUpdateTemp kdpHistUpdTemp
where 
	actTemp.CompanyCode = kdpHistUpdTemp.CompanyCode
	and actTemp.BranchCode = kdpHistUpdTemp.BranchCode
	and actTemp.InquiryNumber = kdpHistUpdTemp.InquiryNumber

insert into PmStatusHistory (InquiryNumber, CompanyCode
, BranchCode, SequenceNo, LastProgress, UpdateDate, UpdateUser)
select 
	kdpHistUpdTemp.NewInquiryNumber, histTemp.CompanyCode, histTemp.BranchCode, histTemp.SequenceNo, histTemp.LastProgress
	, histTemp.UpdateDate, histTemp.UpdateUser
from 
    #PmStatusHistoryTemp histTemp, #KDPHistUpdateTemp kdpHistUpdTemp
where 
	histTemp.CompanyCode = kdpHistUpdTemp.CompanyCode
	and histTemp.BranchCode = kdpHistUpdTemp.BranchCode
	and histTemp.InquiryNumber = kdpHistUpdTemp.InquiryNumber

update PmDPTHistory set LastUpdateBy = @UserID, LastUpdateDate = GetDate()
where CompanyCode in (select CompanyCode from #KDPHistUpdateTemp)
	and BranchCode in (select BranchCode from #KDPHistUpdateTemp)
	and OutletID in (select OutletID from #KDPHistUpdateTemp)
	and InquiryNumber In (select InquiryNumber from #KDPHistUpdateTemp)

-- INSERT NEW KDP --
-- create preparation table to insert
select * into #KDPHistInsertTemp  from (select 
	(select isnull(max(InquiryNumber), 0) from PmKDP) + ROW_NUMBER() OVER(ORDER BY KDPHistTemp.InquiryNumber) NewInquiryNumber
	, 'Success' [Status], KDPHistTemp.* 
from (
	select * from #PmKDPTemp
	except
	select 
		kdpTemp.* 
	from 
        #PmKDPTemp kdpTemp, PmDPTHistory dptHist
	where
		kdpTemp.CompanyCode = dptHist.CompanyCode
		and kdpTemp.BranchCode = dptHist.BranchCode
		and kdpTemp.OutletID = dptHist.OutletID
		and kdpTemp.InquiryNumber = dptHist.InquiryNumber
) KDPHistTemp)#KDPHistInsertTemp 

-- insert KDP
insert into PmKDP (InquiryNumber, CompanyCode, BranchCode
, EmployeeID, SpvemployeeID, InquiryDate, OutletID, StatusProspek
, PerolehanData, NamaProspek, AlamatProspek, TelpRumah, CityID
, NamaPerusahaan, AlamatPerusahaan, Jabatan, HandPhone, Faximile
, Email, TipeKendaraan, Variant, Transmisi, ColourCode, CaraPembayaran
, TestDrive, QuantityInquiry, LastProgress, LastUpdateStatus, SPKDate
, LostCaseDate, LostCaseCategory, LostCaseReasonID, LostCaseOtherReason
, LostCaseVoiceOfCustomer, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate)
select 
	NewInquiryNumber, CompanyCode, BranchCode, EmployeeID, SpvemployeeID, InquiryDate, OutletID, StatusProspek
	, PerolehanData, NamaProspek, AlamatProspek, TelpRumah, CityID, NamaPerusahaan, AlamatPerusahaan, Jabatan
	, HandPhone, Faximile, Email, TipeKendaraan, Variant, Transmisi, ColourCode, CaraPembayaran, TestDrive
	, QuantityInquiry, LastProgress, LastUpdateStatus, SPKDate, LostCaseDate, LostCaseCategory, LostCaseReasonID
	, LostCaseOtherReason, LostCaseVoiceOfCustomer, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate
from #KDPHistInsertTemp 

-- insert PmActivities
insert into PmActivities (CompanyCode, BranchCode, InquiryNumber
, ActivityID, ActivityDate, ActivityType, ActivityDetail, NextFollowUpDate
, CreationDate, CreatedBy, LastUpdateBy, LastUpdateDate)
select actTemp.CompanyCode, actTemp.BranchCode, kdpHistInsTemp.NewInquiryNumber
, actTemp.ActivityID, actTemp.ActivityDate, actTemp.ActivityType, actTemp.ActivityDetail
, actTemp.NextFollowUpDate, actTemp.CreationDate, actTemp.CreatedBy, actTemp.LastUpdateBy
, actTemp.LastUpdateDate
from #PmActivitiesTemp actTemp, #KDPHistInsertTemp kdpHistInsTemp
where 
	actTemp.CompanyCode = kdpHistInsTemp.CompanyCode
	and actTemp.BranchCode = kdpHistInsTemp.BranchCode
	and actTemp.InquiryNumber = kdpHistInsTemp.InquiryNumber

-- insert PmStatusHistory
insert into PmStatusHistory (InquiryNumber, CompanyCode
, BranchCode, SequenceNo, LastProgress, UpdateDate, UpdateUser)
select 
	kdpHistInsTemp.NewInquiryNumber, histTemp.CompanyCode, histTemp.BranchCode, histTemp.SequenceNo
	, histTemp.LastProgress, histTemp.UpdateDate, histTemp.UpdateUser
from #PmStatusHistoryTemp histTemp, #KDPHistInsertTemp kdpHistInsTemp
where
	histTemp.CompanyCode = kdpHistInsTemp.CompanyCode
	and histTemp.BranchCode = kdpHistInsTemp.BranchCode
	and histTemp.InquiryNumber = kdpHistInsTemp.InquiryNumber

-- insert PmDPTHistory
insert into PmDPTHistory (CompanyCode, BranchCode, InquiryNumber, OutletID
, NewInquiryNumber, CreatedBy, CreatedDate, LastUpdateBy, LastUpdateDate)
select 
	kdpHistInsTemp.CompanyCode, kdpHistInsTemp.BranchCode, kdpHistInsTemp.InquiryNumber, kdpHistInsTemp.OutletID
	, kdpHistInsTemp.NewInquiryNumber, @UserID, GetDate(), @UserID, GetDate()
from 
	#KDPHistInsertTemp kdpHistInsTemp

select ROW_NUMBER() OVER(ORDER BY InquiryNumber) SeqNo, InquiryNumber, NewInquiryNumber, Status from (
select * from #KDPHistUpdateTemp
union
select * from #KDPHistInsertTemp
) dptHist

drop table #KDPHistUpdateTemp
drop table #KDPHistInsertTemp
drop table #PmKDPTemp
drop table #PmActivitiesTemp
drop table #PmStatusHistoryTemp
";
                #endregion

                result = ctx.Database.SqlQuery<UploadFile>(queryGenerateKDP, CurrentUser.UserId).AsQueryable();

            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return message;
        }
    }
}