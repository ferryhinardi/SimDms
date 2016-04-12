var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');
var util = require('util');
var archiver = require('archiver');
var sqlODBC = require('node-sqlserver');
var Client = require('q-svn-spawn');
var config = require('../config');
var http = require('http');
var argv = require('yargs')
    .string('d')
    .argv;
	
var TaskName = "{{$TASKNAME$}}";
var TaskNo = "{{$TASKNO$}}";
var CurrentDealerCode = argv.d;

function hereDoc(f) {
  return f.toString().
	  replace(/^[^\/]+\/\*!?/, '').
	  replace(/\*\/[^\/]+$/, '');
}

var SQLCheck = hereDoc(function(){/*!
    declare @column_list varchar(MAX)
    SELECT @column_list = COALESCE(@column_list + ', ', '') + COLUMN_NAME 
    FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
    SELECT @column_list list, COUNT(*) total
    FROM INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='#TABLENAME#'
*/});

var SQL = hereDoc(function(){/*!
{{$SQL$}}
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


var start = function (cfg, callback) 
{
    log("Starting " + TaskName + " for " + CurrentDealerCode); 

	var file = path.join(__dirname,  TaskNo + ".sql");
	log(file);
	
    fs.writeFileSync(file, SQL );
	
	var i = cfg.ConnString.indexOf('}');
	var s = (cfg.ConnString.substr(i+2));
	
	s = s.replace('Server=','');
	i = s.indexOf(';');
	var server = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Database=','');
	i = s.indexOf(';');
	var dbname = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Uid=','');
	i = s.indexOf(';');
	var userid = s.substr(0,i);
	s = s.substr(i+1);
	s = s.replace('Pwd=','');
	var password = s;
		
	var StartTime = moment().format("YYYY-MM-DD HH:mm:ss");
	
    run_shell ('osql', ['-S',server,'-d',dbname,'-U',userid,'-P',password,'-i',file] ,function(err, code, result) {
        var output = result;						
        var FinishTime = moment().format("YYYY-MM-DD HH:mm:ss");                        
        var errDesc = err || '';
    });

	log('done');
}

function log(info) {
    var file = path.join(__dirname,  TaskNo + "-" + TaskName + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info );
}

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

startTasks();