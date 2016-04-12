var config = require('./config');
var moment = require('moment');
var unzip = require('unzip');
var async = require('async');
var sql = require('mssql');
var path = require('path');
var fs = require('fs');

var extractor = {
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
    log: function (info, empty) {
        var dirname = path.join(__dirname, './extract1');
        if (!fs.existsSync(dirname)) {
            fs.mkdirSync(dirname);
        }

        var filename = path.join(dirname, "/log." + moment().format("YYYY_MMDD") + ".log");
        if (empty) {
            fs.appendFileSync(filename, "\n" + info + "\n");
        }
        else {
            fs.appendFileSync(filename, moment().format("YYYY-MM-DD HH:mm:ss") + " : \n" + info + "\n");
        }
    },
    extract: function () {
        var basedir = path.join(__dirname, './extract1');
        var folders = fs.readdirSync(basedir);
        var nrecord = 0;
        var oTable = {};
        var loaded = false;
        var self = this;

        async.eachSeries(folders, process, function (err) {
            if (err) {
                console.log('error: ', JSON.stringify(err));
            }

            console.log('finish');
        });

        function process(folder, callback) {
            if (fs.lstatSync(path.join(basedir, folder)).isDirectory()) {
                var files = fs.readdirSync(path.join(basedir, folder));
                oTable.folder = folder;

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
                var filePath = path.join(basedir, oTable.folder, file);

                if (!loaded) {
                    var row = JSON.parse(fs.readFileSync(filePath))[0];
                    oTable.TableName = file.split('_')[0];
                    oTable.CompanyCode = row.CompanyCode;
                    loaded = true;
                }

                console.log(oTable.CompanyCode, oTable.TableName, 'start');

                self.getkeys({
                    name: oTable.TableName,
                    cnstr: config.sql.simdms,
                    callback: function (keys) {
                        oTable.keys = keys;
                        try {
                            var data = JSON.parse(fs.readFileSync(filePath));
                            var date = new Date();

                            async.eachSeries(data, populate, function (err) {
                                //callback(err);
                                fs.unlink(filePath, function (err) {
                                    if (err) {
                                        log(err);
                                        callback(err);
                                    }
                                    else {
                                        nrecord = 0;
                                        console.log(oTable.CompanyCode, oTable.TableName, 'done\n');
                                        self.query({
                                            cnstr: config.sql.simdms,
                                            query: 'uspfn_SysDealerLastProgress',
                                            params: { DealerCode: oTable.CompanyCode, TableName: oTable.TableName },
                                            callback: callback
                                        });
                                    }
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
            var keys = oTable.keys;

            sql += "delete " + oTable.TableName + " where ";

            try {
                for (var i in keys) {
                    if (i > 0) { sql += " and " }
                    sql += keys[i] + "='" + row[keys[i]] + "'";
                }
                sql += "\n";

                sql += "insert into " + oTable.TableName + " \n(";
                for (var field in row) {
                    cols += "," + field;

                    if (typeof row[field] == 'string') {
                        pars += ",'" + row[field].split("'").join("") + "'";
                    }
                    else if (typeof row[field] == 'boolean') {
                        pars += "," + (row[field] ? "1" : "0");
                    }
                    else {
                        pars += "," + row[field]
                    }
                }

                sql += cols.substr(1) + ") values \n(";
                sql += pars.substr(1) + ")";

                self.query({
                    cnstr: config.sql.simdms,
                    query: sql,
                    type: 'text',
                    callback: function (err) {
                        if (err) {
                            //throw err;
                            self.log(err);
                            self.log(sql, true);

                            var json = '\n';
                            for (var field in row) {
                                json += field + '\t' + typeof (row[field]) + '\t' + row[field] + '\n';
                            }
                            self.log(json, true);
                        }

                        nrecord++;

                        if ((nrecord % 100) == 0) console.log(moment().format('DD-MMM-YYYY  HH:mm:ss'), nrecord, 'uploadad');
                        if (callback) callback(err);
                    }
                });
            } catch (e) {
                //console.log(sql);
            }

        }
    }
}

//extractor.getkeys({
//    name: 'gnMstCoProfile',
//    cnstr: config.sql.simdms,
//    callback: function (keys) {
//        console.log('keys', keys)
//    }
//})

extractor.extract();