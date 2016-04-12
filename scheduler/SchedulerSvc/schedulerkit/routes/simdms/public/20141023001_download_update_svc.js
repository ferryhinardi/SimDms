var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var config = require('../config');
var Client = require('q-svn-spawn');
var http = require('http');

var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "Download Update-20141023.zip";
var TaskNo = "20141023001";

var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQL = hereDoc(function(){/*!

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

	var fileNameUp = "20141023.zip";
	
    log("Starting " + TaskName + " for " + CurrentDealerCode);
	
	var dir = path.join(__dirname, "../downloads");					
	if (!fs.existsSync(dir)) {
		fs.mkdirSync(dir);
	}
	var filePath = path.join(dir,fileNameUp);
			
		
	if (!fs.existsSync(filePath))
	{	
		download(config.api().downloadLink + fileNameUp,
				 filePath, function(err){
					if (err) log(err);
					
					if (!fs.existsSync(filePath))
						log( TaskName + ' failed');
					else
						log( TaskName + ' successfully');
						
					if (callback) callback();
				 });
	} else {
		log('You have ' + TaskName);
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

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

startTasks();