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
var MyScokets = {};
var pageSize = 17500;

module.exports = {
	version: function() {
		return '2014.11.27 v1.0.1';
	},
	service: function()
	{
		var self = this;	
		var IsServiceOnly = false;		
		config.log("SERVICE", "Starting Application");		
		
		setTimeout(function(){		
			if (!IsServiceOnly)
			{
				self.startAllPartial();
			}			
			self.startAllJob();			
			config.conn().forEach(notificationSvc);				
			function notificationSvc(cfg)
			{
				MyScokets[cfg.DealerCode] = (require('child_process').fork('./lib/notificationsvc',['-d',cfg.DealerCode]));		
				//console.log(MyScokets[cfg.DealerCode]);
				MyScokets[cfg.DealerCode].on('exit', function(err){
					console.log('Socket client down .... restart it again');
					notificationSvc(cfg);				
				});
			}			
		}, 10000);		
		
		if(!IsServiceOnly){
			setInterval(function () {
				self.startAllPartial(function(Err)
				{
					console.log("Backup All Finished");
				});
			}, 1000 *  60 * 60 * 3);
		}
		
		setInterval(function () {
			self.startAllJob(function(Err)
			{
				if (Err) console.log(Err);
			});
		}, 1000 *  60 * 30);		
	},	
	
	startAllPartial: function(callback)
	{
		var self = this;
		var tasksBackup = [];
		
		function listWorker(cfg) 
		{
			tasksBackup.push(function(callback){self.startBackupPartial(cfg, '', callback);});
		}
		
		function cleanUp(cfg) 
		{
			sqlODBC.queryRaw(cfg.ConnString, "select 1 a", function (err, data) {
				if (err) {
					config.log("ERROR", err);
				}
				data = null;
				global.gc();
				config.log(cfg.DealerCode, "Memory Usage: " + util.inspect(process.memoryUsage()));
			});
		}
		
		config.conn().forEach(listWorker);
		
		async.series(tasksBackup, function (err, docs) {
			if (err) config.log("ERROR", err);			
			config.conn().forEach(cleanUp);					
			if (callback) callback(null, docs);			
		});
	},	
	
	startBackupTable: function(tableName, callback)
	{
		var self = this;
		var tasksBackup = [];
		
		function listWorker(cfg) 
		{
			tasksBackup.push(function(callback){self.startBackupPartial(cfg, tableName, callback);});
		}
		
		function cleanUp(cfg) 
		{
			sqlODBC.queryRaw(cfg.ConnString, "select 1 a", function (err, data) {
				if (err) {
					config.log("ERROR", err);
				}
				data = null;
				global.gc();
				config.log(cfg.DealerCode, "Memory Usage: " + util.inspect(process.memoryUsage()));
			});
		}		
		config.conn().forEach(listWorker);		
		async.series(tasksBackup, function (err, docs) {
			if (err) config.log("ERROR", err);			
			config.conn().forEach(cleanUp);		
			//console.log(docs);			
			if (callback) callback(null, docs);			
		});
	},	
	
	
	getkeys: function (cfg1, options, callback) {
        var self = this;		
		var tableName = options.TableName.toLowerCase();
		var IsView = tableName.substr(tableName.length - 4,4) == "view";
		if (IsView) {
			tableName = tableName.substr(0,tableName.length-4);
		}			
        var query = "exec sp_pkeys " + tableName;
        sqlODBC.query(cfg1.ConnString, query, function (err, data) {
            if (err) throw err;
            var keys = []; 
			(data || []).forEach(function (row) {
                keys.push(row["COLUMN_NAME"]);
            });			
			if ( options.TableKeys != null)
			{
				callback(null,options.TableKeys);
			} else {
				callback(null,keys.join(','));
			}
        });
    },
	
	startBackupPartial: function (cfg, pTableName, callback) {
        var self = this;
        var params = {
            uri: config.api().listTables,
            method: "POST",
            form: {                    
                DealerCode: cfg.DealerCode,
				DealerType: cfg.DealerType,
				TableName: pTableName
            }
        };	
        request(params, function (e, r, bodyX) {
			if (e) config.log("ERROR", e);
			else {
				var dataX = bodyX.substr(1, bodyX.length - 2);
				var response = JSON.parse(dataX);  

				async.eachSeries(response, function(data, callback){
					config.log(cfg.DealerCode, "starting backup table " + data.TableName );
					self.prepareBackupPartial(cfg, data, function(err){
						if (err) console.log(err);
						config.log(cfg.DealerCode, "Backup table " + data.TableName + " finished");
						if (callback) callback(err, data);
					});					
				}
				, function (err) {
					if (err) console.log(err);
					console.log('');
					if (callback) callback();
				});

			}			
        });
    },
	
	prepareBackupPartial : function(cfg1, table, callback)
	{
		var self = this;		
		var sqlquery = "select count(*) as count from " + table.TableName + " where 1 = 1";		
		if (table.FilterBy !== undefined && table.FilterValue > "2000-01-01") {
			sqlquery = sqlquery + " and " + table.FilterBy + " >= '" + table.FilterValue + "'" ;
		}

		sqlODBC.query(cfg1.ConnString, sqlquery, function (err, data) {
			if (err) {
				config.log(cfg1.DealerCode, "Error on " + table.TableName + " : " + err );
				callback();
			} else {
				var count = data[0].count;
				
				var nBackupDb = parseInt(count / pageSize);
				var tb5 = [];						
					
				if (count==0)
				{
					console.log('Record Not Found');
					if (callback) callback();					
				} else {
					self.getkeys(cfg1, table, function(err,keys) {
						//console.log(table.TableName,' : ', keys);
						for(var iPage=0; iPage <= nBackupDb; iPage++)
						{
							tb5.push(iPage);
						}
						async.eachSeries(tb5, multipleBackup, function (err) {
							if (err) config.log("ERROR", err);
							console.log('');
							if (callback) callback();
						});						
						function multipleBackup (page, callback)
						{
							var start = page * pageSize + 1, finish = (page+1) * pageSize;
							if (page == nBackupDb) finish = start + (count % pageSize) - 1;						
							self.BackupPartial(cfg1, table, start,finish, keys, function(err){
								if(err) console.log(err);
								if (callback) callback();
							});						
						}
					});	
				}
			}			
		});
	
	},	
	
	BackupPartial: function(cfg1, table, xStart, xFinish , keys, callback)
	{
        var self = this;
        var IndexNo = 1;
		var TotalAvailableRecords = 0;
		
        config.log(cfg1.DealerCode, "**************************************************************************");  
        config.log(cfg1.DealerCode, "Starting backup database - " + cfg1.DealerCode + "." + table.TableName + " >> " + xStart + " to " + xFinish);  
		
        var filedb = cfg1.DealerCode + "_" + cfg1.DealerType + moment().format("_YYYYMMDD") + "_" + table.TableName + "_Part" + (xFinish < pageSize ? pageSize : xFinish)/pageSize; 
		var srcFile = "log/" + filedb + ".db3";
        var db = config.sqlite3(srcFile);

        db.query('BEGIN');		
		db.query('CREATE TABLE BACKUP_INFO (code TEXT, name TEXT, count int, lastupdate datetime)');
        config.log(cfg1.DealerCode, "Retrieve table list from server");		
		config.log(cfg1.DealerCode, "Generate contents for backup database");
		//config.log(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");			
		gettable(table, function(err){		
			if (err) config.log("ERROR", err);		
			db.query('COMMIT', function(err, rows) {
				config.log(cfg1.DealerCode, "COMMIT");
				db.query('select sum(count) from backup_info', function(err, rows) {
					TotalAvailableRecords = rows[0];
					config.log(cfg1.DealerCode, "Changes: " + TotalAvailableRecords + " row(s)");					
					setTimeout(function(){
						//config.log(cfg1.DealerCode, "Saving backup database (memory to file db)");
						async.series([
							function(cb1){
								db.close();
								cb1(null, 'DB connection has been closed');
							},
							function(cb2){
								zipDb(function(err){
									cb2(null, 'Backup database has been sent!');
								});								
							}
						],
						// optional callback
						function(err, results){
							//console.log(results);
							callback();
						});						
					}, 1000);
				});	
			}); 		
		});

		function gettable(table, callback) {
			if (table.FilterBy == undefined) {
				callback();
			}
			else {
				sqlODBC.query(cfg1.ConnString, "exec uspfn_CreateSQLiteTable '" + table.TableName + "'", function (err, data) {    
					if (err) {
							config.log(cfg1.DealerCode, "Error on " + table.TableName + " >> " + err);
							callback();
					} else {
					var row = data[0];
					db.query(row.DDL, function(err, rows) {
						var task = {
							name: table.TableName,
							No: IndexNo++,
							DB: db,
							DML: row.DML,
							DealerCode: cfg1.DealerCode,
							columns: row.ColumnList,
							segment: 1000,
							conn: cfg1.ConnString,
							filterBy: table.FilterBy,
							filterValue: table.FilterValue,
							start: xStart,
							finish: xFinish,
							key: keys
						} 
						self.backupTableReadAll(db, task, function (result) {
							task = null;
							callback(null, result);
						});
					});   
					}                     
				});   
			}
		} 

        //db.on('close', function (code) {		
		function zipDb(callback) {		
			//config.log(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
			db = null;						
			if (TotalAvailableRecords > 0){
			//config.log(cfg1.DealerCode, "start compressing database file (zip file)");						
			var filezip = "log/temp/" + filedb + ".zip";			
			var output = fs.createWriteStream(filezip);			
			var archive = archiver('zip',{ zlib:{level:9} });			
			output.on('close', function() {				
				config.log(cfg1.DealerCode, "Backup database finished");
				//config.log(cfg1.DealerCode, "Save to " + filezip);
				fs.unlinkSync(srcFile);
				
				var isProgress = false;
				var iTime = 0;
				
				var timeCheck = setInterval(function () 
				{
					if (!isProgress)
					{
						isProgress = true;
						sendFile(filedb, filezip, function(err){
							if (err) {
								isProgress = false;
							} else {
								isCompleted = true;
								clearInterval(timeCheck);
								//console.log('Uploaded');
								if (callback)  callback();
							}
						});	
					} else {
						console.log('In progress ... (uploading) >> ' + ++iTime);
					}
				}, 1000);	
				srcFile = null;				
				archive = null;
			});			
			archive.on('error', function(err) {
				if (err) config.log("ERROR", err);
			});			
			archive.pipe(output);
			archive.append(fs.createReadStream(srcFile), { name: filedb + ".db3" })
			archive.finalize();				
			} else {
				fs.unlinkSync(srcFile);
				if (callback) callback();
			}		
			
        };	
		
		function sendFile(file, fileName, cb) {
			var dealercode = file.split("_")[0];
			var url = config.api().upload;			
            var req = request.post(url, function (e, r, body) {
				if (e) config.log("ERROR", e);
				else {
					if (body !== undefined) {
						fs.unlinkSync(fileName);
						var info = file + " - uploaded";
						config.log(dealercode, info);
						//config.log(dealercode, "**************************************************************************");
					}
					console.log(body);
				}
                if (cb) cb(e);
            });								
            var form = req.form();
            form.append("DealerCode", dealercode);
            form.append("UploadCode", config.api().uplcd);
            form.append('file', fs.createReadStream(fileName));
        }
		
	},
	
	
    backupTableReadAll: function(db, task, callback) {
    var name = task.name;
    var sqlquery = "select count(*) as count from " + name + " where 1 = 1";
    var queryData = "select " + task.columns + " from " + name + " where 1 = 1";

    if (task.filterBy !== undefined && task.filterValue > "2000-01-01") {
        sqlquery = sqlquery + " and " + task.filterBy + " >= '" + task.filterValue + "'" ;
		queryData = queryData +  " and " + task.filterBy + " >= '" + task.filterValue + "'" ;
    }
		db.query('INSERT INTO BACKUP_INFO VALUES (?,?,?,?)', [ task.DealerCode, name, task.finish - task.start + 1, moment().format("YYYY-MM-DD HH:mm:ss") ] );        
		data = null;		
		if (task.start !== undefined)
		{
			var queryDataSrc = queryData.substr(6);
			var sql1 = "select " + task.columns + " from (select row_number() over(order by " + task.key + ") as row, " + queryDataSrc + ") a where row between " +  task.start + " and " + task.finish;
			queryData = sql1;
			//console.log(queryData);
		}		
		sqlODBC.queryRaw(task.conn, queryData, function (err, data) {	
			if (err) {
				config.log(task.DealerCode, "Error on " + name + " : " + err);
				callback();
			} else {			
				if (data !== undefined && data.rows !== undefined)
				{			
					async.eachSeries(data.rows, WriteData, function (err) {
						if (err) config.log(task.DealerCode, "Error on processing data " + name + " : " + err);
						db.query('UPDATE BACKUP_INFO set lastupdate=(select max(' + task.filterBy + ') from ' + name + ')');
						callback();
					});				
					function WriteData(row, callback) {
						var dataX = JSON.parse(JSON.stringify(row));
						db.query(task.DML, dataX, function(err, rows) {
							if (err) config.log(task.DealerCode, "Error on processing raw data " + name + " : " + err);
							callback();
						});				
					}				
				} else {					
					callback();
				}
			}
		});	
	},
	startAllJob: function(callback)
	{
		var self = this;
		var taskJobs = [];
		
		function listWorker(cfg) 
		{
			taskJobs.push(function(callback){self.startJob(cfg, callback);});
		}
			
		config.conn().forEach(listWorker);
		
		async.series(taskJobs, function (err, docs) {
			if (err) config.log("ERROR", err);			
			if (callback) callback();
		});
	},	
	
    startJob: function (cfg, callback) {
        var self = this;	
        var params = {
            uri: config.api().TaskCheck,
            method: "POST",
            form: {                    
                DealerCode: cfg.DealerCode,
				DealerType: cfg.DealerType
            }
        };	
        request(params, function (e, r, bodyX) {
			if (e) config.log("ERROR", e);
			else {
				var dataX = bodyX.substr(1, bodyX.length - 2);
                if ( dataX.length > 20) {
				var response = JSON.parse(dataX);             
				self.RunToDo(cfg, response, function(Err) { callback(); });
                } else {
                    console.log("No Task available for " + cfg.DealerCode);
                    if(callback) callback();
                }
			}			
        });				
    },
	
	RunToDo: function(cfg1, Tables, callback)
	{
        var self = this;		
		var tasks = [];		
		async.eachSeries(Tables, getTodo, function (err) {
			if (err) config.log("ERROR", err);			
			if(callback) callback();
		});
		function getTodo(task, callback) {

			var dir = path.join(__dirname, "../tasks");					
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir);
            }
			var filePath = path.join(dir,task.FileName);
			
			if (!fs.existsSync(filePath)) {
                console.log('file not found');
				download ( config.api().downloadLink + task.FileName, filePath,  function(err){
                    ExecuteScript(cfg1.DealerCode, cfg1.DealerType, filePath, task.TaskNo, function(e){
                        if (e) console.log(e);
                        if(callback) callback();
                    });				
				});
            } else {
                ExecuteScript(cfg1.DealerCode, cfg1.DealerType, filePath, task.TaskNo, function(e){
                    if (e) console.log(e);
                    if(callback) callback();
                });	
			}			
		}         
	}	
};
var ExecuteScript = function(DealerCode, DealerType, filePath, TaskNo, callback)
{
    var StartTime = moment().format("YYYY-MM-DD HH:mm:ss");
	console.log(StartTime,' : ', TaskNo, ' > ', filePath);
    run_shell ("node",["--expose-gc",filePath,"-d", DealerCode],function(err, code, result){
        var output = result;						
        var FinishTime = moment().format("YYYY-MM-DD HH:mm:ss");                        
        var errDesc = err || '';
        var Logger = {
            DealerCode: DealerCode,
            DealerType: DealerType,
            TaskNo: TaskNo,
            Result: output,
            StartTime: StartTime,
            EndTime: FinishTime,
            Code: code,
            ErrorDesc : errDesc
        };		
		console.log(FinishTime,' : ',Logger);               
        request.post(config.api().TaskLogger, {form:Logger}, function (e, r, body){
            if (e) console.log(e);
			console.log(body);
            if(callback) callback();
        });        
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