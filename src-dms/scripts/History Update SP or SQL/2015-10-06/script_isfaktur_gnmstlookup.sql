Declare @compCode varchar(25)

set @compCode = (SELECT top 1 CompanyCode FROM gnMstCompanyMapping) 

IF @compCode <> ''
BEGIN
	IF (SELECT Count(*) FROM gnMstLookUpHdr WHERE CompanyCode = @compCode AND CodeID='IsFaktur') = 0
	BEGIN 
		Insert into gnMstLookUpHdr(CompanyCode, CodeID, CodeName, FieldLength, CreatedBy, CreatedDate, isLocked) 
		VALUES(@compCode,'IsFaktur','IsFaktur','5','SYSTEM',GETDATE(),0)
		Insert into gnMstLookUpDtl(CompanyCode, CodeID, LookUpValue, SeqNo, ParaValue, LookUpValueName, CreatedBy, CreatedDate)
		VALUES(@compCode,'IsFaktur','1',1,'1','IsFaktur','SYSTEM',GETDATE()) 
	END
END 