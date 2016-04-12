var request = require("request");
var fs = require("fs");
var r = request.post('http://tbsdmsap01:9091/upload');
var form = r.form();
form.append('file', fs.createReadStream('sample.wmv'));

