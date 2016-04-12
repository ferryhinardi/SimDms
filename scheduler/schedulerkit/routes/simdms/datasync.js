var express = require('express');
var moment = require("moment");
var async = require("async");
var unzip = require("unzip");
var path = require('path');
var fs = require('fs');
var config = require("../../config");
var sqlutil = require("../../lib/sqlutil");

var isbusy = false;

exports.start = function () {
    setInterval(function () {
        if (!isbusy) {
            var basedir = path.join(__dirname, "../../", "extract");
            var dealers = fs.readdirSync(basedir);
            if (dealers.length > 0) {
                watchFolder();
            }
            else {
                watchOutstanding();
            }
        }
    }, 2500);

    watchOutstanding();

    // make sure only one service running
    var app = express();
    app.get("/", function (req, res) { res.send("home") });
    app.listen(9090);
}

function watchFolder() {
    isbusy = true;
    var currentDealer = "";
    var currentFolder = "";
    var currentData = [];
    var basedir = path.join(__dirname, "../../", "extract");
    var dealers = fs.readdirSync(basedir);

    if (dealers.length > 0) {
        async.eachSeries(dealers, readDealer, function (err) {
            if (err) {
                isbusy = false;
                throw err;
            }

            setTimeout(function () {
                isbusy = false;
            }, 2500);
        })

        function readDealer(dealer, callback) {
            var dir = path.join(basedir, dealer);

            if ((dealer.length == 7 || dealer.length == 10) && fs.statSync(dir).isDirectory()) {
                currentDealer = dealer;

                var folders = fs.readdirSync(dir);
                async.eachSeries(folders, readFolder, function (err) {
                    if (err) {
                        isbusy = false;
                        throw err;
                    }

                    fs.rmdir(dir, function () {
                        callback();
                    });
                })
            }
            else {
                callback();
            }
        }

        function readFolder(folder, callback) {
            var dir = path.join(basedir, currentDealer, folder);

            if ((folder.length == 9) && fs.statSync(dir).isDirectory()) {
                currentFolder = folder;

                var datas = fs.readdirSync(dir);
                if ((datas || []).length == 0) {
                    fs.rmdir(dir, function (err) {
                        if (err) log(err, 86);

                        callback();
                    });
                }
                else {
                    async.eachSeries(datas, readData, function (err) {
                        if (err) {
                            isbusy = false;
                            throw err;
                        }

                        fs.rmdir(dir, function (err) {
                            if (err) {
                                log(err, 99);
                                isbusy = false;
                            }

                            callback();
                        });
                    })
                }
            }
            else {
                callback();
            }
        }

        function readData(name, callback) {
            var dir = path.join(basedir, currentDealer, currentFolder, name);

            if (fs.statSync(dir).isDirectory()) {
                var keys = [];
                var files = fs.readdirSync(dir);

                sqlutil.getkeys({
                    name: name,
                    sqlcon: config.sql.simdms,
                }, function (result) {
                    keys = result;
                    currentData = files;

                    startQueueing(name, currentDealer, currentFolder);

                    async.eachSeries(files, populateData, function (err) {
                        if (err) {
                            isbusy = false;
                            throw err;
                        }

                        sqlutil.exec({
                            sqlcon: config.sql.simdms,
                            query: "uspfn_SysDealerHistProcess",
                            params: { DealerCode: currentDealer, TableName: name, DataDate: currentFolder }
                        }, function (err) {
                            if (err) {
                                isbusy = false;
                                throw err;
                            }

                            fs.rmdir(dir, function (err) {
                                if (err) {
                                    log(err, 144);
                                    isbusy = false;
                                }

                                log(name + ' Updated');
                                callback();
                            });
                        });
                    });
                });

                function populateData(file, callback) {
                    var date = new Date();
                    var filePath = path.join(dir, file);

                    // check if filesize = 0 bypass
                    if (fs.statSync(filePath).size == 0) {
                        fs.unlink(filePath, function (err) {
                            if (err) {
                                log(err, 160);
                            }
                            else {
                                log(file + " processed 0 (" + currentDealer + "," + currentFolder + ","
									+ parseInt(file.substr(file.length - 8, 3))
									+ " / " + files.length + ")");
                            }
                            callback();
                        });
                    }
                    else {
                        try {
                            var data = JSON.parse(fs.readFileSync(filePath));
                            async.each(data, populateRow, function (err) {
                                if (err) {
                                    isbusy = false;
                                    throw err;
                                }

                                fs.unlink(filePath, function (err) {
                                    if (err) {
                                        log(err, 182);
                                    }
                                    else {
                                        log(file + " processed (" + currentDealer + "," + currentFolder + ","
											+ parseInt(file.substr(file.length - 8, 3))
											+ " / " + files.length + ")");
                                    }
                                    callback();
                                });
                            });
                        }
                        catch (err) {
                            // console.log(err);
                            log(err, 195);
                            fs.unlink(filePath, function (err) {
                                if (err) {
                                    log(err, 198);
                                }
                                else {
                                    log(file + " processed fail (" + currentDealer + "," + currentFolder + ","
										+ parseInt(file.substr(file.length - 8, 3))
										+ " / " + files.length + ")");
                                }
                                callback();
                            });
                        }
                    }
                }

                function populateRow(row, callback) {
                    var sql = "";
                    var cols = "";
                    var pars = "";
                    var data = {};

                    sql += "delete " + name + " where ";

                    for (var i in keys) {
                        if (i > 0) { sql += " and " }
                        sql += keys[i] + "=@" + keys[i];
                        data[keys[i]] = row[keys[i]];
                    }
                    sql += "\n";

                    sql += "insert into " + name + " \n(";
                    for (var field in row) {
                        cols += "," + field;
                        pars += ",@" + field;
                    }
                    sql += cols.substr(1) + ") values \n(";
                    sql += pars.substr(1) + ")";

                    sqlutil.query({
                        sqlcon: config.sql.simdms,
                        query: sql,
                        params: row
                    }, function (err) {
                        if (err) {
                            console.log(sql);
                            console.log(row);
                            isbusy = false;
                            throw err;
                        }
                        callback();
                    });
                }
            }
            else {
                callback();
            }
        }
    }
    else {
        isbusy = false;
    }
}

