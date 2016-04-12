/*
> Menu Id dan Menu Caption Terserah Pak Hasim, 
  Kalau kurang tepat diganti aja pak untuk menu id dan menu caption nya. 
> Untuk Sp ada di tbsdmsap01 database BITJKT 
  Nama Spnya : usprpt_GnGenerateCsvSkemaTaxOut
Thanks :) 
*/
if not exists (Select * from SysMenuDms where menuid = 'GnRpSkemaTaxOut')
begin 
	INSERT INTO [dbo].[SysMenuDms]
			   ([MenuId]
			   ,[MenuCaption]
			   ,[MenuHeader]
			   ,[MenuIndex]
			   ,[MenuLevel]
			   ,[MenuUrl]
			   ,[MenuIcon])
		 VALUES
			   ('GnRpSkemaTaxOut'
			   ,'Laporan eFaktur Pajak Keluaran'
			   ,'taxReport'
			   ,'7'
			   ,'2'
			   ,'report/skemataxout'
			   ,'')
end


