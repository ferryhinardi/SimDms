alter procedure uspfn_SpkeSummary
    @DateFrom varchar(50),
    @DateTo   varchar(50),
    @InqType  varchar(10) = 1  -- 1:Inquiry, 2:Spk
as

select (select COUNT(*) Total 
          from SuzukiR4..msMstGroupModel g, SimDms..pmKDPExhibition e 
         where g.ModelType=e.TipeKendaraan and g.GroupModel='ERTIGA' 
           and (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end)
               between @DateFrom and @DateTo) as 'Ertiga'
     , (select COUNT(*) Total 
          from SuzukiR4..msMstGroupModel g, SimDms..pmKDPExhibition e 
         where g.ModelType=e.TipeKendaraan and g.GroupModel='WAGON R' 
           and (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end)
               between @DateFrom and @DateTo) as 'KarimunWgn'
     , (select COUNT(*) Total 
          from SuzukiR4..msMstGroupModel g, SimDms..pmKDPExhibition e 
         where g.ModelType=e.TipeKendaraan and g.GroupModel='WAGON R' and e.Variant='GS' 
           and (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end)
               between @DateFrom and @DateTo) as 'KarimunWgnGs'
     , (select COUNT(*) Total 
          from SuzukiR4..msMstGroupModel g, SimDms..pmKDPExhibition e 
         where g.ModelType=e.TipeKendaraan and g.GroupModel in ('SL415-PU','ST100-PU') 
           and (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end)
               between @DateFrom and @DateTo) as 'PuFutura'
     , (select COUNT(*) Total 
          from SuzukiR4..msMstGroupModel g, SimDms..pmKDPExhibition e 
         where g.ModelType=e.TipeKendaraan and g.GroupModel='APV-PU' 
           and (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end)
               between @DateFrom and @DateTo) as 'PuMegaCarry'
     , (select COUNT(*) Total 
          from SuzukiR4..msMstGroupModel g, SimDms..pmKDPExhibition e 
         where g.ModelType=e.TipeKendaraan and g.GroupModel not in ('ERTIGA','WAGON R','SL415-PU','ST100-PU','APV-PU')
           and (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end)
               between @DateFrom and @DateTo) as 'Others'
     , (select COUNT(*) Total 
          from SuzukiR4..msMstGroupModel g, SimDms..pmKDPExhibition e 
         where g.ModelType=e.TipeKendaraan 
           and (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end)
               between @DateFrom and @DateTo) as AllModels
 

-- Total SPK per Day & Accumulation PTD (Period to Date)   
select distinct TrxDate = (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end)
     , TotalPerDay = (select COUNT(*) from SimDms..pmKDPExhibition 
                       where (case when @InqType=1 then CONVERT(date,  InquiryDate) else CONVERT(date,  SpkDate) end)
                           = (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end))
     , TotalAccum  = ((select COUNT(*) from SimDms..pmKDPExhibition 
                        where (case when @InqType=1 then CONVERT(date,  InquiryDate) else CONVERT(date,  SpkDate) end)
                           <= (case when @InqType=1 then CONVERT(date,e.InquiryDate) else CONVERT(date,e.SpkDate) end)))
  from SimDms..pmKDPExhibition e
 where (case when @InqType=1 then CONVERT(date,InquiryDate) else CONVERT(date,SpkDate) end) between @DateFrom and @DateTo
 order by (case when @InqType=1 then CONVERT(date,InquiryDate) else CONVERT(date,SpkDate) end)

go

exec uspfn_SpkeSummary '2014-01-01', '2014-09-12', 2
