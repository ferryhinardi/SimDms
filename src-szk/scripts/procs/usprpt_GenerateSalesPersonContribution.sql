
/****** Object:  StoredProcedure [dbo].[usprpt_GenerateSalesPersonContribution]    Script Date: 9/16/2014 8:25:21 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[usprpt_GenerateSalesPersonContribution] @Area varchar(50), @CompanyCode varchar(50), @BranchCode varchar(50), @Filter varchar(50), @DateFrom smalldatetime, @DateTo smalldatetime
as

--set @Filter = case @Filter when 'InquiryDate' then 'a.'+ @Filter
--							when 'DoDate' then 'b.'+ @Filter end

declare @sqlstr varchar(max)
set @sqlstr ='
					select 
					Dodate [Do Date],
					[DeliveryDate] [Delivery Date], 
					cast(InquiryNumber as varchar(50)) [Inquiry No.] , 
					[InquiryDate] [Inquiry Date],
					[SPKDate] [SPK Date],
					AreaDealer [Area], 
					CompanyCode [Company Code], 
					DealerAbbr [Dealer Name], 
					OutletAbbreviation [Outlet], 
					tipekendaraan [Tipe Kendaraan], 
					Variant, 
					Transmisi,
					ColourCode [Colour Code], 
					Wiraniaga, 
					SalesCoordinator [Sales Coordinator], 
					SalesHead [Sales Head],
					BranchHead [Branch Head], 
					isnull((select top 1 LookUpValueName
							from (select top 1 LookUpValueName, codeid, LookUpValue from gnMstLookUpDtl) x
								inner join (select Grade, EmployeeName  from HrEmployee) y
									on x.codeid=''ITSG'' and x.LookUpValue=y.Grade and a.Wiraniaga=y.EmployeeName),
							(select top 1 LookUpValueName
								from (select EmployeeID, Grade from pmSlsGradeHistory)  x 
								inner join ( select top 1 codeid, LookUpValue, LookUpValueName from gnMstLookUpDtl) y
									on y.codeid=''ITSG'' and y.LookUpValue=x.Grade
								inner join (select EmployeeID, CompanyCode, BranchCode, InquiryNumber from pmKDP) z
									on x.EmployeeID=z.EmployeeID and a.CompanyCode=z.CompanyCode and a.BranchCode=z.BranchCode and a.InquiryNumber=z.InquiryNumber
							)) [Grading Wiraniaga], 
					quantityinquiry [Quantity Inquiry], 
					perolehandata [Perolehan Data], 
					testdrive [Test Drive], 
					CaraPembayaran [Cara Pembayaran], 
					leasing [Leasing], 
					downpayment [Down Payment], 
					Tenor, 
					NAMAprospek [Prospect Name], 
					alamatprospek [Prospect Address], 
					case when telprumah = '''' then ''''  else  '''''''' + telprumah end  [Phone], 
					UpdateDate [Next Follow Up Date], LastProgress [Last Progress],
					namaperusahaan [Company Prospect Name], 
					alamatperusahaan [Company Prospect Address], 
					case when Handphone='''' then '''' else '''''''' + Handphone end Handphone,
					merklain [Merk Lain] 
				from (select dodate, a.CompanyCode, a.BranchCode, case when year(Deliverydate) <= ''1900'' then null else Deliverydate end [DeliveryDate], 
						a.InquiryNumber, case when year(InquiryDate) <= ''1900'' then null else InquiryDate end  [InquiryDate], 
						case when year(SPKDAte) <= ''1900'' then null else SPKDAte end  [SPKDate],  
						tipekendaraan, Variant, Transmisi, ColourCode, wiraniaga, SalesCoordinator, SalesHead,branchhead, quantityinquiry, perolehandata, testdrive, CaraPembayaran,						leasing, downpayment, Tenor, NAMAprospek, alamatprospek, TelpRumah, namaperusahaan,
						alamatperusahaan, Handphone, merklain, a.LastProgress, c.UpdateDate, AreaDealer, GroupNo, 
						(select DealerAbbr from DealerGroupMapping where DealerCode=a.CompanyCode) DealerAbbr,
						(select OutletAbbreviation from gnMstDealerOutletMapping where DealerCode=a.CompanyCode and OutletCode=a.BranchCode) OutletAbbreviation
						from Pmhstits a 
							inner join (select CompanyCode, BranchCode, InquiryNumber, SequenceNo, UpdateDate, LastProgress from pmStatusHistory ) c
								on a.CompanyCode=c.CompanyCode and a.BranchCode=c.BranchCode and a.InquiryNumber=c.InquiryNumber 
								and c.SequenceNo=(select max(SequenceNo) from pmStatusHistory where inquirynumber=c.InquiryNumber and CompanyCode=c.CompanyCode)
							inner join (	select f.CompanyCode, f.BranchCode, ProspectNo, f.SONo, DODate
											from omTrSalesSO f with(nolock, nowait)
												inner join (select CompanyCode, BranchCode, SONo, DONo, DODate from OmTrSalesDO with(nolock, nowait)) g 
													on f.CompanyCode=g.CompanyCode and f.BranchCode=g.BranchCode and f.SONo=g.SONo ) b
								on a.CompanyCode=b.CompanyCode and a.BranchCode=b.BranchCode and a.InquiryNumber=b.ProspectNo
							inner join (select DealerCode , AreaDealer, GroupNo from CompaniesGroupMappingView) h
								on a.CompanyCode=h.DealerCode
						where 1=1
						and '+@Filter+' between '''+cast (@DateFrom as varchar(50))+''' and '''+cast(@DateTo as varchar(50))+'''
				) a 
				where 1=1
				and Groupno=case when isnull('''+@Area+''','''')<>'''' then '''+@Area+''' else Groupno end 
				and CompanyCode = case when isnull('''+@CompanyCode+''','''')<>'''' then '''+@CompanyCode+''' else CompanyCode end
				and BranchCode= case when isnull('''+@BranchCode+''','''')<>'''' then '''+@BranchCode+''' else BranchCode end
				order by AreaDealer, DealerAbbr, BranchCode, InquiryNumber

'
--select @sqlstr
exec (@sqlstr)


			


			

					