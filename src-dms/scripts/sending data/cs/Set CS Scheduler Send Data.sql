declare @CompanyCode varchar(25);
set @CompanyCode  = (select top 1 CompanyCode from gnMstOrganizationHdr);



delete
  from GnMstSendScheduleDms 
 where DataType in (
	   'CSBDAY'
	 , 'CSBKPB'
	 , 'CSCUSTDATA'
	 , 'CSFEEDBACK'
	 , 'CSHLDAY'
	 , 'CSVHCL'
	 , 'CSRLTN'
	 , 'CSMSTHLDAY'
	 , 'CSSTNKEXT'
	 , 'CS3DCALL'
	 , 'TRSLSINVIN'
	 , 'TRSLSINV'
	 , 'MSTCUST'
	 , 'TRSLSDODTL'
	 , 'TRSLSDO'
	 , 'SVMSTCVHCL'
	 , 'TRSLSBPK'
	 , 'TRSLSSO'
     )



insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CSBDAY', '1990-01-01', 500, 'uspfn_CsCustomerBirthdaySend', 50, 'S', 'UpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CSBKPB', '1990-01-01', 500, 'uspfn_CsCustomerBPKBSend', 51, 'S', 'UpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CSCUSTDATA', '1990-01-01', 500, 'uspfn_CsCustomerDataSend', 52, 'S', 'UpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CSFEEDBACK', '1990-01-01', 500, 'uspfn_CsFeedbackSend', 53, 'S', 'UpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CSHLDAY', '1990-01-01', 500, 'uspfn_CsHolidaySend', 54, 'S', 'UpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CSVHCL', '1990-01-01', 500, 'uspfn_CsCustomerVehicleSend', 55, 'S', 'UpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CSRLTN', '1990-01-01', 500, 'uspfn_CsRelationSend', 56, 'S', 'UpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CSMSTHLDAY', '1990-01-01', 500, 'uspfn_CsMstHolidaySend', 57, 'S', 'UpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CSSTNKEXT', '1990-01-01', 500, 'uspfn_CsSTNKEstension', 58, 'S', 'LastUpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'CS3DCALL', '1990-01-01', 500, 'uspfn_Cs3DayCallSend', 59, 'S', 'UpdatedDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'TRSLSINVIN', '1990-01-01', 500, 'uspfn_OmTrSalesInvoiceVinSend', 60, 'S', 'LastUpdateDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'TRSLSINV', '1990-01-01', 500, 'uspfn_OmTrSalesInvoiceSend', 61, 'S', 'LastUpdateDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'MSTCUST', '1990-01-01', 500, 'uspfn_CSMstCustomerSend', 62, 'S', 'LastUpdateDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'TRSLSDODTL', '1990-01-01', 500, 'uspfn_OmTrSalesDODetailSend', 63, 'S', 'LastUpdateDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'TRSLSDO', '1990-01-01', 500, 'uspfn_OmTrSalesDOSend', 64, 'S', 'LastUpdateDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'SVMSTCVHCL', '1990-01-01', 500, 'uspfn_SvMsVehicle', 65, 'S', 'LastUpdateDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'TRSLSBPK', '1990-01-01', 500, 'uspfn_OmTrSalesBPKSend', 66, 'S', 'LastUpdateDate')

insert into gnMstSendScheduleDms ( CompanyCode, DataType, LastSendDate, SegmentNo, SPName, Priority, Status, ColumnLastUpdate )
values (@CompanyCode, 'TRSLSSO', '1990-01-01', 500, 'uspfn_OmTrSalesSOSend', 67, 'S', 'LastUpdateDate')




select * from GnMstSendScheduleDms
