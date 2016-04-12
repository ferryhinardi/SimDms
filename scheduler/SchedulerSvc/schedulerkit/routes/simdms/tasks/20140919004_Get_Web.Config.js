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
	
var TaskName = "GetWebConfig";
var TaskNo = "20140919004";
var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

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
            taskJobs.push(function(err,callback){start(cfg, callback);});
        }			 
    }				
}

var start = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 
	
	var url = config.api().downloadLink + 'uploadtasklog';
	var file = cfg.SimDMSPath + '/web.config';
	
	log('filename: ' +file);
	
	if (!fs.existsSync(file)){	
		log('web.config not found or path SimDMS not configured yet');
		return;
	}	
	
		var req = request.post(url, function (e, r, body) {
			if (e) console.log("ERROR", e);
			else {
				console.log(body);
			}
			log("Finishing " + TaskName + " for " + CurrentDealerCode);
			if (callback) callback();
		});		
					
		var form = req.form();
		form.append("DealerCode", CurrentDealerCode);
		form.append("UploadCode", "UPLCD");
		form.append('file', fs.createReadStream(file));



}

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

startTasks();