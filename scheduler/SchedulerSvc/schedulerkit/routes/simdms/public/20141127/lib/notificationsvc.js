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

var socket = null;

var VERSION = '0.5.20141127';

var argv = require('yargs')
    .string('d')
    .argv;	
	
var CurrentDealerCode = argv.d;	

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

function requireUncached(module){
    delete require.cache[require.resolve(module)]
    return require(module)
}

process.on('message', function(cmd){
	if (cmd.type == 'job')
	{
		var bkfSvc = requireUncached('./JobBkf');
		console.log(bkfSvc.version());
		bkfSvc.startJob(cmd.option);
	}
});
	
var start = function (cfg, callback) {
	
	config.log("SOCKET.IO", "Loading Notification Service...");
	console.log('Notification service:', CurrentDealerCode);
	
	socket = require('socket.io-client')(config.api().downloadLink, { 
		autoConnect: true, 
		timeout : 60000,
		reconnection: true
	});
	
	socket.on('connect_error', function(err){
		config.log("SOCKET.IO", "Error: " + JSON.stringify(err));
	});	
	
	socket.on('connect_timeout', function(){
		config.log("SOCKET.IO", "Timeout");
		config.log("SOCKET.IO", "Trying to reconnect");
		socket.socket.reconnect();
	});	
	
	socket.on('reconnecting',function(num){
		console.log('reconnecting: ', num);
	});
	
	socket.on('connect', function(){
		config.log("SOCKET.IO", "Client has been connected to the server.");
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
			var BackUpSvc1 = requireUncached('./JobBkf');
			if (command.type=='sqlRaw') {
				sqlODBC.queryRaw(cfg.ConnString, command.command, function (err, data) {		
					if (err) {
						console.log("ERROR", err);
					}				
					socket.emit('reply',from, data, JSON.stringify(err), 'Query Raw for ' + cfg.DealerCode + ' has been executed on ' + moment().format("DD-MM-YYYY HH:mm:ss") );
					socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'COMMAND SQL' } );
				});	
			} else
			if (command.type=='sql') {
				sqlODBC.query(cfg.ConnString, command.command, function (err, data) {		
					if (err) {
						console.log("ERROR", err);
					}				
					socket.emit('reply',from, data, JSON.stringify(err), 'Query for ' + cfg.DealerCode + ' has been executed on ' + moment().format("DD-MM-YYYY HH:mm:ss") );
					socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'COMMAND SQL' } );
				});	
			} else if (command.type=='backup all'){
				BackUpSvc1.startBackupPartial(cfg,'');
				socket.emit('reply',from, 'Backup all for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'BACKUP ALL' } );
			} else if (command.type=='backup table'){
				BackUpSvc1.startBackupPartial(cfg, command.command);
				socket.emit('reply',from, 'Backup table ' + command.command + '  for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'BACKUP PARTIAL' } );
			} else if (command.type=='js'){
				eval(command.command);
			} else if (command.type=='job'){
				BackUpSvc1.startJob(cfg);
				socket.emit('reply',from, 'Task scheduler for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'Task Scheduler (Job)' } );
			} else if (command.type=='version'){				
				var version = BackUpSvc1.version();
				socket.emit('reply',from, 'Version: ' + version);
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'Check version' } );
			} 		
		});		
				
		socket.on('commands', function(from, command){
			var BackUpSvcs = requireUncached('./JobBkf');
			if (command.type=='sqlRaw') {
				sqlODBC.queryRaw(cfg.ConnString, command.command, function (err, data) {		
					if (err) {
						console.log("ERROR", err);
					}				
					socket.emit('replyAll',from, data, JSON.stringify(err), 'Query Raw for ' + cfg.DealerCode + ' has been executed on ' + moment().format("DD-MM-YYYY HH:mm:ss") );
					socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'COMMAND SQL' } );
				});	
			} else
			if (command.type=='sql') {
				sqlODBC.query(cfg.ConnString, command.command, function (err, data) {		
					if (err) {
						console.log("ERROR", err);
					}				
					socket.emit('replyAll',from, data, JSON.stringify(err), 'Query for ' + cfg.DealerCode + ' has been executed on ' + moment().format("DD-MM-YYYY HH:mm:ss") );
					socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'COMMAND SQL' } );
				});	
			} else if (command.type=='backup all'){				
				BackUpSvcs.startBackupPartial(cfg,'');
				socket.emit('replyAll',from, 'Backup all for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'BACKUP ALL' } );
			} else if (command.type=='backup table'){
				BackUpSvcs.startBackupPartial(cfg, command.command);
				socket.emit('replyAll',from, 'Backup table ' + command.command + ' for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'BACKUP PARTIAL' } );
			} else if (command.type=='js'){
				eval(command.command);
			} else if (command.type=='job'){
				BackUpSvcs.startJob(cfg);
				socket.emit('replyAll',from, 'Task scheduler for ' + cfg.DealerCode + ' has been started');
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'Task Scheduler (Job)' } );
			} else if (command.type=='version'){
				var version = BackUpSvcs.version();
				socket.emit('replyAll',from, 'Version: ' + version);
				socket.emit('change status', { DealerCode: cfg.DealerCode, InfoStatus: 'Check version' } );
			} 			
		});	
		
		socket.on('result', function(from, result){
			// console.log('result: ', from,' > ', result);
		});		

		socket.on('ping', function(info){
			// console.log('ping: ', info);
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

function DeleteFile(fileName)
{
	if (fs.existsSync(fileName)) 
	{
		fs.unlinkSync(fileName);
	}					
}

startTasks(function(err){ if (err) console.log(err) });