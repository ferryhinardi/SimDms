
/****** Object:  StoredProcedure [dbo].[sp_spMstMovingCodeView]    Script Date: 4/1/2014 10:59:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER procedure [dbo].[sp_spMstMovingCodeView]  (@CompanyCode varchar(15))
 as
SELECT MovingCode, MovingCodeName,
Formula = Cast(Param1 as varchar(10)) + Sign1 + Variable + Sign2 + Cast(Param2 as varchar(10)),
Param1, Sign1, Variable, Param2, Sign2,CompanyCode
FROM spMstMovingCode
where CompanyCode=@CompanyCode






