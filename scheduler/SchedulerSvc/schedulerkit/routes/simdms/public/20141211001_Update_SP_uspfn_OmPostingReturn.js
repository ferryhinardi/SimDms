var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var Client = require('q-svn-spawn');
var config = require('../config');
var http = require('http');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "UPDATE SP uspfn_OmPostingReturn";
var TaskNo = "20141211001";
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
ALTER procedure [dbo].[uspfn_OmPostingReturn]
	@CompanyCode varchar(20),
	@BranchCode  varchar(20),
	@DocNo       varchar(20),
	@UserID      varchar(20)
as 
declare @JournalGL table
(
	CodeTrans    varchar(50),
	DocNo        varchar(50),
	DocInfo      varchar(50),
	AccountNo    varchar(50),
	AmountDb     decimal,
	AmountCr     decimal
)

insert into @JournalGL
select '01 RETUR SALES UNIT', @DocNo, b.SalesModelCode
	 , isnull((
		select acc.ReturnAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = b.SalesModelCode
		), '')
	 , sum(isnull(b.Quantity, 0) * (isnull(BeforeDiscDPP, 0)))
	 , 0
  from omTrSalesReturn a
	left join omTrSalesReturnDetailModel b on b.CompanyCode = a.CompanyCode
	   and b.BranchCode  = a.BranchCode
	   and b.ReturnNo    = a.ReturnNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.ReturnNo    = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.ReturnNo, b.SalesModelCode
having sum(isnull(b.Quantity, 0) * (isnull(BeforeDiscDPP, 0))) > 0

insert into @JournalGL
select '02 RETUR AKSESORIS', @DocNo, b.SalesModelCode
	 , isnull((
		select acc.ReturnAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = b.SalesModelCode
		), '')
	 , sum(isnull(b.Quantity, 0) * (isnull(c.BeforeDiscDPP, 0)))
	 , 0
  from omTrSalesReturn a
	left join omTrSalesReturnDetailModel b on b.CompanyCode = a.CompanyCode
	   and b.BranchCode  = a.BranchCode
	   and b.ReturnNo    = a.ReturnNo
	left join (
		select CompanyCode,BranchCode,ReturnNo,BPKNo,SalesModelCode,SalesModelYear
			,sum(BeforeDiscDPP) BeforeDiscDPP
		from omTrSalesReturnOther
		group by CompanyCode,BranchCode,ReturnNo,BPKNo,SalesModelCode,SalesModelYear
	) c on c.CompanyCode = a.CompanyCode
		and c.BranchCode  = a.BranchCode and c.ReturnNo= a.ReturnNo and c.BPKNo=b.BPKNo
		and c.SalesModelCode = b.SalesModelCode and c.SalesModelYear=b.SalesModelYear
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.ReturnNo    = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.ReturnNo, b.SalesModelCode
having sum(isnull(b.Quantity, 0) * (isnull(c.BeforeDiscDPP, 0))) > 0


insert into @JournalGL
select '03 PPN', @DocNo, a.CustomerCode
	 , isnull((
		select acc.TaxOutAccNo
		  from GnMstCustomerProfitCenter cus, GnMstCustomerClass acc
		 where 1 = 1
		   and cus.CompanyCode    = acc.CompanyCode
		   and cus.BranchCode     = acc.BranchCode
		   and cus.CustomerClass  = acc.CustomerClass
		   and cus.CompanyCode    = a.CompanyCode
		   and cus.BranchCode     = a.BranchCode
		   and cus.CustomerCode   = a.CustomerCode
		   and acc.ProfitCenterCode = '100'
		), '')
	 , sum(isnull(b.Quantity, 0) * (isnull(b.AfterDiscPPn, 0)))
	 + sum(isnull(b.Quantity, 0) * (isnull(c.AfterDiscPPn ,0)))
	 + sum(isnull(d.Quantity, 0) * (isnull(d.PPN ,0)))
	 , 0
  from omTrSalesReturn a
	left join omTrSalesReturnDetailModel b on b.CompanyCode = a.CompanyCode
	   and b.BranchCode  = a.BranchCode
	   and b.ReturnNo    = a.ReturnNo
	left join (
		select CompanyCode,BranchCode,ReturnNo,BPKNo,SalesModelCode,SalesModelYear
			,sum(AfterDiscPPn) AfterDiscPPn
		from omTrSalesReturnOther
		group by CompanyCode,BranchCode,ReturnNo,BPKNo,SalesModelCode,SalesModelYear
	) c on c.CompanyCode = a.CompanyCode
		and c.BranchCode  = a.BranchCode and c.ReturnNo= a.ReturnNo and c.BPKNo=b.BPKNo
		and c.SalesModelCode = b.SalesModelCode and c.SalesModelYear=b.SalesModelYear
	left join (
		select CompanyCode,BranchCode,ReturnNo,sum(Quantity) Quantity, sum(PPN) PPN
		from omTrSalesReturnAccs 
		group by CompanyCode,BranchCode,ReturnNo
	)d on d.CompanyCode = a.CompanyCode
		and d.BranchCode  = a.BranchCode and d.ReturnNo= a.ReturnNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.ReturnNo    = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.ReturnNo, a.CustomerCode
