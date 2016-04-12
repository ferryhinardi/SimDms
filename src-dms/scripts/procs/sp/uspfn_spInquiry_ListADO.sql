CREATE procedure [dbo].[uspfn_spInquiry_ListADO]
	@ParentCode		varchar(100),
	@Detail			int 
AS
Begin

	if (@Detail = 1)
	begin
		select distinct convert(varchar,a.GroupNo) [value], a.Area [text]
		from gnMstDealerMapping a
		where a.isActive = 1
		order by [value] asc
	end
	else if(@Detail = 2)
	begin
		
		declare @JBDTB INT
		SET @JBDTB = 0

		if @ParentCode <> ''
		begin
			if (@ParentCode='JABODETABEK' or @ParentCode = 'CABANG')
				SET @JBDTB = 1
		end 
		ELSE
			SET @ParentCode = '%%'

		IF @JBDTB = 1
		BEGIN
			select distinct a.DealerCode [value], a.DealerName [text], convert(varchar,a.GroupNo) [parent]
			from gnMstDealerMapping a
			where a.Area IN ('JABODETABEK', 'CABANG') and a.isActive = 1
			order by a.DealerCode
		END
		ELSE
			select distinct a.DealerCode [value], a.DealerName [text], convert(varchar,a.GroupNo) [parent]
			from gnMstDealerMapping a
			where (a.Area like @ParentCode) and a.isActive = 1
			order by a.DealerCode
	end
	else if(@Detail = 3)
	begin

		select distinct  b.OutletCode [value], b.OutletName [text],a.DealerCode [parent]
		from gnMstDealerMapping a
		left join gnMstDealerOutletMapping b on a.DealerCode = b.DealerCode
		where (a.DealerCode like case when @ParentCode <> '' then @ParentCode else '%%' end) and a.isActive = 1
		order by b.OutletName
	end
end
