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
var LastBackup = {};

module.exports = {
		
	serviceio: function()
	{
		config.conn().forEach(notificationSvc);
		
		function notificationSvc(cfg)
		{
			MyScokets[cfg.DealerCode] = (require('child_process').fork('./lib/notificationsvc',['-d',cfg.DealerCode]));			
			MyScokets[cfg.DealerCode].on('exit', function(err){
				console.log('Socket client down .... restart it again');
				notificationSvc(cfg);				
			});
		}		
		
		var self = this;	
		Logger("SERVICE", "Starting Application");
		
		setTimeout(function(){
			self.startAllJobs();
		},2000);
		
		setTimeout(function(){
			self.startAllPartial();
		},5000);
		
		setInterval(function () {
			self.startAllPartial(function(Err)
			{
				console.log("Backup All Finished");
			});
		}, 1000 *  60 * 60 * 2);
		
		setInterval(function () 
		{
			self.startAllJobs(function(Err)
			{
				if (Err) console.log(Err);
			});
		}, 1000 *  60 * 30);		
		
	},	
	
	startAllJobs: function(callback)
	{
		var self = this;
		var taskJobs = [];
		
		function listWorker(cfg) 
		{
			taskJobs.push(function(callback){self.startJob(cfg, callback);});
		}
			
		config.conn().forEach(listWorker);
		
		async.series(taskJobs, function (err, docs) {
			if (err) Logger("ERROR", err);			
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
			if (e) Logger("ERROR", e);
			else {
				var dataX = bodyX.substr(1, bodyX.length - 2);
                if ( dataX.length > 20) {
				var response = JSON.parse(dataX);             
				self.RunToDo(cfg, response, function(Err) { callback(); });
                } else {
                    console.log("No Task available for " + cfg.DealerCode);
                    callback();
                }
			}			
        });
    },
	
	RunToDo: function(cfg1, Tables, callback)
	{
        var self = this;
		
		var tasks = [];
		
		async.each(Tables, getTodo, function (err) {
			if (err) Logger("ERROR", err);			
			callback();
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
                        callback();
                    });				
				});
            } else {
                ExecuteScript(cfg1.DealerCode, cfg1.DealerType, filePath, task.TaskNo, function(e){
                    if (e) console.log(e);
                    callback();
                });	
			}			
		}         
	},
	
	service: function()
	{
		var self = this;	
		Logger("SERVICE", "Starting Application");
		
		setTimeout(function(){
			self.startAllPartial();
		},10000);
		
		setInterval(function () {
			self.startAllPartial(function(Err)
			{
				console.log("Backup All Finished");
			});
		}, 1000 *  60 * 60 * 3);



		
	},	
	
	startAll: function(callback)
	{
		var self = this;
		var tasksBackup = [];
		
		function listWorker(cfg) 
		{
			tasksBackup.push(function(callback){self.start(cfg, callback);});
		}
		
		function cleanUp(cfg) 
		{
			sqlODBC.queryRaw(cfg.ConnString, "select 1 a", function (err, data) {
				if (err) {
					Logger("ERROR", err);
				}
				data = null;
				global.gc();
				Logger(cfg.DealerCode, "Memory Usage: " + util.inspect(process.memoryUsage()));
			});
		}
		
		config.conn().forEach(listWorker);
		
		async.series(tasksBackup, function (err, docs) {
			if (err) Logger("ERROR", err);			
			config.conn().forEach(cleanUp);			
			if (callback) callback();
		});
	},	
	
	startAllPartial: function(callback)
	{
		var self = this;
		var tasksBackup = [];
		
		function listWorker(cfg) 
		{
			tasksBackup.push(function(callback){self.startBackupPartial(cfg, callback);});
		}
		
		function cleanUp(cfg) 
		{
			sqlODBC.queryRaw(cfg.ConnString, "select 1 a", function (err, data) {
				if (err) {
					Logger("ERROR", err);
				}
				data = null;
				global.gc();
				Logger(cfg.DealerCode, "Memory Usage: " + util.inspect(process.memoryUsage()));
			});
		}
		
		config.conn().forEach(listWorker);
		
		async.series(tasksBackup, function (err, docs) {
			if (err) Logger("ERROR", err);			
			config.conn().forEach(cleanUp);			
			if (callback) callback();
		});
	},	
	
    start: function (cfg, callback) {
        var self = this;
        var params = {
            uri: config.api().listTables,
            method: "POST",
            form: {                    
                DealerCode: cfg.DealerCode,
				DealerType: cfg.DealerType
            }
        };	
        request(params, function (e, r, bodyX) {
			if (e) Logger("ERROR", e);
			else {
				var dataX = bodyX.substr(1, bodyX.length - 2);
				var response = JSON.parse(dataX);             
				self.startBackup(cfg, response, function(Err) { callback(); });
			}			
        });
    },
	
	startBackupPartial: function (cfg, callback) {
        var self = this;
        var params = {
            uri: config.api().listTables,
            method: "POST",
            form: {                    
                DealerCode: cfg.DealerCode,
				DealerType: cfg.DealerType
            }
        };	
        request(params, function (e, r, bodyX) {
			if (e) Logger("ERROR", e);
			else {
				var dataX = bodyX.substr(1, bodyX.length - 2);
				var response = JSON.parse(dataX);  

				async.each(response, function(data, callback){
					Logger(cfg.DealerCode, "starting backup table " + data.TableName );
					self.BackupPartial(cfg, data, function(err){
						if (err) console.log(err);
						Logger(cfg.DealerCode, "Backup table " + data.TableName + " finished");
						if (callback) callback();
					});					
				}
				, function (err) {
					if (err) console.log(err);
					console.log('DONE');
					if (callback) callback();
				});

			}			
        });
    },
	
	startBackup: function(cfg1, Tables, callback)
	{
        var self = this;
        var IndexNo = 1;
		var TotalAvailableRecords = 0;
		
        Logger(cfg1.DealerCode, "**************************************************************************");  
        Logger(cfg1.DealerCode, "Starting backup database - " + cfg1.DealerCode);  
		
        var filedb = cfg1.DealerCode + "_" + cfg1.DealerType + moment().format("_YYYYMMDD"); 
		var srcFile = "log/" + filedb + ".db3";
        var db = config.sqlite3(srcFile);

        db.query('BEGIN');
		
		db.query('CREATE TABLE IF NOT EXISTS BACKUP_INFO (code TEXT, name TEXT, count int, lastupdate datetime)');

        Logger(cfg1.DealerCode, "Retrieve table list from server");
		
		Logger(cfg1.DealerCode, "Generate contents for backup database");
		Logger(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
		
		var tasks = [];
		
		async.each(Tables, gettable, function (err) {

			if (err) Logger("ERROR", err);
			
				db.query('COMMIT', function(err, rows) {
					db.query('BEGIN');
					async.series(tasks, function (err, docs) {
						if (err) {
							Logger(cfg1.DealerCode, "Error >> " + err);
						}
						tasks = null;
						Logger(cfg1.DealerCode, "Saving backup database (memory to file db)");
						db.query('COMMIT', function(err, rows) {
							db.query('select sum(count) from backup_info', function(err, rows) {
								TotalAvailableRecords = rows[0];
								console.log("AVailable records: " + TotalAvailableRecords);
								db.close();
							});							
						});                   
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
							Logger(cfg1.DealerCode, "Error on " + table.TableName + " >> " + err);
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
							filterValue: table.FilterValue
						} 
						backupTableReadAll(db, task, function (result) {
							task = null;
							callback(null, result);
						});
					});   }                     
				});   
			}
		} 

        db.on('close', function (code) {
		
			Logger(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
			db = null;
						
			if (TotalAvailableRecords > 0){
			Logger(cfg1.DealerCode, "start compressing database file (zip file)");	
						
			var filezip = "log/temp/" + filedb + ".zip";
			
			var output = fs.createWriteStream(filezip);			
			var archive = archiver('zip',{ zlib:{level:9} });
			
			output.on('close', function() {				
				Logger(cfg1.DealerCode, "Backup database finished, save to " + filezip);
				DeleteFile(srcFile);
					
				if (fs.existsSync(filezip)) {
					sendFile(filedb, filezip, function(err){
						callback();
					});
				}
				srcFile = null;
				filezip = null;
				archive = null;
			});
			
			archive.on('error', function(err) {
				if (err) Logger("ERROR", err);
			});
			
			archive.pipe(output);
			archive.append(fs.createReadStream(srcFile), { name: filedb + ".db3" })
			archive.finalize();	
			} else {
				DeleteFile(srcFile);
				callback();
			}			
			
        });	
		
		function sendFile(file, fileName, cb) {

			var dealercode = file.split("_")[0];
			var url = config.api().upload;
			
            var req = request.post(url, function (e, r, body) {
				if (e) Logger("ERROR", e);
				else {
					if (body !== undefined) {
						
						DeleteFile(fileName);
						var info = file + " - uploaded";
						Logger(dealercode, info);
						Logger(dealercode, "**************************************************************************");
					}
					console.log(body);
				}
                if (cb) cb();
            });		
						
            var form = req.form();
            form.append("DealerCode", dealercode);
            form.append("UploadCode", config.api().uplcd);
            form.append('file', fs.createReadStream(fileName));
        }
		
	},

	BackupPartial: function(cfg1, table, callback)
	{
        var self = this;
        var IndexNo = 1;
		var TotalAvailableRecords = 0;
		
        Logger(cfg1.DealerCode, "**************************************************************************");  
        Logger(cfg1.DealerCode, "Starting backup database - " + cfg1.DealerCode + "." + table.TableName );  
		
        var filedb = cfg1.DealerCode + "_" + cfg1.DealerType + moment().format("_YYYYMMDD") + "_" + table.TableName; 
		var srcFile = "log/" + filedb + ".db3";
        var db = config.sqlite3(srcFile);

        db.query('BEGIN');
		
		db.query('CREATE TABLE BACKUP_INFO (code TEXT, name TEXT, count int, lastupdate datetime)');

        Logger(cfg1.DealerCode, "Retrieve table list from server");
		
		Logger(cfg1.DealerCode, "Generate contents for backup database");
		Logger(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
		
		var tasks = [];
		
		gettable(table, function(err){		
			if (err) Logger("ERROR", err);		
			db.query('COMMIT', function(err, rows) {
				db.query('BEGIN');
				async.series(tasks, function (err, docs) {
					if (err) {
						Logger(cfg1.DealerCode, "Error >> " + err);
					}
					tasks = null;
					Logger(cfg1.DealerCode, "Saving backup database (memory to file db)");
					db.query('COMMIT', function(err, rows) {
						db.query('select sum(count) from backup_info', function(err, rows) {
							TotalAvailableRecords = rows[0];
							console.log("Available records: " + TotalAvailableRecords);
							db.close();
						});							
					});                   
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
							Logger(cfg1.DealerCode, "Error on " + table.TableName + " >> " + err);
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
							filterValue: table.FilterValue
						} 
						backupTableReadAll(db, task, function (result) {
							task = null;
							callback(null, result);
						});
					});   
					}                     
				});   
			}
		} 

        db.on('close', function (code) {
		
			Logger(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
			db = null;
						
			if (TotalAvailableRecords > 0){
			Logger(cfg1.DealerCode, "start compressing database file (zip file)");	
						
			var filezip = "log/temp/" + filedb + ".zip";
			
			var output = fs.createWriteStream(filezip);			
			var archive = archiver('zip',{ zlib:{level:9} });
			
			output.on('close', function() {				
				Logger(cfg1.DealerCode, "Backup database finished, save to " + filezip);
				DeleteFile(srcFile);
				sendFile(filedb, filezip, function(err){
					callback();
				});
				srcFile = null;
				filezip = null;
				archive = null;
			});
			
			archive.on('error', function(err) {
				if (err) Logger("ERROR", err);
			});
			
			archive.pipe(output);
			archive.append(fs.createReadStream(srcFile), { name: filedb + ".db3" })
			archive.finalize();	
			} else {
				DeleteFile(srcFile);
				callback();
			}			
			
        });	
		
		function sendFile(file, fileName, cb) {

			var dealercode = file.split("_")[0];
			var url = config.api().upload;
			
            var req = request.post(url, function (e, r, body) {
				if (e) Logger("ERROR", e);
				else {
					if (body !== undefined) {
						DeleteFile(fileName);
						var info = file + " - uploaded";
						Logger(dealercode, info);
						Logger(dealercode, "**************************************************************************");
					}
					console.log(body);
				}
                if (cb) cb();
            });		
						
            var form = req.form();
            form.append("DealerCode", dealercode);
            form.append("UploadCode", config.api().uplcd);
            form.append('file', fs.createReadStream(fileName));
        }
		
	},

    upload: function () {
	
		var backup_path = "../log/temp";
		
        fs.readdir(path.join(__dirname, backup_path), function (err, files) {
            async.each(files, sendFile, function (err) {
                if (err) Logger("ERROR", err);
            })
        });

        function sendFile(file, cb) {

            var fileName = path.join(__dirname, backup_path, file);
			var dealercode = file.split("_")[0];
			var url = config.api().upload;
			
            var req = request.post(url, function (e, r, body) {
				if (e) Logger("ERROR", e);
				else {
					if (body !== undefined) {
						DeleteFile(fileName);
						var info = file + " - uploaded";
						Logger(dealercode, info);
						Logger(dealercode, "**************************************************************************");
					}
					console.log(body);
				}
                if (cb) cb();
            });		
						
            var form = req.form();
            form.append("DealerCode", dealercode);
            form.append("UploadCode", config.api().uplcd);
            form.append('file', fs.createReadStream(fileName));
        }
    }
};

