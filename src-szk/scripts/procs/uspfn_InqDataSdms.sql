alter procedure uspfn_InqDataSdms
	@TableName varchar(80) = ''
as

if @TableName = 'PmHstIts' exec uspfn_InqDataIts
if @TableName = 'GnMstCustomer' exec uspfn_InqGnMstCustomer
if @TableName = 'HrEmployee' exec uspfn_InqHrEmployee
if @TableName = 'CsTDayCall' exec uspfn_InqCsTDaysCall
if @TableName = 'CsCustBirthday' exec uspfn_InqCsCustBirthday
if @TableName = 'CsCustBpkb' exec uspfn_InqCsCustBpkb
if @TableName = 'CsStnkExt' exec uspfn_InqCsStnkExt
if @TableName = 'SvMstCustomerVehicle' exec uspfn_InqSvMstCustomerVehicle
if @TableName = 'SvTrnService' exec uspfn_InqSvTrnService
if @TableName = 'SvTrnInvoice' exec uspfn_InqSvTrnInvoice

go

uspfn_InqDataSdms 'SvMstCustomerVehicle'
