
/****** Object:  StoredProcedure [dbo].[uspfn_spTrnIWHTrfDtlview]    Script Date: 6/6/2014 9:37:03 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
  Create PROCEDURE [dbo].[uspfn_spTrnIReservedDtlview]
@CompanyCode varchar(15),
@BranchCode varchar(15),
@ReservedNo  varchar(25) 
 
AS
 


select  a.PartNo ,b.PartName  ,a.AvailableQty ,a.ReservedQty
  from spTrnIReservedDtl a
inner join spMstItemInfo b

on a.PartNo=b.PartNo
where  a.CompanyCode=@CompanyCode and a.ReservedNo =@ReservedNo   and a.BranchCode=@BranchCode