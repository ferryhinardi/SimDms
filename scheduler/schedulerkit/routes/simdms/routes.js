var express = require('express');
var moment = require('moment');
var path = require('path');
var fs = require('fs');
var config = require('../../config');
var sqlutil = require('../../lib/sqlutil');

exports.start = function () {
    var app = express();

    app.use(express.bodyParser({ keepExtensions: false, uploadDir: "temp" }));
    // app.use(express.urlencoded());
    // app.use(express.json());

    app.get("/", function (req, res) { res.send("home") });
    app.post("/upload", function (req, res) {
        var file = req.files.file;
        var body = req.body;
        var data = {
            UploadID: file.path.substr(5),
            DealerCode: body.DealerCode,
            UploadCode: "UPLCD",
            TableName: file.name.substr(0, (file.name.length - 14)),
            FileName: file.filename,
            FileSize: file.size,
            FileType: file.type,
            Status: "UPLOADED",
            UploadedDate: new Date()
        }
        res.send(data);

        if (data.UploadCode == "UPLCD" && data.DealerCode !== undefined) {
            var dir = path.join(__dirname, "../../upload");
            if (!fs.existsSync(dir)) {
                fs.mkdirSync(dir);
            }

            dir = path.join(dir, data.DealerCode);
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

            var source = fs.createReadStream(path.join(__dirname, "../../", file.path));
            var target = fs.createWriteStream(path.join(dir, file.filename));
            data.FilePath = data.DealerCode + "/" + moment().format("YYYY/MM/") + file.filename;

            source.pipe(target);
            source.on('end', function () {
                fs.unlinkSync(path.join(__dirname, "../../", file.path));

                saveData(data, function (err) {
                    if (err) throw err;

                    console.log(data.FileName + " - uploaded")
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