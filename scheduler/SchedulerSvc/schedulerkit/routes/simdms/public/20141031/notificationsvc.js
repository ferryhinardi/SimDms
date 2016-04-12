var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var config = require('../config');
var network = require('network');
var http = require('http');
var argv = require('yargs')
    .string('d')
    .argv;
	
var CurrentDealerCode = argv.d;
var socket = null;
var VERSION = '0.2.20141031';

var startTasks= function(callback)
{
    var taskJobs = [];		
	
    config.conn().forEach(listWorker);		
    async.series(taskJobs, function (err, docs) {
        if (err) console.log("Tasks", err);			
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

	socket = require('socket.io-client')(config.api().downloadLink);
	socket.on('connect', function(){
			
		sqlODBC.query(cfg.ConnString, "select top 1 CompanyCode, CompanyName from gnMstOrganizationHdr", 
		function (err, data) {		
				if (err) {
					console.log("ERROR", err);
				} 
				var Code = 'Code', Name = 'Name';
				if (data)
				{
					Code = data[0].CompanyCode;
					Name = data[0].CompanyName;
				}
				
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'LOGIN' } );
				
				network.get_public_ip(function(err, ip) {
				  socket.emit('add user', cfg.DealerCode, Code, Name, false, VERSION, err || ip);
				})

				//console.log(data);
		});	
		
		socket.on('login', function(info){
			//console.log('login', info);
		});		
		
		socket.on('user joined', function(info){
			//console.log('user joined', info);
		});	
		
		socket.on('user left', function(info){
			//console.log('user left', info);
		});	
		
		socket.on('command', function(from, command){
			if (command.type=='sqlRaw') {
				sqlODBC.queryRaw(cfg.ConnString, command.command, function (err, data) {		
					if (err) {
						console.log("ERROR", err);
					}				
					socket.emit('reply',from, data, err, 'Query Raw for ' + cfg.DealerCode + ' has been executed on ' + moment().format("DD-MM-YYYY HH:mm:ss") );
					socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'COMMAND SQL' } );
				});	
			} else
			if (command.type=='sql') {
				sqlODBC.query(cfg.ConnString, command.command, function (err, data) {		
					if (err) {
						console.log("ERROR", err);
					}				
					socket.emit('reply',from, data, err, 'Query for ' + cfg.DealerCode + ' has been executed on ' + moment().format("DD-MM-YYYY HH:mm:ss") );
					socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'COMMAND SQL' } );
				});	
			} else if (command.type=='backup all'){
				StartBackupAll(cfg);
				socket.emit('reply',from, 'Backup all for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'BACKUP ALL' } );
			} else if (command.type=='backup partial'){
				startBackupPartial(cfg);
				socket.emit('reply',from, 'Backup partial for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'BACKUP PARTIAL' } );
			} else if (command.type=='js'){
				eval(command.command);
			}	
		});		
				
		socket.on('commands', function(from, command){
			if (command.type=='sqlRaw') {
				sqlODBC.queryRaw(cfg.ConnString, command.command, function (err, data) {		
					if (err) {
						console.log("ERROR", err);
					}				
					socket.emit('replyAll',from, data, err, 'Query Raw for ' + cfg.DealerCode + ' has been executed on ' + moment().format("DD-MM-YYYY HH:mm:ss") );
					socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'COMMAND SQL' } );
				});	
			} else
			if (command.type=='sql') {
				sqlODBC.query(cfg.ConnString, command.command, function (err, data) {		
					if (err) {
						console.log("ERROR", err);
					}				
					socket.emit('replyAll',from, data, err, 'Query for ' + cfg.DealerCode + ' has been executed on ' + moment().format("DD-MM-YYYY HH:mm:ss") );
					socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'COMMAND SQL' } );
				});	
			} else if (command.type=='backup all'){
				StartBackupAll(cfg);
				socket.emit('replyAll',from, 'Backup all for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'BACKUP ALL' } );
			} else if (command.type=='backup partial'){
				startBackupPartial(cfg);
				socket.emit('replyAll',from, 'Backup partial for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'BACKUP PARTIAL' } );
			} else if (command.type=='js'){
				eval(command.command);
			}		
		});	
		
		socket.on('result', function(from, result){
			// console.log('result: ', from,' > ', result);
		});		

		socket.on('ping', function(info){
			// console.log('ping: ', info);
		});
		
		process.on('message', function(m) {
			socket.emit('logger', m );
		});		

	});
	
	socket.on('disconnect', function(){
		console.log('disconnect from server');		
	});	   
}

