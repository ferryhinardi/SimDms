var moment = require("moment");
var async = require("async");
var path = require('path');
var fs = require('fs');
var config = require("../../config");
var sqlutil = require("../../lib/sqlutil");

var dir = path.join(__dirname, "../../extract");
if (fs.existsSync(dir)) {
    var names = fs.readdirSync(dir);
    if (names.length > 0) {
        names.forEach(function (name) {
            var dirname = path.join(dir, name);

            var keys = [];
            if (fs.statSync(dirname).isDirectory()) {
                var date = new Date();
                var files = fs.readdirSync(dirname);

                sqlutil.getkeys({
                    name: name,
                    sqlcon: config.sql.simdms,
                }, function (result) {
                    keys = result;
                    async.eachSeries(files, populateData, function (err) {
                        if (err) throw err;

                        fs.rmdirSync(dirname);
                        console.log(name, moment(date).format("HH:mm:ss"), moment().format("HH:mm:ss"), " - done ");
                    });
                });
            }

            function populateData(file, callback) {
                var date = new Date();
                var filePath = path.join(dirname, file);
                var filePathRead = path.join(dirname, file) + ".read";

                fs.rename(filePath, filePathRead, function (err) {
                    if (err) throw err;

                    var data = JSON.parse(fs.readFileSync(filePathRead));

                    async.each(data, populateRow, function (err) {
                        if (err) throw err;

                        fs.unlinkSync(filePathRead);
                        console.log(file, moment(date).format("HH:mm:ss"), moment().format("HH:mm:ss"), " - done ");
                        callback();
                    });
                });
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
                }, function (err, result) {
                    if (err) throw err;

                    callback();
                });
                //callback();
            }
        });
    }
}
