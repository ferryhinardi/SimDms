if object_id('uspfn_SvTrnServiceSelectPesananPekerjaanLuar') is not null
	drop procedure uspfn_SvTrnServiceSelectPesananPekerjaanLuar
GO
CREATE procedure [dbo].[uspfn_SvTrnServiceSelectPesananPekerjaanLuar]
	@CompanyCode varchar(15),
	@BranchCode  varchar(15),
	@ProductType  varchar(15),
	@ShowAll bit
AS
-- EXEC uspfn_SvTrnServiceSelectPesananPekerjaanLuar '6156401000', '6156401001', '4W', '0'

declare @Query varchar(max)
declare @Condition varchar(4000);

set @Condition = '';
if(@ShowAll = 0) begin
	set @Condition = ' AND svTrnService.ServiceStatus IN (0,1,2,3,4) 
		AND srvTask.PONo = '''' AND srvTask.ServiceNo IS NOT NULL AND srvTask.IsSubCon = 1 ';
end 

set @Query = '
SELECT DISTINCT 
    svTrnService.InvoiceNo
    , svTrnService.ServiceNo
    , svTrnService.ServiceType
    , ForemanID
    , EstimationNo
    , EstimationDate
    , BookingNo
    , BookingDate
    , svTrnService.JobOrderNo
    , svTrnService.JobOrderDate
    , svTrnService.PoliceRegNo
    , ServiceBookNo
    , svTrnService.BasicModel
    , TransmissionType
    , svTrnService.ChassisCode
    , svTrnService.ChassisNo
    , svTrnService.EngineCode
    , svTrnService.EngineNo
    , svTrnService.ChassisCode + '' '' + cast(svTrnService.ChassisNo as  varchar) KodeRangka
    , svTrnService.EngineCode + '' '' + cast(svTrnService.EngineNo as varchar) KodeMesin
    , ColorCode
    , (svTrnService.CustomerCode + '' - '' + gnMstCustomer.CustomerName) as Customer
    , (svTrnService.CustomerCodeBill + '' - '' + custBill.CustomerName) as CustomerBill
    , svTrnService.CustomerCode
    , svTrnService.CustomerCodeBill
    , svTrnService.Odometer
    , svTrnService.JobType
    , case when svTrnService.ServiceStatus=''4'' then
            case when ''' + @ProductType + '''=''4W'' then reffService.Description
                else reffService.LockingBy
            end
        else reffService.Description 
    end ServiceStatus
    --, svTrnService.PoliceRegNo
	--, svTrnService.CustomerCode
    , InsurancePayFlag
    , InsuranceOwnRisk
    , InsuranceNo
    , InsuranceJobOrderNo
    --, svTrnService.CustomerCodeBill
    , svTrnService.LaborDiscPct
    , PartDiscPct
    , svTrnService.MaterialDiscPct
    , svTrnService.PPNPct
    , svTrnService.ServiceRequestDesc
    , ConfirmChangingPart
    --, ForemanID
    , svTrnService.MechanicID
    , EstimateFinishDate
    , svTrnService.LaborDPPAmt
    , svTrnService.PartsDPPAmt
    , svTrnService.MaterialDPPAmt
    , TotalDPPAmount
    , TotalPpnAmount
    , TotalPphAmount
    , TotalSrvAmount
    , employee.EmployeeName
    , (custBill.Address1 + '''' + custBill.Address2 + '''' + custBill.Address3 + '''' + custBill.Address4) as AddressBill
    , custBill.NPWPNo
    , custBill.PhoneNo
    , custBill.HPNo
FROM svTrnService WITH(NOLOCK, NOWAIT)
LEFT JOIN gnMstCustomer 
    ON gnMstCustomer.CompanyCode = svTrnService.CompanyCode 
    AND gnMstCustomer.CustomerCode = svTrnService.CustomerCode
LEFT JOIN gnMstCustomer custBill 
    ON custBill.CompanyCode = svTrnService.CompanyCode
    AND custBill.CustomerCode = svTrnService.CustomerCodeBill
LEFT JOIN gnMstEmployee employee
    ON employee.CompanyCode = svTrnService.CompanyCode
    AND employee.BranchCode = svTrnService.BranchCode
	AND employee.EmployeeID = svTrnService.ForemanID
LEFT JOIN svTrnSrvItem srvItem 
    ON srvItem.CompanyCode = svTrnService.CompanyCode
    AND srvItem.BranchCode = svTrnService.BranchCode
    AND srvItem.ProductType = svTrnService.ProductType
    AND srvItem.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svTrnSrvTask srvTask
    ON srvTask.CompanyCode = svTrnService.CompanyCode
    AND srvTask.BranchCode = svTrnService.BranchCode
    AND srvTask.ProductType = svTrnService.ProductType
    AND srvTask.ServiceNo = svTrnService.ServiceNo
LEFT JOIN svMstRefferenceService reffService
    ON reffService.CompanyCode = svTrnService.CompanyCode
    AND reffService.ProductType = svTrnService.ProductType    
    AND reffService.RefferenceCode = svTrnService.ServiceStatus
    AND reffService.RefferenceType = ''SERVSTAS''
LEFT JOIN svTrnInvoice invoice
	ON invoice.CompanyCode = svTrnService.CompanyCode
	AND invoice.BranchCode = svTrnService.BranchCode
	AND invoice.ProductType = svTrnService.ProductType
	AND invoice.JobOrderNo = svTrnService.JobOrderNo
LEFT JOIN svTrnSrvVOR VOR
    ON VOR.CompanyCode = svTrnService.CompanyCode
	AND VOR.BranchCode = svTrnService.BranchCode
    AND VOR.ServiceNo = svTrnService.ServiceNo
WHERE svTrnService.CompanyCode = ''' + @CompanyCode + '''
    AND svTrnService.BranchCode = ''' + @BranchCode + '''
 AND svTrnService.ServiceType = ''2''' + @Condition+ ' ORDER BY svTrnService.JobOrderNo DESC';

EXEC (@Query)
GO
