
delete gnMstLookUpDtl where CodeID = 'ORTP' And LookUpValue = '8'
insert gnMstLookUpDtl values (( (SELECT top 1 CompanyCode FROM gnMstCoProfile)),'ORTP','8',6,'0','AOS (AUTOMATIC ORDER SPAREPART)','SYSTEM',getdate(),'SYSTEM',getdate())
