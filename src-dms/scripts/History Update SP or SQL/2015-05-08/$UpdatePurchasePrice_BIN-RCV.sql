-- IMPORTANT:  DATABASE YANG DI UPDATE ADALAH SMGSMR..
--			   JIKA INGIN MENG-UPDATE DATABASE YANG LAIN HARAP DIUBAH DATABASENYA
--             DENGAN CARA REPLACE ALL DARI SMGSMR KE xyz

begin transaction
 -- declare @Date datetime = '2015/05/05'
 -- BINNING
	;with bin as (select * from smgsmr..spTrnPBinnDtl d
				   where exists (select 1 from smgsmr..spTrnPBinnHdr
								  where CompanyCode =d.CompanyCode
									and BranchCode  =d.BranchCode
									and BinningNo   =d.BinningNo
									and CreatedBy   ='POSTING'
									and LastUpdateBy='POSTING'))
								  --and CreatedDate =@Date)
				   --and PurchasePrice=CostPrice)
	update bin set PurchasePrice = (select PurchasePrice from smgsmr..spTrnPPOSDtl
									 where CompanyCode=bin.CompanyCode
									   and BranchCode =bin.BranchCode
									   and POSNo      =bin.DocNo
									   and PartNo     =bin.PartNo)
	;with bin as (select * from smgsmr..spTrnPBinnDtl d
				   where exists (select 1 from smgsmr..spTrnPBinnHdr
								  where CompanyCode =d.CompanyCode
									and BranchCode  =d.BranchCode
									and BinningNo   =d.BinningNo
									and CreatedBy   ='POSTING'
									and LastUpdateBy='POSTING'))
								  --and CreatedDate =@Date))
    select * from bin order by CompanyCode, BranchCode, DocNo

 -- RECEIVING
	;with wrs as (select * from smgsmr..spTrnPRcvDtl d
				   where exists (select 1 from smgsmr..spTrnPRcvHdr
								  where CompanyCode =d.CompanyCode
									and BranchCode  =d.BranchCode
									and WRSNo       =d.WRSNo
									and CreatedBy   ='POSTING'
									and LastUpdateBy='POSTING'))
								  --and CreatedDate =@Date)
				   --and PurchasePrice=CostPrice)
	update wrs set PurchasePrice = (select PurchasePrice from smgsmr..spTrnPPOSDtl
									 where CompanyCode=wrs.CompanyCode
									   and BranchCode =wrs.BranchCode
									   and POSNo      =wrs.DocNo
									   and PartNo     =wrs.PartNo)
	;with wrs as (select * from smgsmr..spTrnPRcvDtl d
				   where exists (select 1 from smgsmr..spTrnPRcvHdr
								  where CompanyCode =d.CompanyCode
									and BranchCode  =d.BranchCode
									and WRSNo       =d.WRSNo
									and CreatedBy   ='POSTING'
									and LastUpdateBy='POSTING'))
   								  --and CreatedDate =@Date))
    select * from wrs order by CompanyCode, BranchCode, WRSNo

commit transaction

/*
select * from smgsmr..spTrnPHPP     where HPPNo    ='HPP/15/001089'
select * from smgsmr..spTrnPRcvHdr  where WRSNo    ='WRL/15/001089'
select * from smgsmr..spTrnPBinnHdr where BinningNo='BNL/15/001091'
select * from smgsmr..spTrnPPOSHdr  where POSNo    ='PO401/15/001049'

select * from smgsmr..spTrnPRcvDtl  where WRSNo    ='WRL/15/001089'
select * from smgsmr..spTrnPBinnDtl where BinningNo='BNL/15/001091'
select * from smgsmr..spTrnPPOSDtl  where POSNo    ='PO401/15/001049'
*/
