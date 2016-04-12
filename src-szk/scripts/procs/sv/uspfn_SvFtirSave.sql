alter procedure uspfn_SvFtirSave
	@FtirNo varchar(20),
    @DealerCode varchar(20),
    @OutletCode varchar(20),
	@FtirDate date,
	@FtirEventDate date,
	@FtirMaker varchar(50),
	@FtirCategory varchar(10),
	@FtirRegDate date,
	@DealerName varchar(50),
	@VinNo varchar(50),
	@Model varchar(50),
	@Machine varchar(50),
	@TransmNo varchar(50),
	@TitleCategory varchar(20),
	@TitleName varchar(50),
	@Odometer int,
	@UsageTime int,
	@IsAvailPartDmg bit,
	@IsReportToSis bit,
	@EstimatedDelivery date,
	@NotSendingCategory varchar(10),
	@EvtInfoA01 text,
	@EvtInfoA02 bit,
	@EvtInfoA03 text,
	@EvtInfoA04 text,
	@EvtInfoA05 text,
	@EvtInfoA06 text,
	@EvtInfoA07 int,
	@EvtInfoA08 varchar(10),
	@EvtInfoB01 text,
	@EvtInfoB01Chk01 bit,
	@EvtInfoB01Chk02 bit,
	@EvtInfoB01Chk03 bit,
	@EvtInfoB01Chk04 bit,
	@EvtInfoB01Chk05 bit,
	@EvtInfoB01Chk06 bit,
	@EvtInfoB01Chk07 bit,
	@EvtInfoB01Chk08 bit,
	@EvtInfoB01Chk09 bit,
	@EvtInfoB01Chk10 bit,
	@EvtInfoB01Chk11 bit,
	@EvtInfoB01Chk12 bit,
	@EvtInfoB01Chk13 bit,
	@EvtInfoB01Chk14 bit,
	@EvtInfoB01Chk15 bit,
	@EvtInfoB01Chk16 bit,
	@EvtInfoB01Chk17 bit,
	@EvtInfoB01Chk18 bit = 0,
	@EvtInfoB01Chk19 bit = 0,
	@EvtInfoB01Chk20 bit = 0,
	@EvtInfoB02 varchar(10),
	@EvtInfoB0201 varchar(10),
	@EvtInfoB0202A int,
	@EvtInfoB0202B int,
	@EvtInfoB0203 text,
	@EvtInfoB0301 varchar(10),
	@EvtInfoB0302A int,
	@EvtInfoB0302B int,
	@EvtInfoB0303 text,
	@EvtInfoB0401 int,
	@EvtInfoB0402 int,
	@EvtInfoB0403 varchar(10),
	@EvtInfoB0404 varchar(10),
	@EvtInfoB0501 int,
	@EvtInfoB0502 int,
	@EvtInfoB0503 varchar(10),
	@EvtInfoC01 text,
	@EvtInfoC01Chk01 bit,
	@EvtInfoC01Chk02 bit,
	@EvtInfoC01Chk03 bit,
	@EvtInfoC01Chk04 bit,
	@EvtInfoC01Chk05 bit,
	@EvtInfoC01Chk06 bit,
	@EvtInfoC01Chk07 bit,
	@EvtInfoC01Chk08 bit,
	@EvtInfoC01Chk09 bit,
	@EvtInfoC01Chk10 bit,
	@EvtInfoC01Chk11 bit,
	@EvtInfoC01Chk12 bit,
	@EvtInfoC01Chk13 bit = 0,
	@EvtInfoC01Chk14 bit = 0,
	@EvtInfoC01Chk15 bit = 0,
	@EvtInfoC01Chk16 bit = 0,
	@EvtInfoC01Chk17 bit = 0,
	@EvtInfoC01Chk18 bit = 0,
	@EvtInfoC01Chk19 bit = 0,
	@EvtInfoC01Chk20 bit = 0,
	@EvtInfoC02Chk01 bit,
	@EvtInfoC02Chk02 bit,
	@EvtInfoC02Chk03 bit,
	@EvtInfoC03Chk01 bit,
	@EvtInfoC03Chk02 bit,
	@EvtInfoC03Chk03 bit,
	@EvtInfoC03Chk04 bit,
	@EvtInfoC04Chk01 bit,
	@EvtInfoC04Chk02 bit,
	@EvtInfoC04Chk03 bit,
	@EvtInfoC04Chk04 bit,
	@EvtInfoC05 int,
	@EvtInfoC06Chk01 bit,
	@EvtInfoC06Chk02 bit,
	@EvtInfoC06Chk03 bit,
	@EvtInfoC06Chk04 bit,
	@EvtInfoC0701 int,
	@EvtInfoC0702 numeric(8, 2),
	@EvtInfoC0801 text,
	@EvtInfoC0802 text,
	@EvtInfoC0803 text,
	@EvtInfoC0804 text,
	@EvtInfoC0805 text,
	@EvtInfoC0806 varchar(10),
	@EvtInfoC0807 text,
	@EvtInfoC0808 varchar(10),
	@EvtInfoC0809 varchar(10),
	@EvtInfoC0901 varchar(10),
	@EvtInfoC0902 int,
	@EvtInfoC0902Chk01 bit,
	@EvtInfoC0902Chk02 bit,
	@EvtInfoC0902Chk03 bit,
	@EvtInfoC0902Chk04 bit,
	@EvtInfoC0902Chk05 bit,
	@EvtInfoC0902Chk06 bit,
	@EvtInfoC0902Chk07 bit,
	@EvtInfoC0902Chk08 bit = 0,
	@EvtInfoC0903Chk01 bit,
	@EvtInfoC0903Chk02 bit,
	@EvtInfoC0903Chk03 bit,
	@EvtInfoC0903Chk04 bit,
	@EvtInfoC0903Chk05 bit,
	@EvtInfoC0903Chk06 bit = 0,
	@EvtInfoC0904Chk01 bit,
	@EvtInfoC0904Chk02 bit,
	@EvtInfoC0904Chk03 bit,
	@EvtInfoC0904Chk04 bit,
	@EvtInfoC0904Chk05 bit,
	@EvtInfoC0904Chk06 bit,
	@EvtInfoC0904Chk07 bit,
	@EvtInfoC090501 varchar(10),
	@EvtInfoC090502 varchar(10),
	@EvtInfoC090503 varchar(10),
	@EvtInfoC090504 text,
	@EvtInfoC090601 varchar(10),
	@EvtInfoC090602 int,
	@UserId varchar(20)
	
