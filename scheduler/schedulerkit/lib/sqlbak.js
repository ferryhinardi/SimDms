var moment = require('moment');
var async = require('async');
var request = require('request');
var path = require('path');
var fs = require('fs');

var sqlutl = require('./sqlutil');
var config = require('../config');

module.exports = {
    start: function (tables) {
        var self = this;
        async.each(tables, gettable, function (err) {
            if (err) throw err;

            sqlutl.backup(tables, function (err, result) {
                if (err) throw err;

                self.upload();
            });
        })

        function gettable(table, callback) {
            if (table.filter == undefined) {
                callback();
            }
            else {
                var params = {
                    uri: config.dlr.simdata,
                    method: "POST",
                    form: {
                        DealerCode: config.dlr.code,
                        TableName: table.name
                    }
                };

                request(params, function (e, r, body) {
                    table.filter.value = body;
                    callback();
                });
            }
        }
    },
    upload: function (callback) {
        fs.readdir(path.join(__dirname, "../backup"), function (err, files) {
            async.each(files, sendFile, function (err) {
                //if (err) throw err;

                if (callback) {
                    callback(err);
                }
            })
        });

        function sendFile(file, cb) {
            var fileName = path.join(__dirname, "../backup", file);
            var req = request.post(config.dlr.upload, function (e, r, body) {
                if (body !== undefined) {
                    fs.unlinkSync(fileName);

                    var info = file + " - uploaded";
                    log("ZIPTABLE", info);
                }

                console.log(body);
                if (cb) cb();
            });
            var form = req.form();
            form.append("DealerCode", config.dlr.code);
            form.append("UploadCode", config.dlr.uplcd);
            form.append('file', fs.createReadStream(fileName));
        }
    }
};

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