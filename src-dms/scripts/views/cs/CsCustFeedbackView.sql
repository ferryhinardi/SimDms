go
if object_id('CsCustFeedbackView') is not null
	drop view CsCustFeedbackView

go
create view CsCustFeedbackView

as 

select a.CompanyCode, a.CustomerCode, a.CustomerName, a.Address1 as [Address], a.PhoneNo
     , b.ChassisCode + convert(varchar, b.ChassisNo) Chassis
     , b.EngineCode + convert(varchar, b.EngineNo) Engine
     , b.PoliceRegNo, b.ServiceBookNo
     , a.Address1
     , c.FeedbackA
     , c.FeedbackB
     , c.FeedbackC
     , c.FeedbackD
     , b.BasicModel as CarType
     , FeedbackCode = (case isnull(c.Chassis, '') when '' then 'N' else 'Y' end)
  from GnMstCustomer a
  left join svMstCustomerVehicle b
    on b.CompanyCode = a.CompanyCode
   and b.CustomerCode = a.CustomerCode
  left join CsCustFeedback c
    on c.CompanyCode = a.CompanyCode
   and c.CustomerCode = a.CustomerCode
   and c.Chassis = b.ChassisCode + convert(varchar, b.ChassisNo)
 where 1 = 1
   and b.ChassisCode is not null
