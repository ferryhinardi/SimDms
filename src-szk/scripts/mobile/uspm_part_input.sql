alter procedure uspm_part_input
	@BoxID varchar(50),
	@PartNo varchar(50),
	@PartQty numeric(18, 2)
as


insert into SpBarCodeDtl (PartID, BoxID, PartNo, PartQty)
select newid(), @BoxID, @PartNo, @PartQty

select @BoxID BoxID, @PartNo PartNo, @PartQty PartQty