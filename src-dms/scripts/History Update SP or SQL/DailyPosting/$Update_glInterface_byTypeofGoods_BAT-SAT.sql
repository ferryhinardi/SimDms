-- Update glInterface by Type of Goods
-- -----------------------------------
-- Created by   : HTO, 21 May 2015
-- Process time : Depok, 25 May 2015
-- Database MD  : SDMS
-- Database SD  : SAT
-- -----------------------------------

BEGIN TRANSACTION
BEGIN TRY

BEGIN 
		declare @StartDate			varchar(10)   = '2015/05/01'
		declare @EndDate			varchar(10)   = '2015/05/20'
		declare @curCompanyCode		varchar(15)   = ''
		declare @curBranchCode		varchar(15)   = ''
		declare @curDocNo			varchar(15)   = ''
		declare @curTPGO			varchar(15)   = ''
		declare @Company			varchar(15)   = ''
		declare @Branch 			varchar(15)   = ''
		declare @HPPNo				varchar(15)   = ''
		declare @BatchNo			varchar(15)   = ''
		declare @AccSales			varchar(50)   = ''
		declare @AccDisc			varchar(50)   = ''
		declare @AccCoGS			varchar(50)   = ''
		declare @AccInv				varchar(50)   = ''
		declare @AccInvSD			varchar(50)   = ''
		declare @Sales				numeric(18,0) = 0
		declare @Disc				numeric(18,0) = 0
		declare @CoGS				numeric(18,0) = 0
		declare @Inventory			numeric(18,0) = 0
		declare @SeqNo				integer       = 0
		declare @SeqNoSD			integer       = 0
		declare @DocDate			datetime      = @StartDate
		declare @CreateDate		    datetime	  = @StartDate
		declare @LastUpdateDate		datetime	  = @StartDate

		select * into sdms..glInterface_25May2015_RecoveryTPGO
			from (select * from sdms..glInterface 
			       where left(DocNo,3)='FPJ' and CreateBy='POSTING'
						 and convert(varchar,DocDate,111) between @StartDate and @EndDate) a
		select * into sat..glInterface_25May2015_RecoveryTPGO
			from (select * from sat..glInterface 
			       where left(DocNo,3)='HPP' and CreateBy='POSTING'
						 and convert(varchar,DocDate,111) between @StartDate and @EndDate) b

        declare curGL cursor for
			select CompanyCode, BranchCode, DocNo, count(*) TPGO
				from (select distinct gl.CompanyCode, gl.BranchCode, gl.DocNo, it.TypeOfGoods
						from sdms..glInterface gl
					   inner join sdms..spTrnSFPJDtl fp
					 		   on fp.CompanyCode=gl.CompanyCode
							  and fp.BranchCode =gl.BranchCode
							  and fp.FPJNo      =gl.DocNo
					   inner join sdms..spTrnSORDHdr so
						       on so.CompanyCode=fp.CompanyCode
							  and so.BranchCode =fp.BranchCode
							  and so.DocNo      =fp.DocNo
					 		  and left(LockingBy,3)<>'INV'
					   inner join sdms..spMstItems it
							   on it.CompanyCode=fp.CompanyCode
							  and it.BranchCode =fp.BranchCode
							  and it.PartNo     =fp.PartNo
					   where left(gl.DocNo,3)='FPJ'
						 and gl.SeqNo      =1
						 and CreateBy      ='POSTING'
						 and convert(varchar,gl.DocDate,111) between @StartDate and @EndDate) x
			   group by CompanyCode, BranchCode, DocNo
			  having count(*) > 1
			   order by CompanyCode, BranchCode, DocNo desc
		open curGL

		fetch next from curGL
			  into @curCompanyCode, @curBranchCode, @curDocNo, @curTPGO

		while (@@FETCH_STATUS =0)
			begin
				select @DocDate=DocDate, @CreateDate=CreateDate, @LastUpdateDate=LastUpdateDate
				  from sdms..glInterface 
				 where CompanyCode=@curCompanyCode 
				   and BranchCode =@curBranchCode
				   and DocNo      =@curDocNo
				   and SeqNo      =1
				delete sdms..glInterface 
				 where CompanyCode=@curCompanyCode 
				   and BranchCode =@curBranchCode
				   and DocNo      =@curDocNo
				   and SeqNo not in (1,3)

				select * into #t1
					from (select TPGO=i.TypeOfGoods, 
								 AccSales=a.SalesAccNo, AccDisc=a.DiscAccNo, AccCoGS=a.COGSAccNo, AccInv=a.InventoryAccNo, 
								 AR=h.TotFinalSalesAmt, TaxOut=h.TotPpnAmt, Sales=sum(d.SalesAmt), Disc=sum(d.DiscAmt), 
								 CoGS=sum(d.QtyBill*d.CostPrice), Inventory=sum(d.QtyBill*d.CostPrice)
							from sdms..spTrnSFPJHdr h
						   inner join sdms..spTrnSFPJDtl d
								   on d.CompanyCode=h.CompanyCode
								  and d.BranchCode =h.BranchCode
								  and d.FPJNo      =h.FPJNo
						    left join sdms..spMstItems i
								   on i.CompanyCode=d.CompanyCode
								  and i.PartNo     =d.PartNo
						    left join sdms..spMstAccount a
								   on a.CompanyCode=i.CompanyCode
								  and a.BranchCode =i.BranchCode
								  and a.TypeOfGoods=i.TypeOfGoods
						   where h.CompanyCode     =@curCompanyCode
						     and h.BranchCode      =@curBranchCode
							 and h.FPJNo           =@curDocNo
						   group by i.TypeOfGoods, a.SalesAccNo, a.DiscAccNo, a.COGSAccNo, 
								    a.InventoryAccNo, h.TotFinalSalesAmt, h.TotPpnAmt
						 ) y

				select * into #t2
					from (select Company=h.CompanyCode, Branch=h.BranchCode, HPPNo=h.HPPNo, BatchNo=g.BatchNo, TPGO=i.TypeOfGoods, 
					             AccInv=a.InventoryAccNo, Inventory=sum(d.ReceivedQty*d.PurchasePrice*(100-d.DiscPct)/100)
							from sat..spTrnPHPP h
						   inner join sat..sptrnprcvdtl d
								   on d.CompanyCode=h.CompanyCode
								  and d.BranchCode =h.BranchCode
								  and d.WRSNo      =h.WRSNo
						   inner join sat..glInterface g
								   on g.CompanyCode=h.CompanyCode
								  and g.BranchCode =h.BranchCode
								  and g.DocNo      =h.HPPNo
								  and g.SeqNo      =1
						    left join sat..spMstItems i
								   on i.CompanyCode=d.CompanyCode
								  and i.PartNo     =d.PartNo
						    left join sat..spMstAccount a
								   on a.CompanyCode=i.CompanyCode
								  and a.BranchCode =i.BranchCode
								  and a.TypeOfGoods=i.TypeOfGoods
						   where h.ReferenceNo     =@curDocNo
						     and h.ReferenceDate   =@DocDate
						   group by h.CompanyCode, h.BranchCode, h.HPPNo, g.BatchNo, i.TypeOfGoods, a.InventoryAccNo
						 ) z

				delete sat..glInterface 
				 where SeqNo=1
				   and exists (select top 1 1 from #t2
				                where Company=sat..glInterface.CompanyCode
								  and Branch =sat..glInterface.BranchCode
								  and HPPNo  =sat..glInterface.DocNo)

				set @SeqNo   = 3
				set @SeqNoSD = 3

				if (select 1 from #t1 where TPGO = '0') = 1
					begin
						set @SeqNo = @SeqNo + 1
						select @AccSales=AccSales, @Sales=Sales, @AccDisc=AccDisc, @Disc=Disc, @AccCoGS=AccCoGs, 
							   @CoGS=CoGS, @AccInv=AccInv, @Inventory=Inventory from #t1 where TPGO = '0'
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
							       @AccSales, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Sales, 'SALES', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							  
						if @Disc > 0
							begin
								set @SeqNo = @SeqNo + 1
								insert into sdms..glInterface 
									values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
									       @AccDisc, 'SPAREPART', 'INVOICE', @curDocNo, @Disc, 0, 'DISC1', '', '', 0, 
										   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							end
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccCoGS, 'SPAREPART', 'INVOICE', @curDocNo, @CoGS, 0, 'COGS', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Inventory, 'INVENTORY',  '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNoSD = @SeqNoSD + 1
						select @Company=Company, @Branch=Branch, @HPPNo=HPPNo, @BatchNo=BatchNo, @AccInv=AccInv, 
							   @Inventory=Inventory from #t2 where TPGO = '0'
						insert into sat..glInterface 
							values(@Company, @Branch, @HPPNo, @SeqNoSD, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'PURCHASE', @HPPNo, @Inventory, 0, 'INVENTORY', @BatchNo, '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
					end

				if (select 1 from #t1 where TPGO = '1') = 1
					begin
						set @SeqNo = @SeqNo + 1
						select @AccSales=AccSales, @Sales=Sales, @AccDisc=AccDisc, @Disc=Disc, @AccCoGS=AccCoGs, 
							   @CoGS=CoGS, @AccInv=AccInv, @Inventory=Inventory from #t1 where TPGO = '1'
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
							       @AccSales, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Sales, 'SALES', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							  
						if @Disc > 0
							begin
								set @SeqNo = @SeqNo + 1
								insert into sdms..glInterface 
									values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
									       @AccDisc, 'SPAREPART', 'INVOICE', @curDocNo, @Disc, 0, 'DISC1', '', '', 0, 
										   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							end
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccCoGS, 'SPAREPART', 'INVOICE', @curDocNo, @CoGS, 0, 'COGS', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Inventory, 'INVENTORY',  '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNoSD = @SeqNoSD + 1
						select @Company=Company, @Branch=Branch, @HPPNo=HPPNo, @BatchNo=BatchNo, @AccInv=AccInv, 
							   @Inventory=Inventory from #t2 where TPGO = '1'
						insert into sat..glInterface 
							values(@Company, @Branch, @HPPNo, @SeqNoSD, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'PURCHASE', @HPPNo, @Inventory, 0, 'INVENTORY', @BatchNo, '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
					end

				if (select 1 from #t1 where TPGO = '2') = 1
					begin
						set @SeqNo = @SeqNo + 1
						select @AccSales=AccSales, @Sales=Sales, @AccDisc=AccDisc, @Disc=Disc, @AccCoGS=AccCoGs, 
							   @CoGS=CoGS, @AccInv=AccInv, @Inventory=Inventory from #t1 where TPGO = '2'
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
							       @AccSales, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Sales, 'SALES', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							  
						if @Disc > 0
							begin
								set @SeqNo = @SeqNo + 1
								insert into sdms..glInterface 
									values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
									       @AccDisc, 'SPAREPART', 'INVOICE', @curDocNo, @Disc, 0, 'DISC1', '', '', 0, 
										   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							end
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccCoGS, 'SPAREPART', 'INVOICE', @curDocNo, @CoGS, 0, 'COGS', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Inventory, 'INVENTORY',  '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNoSD = @SeqNoSD + 1
						select @Company=Company, @Branch=Branch, @HPPNo=HPPNo, @BatchNo=BatchNo, @AccInv=AccInv, 
							   @Inventory=Inventory from #t2 where TPGO = '2'
						insert into sat..glInterface 
							values(@Company, @Branch, @HPPNo, @SeqNoSD, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'PURCHASE', @HPPNo, @Inventory, 0, 'INVENTORY', @BatchNo, '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
					end

				if (select 1 from #t1 where TPGO = '3') = 1
					begin
						set @SeqNo = @SeqNo + 1
						select @AccSales=AccSales, @Sales=Sales, @AccDisc=AccDisc, @Disc=Disc, @AccCoGS=AccCoGs, 
							   @CoGS=CoGS, @AccInv=AccInv, @Inventory=Inventory from #t1 where TPGO = '3'
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
							       @AccSales, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Sales, 'SALES', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							  
						if @Disc > 0
							begin
								set @SeqNo = @SeqNo + 1
								insert into sdms..glInterface 
									values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
									       @AccDisc, 'SPAREPART', 'INVOICE', @curDocNo, @Disc, 0, 'DISC1', '', '', 0, 
										   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							end
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccCoGS, 'SPAREPART', 'INVOICE', @curDocNo, @CoGS, 0, 'COGS', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Inventory, 'INVENTORY',  '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNoSD = @SeqNoSD + 1
						select @Company=Company, @Branch=Branch, @HPPNo=HPPNo, @BatchNo=BatchNo, @AccInv=AccInv, 
							   @Inventory=Inventory from #t2 where TPGO = '3'
						insert into sat..glInterface 
							values(@Company, @Branch, @HPPNo, @SeqNoSD, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'PURCHASE', @HPPNo, @Inventory, 0, 'INVENTORY', @BatchNo, '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
					end

				if (select 1 from #t1 where TPGO = '4') = 1
					begin
						set @SeqNo = @SeqNo + 1
						select @AccSales=AccSales, @Sales=Sales, @AccDisc=AccDisc, @Disc=Disc, @AccCoGS=AccCoGs, 
							   @CoGS=CoGS, @AccInv=AccInv, @Inventory=Inventory from #t1 where TPGO = '4'
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
							       @AccSales, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Sales, 'SALES', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							  
						if @Disc > 0
							begin
								set @SeqNo = @SeqNo + 1
								insert into sdms..glInterface 
									values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
									       @AccDisc, 'SPAREPART', 'INVOICE', @curDocNo, @Disc, 0, 'DISC1', '', '', 0, 
										   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
							end
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccCoGS, 'SPAREPART', 'INVOICE', @curDocNo, @CoGS, 0, 'COGS', '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNo = @SeqNo + 1
						insert into sdms..glInterface 
							values(@curCompanyCode, @curBranchCode, @curDocNo, @SeqNo, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'INVOICE', @curDocNo, 0, @Inventory, 'INVENTORY',  '', '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
						set @SeqNoSD = @SeqNoSD + 1
						select @Company=Company, @Branch=Branch, @HPPNo=HPPNo, @BatchNo=BatchNo, @AccInv=AccInv, 
							   @Inventory=Inventory from #t2 where TPGO = '4'
						insert into sat..glInterface 
							values(@Company, @Branch, @HPPNo, @SeqNoSD, @DocDate, '300', @DocDate, 
								   @AccInv, 'SPAREPART', 'PURCHASE', @HPPNo, @Inventory, 0, 'INVENTORY', @BatchNo, '', 0, 
								   'POSTING.', @CreateDate, 'POSTING', @LastUpdateDate)
					end

				drop table #t1
				drop table #t2
				--select * from sdms..glInterface 
				-- where CompanyCode=@curCompanyCode and BranchCode=@curBranchCode and DocNo=@curDocNo 
				-- order by CompanyCode, BranchCode, DocNo, SeqNo
				--select * from sat..glInterface 
				-- where CompanyCode=@curCompanyCode and BranchCode=@curBranchCode and DocNo=@curDocNo 
				-- order by CompanyCode, BranchCode, DocNo, SeqNo

			 -- Read next record
				fetch next from curGL
					into @curCompanyCode, @curBranchCode, @curDocNo, @curTPGO
			end 

		close curGL
		deallocate curGL

		--select * from sdms..glInterface where CreateBy='POSTING.'
		--select * from sat..glInterface where CreateBy='POSTING.'
END
END TRY

BEGIN CATCH
    --select ERROR_NUMBER()    AS ErrorNumber,    ERROR_SEVERITY() AS ErrorSeverity, ERROR_STATE()   AS ErrorState,
		  -- ERROR_PROCEDURE() AS ErrorProcedure, ERROR_LINE()     AS ErrorLine    , ERROR_MESSAGE() AS ErrorMessage
	if @@TRANCOUNT > 0
		begin
			select 'Recovery glInterface gagal !!!   ' + ERROR_MESSAGE() [INFO]
			rollback transaction
			return
		end
END CATCH

IF @@TRANCOUNT > 0
	begin
		select 'Recovery glInterface sukses !!!' [INFO]
		rollback transaction
		--commit transaction
	end
