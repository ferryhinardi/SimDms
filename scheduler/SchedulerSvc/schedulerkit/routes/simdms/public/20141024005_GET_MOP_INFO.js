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
	
var TaskName = "Get Info MON MOP ASA";
var TaskNo = "20141024005";

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

function getFiles(dir,files_){
    files_ = files_ || [];
    if (typeof files_ === 'undefined') files_=[];
    var files = fs.readdirSync(dir);
    for(var i in files){
        if (!files.hasOwnProperty(i)) continue;
        var name = files[i];
        if (fs.statSync(name).isDirectory()){
            getFiles(name,files_);
        } else {
			log(name);
            files_.push(name);
        }
    }
    return files_;
};




var start = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 
	
	var url = config.api().downloadLink + 'uploadtasklog';

	var dir = path.join(__dirname, '../lib');
	
	log('dir: ' +dir);
	var listFiles = getFiles(dir);
	var indexFile = path.join(__dirname, '../routes/dealer/index.js');
	var contents = fs.readFileSync(indexFile).toString();
	log(contents);
	
		var req = request.post(url, function (e, r, body) {
			if (e) console.log("ERROR", e);
			else {
				console.log(body);
			}
			log("Finishing " + TaskName + " for " + CurrentDealerCode);
			if (callback) callback();
		});		
					
	var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
	
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