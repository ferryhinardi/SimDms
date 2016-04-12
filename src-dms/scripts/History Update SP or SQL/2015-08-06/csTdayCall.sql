alter table CsTDayCall add Reason varchar(50)
GO
BEGIN
declare @companyCode varchar(20)
select top 1 @companyCode=companycode from gnMstOrganizationHdr

insert into [gnMstLookUpHdr]([CompanyCode],[CodeID],[CodeName],[FieldLength],[isNumber],[CreatedBy],[CreatedDate])
values (@companyCode,'CSREASON','CS Module: Alasan tidak dapat dihubungi',1,1,'SYSTEM',getdate())

insert into gnMstLookUpDtl (CompanyCode, CodeID, lookupvalue, seqno,ParaValue, LookUpValueName, CreatedBy, CreatedDate)
values (@companyCode, 'CSREASON','1',1,'1','Telepon tidak diangkat.','SYSTEM',getdate())
insert into gnMstLookUpDtl (CompanyCode, CodeID, lookupvalue, seqno,ParaValue, LookUpValueName, CreatedBy, CreatedDate)
values (@companyCode, 'CSREASON','2',2,'2','Tidak ada waktu.','SYSTEM',getdate())
insert into gnMstLookUpDtl (CompanyCode, CodeID, lookupvalue, seqno,ParaValue, LookUpValueName, CreatedBy, CreatedDate)
values (@companyCode, 'CSREASON','3',3,'3','No Telepon salah.','SYSTEM',getdate())
END
GO

ALTER view [dbo].[CsLkuTDaysCallView]
as
select a.CompanyCode
     , a.BranchCode
	 , a.CustomerCode
	 , a.CustomerName
	 , a.Address 
	 , a.PhoneNo
	 , a.HPNo
	 , AddPhone1 = isnull(a.AddPhone1, '-')
	 , AddPhone2 = isnull(a.AddPhone2, '-')
	 , CreatedDate = c.CreatedDate
	 , Outstanding = (
			case
				when isnull(c.CustomerCode, '') = '' then 'Y'
				else 'N'
			end
	   )
     , a.BirthDate,
	   b.CarType + ' - ' + mo.[SalesModelDesc] CarType, 
	   b.Color + ' - ' + col.[RefferenceDesc1] [Color]
     --, b.CarType
     --, b.Color
     , b.PoliceRegNo
     , b.Chassis
     , b.Engine 
     , b.SONo
     , b.SalesmanCode
     , b.SalesmanName
     , b.SalesmanName as Salesman
     , a.ReligionCode
	 , b.SalesModelCode
	 , b.SalesModelYear
	 , b.DODate
	 , b.BPKDate
     , case c.IsDeliveredA when 1 then 'Ya' else 'Tidak' end IsDeliveredA
     , case c.IsDeliveredB when 1 then 'Ya' else 'Tidak' end IsDeliveredB
     , case c.IsDeliveredC when 1 then 'Ya' else 'Tidak' end IsDeliveredC
     , case c.IsDeliveredD when 1 then 'Ya' else 'Tidak' end IsDeliveredD
     , case c.IsDeliveredE when 1 then 'Ya' else 'Tidak' end IsDeliveredE
     , case c.IsDeliveredF when 1 then 'Ya' else 'Tidak' end IsDeliveredF
     , case c.IsDeliveredG when 1 then 'Ya' else 'Tidak' end IsDeliveredG
     , c.Comment
	 , c.Additional
	 , c.Status
	 , b.DeliveryDate
	 , d.LookUpValueName Religion
	 , c.Reason
  from CsCustomerView a
  left join CsCustomerVehicleView b
    on b.CompanyCode = a.CompanyCode
   and b.BranchCode = a.BranchCode
   and b.CustomerCode = a.CustomerCode
  left join CsTDayCall c
    on c.CompanyCode = b.CompanyCode
   and c.CustomerCode = b.CustomerCode
   left join gnMstLookUpDtl d ON  (d.CodeID='RLGN' and d.CompanyCode = a.CompanyCode and d.LookUpValue=a.ReligionCode)
    LEFT OUTER JOIN [dbo].[omMstRefference] col on (b.CompanyCode=col.CompanyCode and b.Color = col.[RefferenceCode] and col.[RefferenceType]='COLO')
	LEFT OUTER JOIN [dbo].[omMstModel] mo on (b.CompanyCode=mo.CompanyCode and b.Cartype = mo.[SalesModelCode])
GO
