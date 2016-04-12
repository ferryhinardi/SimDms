var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');

var dblite = require('dblite');
var config = module.exports = {};

// server configuration
config.sql = {};
config.sql.simdms = { user: 'sa', password: 'P4ssw0rd-01', server: 'tbsdmsdb01', database: 'SimDms' };

config.dlr = {};
config.dlr.saveLogUrl = 'http://tbsdmsap01:9091/SchedulerLog/Add';
// eo server configuration

var connections = 
[
	{ 
		ConnString: 'Driver={SQL Server Native Client 10.0};Server=.;Database=SDMS;Uid=sa;Pwd=P4ssw0rd-01', 
		DealerCode: '6115204001ACH',
		DealerType: '4W',
		SimDMSPath: 'D:/SDMS/WEB_APPLICATION/SIMDMSWEB/RELEASE'
	}
];


var Api = { 
	simdata: 'http://180.250.66.246:9091/simdata/',
	upload : 'http://180.250.66.246:9091/upload',
	listTables: 'http://180.250.66.246:9091/api/tablelist',	
	TaskCheck: 'http://180.250.66.246:9091/api/jobs/check',	
	TaskLogger: 'http://180.250.66.246:9091/api/jobs/logger',	
	downloadLink: 'http://180.250.66.246:9091/',	
	uplcd  : 'UPLCD'
};

module.exports = {
	saveLogUrl: function()
	{
		return 'http://tbsdmsap01:9091/SchedulerLog/Add';
	},
	serverdb: function()
	{
		return { user: 'sa', password: 'P4ssw0rd-01', server: 'tbsdmsdb01', database: 'SimDms' };
	},
	sqlite3: function(filedb)
	{
		dblite.bin = "bin/sqlite3.exe";
		var dir = path.join(__dirname, '../log/temp');
	    if (!fs.existsSync(dir)) {
	        fs.mkdirSync(dir);
	    }
        return dblite(filedb);
	},
	sdmspath: function()
	{
		return "D:/Projects/SDMS";
	},
	api: function()
	{
		return Api;
	},
	conn: function()
	{
		return connections;
	},
	log: function (name, info) {
	    var dir = path.join(__dirname, '../log');
	    if (!fs.existsSync(dir)) {
	        fs.mkdirSync(dir);
	    }

	    dir = path.join(dir, moment().format("YYYY"));
	    if (!fs.existsSync(dir)) {
	        fs.mkdirSync(dir);
	    }

	    dir = path.join(dir, moment().format("MM"));
	    if (!fs.existsSync(dir)) {
	        fs.mkdirSync(dir);
	    }

	    var file = path.join(dir, name + "-" + moment().format("YYYYMMDD") + ".log");
	    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
	    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info);
	}
};