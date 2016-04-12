var config = require('../config');
var moment = require('moment');
var unzip = require('unzip');
var async = require('async');
var sql = require('mssql');
var path = require('path');
var fs = require('fs');

module.exports = {
    start: function () {
        var dir = config.upload.path;
        var self = this;
        var busy = false;
        var dealers = fs.readdirSync(dir);

        runExtractor(20, 1);

        function runExtractor(length, segment) {
            busy = true;
            getOutstanding(length, function (err, docs) {
                console.log(length, err, docs);
                if (err) {
                    JSON.stringify(function () {
                        self.log('error connect to database')

                        busy = false;
                    }, 5000);
                }
                else {
                    if (docs && docs[0]) {
                        async.eachSeries(docs[0], extractData, function (err) {
                            if (err) {
                                console.log('error: ', JSON.stringify(err));
                            }

                            busy = false;
                        });
                    }
                }
            });
        }

        function extractData(options, callback) {
            var zippath = path.join(dir, options.FilePath);

            if (fs.existsSync(zippath)) {
                console.log(zippath);
                flagQueueing({
                    id: options.UploadId,
                    callback: function (e, docs) {

                        console.log(e, docs)

                        if (callback) callback();
                    }
                })
            }
            else {
                flagError({ id: options.UploadId, message: 'file not found' });
                callback();
            }

            callback();

            //console.log(iterator++);

            return;
            if (fs.existsSync(zippath)) {
                var basedir = path.join(__dirname, '../');
                var arrpath = options.FilePath.split('/');

                var dirpath = path.join(basedir, 'temp');
                if (!fs.existsSync(dirpath)) {
                    fs.mkdirSync(dirpath);
                }

                var dirpath = path.join(dirpath, arrpath[0] + '_' + arrpath[1] + arrpath[2]);
                if (!fs.existsSync(dirpath)) {
                    fs.mkdirSync(dirpath);
                }

                var stream = fs.createReadStream(zippath).pipe(unzip.Extract({ path: dirpath }));
                stream.on('error', function (err) {
                    if (callback) callback(err);
                });
                stream.on('close', function () {
                    var tableName = arrpath[3].substr(0, arrpath[3].length - 14);
                    var tableKeys = [];

                    self.getkeys({
                        name: tableName,
                        cnstr: config.sql.simdms,
                        callback: function (keys) {
                            tableKeys = keys;
                            if (keys.length > 0) {
                                log(tableName + ' - begin');

                                try {
                                    var files = fs.readdirSync(path.join(dirpath, tableName));
                                    async.eachSeries(files, populateData, function (err) {
                                        if (err) {
                                            console.log('ERR Populate Data: ', JSON.stringify(err));
                                            flagNotFound({
                                                id: options.UploadId,
                                                callback: callback
                                            });
                                        }
                                        else {
                                            log(tableName + ' - end\n');

                                            try {
                                                fs.rmdir(path.join(dirpath, tableName));
                                            } catch (e) {
                                                log(JSON.stringify(e));
                                            }

                                            flagFinish({
                                                id: options.UploadId,
                                                callback: callback
                                            });
                                        }
                                    });
                                } catch (e) {
                                    flagNotFound({
                                        id: options.UploadId,
                                        callback: callback
                                    });
                                }
                            }
                        }
                    });


                    function populateData(file, callback) {
                        log(file);

                        var filePath = path.join(dirpath, tableName, file);
                        try {
                            var data = JSON.parse(fs.readFileSync(filePath));
                            var date = new Date();

                            async.eachLimit(data, 10, populateRow, function (err) {
                                //async.each(data, populateRow, function (err) {
                                if (err) throw err;

                                fs.unlink(filePath, function (err) {
                                    if (err) log(err);

                                    if (callback) callback();
                                });
                            });
                        } catch (e) {
                            log('ERR Read File: ' + filePath);
                            log('ERR Read File: ' + 'continue running');

                            if (callback) callback();
                        }
                    }

                    function populateRow(row, callback) {
                        var sql = "";
                        var cols = "";
                        var pars = "";
                        var data = {};
                        var keys = tableKeys;

                        sql += "delete " + tableName + " where ";

                        for (var i in keys) {
                            if (i > 0) { sql += " and " }
                            //sql += keys[i] + "=@" + keys[i];
                            //data[keys[i]] = row[keys[i]];
                            sql += keys[i] + "='" + row[keys[i]] + "'";
                        }
                        sql += "\n";

                        sql += "insert into " + tableName + " \n(";
                        for (var field in row) {
                            cols += "," + field;
                            pars += ",'" + row[field].split("'").join("") + "'";
                        }
                        sql += cols.substr(1) + ") values \n(";
                        sql += pars.substr(1) + ")";


                        log(sql);
                        //log(sql);
                        //log(sql);
                        //if (callback) callback();

                        self.query({
                            cnstr: config.sql.simdms,
                            query: sql,
                            //params: row,
                            type: 'text',
                            callback: function (err) {
                                if (err) {
                                    log('ERR PG 150: ' + sql);
                                    log('ERR PG 151: ' + JSON.stringify(row));
                                    log('ERR PG 152: ' + JSON.stringify(err));
                                    log('ERR PG 153: ' + 'continue running');
                                }

                                if (callback) callback();
                            }
                        });
                    }
                });
            }
            else {
                flagNotFound({
                    id: options.UploadId,
                    callback: callback
                });
            }
        }

        function flagQueueing(options) {
            self.query({
                cnstr: config.sql.simdms,
                query: 'uspfn_SysDealerFlagQueueing',
                params: { id: options.id },
                callback: options.callback
            });
        }

        function flagError(options) {
            self.query({
                cnstr: config.sql.simdms,
                query: 'uspfn_SysDealerFlagError',
                params: { id: options.id, message: options.message },
                callback: options.callback
            });
        }

        function flagFinish(options) {
            self.query({
                cnstr: config.sql.simdms,
                query: 'uspfn_SysDealerUpdated',
                params: { id: options.id },
                callback: options.callback
            });
        }

        function getOutstanding(length, callback) {
            self.query({
                cnstr: config.sql.simdms,
                query: 'uspfn_SysDealerHistGet2',
                params: { length: length },
                callback: callback
            });
        }
    },
    log: function (info, id) {
        var dirname = path.join(__dirname, '../log');
        if (!fs.existsSync(dirname)) {
            fs.mkdirSync(dirname);
        }

        dirname = path.join(dirname, moment().format("YYYY"));
        if (!fs.existsSync(dirname)) {
            fs.mkdirSync(dirname);
        }

        dirname = path.join(dirname, moment().format("MM"));
        if (!fs.existsSync(dirname)) {
            fs.mkdirSync(dirname);
        }

        var filename = path.join(dirname, "/EXTRACTOR." + moment().format("YYYY_MMDD") + ".log");
        fs.appendFileSync(filename, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + "\n");
        console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info);
    },
    query: function (options) {
        var self = this;
        var conn = new sql.Connection(options.cnstr, function (err) {
            if (err) throw err;

            var req = new sql.Request(conn);
            if (options.params) {
                var params = options.params;
                for (var key in params) {
                    req.input(key, params[key]);
                }
            }

            if (options.type == 'text') {
                req.query(options.query, function (err, docs) {
                    conn.close();

                    if (options.callback) options.callback(err, docs);
                });
            }
            else {
                req.execute(options.query, function (err, docs) {
                    conn.close();

                    if (options.callback) options.callback(err, docs);
                });
            }
        });
    },
    getkeys: function (options) {
        this.query({
            cnstr: options.cnstr,
            query: 'exec sp_pkeys ' + options.name,
            type: 'text',
            callback: function (err, data) {
                if (err) throw err;

                var keys = [];
                (data || []).forEach(function (row) {
                    keys.push(row["COLUMN_NAME"]);
                });

                if (options.callback) options.callback(keys);
            }
        });
    },
    extract: function () {
        var basedir = path.join(__dirname, '../extract');
        var folders = fs.readdirSync(basedir);
        var dealer = '';
        var tableName = '';
        var tableKeys = [];
        var self = this;

        async.eachSeries(folders, process, function (err) {
            if (err) {
                console.log('error: ', JSON.stringify(err));
            }

            console.log('finish');
        });

        function process(folder, callback) {
            if (fs.lstatSync(path.join(basedir, folder)).isDirectory()) {
                var row = { DealerName: folder };
                var files = fs.readdirSync(path.join(basedir, folder));
                dealer = folder;

                console.log('start process', row);
                async.eachSeries(files, processFile, function (err) {
                    if (callback) callback();
                });
            }
            else {
                if (callback) callback();
            }
        }

        function processFile(file, callback) {
            if (file.split('.')[1] == 'json') {
                var filePath = path.join(basedir, dealer, file);
                tableName = file.split('_')[0];

                //console.log('start process', file);
                self.getkeys({
                    name: tableName,
                    cnstr: config.sql.simdms,
                    callback: function (keys) {
                        tableKeys = keys;
                        try {
                            var data = JSON.parse(fs.readFileSync(filePath));
                            var date = new Date();

                            //console.log(data.length, data[0]);
                            async.eachSeries(data, populate, function (err) {
                                fs.unlink(filePath, function (err) {
                                    if (err) log(err);

                                    console.log('finish process', file);

                                    self.query({
                                        cnstr: config.sql.simdms,
                                        query: 'uspfn_SysDealerLastProgress',
                                        params: { DealerCode: dealer, TableName: tableName },
                                        callback: callback
                                    });
                                });
                            });
                        } catch (e) {
                            console.log('ERR Read File: ' + filePath);
                            if (callback) callback(e);
                        }
                    }
                });
            }
            else {
                if (callback) callback();
            }
        }

        function populate(row, callback) {
            var sql = "";
            var cols = "";
            var pars = "";
            var data = {};
            var keys = tableKeys;

            sql += "delete " + tableName + " where ";

            try {
                for (var i in keys) {
                    if (i > 0) { sql += " and " }
                    sql += keys[i] + "='" + row[keys[i]] + "'";
                }
                sql += "\n";

                sql += "insert into " + tableName + " \n(";
                for (var field in row) {
                    cols += "," + field;

                    if (row[field] == 'null') {
                        pars += "," + row[field]
                    }
                    else if (typeof row[field] == 'string') {
                        pars += ",'" + row[field].split("'").join("") + "'";
                    }
                    else if (typeof row[field] == 'number') {
                        pars += "," + row[field]
                    }
                    else {
                        pars += ",'" + row[field] + "'"
                    }
                }

                sql += cols.substr(1) + ") values \n(";
                sql += pars.substr(1) + ")";

                self.query({
                    cnstr: config.sql.simdms,
                    query: sql,
                    //params: row,
                    type: 'text',
                    callback: function (err) {
                        if (err) {
                            console.log('ERR PG 150: ' + sql);
                            console.log('ERR PG 151: ' + JSON.stringify(row));
                            console.log('ERR PG 152: ' + JSON.stringify(err));
                        }

                        if (callback) callback();
                    }
                });
            } catch (e) {
                //console.log(sql);
            }

        }
    }
}