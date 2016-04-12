 ALTER PROCEDURE [dbo].[uspfn_Select4SO]  
  
@CompanyCode AS VARCHAR(20),  
@BranchCode  AS VARCHAR(20)  
--@TitleCode   AS VARCHAR(20)  
  
--DECLARE   
--@CompanyCode AS VARCHAR(20)='6435203',  
--@BranchCode  AS VARCHAR(20)='643520300',  
--@TitleCode   AS VARCHAR(20)='31'  
--SELECT @CompanyCode = '6114201', @BranchCode = '611420100', @TitleCode='31'  
  
AS  
BEGIN  
  
DECLARE @SrcEmp AS VARCHAR(2)  
SET @SrcEmp = (SELECT ParaValue FROM gnMstLookUpDtl WHERE CodeID = 'EMP_SFM' and LookUpValue = 'SRC_EMP')  
  
IF @SrcEmp = 'GN'  
BEGIN  
 SELECT   
  a.EmployeeID, a.EmployeeName, a.TitleCode  
  , (SELECT LookupValueName FROM gnMstLookupDtl 
 WHERE CompanyCode = a.CompanyCode and 
	CodeID = 'TITL' and 
	LookUpValue = a.TitleCode) AS TitleName  
 FROM GnMstEmployee a  
 WHERE a.CompanyCode = @CompanyCode  
  and case when @BranchCode = '' then '' else a.BranchCode end = @BranchCode -- update
  and a.PersonnelStatus = '1'   
 -- and a.TitleCode =@TitleCode   -- Perubahan  
  and a.TitleCode ='12'   -- Penambahan  
END  
ELSE  
BEGIN  
 SELECT a.EmployeeID, a.EmployeeName, a.Position, b.PosName  
   FROM HrEmployee a WITH (NOLOCK, NOWAIT)  
   LEFT JOIN gnMstPosition b   
  ON b.CompanyCode = a.CompanyCode  
    AND b.DeptCode = 'SALES'  
    AND b.PosCode = a.Position   
  WHERE a.CompanyCode = @CompanyCode  
    AND a.Position = 'S' -- Penambahan  
  ORDER BY EmployeeID  
END  
  
END  
  