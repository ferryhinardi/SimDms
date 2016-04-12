alter procedure uspfn_WhInqDataList
	@CompanyCode varchar(20),
	@DataType varchar(20),
	@Status varchar(20)
as

select UniqueID, CompanyCode, DataType, Segment, LastSendDate, Status, CreatedDate, UpdatedDate
  from GnMstScheduleData
 where CompanyCode = @CompanyCode
   and DataType = case isnull(@DataType, '') when '' then DataType else @DataType end 
   and Status = case isnull(@Status, '') when '' then Status else @Status end 
