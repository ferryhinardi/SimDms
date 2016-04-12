USE [DMS_V2]
GO
/****** Object:  StoredProcedure [dbo].[uspfn_GetSPKForUnApprovalPdiFsc]    Script Date: 3/3/2015 5:03:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[uspfn_GetSPKForUnApprovalPdiFsc]
	@CompanyCode varchar(15),
	@BranchCode varchar(15),
	@ProductType varchar(15),
	@IsPdi bit
AS
BEGIN
	--declare @CompanyCode as varchar(15)
	--declare @BranchCode as varchar(15)
	--declare @ProductType as varchar(15)
	--declare @IsPDI as bit

	--set @CompanyCode = '6058401'
	--set @BranchCode = '605840103'
	--set @ProductType = '4W'
	--set @IsPDI = 0
	
	select convert(bit, 0) IsSelected, row_number() over(order by a.ServiceNo asc) as No,a.BranchCode, a.ServiceNo, a.JobOrderNo, a.JobOrderDate
	, a.ServiceBookNo, a.ChassisNo, a.BasicModel, a.JobType, (isnull(srv.ValItem, 0) + isnull(task.valTask,0)) TotalApprove
	from SvTrnService a
	left join (select CompanyCode, BranchCode, ServiceNo, sum((OperationHour * OperationCost)) valTask
		from svTrnSrvTask
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) task on task.CompanyCode = a.CompanyCode
											and task.BranchCode = a.BranchCode
											and task.ServiceNo = a.ServiceNo
	left join (select CompanyCode, BranchCode, ServiceNo, sum(((SupplyQty - ReturnQty) * RetailPrice)) valItem 
		from svTrnSrvItem 
		where BillType = 'F'
		group by CompanyCode, BranchCode, ServiceNo) srv on srv.CompanyCode = a.CompanyCode
											and srv.BranchCode = a.BranchCode
											and srv.ServiceNo = a.ServiceNo
	where a.CompanyCode = @CompanyCode
	and a.BranchCode = @BranchCode
	and a.ProductType = @ProductType
	and a.ServiceStatus = '5 '
	and a.IsLocked = 1
	and a.JobType like (case when @IsPDI = 1 then 'PDI%' else 'FS%' end )
END
