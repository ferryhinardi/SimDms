ALTER procedure [dbo].[uspfn_SvTrnListKsgFromSPKPerJobNo]
--DECLARE
 @CompanyCode varchar(15),  
 @ProductType varchar(15),   
 @BranchFrom varchar(15),  
 @BranchTo varchar(15),  
 @PeriodFrom datetime,  
 @PeriodTo datetime,  
 @JobPDI as varchar(15),  
 @JobFSC as varchar(15),
 @BranchCode varchar(15),
 @BranchCodePar varchar(max),
 @JobOrderPar varchar(max)
as        

--select @CompanyCode = '6115204001',@ProductType='2W',@BranchFrom='6115204101', @BranchTo='6115204125',@PeriodFrom='2015-05-01',
--@PeriodTo='2015-05-27',@JobPDI='True',@JobFSC='False',@BranchCode='6115204101', 
--@BranchCodePar='''6115204102'',''6115204105'',''6115204106'',''6115204107'',''6115204109'',''6115204110'',''6115204111'',''6115204112'',''6115204116'',''6115204117'',''6115204124''',
--@JobOrderPar='''SPK/15/005185'',''SPK/15/005186'',''SPK/15/005187'',''SPK/15/005188'',''SPK/15/005189'',''SPK/15/005194'',''SPK/15/005195'',''SPK/15/005203'',''SPK/15/005204'',''SPK/15/005206'',''SPK/15/005208'',''SPK/15/005209'',''SPK/15/005210'',''SPK/15/005211'',''SPK/15/005214'',''SPK/15/005220'',''SPK/15/005222'',''SPK/15/005223'',''SPK/15/005225'',''SPK/15/005229'',''SPK/15/005231'',''SPK/15/005246'',''SPK/15/005249'',''SPK/15/005386'',''SPK/15/005387'',''SPK/15/005388'',''SPK/15/005389'',''SPK/15/005390'',''SPK/15/005391'',''SPK/15/005392'',''SPK/15/005393'',''SPK/15/005394'',''SPK/15/005395'',''SPK/15/005396'',''SPK/15/005397'',''SPK/15/005398'',''SPK/15/005399'',''SPK/15/005400'',''SPK/15/005449'',''SPK/15/005450'',''SPK/15/005451'',''SPK/15/005452'',''SPK/15/005453'',''SPK/15/005454'',''SPK/15/005455'',''SPK/15/005456'',''SPK/15/005457'',''SPK/15/005458'',''SPK/15/005459'',''SPK/15/005460'',''SPK/15/005461'',''SPK/15/005462'',''SPK/15/005463'',''SPK/15/005464'',''SPK/15/005465'',''SPK/15/005466'',''SPK/15/005467'',''SPK/15/005468'',''SPK/15/005469'',''SPK/15/005470'',''SPK/15/005471'',''SPK/15/005472'',''SPK/15/005473'',''SPK/15/005474'',''SPK/15/005475'',''SPK/15/005476'',''SPK/15/005477'',''SPK/15/005478'',''SPK/15/005479'',''SPK/15/005480'',''SPK/15/005481'',''SPK/15/005482'',''SPK/15/005484'',''SPK/15/005485'',''SPK/15/005486'',''SPK/15/005488'',''SPK/15/005489'',''SPK/15/005490'',''SPK/15/005491'',''SPK/15/005492'',''SPK/15/005493'',''SPK/15/005494'',''SPK/15/002680'',''SPK/15/002708'',''SPK/15/002709'',''SPK/15/002710'',''SPK/15/002711'',''SPK/15/002714'',''SPK/15/002716'',''SPK/15/002717'',''SPK/15/002718'',''SPK/15/002719'',''SPK/15/002722'',''SPK/15/002723'',''SPK/15/002725'',''SPK/15/002726'',''SPK/15/002727'',''SPK/15/002728'',''SPK/15/002729'',''SPK/15/002730'',''SPK/15/002732'',''SPK/15/002734'',''SPK/15/002735'',''SPK/15/002736'',''SPK/15/002737'',''SPK/15/002738'',''SPK/15/002739'',''SPK/15/002740'',''SPK/15/001692'',''SPK/15/001696'',''SPK/15/001697'',''SPK/15/001756'',''SPK/15/001783'',''SPK/15/001822'',''SPK/15/001879'',''SPK/15/001882'',''SPK/15/001883'',''SPK/15/001374'',''SPK/15/001375'',''SPK/15/001380'',''SPK/15/001381'',''SPK/15/001382'',''SPK/15/001385'',''SPK/15/001388'',''SPK/15/001389'',''SPK/15/001390'',''SPK/15/001391'',''SPK/15/001392'',''SPK/15/001393'',''SPK/15/001394'',''SPK/15/001395'',''SPK/15/001396'',''SPK/15/001398'',''SPK/15/001399'',''SPK/15/001400'',''SPK/15/001401'',''SPK/15/001402'',''SPK/15/001403'',''SPK/15/001404'',''SPK/15/001446'',''SPK/15/001447'',''SPK/15/001161'',''SPK/15/001163'',''SPK/15/001164'',''SPK/15/001166'',''SPK/15/001168'',''SPK/15/001171'',''SPK/15/001172'',''SPK/15/001173'',''SPK/15/001174'',''SPK/15/001177'',''SPK/15/001178'',''SPK/15/001179'',''SPK/15/001180'',''SPK/15/001181'',''SPK/15/001182'',''SPK/15/001209'',''SPK/15/001210'',''SPK/15/001212'',''SPK/15/001213'',''SPK/15/001214'',''SPK/15/001216'',''SPK/15/001218'',''SPK/15/001219'',''SPK/15/001220'',''SPK/15/001262'',''SPK/15/001263'',''SPK/15/001270'',''SPK/15/001271'',''SPK/15/001272'',''SPK/15/001273'',''SPK/15/001274'',''SPK/15/001276'',''SPK/15/001277'',''SPK/15/001278'',''SPK/15/001279'',''SPK/15/001280'',''SPK/15/001281'',''SPK/15/001282'',''SPK/15/001283'',''SPK/15/001310'',''SPK/15/001311'',''SPK/15/001312'',''SPK/15/001313'',''SPK/15/001314'',''SPK/15/001315'',''SPK/15/001316'',''SPK/15/001317'',''SPK/15/001318'',''SPK/15/001319'',''SPK/15/001320'',''SPK/15/001327'',''SPK/15/001328'',''SPK/15/001329'',''SPK/15/001330'',''SPK/15/001957'',''SPK/15/001958'',''SPK/15/001970'',''SPK/15/001971'',''SPK/15/001972'',''SPK/15/001973'',''SPK/15/002021'',''SPK/15/002023'',''SPK/15/002032'',''SPK/15/002033'',''SPK/15/002034'',''SPK/15/002035'',''SPK/15/002036'',''SPK/15/002039'',''SPK/15/002040'',''SPK/15/002041'',''SPK/15/002042'',''SPK/15/002044'',''SPK/15/002045'',''SPK/15/002046'',''SPK/15/002047'',''SPK/15/002048'',''SPK/15/002049'',''SPK/15/002050'',''SPK/15/002051'',''SPK/15/002052'',''SPK/15/002053'',''SPK/15/002054'',''SPK/15/002055'',''SPK/15/002056'',''SPK/15/002057'',''SPK/15/002059'',''SPK/15/002060'',''SPK/15/002061'',''SPK/15/002065'',''SPK/15/002066'',''SPK/15/002067'',''SPK/15/002068'',''SPK/15/002069'',''SPK/15/002070'',''SPK/15/002071'',''SPK/15/002072'',''SPK/15/002073'',''SPK/15/002110'',''SPK/15/002111'',''SPK/15/001566'',''SPK/15/001567'',''SPK/15/001681'',''SPK/15/001682'',''SPK/15/001683'',''SPK/15/001684'',''SPK/15/001691'',''SPK/15/001692'',''SPK/15/001693'',''SPK/15/001694'',''SPK/15/001695'',''SPK/15/001696'',''SPK/15/001697'',''SPK/15/001698'',''SPK/15/001699'',''SPK/15/001700'',''SPK/15/001701'',''SPK/15/001702'',''SPK/15/001703'',''SPK/15/001704'',''SPK/15/001705'',''SPK/15/001706'',''SPK/15/001707'',''SPK/15/001708'',''SPK/15/001770'',''SPK/15/001771'',''SPK/15/001772'',''SPK/15/001773'',''SPK/15/001774'',''SPK/15/001775'',''SPK/15/001099'',''SPK/15/001100'',''SPK/15/001101'',''SPK/15/001102'',''SPK/15/001103'',''SPK/15/001104'',''SPK/15/001105'',''SPK/15/001106'',''SPK/15/001107'',''SPK/15/001108'',''SPK/15/001109'',''SPK/15/001111'',''SPK/15/001112'',''SPK/15/001113'',''SPK/15/001114'',''SPK/15/001115'',''SPK/15/001116'',''SPK/15/001117'',''SPK/15/001118'',''SPK/15/001119'',''SPK/15/001120'',''SPK/15/001121'',''SPK/15/001122'',''SPK/15/001123'',''SPK/15/001124'',''SPK/15/001125'',''SPK/15/001126'',''SPK/15/001127'',''SPK/15/001128'',''SPK/15/001129'',''SPK/15/001130'',''SPK/15/001131'',''SPK/15/001132'',''SPK/15/001133'',''SPK/15/001134'',''SPK/15/001135'',''SPK/15/001136'',''SPK/15/001137'',''SPK/15/001138'',''SPK/15/001139'',''SPK/15/001140'',''SPK/15/001141'',''SPK/15/001174'',''SPK/15/001175'',''SPK/15/001218'',''SPK/15/001219'',''SPK/15/002237'',''SPK/15/002238'',''SPK/15/002239'',''SPK/15/002240'',''SPK/15/002242'',''SPK/15/002243'',''SPK/15/002244'',''SPK/15/002245'',''SPK/15/002246'',''SPK/15/002247'',''SPK/15/002248'',''SPK/15/002249'',''SPK/15/002250'',''SPK/15/002252'',''SPK/15/002253'',''SPK/15/002254'',''SPK/15/002255'',''SPK/15/002256'',''SPK/15/002257'',''SPK/15/002258'',''SPK/15/002259'',''SPK/15/001075'',''SPK/15/001076'',''SPK/15/001077'',''SPK/15/001078'',''SPK/15/001079'',''SPK/15/001080'',''SPK/15/001081'',''SPK/15/001082'',''SPK/15/001083'',''SPK/15/001084'',''SPK/15/001087'',''SPK/15/001096'',''SPK/15/001097'',''SPK/15/001098'',''SPK/15/001099'',''SPK/15/001100'',''SPK/15/001101'',''SPK/15/001167'',''SPK/15/000001'''

