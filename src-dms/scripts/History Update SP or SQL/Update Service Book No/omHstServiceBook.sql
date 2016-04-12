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
