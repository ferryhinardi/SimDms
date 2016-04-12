if object_id('uspfn_itsinqfollowup') is not null
	drop procedure uspfn_itsinqfollowup
GO
CREATE Procedure uspfn_itsinqfollowup  
(      
 @CompanyCode VARCHAR(15),      
 @BranchCode  VARCHAR(15),      
 @DateAwal  VARCHAR(10),      
 @DateAkhir  VARCHAR(10),      
 @Outlet   VARCHAR(max),      
 @SPV   VARCHAR(max),      
 @EMP   VARCHAR(max),         
 @Head   VARCHAR(max)      
)      
AS      
BEGIN      
  
SELECT * INTO #t1 FROM (
SELECT
    f.OutletName, a.InquiryNumber, a.NamaProspek Pelanggan, a.InquiryDate, a.TipeKendaraan,
    a.Variant, a.Transmisi, b.RefferenceDesc1 Warna, a.PerolehanData,
    emp1.EmployeeName Employee, emp2.EmployeeName Supervisor, e.NextFollowUpDate, a.LastProgress, e.ActivityDetail, a.SpvEmployeeId
FROM
    PmKDP a
LEFT JOIN OmMstRefference b
    ON b.CompanyCode = a.CompanyCode AND b.RefferenceType='COLO' AND b.RefferenceCode=a.ColourCode
left join HrEmployee emp1 on emp1.CompanyCode = a.CompanyCode  
     and emp1.EmployeeID = a.EmployeeID  
--LEFT JOIN GnMstEmployee c
--    ON c.CompanyCode = a.CompanyCode AND c.BranchCode = a.BranchCode AND c.EmployeeID = a.EmployeeID
left join HrEmployee emp2 on emp2.CompanyCode = a.CompanyCode  
     and emp2.EmployeeID = a.EmployeeID  
--LEFT JOIN GnMstEmployee d
--    ON d.CompanyCode = a.CompanyCode AND d.BranchCode = a.BranchCode AND d.EmployeeID = a.SpvEmployeeID
LEFT JOIN PmActivities e
    ON e.CompanyCode = a.CompanyCode AND e.BranchCode = a.BranchCode AND e.InquiryNumber=a.InquiryNumber
    AND e.ActivityID = (SELECT TOP 1 ActivityID FROM PmActivities WHERE CompanyCode = a.CompanyCode 
	AND BranchCode = a.BranchCode AND InquiryNumber=a.InquiryNumber ORDER BY ActivityID DESC) 
LEFT JOIN PmBranchOutlets f
	ON f.CompanyCode = a.CompanyCode AND f.BranchCode = a.BranchCode AND f.OutletID = a.OutletID
WHERE
    a.CompanyCode = @CompanyCode 
	AND ((CASE WHEN @BranchCode='' THEN a.BranchCode END)<>''OR (CASE WHEN @BranchCode<>'' THEN a.BranchCode END)=@BranchCode)
    AND CONVERT(VARCHAR, e.NextFollowUpDate, 112) BETWEEN @DateAwal AND @DateAkhir
    AND ((CASE WHEN @Outlet='' THEN a.OutletID END)<>'' OR (CASE WHEN @Outlet<>'' THEN a.OutletID END)=@Outlet)
    AND ((CASE WHEN @SPV='' THEN a.SpvEmployeeID END)<>'' OR (CASE WHEN @SPV<>'' THEN a.SpvEmployeeID END)=@SPV)
    AND ((CASE WHEN @EMP='' THEN a.EmployeeID END)<>'' OR (CASE WHEN @EMP<>'' THEN a.EmployeeID END)=@EMP)
) #t1

IF (@HEAD='')
BEGIN
    SELECT * FROM #t1 ORDER BY InquiryNumber 
END
ELSE
BEGIN
	declare @teamid varchar(max)
	set @teamid = (select teamid from pmmstteammembers where companycode=@CompanyCode 
		and branchcode=case @branchcode when '' then branchcode else @branchcode end
		and employeeid=@HEAD 
		and issupervisor='1')
	
	SELECT * FROM #t1 WHERE SpvEmployeeID IN (select employeeid from pmmstteammembers where companycode=@companycode 
		and branchcode=case @branchcode when '' then branchcode else @branchcode end
		and teamid=@teamid
		and issupervisor='0') 
	ORDER BY InquiryNumber 
END
DROP TABLE #t1 
END
GO
