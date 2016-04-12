
DECLARE @CompanyCode varchar(15) = '0000000',
		@CodeID varchar(15) = 'IVLD'

BEGIN TRAN

DELETE FROM gnMstLookupHdr WHERE CodeID = @CodeID
DELETE FROM gnMstLookUpDtl WHERE CodeID = @CodeID

INSERT INTO gnMstLookupHdr
VALUES (@CompanyCode, @CodeID, 'Invalid Employee Data', 3, 1, 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE(), 0, NULL, NULL)

INSERT INTO gnMstLookUpDtl VALUES (@CompanyCode, @CodeID, '1', 1, '1', 'Tgl Resign <= Tgl Join', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE())
INSERT INTO gnMstLookUpDtl VALUES (@CompanyCode, @CodeID, '2', 2, '2', 'Tgl Mutasi Pertama < Tgl Join', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE())
INSERT INTO gnMstLookUpDtl VALUES (@CompanyCode, @CodeID, '3', 3, '3', 'Tgl Assign Pertama < Tgl Join', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE())
INSERT INTO gnMstLookUpDtl VALUES (@CompanyCode, @CodeID, '4', 4, '4', 'Tgl Resign < Tgl Mutasi Terakhir', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE())
INSERT INTO gnMstLookUpDtl VALUES (@CompanyCode, @CodeID, '5', 5, '5', 'Tgl Resign < Tgl Assign Terakhir', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE())
INSERT INTO gnMstLookUpDtl VALUES (@CompanyCode, @CodeID, '6', 6, '6', 'Tidak ada tanggal Mutasi', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE())
INSERT INTO gnMstLookUpDtl VALUES (@CompanyCode, @CodeID, '7', 7, '7', 'Tidak ada tanggal Assign', 'SYSTEM', GETDATE(), 'SYSTEM', GETDATE())

SELECT * FROM gnMstLookUpHdr WHERE CodeID = @CodeID
SELECT * FROM gnMstLookUpDtl WHERE CodeID = @CodeID

COMMIT TRAN
--ROLLBACK TRAN



