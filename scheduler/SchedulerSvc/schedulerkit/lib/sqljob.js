var sql = require('mssql');
var async = require('async');
var moment = require('moment');
var fs = require('fs');
var request = require('request');
var config = require('../config');

module.exports = {
    run: function (options) {
        var duration = options.duration;
        var interval = (duration.d || 0) * 1000 * 60 * 60 * 24
                     + (duration.h || 0) * 1000 * 60 * 60
                     + (duration.m || 0) * 1000 * 60
                     + (duration.s || 0) * 1000;

        run(options, interval);

        setInterval(function () {
            run(options, interval);
        }, interval);
    },
    exec: function () { },
    log: function (name, data) {
        log(name, data);
    }
}

function run(options, interval) {
    var method = options.method || "parallel";
    var dateStart = new Date();
    var tasks = [];

    if ((options.runningtimes || []).length > 0) {
        var stime = moment().format('HH:mm:ss');
        var valid = false;
        (options.runningtimes).forEach(function (runningtime) {
            valid = (valid || (stime >= runningtime.start && stime <= runningtime.finish));
        });

        if (!valid) return;
    }

    (options.tasks || []).forEach(function (task) {
        if (task.sqlcon == undefined) {
            task.sqlcon = options.sqlcon;
        }
        tasks.push(exec(task));
    });

    switch (method) {
        case "parallel":
            async.parallel(tasks, function (err, docs) {
                var dateFinish = new Date();

                log("jobs", {
                    Name: options.name,
                    Info: options.info,
                    Interval: (interval / (1000 * 60)).toString() + " minutes",
                    DateStart: dateStart,
                    DateFinish: dateFinish,
                    RunningTimes: options.runningtimes,
                    Err: err,
                    Docs: docs
                });
				
				serverLogging(options, err, dateStart, dateFinish);
            });	
			
            break;
        case "series":
            async.series(tasks, function (err, docs) {
                var dateFinish = new Date();

                log("jobs", {
                    Name: options.name,
                    Info: options.info,
                    Interval: (interval / (1000 * 60)).toString() + " minutes",
                    DateStart: dateStart,
                    DateFinish: dateFinish,
                    RunningTimes: options.runningtimes,
                    Err: err,
                    Docs: docs
                });
				
				serverLogging(options, err, dateStart, dateFinish);
            });
			
            break;
        default:
            break;
    }
}

function exec(task) {
    return function (callback) {
        var dateStart = new Date();
        var connection = new sql.Connection(task.sqlcon, function (err) {
            var request = new sql.Request(connection);
            var query = task.command;
            request.query(query, function (err, result) {
                connection.close();

                var errmsg;
                if ((err || "") !== "") {
                    errmsg = err.toString();
                }

                var dateFinish = new Date();
                callback(err, { TaskName: task.name, TaskInfo: task.info, Command: task.command, DateStart: dateStart, DateEnd: dateFinish, ErrMsg: errmsg, Docs: result })
            });
        });
    }
}

function log(name, data) {
    var dir = 'log';
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir);
    }

    dir = dir + "/" + moment().format("YYYY");
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir);
    }

    dir = dir + "/" + moment().format("MM");
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir);
    }

    dir = dir + "/" + moment().format("DD");
    if (!fs.existsSync(dir)) {
        fs.mkdirSync(dir);
    }

    var file = dir + "/" + (data.Name || name) + "." + moment().format("YYYY.MM.DD__HH.mm.ss") + ".log";
    fs.writeFile(file, JSON.stringify(data, null, "\t"), function (err) {
        if (err) throw err;
    });
}

Date.prototype.toJSON = function () {
    var dt = new Date().toISOString().substr(0, 10) + " " + this.toTimeString().substr(0, 8);
    return dt;
}

function serverLogging(options, err, dateStart, dateFinish) {
	console.log('logging');
	var isError = false;
	
	if(err) {
		isError = true;
	}
		
	request.post(
		config.saveLogUrl(),
		{ 
			form: { 
				DealerCode: "0000000000",
				ScheduleName: options.name || "",
				DateStart: moment(dateStart).format('YYYY-MM-DD hh:mm:ss'),
				DateFinish: moment(dateFinish).format('YYYY-MM-DD hh:mm:ss'),
				RunningTimes: options.runningtimes,
				IsError: isError,
				ErrorMessage: err,
				Info: options.info
			}
		},
		function (error, response, body) {
			if(error == null || error == undefined) {
				var result = JSON.parse(body);
				log((options.name || 'ServerLogging'), moment(new Date()).format('YYYY-MMM-DD mm:hh:ss') + ' ' + result.info);
			}
		}
	);
}