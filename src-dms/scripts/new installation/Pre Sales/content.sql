
declare @CompanyCode varchar(13);
set @CompanyCode = (select top 1 a.CompanyCode from gnMstOrganizationHdr a);

delete from SysControlDms where MenuID='itsinkdp';

insert into SysControlDms (CompanyCode, MenuID, FieldID, RoleID, Visibility, Type, Title)
values (@CompanyCode, 'itsinkdp', 'NikSales', 'its', 1, 'select', 'Sales/Sales Ctr.')

insert into SysControlDms (CompanyCode, MenuID, FieldID, RoleID, Visibility, Type, Title)
values (@CompanyCode, 'itsinkdp', 'NikSales', 'its-om', 1, 'select', 'Sales/Sales Ctr.')

insert into SysControlDms (CompanyCode, MenuID, FieldID, RoleID, Visibility, Type, Title)
values (@CompanyCode, 'itsinkdp', 'NikSales', 'admin', 1, 'select', 'Sales/Sales Ctr.')





insert into SysControlDms (CompanyCode, MenuID, FieldID, RoleID, Visibility, Type, Title)
values (@CompanyCode, 'itsinkdp', 'NikSH', 'its', 1, 'select', 'Operational Manager')

insert into SysControlDms (CompanyCode, MenuID, FieldID, RoleID, Visibility, Type, Title)
values (@CompanyCode, 'itsinkdp', 'NikSH', 'its-om', 1, 'select', 'Operational Manager')

insert into SysControlDms (CompanyCode, MenuID, FieldID, RoleID, Visibility, Type, Title)
values (@CompanyCode, 'itsinkdp', 'NikSH', 'admin', 1, 'select', 'Operational Manager')


select * from SysControlDms