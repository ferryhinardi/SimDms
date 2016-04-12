-- SERVICE BOOK NO PROCESS, by HTO, 14 March 2016
-- Steps:  1. Read ServiceBookNo from [172.16.172.132].[repdb].dbo.[UPDATE_SERVICE_BOOK]
--         2. Insert/Update to table omHstServiceBook
--         3. Scheduling process for update ServiceBookNo to all 4W dealer
----------------------------------------------------------------------------------------

ALTER procedure [dbo].[uspfn_ServiceBookFromNSDS]
AS	

BEGIN TRANSACTION
BEGIN
  --use [SIMDMS]
	if not exists (select * from sys.objects where object_id = object_id(N'[dbo].[omHstServiceBook]') and type in (N'U'))
		create table [dbo].[omHstServiceBook](
				[SJNo]			[varchar](10) NOT NULL,
				[DealerCode]	[varchar](10) NOT NULL,
				[VIN]			[varchar](20) NOT NULL,
				[ServiceBookNo]	[varchar](10) NOT NULL,
				[ProcessDate]	[datetime]		  NULL,
			constraint [PK_omHstServiceBook] primary key clustered 
			(
				[VIN]			asc
			)	with (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 70) ON [PRIMARY]
		) on [PRIMARY]

	;with x as (select * from [dbo].[omHstServiceBook] a
				 where exists (select 1 from [172.16.172.132].[repdb].dbo.[UPDATE_SERVICE_BOOK] b
								where b.VIN=a.VIN and b.Service_Book_No<>a.ServiceBookNo))
		update x set ServiceBookNo = (select Service_Book_No 
										from [172.16.172.132].[repdb].dbo.[UPDATE_SERVICE_BOOK] b
									   where b.VIN=x.VIN)
				   , ProcessDate = NULL

	insert into [dbo].[omHstServiceBook] 
				(SJNo, DealerCode, VIN, ServiceBookNo, ProcessDate)
		 select SJ_No, Customer_Code, VIN, Service_Book_No, NULL
		   from [172.16.172.132].[repdb].dbo.[UPDATE_SERVICE_BOOK] a
		  where not exists (select 1 from [dbo].[omHstServiceBook] b where b.VIN=a.VIN)

	--;with y as (select * from [172.16.172.132].[repdb].dbo.[UPDATE_SERVICE_BOOK])
	--	delete y where exists (select 1 from [dbo].[omHstServiceBook] b where b.VIN=y.VIN)
	--delete [172.16.172.132].[repdb].dbo.[UPDATE_SERVICE_BOOK]

	select * from [172.16.172.132].[repdb].dbo.[UPDATE_SERVICE_BOOK]
	select * from [dbo].[omHstServiceBook]
END

IF @@TRANCOUNT > 0
	COMMIT TRANSACTION
else
	ROLLBACK TRANSACTION
