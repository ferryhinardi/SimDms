SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		David Leonardo
-- Create date: 23 October 2014
-- Description:	Log Header
-- Sementara ini baru dipakai di Pembelian - Entry Order
-- =============================================
CREATE PROCEDURE uspfn_spLogHeader
	(
	@DataID varchar(20),
	@CustomerCode varchar(20),
	@ProductType varchar(20),
	@Status varchar(10),
	@CreatedDate datetime,
	@Header varchar(max)
	)
AS
BEGIN
declare 	
@ID numeric(18,0), 
@Range numeric(18,0)

	 select @ID = (select isnull(max(id), 0) from gnDcsUploadFile), @Range = 9000000000
	 if (@ID < @range)
	 set @ID = @Range + 1
     else
	 set @ID = @ID + 1

     insert into gnDcsUploadFile
           ( ID, DataID, CustomerCode, ProductType, Status, CreatedDate, Header)
     values(@ID,@DataID,@CustomerCode,@ProductType,@Status,@CreatedDate,@Header)
END
GO




