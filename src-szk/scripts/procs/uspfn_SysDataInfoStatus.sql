alter procedure uspfn_SysDataInfoStatus
as

select a.TableName
     , a.DealerCode
     , c.CompanyName
	 , a.LastUpdate 
	 , count(1) as RecordCount
  from SysDealer a
  left join OmHstInquirySales b
    on b.CompanyCode = a.DealerCode
  left join gnMstOrganizationHdr c
    on c.CompanyCode = a.DealerCode
 where a.TableName = 'OmHstInquirySales' and LastUpdate > '1900-01-01'
 group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
 order by a.LastUpdate desc

select a.TableName
     , a.DealerCode
     , c.CompanyName
	 , a.LastUpdate 
	 , count(1) as RecordCount
  from SysDealer a
  left join PmActivities b
    on b.CompanyCode = a.DealerCode
  left join gnMstOrganizationHdr c
    on c.CompanyCode = a.DealerCode
 where a.TableName = 'PmActivities' and LastUpdate > '1900-01-01'
 group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
 order by a.LastUpdate desc

select a.TableName
     , a.DealerCode
     , c.CompanyName
	 , a.LastUpdate 
	 , count(1) as RecordCount
  from SysDealer a
  left join PmKdpAdditional b
    on b.CompanyCode = a.DealerCode
  left join gnMstOrganizationHdr c
    on c.CompanyCode = a.DealerCode
 where a.TableName = 'PmKdpAdditional' and LastUpdate > '1900-01-01'
 group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
 order by a.LastUpdate desc
 
select a.TableName
     , a.DealerCode
     , c.CompanyName
	 , a.LastUpdate 
	 , count(1) as RecordCount
  from SysDealer a
  left join GnMstCustomer b
    on b.CompanyCode = a.DealerCode
  left join gnMstOrganizationHdr c
    on c.CompanyCode = a.DealerCode
 where a.TableName = 'GnMstCustomer' and LastUpdate > '1900-01-01'
 group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
 order by a.LastUpdate desc
 
--select a.TableName
--     , a.DealerCode
--     , c.CompanyName
--	 , a.LastUpdate 
--	 , count(1) as RecordCount
--  from SysDealer a
--  left join PmBranchOutlets b
--    on b.CompanyCode = a.DealerCode
--  left join gnMstOrganizationHdr c
--    on c.CompanyCode = a.DealerCode
-- where a.TableName = 'PmBranchOutlets' and LastUpdate > '1900-01-01'
-- group by c.CompanyName, a.DealerCode, a.TableName, a.LastUpdate
-- order by a.LastUpdate desc
