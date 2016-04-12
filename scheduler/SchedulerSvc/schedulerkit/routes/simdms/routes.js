var express = require('express');
var moment = require('moment');
var path = require('path');
var fs = require('fs');
var config = require('../../config');
var req = require('request');
var sqlutil = require('../../lib/sqlutil');
var sql = require('mssql'); 

exports.start = function () {

    var app = express();
	var allowTrigger = true;
	
	var server = require('http').createServer(app);
	var io = require('socket.io')(server);	
	var port = process.env.PORT || 9091;
	
	server.listen(port, function () {
		console.log('Server listening at port %d', port);
	});	
	
	//CORS middleware
	var allowCrossDomain = function(req, res, next) {
		res.header('Access-Control-Allow-Origin', '*');
		res.header('Access-Control-Allow-Methods', 'GET,PUT,POST,DELETE');
		res.header('Access-Control-Allow-Headers', 'Content-Type');
		next();
	}

	//...
	app.configure(function() {
		app.use(express.bodyParser({ keepExtensions: false, uploadDir: "temp" }));
		app.use(express.cookieParser());
		app.use(express.session({ secret: 'cool beans' }));
		app.use(express.methodOverride());
		app.use(allowCrossDomain);
		app.use(app.router);
		app.use(express.static(__dirname + '/public'));
	});
	
	/* socket io */
	var usernames = {};
	var sockets = {};
	var numUsers = 0;
	var loggers = {};
	
	var socketClient = require('socket.io-client')('http://localhost:' + port);
	
	socketClient.on('connect', function(){	
	
		socketClient.emit('add user','SERVER', 'SERVER', 'SERVER', false, '1.0');		
		
		socketClient.on('login', function(info){
			//console.log('login', info);
		});		
		
		socketClient.on('user joined', function(info){
			//console.log('user joined', info);
		});	
		
		socketClient.on('user left', function(info){
			//console.log('user left', info);
		});	
		
		socketClient.on('pm', function(from, command){
			//console.log('pm: ', from,' > ', command);			
			socketClient.emit('reply',from,command + ' result');
		});	

		socketClient.on('ping', function(info){
			console.log('ping: ', info);
		});	

		socketClient.on('log', function(info){
			//console.log('SERVER log: ' + info);
		});
		
		socketClient.on('command', function(from, command){
			if (command.type=='sql') {				
				sqlutil.query({
					sqlcon: config.serverdb(),
					query: command.command,
					params: ''
				}, function (err, result) {
					if (err) console.log(err);
					socketClient.emit('reply',from, result, err, 'Query for SIMDMS has been executed on ' + moment().format("DD-MM-YYYY HH:mm:ss") );
					socketClient.emit('change status', { DealerCode: 'SERVER', InfoStatus: 'COMMAND SQL' } );					
				})
			} else {
				socketClient.emit('reply',from, null, null, 'Command type is not recognized!' );
			}
		});	
		
		
	});

	socketClient.on('disconnect', function(){
		console.log('disconnect from server');
	});

	io.on('connection', function(socket) {
		var addedUser = false;
		
		socket.on('update', function(data) {
			socket.broadcast.emit('update', {
				username: socket.username,
				info: data
			});
		});
		
		socket.on('ping', function(data) {
			socket.broadcast.emit('ping', {
				username: socket.username,
				info: data
			});
		});
		
		socket.on('pm', function (params){
			var target = sockets[params.to];
			if (target) target.emit('command', socket.username, params);
		});
		
		socket.on('reply', function (dest, data, err, info)
		{
			var target = sockets[dest];
			if (target) target.emit('result', socket.username, data, err, info);
		});

		socket.on('change status', function(newStatus) {
			sqlutil.exec({
				sqlcon: config.serverdb(),
				query: "uspfn_SysDealerStatus",
				params: newStatus
			}, function (err, result) {
				if (err) console.log(err);
			})
		});
		
		socket.on('logger', function(newStatus) {
			//console.log('Logger: ', newStatus);
			for (var key in loggers) {
			   loggers[key].emit('log', newStatus);
			}
		});
		
		// when the client emits 'add user', this listens and executes
		socket.on('add user', function(username, CompanyCode, CompanyName, UseLogger, VERSION, IpAddress) {
		
			// we store the username in the socket session for this client
			//console.log(username);
			socket.username = username;
			
			// add the client's username to the global list
			usernames[username] = username;
			sockets[username] = socket;
			
			if (UseLogger)
			{
				loggers[username] = socket;
			}
			++numUsers;
			
			//console.log('id: ', socket.id);
			
			//console.log('address: ', socket.request);
			
			var UserLogin = {
				DealerCode: username,
				CompanyCode: CompanyCode,
				CompanyName: CompanyName,
				SessionId: socket.id,
				Location: IpAddress || '0.0.0.0', // + ':' + socket.request.connection._peername.port,
				Version: VERSION
			};
			
			sqlutil.exec({
				sqlcon: config.serverdb(),
				query: "uspfn_SysDealerLogin",
				params: UserLogin
			}, function (err, result) {
				if (err) console.log(err);
			})
		
			addedUser = true;
			socket.emit('login', {
				numUsers: numUsers
			});
			
			// echo globally (all clients) that a person has connected
			socket.broadcast.emit('user joined', {
				username: socket.username,
				numUsers: numUsers
			});
			
		});

		socket.on('log', function(info) {
			console.log(socket.username,' : ',info);
		});
		
		socket.on('disconnect', function() {
			if (addedUser) {
				var userid = socket.username;
				delete usernames[userid];
				delete loggers[userid];
				delete sockets[userid];
				--numUsers;
				var Param = {
					DealerCode: userid
				};
				
				sqlutil.exec({
					sqlcon: config.serverdb(),
					query: "uspfn_SysDealerLogout",
					params: Param
				}, function (err, result) {
					if (err) console.log(err);
				})
			
				socket.broadcast.emit('user left', {
					username: userid,
					numUsers: numUsers
				});
			}
		});
	});

    app.get("/", function (req, res) { res.send("home") });
	
	app.post('/SchedulerLog/Add', function(req, res) {
		console.log('Server loggin invoked');
		var params = req.body;
		console.log(params);
		
		var isError = 0;
		
		if(params.IsError == true) {
			isError = 1;
		}	
		
		var ip = req.header('x-forwarded-for') || req.connection.remoteAddress;
		var createdDate = new Date();
        var connection = new sql.Connection(config.serverdb(), function (err) {
			var request = new sql.Request(connection);
			request.input('DealerCode', sql.VarChar , params.DealerCode);
			request.input('ScheduleName', sql.VarChar , params.ScheduleName);
			request.input('DateStart', sql.VarChar , params.DateStart);
			request.input('DateFinish', sql.VarChar , params.DateFinish);
			request.input('CreatedBy', sql.VarChar , ip);
			request.input('RunningTimes', sql.VarChar , params.RunningTimes);
			request.input('IsError', sql.Bit , isError);
			request.input('ErrorMessage', sql.VarChar , params.ErrorMessage);
			request.input('Info', sql.VarChar , params.Info);
			request.execute('uspfn_SchedulerSave', function(err, recordsets, returnValue) {
				var result = {
					status: true,
					info: 'Log has been saved'
				};
				if(err) {
					result.status = false;
					result.info = 'Log cannot be saved.';
				}
				
				console.log('Sending result');
				res.send(JSON.stringify(result));
			});
        });		
	});
	
    app.post("/upload", function (req, res) {
        var file = req.files.file;
        var body = req.body;
        var data = {
            UploadID: file.path.substr(5),
            DealerCode: body.DealerCode,
            UploadCode: "UPLCD",
            TableName: file.name.split("_")[0],
            FileName: file.filename,
            FileSize: file.size,
            FileType: file.type,
            Status: "UPLOADED",
            UploadedDate: new Date()
        }
        res.send(data);

        if (data.UploadCode == "UPLCD" && data.DealerCode !== undefined) {
		
            var dir = path.join(__dirname, "../../upload");
			var filePath = "";
			
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir);
            }

			if (data.TableName != data.DealerCode){
				filePath += data.DealerCode + "/";
				dir = path.join(dir, data.DealerCode);
				if (!fs.existsSync(dir)) {
					fs.mkdirSync(dir);
				}
			}

            dir = path.join(dir, moment().format("YYYY"));
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir);
            }

            dir = path.join(dir, moment().format("MM"));
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir);
            }
			
			if (data.TableName == data.DealerCode){
				filePath += moment().format("YYYY/MM/") + "R" + data.DealerCode.substr(4,1) + "/" + file.filename;
				dir = path.join(dir, "R" + data.DealerCode.substr(4,1) );
				data.FileType += "db";
				if (!fs.existsSync(dir)) {
					fs.mkdirSync(dir);
				}			
			} else {
				filePath += moment().format("YYYY/MM/") + file.filename;
			}
			
            var source = fs.createReadStream(path.join(__dirname, "../../", file.path));
            var targetFileLocation = path.join(dir, file.filename);
            
            /*if (fs.existsSync(targetFileLocation)) {
				log('file exist: ' + targetFileLocation);
                fs.unlink(targetFileLocation, function (err) {
                    if (err) {
                        console.log(err);
                    }
					log('file removed: ' + targetFileLocation);
                });
            }*/
            
            var target = fs.createWriteStream(targetFileLocation);
            data.FilePath = filePath; // data.DealerCode + "/" + moment().format("YYYY/MM/") + file.filename;

            source.pipe(target);
            source.on('end', function () {
                fs.unlinkSync(path.join(__dirname, "../../", file.path));
				log('file saved: ' + targetFileLocation);
                saveData(data, function (err) {
                    if (err) throw err;
                    console.log(data.FileName + " - uploaded")
					//Trigger mergerDB to run merge processing
					if (data.TableName == data.DealerCode && allowTrigger ){
						allowTrigger = false;
						setTimeout(function()
						{
							var request = require('request');
							var paramsX = {
								uri: 'http://tbsdmsap01:9009/api/sdms/process',
								method: "GET"
							};	
							request(paramsX, function (e, r, bodyX) {
								console.log(bodyX);
								allowTrigger = true;
							}); } , 
						10000);
					}
                });
            });
            source.on('error', function (err) {
                if (err) throw err;
            });
        }
    });
	
    app.post("/simdata/", function (req, res) {
        sqlutil.exec({
            sqlcon: config.serverdb(),
            query: "uspfn_SysDealerLastDate",
            params: req.body
        }, function (err, result) {
            if (err) throw err;

            var lastDate = moment(result[0][0]["LastUpdate"]).format("YYYY-MM-DD HH:mm:ss");
            console.log(lastDate + " (" + req.body.DealerCode + " - " + req.body.TableName + ")");

            res.send(lastDate);
        })
    });
	
	app.post("/api/tablelist", function (req, res) {
		
        sqlutil.exec({
            sqlcon: config.serverdb(),
            query: "uspfn_SysDealerTables",
            params: req.body
        }, function (err, result) {
            if (err) throw err;			
			if (result)
			{
				res.send( result);
			} else {
				res.send("[[]]");
			}
            
        })
    });
	
	app.post("/api/synctablecs", function (req, res) {		
        sqlutil.exec({
            sqlcon: config.serverdb(),
            query: "uspfn_SyncTableDealerCS",
            params: req.body
        }, function (err, result) {
            if (err) throw err;			
			if (result)
			{
				res.send( result);
			} else {
				res.send("[[]]");
			}
            
        })
    });
	
	app.post("/api/synctablelogger", function (req, res) {		
        sqlutil.exec({
            sqlcon: config.serverdb(),
            query: "uspfn_SyncTableDealerLogger",
            params: req.body
        }, function (err, result) {
            if (err) throw err;			
			if (result)
			{
				res.send( result);
			} else {
				res.send("[[]]");
			}
            
        })
    });
	
	app.post("/api/jobs/check", function (req, res) {
		log('Task Checking: ' + JSON.stringify(req.body));
        sqlutil.exec({
            sqlcon: config.serverdb(),
            query: "uspfn_SysDealerTaskCheck",
            params: req.body
        }, function (err, result) {
            if (err) throw err;			
			if (result)
			{
				res.send(result);
			} else {
				res.send("");
			}            
        })
    });
	
	app.post("/api/dealeronline", function (req, res) {
        sqlutil.exec({
            sqlcon: config.serverdb(),
            query: "uspfn_SysDealerOnline",
            params: null
        }, function (err, result) {
            if (err) throw err;			
			if (result)
			{
				res.send({Result: result[0]});
			} else {
				res.send("");
			}            
        })
    });
	
	app.post("/api/jobs/logger", function (req, res) {
        sqlutil.exec({
            sqlcon: config.serverdb(),
            query: "uspfn_SysDealerTaskLogger",
            params: req.body
        }, function (err, result) {
            if (err) throw err;			
			if (result)
			{
				res.send( result);
			} else {
				res.send("");
			}
        })
    });
	/*
	app.get('/:file(*)', function(req, res, next){
	  var file = req.params.file
		, path = __dirname + '/tasks/' + file;
	  log('user request: ' + path);
	  res.download(path);
	});

	app.use(function(err, req, res, next){
	  if (404 == err.status) {
		res.statusCode = 404;
		res.send('Cant find that file, sorry!');
	  } else {
		next(err);
	  }
	});
	//*/
	
	app.post("/api/adddealertable", function (req, res) {
        sqlutil.exec({
            sqlcon: config.serverdb(),
            query: "uspfn_SysDealerInsertTableList",
            params: req.body
        }, function (err, result) {
            if (err) throw err;			
			if (result)
			{
				res.send("OK");
			} else {
				res.send("");
			}            
        })
    });
	
	app.post("/uploadtasklog", function (req, res) {
        var file = req.files.file;
        var body = req.body;
        var data = {
            UploadID: file.path.substr(5),
            DealerCode: body.DealerCode,
            UploadCode: "UPLCD",
            FileName: file.filename,
            FileSize: file.size,
            FileType: file.type,
            Status: "UPLOADED",
            UploadedDate: new Date()
        }
        res.send(data);

        if (data.UploadCode == "UPLCD" && data.DealerCode !== undefined) {
		
            var dir = path.join(__dirname, "../../log/TaskLogs");
			
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir);
            }
			
			dir = path.join(dir, data.DealerCode);			
			if (!fs.existsSync(dir)) {
				fs.mkdirSync(dir);
			}	
			
            var source = fs.createReadStream(path.join(__dirname, "../../", file.path));
            var targetFileLocation = path.join(dir, file.filename);                     
            var target = fs.createWriteStream(targetFileLocation);

            source.pipe(target);

            source.on('end', function () {
                fs.unlinkSync(path.join(__dirname, "../../", file.path));
								
				log('file saved: ' + targetFileLocation);
					setTimeout(function(){
					
						var ret = fs.readFileSync(targetFileLocation);
						var rawdata = ret.toString();
						console.log(rawdata);
						
						if (rawdata.indexOf("ERROR") > -1 )
						{					
							var request = require('request');
							var paramsX = {
								uri: 'http://tbsdmsap01:9009/api/sdms/senderror',
								form: {
									subject: '[Error Notification] ' + data.DealerCode + ' - ' + data.FileName,
									message: "Error found on " + targetFileLocation.replaceAll('\\','/'),
									filename: targetFileLocation.replaceAll('\\','/')
								},
								method: "POST"
							};	
							console.log(paramsX);
							request(paramsX, function (e, r, bodyX) {
								console.log(bodyX);						
							});
						}
					
					
					},1000);				
            });			
            source.on('error', function (err) {
                if (err) log(err);
            });
        }
    });
	
	app.post("/api/command", function (req, res) {
        var param = req.body;
		socketClient.emit('pm',param);		
		socketClient.on('result', function(from, result, err, info){
			res.send({info: info, data: result, error: err});
		});	
	});
	
	
	
}

function saveData(data, callback) {
    sqlutil.query({
        sqlcon: config.serverdb(),
        query: "insert into SysDealerHist " +
               "(UploadID,DealerCode,UploadCode,TableName,FileName,FilePath,FileSize,FileType,Status,UploadedDate)values" +
               "(@UploadID,@DealerCode,@UploadCode,@TableName,@FileName,@FilePath,@FileSize,@FileType,@Status,@UploadedDate)",
        params: data
    }, function (err) {
        if (err) throw err;

        callback();
    });
}

String.prototype.replaceAll = function (find, replace) {
    var str = this;
    return str.replace(new RegExp(find.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&'), 'g'), replace);
};

function log(info, line) {
    var dir = path.join(__dirname, '../../log');
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

    var file = path.join(dir, "/SERVICE." + moment().format("YYYY_MMDD") + ".log");
    fs.appendFileSync(file, moment().format("YYYY-MM-DD HH:mm:ss") + " : " + info + ((line == undefined) ? '' : ' - ' + line.toString()) + "\n");
    console.log(moment().format("YYYY-MM-DD HH:mm:ss"), ":", info, (line || ''));
	
	
}