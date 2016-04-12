var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var config = require('../config');
var http = require('http');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "GET_SPHSTPARTSALES";
var TaskNo = "20141121001";
var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQLCheck = hereDoc(function(){/*!
declare @column_list varchar(MAX)
SELECT @column_list = COALESCE(@column_list + ', ', '') + COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
SELECT @column_list list, COUNT(*) total
FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
*/});

var SQL = hereDoc(function(){/*!
if object_id('uspfn_spPartSalesPOSTING') is not null
	drop procedure uspfn_spPartSalesPOSTING
GO
CREATE PROCEDURE [dbo].[uspfn_spPartSalesPOSTING]
AS
BEGIN
	declare @PeriodStart date
	declare @PeriodEnd   date
	declare @curDate	 datetime
	declare @CompanyCode varchar(15)

 -- Setup collection date
    set @curDate     = getdate()
    set @PeriodStart = '2000/01/01'
    set @PeriodEnd   = '2014/12/31'

	set @CompanyCode = isnull((select top 1 CompanyCode from gnMstOrganizationHdr),'9999999')

 -- Setup Part Sales table
	if not exists (select * from sys.objects 
                     where object_id = object_id(N'[spHstPartSales]') and type=N'U')
		create table [spHstPartSales]
			(
			[RecordID]			[uniqueidentifier]	NOT NULL,
			[RecordDate]		[datetime]			NOT NULL,
			[CompanyCode]		[varchar](15)		NOT NULL,
			[BranchCode]		[varchar](15)		NOT NULL,
			[InvoiceNo]			[varchar](15)		NOT NULL,
			[InvoiceDate]		[datetime]			NOT NULL,
			[FPJNo]				[varchar](15)		NOT NULL,
			[FPJDate]			[datetime]			NOT NULL,
			[CustomerCode]		[varchar](15)		NOT NULL,
			[CustomerName]		[varchar](100)		NOT NULL,
			[CustomerClass]		[varchar](15)		NOT NULL,
			[PartNo]			[varchar](20)		NOT NULL,
			[PartName]			[varchar](100)		NULL,
			[TypeOfGoods]		[char]   (1)		NOT NULL,
			[TypeOfGoodsDesc]	[varchar](30)		NOT NULL,
			[QtyBill]			[numeric]( 9,2)		NOT NULL,
			[CostPrice]			[numeric](12,0)		NOT NULL,
			[RetailPrice]		[numeric](12,0)		NOT NULL,
			[DiscPct]			[numeric]( 5,2)		NOT NULL,
			[DiscAmt]			[numeric](12,0)		NOT NULL,
			[NetSalesAmt]		[numeric](12,0)		NOT NULL,
			[SendDate]			[datetime]			NULL,
			constraint [PK_spHstPartSales] primary key clustered 
			( [RecordID] ASC)
			with (pad_index=OFF, statistics_norecompute=OFF, ignore_dup_key=OFF, 
					allow_row_locks=ON, allow_page_locks=ON) ON [PRIMARY]
			) on [PRIMARY]

 -- Data Collection
	select * into #SGO
	  from ( select h.CompanyCode, h.BranchCode, h.InvoiceNo, h.InvoiceDate, 
					h.FPJNo, h.FPJDate, h.CustomerCode, c.CustomerName, 
					CustomerClass = case when c.CategoryCode='00' then 'MAIN DEALER'
										 when c.CategoryCode='01' then 'SUB DEALER'
										 when c.CategoryCode='15' then 'PART SHOP'
										 else                          'DIRECT CUSTOMER'
									end,
					d.PartNo, i.PartName, h.TypeOfGoods, 
					TypeOfGoodsDesc = isnull((select LookUpValueName from gnMstLookUpDtl
											   where CompanyCode=h.CompanyCode
												 and CodeID='TPGO'
												 and LookUpValue=h.TypeOfGoods),''),
					d.QtyBill, d.CostPrice, d.RetailPrice, d.DiscPct, d.DiscAmt, d.NetSalesAmt
			   from spTrnSInvoiceHdr h
			  inner join gnMstCustomer c
					  on h.CompanyCode=c.CompanyCode
					 and h.CustomerCode=c.CustomerCode
			  inner join spTrnSInvoiceDtl d
					  on h.CompanyCode=d.CompanyCode
					 and h.BranchCode=d.BranchCode
					 and h.InvoiceNo=d.InvoiceNo
			  inner join spMstItemInfo i
					  on d.CompanyCode=i.CompanyCode
					 and d.PartNo=i.PartNo
	 where 1=1  --h.TypeOfGoods in ('1','2','5')  -- 0:SGP, 1:SGO, 2:SGA, 3:NON SGP, 4:OTHERS, 5:NON SGA
	   and convert(varchar,h.InvoiceDate,111) between @PeriodStart and @PeriodEnd
	   and not exists (select top 1 1 from spHstPartSales
	                    where CompanyCode=h.CompanyCode
						  and BranchCode =h.BranchCode
						  and InvoiceNo  =h.InvoiceNo
						  and FPJNo      =h.FPJNo
						  and PartNo     =d.PartNo)
	 union all
	select h.CompanyCode, h.BranchCode, h.LmpNo InvoiceNo, h.CreatedDate InvoiceDate, 
		   h.BPSFNo, h.BPSFDate, h.CustomerCode, c.CustomerName, 
		   'SERVICE' CustomerClass, d.PartNo, i.PartName, h.TypeOfGoods, 
		   TypeOfGoodsDesc = isnull((select LookUpValueName from gnMstLookUpDtl
									  where CompanyCode=h.CompanyCode
										and CodeID='TPGO'
										and LookUpValue=h.TypeOfGoods),''),
		   d.QtyBill, d.CostPrice, d.RetailPrice, d.DiscPct, d.DiscAmt, d.NetSalesAmt
	  from spTrnSLmpHdr h
	 inner join gnMstCustomer c
			 on h.CompanyCode=c.CompanyCode
			and h.CustomerCode=c.CustomerCode
	 inner join spTrnSLmpDtl d
			 on h.CompanyCode=d.CompanyCode
			and h.BranchCode=d.BranchCode
			and h.LmpNo=d.LmpNo
	 inner join spMstItemInfo i
			 on d.CompanyCode=i.CompanyCode
			and d.PartNo=i.PartNo
	 where 1=1 --h.TypeOfGoods in ('1','2','5')  -- 0:SGP, 1:SGO, 2:SGA, 3:NON SGP, 4:OTHERS, 5:NON SGA
	   and convert(varchar,h.LmpDate,111) between @PeriodStart and @PeriodEnd 
	   and not exists (select top 1 1 from spHstPartSales
	                    where CompanyCode=h.CompanyCode
						  and BranchCode =h.BranchCode
						  and InvoiceNo  =h.LmpNo
						  and FPJNo      =h.BPSFNo
						  and PartNo     =d.PartNo)
		   ) a

 -- insert to Part Sales table
	insert into spHstPartSales  select NEWID(), @curDate, *, NULL from #SGO

 -- select Part Sales data
 -- select * from spHstPartSales
	select h.CompanyCode, dm.DealerAbbreviation, h.BranchCode, do.OutletAbbreviation, h.InvoiceNo, h.InvoiceDate, 
		   h.FPJNo, h.FPJDate, h.CustomerCode, h.CustomerName, h.CustomerClass, h.PartNo, h.PartName, h.TypeOfGoods, 
		   h.TypeOfGoodsDesc, h.QtyBill, h.CostPrice, h.RetailPrice, h.DiscPct, h.DiscAmt, h.NetSalesAmt
	  from spHstPartSales h
	  left join gnMstDealerMapping dm
			on h.CompanyCode=dm.DealerCode
	  left join gnMstDealerOutletMapping do
			on h.CompanyCode=do.DealerCode
		   and h.BranchCode=do.OutletCode
     where h.RecordDate=@curDate
	 order by dm.DealerAbbreviation, h.BranchCode, h.InvoiceDate, h.PartNo
	drop table #SGO
END
GO
EXEC uspfn_spPartSalesPOSTING
*/});


