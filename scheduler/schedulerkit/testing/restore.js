var express = require('express');
var moment = require('moment');
var async = require('async');
var unzip = require('unzip');
var path = require('path');
var fs = require('fs');

var config = require("../config");
var sqlutil = require("../lib/sqlutil");

var isbusy = false;

var currentDealer = '';
var currentFolder = '';
var currentData = [];
var basedir = path.join(__dirname, 'data');
var files = fs.readdirSync(basedir);

if (files.length > 0) {
    async.eachSeries(files, populateData, function (err) {
        if (err) throw err;

        setTimeout(function () {
            isbusy = false;
        }, 2500);
    })
}

function populateData(file, callback) {
    var date = new Date();
    var filePath = path.join(basedir, file);
    var filePathRead = path.join(basedir, file);
    var name = file.substr(0, file.length - 9);
    var keys = [];
    var seq = 0;

    sqlutil.getkeys({
        name: name,
        sqlcon: config.sql.simdms,
    }, function (result) {
        keys = result;

        var data = JSON.parse(fs.readFileSync(filePathRead));

        var page = Math.ceil(data.length / 100.0);
        for (var i = 0; i < page; i++) {
        }

        var data_i = [];
        for (var i = 0; i < 100; i++) {
            data_i.push(data[i + 100]);
        }

        async.eachSeries(data_i, populateRow, function (err) {
            if (err) throw err;
            callback();
            console.log("done");
        });

        async.eachSeries(data, populateRow, function (err) {
            if (err) throw err;
            callback();
            console.log("done");
        });

        //populateRow(data[600], function () { console.log('done') });

        //var sql = populateSql(data[100]);
        //console.log(sql);
    });

    function populateSql(row) {
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
        sql += pars.substr(1) + ")\n";

        return sql;
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

        //console.log(config.sql.simdms);

        sqlutil.query({
            sqlcon: config.sql.simdms,
            query: sql,
            params: row
        }, function (err) {
            if (err) {
                console.log(err);
                throw err;
            }

            console.log(sql, row, name, seq++);
            setTimeout(function () {
                callback();
            }, 10);
        });

        //console.log(sql, row, seq++);

        //setTimeout(function () {
        //    console.log(seq++);
        //    callback();
        //}, 10);
    }

}