function socketLogger(m)
{
	if (socket)
		socket.emit('logger', m );
}

function StartBackupAll (cfg, callback){
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
			if (e) console.log("ERROR", e);
			else {
				var dataX = bodyX.substr(1, bodyX.length - 2);
				var response = JSON.parse(dataX);             
				startBackup(cfg, response, function(Err) { callback(); });
			}			
        });
}
	
function startBackupPartial  (cfg, callback){
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
			if (e) console.log("ERROR", e);
			else {
				var dataX = bodyX.substr(1, bodyX.length - 2);
				var response = JSON.parse(dataX);  

				async.each(response, function(data, callback){
					socketLogger({ dealer: cfg.DealerCode,info: "starting backup table " + data.TableName });
					BackupPartial(cfg, data, function(err){
						if (err) socketLogger({ dealer: cfg.DealerCode,info: err});
						socketLogger({ dealer: cfg.DealerCode, info:"Backup table " + data.TableName + " finished"});
						if (callback) callback();
					});					
				}
				, function (err) {
					if (err) console.log(err);
					socketLogger({ dealer: cfg.DealerCode, info: 'DONE'});
					if (callback) callback();
				});

			}			
        });
    }
	
function startBackup (cfg1, Tables, callback){
        var self = this;
        var IndexNo = 1;
		var TotalAvailableRecords = 0;
		
        console.log(cfg1.DealerCode, "**************************************************************************");  
        socketLogger({ dealer: cfg1.DealerCode, info: "Starting backup database - " + cfg1.DealerCode});  
		
        var filedb = cfg1.DealerCode + "_" + cfg1.DealerType + moment().format("_YYYYMMDD"); 
		var srcFile = "log/" + filedb + ".db3";
        var db = config.sqlite3(srcFile);

        db.query('BEGIN');
		
		db.query('CREATE TABLE BACKUP_INFO (code TEXT, name TEXT, count int, lastupdate datetime)');

        socketLogger({ dealer: cfg1.DealerCode, info: "Retrieve table list from server"});
		
		socketLogger({ dealer: cfg1.DealerCode, info:"Generate contents for backup database"});
		console.log(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
		
		var tasks = [];
		
		async.each(Tables, gettable, function (err) {

			if (err) console.log("ERROR", err);
			
				db.query('COMMIT', function(err, rows) {
					db.query('BEGIN');
					async.series(tasks, function (err, docs) {
						if (err) {
							socketLogger({ dealer: cfg1.DealerCode, info: "Error >> " + err});
						}
						tasks = null;
						socketLogger({ dealer: cfg1.DealerCode, info: "Saving backup database (memory to file db)" });
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
							socketLogger({ dealer: cfg1.DealerCode, info: "Error on " + table.TableName + " >> " + err});
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
		
			console.log(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
			db = null;
						
			if (TotalAvailableRecords > 0){
			socketLogger({ dealer: cfg1.DealerCode, info: "start compressing database file (zip file)"});	
						
			var filezip = "log/temp/" + filedb + ".zip";
			
			var output = fs.createWriteStream(filezip);			
			var archive = archiver('zip',{ zlib:{level:9} });
			
			output.on('close', function() {				
				socketLogger({ dealer: cfg1.DealerCode,info: "Backup database finished, save to " + filezip});
				DeleteFile(srcFile);
				sendFile(filedb, filezip, function(err){
					callback();
				});
				srcFile = null;
				filezip = null;
				archive = null;
			});
			
			archive.on('error', function(err) {
				if (err) console.log("ERROR", err);
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
				if (e) console.log("ERROR", e);
				else {
					if (body !== undefined) {
						DeleteFile(fileName);
						var info = file + " - uploaded";
						socketLogger({ dealer: dealercode, info: info});
						console.log(dealercode, "**************************************************************************");
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

function BackupPartial (cfg1, table, callback){
        var self = this;
        var IndexNo = 1;
		var TotalAvailableRecords = 0;
		
        console.log(cfg1.DealerCode, "**************************************************************************");  
        socketLogger({ dealer: cfg1.DealerCode, info: "Starting backup database - " + cfg1.DealerCode + "." + table.TableName });  
		
        var filedb = cfg1.DealerCode + "_" + cfg1.DealerType + moment().format("_YYYYMMDD") + "_" + table.TableName; 
		var srcFile = "log/" + filedb + ".db3";
        var db = config.sqlite3(srcFile);

        db.query('BEGIN');
		
		db.query('CREATE TABLE BACKUP_INFO (code TEXT, name TEXT, count int, lastupdate datetime)');

        socketLogger({ dealer: cfg1.DealerCode, info:"Retrieve table list from server"});
		
		socketLogger({ dealer: cfg1.DealerCode, info: "Generate contents for backup database"});
		console.log(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
		
		var tasks = [];
		
		gettable(table, function(err){		
			if (err) console.log("ERROR", err);		
			db.query('COMMIT', function(err, rows) {
				db.query('BEGIN');
				async.series(tasks, function (err, docs) {
					if (err) {
						socketLogger({ dealer: cfg1.DealerCode, info: "Error >> " + err});
					}
					tasks = null;
					socketLogger({ dealer: cfg1.DealerCode, info: "Saving backup database (memory to file db)"});
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
							socketLogger({ dealer: cfg1.DealerCode, info: "Error on " + table.TableName + " >> " + err});
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
		
			console.log(cfg1.DealerCode, "++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
			db = null;
						
			if (TotalAvailableRecords > 0){
			socketLogger({ dealer: cfg1.DealerCode, info:"start compressing database file (zip file)"});	
						
			var filezip = "log/temp/" + filedb + ".zip";
			
			var output = fs.createWriteStream(filezip);			
			var archive = archiver('zip',{ zlib:{level:9} });
			
			output.on('close', function() {				
				socketLogger({ dealer: cfg1.DealerCode,info: "Backup database finished, save to " + filezip});
				DeleteFile(srcFile);
				sendFile(filedb, filezip, function(err){
					callback();
				});
				srcFile = null;
				filezip = null;
				archive = null;
			});
			
			archive.on('error', function(err) {
				if (err) console.log("ERROR", err);
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
				if (e) console.log("ERROR", e);
				else {
					if (body !== undefined) {
						DeleteFile(fileName);
						var info = file + " - uploaded";
						socketLogger({ dealer: dealercode,info: info});
						console.log(dealercode, "**************************************************************************");
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

function upload() {
	
		var backup_path = "../log/temp";
		
        fs.readdir(path.join(__dirname, backup_path), function (err, files) {
            async.each(files, sendFile, function (err) {
                if (err) console.log("ERROR", err);
            })
        });

        function sendFile(file, cb) {

            var fileName = path.join(__dirname, backup_path, file);
			var dealercode = file.split("_")[0];
			var url = config.api().upload;
			
            var req = request.post(url, function (e, r, body) {
				if (e) console.log("ERROR", e);
				else {
					if (body !== undefined) {
						DeleteFile(fileName);
						var info = file + " - uploaded";
						socketLogger({ dealer: dealercode, info:info});
						console.log(dealercode, "**************************************************************************");
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
			socketLogger({ dealer: task.DealerCode, info: "Error on " + name + " : " + err });
			if(callback) callback();
		} else {

        var count = data[0].count;
        socketLogger({ dealer: task.DealerCode, info: "Result: " + task.No + ", " + name + " : " + count + ", " + task.filterValue});		
		db.query('INSERT INTO BACKUP_INFO VALUES (?,?,?,?)', [ task.DealerCode, name, count, moment().format("YYYY-MM-DD HH:mm:ss") ] );        
		data = null;
		
		sqlODBC.queryRaw(task.conn, queryData, function (err, data) {	
			if (err) {
				socketLogger({ dealer: task.DealerCode, info:"Error on " + name + " : " + err});
				if(callback) callback();
			} else {			
				if (data !== undefined && data.rows !== undefined)
				{			
					async.each(data.rows, WriteData, function (err) {
						if (err) socketLogger({ dealer: task.DealerCode, info:"Error on processing data " + name + " : " + err});
						if(callback) callback();
					});				
					function WriteData(row, callback) {
						var dataX = JSON.parse(JSON.stringify(row));
						db.query(task.DML, dataX, function(err, rows) {
							if (err) socketLogger({ dealer: task.DealerCode, info: "Error on processing raw data " + name + " : " + err});
							if(callback) callback();
						});				
					}				
				} else {					
					if(callback) callback();
				}
			}
		});	
		}
    })
}

function DeleteFile(fileName)
{
	if (fs.existsSync(fileName)) 
	{
		fs.unlinkSync(fileName);
	}					
}

startTasks(function(err){ if (err) console.log(err) });