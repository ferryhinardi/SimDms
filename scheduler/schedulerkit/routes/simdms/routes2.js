var express = require('express');
var moment = require('moment');
var path = require('path');
var fs = require('fs');
var config = require('../../config');
var sqlutil = require('../../lib/sqlutil');
var sql = require('mssql'); 
var req = require('request');

exports.start = function () {
    var app = express();
	var allowTrigger = true;

    app.use(express.bodyParser({ keepExtensions: false, uploadDir: "temp" }));
    // app.use(express.urlencoded());
    // app.use(express.json());

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
        var connection = new sql.Connection(config.sql.simdms, function (err) {
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
            var target = fs.createWriteStream(path.join(dir, file.filename));
            data.FilePath = filePath; // data.DealerCode + "/" + moment().format("YYYY/MM/") + file.filename;

            source.pipe(target);
            source.on('end', function () {
                fs.unlinkSync(path.join(__dirname, "../../", file.path));

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
								uri: 'http://localhost:9009/api/sdms/process',
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
            sqlcon: config.sql.simdms,
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
            sqlcon: config.sql.simdms,
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
	
    app.listen(9091);
}

function saveData(data, callback) {
    sqlutil.query({
        sqlcon: config.sql.simdms,
        query: "insert into SysDealerHist " +
               "(UploadID,DealerCode,UploadCode,TableName,FileName,FilePath,FileSize,FileType,Status,UploadedDate)values" +
               "(@UploadID,@DealerCode,@UploadCode,@TableName,@FileName,@FilePath,@FileSize,@FileType,@Status,@UploadedDate)",
        params: data
    }, function (err) {
        if (err) throw err;

        callback();
    });
}