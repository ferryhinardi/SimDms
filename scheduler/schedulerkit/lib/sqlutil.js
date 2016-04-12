var moment = require('moment');
var async = require('async');
var path = require('path');
var sql = require('mssql');
var fs = require('fs');

var config = require('../config');

module.exports = {
    backup: function (tables, callback) {
        var options = config.backup.options;
        var method = options.method || "parallel";
        var dateStart = new Date();
        var tasks = [];

        options.tables = tables;
        (options.tables || []).forEach(function (table) {
            tasks.push(function (callback) {
                var conn = new sql.Connection(options.sqlcon, function (err) {
                    if (err) throw err;

                    var name = table.name;
                    var req = new sql.Request(conn);
                    req.query("exec sp_pkeys " + name, function (err, data) {
                        if (err) throw err;

                        var keys = [];
                        (data || []).forEach(function (row) {
                            keys.push(row["COLUMN_NAME"]);
                        });

                        if (keys.length == 0) {
                            keys = table.keys;
                        }

                        console.log(keys);

                        req.query("exec sp_columns " + name, function (err, data) {
                            var columns = [];
                            (data || []).forEach(function (row) {
                                columns.push(row["COLUMN_NAME"]);
                            });

                            var task = {
                                name: name,
                                keys: keys,
                                columns: columns,
                                segment: (options.segment || 1000),
                                zip: options.zip,
                                req: req,
                                filter: table.filter
                            }

                            backupTable(task, function (result) {
                                conn.close();
                                callback(null, result);
                            });
                        });
                    });
                });
            });
        });

        async.series(tasks, function (err, data) {
            if (err) throw err;

            callback(null, data);
        });
    },
    query: function (options, callback) {
        var conn = new sql.Connection(options.sqlcon, function (err) {
            if (err) throw err;

            var req = new sql.Request(conn);
            for (var key in (options.params || {})) {
                //console.log(typeof (options.params[key]));
                req.input(key, getParamValue(options.params[key]));
            }

            req.query(options.query, function (err, docs) {
                conn.close();
                callback(err, docs);
            });
        });
    },
    exec: function (options, callback) {
        var conn = new sql.Connection(options.sqlcon, function (err) {
            if (err) throw err;

            var req = new sql.Request(conn);
            for (var key in (options.params || {})) {
                req.input(key, options.params[key]);
            }

            req.execute(options.query, function (err, docs) {
                conn.close();
                callback(err, docs);
            });
        });
    },
    getkeys: function (options, callback) {
        var self = this;
        options.query = "exec sp_pkeys " + options.name;
        self.query(options, function (err, data) {
            if (err) throw err;

            var keys = [];
            (data || []).forEach(function (row) {
                keys.push(row["COLUMN_NAME"]);
            });

            callback(keys);
        });
    }
}

function getParamValue(val) {
    if (typeof val == "number") {
        return val.toString();
    }
    return val;
}


function backupTable(task, callback) {
    var name = task.name;
    var sql = "select count(*) as count from " + name + " where 1 = 1";
    var sqlflt = "";
    var req = task.req;
    var zip = new require('node-zip')();

    if (task.filter !== undefined && task.filter.value !== "1900-01-01") {
        var filter = task.filter;
        sqlflt = " and convert(varchar, " + filter.field + ", 112) >= " + moment(filter.value).format("YYYYMMDD");
    }

    req.query(sql + sqlflt, function (err, data) {
        var count = data[0].count;
        var pages = Math.ceil(count / task.segment);
        var segment = task.segment;

        var backups = [];

        for (var i = 0; i < pages; i++) {
            var builder = "select row_number() over(order by " + task.keys + ") as row, * from " + name + " where 1 = 1" + sqlflt;
            builder = "select " + task.columns + " from (" + builder + ")# where row > " + (segment * i) + " and row <= " + (segment * (i + 1))

            if (task.zip == true) {
                backups.push(zipTable({ name: name, req: req, sql: builder, page: i, pages: pages, zip: zip }));
            }
            else {
                backups.push(saveTable({ name: name, req: req, sql: builder, page: i, pages: pages }));
            }
        }

        async.series(backups, function (err, docs) {
            if (err) throw err;

            callback(docs);
        })
    })
}

function saveTable(options) {
    var req = options.req;
    var sql = options.sql;
    var name = options.name;
    var page = options.page;
    var pages = options.pages;

    return function (callback) {
        req.query(sql, function (err, result) {
            var dir = "backup";
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir);
            }

            dir = dir + "/" + name;
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir);
            }

            var file = dir + "/" + name + "_" + (page + 1001).toString().substr(1) + ".json";
            fs.writeFile(file, JSON.stringify(result), function (err) {
                if (err) { throw err };

                var info = file + " - saved" + ", progress " + (page + 1).toString() + " / " + pages.toString();
                log("FILEDATA", info);
                callback(null, file + " - saved");
            })
        })
    }
}

function zipTable(options) {
    var req = options.req;
    var sql = options.sql;
    var name = options.name;
    var page = options.page;
    var pages = options.pages;
    var zip = options.zip;

    return function (callback) {
        req.query(sql, function (err, result) {
            var dir = "backup";
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir);
            }

            var file = name + "/" + name + "_" + (page + 1001).toString().substr(1) + ".json";
            zip.file(file, JSON.stringify(result));

            var data = zip.generate({ base64: false, compression: 'DEFLATE' });
            var fileZip = (dir + "/" + name + moment(new Date()).format("_YYYY_MMDD") + ".zip");
            fs.writeFile(fileZip, data, 'binary', function (err) {
                if (err) { throw err };

                var info = fileZip + " - saved" + ", progress " + (page + 1).toString() + " / " + pages.toString();
                log("ZIPTABLE", info);
                callback(null, fileZip + " - saved");
            });
        })
    }
}

Date.prototype.toJSON = function () {
    var dt = this.toISOString().substr(0, 10) + " " + this.toTimeString().substr(0, 8);
    return dt;
}

function log(name, info) {
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

    var file = path.join(dir, name + "." + moment().format("YYYY_MMDD") + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info);
}