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

            extract();
        }
    }, 1000);

    extract();

    // make sure only one service running
    var app = express();
    app.get("/", function (req, res) { res.send("home") });
    app.listen(9093);
}

function extract() {
    if (isbusy) return;

    var id = createGuid();
    isbusy = true;
    logData(id, '-- begin --');

    checkOutstanding({ id: id }, function (err, docs) {
        isbusy = false;
        logData(id, '-- finish --\n');
    });
}

function extractData(options, callback) {
    var basedir = path.join(__dirname, '../../', 'extract', options.DealerCode);
    var cfolder = '';

    if (fs.existsSync(basedir)) {
        var folders = fs.readdirSync(basedir);
        async.eachSeries(folders, readFolder, function (err) {
            fs.rmdirSync(basedir);

            if (err) throw err;
            if (callback) callback();
        })

        function readFolder(folder, callback) {
            var dir = path.join(basedir, folder);

            if ((folder.length == 9) && fs.statSync(dir).isDirectory()) {
                cfolder = folder;

                var datas = fs.readdirSync(dir);
                if ((datas || []).length == 0) {
                    fs.rmdirSync(path);
                    if (callback) callback();
                }
                else {
                    async.eachSeries(datas, readData, function (err) {
                        fs.rmdirSync(dir);

                        if (err) throw err;
                        if (callback) callback();
                    })
                }
            }
            else {
                logData(options, 'invalid folder');

                fs.rmdirSync(dir);
                if (callback) callback();
            }
        };

        function readData(name, callback) {
            var dir = path.join(basedir, cfolder, name);
            var datas = fs.readdirSync(dir);

            if ((datas || []).length == 0) {
                fs.rmdirSync(path);
                if (callback) callback();
            }
            else {
                async.eachSeries(datas, populateData, function (err) {
                    fs.rmdirSync(dir);

                    if (err) throw err;
                    if (callback) callback();
                })
            }

            function populateData(data, callback) {
                var filePath = path.join(dir, data);

                fs.unlinkSync(filePath);
                if (callback) callback();
            }
        }
    }
    else {
        flagError(options.UploadID, 'Extracted data not found')
        if (callback) callback();
    }
}

function checkOutstanding(options, callback) {
    sqlutil.exec({
        sqlcon: config.sql.simdms,
        query: "uspfn_SysDealerHistGet",
    }, function (err, docs) {
        if (err) logData(options, err);

        if (docs.length > 0 && docs[0].length > 0 && docs[0][0].FileName.length > 15) {
            var doc = docs[0][0];
            var zippath = path.join(__dirname, "../../upload", doc.FilePath);

            if (fs.existsSync(zippath)) {
                var data = {
                    UploadID: doc.UploadID,
                    DealerCode: doc.DealerCode,
                    DataDate: doc.FileName.substr(doc.FileName.length - 13, 9),
                    TableName: doc.TableName,
                }
                options.DealerCode = doc.DealerCode;
                options.UploadID = doc.UploadID;

                // check if filesize = 0 bypass
                if (fs.statSync(zippath).size == 0) {
                    flagError(data.UploadID);

                    isbusy = false;
                    if (callback) callback();
                    return;
                }

                // make sure folder dealer exists
                var dirpath = path.join(__dirname, "../../extract", data.DealerCode);
                if (!fs.existsSync(dirpath)) {
                    fs.mkdirSync(dirpath);
                }

                // make sure folder datadate exists
                var dirpath = path.join(dirpath, data.DataDate);
                debug(options, dirpath)
                if (!fs.existsSync(dirpath)) {
                    fs.mkdirSync(dirpath);
                }

                var stream = fs.createReadStream(zippath).pipe(unzip.Extract({ path: dirpath }));
                debug(options, 'stream');
                stream.on('error', function (err) {
                    flagError(data.UploadID);

                    isbusy = false;
                    if (callback) callback();
                });
                stream.on('close', function () {
                    debug(options, 'close');
                    debug(options, doc.FileName + " Extracted");

                    logData(options, 'begin extract data');
                    extractData(options, function () {
                        isbusy = false;
                        logData(options, 'end extract data');
                        if (callback) callback();
                    });
                });
            }
            else {
                flagError(doc.UploadID);

                isbusy = false;
                if (callback) callback();
            }
        }
        else {
            isbusy = false;
            if (callback) callback();
        }
    });
}

function debug(options, msg) {
    var id = (typeof options == "string") ? options : options.id;
    //console.log(id.substr(24).toUpperCase(), msg);
    //console.log(moment().format("YYYY-MM-DD HH:mm:ss"), "-", id.substr(24).toUpperCase(), ":", msg);
}

function logData(options, msg) {
    var id = (typeof options == "string") ? options : options.id;
    var date = moment().format("YYYY-MM-DD HH:mm:ss");
    var info = id.substr(24, 8).toUpperCase();
    msg = ((options.DealerCode == undefined) ? '' : ('(' + options.DealerCode + ') '))
        + msg;

    console.log(date, "-", info, ":", msg);

    var dir = path.join(__dirname, '../../log');
    if (!fs.existsSync(dir)) { fs.mkdirSync(dir); }

    dir = path.join(dir, moment().format("YYYY"));
    if (!fs.existsSync(dir)) { fs.mkdirSync(dir); }

    dir = path.join(dir, moment().format("MM"));
    if (!fs.existsSync(dir)) { fs.mkdirSync(dir); }

    var file = path.join(dir, "/DATASYNC." + moment().format("YYYY_MMDD") + ".log");
    fs.appendFileSync(file, date + ' - ' + info + ' : ' + msg + "\n");
}

function flagError(id, message) {
    sqlutil.exec({
        sqlcon: config.sql.simdms,
        query: "uspfn_SysDealerHistUpdStatus",
        params: { UploadID: id, Status: 'ERROR', ErrMsg: (message || ''), Execute: 1 }
    }, function (err) {
        //if (err) throw err;
        console.log(err);
    });
}

function createGuid() {
    var s = [];
    var hexDigits = "0123456789abcdef";
    for (var i = 0; i < 36; i++) {
        s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
    }
    s[14] = "4";  // bits 12-15 of the time_hi_and_version field to 0010
    s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01
    s[8] = s[13] = s[18] = s[23] = "-";

    var uuid = s.join("");
    return uuid;
}