declare @IsCentralize as varchar(1)
set @IsCentralize = '0'
if(select ParaValue from gnMstLookUpDtl where CompanyCode= @CompanyCode and CodeID='SRV_FLAG' and LookUpValue='KSG_HOLDING') > '0'
	set @IsCentralize = '1'
	 
select * into #t1 from(  
select  
    convert(bit, 1) Process      
 , srv.BranchCode  
 , srv.JobOrderNo  
 , case when convert(varchar, srv.JobOrderDate, 106) = '19000101' then '' else convert(varchar, srv.JobOrderDate, 106) end JobOrderDate  
 , srv.BasicModel  
 , srv.ServiceBookNo  
 , job.PdiFscSeq  
 , srv.Odometer  
 , srv.LaborGrossAmt  
 , round((select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F'),0) MaterialGrossAmt --Pembulatan
 , round((srv.LaborGrossAmt + (select isnull(SUM((SupplyQty - ReturnQty) * RetailPrice), 0) from svTrnSrvItem where BranchCode = srv.BranchCode and ServiceNo = srv.ServiceNo and BillType = 'F')),0) PdiFscAmount  --Pembulatan
 , isnull(case when convert(varchar, veh.FakturPolisiDate, 112) = '19000101' then '' else convert(varchar, veh.FakturPolisiDate, 106) end, '')  FakturPolisiDate  
 , isnull(case when convert(varchar, mstVeh.BPKDate, 112) = '19000101' then '' else convert(varchar, mstVeh.BPKDate, 106) end, '')  BPKDate  
 , srv.ChassisCode  
 , srv.ChassisNo  
 , srv.EngineCode  
 , srv.EngineNo   
    , srv.InvoiceNo  
 , isnull(inv.FPJNo, '') FPJNo  
 , isnull(case when convert(varchar, inv.FPJDate, 112) = '19000101' then '' else convert(varchar, inv.FPJDate, 106) end, '')  FPJDate  
 , isnull(fpj.FPJGovNo, '') FPJGovNo  
 , srv.TransmissionType  
 , srv.ServiceStatus  
 , srv.CompanyCode  
 , srv.ProductType  
from svTrnService srv  
left join svMstJob job  
 on job.CompanyCode = srv.CompanyCode  
  and job.ProductType = srv.ProductType  
  and job.BasicModel = srv.BasicModel  
  and job.JobType = srv.JobType  
left join svMstCustomerVehicle veh  
 on veh.CompanyCode = srv.CompanyCode  
  and veh.ChassisCode = srv.ChassisCode  
  and veh.ChassisNo = srv.ChassisNo  
left join omMstVehicle mstVeh  
 on mstVeh.CompanyCode = srv.CompanyCode  
  and mstVeh.ChassisCode = srv.ChassisCode  
  and mstVeh.ChassisNo = srv.ChassisNo  
left join svTrnInvoice inv  
 on inv.CompanyCode = srv.CompanyCode  
  and inv.BranchCode = srv.BranchCode  
  and inv.ProductType = srv.ProductType  
  and inv.InvoiceNo = srv.InvoiceNo  
left join svTrnFakturPajak fpj  
 on fpj.CompanyCode = srv.CompanyCode  
  and fpj.BranchCode = srv.BranchCode  
  and fpj.FPJNo = inv.FPJNo  
where   
 srv.CompanyCode = @CompanyCode  
 and srv.BranchCode between @BranchFrom and @BranchTo  
 and srv.ProductType = @ProductType  
 --and srv.isLocked = 0  
 and job.GroupJobType = 'FSC'  
 and ((job.GroupJobType like @JobFSC and job.PdiFscSeq > 0 )  or (job.JobType like @JobPDI and job.PdiFscSeq=0))
 and convert(varchar, srv.JobOrderDate, 112) between convert(varchar, @PeriodFrom, 112) and convert(varchar, @PeriodTo, 112)   
 --and  not exists (  
 -- select 1   
 -- from svTrnPdiFscApplication   
 -- where CompanyCode=srv.CompanyCode  
 --  and (case when @IsCentralize = '0' then BranchCode end) = srv.BranchCode   
 --  and InvoiceNo=srv.JobOrderNo  
 --  and ProductType=srv.ProductType					
 and  not exists (  
  select 1   
  from svTrnPdiFscApplication   
  where CompanyCode=srv.CompanyCode  
   and BranchCode = (case when @IsCentralize = '0' then srv.BranchCode  else @BranchCode end)
   and InvoiceNo=srv.JobOrderNo  
   and ProductType=srv.ProductType  
 )--)
) #t1  
declare @sql as varchar(max)
set @sql =
'select   
row_number() over (order by #t1.BranchCode, #t1.JobOrderNo) No,  
* from #t1   
where ServiceStatus=5 
and BranchCode in (' + @BranchCodePar + ')
and JobOrderNo in (' + @JobOrderPar + ')
order by BranchCode, JobOrderNo'  
exec(@sql)
drop table #t1