function backupTableReadAll(db, task, callback) {
    var name = task.name;
    var sqlquery = "select count(*) as count from " + name + " where 1 = 1";
    var queryData = "select " + task.columns + " from " + name + " where 1 = 1";

    if (task.filterBy !== undefined && task.filterValue > "2000-01-01") {
        sqlquery = sqlquery + " and " + task.filterBy + " >= '" + task.filterValue + "'" ;
		queryData = queryData +  " and " + task.filterBy + " >= '" + task.filterValue + "'" ;
    }

    sqlODBC.query(task.conn, sqlquery, function (err, data) {
        if (err) {
			Logger(task.DealerCode, "Error on " + name + " : " + err );
			callback();
		} else {

        var count = data[0].count;
        Logger(task.DealerCode, "Result: " + task.No + ", " + name + " : " + count + ", " + task.filterValue);		
		db.query('INSERT INTO BACKUP_INFO VALUES (?,?,?,?)', [ task.DealerCode, name, count, moment().format("YYYY-MM-DD HH:mm:ss") ] );        
		data = null;
		
		sqlODBC.queryRaw(task.conn, queryData, function (err, data) {	
			if (err) {
				Logger(task.DealerCode, "Error on " + name + " : " + err);
				callback();
			} else {			
				if (data !== undefined && data.rows !== undefined)
				{			
					async.each(data.rows, WriteData, function (err) {
						if (err) Logger(task.DealerCode, "Error on processing data " + name + " : " + err);
						callback();
					});				
					function WriteData(row, callback) {
						var dataX = JSON.parse(JSON.stringify(row));
						db.query(task.DML, dataX, function(err, rows) {
							if (err) Logger(task.DealerCode, "Error on processing raw data " + name + " : " + err);
							callback();
						});				
					}				
				} else {					
					callback();
				}
			}
		});	
		}
    })
}

var ExecuteScript = function(DealerCode, DealerType, filePath, TaskNo, callback)
{
    var StartTime = moment().format("YYYY-MM-DD HH:mm:ss");
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
               
        request.post(config.api().TaskLogger, {form:Logger}, function (e, r, body){
            if (e) console.log(e);
            callback();
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
    DeleteFile(dest); // Delete the file async. (But we don't check the result)
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

function Logger (name, info) {
	var dir = path.join(__dirname, '../log');
	if (!fs.existsSync(dir)) {
		fs.mkdirSync(dir);
	}

	dir = path.join(dir, moment().format("YYYY"));
	if (!fs.existsSync(dir)) {
		fs.mkdirSync(dir);
	}

	dir = path.join(dir, moment().format("MM"));
	if (!fs.existsSync(dir)) {
		fs.mkdirSync(dir);
	}

	var file = path.join(dir, name + "-" + moment().format("YYYYMMDD") + ".log");
	fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
	console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info);
	if (MyScokets[name])
	{
		MyScokets[name].send({ dealer: name, info: info });
	}
}

function DeleteFile(fileName)
{
	if (fs.existsSync(fileName)) 
	{
		fs.unlinkSync(fileName);
	}					
}