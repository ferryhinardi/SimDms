
/****** Object:  StoredProcedure [dbo].[uspfn_spGetReturnSupplySlip]    Script Date: 6/19/2014 11:22:04 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_spGetReturnSupplySlip] @CompanyCode varchar(15), @BranchCode varchar(15), @TypeOfGoods varchar(2)  
as  
SELECT ReturnNo, ReturnDate   
FROM spTrnSRTurSSHdr   
WHERE Status IN (0,1) AND  
    spTrnSRTurSSHdr.CompanyCode = @CompanyCode AND  
    spTrnSRTurSSHdr.BranchCode = @BranchCode AND  
    spTrnSRTurSSHdr.TypeOfGoods = @TypeOfGoods  
ORDER BY ReturnNo DESC  