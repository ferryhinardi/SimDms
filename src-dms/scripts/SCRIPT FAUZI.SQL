select e.EmployeeID "SALES_ID"
      ,e.EmployeeName "SALES_NAME"
      ,u.FullName "SALES_FULL_NAME"
      ,5 "AREA_CODE"
      ,'' as "DEALER_GROUP"
      ,e.CompanyCode "DEALER_CODE"
      ,(select  distinct TOP 1 CompanyGovName from gnMstCoProfile) "CUSTOMER_NAME"
      ,e.BranchCode "OUTLET_CODE"
      , 'D' as "OUTLET_TYPE"
      ,(select companyName from gnMstCoProfile cp where cp.BranchCode=e.BranchCode) "OUTLET_NAME"
      ,p.PositionName "JOB_CODE"
      ,p.PositionName "JOB_DESC"
      ,'' "GRADE_CODE"
      ,convert(varchar(12),e.JoinDate, 106) "START_Date"
      ,'' "IS_ADD_JOB"
      ,'' "ADD_JOB_CODE"
      ,e.BirthPlace "BIRTHPLACE"
      ,convert(varchar(12),e.BirthDate,106) "BIRTHDATE"
      ,CASE e.FormalEducation WHEN '1' THEN 'STM'
WHEN '10' THEN 'S3'
WHEN '2' THEN 'SD'
WHEN '3' THEN 'SMP'
WHEN '4' THEN 'SMU / SMK'
WHEN '5' THEN 'D1'
WHEN '6' THEN 'D2'
WHEN '7' THEN 'D3'
WHEN '8' THEN 'S1'
WHEN '9' THEN 'S2' ELSE 'N/A' END as "EDUCATION"
      ,e.HpNo "PHONE1"
      ,e.PhoneNo "PHONE2"
      ,CASE e.ReligionCode WHEN 'B' THEN 'Budha'
 WHEN 'H' THEN 'Hindu'
 WHEN 'I' THEN 'Islam'
 WHEN 'K' THEN 'Kristen'
 WHEN 'T' THEN 'Katholik' END "RELIGION"
      ,e.IdentityNo "KTP_NO"
      ,'' "SIM_1"
      ,'' "SIM_2"
      ,'' "TERMINATE_STATUS"
      ,'' "TERMINATE_REASON"
      ,convert(varchar(12),e.ResignDate, 106) "Terminate_Date"
      ,CASE e.MaritalStatusCode WHEN 'K' THEN 'Married' ELSE 'SINGLE' END as "MARITAL_STATUS"
      ,CASE e.PersonnelStatus WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif' END as "STATUS"
	  ,CASE e.GenderCode WHEN 'P' THEN 'Male' ELSE 'Female' END as "SEX"
      ,'' "EMAIL"
      ,m.EmployeeID "T_LEADER"
      ,e.Nik "NIK"
      ,'' "STATUS_SF"
      , (SELECT LookUpValueName from gnMstLookupDtl where CodeId='TITL' and LookUpValue=e.TitleCode) "STATUS_SM"       
from gnMstEmployee e, pmMstTeamMembers t, pmMstTeamMembers m, pmPosition p, SysUser u
where 
t.EmployeeID=e.EmployeeID 
AND e.BranchCode = t.BranchCode
AND p.BRanchCode=e.BranchCode
AND t.issalesman=1 AND
m.TeamID=t.TeamID AND
m.IsSupervisor =1 AND
p.employeeId=e.employeeID 
and t.employeeId=p.employeeid
and p.PositionID=10
AND e.PersonnelStatus=1
AND u.UserId=p.UserId
UNION
select e.EmployeeID "SALES_ID"
      ,e.EmployeeName "SALES_NAME"
      ,u.FullName "SALES_FULL_NAME"
      ,5 "AREA_CODE"
      ,'' as "DEALER_GROUP"
      ,e.CompanyCode "DEALER_CODE"
      ,(select  distinct TOP 1 CompanyGovName from gnMstCoProfile) "CUSTOMER_NAME"
      ,e.BranchCode "OUTLET_CODE"
      , 'D' as "OUTLET_TYPE"
      ,(select companyName from gnMstCoProfile cp where cp.BranchCode=e.BranchCode) "OUTLET_NAME"
      ,p.PositionName "JOB_CODE"
      ,p.PositionName "JOB_DESC"
      ,'' "GRADE_CODE"
      ,convert(varchar(12),e.JoinDate, 106) "START_Date"
      ,'' "IS_ADD_JOB"
      ,'' "ADD_JOB_CODE"
      ,e.BirthPlace "BIRTHPLACE"
      ,convert(varchar(12),e.BirthDate,106) "BIRTHDATE"
      ,CASE e.FormalEducation WHEN '1' THEN 'STM'
