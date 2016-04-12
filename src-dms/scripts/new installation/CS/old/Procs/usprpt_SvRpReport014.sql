
/****** Object:  StoredProcedure [dbo].[usprpt_SvRpReport014]    Script Date: 11/11/2013 11:44:13 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[usprpt_SvRpReport014]
 @p_CompanyCode VARCHAR(20),    
 @p_BranchCode	VARCHAR(20),    
 @p_ProductType VARCHAR(20),    
 @p_SPKDateFrom VARCHAR(15),    
 @p_SPKDateTo	VARCHAR(15),    
 @p_OutStanding BIT,
 @GroupJobType	VARCHAR(15),
 @param			DECIMAL(18,2)     
    
AS    
    
BEGIN
-- usprpt_SvRpReport014 '6159401','615940100','4W','20100513','20110513', '0', ''     

-- declare @p_CompanyCode VARCHAR(20),    
--		@p_BranchCode	VARCHAR(20),    
--		@p_ProductType	VARCHAR(20),    
--		@p_SPKDateFrom	VARCHAR(15),    
--		@p_SPKDateTo	VARCHAR(15),    
--		@p_OutStanding	BIT,
--		@GroupJobType	VARCHAR(15)      
--
--set @p_CompanyCode	= '6114201'    
--set	@p_BranchCode	= '611420101'
--set	@p_ProductType	= '2W'
--set	@p_SPKDateFrom	= '20101103'
--set	@p_SPKDateTo	= '20101103'
--set	@p_OutStanding	= '0'
--set @GroupJobType	= ''

select * into #t1 from (
SELECT DISTINCT
	  job.GroupJobType
	, case @GroupJobType when '' then '' else job.GroupJobType + ' - ' + reff1.Description end DescGroupJobType
	, prf.CompanyName    
	,(ISNULL(prf.Address1,'') + (CASE ISNULL(prf.Address2,'') WHEN '' THEN '' ELSE ', ' END) 
		+ ISNULL(prf.Address2,'') + (CASE ISNULL(prf.Address3,'') WHEN '' THEN '' ELSE ', ' END) 
		+ ISNULL(prf.Address3,'') + (CASE ISNULL(prf.Address4,'') WHEN '' THEN '' ELSE ', ' END) 
		+ ISNULL(prf.Address4,'')) [Address]    
	,srv.JobOrderNo
	,srv.JobOrderDate JobOrderDate
	,srv.BasicModel
	,srv.policeRegNo
	,srv.JobOrderDate JobOrderTime    
	,srv.Odometer, case srv.LockingDate when ('19000101') then NULL else srv.LockingDate end LockingDate    
	,srv.LockingDate LockingTime, srv.CreatedBy
	,LEFT(LTRIM(frm.EmployeeName),12) ForemanID    
	,LEFT(LTRIM(mek.EmployeeName),12) MechanicID
	,srv.ServiceRequestDesc
	,(srv.CustomerCode + '-' + cust.CustomerName) Customer    
	,(srv.JobType + '-' + reff.Description) JobType
	,CASE srv.ServiceStatus    
		WHEN '0' THEN 'Open Job Order'    
		WHEN '1' THEN 'Progress Service'    
		WHEN '2' THEN 'Test Drive'    
		WHEN '3' THEN 'Final Check'    
		WHEN '4' THEN 'Washing'    
		WHEN '5' THEN 'Closed SPK'    
		WHEN '6' THEN 'Cancel SPK'    
		WHEN '7' THEN 'Invoice'    
		WHEN '8' THEN 'Cancel Invoice'    
		WHEN '9' THEN 'Faktur Pajak'    
		WHEN 'A' THEN 'Cancel Faktur Pajak'    
		WHEN 'B' THEN 'Faktur Pajak Return'    
	END [Status]    
	,InvoiceNo = (SELECT TOP 1 InvoiceNo FROM SvTrnInvoice inv WHERE inv.CompanyCode = srv.CompanyCode    
		AND inv.BranchCode = srv.BranchCode 
		AND inv.ProductType = srv.ProductType 
		AND inv.JobOrderNo = srv.JobOrderNo)  
	,it.PartNo
	,ifo.PartName  
	,it.DemandQty
	,it.SupplyQty
	,it.ReturnQty
	,it.SupplySlipNo SSNo
	,it.SSReturnNo SSReturn
	,(select EmployeeName from gnMstEmployee where CompanyCode = @p_CompanyCode and BranchCode = @p_BranchCode and EmployeeID = mech.MechanicID) Mechanic
	,task.OperationNo + ' - ' + mstTask.Description Task
	,task.OperationHour
	,(select count(OperationNo) from svTrnSrvTask where CompanyCode = @p_CompanyCode and BranchCode = @p_BranchCode and ProductType = @p_ProductType and ServiceNo = srv.ServiceNo) countOperationNo
	, srv.ServiceStatus
	, srv.TotalSrvAmount
FROM 
	SvTrnService srv
INNER JOIN GnMstCoProfile prf 
	ON srv.CompanyCode = prf.CompanyCode    
    AND srv.BranchCode = prf.BranchCode    
LEFT JOIN GnMstCustomer cust 
	ON srv.CustomerCode = cust.CustomerCode     
    AND srv.CompanyCode = cust.CompanyCode    
LEFT JOIN SvMstRefferenceService reff 
	ON srv.CompanyCode  = reff.CompanyCode    
    AND srv.ProductType = reff.ProductType 
	AND srv.JobType		= reff.RefferenceCode    
    AND reff.RefferenceType = 'JOBSTYPE'    
LEFT JOIN gnMstEmployee frm 
	ON frm.CompanyCode = srv.CompanyCode
    AND frm.BranchCode = srv.BranchCode
    AND frm.EmployeeID = srv.ForemanID
LEFT JOIN gnMstEmployee mek 
	ON mek.CompanyCode = srv.CompanyCode
    AND mek.BranchCode = srv.BranchCode
    AND mek.EmployeeID = srv.MechanicID
LEFT JOIN SvTrnSrvItem it 
	ON it.CompanyCode  = srv.CompanyCode
	AND it.BranchCode  = srv.BranchCode
	AND it.ProductType = srv.ProductType
	AND it.ServiceNo   = srv.ServiceNo	
LEFT JOIN SpMstItemInfo ifo 
	ON ifo.CompanyCode = srv.CompanyCode
	AND ifo.PartNo	   = it.PartNo
LEFT JOIN svMstJob job
	ON job.CompanyCode	= srv.CompanyCode
	AND job.ProductType = srv.ProductType
	AND job.BasicModel	= srv.BasicModel
	AND job.JobType		= srv.JobType
LEFT JOIN svMstRefferenceService reff1
    on reff1.CompanyCode = srv.CompanyCode
   and reff1.ProductType = srv.ProductType
   and reff1.RefferenceCode = job.GroupJobType
   and reff1.RefferenceType = 'GRPJOBTY'
LEFT JOIN svTrnSrvTask task
	on task.CompanyCode = srv.CompanyCode
	and task.BranchCode = srv.BranchCode
	and task.ProductType = srv.ProductType
	and task.ServiceNo = srv.ServiceNo
LEFT JOIN svTrnSrvMechanic mech
	on mech.CompanyCode = srv.CompanyCode
	and mech.BranchCode = srv.BranchCode
	and mech.ProductType = srv.ProductType
	and mech.ServiceNo = srv.ServiceNo
	and mech.OperationNo = task.OperationNo
LEFT JOIN svMstTask mstTask
	on mstTask.CompanyCode = srv.CompanyCode
	and mstTask.ProductType = srv.ProductType
	and mstTask.BasicModel =  srv.BasicModel
	and mstTask.JobType = srv.JobType
	and mstTask.OperationNo = task.OperationNo
WHERE
	srv.CompanyCode		= @p_CompanyCode 
	AND srv.BranchCode	= @p_BranchCode    
	AND srv.ProductType = @p_ProductType 
	AND srv.ServiceType = '2'
	AND CONVERT(VARCHAR, srv.JobOrderDate, 112) BETWEEN @p_SPKDateFrom AND @p_SPKDateTo      
	AND job.GroupJobType like '%' + @GroupJobType + '%'
)#t1

if (@p_OutStanding = '1')
begin
	select * from #t1 where ServiceStatus in ('0','1','2','3','4') and TotalSrvAmount >= @param
	order by JobOrderNo,Task
	
	select distinct JobOrderNo,Task,OperationHour,Mechanic
	from #t1
	where ServiceStatus in ('0','1','2','3','4') and TotalSrvAmount >= @param
	order by JobOrderNo,Task
end
else
begin
	select * from #t1 where ServiceStatus in ('0','1','2','3','4','5','7','8','9')
	order by JobOrderNo,Task
	
	select distinct JobOrderNo,Task,OperationHour,Mechanic
	from #t1
	where ServiceStatus in ('0','1','2','3','4','5','7','8','9')
	order by JobOrderNo,Task
end

END

GO


