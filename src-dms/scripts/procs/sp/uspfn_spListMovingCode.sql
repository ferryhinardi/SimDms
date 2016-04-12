
/****** Object:  StoredProcedure [dbo].[uspfn_spListMovingCode]    Script Date: 6/19/2014 10:58:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[uspfn_spListMovingCode]
@CompanyName varchar(15)
AS
SELECT MovingCode, MovingCodeName,
Formula = Cast(Param1 as varchar(10)) + Sign1 + Variable + Sign2 + Cast(Param2 as varchar(10)),
Param1, Sign1, Variable, Param2, Sign2
FROM spMstMovingCode
WHERE CompanyCode=@CompanyName