having sum(isnull(b.Quantity, 0) * (isnull(b.AfterDiscPPn, 0)))
	 + sum(isnull(b.Quantity, 0) * (isnull(c.AfterDiscPPn ,0)))
	 + sum(isnull(d.Quantity, 0) * (isnull(d.PPN ,0))) > 0

insert into @JournalGL
select '04 PPN BM', @DocNo, a.CustomerCode
	 , isnull((
		select acc.LuxuryTaxAccNo
		  from GnMstCustomerProfitCenter cus, GnMstCustomerClass acc
		 where 1 = 1
		   and cus.CompanyCode    = acc.CompanyCode
		   and cus.BranchCode     = acc.BranchCode
		   and cus.CustomerClass  = acc.CustomerClass
		   and cus.CompanyCode    = a.CompanyCode
		   and cus.BranchCode     = a.BranchCode
		   and cus.CustomerCode   = a.CustomerCode
		   and acc.ProfitCenterCode = '100'
		), '')
	 , sum(isnull(b.Quantity, 0) * (isnull(b.AfterDiscPPnBm, 0)))
	 , 0
  from omTrSalesReturn a
	left join omTrSalesReturnDetailModel b on b.CompanyCode = a.CompanyCode
	   and b.BranchCode  = a.BranchCode
	   and b.ReturnNo    = a.ReturnNo
  where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.ReturnNo    = @DocNo
  group by a.CompanyCode, a.BranchCode, a.ReturnNo, a.CustomerCode
having sum(isnull(b.Quantity, 0) * (isnull(b.AfterDiscPPnBm, 0))) > 0

insert into @JournalGL
select '05 RETURN', @DocNo, b.SalesModelCode
	 , isnull((
		select acc.HReturnAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = b.SalesModelCode
		), '')
	 , 0
	 ,sum(isnull(b.Quantity, 0) * (isnull(b.AfterDiscDPP,0)+isnull(b.AfterDiscPPN,0)+isnull(b.AfterDiscPPNBm,0))) --Unit
	 + sum(isnull(b.Quantity, 0) * (isnull(c.AfterDiscDPP,0)+isnull(c.AfterDiscPPN,0))) -- Others
	 + sum(isnull(d.Quantity, 0) * (isnull(d.DPP,0)+isnull(d.PPN,0))) -- Acc
  from omTrSalesReturn a
	left join omTrSalesReturnDetailModel b on b.CompanyCode = a.CompanyCode
		and b.BranchCode  = a.BranchCode and b.ReturnNo= a.ReturnNo
	left join (
		select CompanyCode,BranchCode,ReturnNo,BPKNo,SalesModelCode,SalesModelYear
			,sum(AfterDiscDPP) AfterDiscDPP,sum(AfterDiscPPN) AfterDiscPPN
		from omTrSalesReturnOther
		group by CompanyCode,BranchCode,ReturnNo,BPKNo,SalesModelCode,SalesModelYear
	) c on c.CompanyCode = a.CompanyCode
		and c.BranchCode  = a.BranchCode and c.ReturnNo= a.ReturnNo and c.BPKNo=b.BPKNo
		and c.SalesModelCode = b.SalesModelCode and c.SalesModelYear=b.SalesModelYear
	left join (
		select CompanyCode,BranchCode,ReturnNo,sum(Quantity) Quantity
			, sum(DPP) DPP, sum (PPN) PPN
		from omTrSalesReturnAccs 
		group by CompanyCode,BranchCode,ReturnNo
	)d on d.CompanyCode = a.CompanyCode
		and d.BranchCode  = a.BranchCode and d.ReturnNo= a.ReturnNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.ReturnNo    = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.ReturnNo, b.SalesModelCode
having sum(isnull(b.Quantity, 0) * (isnull(b.AfterDiscDPP,0)+isnull(b.AfterDiscPPN,0)+isnull(b.AfterDiscPPNBm,0))) 
	 + sum(isnull(b.Quantity, 0) * (isnull(c.AfterDiscDPP,0)+isnull(c.AfterDiscPPN,0))) 
	 + sum(isnull(d.Quantity, 0) * (isnull(d.DPP,0)+isnull(d.PPN,0))) > 0