var startTasks= function(callback)
{
    var taskJobs = [];		
    config.conn().forEach(listWorker);		
    async.series(taskJobs, function (err, docs) {
        if (err) config.log("Tasks", err);			
        if (callback) callback();
    });
    function listWorker(cfg){
        if(cfg.DealerCode == CurrentDealerCode)
        {
            taskJobs.push(function(callback){start(cfg, callback);});
        }			 
    }				
}


var start = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 

	var file = path.join(__dirname,  TaskNo + ".sql");
	log(file);
	
    fs.writeFileSync(file, SQL );
	
	var i = cfg.ConnString.indexOf('}');
	var s = (cfg.ConnString.substr(i+2));
	
	s = s.replace('Server=','');
	i = s.indexOf(';');
	var server = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Database=','');
	i = s.indexOf(';');
	var dbname = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Uid=','');
	i = s.indexOf(';');
	var userid = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Pwd=','');
	var password = s;
		
	var StartTime = moment().format("YYYY-MM-DD HH:mm:ss");
	
    run_shell ('osql', ['-S',server,'-d',dbname,'-U',userid,'-P',password,'-i',file] ,function(err, code, result) {
	
        var output = result;						
        var FinishTime = moment().format("YYYY-MM-DD HH:mm:ss");                        
        var errDesc = err || '';
		
		log(result);
        
    });	
	
	log('done');
	
}


function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}


function run_shell(cmd, args, cb)
{
	var proc = require('child_process').spawn(cmd, args)
	var result = '', strErr = '';
	
	proc.stdout.on('data', function (data) {
		result += data;
	});
	proc.stderr.on('data', function (data) {
		strErr += data;
	});
	proc.on('close', function (code) {		
		cb(strErr,code,result);
	});
}

var download = function(url, dest, cb) {
  var file = fs.createWriteStream(dest);
  var request = http.get(url, function(response) {
    response.pipe(file);
    file.on('finish', function() {
      file.close(cb);  // close() is async, call cb after close completes.
    });
  }).on('error', function(err) { // Handle errors
    fs.unlink(dest); // Delete the file async. (But we don't check the result)
    if (cb) cb(err.message);
  });
};


startTasks();