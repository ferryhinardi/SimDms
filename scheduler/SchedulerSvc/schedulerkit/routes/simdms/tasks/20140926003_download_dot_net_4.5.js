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
	
var TaskName = "Download_dot_NET_Framework_4.5";
var TaskNo = "20140926003";

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
	
	var dir = path.join(__dirname, "../downloads");					
	if (!fs.existsSync(dir)) {
		fs.mkdirSync(dir);
	}
	var filePath = path.join(dir,"dot_net_4.5_setup.exe");
			
	
	
	if (!fs.existsSync(filePath))
	{	
		download("http://wsugiri.cloudapp.net/downloads/dotnet%20framework/mu_.net_framework_4.5_r2_x86_x64_1076098.exe",
				 filePath, function(err){
					if (err) log(err);
					
					if (!fs.existsSync(filePath))
						log('Download dot NET 4.5 failed');
					else
						log('Download dot NET 4.5 successfully');
						
					if (callback) callback();
				 });
	} else {
		log('You have downloaded dot NET framework 4.5');
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