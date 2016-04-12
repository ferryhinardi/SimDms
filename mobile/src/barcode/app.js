var path = require('path');
var http = require('http');
var express = require('express');
var app = new express();

// all environments
app.set('port', process.env.PORT || 8082);
app.use(express.static(path.join(__dirname, 'www')));

app.get('/', function (req, res) {
    fs.readFile(path.join(__dirname, 'www', 'index.html'), 'utf8', function (err, data) {
        if (err) { throw err };
        res.send(data);
    })
});

http.createServer(app).listen(app.get('port'), function () {
    console.log('Express server listening on port ' + app.get('port'));
});
