var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var config = require('../config');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "GENERATE_SP_PMHSTITS";
var TaskNo = "20140930001";
var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQLCheck = hereDoc(function(){/*!
declare @column_list varchar(MAX)
SELECT @column_list = COALESCE(@column_list + ', ', '') + COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
SELECT @column_list list, COUNT(*) total
FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
*/});

var SQL = hereDoc(function(){/*!
if object_id('uspfn_SyncPmHstIts') is not null
	drop procedure uspfn_SyncPmHstIts
GO
select 'Create SP uspfn_SyncPmHstIts' info
GO
CREATE procedure [dbo].[uspfn_SyncPmHstIts]
as
declare @CompanyCode varchar(max)
set @CompanyCode = (select top 1 CompanyCode from gnMstOrganizationDtl)

declare @BranchCode varchar(max)
set @BranchCode = (select top 1 BranchCode from gnMstOrganizationDtl where IsBranch = 0)

declare @DealerCode varchar(max)
set @DealerCode  = isnull((select LockingBy from gnMstCoProfileSales where CompanyCode = @CompanyCode and BranchCode = @BranchCode), @CompanyCode)

declare @DealerName varchar(max)
set @DealerName = isnull((select top 1 CompanyName from gnmstorganizationhdr where companycode=@companyCode),
					(select isnull(BranchName,' ') from gnMstOrganizationDtl where companycode=@companycode and branchcode=@dealercode))

declare @LastDate datetime
declare @NoInquiry int

select * into #t2
from (
select pp.CompanyCode, pp.BranchCode, pp.EmployeeID BranchHeadID, ge.EmployeeName BranchHeadName, pt.TeamID
  from pmPosition pp
  left join gnMstEmployee ge
	on pp.CompanyCode = ge.CompanyCode
   and pp.BranchCode = ge.BranchCode
   and pp.EmployeeID = ge.EmployeeID
   and ge.PersonnelStatus = 1
  left join pmMstTeamMembers pt
	on pp.CompanyCode = pt.CompanyCode
   and pp.BranchCode = pt.BranchCode
   and pp.EmployeeID = pt.EmployeeID
   and pt.IsSupervisor = 1
 where pp.PositionId = '40'
) #t2

select * into #t3
from (
select pp.CompanyCode, pp.BranchCode, pp.BranchHeadID, pp.BranchHeadName,
	   pt.EmployeeID SalesHeadID, ge.EmployeeName SalesHeadName, pd.TeamID
  from #t2 pp
  left join pmMstTeamMembers pt
	on pp.CompanyCode=pt.CompanyCode
   and pp.BranchCode=pt.BranchCode
   and pp.TeamID=pt.TeamID
   and pt.IsSupervisor=0
  left join pmMstTeamMembers pd
	on pp.CompanyCode = pd.CompanyCode
   and pp.BranchCode = pd.BranchCode
   and pt.EmployeeID = pd.EmployeeID
   and pd.IsSupervisor = 1
  left join gnMstEmployee ge
	on pp.CompanyCode = ge.CompanyCode
   and pp.BranchCode = ge.BranchCode
   and pt.EmployeeID = ge.EmployeeID
   and ge.PersonnelStatus = 1
) #t3


select * into #t4
from (
select pp.CompanyCode, pp.BranchCode, pp.BranchHeadID, pp.BranchHeadName, pp.SalesHeadID, pp.SalesHeadName, 
	   pt.EmployeeID SalesCoordinatorID, ge.EmployeeName SalesCoordinatorName, pd.TeamID
  from #t3 pp
  left join pmMstTeamMembers pt
	on pp.CompanyCode=pt.CompanyCode
   and pp.BranchCode=pt.BranchCode
   and pp.TeamID=pt.TeamID
   and pt.IsSupervisor=0
  left join pmMstTeamMembers pd
	on pp.CompanyCode = pd.CompanyCode
   and pp.BranchCode = pd.BranchCode
   and pt.EmployeeID = pd.EmployeeID
   and pd.IsSupervisor = 1
  left join gnMstEmployee ge
	on pp.CompanyCode = ge.CompanyCode
   and pp.BranchCode = ge.BranchCode
   and pt.EmployeeID = ge.EmployeeID
   and ge.PersonnelStatus = 1
) #t4

select * into #t5
from (
select pp.CompanyCode, pp.BranchCode, pp.BranchHeadID, pp.BranchHeadName, pp.SalesHeadID, pp.SalesHeadName, 
	   pp.SalesCoordinatorID, pp.SalesCoordinatorName, pt.EmployeeID SalesmanID, ge.EmployeeName SalesmanName
  from #t4 pp
  left join pmMstTeamMembers pt
	on pp.CompanyCode=pt.CompanyCode
   and pp.BranchCode=pt.BranchCode
   and pp.TeamID=pt.TeamID
   and pt.IsSupervisor=0
  left join pmMstTeamMembers pd
	on pp.CompanyCode = pd.CompanyCode
   and pp.BranchCode = pd.BranchCode
   and pt.EmployeeID = pd.EmployeeID
   and pd.IsSupervisor = 1
  left join gnMstEmployee ge
	on pp.CompanyCode = ge.CompanyCode
   and pp.BranchCode = ge.BranchCode
   and pt.EmployeeID = ge.EmployeeID
   and ge.PersonnelStatus = 1
) #t5

insert into PmHstITS
select a.CompanyCode
	 , a.BranchCode
	 , a.InquiryNumber
	 , a.InquiryDate
	 , a.OutletID
	 , LEFT(isnull((select Top 1 BranchHeadname from #t5 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and SalesCoordinatorID = a.SpvEmployeeID),''),50) BranchHead
	 , LEFT(isnull((select Top 1 SalesHeadName from #t5 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and SalesCoordinatorID = a.SpvEmployeeID),''),50) SalesHead
	 , LEFT(isnull((select Top 1 SalesCoordinatorName from #t5 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and SalesCoordinatorID = a.SpvEmployeeID),''),50) SalesCoordinator
	 , LEFT(isnull((select Top 1 SalesmanName from #t5 where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and SalesmanID = a.EmployeeID),''),50) Wiraniaga
	 , a.StatusProspek
	 , a.PerolehanData
	 , LEFT(replace(replace(isnull(a.NamaProspek,' '),CHAR(13), ' '),CHAR(10), ' '),50) NamaProspek
	 , LEFT(replace(replace(replace(isnull(a.AlamatProspek,' '),';',':'),CHAR(13), ' '),CHAR(10), ' '),200) AlamatProspek
	 , LEFT(a.TelpRumah,15) TelpRumah
	 , LEFT(isnull((select TOP 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'CITY' and LookUpValue = a.CityID),''),50) City
	 , LEFT(replace(replace(isnull(a.NamaPerusahaan,' '),CHAR(13), ' '),CHAR(10), ' '),50) NamaPerusahaan
	 , LEFT(replace(replace(replace(isnull(a.AlamatPerusahaan,' '),';',':'),CHAR(13), ' '),CHAR(10), ' '),200) AlamatPerusahaan
	 , a.Jabatan
	 , a.Handphone
	 , a.Faximile
	 , a.Email
	 , a.TipeKendaraan
	 , a.Variant
	 , a.Transmisi
	 , a.ColourCode
	 , LEFT(isnull((select TOP 1 RefferenceDesc1 from omMstRefference where CompanyCode = a.CompanyCode and RefferenceType = 'COLO' and RefferenceCode = a.ColourCode),''),50) ColourDescription
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'PMBY' and LookUpValue = a.CaraPembayaran),''),30) CaraPembayaran
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'PMOP' and LookUpValue = a.TestDrive),''),15) TestDrive
	 , a.QuantityInquiry
	 , LEFT(a.LastProgress,15) LastProgress
	 , a.LastUpdateStatus
	 , isnull((select top 1 isnull(UpdateDate,'19000101') from pmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber and LastProgress IN ('P','PROSPECT')),'19000101') ProspectDate
	 , isnull((select top 1 isnull(UpdateDate,'19000101') from pmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber and LastProgress IN ('HP','HOT')),'19000101') HotDate
	 , isnull((select top 1 isnull(UpdateDate,'19000101') from pmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber and LastProgress IN ('SPK')),'19000101') SPKDate
	 , isnull((select top 1 isnull(UpdateDate,'19000101') from pmStatusHistory where CompanyCode = a.CompanyCode and BranchCode = a.BranchCode and InquiryNumber = a.InquiryNumber and LastProgress IN ('DELIVER','DO','DELIVERY','DEAL')),'19000101') DeliveryDate
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'LSNG' and LookUpValue = a.Leasing),''),30) Leasing
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'DWPM' and LookUpValue = a.DownPayment),''),30) DownPayment
	 , LEFT(isnull((select Top 1 LookUpValueName from GnMstLookUpDtl where CompanyCode = a.CompanyCode and CodeID = 'TENOR' and LookUpValue = a.Tenor),''),30) Tenor
	 , a.LostCaseDate
	 , LEFT(replace(replace(isnull(a.LostCaseCategory,' '),CHAR(13), ' '),CHAR(10), ' '),30) LostCaseCategory
	 , LEFT(a.LostCaseReasonID,30) LostCaseReasonID
	 , LEFT(replace(replace(isnull(a.LostCaseOtherReason,' '),CHAR(13), ' '),CHAR(10), ' '),100) LostCaseOtherReason
	 , LEFT(replace(replace(isnull(a.LostCaseVoiceOfCustomer,' '),CHAR(13), ' '),CHAR(10), ' '),200) LostCaseVoiceOfCustomer
	 , LEFT(a.MerkLain,50) MerkLain
	 , a.CreatedBy
	 , a.CreationDate CreatedDate
	 , a.LastUpdateBy
	 , a.LastUpdateDate
from pmKDP a
where 1 = 1
and not exists (select * from PmHstITS b
				 where  b.CompanyCode = a.CompanyCode
				   and b.BranchCode = a.BranchCOde
				   and b.InquiryNumber = a.InquiryNumber)
				   order by LastUpdateDate

drop table #t2
drop table #t3
drop table #t4
drop table #t5
GO
EXEC uspfn_SyncPmHstIts
*/});


var startTasks= function(callback)
{
    var taskJobs = [];		
    config.conn().forEach(listWorker);		
    async.series(taskJobs, function (err, docs) {
        if (err) config.log("Tasks", err);			
        if (callback) callback();
    });
    function listWorker(cfg){
        if(cfg.DealerCode == CurrentDealerCode)
        {
            taskJobs.push(function(callback){start(cfg, callback);});
        }			 
    }				
}


var start = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 

 var xSQL= [], sql = SQL.split('\nGO');	
    sql.forEach(ExecuteSQL);

    async.series(xSQL, function (err, docs) {
        if (err) log("ERROR" + err);
		log('done');
    });	
	
	function ExecuteSQL(s){
        xSQL.push(function(callback){		
			
            sqlODBC.query(cfg.ConnString,s , function (err, data) { 
				if (err) { 
					log("ERROR >> " + err);
				} else {
					if (data && data.length == 1) {
						if (data[0].info !== undefined)
							log(data[0].info);
					}
				}
                callback(); 
            });		
        });
    }	
}

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

startTasks();