function watchOutstanding() {
    if (isbusy) return;

    isbusy = true;

    var basedir = path.join(__dirname, "../../", "extract");
    var dealers = fs.readdirSync(basedir);
    if (dealers.length > 0) {
        watchFolder();
        return;
    }

    sqlutil.exec({
        sqlcon: config.sql.simdms,
        query: "uspfn_SysDealerHistGet",
    }, function (err, docs) {
        if (err) {
            isbusy = false;
            throw err;
        }

        if (docs.length > 0 && docs[0].length > 0 && docs[0][0].FileName.length > 15) {
            var doc = docs[0][0];
            var zippath = path.join(__dirname, "../../upload", doc.FilePath);
            if (fs.existsSync(zippath)) {
                var data = {
                    UploadID: doc.UploadID,
                    DealerCode: doc.DealerCode,
                    DataDate: doc.FileName.substr(doc.FileName.length - 13, 9),
                    TableName: doc.TableName
                }

                // check if filesize = 0 bypass
                if (fs.statSync(zippath).size == 0) {
                    log(data.UploadID + ' ERROR');
                    flagError(data.UploadID);
                    isbusy = false;
                    return;
                }

                // make sure folder dealer exists
                var dirpath = path.join(__dirname, "../../extract", data.DealerCode);
                if (!fs.existsSync(dirpath)) {
                    fs.mkdirSync(dirpath);
                }

                // make sure folder datadate exists
                var dirpath = path.join(dirpath, data.DataDate);
                if (!fs.existsSync(dirpath)) {
                    fs.mkdirSync(dirpath);
                }

                var stream = fs.createReadStream(zippath).pipe(unzip.Extract({ path: dirpath }));

                stream.on('error', function (err) {
                    log(err, 312);
                    isbusy = false;
                });
                stream.on('close', function () {
                    log(doc.FileName + " Extracted");
                    setTimeout(function () {
                        watchFolder()
                    }, 2500);
                });
            }
            else {
                isbusy = false;
            }
        }
        else {
            isbusy = false;
        }
    });
}

function startQueueing(name, currentDealer, currentFolder) {
    sqlutil.exec({
        sqlcon: config.sql.simdms,
        query: "uspfn_SysDealerHistQueueing",
        params: { DealerCode: currentDealer, TableName: name, DataDate: currentFolder }
    }, function (err) {
        if (err) {
            isbusy = false;
            throw err;
        }
    });
}

function flagError(id) {
    sqlutil.exec({
        sqlcon: config.sql.simdms,
        query: "uspfn_SysDealerHistUpdStatus",
        params: { UploadID: id, Status: 'ERROR', Execute: 1 }
    }, function (err) {
        if (err) {
            isbusy = false;
            throw err;
        }
    });
}

function log(info, line) {
    var dir = path.join(__dirname, '../../log');
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

    var file = path.join(dir, "/DATASYNC." + moment().format("YYYY_MMDD") + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + ((line == undefined) ? '' : ' - ' + line.toString()) + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info, (line || ''));
}