USE [BITPKU]
GO
/****** Object:  StoredProcedure [dbo].[usprpt_SvRpMst003]    Script Date: 1/15/2015 4:37:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<MASTER STALL>
-- =============================================
ALTER procedure [dbo].[usprpt_SvRpMst003]    
  @CompanyCode  varchar(15),
  @BranchCode varchar(15)    
as    
    
begin    
    
select  
 a.StallCode  
,a.Description  
,case a.HaveLift when 1 then 'Ya' else 'Tidak' end as HaveLift  
,case a.IsActive when 1 then 'Aktif' else 'Tidak Aktif' end as IsActive  
from svMstStall a    
  where a.CompanyCode =@CompanyCode and a.BranchCode = @BranchCode
end