WHEN '10' THEN 'S3'
WHEN '2' THEN 'SD'
WHEN '3' THEN 'SMP'
WHEN '4' THEN 'SMU / SMK'
WHEN '5' THEN 'D1'
WHEN '6' THEN 'D2'
WHEN '7' THEN 'D3'
WHEN '8' THEN 'S1'
WHEN '9' THEN 'S2' ELSE 'N/A' END as "EDUCATION"
      ,e.HpNo "PHONE1"
      ,e.PhoneNo "PHONE2"
      ,CASE e.ReligionCode WHEN 'B' THEN 'Budha'
 WHEN 'H' THEN 'Hindu'
 WHEN 'I' THEN 'Islam'
 WHEN 'K' THEN 'Kristen'
 WHEN 'T' THEN 'Katholik' END "RELIGION"
      ,e.IdentityNo "KTP_NO"
      ,'' "SIM_1"
      ,'' "SIM_2"
      ,'' "TERMINATE_STATUS"
      ,'' "TERMINATE_REASON"
      ,convert(varchar(12),e.ResignDate, 106) "Terminate_Date"
      ,CASE e.MaritalStatusCode WHEN 'K' THEN 'Married' ELSE 'SINGLE' END as "MARITAL_STATUS"
      ,CASE e.PersonnelStatus WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif' END as "STATUS"
	  ,CASE e.GenderCode WHEN 'P' THEN 'Male' ELSE 'Female' END as "SEX"
      ,'' "EMAIL"
      ,m.EmployeeID "T_LEADER"
      ,e.Nik "NIK"
      ,'' "STATUS_SF"
      ,(SELECT LookUpValueName from gnMstLookupDtl where CodeId='TITL' and LookUpValue=e.TitleCode) "STATUS_SM"
from gnMstEmployee e, pmMstTeamMembers t, pmMstTeamMembers m, pmPosition p, sysuser u
where 
t.EmployeeID=e.EmployeeID 
AND e.BranchCode = t.BranchCode
AND p.BRanchCode=e.BranchCode
AND t.issupervisor=0
AND m.TeamID=t.TeamID AND
m.IsSupervisor =1 AND
p.employeeId=e.employeeID 
and t.employeeId=p.employeeid
AND u.UserId=p.UserId
and p.PositionID=20
AND e.PersonnelStatus=1
UNION 
select e.EmployeeID "SALES_ID"
      ,e.EmployeeName "SALES_NAME"
      ,u.FullName "SALES_FULL_NAME"
      ,5 "AREA_CODE"
      ,'' as "DEALER_GROUP"
      ,e.CompanyCode "DEALER_CODE"
      ,(select  distinct TOP 1 CompanyGovName from gnMstCoProfile) "CUSTOMER_NAME"
      ,e.BranchCode "OUTLET_CODE"
      , 'D' as "OUTLET_TYPE"
      ,(select companyName from gnMstCoProfile cp where cp.BranchCode=e.BranchCode) "OUTLET_NAME"
      ,p.PositionName "JOB_CODE"
      ,p.PositionName "JOB_DESC"
      ,'' "GRADE_CODE"
      ,convert(varchar(12),e.JoinDate, 106) "START_Date"
      ,'' "IS_ADD_JOB"
      ,'' "ADD_JOB_CODE"
      ,e.BirthPlace "BIRTHPLACE"
      ,convert(varchar(12),e.BirthDate,106) "BIRTHDATE"
      ,CASE e.FormalEducation WHEN '1' THEN 'STM'
WHEN '10' THEN 'S3'
WHEN '2' THEN 'SD'
WHEN '3' THEN 'SMP'
WHEN '4' THEN 'SMU / SMK'
WHEN '5' THEN 'D1'
WHEN '6' THEN 'D2'
WHEN '7' THEN 'D3'
WHEN '8' THEN 'S1'
WHEN '9' THEN 'S2' ELSE 'N/A' END as "EDUCATION"
      ,e.HpNo "PHONE1"
      ,e.PhoneNo "PHONE2"
      ,CASE e.ReligionCode WHEN 'B' THEN 'Budha'
 WHEN 'H' THEN 'Hindu'
 WHEN 'I' THEN 'Islam'
 WHEN 'K' THEN 'Kristen'
 WHEN 'T' THEN 'Katholik' END "RELIGION"
      ,e.IdentityNo "KTP_NO"
      ,'' "SIM_1"
      ,'' "SIM_2"
      ,'' "TERMINATE_STATUS"
      ,'' "TERMINATE_REASON"
      ,convert(varchar(12),e.ResignDate, 106) "Terminate_Date"
      ,CASE e.MaritalStatusCode WHEN 'K' THEN 'Married' ELSE 'SINGLE' END as "MARITAL_STATUS"
      ,CASE e.PersonnelStatus WHEN 1 THEN 'Aktif' ELSE 'Tidak Aktif' END as "STATUS"
	  ,CASE e.GenderCode WHEN 'P' THEN 'Male' ELSE 'Female' END as "SEX"
      ,'' "EMAIL"
      ,m.EmployeeID "T_LEADER"
      ,e.Nik "NIK"
      ,'' "STATUS_SF"
      ,(SELECT LookUpValueName from gnMstLookupDtl where CodeId='TITL' and LookUpValue=e.TitleCode) "STATUS_SM"
      from gnMstEmployee e, pmMstTeamMembers t, pmMstTeamMembers m, pmPosition p, sysuser u
where 
t.EmployeeID=e.EmployeeID 
AND e.BranchCode = t.BranchCode
AND p.BRanchCode=e.BranchCode
AND t.issupervisor=0
AND m.TeamID=t.TeamID AND
m.IsSupervisor =1 AND
p.employeeId=e.employeeID 
and t.employeeId=p.employeeid
and p.PositionID=30
AND e.PersonnelStatus=1
AND u.UserId=p.UserId
