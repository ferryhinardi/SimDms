USE [SimDms]
GO

/****** Object:  StoredProcedure [dbo].[uspfn_BpkbReminderOutstanding]    Script Date: 4/30/2015 1:54:18 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON

Create procedure [dbo].[uspfn_MonitoringDealer]
	@ProductType nvarchar(20),
	@DealerCode varchar(20),
	@IsWhat varchar(200)
as
	
	select a.DealerCode, b.DealerName, a.OutletCode, c.OutletName, a.UserName, a.LastLogin, a.LastUpdated
	from sysLastTrnInfo a
	inner join (select DealerCode, DealerName, ProductType from dealerinfo) b
		on a.DealerCode = b.DealerCode
	inner join (select DealerCode, OutletCode, OutletName from gnMstDealerOutletMapping) c
		on a.DealerCode = c.DealerCode and a.OutletCode = c.OutletCode
	where b.ProductType = case when @ProductType = '' then b.ProductType else @ProductType end  
	and a.DealerCode = case when @DealerCode = '' then a.DealerCode else @DealerCode end 
	and a.ModuleName = case when @IsWhat = '' then a.ModuleName else @IsWhat end

GO


