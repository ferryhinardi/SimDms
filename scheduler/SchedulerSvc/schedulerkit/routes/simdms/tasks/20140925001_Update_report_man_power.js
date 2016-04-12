var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var config = require('../config');
var Client = require('q-svn-spawn');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "UPDATE_REPORT_MAN_POWER";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
if object_id('uspfn_HrInqPersInfo') is not null
	drop procedure uspfn_HrInqPersInfo
GO
select 'Create SP uspfn_HrInqPersInfo'
GO
CREATE procedure [dbo].[uspfn_HrInqPersInfo]
  @CompanyCode varchar(20),
  @DeptCode varchar(20),
  @PosCode varchar(20),
  @Status varchar(20),
  @BranchCode varchar(20) = ''
as      

;with x as (      
select a.CompanyCode
     , a.EmployeeID      
     , e.SalesID      
     , a.EmployeeName      
     , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and isnull(isDeleted, 0) != 1 order by MutationDate desc), '')      
     , DeptCode = isnull((select top 1 HrEmployeeAchievement.Department from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and isDeleted != 1 and AssignDate <= getdate() order by AssignDate desc), '')      
     , PosCode = isnull((select top 1 HrEmployeeAchievement.Position from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and isDeleted != 1 and AssignDate <= getdate() order by AssignDate desc), '')      
     , Status = (case isnull(a.PersonnelStatus, '')      
		  when '1' then 'Aktif'      
		  when '2' then 'Non Aktif'      
		  when '3' then 'Keluar'      
		  when '4' then 'Pensiun'      
		  else 'No Status' end)      
     , a.Address1      
     , a.Address2      
     , a.Address3      
     , a.Address4      
     , Address = isnull(a.Address1, '') --+ ' ' + ltrim(isnull(a.Address2, '')) + ' ' + ltrim(isnull(a.Address3, '')) + ' ' + ltrim(isnull(a.Address4, ''))      
	 , a.Grade  
     , convert(varchar(10),a.BirthDate,120) BirthDate      
     , upper(a.BirthPlace) BirthPlace
     , a.BloodCode      
     , SubOrdinates = (select count(x.EmployeeID) from HrEmployee x where x.TeamLeader=a.EmployeeID)      
     , MutationTimes = (select count(x.EmployeeID) from HrEmployeeMutation x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
     , AchieveTimes = (select count(x.EmployeeID) from HrEmployeeAchievement x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
     , TeamLeader = (select top 1 x.EmployeeName from HrEmployee x where x.EmployeeID=a.TeamLeader)      
     , convert(varchar(10),a.JoinDate,120) JoinDate      
     , a.ResignDate      
     , a.ResignDescription      
     , MaritalStatus = upper(f.LookUpValueName)
     , Religion = upper(g.LookUpValueName)
     , Education = upper(h.LookUpValueName)
     , Gender = upper(i.LookUpValueName)
     , a.Province      
     , a.District      
     , a.SubDistrict      
     , a.Village      
     , a.ZipCode      
     , a.IdentityNo      
     , a.NPWPNo      
     , a.NPWPDate      
     , a.Email      
     , a.FaxNo      
     , a.Telephone1      
     , a.Telephone2      
     , a.Handphone1      
     , a.Handphone2      
     , a.Handphone3      
     , a.Handphone4      
     , a.DrivingLicense1      
     , a.DrivingLicense2      
     , a.Height      
     , a.Weight      
     , a.UniformSize      
     , a.UniformSizeAlt      
  from HrEmployee a      
  left join GnMstPosition c      
    on c.CompanyCode = a.CompanyCode      
   and c.DeptCode = a.Department      
   and c.PosCode = a.Position      
  left join GnMstLookUpDtl f      
    on f.CompanyCode = a.CompanyCode      
   and f.CodeID = 'MRTL'      
   and f.LookUpValue = a.MaritalStatus      
  left join GnMstLookUpDtl g      
    on g.CompanyCode = a.CompanyCode 
   and g.CodeID = 'RLGN'      
   and g.LookUpValue = a.Religion      
  left join GnMstLookUpDtl h      
    on h.CompanyCode = a.CompanyCode 
   and h.CodeID = 'FEDU'      
   and h.LookUpValue = a.FormalEducation      
  left join GnMstLookUpDtl i      
    on i.CompanyCode = a.CompanyCode 
   and i.CodeID = 'GNDR'      
   and i.LookUpValue = a.Gender  
  left join HrEmployeeSales e      
    on e.CompanyCode = a.CompanyCode      
   and e.EmployeeID = a.EmployeeID      
 where 1 = 1
   --and a.CompanyCode = @CompanyCode      
   and a.CompanyCode = (case @CompanyCode when '' then a.CompanyCode else @CompanyCode end)      
   and a.Department = @DeptCode      
   and a.Position = (case @PosCode when '' then a.Position else @PosCode end)      
   and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)      
)      
select x.CompanyCode
     , x.EmployeeID
	 , x.SalesID
	 , x.EmployeeName
	 , x.BranchCode
	 , c.BranchName
	 , x.DeptCode
	 , x.PosCode
     , Position = ''
	     + upper(x.DeptCode)      
         + upper(case isnull(b.PosName, '') when '' then '' else ' - ' + b.PosName end)      
         + upper(case isnull(d.LookUpValueName, '') when '' then '' else ' - ' + d.LookUpValueName end)    
	 , b.PosName
	 , GradeCode = x.Grade
	 , Grade = upper(d.LookUpValueName)
	 , x.Status
	 , x.Address1
	 , x.Address2
	 , x.Address3
	 , x.Address4
	 , x.Address
	 , x.BirthDate
	 , x.BirthPlace
	 , x.BloodCode
	 , x.SubOrdinates
	 , x.MutationTimes
	 , x.AchieveTimes
	 , x.TeamLeader
	 , x.JoinDate
	 , x.ResignDate
	 , x.ResignDescription
	 , x.MaritalStatus
	 , x.Religion
	 , x.Education
	 , x.Gender
	 , upper(x.Province) as Province
	 , x.District as City
	 , x.SubDistrict
	 , x.Village
	 , x.ZipCode
	 , x.IdentityNo
	 , x.NPWPNo
	 , x.NPWPDate
	 , x.Email
	 , x.FaxNo
	 , x.Telephone1
	 , x.Telephone2
	 , x.Handphone1
	 , x.Handphone2
	 , x.Handphone3
	 , x.Handphone4
	 , x.DrivingLicense1
	 , x.DrivingLicense2
	 , x.Height
	 , x.Weight
	 , x.UniformSize
	 , x.UniformSizeAlt
  from x 
  left join GnMstPosition b      
    on b.CompanyCode = x.CompanyCode
   and b.DeptCode = x.DeptCode      
   and b.PosCode = x.PosCode  
  left join OutletInfo c
    on c.CompanyCode = x.CompanyCode
   and c.BranchCode = x.BranchCode     
  left join GnMstLookUpDtl d      
    on d.CompanyCode = x.CompanyCode     
   and d.CodeID = 'ITSG'      
   and d.LookUpValue = x.Grade      
 where isnull(x.BranchCode, '') = (case @BranchCode when '' then x.BranchCode else @BranchCode end)