as

if not exists (select top 1 1 from SvFtir where FtirNo = @FtirNo)
begin
    declare @LastNo int
    set @LastNo = isnull((
                    select top 1 convert(int, right(FtirNo, 4)) from SvFtir
                     where substring(replace(FtirNo, '.', ''), 5, 8) = convert(varchar, getdate(), 112)
                     order by FtirNo desc
                    ), 0)

    set @FtirNo = 'FTIR.' + left(convert(varchar, getdate(), 112), 4)
                   + '.' + right(convert(varchar, getdate(), 112), 4)
                   + '.' + (select right(replicate('0', 4) + convert(varchar, (@LastNo + 1)), 4))

    insert into SvFtir (
        FtirNo, 
        FtirDate, 
        FtirEventDate, 
        FtirMaker, 
        FtirCategory, 
        FtirRegDate, 
        DealerName, 
        VinNo, 
        Model, 
        Machine, 
        TransmNo, 
        TitleCategory, 
        TitleName, 
        Odometer, 
        UsageTime, 
        IsAvailPartDmg, 
        IsReportToSis, 
        EstimatedDelivery, 
        NotSendingCategory, 
        EvtInfoA01, 
        EvtInfoA02, 
        EvtInfoA03, 
        EvtInfoA04, 
        EvtInfoA05, 
        EvtInfoA06, 
        EvtInfoA07, 
        EvtInfoA08, 
        EvtInfoB01, 
        EvtInfoB01Chk01, 
        EvtInfoB01Chk02, 
        EvtInfoB01Chk03, 
        EvtInfoB01Chk04, 
        EvtInfoB01Chk05, 
        EvtInfoB01Chk06, 
        EvtInfoB01Chk07, 
        EvtInfoB01Chk08, 
        EvtInfoB01Chk09, 
        EvtInfoB01Chk10, 
        EvtInfoB01Chk11, 
        EvtInfoB01Chk12, 
        EvtInfoB01Chk13, 
        EvtInfoB01Chk14, 
        EvtInfoB01Chk15, 
        EvtInfoB01Chk16, 
        EvtInfoB01Chk17, 
        EvtInfoB01Chk18, 
        EvtInfoB01Chk19, 
        EvtInfoB01Chk20, 
        EvtInfoB02, 
        EvtInfoB0201, 
        EvtInfoB0202A, 
        EvtInfoB0202B, 
        EvtInfoB0203, 
        EvtInfoB0301, 
        EvtInfoB0302A, 
        EvtInfoB0302B, 
        EvtInfoB0303, 
        EvtInfoB0401, 
        EvtInfoB0402, 
        EvtInfoB0403, 
        EvtInfoB0404, 
        EvtInfoB0501, 
        EvtInfoB0502, 
        EvtInfoB0503, 
        EvtInfoC01, 
        EvtInfoC01Chk01, 
        EvtInfoC01Chk02, 
        EvtInfoC01Chk03, 
        EvtInfoC01Chk04, 
        EvtInfoC01Chk05, 
        EvtInfoC01Chk06, 
        EvtInfoC01Chk07, 
        EvtInfoC01Chk08, 
        EvtInfoC01Chk09, 
        EvtInfoC01Chk10, 
        EvtInfoC01Chk11, 
        EvtInfoC01Chk12, 
        EvtInfoC01Chk13, 
        EvtInfoC01Chk14, 
        EvtInfoC01Chk15, 
        EvtInfoC01Chk16, 
        EvtInfoC01Chk17, 
        EvtInfoC01Chk18, 
        EvtInfoC01Chk19, 
        EvtInfoC01Chk20, 
        EvtInfoC02Chk01, 
        EvtInfoC02Chk02, 
        EvtInfoC02Chk03, 
        EvtInfoC03Chk01, 
        EvtInfoC03Chk02, 
        EvtInfoC03Chk03, 
        EvtInfoC03Chk04, 
        EvtInfoC04Chk01, 
        EvtInfoC04Chk02, 
        EvtInfoC04Chk03, 
        EvtInfoC04Chk04, 
        EvtInfoC05, 
        EvtInfoC06Chk01, 
        EvtInfoC06Chk02, 
        EvtInfoC06Chk03, 
        EvtInfoC06Chk04, 
        EvtInfoC0701, 
        EvtInfoC0702, 
        EvtInfoC0801, 
        EvtInfoC0802, 
        EvtInfoC0803, 
        EvtInfoC0804, 
        EvtInfoC0805, 
        EvtInfoC0806, 
        EvtInfoC0807, 
        EvtInfoC0808, 
        EvtInfoC0809, 
        EvtInfoC0901, 
        EvtInfoC0902, 
        EvtInfoC0902Chk01, 
        EvtInfoC0902Chk02, 
        EvtInfoC0902Chk03, 
        EvtInfoC0902Chk04, 
        EvtInfoC0902Chk05, 
        EvtInfoC0902Chk06, 
        EvtInfoC0902Chk07, 
        EvtInfoC0902Chk08, 
        EvtInfoC0903Chk01, 
        EvtInfoC0903Chk02, 
        EvtInfoC0903Chk03, 
        EvtInfoC0903Chk04, 
        EvtInfoC0903Chk05, 
        EvtInfoC0903Chk06, 
        EvtInfoC0904Chk01, 
        EvtInfoC0904Chk02, 
        EvtInfoC0904Chk03, 
        EvtInfoC0904Chk04, 
        EvtInfoC0904Chk05, 
        EvtInfoC0904Chk06, 
        EvtInfoC0904Chk07, 
        EvtInfoC090501, 
        EvtInfoC090502, 
        EvtInfoC090503, 
        EvtInfoC090504, 
        EvtInfoC090601, 
        EvtInfoC090602, 
        DealerCode,
        OutletCode,
        CreatedBy, 
        CreatedDate, 
        UpdatedBy, 
        UpdatedDate
    ) values (
        @FtirNo, 
        @FtirDate, 
        @FtirEventDate, 
        @FtirMaker, 
        @FtirCategory, 
        @FtirRegDate, 
        @DealerName, 
        @VinNo, 
        @Model, 
        @Machine, 
        @TransmNo, 
        @TitleCategory, 
        @TitleName, 
        @Odometer, 
        @UsageTime, 
        @IsAvailPartDmg, 
        @IsReportToSis, 
        @EstimatedDelivery, 
        @NotSendingCategory, 
        @EvtInfoA01, 
        @EvtInfoA02, 
        @EvtInfoA03, 
        @EvtInfoA04, 
        @EvtInfoA05, 
        @EvtInfoA06, 
        @EvtInfoA07, 
        @EvtInfoA08, 
        @EvtInfoB01, 
        @EvtInfoB01Chk01, 
        @EvtInfoB01Chk02, 
        @EvtInfoB01Chk03, 
        @EvtInfoB01Chk04, 
        @EvtInfoB01Chk05, 
        @EvtInfoB01Chk06, 
        @EvtInfoB01Chk07, 
        @EvtInfoB01Chk08, 
        @EvtInfoB01Chk09, 
        @EvtInfoB01Chk10, 
        @EvtInfoB01Chk11, 
        @EvtInfoB01Chk12, 
        @EvtInfoB01Chk13, 
        @EvtInfoB01Chk14, 
        @EvtInfoB01Chk15, 
        @EvtInfoB01Chk16, 
        @EvtInfoB01Chk17, 
        @EvtInfoB01Chk18, 
        @EvtInfoB01Chk19, 
        @EvtInfoB01Chk20, 
        @EvtInfoB02, 
        @EvtInfoB0201, 
        @EvtInfoB0202A, 
        @EvtInfoB0202B, 
        @EvtInfoB0203, 
        @EvtInfoB0301, 
        @EvtInfoB0302A, 
        @EvtInfoB0302B, 
        @EvtInfoB0303, 
        @EvtInfoB0401, 
        @EvtInfoB0402, 
        @EvtInfoB0403, 
        @EvtInfoB0404, 
        @EvtInfoB0501, 
        @EvtInfoB0502, 
        @EvtInfoB0503, 
        @EvtInfoC01, 
        @EvtInfoC01Chk01, 
        @EvtInfoC01Chk02, 
        @EvtInfoC01Chk03, 
        @EvtInfoC01Chk04, 
        @EvtInfoC01Chk05, 
        @EvtInfoC01Chk06, 
        @EvtInfoC01Chk07, 
        @EvtInfoC01Chk08, 
        @EvtInfoC01Chk09, 
        @EvtInfoC01Chk10, 
        @EvtInfoC01Chk11, 
        @EvtInfoC01Chk12, 
        @EvtInfoC01Chk13, 
        @EvtInfoC01Chk14, 
        @EvtInfoC01Chk15, 
        @EvtInfoC01Chk16, 
        @EvtInfoC01Chk17, 
        @EvtInfoC01Chk18, 
        @EvtInfoC01Chk19, 
        @EvtInfoC01Chk20, 
        @EvtInfoC02Chk01, 
        @EvtInfoC02Chk02, 
        @EvtInfoC02Chk03, 
        @EvtInfoC03Chk01, 
        @EvtInfoC03Chk02, 
        @EvtInfoC03Chk03, 
        @EvtInfoC03Chk04, 
        @EvtInfoC04Chk01, 
        @EvtInfoC04Chk02, 
        @EvtInfoC04Chk03, 
        @EvtInfoC04Chk04, 
        @EvtInfoC05, 
        @EvtInfoC06Chk01, 
        @EvtInfoC06Chk02, 
        @EvtInfoC06Chk03, 
        @EvtInfoC06Chk04, 
        @EvtInfoC0701, 
        @EvtInfoC0702, 
        @EvtInfoC0801, 
        @EvtInfoC0802, 
        @EvtInfoC0803, 
        @EvtInfoC0804, 
        @EvtInfoC0805, 
        @EvtInfoC0806, 
        @EvtInfoC0807, 
        @EvtInfoC0808, 
        @EvtInfoC0809, 
        @EvtInfoC0901, 
        @EvtInfoC0902, 
        @EvtInfoC0902Chk01, 
        @EvtInfoC0902Chk02, 
        @EvtInfoC0902Chk03, 
        @EvtInfoC0902Chk04, 
        @EvtInfoC0902Chk05, 
        @EvtInfoC0902Chk06, 
        @EvtInfoC0902Chk07, 
        @EvtInfoC0902Chk08, 
        @EvtInfoC0903Chk01, 
        @EvtInfoC0903Chk02, 
        @EvtInfoC0903Chk03, 
        @EvtInfoC0903Chk04, 
        @EvtInfoC0903Chk05, 
        @EvtInfoC0903Chk06, 
        @EvtInfoC0904Chk01, 
        @EvtInfoC0904Chk02, 
        @EvtInfoC0904Chk03, 
        @EvtInfoC0904Chk04, 
        @EvtInfoC0904Chk05, 
        @EvtInfoC0904Chk06, 
        @EvtInfoC0904Chk07, 
        @EvtInfoC090501, 
        @EvtInfoC090502, 
        @EvtInfoC090503, 
        @EvtInfoC090504, 
        @EvtInfoC090601, 
        @EvtInfoC090602, 
        @DealerCode,
        @OutletCode,
        @UserId, 
        getdate(), 
        @UserId, 
        getdate()
    )
