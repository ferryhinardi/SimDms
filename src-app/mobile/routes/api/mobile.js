var sqlutil = require("../../libs/sqlutil");
var config = require("../../config");

module.exports = {
    accounts: function (req, res) {
        sqlutil.exec({
            sqlcon: config.sql.mobile,
            query: "msp_account_list",
        }, function (err, docs) {
            if (err) throw err;

            res.header("Access-Control-Allow-Origin", "*");
            res.header("Access-Control-Allow-Headers", "X-Requested-With");
            res.send(docs[0]);
        });
    },
    stores: function (req, res) {
        sqlutil.exec({
            sqlcon: config.sql.mobile,
            query: "msp_store_list",
            params: req.body,
        }, function (err, docs) {
            if (err) throw err;

            res.header("Access-Control-Allow-Origin", "*");
            res.header("Access-Control-Allow-Headers", "X-Requested-With");
            res.send(docs[0]);
        });
    },
    actions: function (req, res) {
        sqlutil.exec({
            sqlcon: config.sql.mobile,
            query: "msp_activity_list",
        }, function (err, docs) {
            if (err) throw err;

            res.header("Access-Control-Allow-Origin", "*");
            res.header("Access-Control-Allow-Headers", "X-Requested-With");
            res.send(docs[0]);
        });
    },
    trainings: function (req, res) {
        sqlutil.exec({
            sqlcon: config.sql.mobile,
            query: "msp_training_list",
        }, function (err, docs) {
            if (err) throw err;

            res.header("Access-Control-Allow-Origin", "*");
            res.header("Access-Control-Allow-Headers", "X-Requested-With");
            res.send(docs[0]);
        });
    },
    visibilities: function (req, res) {
        sqlutil.exec({
            sqlcon: config.sql.mobile,
            query: "msp_visibility_list",
        }, function (err, docs) {
            if (err) throw err;

            res.header("Access-Control-Allow-Origin", "*");
            res.header("Access-Control-Allow-Headers", "X-Requested-With");
            res.send(docs[0]);
        });
    },
    trainings: function (req, res) {
        sqlutil.exec({
            sqlcon: config.sql.mobile,
            query: "msp_training_list",
        }, function (err, docs) {
            if (err) throw err;

            res.header("Access-Control-Allow-Origin", "*");
            res.header("Access-Control-Allow-Headers", "X-Requested-With");
            res.send(docs[0]);
        });
    },
    mtrainings: function (req, res) {
        sqlutil.exec({
            sqlcon: config.sql.mobile,
            query: "msp_mtraining_list",
        }, function (err, docs) {
            if (err) throw err;

            res.header("Access-Control-Allow-Origin", "*");
            res.header("Access-Control-Allow-Headers", "X-Requested-With");
            res.send(docs[0]);
        });
    },
}