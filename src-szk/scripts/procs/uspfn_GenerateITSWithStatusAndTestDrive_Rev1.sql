alter procedure [dbo].[uspfn_GenerateITSWithStatusAndTestDrive_Rev1]                        
 @StartDate varchar(8),                        
 @EndDate varchar(8)                        
as                        
begin                        
 --exec uspfn_GenerateITSWithStatusAndTestDrive_Rev1 '20150801', '20151031'          
 --exec uspfn_GenerateITSWithStatusAndTestDrive '20150101', '20150131'                      
 --declare @StartDate varchar(8)                        
 --declare @EndDate   varchar(8)                        
                        
 --set @StartDate = '20141201'                        
 --set @EndDate   = '20141201'                        
                        
 select * into #INQ                        
   from ( select CompanyCode, BranchCode, InqDate,CityId,CityName, TipeKendaraan, Variant, ColourCode, count(InqDate) INQ,                         
     sum(InqTestDrive) InqTestDrive, 0 Lost                        
   from ( select h.CompanyCode, h.BranchCode, h.TipeKendaraan,isnull(h.Variant,'') Variant, isnull(h.ColourCode,'') ColourCode,                        
         convert(varchar,InquiryDate,112) InqDate,                         
         InqTestDrive = case when isnull(h.TestDrive,'') = '10' then 1 else 0 end,                        
         Lost = isnull((select count(*) from pmStatusHistory x                        
             where x.CompanyCode  =h.CompanyCode                        
            and x.BranchCode   =h.BranchCOde                        
            and x.InquiryNumber=h.InquiryNumber                        
            and x.LastProgress ='LOST'                        
            and not exists (select top 1 1 from pmStatusHistory                        
                 where CompanyCode  =x.CompanyCode                        
                and BranchCode   =x.BranchCode                        
                and InquiryNumber=x.InquiryNumber                        
                and LastProgress ='SPK')                        
            ),0),isnull(h.CityId,'') CityId,      
            isnull((select top 1 LookUpValueName from gnmstlookupdtl where LookUpValue=h.cityid and  CodeId='City'),'')  CityName        
       from  pmKdp h                       
      where convert(varchar,InquiryDate,112) between @StartDate and @EndDate                      
    ) a                        
      group by CompanyCode, BranchCode, InqDate,cityid,cityname, TipeKendaraan, Variant, ColourCode                         
     ) #INQ                        
                        
 select * into #SPK                        
   from ( select CompanyCode, BranchCode, SPKDate, CityId,CityName,TipeKendaraan, Variant, ColourCode, count(SPKDate) SPK,                         
     sum(SPKTestDrive) SPKTestDrive, sum(Lost) Lost                        
      from ( select h.CompanyCode, h.BranchCode, h.TipeKendaraan, isnull(h.Variant,'') Variant, isnull( h.ColourCode,'') ColourCode,                        
        convert(varchar,s.UpdateDate,112) SPKDate,                         
        SPKTestDrive = case when isnull(h.TestDrive,'') = '10' then 1 else 0 end,                        
        Lost = isnull((select count(*) from pmStatusHistory                        
            where CompanyCode  =s.CompanyCode                        
              and BranchCode   =s.BranchCOde                        
              and InquiryNumber=s.InquiryNumber                        
              and LastProgress ='LOST'),0)            
              ,isnull(h.CityId,'') CityId,       
              isnull((select top 1 LookUpValueName from gnmstlookupdtl where LookUpValue=h.cityid and  CodeId='City'),'')  CityName        
       from pmKdp h inner join pmStatusHistory s                
        on h.CompanyCode=s.CompanyCode                        
       and h.BranchCode=s.BranchCode                        
       and h.InquiryNumber=s.InquiryNumber                        
       and s.LastProgress='SPK'                             and convert(varchar,s.UpdateDate,112) between @StartDate and @EndDate                       
     ) a                        
     group by CompanyCode, BranchCode, SPKDate,cityid, cityname, TipeKendaraan, Variant, ColourCode                        
     ) #SPK         
                        
                        
                        
 select * into #HP                        
   from ( select CompanyCode, BranchCode, HPDate, CityId,CityName,TipeKendaraan, Variant, ColourCode, count(HPDate) HP,                         
     sum(HPTestDrive) HPTestDrive, sum(Lost) Lost                        
      from ( select h.CompanyCode, h.BranchCode, h.TipeKendaraan, isnull(h.Variant,'') Variant, h.ColourCode,                        
        convert(varchar,s.UpdateDate,112) HPDate,                         
        HPTestDrive = case when isnull(h.TestDrive,'') = '10' then 1 else 0 end,                        
        Lost = isnull((select count(*) from pmStatusHistory                        
            where CompanyCode  =s.CompanyCode                        
              and BranchCode   =s.BranchCOde                        
              and InquiryNumber=s.InquiryNumber                        
              and LastProgress ='LOST'),0)            
              ,isnull(h.CityId,'') CityId,       
              isnull((select top 1 LookUpValueName from gnmstlookupdtl where LookUpValue=h.cityid and  CodeId='City'),'')  CityName        
      from pmKdp h inner join pmStatusHistory s                        
        on h.CompanyCode=s.CompanyCode                        
       and h.BranchCode=s.BranchCode                        
       and h.InquiryNumber=s.InquiryNumber                        
       and s.LastProgress='HP'                        
       and convert(varchar,s.UpdateDate,112) between @StartDate and @EndDate                             
     ) a                        
     group by CompanyCode, BranchCode, HPDate,cityid, cityname, TipeKendaraan, Variant, ColourCode                        
     ) #HP                        
                        
                        
 select * into #QRY                        
   from (select distinct CompanyCode, BranchCode, InqDate,CityId,CityName, TipeKendaraan, Variant, ColourCode,                        
       0 INQ, 0 InqTestDrive,0 HP,0 HPTestDrive, 0 SPK, 0 SPKTestDrive, 0 Lost                        
 from #INQ) #QRY                        
                        
 insert into #QRY                        
   select distinct CompanyCode, BranchCode, SPKDate InqDate,CityId,CityName, TipeKendaraan, Variant, ColourCode,                          
       0 INQ, 0 InqTestDrive, 0 HP,0 HPTestDrive,0 SPK, 0 SPKTestDrive, 0 Lost                        
     from #SPK                        
    where not exists (select 1 from #INQ                        
        where CompanyCode=#SPK.CompanyCode                        
          and BranchCode = #SPK.BranchCode                        
          and InqDate=#SPK.SPKDate            
          and CityId=#SPK.CityId                      
          and TipeKendaraan=#SPK.TipeKendaraan                        
          and Variant=#SPK.Variant                        
          and ColourCode=#SPK.ColourCode)                        
                        
                        
                        
 Update #QRY                          
    set INQ          = isnull((select sum(INQ) from #INQ                        
           where CompanyCode  =#QRY.CompanyCode                        
          and BranchCode   = #QRY.BranchCode                        
          and InqDate      =#QRY.InqDate                        
          and TipeKendaraan=#QRY.TipeKendaraan                        
          and CityId=isnull(#QRY.CityId ,'')                        
          and isnull(Variant,'')      =isnull(#QRY.Variant,'')            
          and isnull(ColourCode,'')=isnull(#QRY.ColourCode,'')),0),                        
     InqTestDrive = isnull((select sum(InqTestDrive) from #INQ                  
           where CompanyCode  =#QRY.CompanyCode       
             and BranchCode   = #QRY.BranchCode                        
          and InqDate      =#QRY.InqDate                        
          and TipeKendaraan=#QRY.TipeKendaraan           
          and CityId=#QRY.CityId                       
          and isnull(Variant,'')      =isnull(#QRY.Variant,'')                    
          and ColourCode   =#QRY.ColourCode),0),                        
     Lost         = isnull((select sum(Lost) from #INQ                        
           where CompanyCode  =#QRY.CompanyCode                        
             and BranchCode   = #QRY.BranchCode                        
          and InqDate      =#QRY.InqDate                        
          and TipeKendaraan=#QRY.TipeKendaraan              
          and CityId=#QRY.CityId                    
        and isnull(Variant,'')      =isnull(#QRY.Variant,'')            
         and ColourCode   =#QRY.ColourCode),0)                        
  where exists (select 1 from #INQ                        
     where CompanyCode  =#QRY.CompanyCode                        
       and BranchCode   = #QRY.BranchCode                        
       and InqDate      =#QRY.InqDate                     
       and CityId=#QRY.CityId             
       and TipeKendaraan=#QRY.TipeKendaraan                        
       and Variant      =#QRY.Variant                        
       and ColourCode   =#QRY.ColourCode)                        
                        
                        
 Update #QRY                        
    set SPK          = (select SUM(SPK) from #SPK                        
       where CompanyCode  =#QRY.CompanyCode                        
          and BranchCode   = #QRY.BranchCode                        
         and SPKDate      =#QRY.InqDate                        
         and TipeKendaraan=#QRY.TipeKendaraan              
         and CityId=isnull(#QRY.CityId ,'')                   
       and isnull(Variant,'')      =isnull(#QRY.Variant,'')                        
         and isnull(ColourCode,'')   =isnull(#QRY.ColourCode,'')),                        
     SPKTestDrive = (select SUM(SPKTestDrive) from #SPK                        
       where CompanyCode  =#QRY.CompanyCode                
          and BranchCode   = #QRY.BranchCode                        
         and SPKDate      =#QRY.InqDate                        
         and TipeKendaraan=#QRY.TipeKendaraan            
         and CityId=#QRY.CityId                      
       and isnull(Variant,'')      =isnull(#QRY.Variant,'')            
         and ColourCode   =#QRY.ColourCode),                        
     Lost         = (select SUM(Lost) from #SPK                        
       where CompanyCode  =#QRY.CompanyCode                        
    and BranchCode   = #QRY.BranchCode                        
         and SPKDate      =#QRY.InqDate                        
         and TipeKendaraan=#QRY.TipeKendaraan             
         and CityId=#QRY.CityId                     
         and isnull(Variant,'')      =isnull(#QRY.Variant,'')             
         and ColourCode   =#QRY.ColourCode)                        
  where exists (select 1 from #SPK                        
     where CompanyCode  =#QRY.CompanyCode                        
        and BranchCode   = #QRY.BranchCode                        
       and SPKDate      =#QRY.InqDate                        
       and TipeKendaraan=#QRY.TipeKendaraan              
       and CityId=#QRY.CityId                    
      and isnull(Variant,'')      =isnull(#QRY.Variant,'')                 
       and ColourCode   =#QRY.ColourCode)                        
                             
                             
                      
 Update #QRY                        
    set HP          = (select SUM(HP) from #HP                      
    where CompanyCode  =#QRY.CompanyCode                        
          and BranchCode   = #QRY.BranchCode             
         and HPDate      =#QRY.InqDate                        
         and TipeKendaraan=#QRY.TipeKendaraan              
         and CityId=isnull(#QRY.CityId ,'')                     
         and isnull(Variant,'')      =isnull(#QRY.Variant,'')              
         and ColourCode   =#QRY.ColourCode),                        
     HPTestDrive = (select SUM(HPTestDrive) from #HP                        
       where CompanyCode  =#QRY.CompanyCode                        
          and BranchCode   = #QRY.BranchCode                        
         and HPDate      =#QRY.InqDate                        
         and TipeKendaraan=#QRY.TipeKendaraan          
         and CityId=#QRY.CityId                        
         and isnull(Variant,'')      =isnull(#QRY.Variant,'')                     
         and ColourCode   =#QRY.ColourCode),                        
     Lost         = (select SUM(Lost) from #HP                        
       where CompanyCode  =#QRY.CompanyCode                        
          and BranchCode = #QRY.BranchCode                        
         and HPDate      =#QRY.InqDate                        
         and TipeKendaraan=#QRY.TipeKendaraan             
         and CityId=#QRY.CityId                     
         and isnull(Variant,'')      =isnull(#QRY.Variant,'')                        
         and ColourCode   =#QRY.ColourCode)                        
  where exists (select 1 from #HP                        
     where CompanyCode  =#QRY.CompanyCode                        
        and BranchCode   = #QRY.BranchCode                        
     and HPDate      =#QRY.InqDate                        
       and TipeKendaraan=#QRY.TipeKendaraan             
       and CityId=#QRY.CityId                     
       and isnull(Variant,'')      =isnull(#QRY.Variant,'')                   
       and ColourCode   =#QRY.ColourCode)                        
                             
                             
                             
                              
select Area,Dealer, isnull(Abbr, '') Abbr,OutletCode,OutletAbbreviation, Date, CityId,CityName,Model, Var, Col, ColourName,                        
     sum(INQ) INQ, sum(InqTestDrive) InqTestDrive, sum(HP) HP,sum(HPTestDrive) HPTestDrive,sum(SPK) SPK, sum(SPKTestDrive) SPKTestDrive,                        
     sum(LOST) LOST from(                        
 select distinct dm.Area,q.CompanyCode Dealer,  dm.DealerAbbreviation Abbr,q.BranchCode OutletCode,b.OutletAbbreviation, q.InqDate Date, CityId,CityName,                        
     isnull(m.GroupModel,q.TipeKendaraan) Model, q.Variant Var, q.ColourCode Col, isnull(c.RefferenceDesc1,'') ColourName,                        
     sum(isnull(q.INQ,0)) INQ, sum(isnull(q.InqTestDrive,0)) InqTestDrive,                         
     sum(isnull(q.HP,0)) HP, sum(isnull(q.HPTestDrive,0)) HPTestDrive,                         
     sum(isnull(q.SPK,0)) SPK, sum(isnull(q.SPKTestDrive,0)) SPKTestDrive,                        
     sum(isnull(q.Lost,0)) LOST                      
   from #QRY q                           
   left join gnMstDealerOutletMapping b on b.DealerCode=q.CompanyCode and b.OutletCode = q.BranchCode                        
   left join gnMstDealerMapping dm on  dm.DealerCode = q.CompanyCode and dm.GroupNo = b.GroupNo   
   left join msMstGroupModel m                        
    on m.ModelType=q.TipeKendaraan                        
   left join omMstRefference c                        
    on c.RefferenceType='COLO'                        
   and c.RefferenceCode=q.ColourCode                        
   group by dm.area,q.CompanyCode,dm.DealerAbbreviation,b.OutletCode, b.OutletAbbreviation,q.InqDate,q.cityid,q.cityname,                       
      isnull(m.GroupModel,q.TipeKendaraan), q.Variant, q.ColourCode, isnull(c.RefferenceDesc1,''), q.BranchCode, b.GroupNo                        
      ) t1                        
   group by Area,Dealer, Abbr,OutletCode,OutletAbbreviation, Date, CityId,CityName,Model, Var, Col, ColourName                        
  order by Abbr, Date, Model, Var, Col                        
                     
                        
                        
       
                        
                        
 drop table #INQ, #SPK, #HP, #QRY                       
                         
                        
end 