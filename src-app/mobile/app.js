var path = require('path');
var http = require('http');
var express = require('express');
var app = new express();

// all environments
app.set('port', process.env.PORT || 9092);
app.use(express.static(path.join(__dirname, 'client')));
app.use(express.urlencoded());
app.use(express.methodOverride());

var routes = require('./routes');

app.get('/', routes.map('index'));

routes.start(app, http);