insert into @JournalGL
select '06 DISCOUNT UNIT', @DocNo, b.SalesModelCode
	 , isnull((
		select acc.DiscountAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		  and acc.SalesModelCode = b.SalesModelCode
		), '')
	 , 0
	 , sum(isnull(b.Quantity, 0) * (isnull(b.DiscExcludePPn, 0)))  -- Disc 
  from omTrSalesReturn a
	left join omTrSalesReturnDetailModel b on b.CompanyCode = a.CompanyCode
   and b.BranchCode  = a.BranchCode
   and b.ReturnNo    = a.ReturnNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.ReturnNo    = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.ReturnNo, b.SalesModelCode
having sum(isnull(b.Quantity, 0) * (isnull(b.DiscExcludePPn, 0))) > 0

insert into @JournalGL
select '07 DISCOUNT AKSESORIS', @DocNo, b.SalesModelCode
	 , isnull((
		select acc.DiscountAccNoAks
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		  and acc.SalesModelCode = b.SalesModelCode
		), '')
	 , 0
	 , sum(isnull(b.Quantity, 0) * (isnull(c.DiscExcludePPn, 0)))  -- Disc 
  from omTrSalesReturn a
	left join omTrSalesReturnDetailModel b on b.CompanyCode = a.CompanyCode
	   and b.BranchCode  = a.BranchCode
	   and b.ReturnNo    = a.ReturnNo
	left join (
		select CompanyCode,BranchCode,ReturnNo,BPKNo,SalesModelCode,SalesModelYear
			,sum(DiscExcludePPn) DiscExcludePPn
		from omTrSalesReturnOther
		group by CompanyCode,BranchCode,ReturnNo,BPKNo,SalesModelCode,SalesModelYear
	) c on c.CompanyCode = a.CompanyCode and c.BranchCode = a.BranchCode and c.ReturnNo = a.ReturnNo
 where 1 = 1
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.ReturnNo    = @DocNo 
 group by a.CompanyCode, a.BranchCode, a.ReturnNo, b.SalesModelCode
having sum(isnull(b.Quantity, 0) * (isnull(b.DiscExcludePPn, 0))) > 0

insert into @JournalGL
select '08 INVENTORY UNIT', @DocNo, b.SalesModelCode
	 , isnull((
		select acc.InventoryAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = b.SalesModelCode
		), '')
	 , sum(isnull(c.COGS, 0))
	 , 0
  from omTrSalesReturn a, OmTrSalesReturnVin b, OmTrSalesInvoiceVin c
 where 1 = 1
   and c.CompanyCode = b.CompanyCode
   and c.BranchCode  = b.BranchCode
   and c.SalesModelCode = b.SalesModelCode
   and c.SalesModelYear = b.SalesModelYear
   and c.EngineCode  = b.EngineCode
   and c.EngineNo    = b.EngineNo
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode  = a.BranchCode
   and b.ReturnNo    = a.ReturnNo
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.ReturnNo    = @DocNo 
   and a.InvoiceNo	 = c.InvoiceNo
 group by a.CompanyCode, a.BranchCode, a.ReturnNo, b.SalesModelCode
having sum(isnull(c.COGS, 0)) > 0

insert into @JournalGL
select '09 HPP UNIT', @DocNo, b.SalesModelCode
	 , isnull((
		select acc.COGSAccNo
		  from omMstModelAccount acc
		 where 1 = 1
		   and acc.CompanyCode    = a.CompanyCode
		   and acc.BranchCode     = a.BranchCode
		   and acc.SalesModelCode = b.SalesModelCode
		), '')
	 , 0
	 , sum(isnull(c.COGS, 0))
  from omTrSalesReturn a, OmTrSalesReturnVin b, OmTrSalesInvoiceVin c
 where 1 = 1
   and c.CompanyCode = b.CompanyCode
   and c.BranchCode  = b.BranchCode
   and c.SalesModelCode = b.SalesModelCode
   and c.SalesModelYear = b.SalesModelYear
   and c.EngineCode  = b.EngineCode
   and c.EngineNo    = b.EngineNo
   and b.CompanyCode = a.CompanyCode
   and b.BranchCode  = a.BranchCode
   and b.ReturnNo    = a.ReturnNo
   and a.CompanyCode = @CompanyCode 
   and a.BranchCode  = @BranchCode
   and a.ReturnNo    = @DocNo 
   and a.InvoiceNo	 = c.InvoiceNo
 group by a.CompanyCode, a.BranchCode, a.ReturnNo, b.SalesModelCode
having sum(isnull(c.COGS, 0)) > 0 

