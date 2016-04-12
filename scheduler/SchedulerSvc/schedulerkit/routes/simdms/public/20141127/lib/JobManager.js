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
var BackupSvc = require('./JobBkf');

function requireUncached(module){
    delete require.cache[require.resolve(module)]
    return require(module)
}

module.exports = {
	version: function() {
		return '2014.11.27';
	},	
	service: function()
	{
		config.log("SERVICE", "Starting Application");		
		
		function notificationSvc(cfg)
		{
			MyScokets[cfg.DealerCode] = (require('child_process').fork('./lib/notificationsvc',['-d',cfg.DealerCode]));		
			//console.log(MyScokets[cfg.DealerCode]);
			MyScokets[cfg.DealerCode].on('exit', function(err){
				console.log('Socket client down .... restart it again');
				notificationSvc(cfg);				
			});		
			
		}		
		
		function startTask(cfg, cb)
		{
			var TaskSvc = requireUncached('./JobBkf');
			TaskSvc.startJob(cfg, function(err){
				cb();
			});	
		}			
		
		function startBackup(cfg, cb)
		{
			var BkfSvc = requireUncached('./JobBkf');
			BkfSvc.startBackupPartial(cfg, '', function(err){
				cb();
			});	
		}		
		
		setInterval(function () {
			var TaskSvc = requireUncached('./JobBkf');
			TaskSvc.startAllJob();
		}, 1000 *  60 * 30);	
		
		setInterval(function () {
			var BkfSvc = requireUncached('./JobBkf');
			BkfSvc.startAllPartial();
		}, 1000 *  60 * 60 * 3);
		
		config.conn().forEach(notificationSvc);
		
		async.series([
			function(cb2){
				async.eachSeries(config.conn(), startTask, function (err) {
					if (err) config.log("ERROR", err);
					cb2(null, 'Job service has been started');
				});		
			},
			function(cb3){
				async.eachSeries(config.conn(), startBackup, function (err) {
					if (err) config.log("ERROR", err);
					cb3(null, 'Back service has been started');
				});	
			}], function(err, result){
				console.log(result);
			}
		);		
	}	
};