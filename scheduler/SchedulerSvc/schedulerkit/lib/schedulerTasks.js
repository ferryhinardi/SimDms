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

module.exports = {
		
	service: function()
	{
		var self = this;	
		config.log("SERVICE", "Scheduler Task is starting... ");
		
		setTimeout(function(){
			self.startAllJobs();
		},2000);
		
		setInterval(function () {
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
			if (err) config.log("ERROR", err);			
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
	}
};

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