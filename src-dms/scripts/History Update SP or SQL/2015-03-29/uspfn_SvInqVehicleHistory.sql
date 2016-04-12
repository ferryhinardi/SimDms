/****** Object:  StoredProcedure [dbo].[uspfn_SvInqVehicleHistory]    Script Date: 04/04/2015 14:19:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[uspfn_SvInqVehicleHistory]
	 @CompanyCode varchar(20),
	 @BranchCode  varchar(20),
	 @ProductType varchar(20),
	 @PoliceRegNo varchar(20) = '',
	 @ServiceBookNo varchar(20) = '',
	 @CustomerCode varchar(20) = '',
	 @ChassisCode  varchar(20) = '',
	 @ChassisNo    varchar(20) = '',
	 @EngineCode  varchar(20) = '',
	 @EngineNo    varchar(20) = '',
	 @BasicModel  varchar(20) = '',
	 @IsCheckDate bit,
	 @ServiceDate datetime,
	 @IsAllBranch bit

as

select convert(bit, '0') as Cetak
     , isnull(b.BranchCode, '') as BranchCode
     , a.PoliceRegNo
     , a.BasicModel
     , a.TransmissionType
     , ltrim(rtrim(a.ChassisCode)) + convert(varchar, a.ChassisNo) as Chassis
     , a.ChassisCode
     , a.ChassisNo
     , ltrim(rtrim(a.EngineCode)) + convert(varchar, a.EngineNo) as Engine
     , a.ServiceBookNo
     , a.ColourCode
     , b.CustomerCode
     , c.CustomerName
     , a.CustomerCode + ' - ' + isnull(c.CustomerName, '') Customer
     , a.FakturPolisiDate
     , a.LastServiceDate
     , a.LastServiceOdometer
     , a.DealerCode
     , d.CustomerName as DealerName
     , a.DealerCode + ' - ' + isnull(d.CustomerName, '') Dealer
     --, isnull(a.RemainderDescription, '') Remarks
     ,(select Remarks from svTrnInvoice where CreatedDate = (select max(CreatedDate) from svTrnInvoice where PoliceRegNo=@PoliceRegNo)) Remarks
     , a.CustomerCode
  from svMstCustomerVehicle a with (nolock, nowait)
 inner join svTrnService b with (nolock, nowait)
    on b.CompanyCode = a.CompanyCode
   and b.ChassisCode = a.ChassisCode
   and b.ChassisNo = a.ChassisNo
  left join gnMstCustomer c with(nolock, nowait) 
    on c.CompanyCode  = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
  left join gnMstCustomer d with(nolock, nowait) 
    on d.CompanyCode  = a.CompanyCode
   and d.CustomerCode = a.DealerCode
 where 1 = 1
   and a.CompanyCode = a.CompanyCode
   and b.BranchCode like (case @IsAllBranch when 1 then '%' else @BranchCode end)
   and b.ProductType = @ProductType
   and a.PoliceRegNo like ('%' + @PoliceRegNo + '%')
   and a.ServiceBookNo like ('%' + @ServiceBookNo + '%')
   and a.CustomerCode like ('%' + @CustomerCode + '%')
   and a.ChassisCode like ('%' + @ChassisCode + '%')
   and convert(varchar, a.ChassisNo) like ('%' + @ChassisNo + '%')
   and a.EngineCode like ('%' + @EngineCode + '%')
   and convert(varchar, a.EngineNo) like ('%' + @EngineNo  + '%')
   and a.BasicModel like ('%' + @BasicModel + '%')
   and convert(varchar, isnull(b.JobOrderDate, convert(datetime, '19000101')), 112) >=  (case @IsCheckDate when 1 then convert(varchar, @ServiceDate, 112) else '19000102' end)
   and b.ServiceStatus <> '6'
 Group BY b.BranchCode
     , a.PoliceRegNo
     , a.BasicModel
     , a.TransmissionType
     , a.ChassisCode
     , a.ChassisNo
     , a.ChassisCode
     , a.ChassisNo
     , a.EngineCode
     , a.EngineNo
     , a.ServiceBookNo
     , a.ColourCode
     , b.CustomerCode
     , c.CustomerName
     , a.CustomerCode
     , c.CustomerName
     , a.FakturPolisiDate
     , a.LastServiceDate
     , a.LastServiceOdometer
     , a.DealerCode
     , d.CustomerName
     , a.DealerCode
     , d.CustomerName
     , a.CustomerCode

