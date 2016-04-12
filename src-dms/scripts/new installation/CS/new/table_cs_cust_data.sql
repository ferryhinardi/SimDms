create table CsCustData(
	CompanyCode varchar(20),
	CustomerCode varchar(20),
	AddPhone1 varchar(20),
	AddPhone2 varchar(20),
	ReligionCode varchar(20),
	IsDeleted bit,
	CreatedBy varchar(36),
	CreatedDate datetime,
	UpdatedBy varchar(36),
	UpdatedDate datetime,
 primary key clustered 
(
	CompanyCode asc,
	CustomerCode asc
)) 