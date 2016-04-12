var moment = require('moment');
var async = require('async');
var path = require('path');
var sql = require('mssql');
var fs = require('fs');

var config = require('../config');

module.exports = {
    query: function (options, callback) {
        var conn = new sql.Connection(options.sqlcon, function (err) {
            if (err) throw err;

            var req = new sql.Request(conn);
            for (var key in (options.params || {})) {
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