if exists (select * from @JournalGL where isnull(AccountNo, '') = '' and (AmountDb>0 or AmountCr>0))
begin
	raiserror('terdapat transaksi yang belum memiliki AccountNo',16 ,1 );
	return
end

if (select abs(sum(AmountDb) - sum(AmountCr)) from @JournalGL) > 0
begin
	raiserror('journal belum balance, mohon di check kembali',16 ,1 );
	return
end

--select * from arInterface where DocNo = @DocNo
--delete arInterface where DocNo = @DocNo
insert into arInterface
(CompanyCode,BranchCode,DocNo,DocDate,ProfitCenterCode
,NettAmt,ReceiveAmt,CustomerCode,TOPCode,DueDate,TypeTrans
,BlockAmt,DebetAmt,CreditAmt,SalesCode,LeasingCode,StatusFlag
,CreateBy,CreateDate,AccountNo,FakturPajakNo,FakturPajakDate)
select @CompanyCode CompanyCode, @BranchCode BranchCode
	 , b.ReturnNo, b.ReturnDate
	 , '100' as ProfitCenterCode 
	 , sum(a.AmountCr) as NettAmt, 0 ReceiceAmt
	 , b.CustomerCode, null, b.ReturnDate
	 , substring(CodeTrans, 4, len(CodeTrans) - 3) TypeTrans
	 , 0 as BlockAmt, 0 as DbAmt, 0 CrAmt
	 , '' Salesman, '' LeasingCo
	 , '0' StatusFlag, @UserID CreatedBy, getdate() CreatedDate
	 , a.AccountNo, b.FakturPajakNo, b.FakturPajakDate
  from @JournalGL a, omTrSalesReturn b --, omTrPurchaseHPP c
 where substring(CodeTrans, 4, len(CodeTrans) - 3) = 'RETURN'
   and b.CompanyCode = @CompanyCode
   and b.BranchCode  = @BranchCode
   and b.ReturnNo    = @DocNo
group by b.ReturnNo, b.ReturnDate, b.CustomerCode
	, substring(CodeTrans, 4, len(CodeTrans) - 3)
	, a.AccountNo, b.FakturPajakNo, b.FakturPajakDate

--select * from glInterface where DocNo = @DocNo
--delete glInterface where DocNo = @DocNo
insert into glInterface
(CompanyCode,BranchCode,DocNo,SeqNo,DocDate,ProfitCenterCode
,AccDate,AccountNo,JournalCode,TypeJournal,ApplyTo,AmountDb,AmountCr
,TypeTrans,BatchNo,BatchDate,StatusFlag
,CreateBy,CreateDate,LastUpdateBy,LastUpdateDate)
select @CompanyCode CompanyCode, @BranchCode BranchCode
	 , b.ReturnNo DocNo, (row_number() over (order by CodeTrans)) SeqNo
	 , b.ReturnDate DocDate, '100' ProfitCenterCode
	 , b.ReturnDate AccDate, a.AccountNo
	 , 'UNIT' JournalCode, 'RETURN' TypeJournal
	 , b.InvoiceNo ApplyTo, sum(a.AmountDb) AmountDb, sum(a.AmountCr) AmountCr
	 , substring(CodeTrans, 4, len(CodeTrans) - 3) TypeTrans
	 , '' BatchNo, null BatchDate
	 , '0' StatusFlag, @UserID CreatedBy, getdate() CreatedDate
	 , @UserID LastUpdBy, getdate() LastUpdDate
  from @JournalGL a, omTrSalesReturn b
 where 1 = 1
   and b.CompanyCode = @CompanyCode
   and b.BranchCode  = @BranchCode
   and b.ReturnNo    = @DocNo
   and (a.AmountDb   > 0 or a.AmountCr > 0)
group by b.ReturnNo, b.ReturnDate, CodeTrans, a.AccountNo, b.InvoiceNo

update omTrSalesReturn
   set Status = '5'
 where 1 = 1
   and CompanyCode = @CompanyCode
   and BranchCode  = @BranchCode
   and ReturnNo    = @DocNo
GO
PRINT 'ALTER SP DONE!'
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


var start1 = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 

	var xSQL= [], sql = SQL.split('\nGO');	
    sql.forEach(ExecuteSQL);

    async.series(xSQL, function (err, docs) {
        if (err) log("ERROR" + err);
		log('done');
    });	
	
	function ExecuteSQL(s){
        xSQL.push(function(callback){		
			
            sqlODBC.query(cfg.ConnString,s , function (err, data) { 
				if (err) { 
					log("ERROR >> " + err);
				} else {
					if (data && data.length == 1) {
						if (data[0].info !== undefined)
							log(data[0].info);
					}
				}
                callback(); 
            });		
        });
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

startTasks();