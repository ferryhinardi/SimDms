var express = require('express');

exports.start = function () {
    var app = express();
    app.get("/", function (req, res) { res.send("home") });
    app.listen(9092);
}
