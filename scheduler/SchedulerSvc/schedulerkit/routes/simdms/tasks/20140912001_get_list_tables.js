var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var config = require('../config');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "get_list_tables_info";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!
if object_id('uspfn_SysDealerGetDatabaseInfo') is not null
drop PROCEDURE uspfn_SysDealerGetDatabaseInfo
GO
CREATE PROCEDURE [dbo].[uspfn_SysDealerGetDatabaseInfo]
AS

declare @TableName varchar(100),
		@row_count int,
		@col_count int,
		@column_list VARCHAR(MAX) 

CREATE TABLE #temp (
table_name sysname ,
row_count INT,
reserved_size VARCHAR(50),
data_size VARCHAR(50),
index_size VARCHAR(50),
unused_size VARCHAR(50))

CREATE TABLE #temp2 (
table_name VARCHAR(100),
row_count INT,
col_count INT,
col_list VARCHAR(MAX)
)

SET NOCOUNT ON
INSERT #temp
EXEC sp_msforeachtable 'sp_spaceused ''?'''

declare crListTableInfo cursor for 
SELECT a.table_name,
a.row_count,
COUNT(*) AS col_count
FROM #temp a
INNER JOIN information_schema.columns b
ON a.table_name collate database_default
= b.table_name collate database_default
GROUP BY a.table_name, a.row_count

OPEN crListTableInfo
FETCH NEXT FROM crListTableInfo
INTO @TableName, @row_count, @col_count

WHILE @@FETCH_STATUS = 0
BEGIN
	
	set @column_list = null

	SELECT @column_list = COALESCE(@column_list + ', ', '') + COLUMN_NAME 
	FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME=@TableName

	insert into #temp2 values (@TableName, @row_count, @col_count,@column_list)

	FETCH NEXT FROM crListTableInfo
	INTO @TableName, @row_count, @col_count

END

CLOSE crListTableInfo
DEALLOCATE crListTableInfo

select * from #temp2

DROP TABLE #temp

DROP TABLE #temp2
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
var start = function (cfg, callback) {

    config.log("Tasks", "Starting " + TaskName + " for " + CurrentDealerCode); 

    var xSQL= [], sql = SQL.split('\nGO');	
    sql.forEach(ExecuteSQL);

    async.series(xSQL, function (err, docs) {
        if (err) config.log("ERROR", err);	
        config.log("Tasks", TaskName + " has been executed"); 	
		
		sqlODBC.query(cfg.ConnString, "uspfn_SysDealerGetDatabaseInfo" , function (err, data) {    
				if (data !== undefined)
				{	var iNomor = 1;		
					async.each(data, WriteData, function (err) {
						if (err) config.log(cfg.DealerCode, "Error on processing uspfn_SysDealerGetDatabaseInfo  : " + err);
						callback();
					});				
					function WriteData(row, callback) {
						var Param = {
							DealerCode : cfg.DealerCode,
							TableName : row.table_name,
							rows : row.row_count,
							cols : row.col_count,
							columns: row.col_list							
						}
						var url = config.api().downloadLink + 'api/adddealertable';
						request.post(url, {form:Param}, function (e, r, body){
							if (e) console.log(e);
							console.dir(['NO: ' + iNomor++, 'Dealer Code: ' + cfg.DealerCode, 'Table Name: ' + row.table_name, body]);
							callback();
						});														
					}				
				} else {					
					callback();
				}
		});		
        
    });	

    function ExecuteSQL(s){
        xSQL.push(function(callback){
            sqlODBC.query(cfg.ConnString, s , function (err, data) {    
                if (err) config.log("Tasks", err);                  
                callback(); 
            });		
        });
    }	
}
startTasks();