end
else
begin
    update SvFtir
       set FtirDate = @FtirDate
         , FtirEventDate = @FtirEventDate
         , FtirMaker = @FtirMaker
         , FtirCategory = @FtirCategory
         , FtirRegDate = @FtirRegDate
         , DealerCode = @DealerCode
         , OutletCode = @OutletCode
         , DealerName = @DealerName
         , VinNo = @VinNo
         , Model = @Model
         , Machine = @Machine
         , TransmNo = @TransmNo
         , TitleCategory = @TitleCategory
         , TitleName = @TitleName
         , Odometer = @Odometer
         , UsageTime = @UsageTime
         , IsAvailPartDmg = @IsAvailPartDmg
         , IsReportToSis = @IsReportToSis
         , EstimatedDelivery = @EstimatedDelivery
         , NotSendingCategory = @NotSendingCategory
         , EvtInfoA01 = @EvtInfoA01
         , EvtInfoA02 = @EvtInfoA02
         , EvtInfoA03 = @EvtInfoA03
         , EvtInfoA04 = @EvtInfoA04
         , EvtInfoA05 = @EvtInfoA05
         , EvtInfoA06 = @EvtInfoA06
         , EvtInfoA07 = @EvtInfoA07
         , EvtInfoA08 = @EvtInfoA08
         , EvtInfoB01 = @EvtInfoB01
         , EvtInfoB01Chk01 = @EvtInfoB01Chk01
         , EvtInfoB01Chk02 = @EvtInfoB01Chk02
         , EvtInfoB01Chk03 = @EvtInfoB01Chk03
         , EvtInfoB01Chk04 = @EvtInfoB01Chk04
         , EvtInfoB01Chk05 = @EvtInfoB01Chk05
         , EvtInfoB01Chk06 = @EvtInfoB01Chk06
         , EvtInfoB01Chk07 = @EvtInfoB01Chk07
         , EvtInfoB01Chk08 = @EvtInfoB01Chk08
         , EvtInfoB01Chk09 = @EvtInfoB01Chk09
         , EvtInfoB01Chk10 = @EvtInfoB01Chk10
         , EvtInfoB01Chk11 = @EvtInfoB01Chk11
         , EvtInfoB01Chk12 = @EvtInfoB01Chk12
         , EvtInfoB01Chk13 = @EvtInfoB01Chk13
         , EvtInfoB01Chk14 = @EvtInfoB01Chk14
         , EvtInfoB01Chk15 = @EvtInfoB01Chk15
         , EvtInfoB01Chk16 = @EvtInfoB01Chk16
         , EvtInfoB01Chk17 = @EvtInfoB01Chk17
         , EvtInfoB01Chk18 = @EvtInfoB01Chk18
         , EvtInfoB01Chk19 = @EvtInfoB01Chk19
         , EvtInfoB01Chk20 = @EvtInfoB01Chk20
         , EvtInfoB02 = @EvtInfoB02
         , EvtInfoB0201 = @EvtInfoB0201
         , EvtInfoB0202A = @EvtInfoB0202A
         , EvtInfoB0202B = @EvtInfoB0202B
         , EvtInfoB0203 = @EvtInfoB0203
         , EvtInfoB0301 = @EvtInfoB0301
         , EvtInfoB0302A = @EvtInfoB0302A
         , EvtInfoB0302B = @EvtInfoB0302B
         , EvtInfoB0303 = @EvtInfoB0303
         , EvtInfoB0401 = @EvtInfoB0401
         , EvtInfoB0402 = @EvtInfoB0402
         , EvtInfoB0403 = @EvtInfoB0403
         , EvtInfoB0404 = @EvtInfoB0404
         , EvtInfoB0501 = @EvtInfoB0501
         , EvtInfoB0502 = @EvtInfoB0502
         , EvtInfoB0503 = @EvtInfoB0503
         , EvtInfoC01 = @EvtInfoC01
         , EvtInfoC01Chk01 = @EvtInfoC01Chk01
         , EvtInfoC01Chk02 = @EvtInfoC01Chk02
         , EvtInfoC01Chk03 = @EvtInfoC01Chk03
         , EvtInfoC01Chk04 = @EvtInfoC01Chk04
         , EvtInfoC01Chk05 = @EvtInfoC01Chk05
         , EvtInfoC01Chk06 = @EvtInfoC01Chk06
         , EvtInfoC01Chk07 = @EvtInfoC01Chk07
         , EvtInfoC01Chk08 = @EvtInfoC01Chk08
         , EvtInfoC01Chk09 = @EvtInfoC01Chk09
         , EvtInfoC01Chk10 = @EvtInfoC01Chk10
         , EvtInfoC01Chk11 = @EvtInfoC01Chk11
         , EvtInfoC01Chk12 = @EvtInfoC01Chk12
         , EvtInfoC01Chk13 = @EvtInfoC01Chk13
         , EvtInfoC01Chk14 = @EvtInfoC01Chk14
         , EvtInfoC01Chk15 = @EvtInfoC01Chk15
         , EvtInfoC01Chk16 = @EvtInfoC01Chk16
         , EvtInfoC01Chk17 = @EvtInfoC01Chk17
         , EvtInfoC01Chk18 = @EvtInfoC01Chk18
         , EvtInfoC01Chk19 = @EvtInfoC01Chk19
         , EvtInfoC01Chk20 = @EvtInfoC01Chk20
         , EvtInfoC02Chk01 = @EvtInfoC02Chk01
         , EvtInfoC02Chk02 = @EvtInfoC02Chk02
         , EvtInfoC02Chk03 = @EvtInfoC02Chk03
         , EvtInfoC03Chk01 = @EvtInfoC03Chk01
         , EvtInfoC03Chk02 = @EvtInfoC03Chk02
         , EvtInfoC03Chk03 = @EvtInfoC03Chk03
         , EvtInfoC03Chk04 = @EvtInfoC03Chk04
         , EvtInfoC04Chk01 = @EvtInfoC04Chk01
         , EvtInfoC04Chk02 = @EvtInfoC04Chk02
         , EvtInfoC04Chk03 = @EvtInfoC04Chk03
         , EvtInfoC04Chk04 = @EvtInfoC04Chk04
         , EvtInfoC05 = @EvtInfoC05
         , EvtInfoC06Chk01 = @EvtInfoC06Chk01
         , EvtInfoC06Chk02 = @EvtInfoC06Chk02
         , EvtInfoC06Chk03 = @EvtInfoC06Chk03
         , EvtInfoC06Chk04 = @EvtInfoC06Chk04
         , EvtInfoC0701 = @EvtInfoC0701
         , EvtInfoC0702 = @EvtInfoC0702
         , EvtInfoC0801 = @EvtInfoC0801
         , EvtInfoC0802 = @EvtInfoC0802
         , EvtInfoC0803 = @EvtInfoC0803
         , EvtInfoC0804 = @EvtInfoC0804
         , EvtInfoC0805 = @EvtInfoC0805
         , EvtInfoC0806 = @EvtInfoC0806
         , EvtInfoC0807 = @EvtInfoC0807
         , EvtInfoC0808 = @EvtInfoC0808
         , EvtInfoC0809 = @EvtInfoC0809
         , EvtInfoC0901 = @EvtInfoC0901
         , EvtInfoC0902 = @EvtInfoC0902
         , EvtInfoC0902Chk01 = @EvtInfoC0902Chk01
         , EvtInfoC0902Chk02 = @EvtInfoC0902Chk02
         , EvtInfoC0902Chk03 = @EvtInfoC0902Chk03
         , EvtInfoC0902Chk04 = @EvtInfoC0902Chk04
         , EvtInfoC0902Chk05 = @EvtInfoC0902Chk05
         , EvtInfoC0902Chk06 = @EvtInfoC0902Chk06
         , EvtInfoC0902Chk07 = @EvtInfoC0902Chk07
         , EvtInfoC0902Chk08 = @EvtInfoC0902Chk08
         , EvtInfoC0903Chk01 = @EvtInfoC0903Chk01
         , EvtInfoC0903Chk02 = @EvtInfoC0903Chk02
         , EvtInfoC0903Chk03 = @EvtInfoC0903Chk03
         , EvtInfoC0903Chk04 = @EvtInfoC0903Chk04
         , EvtInfoC0903Chk05 = @EvtInfoC0903Chk05
         , EvtInfoC0903Chk06 = @EvtInfoC0903Chk06
         , EvtInfoC0904Chk01 = @EvtInfoC0904Chk01
         , EvtInfoC0904Chk02 = @EvtInfoC0904Chk02
         , EvtInfoC0904Chk03 = @EvtInfoC0904Chk03
         , EvtInfoC0904Chk04 = @EvtInfoC0904Chk04
         , EvtInfoC0904Chk05 = @EvtInfoC0904Chk05
         , EvtInfoC0904Chk06 = @EvtInfoC0904Chk06
         , EvtInfoC0904Chk07 = @EvtInfoC0904Chk07
         , EvtInfoC090501 = @EvtInfoC090501
         , EvtInfoC090502 = @EvtInfoC090502
         , EvtInfoC090503 = @EvtInfoC090503
         , EvtInfoC090504 = @EvtInfoC090504
         , EvtInfoC090601 = @EvtInfoC090601
         , EvtInfoC090602 = @EvtInfoC090602
         , UpdatedBy = @UserId
         , UpdatedDate = getdate()
     where FtirNo = @FtirNo
 end

select * from SvFtir where FtirNo = @FtirNo
