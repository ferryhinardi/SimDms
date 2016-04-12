var express = require('express');
var request = require('request');
var config = require('../../config');

exports.start = function () {
    var app = express();
    app.get("/", function (req, res) { res.send("home") });
    app.listen(9009);
}
