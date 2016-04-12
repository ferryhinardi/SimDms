ALTER procedure [dbo].[uspfn_SyncPmFakturPolisi]
as

insert into SuzukiR4..OmHstInquirySalesNSDS
    select *, 0 from SDMSDOM4W..DOM4W.NSDSFPOL a
    where (Year(DO_Date) >= 2010 or YEAR(PROCESS_DATE) >= 2010)
        and left(Chassis_Code,5)<>'MHMUD'
        and not exists (select 1 from SuzukiR4..OmHstInquirySalesNSDS
                        where a.Chassis_Code = ChassisCode
                            and a.Chassis_No   = ChassisNo)
						  --and left(ChassisNo, 1) != 'P')

insert into SimDms..omHstInquirySalesNSDSSJ
    select * from SDMSDOM4W..DOM4W.NSDSSJ a
     where not exists (select 1 from SimDms..omHstInquirySalesNSDSSJ
                        where VIN=a.VIN
                          and CUSTOMER_CODE=a.CUSTOMER_CODE)

-- update DODate & FPolDate in table OmHstInquirySales
Update SuzukiR4..OmHstInquirySales
   set SuzukiDODate   = (select Top 1 DODate from SuzukiR4..OmHstInquirySalesNSDS
                          where OmHstInquirySales.ChassisCode = ChassisCode
                            and OmHstInquirySales.ChassisNo   = ChassisNo
							and left(ChassisNo, 1)           != 'P'
                            --and OmHstInquirySales.CompanyCode = CustomerCode
                            and OmHstInquirySales.CompanyCode = case when CustomerCode = '6015402' then '6015401' else 
                                                                case when CustomerCode = '6051402' then '6051401' else
                                                                         CustomerCode 
                                                                     end 
                                                                end 
                                      order by DODate desc)
     , SuzukiFPolDate = (select Top 1 ProcessDate
                           from SuzukiR4..OmHstInquirySalesNSDS
                          where OmHstInquirySales.ChassisCode = ChassisCode
                            and OmHstInquirySales.ChassisNo   = ChassisNo
							and left(ChassisNo, 1)           != 'P'
                            --and OmHstInquirySales.CompanyCode = CustomerCode
                            and OmHstInquirySales.CompanyCode = case when CustomerCode = '6015402' then '6015401' else 
                                                                case when CustomerCode = '6051402' then '6051401' else
                                                                         CustomerCode 
                                                                     end 
                                                                end
                                      order by ProcessDate desc)
where (isnull(SuzukiDODate,'') = '' or isnull(SuzukiFPolDate,'') = ''
    or  year(SuzukiDODate)=1900 or year(SuzukiFPolDate)=1900)
   and exists (select top 1 ChassisCode from SuzukiR4..OmHstInquirySalesNSDS
                where ChassisCode  = OmHstInquirySales.ChassisCode
                  and ChassisNo    = OmHstInquirySales.ChassisNo
				  and left(ChassisNo, 1) != 'P'
                  --and CustomerCode = OmHstInquirySales.CompanyCode)
                  and OmHstInquirySales.CompanyCode=case when CustomerCode = '6015402' then '6015401' else 
                                                    case when CustomerCode = '6051402' then '6051401' else
                                                             CustomerCode 
                                                         end 
                                                    end)

update SuzukiR4..OmHstInquirySalesNSDS
   set IsExists = '1'
 where IsExists = '0'
   and left(ChassisNo, 1) != 'P'
   and exists (select top 1 ChassisCode from SuzukiR4..OmHstInquirySales 
                where ChassisCode = OmHstInquirySalesNSDS.ChassisCode 
                  and ChassisNo   = OmHstInquirySalesNSDS.ChassisNo
				  and CompanyCode = OmHstInquirySalesNSDS.CustomerCode
                  and Status = 1)