GO
if object_id('uspfn_HrInqPersInfoDetail') is not null
	drop procedure uspfn_HrInqPersInfoDetail
GO
select 'Create SP uspfn_HrInqPersInfoDetail'
GO
CREATE procedure [dbo].[uspfn_HrInqPersInfoDetail]
  @CompanyCode varchar(20),
  @DeptCode varchar(20),
  @PosCode varchar(20),
  @Status varchar(20),
  @BranchCode varchar(20) = ''
as      

;with x as (      
select a.CompanyCode
     , a.EmployeeID      
     , e.SalesID      
     , a.EmployeeName      
     , BranchCode = isnull((select top 1 BranchCode from HrEmployeeMutation where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and isnull(isDeleted, 0) != 1 order by MutationDate desc), '')      
     , DeptCode = isnull((select top 1 HrEmployeeAchievement.Department from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and isDeleted != 1 and AssignDate <= getdate() order by AssignDate desc), '')      
     , PosCode = isnull((select top 1 HrEmployeeAchievement.Position from HrEmployeeAchievement where CompanyCode = a.CompanyCode and EmployeeID = a.EmployeeID and isDeleted != 1 and AssignDate <= getdate() order by AssignDate desc), '')      
     , Status = (case isnull(a.PersonnelStatus, '')      
		  when '1' then 'Aktif'      
		  when '2' then 'Non Aktif'      
		  when '3' then 'Keluar'      
		  when '4' then 'Pensiun'      
		  else 'No Status' end)      
     , a.Address1      
     , a.Address2      
     , a.Address3      
     , a.Address4      
     , Address = isnull(a.Address1, '') --+ ' ' + ltrim(isnull(a.Address2, '')) + ' ' + ltrim(isnull(a.Address3, '')) + ' ' + ltrim(isnull(a.Address4, ''))      
	 , a.Grade  
     , a.BirthDate      
     , upper(a.BirthPlace) BirthPlace
     , a.BloodCode      
     , SubOrdinates = (select count(x.EmployeeID) from HrEmployee x where x.TeamLeader=a.EmployeeID)      
     , MutationTimes = (select count(x.EmployeeID) from HrEmployeeMutation x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
     , AchieveTimes = (select count(x.EmployeeID) from HrEmployeeAchievement x where x.CompanyCode=a.CompanyCode and x.EmployeeID=a.EmployeeID and x.IsDeleted != 1)      
     , TeamLeader = (select top 1 x.EmployeeName from HrEmployee x where x.EmployeeID=a.TeamLeader)      
     , a.JoinDate      
     , a.ResignDate      
     , a.ResignDescription      
     , MaritalStatus = upper(f.LookUpValueName)
     , Religion = upper(g.LookUpValueName)
     , Education = upper(h.LookUpValueName)
     , Gender = upper(i.LookUpValueName)
     , a.Province      
     , a.District      
     , a.SubDistrict      
     , a.Village      
     , a.ZipCode      
     , a.IdentityNo      
     , a.NPWPNo      
     , a.NPWPDate      
     , a.Email      
     , a.FaxNo      
     , a.Telephone1      
     , a.Telephone2      
     , a.Handphone1      
     , a.Handphone2      
     , a.Handphone3      
     , a.Handphone4      
     , a.DrivingLicense1      
     , a.DrivingLicense2      
     , a.Height      
     , a.Weight      
     , a.UniformSize      
     , a.UniformSizeAlt      
  from HrEmployee a      
  left join GnMstPosition c      
    on c.CompanyCode = a.CompanyCode      
   and c.DeptCode = a.Department      
   and c.PosCode = a.Position      
  left join GnMstLookUpDtl f      
    on f.CompanyCode = a.CompanyCode      
   and f.CodeID = 'MRTL'      
   and f.LookUpValue = a.MaritalStatus      
  left join GnMstLookUpDtl g      
    on g.CompanyCode =  a.CompanyCode 
   and g.CodeID = 'RLGN'      
   and g.LookUpValue = a.Religion      
  left join GnMstLookUpDtl h      
    on h.CompanyCode =  a.CompanyCode 
   and h.CodeID = 'FEDU'      
   and h.LookUpValue = a.FormalEducation      
  left join GnMstLookUpDtl i      
    on i.CompanyCode =  a.CompanyCode 
   and i.CodeID = 'GNDR'      
   and i.LookUpValue = a.Gender  
  left join HrEmployeeSales e      
    on e.CompanyCode = a.CompanyCode      
   and e.EmployeeID = a.EmployeeID      
 where 1 = 1
   and a.CompanyCode = (case @CompanyCode when '' then a.CompanyCode else @CompanyCode end)
   and a.Department = @DeptCode      
   and a.Position = (case @PosCode when '' then a.Position else @PosCode end)      
   and a.PersonnelStatus = (case @Status when '' then a.PersonnelStatus else @Status end)      
)    
,y as (  
select x.CompanyCode
     , e.DealerName
     , x.EmployeeID
	 , x.SalesID
	 , x.EmployeeName
	 , x.BranchCode
	 , c.BranchName
	 , x.DeptCode
	 , x.PosCode
     , Position = ''
	     + upper(x.DeptCode)      
         + upper(case isnull(b.PosName, '') when '' then '' else ' - ' + b.PosName end)      
         + upper(case isnull(d.LookUpValueName, '') when '' then '' else ' - ' + d.LookUpValueName end)    
	 , b.PosName
	 , GradeCode = x.Grade
	 , Grade = upper(d.LookUpValueName)
	 , x.Status
	 , x.Address1
	 , x.Address2
	 , x.Address3
	 , x.Address4
	 , x.Address
	 , x.BirthDate
	 , x.BirthPlace
	 , x.BloodCode
	 , x.SubOrdinates
	 , x.MutationTimes
	 , x.AchieveTimes
	 , x.TeamLeader
	 , x.JoinDate
	 , x.ResignDate
	 , x.ResignDescription
	 , x.MaritalStatus
	 , x.Religion
	 , x.Education
	 , x.Gender
	 , upper(x.Province) as Province
	 , x.District
	 , x.SubDistrict
	 , x.Village
	 , x.ZipCode
	 , x.IdentityNo
	 , x.NPWPNo
	 , x.NPWPDate
	 , x.Email
	 , x.FaxNo
	 , x.Telephone1
	 , x.Telephone2
	 , x.Handphone1
	 , x.Handphone2
	 , x.Handphone3
	 , x.Handphone4
	 , x.DrivingLicense1
	 , x.DrivingLicense2
	 , x.Height
	 , x.Weight
	 , x.UniformSize
	 , x.UniformSizeAlt
     , PreTraining = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP1')
     , PreTrainingPostTest = (select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP1')
     , Pembekalan = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP2')
     , PembekalanPostTest = (select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP2')
     , Salesmanship = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP3')
     , SalesmanshipPostTest = (select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP3')
     , Ojt = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP4')
     , OjtPostTest = (select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP4')
     , FinalReview = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP7')
     , FinalReviewPostTest = (select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'STDP7')
     , SpsSlv = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSS')
     , SpsSlvPostTest = (select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSS')
     , SpsGld = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSG')
     , SpsGldPostTest = (select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSG')
     , SpsPlt = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSP')
     , SpsPltPostTest = (select top 1 PostTest from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SPSP')
     , SCBsc = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SCB')
     , SCAdv = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SCA')
     , SHBsc = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SHB')
     , SHInt = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'SHI')
     , BMBsc = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'BMB')
     , BMInt = (select top 1 TrainingDate from HrEmployeeTraining where CompanyCode = x.CompanyCode and EmployeeID = x.EmployeeID and TrainingCode = 'BMI')
  from x 
  left join GnMstPosition b      
    on b.CompanyCode =  x.CompanyCode 
   and b.DeptCode = x.DeptCode      
   and b.PosCode = x.PosCode  
  left join OutletInfo c
    on c.CompanyCode = x.CompanyCode
   and c.BranchCode = x.BranchCode     
  left join GnMstLookUpDtl d      
    on d.CompanyCode =  x.CompanyCode      
   and d.CodeID = 'ITSG'      
   and d.LookUpValue = x.Grade      
  left join DealerInfo e      
    on e.DealerCode = x.CompanyCode
 where isnull(x.BranchCode, '') = (case @BranchCode when '' then x.BranchCode else @BranchCode end)
)
--select * from x
select y.CompanyCode as [Kode Dealer]
     , y.DealerName as [Nama Dealer]
     , y.BranchCode as [Kode Outlet]
     , y.BranchName as [Nama Outlet]
     , y.DeptCode as Department
     , y.EmployeeID as Nik
     , y.EmployeeName as Nama
     , y.PosName as Jabatan
     , y.Grade as Grade
     , '' as AdditionalJob
     , y.Status
     , y.JoinDate
     , y.TeamLeader
     , y.ResignDate
     , y.ResignDescription
     , y.MaritalStatus
     , y.Religion
     , y.Gender
     , y.Education
     , y.BirthPlace
     , y.BirthDate
     , y.Address
     , y.District as City
     , y.Province
     , y.ZipCode
     , y.IdentityNo
     , y.NPWPNo
     , y.Email
     , y.Telephone1
     , y.Telephone2
     , y.Handphone1
     , y.Handphone2
     , y.Handphone3
     , y.Handphone4 as PinBB
     , y.Height
     , y.Weight
     , y.UniformSize
     , y.UniformSizeAlt
     , y.PreTraining
     , y.PreTrainingPostTest
     , y.Pembekalan 
     , y.PembekalanPostTest as [Nilai Akhir Pembekalan]
     , y.Salesmanship as [Salesmanship]
     , y.SalesmanshipPostTest as [Nilai Akhir Salesmanship]
     , y.Ojt as [OJT]
     , y.OjtPostTest as [Nilai Akhir Ojt]
     , y.FinalReview as [Final Review]
     , y.FinalReviewPostTest as [Nilai Akhir Final Review]
     , y.SpsSlv as [Sales Silver]
     , y.SpsSlvPostTest as [Sales Silver PostTest]
     , y.SpsGld as [Sales Gold]
     , y.SpsGldPostTest as [Sales Gold PostTest]
     , y.SpsPlt as [Sales Platinum]
     , y.SpsPltPostTest as [Sales Platinum PostTest]
     , y.SCBsc as [SC Basic]
     , y.SCAdv as [SC Advance]
     , y.SHBsc as [SH Basic]
     , y.SHInt as [SH Intermediate]
     , y.BMBsc as [BM Basic]
     , y.BMInt as [BM Intermediate]
  from y where 1 = 1
   --and (y.PreTraining is not null or y.Pembekalan is not null or y.Salesmanship is not null or y.Ojt is not null or y.FinalReview is not null )
   --and (y.SpsSlv is not null or y.SpsGld is not null or y.SpsPlt is not null)
   --and (y.SCBsc is not null or y.SCAdv is not null)
   --and (y.SHBsc is not null or y.SHInt is not null)
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

var CheckWebConfig = function(cfg, callback)
{
	var file = cfg.SimDMSPath + '/web.config';			
	log('Config File: ' + file);	
	if (fs.existsSync(file))
	{
		var contents = fs.readFileSync(file).toString();
		var array = contents.split("\n");
		var haveMultipleApp = false, newContents = '';
		
		for(i=0;i<array.length;i++) {
		  
			if (!haveMultipleApp)
			{
				if ( array[i].toString().indexOf('key="MultipleApp"') > 0)
					haveMultipleApp = true;
			}
			if ( array[i].toString().indexOf("</appSettings>") > 0 && !haveMultipleApp)
			{
				newContents += '    <add key="MultipleApp" value="FALSE" />' + '\n';
			}
			
			newContents += array[i]+'\n';
		}
		
		fs.writeFileSync(file,newContents);
		
		var client = new Client({
			cwd: cfg.SimDMSPath,
			// username: 'svn-user', // optional if authentication not required or is already saved
			// password: 'user-password' // optional if authentication not required or is already saved
		});
		
		client.update(function(data) {
			log(data);
			log('updated');
			if (callback) callback();
		});
	} else {
		log('Configuration file not found or simdms path not configured');
		if (callback) callback();
	}
}

var start = function (cfg, callback) {

    log("Starting " + TaskName + " for " + CurrentDealerCode); 

	
    var xSQL= [], sql = SQL.split('\nGO');	
    sql.forEach(ExecuteSQL);

    async.series(xSQL, function (err, docs) {
        if (err) log("ERROR" + err);
		CheckWebConfig(cfg, function(ErrCfg, callback){
			log("Tasks" + TaskName + " has been executed"); 
			
			var url = config.api().downloadLink + 'uploadtasklog';
			var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
			var req = request.post(url, function (e, r, body) {
				if (e) console.log("ERROR", e);
				else {
					if (body !== undefined) {
						fs.unlinkSync(file);
					}
					console.log(body);
				}
				if (callback) callback();
			});		
						
			var form = req.form();
			form.append("DealerCode", CurrentDealerCode);
			form.append("UploadCode", "UPLCD");
			form.append('file', fs.createReadStream(file));
		});		
			
    });	

    function ExecuteSQL(s){
        xSQL.push(function(err, callback){
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