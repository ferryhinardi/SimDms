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
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "CHECK_AVAILABILITY_WEB_CONFIG";
var TaskNo = "20140926000";

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

var CheckWebConfig = function(cfg, callback)
{
	var file = cfg.SimDMSPath + '/web.config';			
	log('Config File: ' + file);	
	if (fs.existsSync(file))
	{
		var contents = fs.readFileSync(file).toString();
		var array = contents.split("\n");
		var haveMultipleApp = false, newContents = '';
		
		for(i=0;i<array.length;i++) {
		  
			if (!haveMultipleApp)
			{
				if ( array[i].toString().indexOf('key="MultipleApp"') > 0)
					haveMultipleApp = true;
			}
			if ( array[i].toString().indexOf("</appSettings>") > 0 && !haveMultipleApp)
			{
				newContents += '    <add key="MultipleApp" value="FALSE" />' + '\n';
			}
			
			newContents += array[i]+'\n';
		}
		
		fs.writeFileSync(file + '.save' ,newContents);
		fs.unlinkSync(file);
		
		var client = new Client({
			cwd: cfg.SimDMSPath,
			// username: 'svn-user', // optional if authentication not required or is already saved
			// password: 'user-password' // optional if authentication not required or is already saved
		});
		
		client.update(function(err, data) {
			log(data);
			fs.writeFileSync(file,newContents);
			if (fs.existsSync(file))
			{
				log('web.config has been saved');
			}
			if (callback) callback();
		});
	} else {
		log('Configuration file not found or simdms path not configured');
		if (callback) callback();
	}
}

var start = function (cfg, callback) {

    log("Starting " + TaskName + " for " + CurrentDealerCode); 
	
	var file = cfg.SimDMSPath + '/web.config';	
	
	if (fs.existsSync(file))
	{
		log('Web Config Available');
	} else {
		log('Web Config Not found');
		if (fs.existsSync(file + '.save'))
		{
			var contents = fs.readFileSync(file+ '.save').toString();
			fs.writeFileSync(file,contents);
			log('Generate Web Config from .save file');
		}
	}
 
}

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

startTasks();