insert into SuzukiR4..OmHstInquirySales 
     select YEAR(ProcessDate), MONTH(ProcessDate), CustomerCode, 
              (select TOP 1 OutletCode from SuzukiR4..GnMstDealerOutletMapping 
                where DealerCode = case when CustomerCode = '6015402' then '6015401' else 
				                   case when CustomerCode = '6051402' then '6051401' else 
								             CustomerCode 
                                        end 
                                   end  
                   and LastUpdateBy = 'HQ')
            , (select Top 1 DealerAbbreviation from SuzukiR4..GnMstDealerMapping where DealerCode = CustomerCode)
            , (select Top 1 OutletAbbreviation from SuzukiR4..GnMstDealerOutletMapping where DealerCode = CustomerCode and LastUpdateBy = 'HQ')
            , (select Top 1 Area from SuzukiR4..GnMstDealerMapping where DealerCode = CustomerCode)
            , null, null, null, null, null, null, null, null, null
            , null, null, null, null, null, null, null, null, null
            , FPNo, ProcessDate, SalesModelCode
            , null, null, null
            , (select top 1 b.LookUpValueName MarketModel from SuzukiR4..omMstModel a, SuzukiR4..gnMstLookUpDtl b 
                where a.MarketModelCode = b.LookUpValue
                and a.SalesModelCode  = omHstInquirySalesNSDS.SalesModelCode
                and isnull(a.MarketModelCode,'') <> '')
            , (select top 1 GroupMarketModel   from SuzukiR4..omMstModel
              where SalesModelCode   = omHstInquirySalesNSDS.SalesModelCode
                and isnull(GroupMarketModel,'') <> '')
            , (select top 1 ColumnMarketModel  from SuzukiR4..omMstModel
              where SalesModelCode   = omHstInquirySalesNSDS.SalesModelCode
                and isnull(ColumnMarketModel,'') <> '')
            , ChassisCode, ChassisNo,EngineCode,EngineNo
            , null, null, null, null, null, null, null, null, null
            , null, null, null, null, null, null, null, 1
            , 1, getDate(), 'SIM', getdate(), 'SIM', getdate(), 'NSDS', DODate, ProcessDate
       from SuzukiR4..omHstInquirySalesNSDS
        where (Year(DODate) >= 2010 or YEAR(PROCESSDATE) >= 2010)
          and CustomerCode in (Select distinct DealerCode from SuzukiR4..GnMstDealerMapping where isActive = 1)
          and not exists (select 1 from SuzukiR4..OmHstInquirySales 
                                     where ChassisCode = OmHstInquirySalesNSDS.ChassisCode
                                       and ChassisNo   = OmHstInquirySalesNSDS.ChassisNo
                                       and CompanyCode = OmHstInquirySalesNSDS.CustomerCode)


--- update market model
update SuzukiR4..omHstInquirySales
   set MarketModel=(select top 1 MarketModel from SuzukiR4..omHstInquirySales a 
                     where a.SalesModelCode=omHstInquirySales.SalesModelCode 
                       and isnull(a.MarketModel,'')<>'')
where isnull(MarketModel,'')='' and isnull(SalesModelCode,'')<>''

update SuzukiR4..omHstInquirySales
   set GroupMarketModel=(select top 1 GroupMarketModel from SuzukiR4..omHstInquirySales a 
                          where a.SalesModelCode=omHstInquirySales.SalesModelCode 
                            and isnull(a.GroupMarketModel,'')<>'')
where isnull(GroupMarketModel,'')='' and isnull(SalesModelCode,'')<>''

update SuzukiR4..omHstInquirySales
   set ColumnMarketModel=(select top 1 ColumnMarketModel from SuzukiR4..omHstInquirySales a 
                           where a.SalesModelCode=omHstInquirySales.SalesModelCode 
                             and isnull(a.ColumnMarketModel,'')<>'')
where isnull(ColumnMarketModel,'')='' and isnull(SalesModelCode,'')<>''
